﻿using Game.Item;
using Game.PlayerStateMachine;

namespace Game.Character.Implementations.Player.States
{
    public class BasePlayerJumpRunInState : AbstractPlayerState
    {
        public override string CameraAnimationName { get; set; } = "JumpRunInCamera";
        public override float AnimationCrossFadeDuration { get; set; } = 0.1f;
        public override FPSObjectAnimationType FPSObjectAnimation { get; set; } = FPSObjectAnimationType.JumpRunIn;

        public BasePlayerJumpRunInState(BasePlayerController playerController) : base(playerController) { }

        public override void Enter(IState fromState)
        {
            base.Enter(fromState);
            InteractionBlocking.Block(Constants.AimingBlock);
        }
        public override void Exit(IState toState)
        {
            base.Exit(toState);
            InteractionBlocking.Unblock(Constants.AimingBlock);
        }
        public override void Update()
        {
            if (playerController.TargetPlayerCharacter.JumpVelocity.y <= 0)
                playerController.SwitchState<BasePlayerJumpRunOutState>();
        }
    }
}