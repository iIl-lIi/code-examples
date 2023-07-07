using Game.Item;
using Game.PlayerStateMachine;

namespace Game.Character.Implementations.Player.States
{
    public class BasePlayerJumpOutState : IState
    {
        public FPSObjectAnimationType FPSObjectAnimation { get; set; } = FPSObjectAnimationType.OnGrounded;
        
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