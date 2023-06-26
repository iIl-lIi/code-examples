using Cysharp.Threading.Tasks;
using Game.Character.Implementations.Player;
using Game.Input;
using Game.StateMachine.Player;
using OtherTools._3D.TransformAnimatorSystem;
using UnityEngine;

namespace Game.FPSObject.Implementations
{
    public class BaseFPSObject : MonoBehaviour, IFPSObject
    {
        [Header("FPS Object")]
        [SerializeField] private FPSObjectIndex _Index;
        [SerializeField] private GameObject _MeshMeshRoot;
        [SerializeField] private TransformAnimatorsController _ShakeAnimatorsController;
        [SerializeField] private float _ShakeFadeDuration = 0.1f;
        
        public FPSObjectIndex Type { get => _Index; set => _Index = value; }
        public FPSObjectState State { get; set; }
        public GameObject Root { get; set; }
        public GameObject MeshRoot { get => _MeshMeshRoot; set => _MeshMeshRoot = value; }

        private TransformAnimator _currentControlShaker;
        private TransformAnimator _lastPlayingShaker;
        private IJoystickInput _playerMovementInput;
        private BasePlayerController _playerController;
        private bool _allowControlFactor;

        public virtual void Initialize()
        {
            State = FPSObjectState.Busy;
            Root = gameObject;
            MeshRoot.SetActive(false);
            _playerMovementInput = (IJoystickInput)InputManager.PlayerMovementInput;
            _playerController = (PlayerLoader.PlayerInstance as BasePlayerCharacter).PlayerController;
            SubscribeShake();
            State = FPSObjectState.PuttedAway;
        }
        public virtual async UniTask TakeUp()
        {
            if (State == FPSObjectState.Busy || MeshRoot == null) return;
            State = FPSObjectState.Busy;
            MeshRoot.SetActive(true);
            SetShakerFromPlayerState();
            State = FPSObjectState.Taken;
        }
        public virtual async UniTask PutAway()
        {
            if (State == FPSObjectState.Busy || MeshRoot == null) return;
            State = FPSObjectState.Busy;
            MeshRoot.SetActive(false);
            StopPlayingLastShaker();
            _allowControlFactor = false;
            State = FPSObjectState.PuttedAway;
        }
        
        protected void StopPlayingLastShaker() => _lastPlayingShaker.StopAnimation();
        protected void SetShakerFromPlayerState()
        {
            if(_lastPlayingShaker) _lastPlayingShaker.StopAnimation();
            if(_currentControlShaker) _currentControlShaker.StopAnimation();
            var currentPlayerShakerIndex = _playerController.CurrentState.FPSObjectShaker;
            _lastPlayingShaker = _ShakeAnimatorsController.GetShaker(currentPlayerShakerIndex.ToString());
            var isWalkOrCrouchMoveTo = currentPlayerShakerIndex is FPSObjectShakerType.Walk or FPSObjectShakerType.CrouchMove;

            if (isWalkOrCrouchMoveTo)
            {
                _currentControlShaker = _lastPlayingShaker;
                _allowControlFactor = true;
            }
            else _lastPlayingShaker.SetFactor(1, _ShakeFadeDuration);
            _lastPlayingShaker.StartAnimation();
        }
        
        private void SubscribeShake()
        {
            _playerController.StateSwitched += OnStateSwitched;
            _playerController.GroundedFromState += OnGroundedFromState;
        }
        private void UnsubscribeShake()
        {
            _playerController.StateSwitched -= OnStateSwitched;
            _playerController.GroundedFromState -= OnGroundedFromState;
        }
        private void OnStateSwitched(IBaseState from, IBaseState to)
        {
            var shakerFrom = from.FPSObjectShaker;
            var shakerTo = to.FPSObjectShaker;
            var onGroundedTo = shakerTo is FPSObjectShakerType.OnGrounded or FPSObjectShakerType.OnGroundedRun;

            if (onGroundedTo || State != FPSObjectState.Taken) return;

            var shakerFromAnimator = _ShakeAnimatorsController.GetShaker(shakerFrom.ToString());
            var onGroundedFrom = shakerFrom is FPSObjectShakerType.OnGrounded or FPSObjectShakerType.OnGroundedRun;
            var isWalkOrCrouchMoveTo = shakerTo is FPSObjectShakerType.Walk or FPSObjectShakerType.CrouchMove;
            
            if(shakerFromAnimator == null) return;
            if (onGroundedFrom == false) shakerFromAnimator.SetFactor(0, _ShakeFadeDuration, shakerFromAnimator.StopAnimation);
            else if (_currentControlShaker == shakerFromAnimator)
            {
                _currentControlShaker.StopAnimation();
                _allowControlFactor = false;
            }
            
            _lastPlayingShaker = _ShakeAnimatorsController.GetShaker(shakerTo.ToString());
            if(shakerFromAnimator == null || _lastPlayingShaker == null) return;
            if (isWalkOrCrouchMoveTo)
            {
                _currentControlShaker = _lastPlayingShaker;
                _allowControlFactor = true;
            }
            else _lastPlayingShaker.SetFactor(1, _ShakeFadeDuration);
            _lastPlayingShaker.StartAnimation();
        }
        private void OnGroundedFromState(IBaseState state)
        {
            if (State != FPSObjectState.Taken) return;
            _ShakeAnimatorsController.Stop(FPSObjectShakerType.JumpIn.ToString(), _ShakeFadeDuration);
            _ShakeAnimatorsController.Stop(FPSObjectShakerType.JumpRunIn.ToString(), _ShakeFadeDuration);
            _ShakeAnimatorsController.Play(state.FPSObjectShaker.ToString(), _ShakeFadeDuration);
        }

        public virtual void Update()
        {
            if (_currentControlShaker == null) return;
            var value = _playerMovementInput.StickValue;
            if(_allowControlFactor) _currentControlShaker.SetFactorImmediate(value);
            _currentControlShaker.SetSpeedFactorImmediate(value);
        }
        public virtual void OnDestroy()
        {
            UnsubscribeShake();
        }
    }
}