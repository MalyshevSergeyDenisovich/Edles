using System.Collections;
using UnityEngine;


namespace Bin.Pathfindings
{
    public class PathFinderUnit : MonoBehaviour
    {
        [SerializeField] private Transform target;
        private float _speed = 10f;
        private Vector3[] _path;
        private int _targetIndex;

        private void Start()
        {
            PathRequestManager.RequestPath(transform.position, target.position, OnPathFound);
        }

        private void OnPathFound(Vector3[] newPath, bool pathSuccessful)
        {
            if (pathSuccessful)
            {
                _path = newPath;
                StopCoroutine(FollowPath());
                StartCoroutine(FollowPath());
            }
        }

        private IEnumerator FollowPath()
        {
            var currentWaypoint = _path[0];
            while (true)
            {
                if (transform.position == currentWaypoint)
                {
                    _targetIndex++;
                    if (_targetIndex >= _path.Length)
                    {
                        yield break;
                    }

                    currentWaypoint = _path[_targetIndex];
                }

                transform.position = Vector3.MoveTowards(transform.position, currentWaypoint, _speed * Time.deltaTime);
                yield return null;
            }
        }

        public void OnDrawGizmos()
        {
            if (_path == null) return;
            for (var i = _targetIndex; i < _path.Length; i++)
            {
                Gizmos.color = Color.black;
                Gizmos.DrawCube(_path[i], Vector3.one);
                if (i == _targetIndex)
                {
                    Gizmos.DrawLine(transform.position, _path[i]);
                }
                else
                {
                    Gizmos.DrawLine(_path[i - 1], _path[i]);
                }
            }
        }
    }
}