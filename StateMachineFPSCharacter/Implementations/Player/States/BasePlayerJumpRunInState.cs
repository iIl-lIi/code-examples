﻿using Game.Item;
using Game.PlayerStateMachine;
using OtherTools._3D.TransformAnimatorSystem;

namespace Game.Character.Implementations.Player.States
{
    public class BasePlayerJumpRunInState : IState
    {
        private const string CAMERA_SHAKER_NAME = "JumpRunInCamera";
        private const float CAMERA_SHAKER_CROSS_FADE_DURATION = 0.1f;
        
        public FPSObjectAnimationType FPSObjectAnimation { get; set; } = FPSObjectAnimationType.JumpRunIn;

        private readonly BasePlayerController _playerController;
        private readonly TransformAnimatorsController _animatorController;

        public BasePlayerJumpRunInState(TransformAnimatorsController animatorController, BasePlayerController playerController)
        {
            this._playerController = playerController;
            this._animatorController = animatorController;
            
            var target = animatorController.GetAnimator(CAMERA_SHAKER_NAME).Target;
            playerController.TargetPlayerCharacter.AddOffsets.Add(new AddOffset()
            {
                UseParameters = UseTransformParameters.Everything,
                GetPosition = () => target.localPosition,
                GetRotation = () => target.localRotation
            });
        }

        public void Enter(IState fromState)
        {
            _animatorController.Play(CAMERA_SHAKER_NAME, CAMERA_SHAKER_CROSS_FADE_DURATION);
            InteractionBlocking.Block(Constants.AimingBlock);
        }
        public void Exit(IState toState)
        {
            _animatorController.Stop(CAMERA_SHAKER_NAME, CAMERA_SHAKER_CROSS_FADE_DURATION);
            InteractionBlocking.Unblock(Constants.AimingBlock);
        }
        public void Update()
        {
            if(_playerController.TargetPlayerCharacter.JumpVelocity.y <= 0)
                _playerController.SwitchState<BasePlayerJumpRunOutState>();
        }
    }
}