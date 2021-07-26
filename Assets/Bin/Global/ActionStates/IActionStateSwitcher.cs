namespace Bin.Global.ActionStates
{
    public interface IActionStateSwitcher
    {
        void SwitchState<T>() where T : ActionState;
    }
}