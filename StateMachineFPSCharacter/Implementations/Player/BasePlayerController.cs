using System;
using System.Collections.Generic;
using Game.Character.Implementations.Player.States;
using Game.Item;
using Game.Input;
using Game.PlayerStateMachine;
using OtherTools._3D.TransformAnimatorSystem;
using UnityEngine;

namespace Game.Character.Implementations.Player
{
    public sealed class BasePlayerController : MonoBehaviour, IStateSwitcher
    {
        [Header("Player Movement")]
        [SerializeField] private BasePlayerCharacter _TargetPlayerCharacter;
        [SerializeField] private float _SpeedMove = 1.5f;
        [SerializeField] private float _SprintSpeedMultiplier = 3.5f;
        [SerializeField] private float _CrouchSpeedMultiplier = 0.75f;
        [SerializeField] private float _JumpLength = 10f;
        [SerializeField] private Vector3 _GravityValue = Vector3.down;

        [field: SerializeField, Header("Player Camera")] 
        public TransformAnimatorsController AnimatorsController { get; private set; }

#if UNITY_DEBUG
        [Space]
        [SerializeField] private bool _Debug;
#endif
        
        #region Static
        public static bool CrouchMovement => IsCrouch && IsMove;
        public static bool IsCrouch { get; private set; }
        public static bool IsSprint { get; private set; }
        public static bool IsMove { get; private set; }
        #endregion
        
        #region StateMachine
        public event Action<IState, IState> StateSwitched;
        public event Action<IState> GroundedFromState;
        public IState CurrentState {get; set; }
        public List<IState> States {get; set; }
        
        public void SwitchState<T>() where T : IState
        {
            foreach (var item in States)
            {
                if(item is not T) continue;
                SwitchState(item);
                break;
            }
#if UNITY_DEBUG
            if(_Debug)
                Debug.LogError($"State <<color=yellow>{typeof(T).Name}</color>> don't exist in states.");
#endif     
        }
        public void SwitchState(Type stateType)
        {
            foreach (var item in States)
            {
                if(item.GetType() != stateType) continue;
                SwitchState(item);
                break;
            }
#if UNITY_DEBUG
            if (_Debug)
            {
                var toStateName = stateType == null ? "null" : stateType.Name;
                Debug.LogError($"State <<color=yellow>{toStateName}</color>> don't exist in states.");
            }
#endif
        }
        private void SwitchState(IState toState)
        {
            if (toState == null) return;
#if UNITY_DEBUG
            if (_Debug)
            {
                var fromStateName = CurrentState == null ? "null" : CurrentState.GetType().Name;
                Debug.Log($"Switch state <color=yellow>{fromStateName}</color> -> <color=yellow>{toState.GetType().Name}</color>.");
            }
#endif
            var fromState = CurrentState;
            CurrentState?.Exit(toState);
            CurrentState = toState;
            toState.Enter(fromState);
            StateSwitched?.Invoke(fromState, toState);
            PlayerEvents.StateSwitched.Invoke(fromState, toState);
        }

        private void SetStates()
        {
            States = new List<IState>()
            {
                new BasePlayerIdleState      (this),
                new BasePlayerWalkState      (this),
                new BasePlayerRunState       (this),
                new BasePlayerJumpInState    (this),
                new BasePlayerJumpOutState   (this),
                new BasePlayerJumpRunInState (this),
                new BasePlayerJumpRunOutState(this),
                new BasePlayerCrouchIdleState(this),
                new BasePlayerCrouchMoveState(this),
            };
        }
        #endregion

        #region Controller
        private const float PLAYER_MOVEMENT_JOYSTICK_SPRINT_VALUE = 1f; //max value = joystick max Y value = 1
        private const float PLAYER_MOVEMENT_JOYSTICK_SPRINT_RESET_VALUE = 0.5f; //used for joystick X value
        
        private const string CAMERA_ON_GROUNDED_SHAKER_NAME = "OnGroundedCamera";
        private const float CAMERA_ON_GROUNDED_SHAKER_FADE_IN_DURATION = 0.025f;
        private const string CAMERA_ON_GROUNDED_RUN_SHAKER_NAME = "OnGroundedRunCamera";
        private const float CAMERA_ON_GROUNDED_RUN_SHAKER_FADE_IN_DURATION = 0.025f;
        
