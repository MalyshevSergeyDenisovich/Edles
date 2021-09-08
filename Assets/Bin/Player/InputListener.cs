
using System.Collections.Generic;
using UnityEngine;

namespace Bin.Player
{
    public class InputListener
    {
        private Dictionary<KeyCode, GameAction> keys;
        public InputListener()
        {
            keys = new Dictionary<KeyCode, GameAction>
            {
                {KeyCode.W, new WAction("W")},
                {KeyCode.A, new WAction("A")},
                {KeyCode.S, new WAction("S")},
                {KeyCode.D, new WAction("D")},
                {KeyCode.None, new WAction("None")}
            };

        }

        public void OnAction(Event e)
        {
            if (keys.ContainsKey(e.keyCode))
            {
                Debug.Log(keys[e.keyCode].Action());    
            }else {
               //
                Debug.Log("No " + e.keyCode);
            }
        }
    }

    public abstract class GameAction
    {
        private readonly string _action;
        
        protected GameAction(string action)
        {
            _action = action;
        }

        public string Action()
        {
            return _action;
        }
    }

    public class WAction : GameAction
    {
        public WAction(string action) : base(action)
        {
            
        }
    }
    public class AAction : GameAction
    {
        public AAction(string action) : base(action)
        {
            
        }
    }
    public class SAction : GameAction
    {
        public SAction(string action) : base(action)
        {
            
        }
    }
    public class DAction : GameAction
    {
        public DAction(string action) : base(action)
        {
            
        }
    }
}