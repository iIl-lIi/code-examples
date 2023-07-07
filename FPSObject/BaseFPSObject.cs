using Cysharp.Threading.Tasks;
using Game.Character.Implementations.Player;
using Game.Input;
using Game.PlayerStateMachine;
using OtherTools._3D.TransformAnimatorSystem;

namespace Game.Item
{
    using UnityEngine;
    
    public class BaseFPSObject : MonoBehaviour, IFPSObject
    {
        [field: SerializeField, Header("FPS Object")] public ItemIndex Index { get; set; }
        [field: SerializeField] public GameObject MeshRoot { get; set; }
        [field: SerializeField] public TransformAnimatorFollower AnimatorsFollower { get; set; }
        [field: SerializeField] public TransformAnimatorsController AnimatorsController { get; set; }
        [SerializeField] protected float _AnimationsFadeDuration = 0.1f;
        [SerializeField] protected float _StopAnimationsFadeDuration = 0.75f;

        public GameObject Root { get; set; }
        public FPSObjectState FPSState { get; set; }

        private TransformAnimator _currentControlAnimation;
        private TransformAnimator _lastPlayingAnimation;
        private IJoystickInput _playerMovementInput;
        private BasePlayerController _playerController;
        private bool _allowControlFactor;

        public virtual void Initialize()
        {
            FPSState = FPSObjectState.Busy;
            Root = gameObject;
            MeshRoot.SetActive(false);
            _playerMovementInput = (IJoystickInput)InputManager.PlayerMovementInput;
            _playerController = (PlayerLoader.PlayerInstance as BasePlayerCharacter).PlayerController;
            SubscribeAnimations();
            FPSState = FPSObjectState.PuttedAway;
        }
        public virtual async UniTask TakeUp()
        {
            if (FPSState == FPSObjectState.Busy || MeshRoot == null) return;
            FPSState = FPSObjectState.Busy;
            MeshRoot.SetActive(true);
            SetAnimationFromPlayerState();
            FPSState = FPSObjectState.Taken;
        }
        public virtual async UniTask PutAway()
        {
            if (FPSState == FPSObjectState.Busy || MeshRoot == null) return;
            FPSState = FPSObjectState.Busy;
            MeshRoot.SetActive(false);
            StopPlayingLastAnimation();
            _allowControlFactor = false;
            FPSState = FPSObjectState.PuttedAway;
        }
        public void UpdateFPSObject()
        {
            if (_currentControlAnimation == null) return;
            var value = _playerMovementInput.StickValue;
            if(_allowControlFactor) _currentControlAnimation.SetFactorImmediate(value);
            _currentControlAnimation.SetSpeedFactorImmediate(value);
        }
        
        protected void StopPlayingLastAnimation()
        {
            _lastPlayingAnimation.SetFactor(0, _StopAnimationsFadeDuration, 
                _lastPlayingAnimation.StopAnimation);
        }
        
        protected void SetAnimationFromPlayerState()
        {
            var currentStateAnimation = _playerController.CurrentState.FPSObjectAnimation;
            var findAnimation = AnimatorsController.GetAnimator($"{currentStateAnimation}");

            if(findAnimation == _currentControlAnimation 
                || findAnimation ==  _lastPlayingAnimation) 
                    return;

            if (_lastPlayingAnimation) _lastPlayingAnimation.SetFactor(0, _AnimationsFadeDuration, 
                _lastPlayingAnimation.StopAnimation);
            if (_currentControlAnimation) _currentControlAnimation.SetFactor(0, _AnimationsFadeDuration, 
                _currentControlAnimation.StopAnimation);

            _lastPlayingAnimation = findAnimation;
            var isWalkOrCrouchMoveTo = currentStateAnimation is FPSObjectAnimationType.Walk or FPSObjectAnimationType.CrouchMove;

            if (isWalkOrCrouchMoveTo)
            {
                _currentControlAnimation = _lastPlayingAnimation;
                _allowControlFactor = true;
            }
            else _lastPlayingAnimation.SetFactor(1, _AnimationsFadeDuration);
            _lastPlayingAnimation.StartAnimation();
        }

        private void SubscribeAnimations()
        {
            _playerController.StateSwitched += OnStateSwitched;
            _playerController.GroundedFromState += OnGroundedFromState;
        }
        private void UnsubscribeShake()
        {
            _playerController.StateSwitched -= OnStateSwitched;
            _playerController.GroundedFromState -= OnGroundedFromState;
        }
        private void OnStateSwitched(IState from, IState to)
        {
            var animationFromName = from.FPSObjectAnimation;
            var animationTo = to.FPSObjectAnimation;
            var onGroundedTo = animationTo is FPSObjectAnimationType.OnGrounded or FPSObjectAnimationType.OnGroundedRun;

            if (onGroundedTo || FPSState != FPSObjectState.Taken) return;

            var animationFrom = AnimatorsController.GetAnimator(animationFromName.ToString());
            var onGroundedFrom = animationFromName is FPSObjectAnimationType.OnGrounded or FPSObjectAnimationType.OnGroundedRun;
            var isWalkOrCrouchMoveTo = animationTo is FPSObjectAnimationType.Walk or FPSObjectAnimationType.CrouchMove;
            
            if (animationFrom == null) return;
            if (onGroundedFrom == false) animationFrom.SetFactor(0, _AnimationsFadeDuration, animationFrom.StopAnimation);
            else if (_currentControlAnimation == animationFrom)
            {
                animationFrom.SetFactor(0, _AnimationsFadeDuration, animationFrom.StopAnimation);
                _allowControlFactor = false;
            }
            
            _lastPlayingAnimation = AnimatorsController.GetAnimator(animationTo.ToString());
            if (animationFrom == null || _lastPlayingAnimation == null) return;
            if (isWalkOrCrouchMoveTo)
            {
                _currentControlAnimation = _lastPlayingAnimation;
                _allowControlFactor = true;
            }
            else _lastPlayingAnimation.SetFactor(1, _AnimationsFadeDuration);
            _lastPlayingAnimation.StartAnimation();
        }
        private void OnGroundedFromState(IState state)
        {
            if (FPSState != FPSObjectState.Taken) return;
            AnimatorsController.Stop(FPSObjectAnimationType.JumpIn.ToString(), _AnimationsFadeDuration);
            AnimatorsController.Stop(FPSObjectAnimationType.JumpRunIn.ToString(), _AnimationsFadeDuration);
            AnimatorsController.Play(state.FPSObjectAnimation.ToString(), _AnimationsFadeDuration);
        }

        protected virtual void Update() => UpdateFPSObject();
        protected virtual void OnDestroy() => UnsubscribeShake();
    }
}