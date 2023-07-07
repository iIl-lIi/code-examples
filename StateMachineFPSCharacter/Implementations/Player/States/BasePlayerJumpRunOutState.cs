using Game.Item;
using Game.PlayerStateMachine;

namespace Game.Character.Implementations.Player.States
{
    public class BasePlayerJumpRunOutState : IState
    { 
        public FPSObjectAnimationType FPSObjectAnimation { get; set; } = FPSObjectAnimationType.OnGroundedRun;
        
        public void Enter(IState fromState)
        {
        }
        public void Exit(IState toState)
        {
        }
        public void Update()
        {
        }
    }
}