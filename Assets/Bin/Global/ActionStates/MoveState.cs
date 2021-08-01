using UnityEngine;


namespace Bin.Global.ActionStates
{
    public class MoveState : ActionState 
    {

        public override void Start()
        {
            RotateTo();
            _animator.runtimeAnimatorController = Globals.WalkAnimation;
        }

        public override void Stop()
        {
                
        }

        public override void OnUpdateCheck()
        {
            if (Unit.transform.position.Equals(Unit.targetPos))
            {
                _actionStateSwitcher.SwitchState<IdleState>();
            }
            else
            {
                MoveToTarget();
            }
        }

        private void MoveToTarget()
        {

            Unit.transform.position =
                Vector3.MoveTowards(
                    Unit.transform.position,
                    Unit.targetPos,
                    Speed * Time.deltaTime);
        }

        private void RotateTo()
        {
            var q = Quaternion.LookRotation(Unit.targetPos - Unit.transform.position);
            Unit.transform.rotation = Quaternion.RotateTowards(Unit.transform.rotation, q,1000f );
        }

        public MoveState(UnitManager unit, Animator animator, IActionStateSwitcher actionStateSwitcher) : base(unit, animator, actionStateSwitcher)
        {
                
        }
    }
}