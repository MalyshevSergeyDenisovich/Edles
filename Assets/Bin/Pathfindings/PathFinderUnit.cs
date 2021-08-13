using System;
using System.Collections;
using UnityEngine;


namespace Bin.Pathfindings
{
    public class PathFinderUnit : MonoBehaviour
    {
        private const float MINPathUpdateTime = 0.2f;
        private const float PathUpdateMoveThreshold = 0.5f;
        
        [SerializeField] private Transform target;
        [SerializeField] private float speed = 7f;
        [SerializeField] private float turnSpeed = 20f;
        [SerializeField] private float turnDst = .1f;
        [SerializeField] private float stoppingDst = 0.1f;

        private Path _path;
        
        private void Start()
        {
            StartCoroutine(UpdatePath());
        }

        private void OnPathFound(Vector3[] waypoints, bool pathSuccessful)
        {
            if (pathSuccessful)
            {
                _path = new Path(waypoints, transform.position, turnDst, stoppingDst);
                StopCoroutine(FollowPath());
                StartCoroutine(FollowPath());
            }
        }

        private IEnumerator UpdatePath()
        {
            if (Time.timeSinceLevelLoad < .3f)
            {
                yield return new WaitForSeconds(.3f);
            }

            PathRequestManager.RequestPath(new PathRequest(transform.position, target.position, OnPathFound));
            
            const float sqrThreshold = PathUpdateMoveThreshold * PathUpdateMoveThreshold;
            var targetPosOld = target.position;
            
            while (true)
            {
                yield return new WaitForSeconds(MINPathUpdateTime);
                if ((target.position - targetPosOld).sqrMagnitude > sqrThreshold)
                {
                    PathRequestManager.RequestPath(new PathRequest(transform.position, target.position, OnPathFound));
                    targetPosOld = target.position;
                }
            }
        }

        private IEnumerator FollowPath()
        {
            var followingPath = true;
            var pathIndex = 0;
            transform.LookAt(_path.LookPoints [0]);

            var speedPercent = 1f;
            
            while (followingPath)
            {
                var pos2D = new Vector2(transform.position.x, transform.position.z);
                while (_path.TurnBoundaries[pathIndex].HasCrossedLine(pos2D))
                {
                    if (pathIndex == _path.FinishLineIndex)
                    {
                        followingPath = false;
                        break;
                    }
                    else
                    {
                        pathIndex++;
                    }
                }

                if (followingPath)
                {
                    if (pathIndex >= _path.SlowDownIndex && stoppingDst > 0)
                    {
                        speedPercent =
                            Mathf.Clamp01(_path.TurnBoundaries[_path.FinishLineIndex].DistanceFromPoint(pos2D) /
                                          stoppingDst);
                        if (speedPercent<0.01f)
                        {
                            followingPath = false;
                        }
                    }

                    var targetRotation = Quaternion.LookRotation(_path.LookPoints[pathIndex] - transform.position);
                    transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, Time.deltaTime * turnSpeed);
                    transform.Translate(Vector3.forward * (Time.deltaTime * speed * speedPercent), Space.Self);
                }

                yield return null;
            }
        }

        public void OnDrawGizmos()
        {
            if (_path != null)
            {
                _path.DrawWithGizmos();
            }
        }
    }
}