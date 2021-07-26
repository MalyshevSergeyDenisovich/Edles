using UnityEngine;

namespace Bin.Global.ActionStates
{
    public abstract class ActionState
    {
        protected readonly IActionStateSwitcher _actionStateSwitcher;
        protected readonly UnitManager Unit;
            
        protected const float Speed = 5f;

        protected readonly Animator _animator;
            
            

        protected ActionState(UnitManager unit, Animator animator, IActionStateSwitcher actionStateSwitcher)
        {
            Unit = unit;
            _animator = animator;
            _actionStateSwitcher = actionStateSwitcher;
        }
        public abstract void Start();
        public abstract void Stop();

        public abstract void OnUpdateCheck();
    }
}