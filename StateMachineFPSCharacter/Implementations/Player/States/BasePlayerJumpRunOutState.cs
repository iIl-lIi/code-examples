using Game.FPSObject;
using Game.StateMachine.Player;

namespace Game.Character.Implementations.Player.States
{
    public class BasePlayerJumpRunOutState : IBaseState
    { 
        public FPSObjectShakerType FPSObjectShaker { get; set; } = FPSObjectShakerType.OnGroundedRun;
        
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