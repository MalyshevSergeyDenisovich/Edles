
using UnityEngine;

namespace Bin.Player
{
    public class InputManager : MonoBehaviour
    {
        private InputListener _inputListener;

        private void Awake()
        {
            _inputListener = new InputListener();
        }

        private void OnGUI()
        {
            _inputListener.OnAction(Event.current);
        }
    }
}