        public BasePlayerCharacter TargetPlayerCharacter => _TargetPlayerCharacter;
        public IJoystickInput PlayerMovementInput => _playerMovementInput;
        
        private IJoystickInput _playerMovementInput;
        private IInput<Vector2> _playerCameraRotateInput;
        private IInput<bool> _playerCrouchInput;
        private Vector2 _movementDirection;
        private Vector2 _cameraRotateVelocity;
        private bool _startMove;
        private bool _stopMove;
        
        private void UpdateCrouch()
        {
            if (_playerCrouchInput == null) return;
            IsCrouch = _playerCrouchInput.Value;
            if (IsMove 
                || IsCrouch == false 
                || _TargetPlayerCharacter.IsGrounded == false
                || CurrentState.FPSObjectAnimation == FPSObjectAnimationType.CrouchIdle) 
                return;
            
            SwitchState<BasePlayerCrouchIdleState>();
        }
        private void UpdateCamera()
        {
            if(_playerCameraRotateInput == null) return;
            var velocity = _playerCameraRotateInput.Value;
            _TargetPlayerCharacter.RotateCamera(velocity);
            _playerCameraRotateInput.Value = Vector2.zero;
        }
        private void UpdateMovement()
        {
            if (_playerMovementInput == null) return;
            var movementValue = _playerMovementInput.Value;
            var isGroundedIdle = _TargetPlayerCharacter.IsCrouch == false && _TargetPlayerCharacter.IsGrounded;
            var isShakerIdle = CurrentState.FPSObjectAnimation == FPSObjectAnimationType.Idle;
            IsMove = movementValue != Vector2.zero;
            var xSprint = Mathf.Abs(movementValue.x) < PLAYER_MOVEMENT_JOYSTICK_SPRINT_RESET_VALUE;
            var stickSprint = movementValue.y > 0 && _playerMovementInput.StickValue >= PLAYER_MOVEMENT_JOYSTICK_SPRINT_VALUE;
            IsSprint = xSprint && stickSprint;

            if (IsMove && isShakerIdle && isGroundedIdle)
            {
                if (_TargetPlayerCharacter.IsGrounded)
                {
                    _startMove = true;
                    if (IsSprint) SwitchState<BasePlayerRunState>();
                    else SwitchState<BasePlayerWalkState>();
                }
            }

            _stopMove = _playerMovementInput.Value == Vector2.zero;
            
            if (_stopMove && _startMove && !IsCrouch)
            {
                SwitchState<BasePlayerIdleState>();
                _startMove = false;
            }
            
            var speedMultiplier = 1f;
            var isCrouch = _TargetPlayerCharacter.IsCrouch;
            if (IsSprint) speedMultiplier = isCrouch ? _CrouchSpeedMultiplier : _SprintSpeedMultiplier;
            else if (isCrouch) speedMultiplier = _CrouchSpeedMultiplier;

            _movementDirection = _playerMovementInput.Value;
            _TargetPlayerCharacter.Move(_movementDirection * _SpeedMove * speedMultiplier);
        }
        private void OnJumpInput()
        {
            if (_TargetPlayerCharacter.IsCrouch || _TargetPlayerCharacter.IsGrounded == false) 
                return;
            
            _startMove = false;
            _TargetPlayerCharacter.Jump(_JumpLength);
            if(IsSprint) SwitchState<BasePlayerJumpRunInState>();
            else SwitchState<BasePlayerJumpInState>();
        }
        private void OnGrounded()
        {
            OnGroundedFromState();

            if (IsMove)
            {
                _startMove = true;
                if (IsCrouch) SwitchState<BasePlayerCrouchMoveState>();
                else if(IsSprint) SwitchState<BasePlayerRunState>();
                else SwitchState<BasePlayerWalkState>();
            }
            else
            {
                SwitchState<BasePlayerIdleState>();
            }
        }
        private void OnPutAwayFPSObject(IFPSObject obj)
        {
            // Code for fix animation bug 
            // in BaseFPSObject when Player Character
            // puted away BaseFPSObject not from Idle state
        }
        private void OnTakenUpFPSObject(IFPSObject obj)
        {
            // Code for fix animation bug 
            // in BaseFPSObject when Player Character
            // taken up BaseFPSObject not from Idle state
        }
        private void OnGroundedFromState()
        {
            if(CurrentState.FPSObjectAnimation == FPSObjectAnimationType.OnGroundedRun)
                AnimatorsController.Play(CAMERA_ON_GROUNDED_RUN_SHAKER_NAME, CAMERA_ON_GROUNDED_RUN_SHAKER_FADE_IN_DURATION);
            else if(CurrentState.FPSObjectAnimation == FPSObjectAnimationType.OnGrounded)
                AnimatorsController.Play(CAMERA_ON_GROUNDED_SHAKER_NAME, CAMERA_ON_GROUNDED_SHAKER_FADE_IN_DURATION);
            
            GroundedFromState?.Invoke(CurrentState);
        }
        private void OnSettedPlayerMovementInput(IInput<Vector2> input)
        {
            _playerMovementInput = (IJoystickInput)input;
        }
        private void OnSettedPlayerCameraRotateInput(IInput<Vector2> input)
        {
            _playerCameraRotateInput = input;
        }
        private void OnSettedPlayerJumpInput(IInput<Action> input)
        {
            input.Value += OnJumpInput;
        }
        private void OnSettedPlayerCrouchInput(IInput<bool> input)
        {
            _playerCrouchInput = input;
        }
        #endregion

