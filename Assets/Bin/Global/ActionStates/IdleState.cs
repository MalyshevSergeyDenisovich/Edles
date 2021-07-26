using UnityEngine;


namespace Bin.Global.ActionStates
{
    public class IdleState : ActionState
    {
        private float _waitTime = 1f;
        private void Wait()
        {
            _waitTime -= Time.deltaTime;
        }

        public override void Start()
        {
            _animator.runtimeAnimatorController = Globals.IdleAnimation;
            _waitTime = Globals.Random.Next(1,5);
            SetTargetPoint();
        }

        public override void Stop()
        {

        }

        public override void OnUpdateCheck()
        {
            if (_waitTime <= 0)
            {
                SetTargetPoint();
                _actionStateSwitcher.SwitchState<MoveState>();
            }
            else
            {
                Wait();
            }
        }

        private void SetTargetPoint()
        {
            var x = Globals.Random.Next(-10,10);
            var z = Globals.Random.Next(-10,10);
            Unit.SetTargetPoint(new Vector3(x, 0, z));
        }

        public IdleState(UnitManager unit, Animator animator, IActionStateSwitcher actionStateSwitcher) : base(unit, animator, actionStateSwitcher)
        {
                
        }
    }
}