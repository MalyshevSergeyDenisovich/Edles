using UnityEngine;

namespace Bin.Pathfindings
{
    public class Path
    {
        public readonly Vector3[] LookPoints;
        public readonly Line[] TurnBoundaries;
        public readonly int FinishLineIndex;
        public readonly int SlowDownIndex;

        public Path(Vector3[] waypoints, Vector3 startPos, float turnDst, float stoppingDst)
        {
            LookPoints = waypoints;
            TurnBoundaries = new Line[waypoints.Length];
            FinishLineIndex = TurnBoundaries.Length - 1;

            var previousPoint = V3ToV2(startPos);
            for (var i = 0; i < LookPoints.Length; i++)
            {
                var curPoint = V3ToV2(LookPoints[i]);
                var dirToCurPoint = (curPoint - previousPoint).normalized;
                var turnBoundaryPoint = i == FinishLineIndex ? curPoint : curPoint - dirToCurPoint * turnDst;

                TurnBoundaries[i] = new Line(turnBoundaryPoint, previousPoint - dirToCurPoint * turnDst);
                previousPoint = turnBoundaryPoint;
            }

            var dstFromEndPoint = 0f;
            for (var i = LookPoints.Length-1; i > 0; i--)
            {
                dstFromEndPoint += Vector3.Distance(LookPoints[i], LookPoints[i - 1]);
                if (dstFromEndPoint > stoppingDst)
                {
                    SlowDownIndex = i;
                    break;
                }
            }
        }

        private Vector2 V3ToV2(Vector3 v3)
        {
            return new Vector2(v3.x, v3.z);
        }

        public void DrawWithGizmos()
        {
            Gizmos.color = Color.black;
            foreach (var p in LookPoints)
            {
                Gizmos.DrawCube(p + Vector3.up, Vector3.one);
            }

            Gizmos.color = Color.white;
            foreach (var l in TurnBoundaries)
            {
                l.DrawWithGizmos(10);
            }

        }
    }
}