        #region Monobehaviour
        private void Start()
        {
            SetStates();
            SwitchState<BasePlayerIdleState>();
            _TargetPlayerCharacter.Grounded            += OnGrounded;
            FPSObjectEvents.TakenUp.Event              += OnTakenUpFPSObject;
            FPSObjectEvents.PutedAway.Event            += OnPutAwayFPSObject;
            InputManager.SettedPlayerMovementInput     += OnSettedPlayerMovementInput;
            InputManager.SettedPlayerCameraRotateInput += OnSettedPlayerCameraRotateInput;
            InputManager.SettedPlayerJumpInput         += OnSettedPlayerJumpInput;
            InputManager.SettedPlayerCrouchInput       += OnSettedPlayerCrouchInput;

            var targetGround = AnimatorsController.GetAnimator(CAMERA_ON_GROUNDED_SHAKER_NAME).Target;
            var targetGroundRun = AnimatorsController.GetAnimator(CAMERA_ON_GROUNDED_RUN_SHAKER_NAME).Target;
            var addTransformOnGrounded = new AddOffset
            (
                CAMERA_ON_GROUNDED_SHAKER_NAME,
                UseTransformParameters.Everything,
                () => targetGround.localPosition,
                () => targetGround.localRotation
            );
            var addTransformOnGroundedRun = new AddOffset
            (
                CAMERA_ON_GROUNDED_RUN_SHAKER_NAME,
                UseTransformParameters.Everything,
                () => targetGroundRun.localPosition,
                () => targetGroundRun.localRotation
            );
            
            _TargetPlayerCharacter.AddOffsets.Add(addTransformOnGrounded);
            _TargetPlayerCharacter.AddOffsets.Add(addTransformOnGroundedRun);
        }
        private void Update()
        {
            _TargetPlayerCharacter.GravityValue = _GravityValue;
            UpdateMovement();
            UpdateCamera();
            UpdateCrouch();
            CurrentState?.Update();
        }
        private void OnDestroy()
        {
            _TargetPlayerCharacter.Grounded            -= OnGrounded;
            FPSObjectEvents.TakenUp.Event              -= OnTakenUpFPSObject;
            FPSObjectEvents.PutedAway.Event            -= OnPutAwayFPSObject;
            InputManager.SettedPlayerMovementInput     -= OnSettedPlayerMovementInput;
            InputManager.SettedPlayerCameraRotateInput -= OnSettedPlayerCameraRotateInput;
            InputManager.SettedPlayerJumpInput         -= OnSettedPlayerJumpInput;
            InputManager.SettedPlayerCrouchInput       -= OnSettedPlayerCrouchInput;
        }
        #endregion
    }
}