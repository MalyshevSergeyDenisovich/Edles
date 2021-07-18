using System;
using UnityEngine;

namespace Test
{
    public abstract class MoodState
    {
        private float _speed;

        protected MoodState(float speed)
        {
            _speed = speed;
        }

        private void MoveTo(Vector3 target)
        {
            
        }
    }
}