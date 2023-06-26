using Game.FPSObject;
using Game.StateMachine.Player;

namespace Game.Character.Implementations.Player.States
{
    public class BasePlayerJumpOutState : IBaseState
    {
        public FPSObjectShakerType FPSObjectShaker { get; set; } = FPSObjectShakerType.OnGrounded;
        
        public void Enter(IBaseState fromState)
        {
            
        }
        public void Exit(IBaseState toState)
        {

        }
        public void Update()
        {

        }
    }
}