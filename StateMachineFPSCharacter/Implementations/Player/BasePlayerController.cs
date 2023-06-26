using System;
using System.Collections.Generic;
using Game.Character.Implementations.Player.States;
using Game.FPSObject;
using Game.FPSObject.Implementations;
using Game.Input;
using Game.StateMachine.Player;
using OtherTools._3D.TransformAnimatorSystem;
using UnityEngine;

namespace Game.Character.Implementations.Player
{
    public sealed class BasePlayerController : MonoBehaviour, IStateSwitcher
    {
        [Header("Player Movement")]
        [SerializeField] private BasePlayerCharacter _TargetPlayerCharacter;
        [SerializeField] private FPSObjectsController _TargetPlayerFPSObjectsController;
        [SerializeField] private float _SpeedMove = 1.5f;
        [SerializeField] private float _SprintSpeedMultiplier = 3.5f;
        [SerializeField] private float _CrouchSpeedMultiplier = 0.75f;
        [SerializeField] private float _JumpLength = 10f;
        [SerializeField] private Vector3 _GravityValue = Vector3.down;

        [Header("Player Camera")]
        [SerializeField] private TransformAnimatorsController _AnimatorsController;

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
        public event Action<IBaseState, IBaseState> StateSwitched;
        public event Action<IBaseState> GroundedFromState;
        public IBaseState CurrentState {get; set; }
        public List<IBaseState> States {get; set; }
        
        public void SwitchState<T>() where T : IBaseState
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
        private void SwitchState(IBaseState toState)
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
        }

        private void SetStates()
        {
            States = new List<IBaseState>()
            {
                new BasePlayerIdleState      (_AnimatorsController, this),
                new BasePlayerWalkState      (_AnimatorsController, this),
                new BasePlayerRunState       (_AnimatorsController, this),
                new BasePlayerJumpInState    (_AnimatorsController, this),
                new BasePlayerJumpOutState   (),
                new BasePlayerJumpRunInState (_AnimatorsController, this),
                new BasePlayerJumpRunOutState(),
                new BasePlayerCrouchIdleState(_AnimatorsController, this),
                new BasePlayerCrouchMoveState(_AnimatorsController, this),
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
                || CurrentState.FPSObjectShaker == FPSObjectShakerType.CrouchIdle) 
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
            var isIdle = CurrentState.FPSObjectShaker == FPSObjectShakerType.Idle;
            IsMove = movementValue != Vector2.zero;
            var xSprint = Mathf.Abs(movementValue.x) < PLAYER_MOVEMENT_JOYSTICK_SPRINT_RESET_VALUE;
            var stickSprint = movementValue.y > 0 && _playerMovementInput.StickValue >= PLAYER_MOVEMENT_JOYSTICK_SPRINT_VALUE;
            IsSprint = xSprint && stickSprint;

            if (IsMove && isIdle && isGroundedIdle)
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
            
        }
        private void OnTakenUpFPSObject(IFPSObject obj)
        {
            
        }
        private void OnGroundedFromState()
        {
            if(CurrentState.FPSObjectShaker == FPSObjectShakerType.OnGroundedRun)
                _AnimatorsController.Play(CAMERA_ON_GROUNDED_RUN_SHAKER_NAME, CAMERA_ON_GROUNDED_RUN_SHAKER_FADE_IN_DURATION);
            else if(CurrentState.FPSObjectShaker == FPSObjectShakerType.OnGrounded)
                _AnimatorsController.Play(CAMERA_ON_GROUNDED_SHAKER_NAME, CAMERA_ON_GROUNDED_SHAKER_FADE_IN_DURATION);
            
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
            _TargetPlayerCharacter.Grounded                 += OnGrounded;
            _TargetPlayerFPSObjectsController.TakenUpObject += OnTakenUpFPSObject;
            _TargetPlayerFPSObjectsController.PutAwayObject += OnPutAwayFPSObject;
            InputManager.SettedPlayerMovementInput          += OnSettedPlayerMovementInput;
            InputManager.SettedPlayerCameraRotateInput      += OnSettedPlayerCameraRotateInput;
            InputManager.SettedPlayerJumpInput              += OnSettedPlayerJumpInput;
            InputManager.SettedPlayerCrouchInput            += OnSettedPlayerCrouchInput;

            var addTransformOnGrounded = new AddTransform() 
            {
                UseParameters = UseTransformParameters.Everything,
                Transform = _AnimatorsController.GetShaker(CAMERA_ON_GROUNDED_SHAKER_NAME).Target
            };
            var addTransformOnGroundedRun = new AddTransform()
            {
                UseParameters = UseTransformParameters.Everything,
                Transform = _AnimatorsController.GetShaker(CAMERA_ON_GROUNDED_RUN_SHAKER_NAME).Target
            };
            
            _TargetPlayerCharacter.AddTransforms.Add(addTransformOnGrounded);
            _TargetPlayerCharacter.AddTransforms.Add(addTransformOnGroundedRun);
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
            _TargetPlayerCharacter.Grounded                 -= OnGrounded;
            _TargetPlayerFPSObjectsController.TakenUpObject -= OnTakenUpFPSObject;
            _TargetPlayerFPSObjectsController.PutAwayObject -= OnPutAwayFPSObject;
            InputManager.SettedPlayerMovementInput          -= OnSettedPlayerMovementInput;
            InputManager.SettedPlayerCameraRotateInput      -= OnSettedPlayerCameraRotateInput;
            InputManager.SettedPlayerJumpInput              -= OnSettedPlayerJumpInput;
            InputManager.SettedPlayerCrouchInput            -= OnSettedPlayerCrouchInput;
        }
        #endregion
    }
}