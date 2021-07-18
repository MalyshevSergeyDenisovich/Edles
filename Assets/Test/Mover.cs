using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;


namespace Test
{
    public class Mover : MonoBehaviour
    {
        [SerializeField] private Vector3 _targetPosition;
        [SerializeField] private bool _isMovable;
        
        private float _totalMovementTime;
        

        private void Awake()
        {
            _totalMovementTime = GlobalRandom.Random.Next(3, 10);
            _isMovable = false;
        }

        private void Start()
        {
            
        }

        private void Update()
        {
            if (!_isMovable)
            {
                SetTargetPoint();
                
                StartMove();
            }


            if (InTargetPoint())
            {
                StopMove();
            }
        }

        public void StartMoveCor()
        {
            StartCoroutine(MoveObject());;
        }

        private IEnumerator MoveObject()
        {
            var curPos = transform.position; 
            var currentMovementTime = 0f;
            while (!InTargetPoint()) {
                currentMovementTime += Time.deltaTime;
                transform.localPosition = Vector3.Lerp(curPos, _targetPosition, currentMovementTime / _totalMovementTime);
                yield return null;
            }
            
        }

        public void SetTargetPoint()
        {
            _targetPosition = GetNewRandomVector();
        }

        private void StartMove()
        {
            _isMovable = true;
        }
        private void StopMove()
        {
            _isMovable = false;
        }

        public bool IsMovable()
        {
            return _isMovable;
        }
        
        public bool InTargetPoint()
        {
            return transform.position.Equals(_targetPosition);
        }

        private  Vector3 GetNewRandomVector()
        {
            const int i = 10;
            const int di = i * 2;

            return  transform.position + new Vector3(GlobalRandom.Random.Next(di) - i,
                0,
                GlobalRandom.Random.Next(di) - i);
        }
    }
}
