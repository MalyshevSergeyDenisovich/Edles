using UnityEngine;

namespace Bin.Pathfindings
{
    public class Line
    {
        private const float VerticalLineGradient = 1e5f;
        
        private float _gradient;
        private float _yIntercept;

        private Vector2 _pointOnLine1;
        private Vector2 _pointOnLine2;

        private float _gradientPerpendicular;

        private bool _approachSide;
        
        public Line(Vector2 pointOnLine, Vector2 pointPerpendicularToLine)
        {
            var dx = pointOnLine.x - pointPerpendicularToLine.x;
            var dy = pointOnLine.y - pointPerpendicularToLine.y;

            if (dx != 0)
            {
                _gradientPerpendicular = dy / dx;
            }
            else
            {
                _gradientPerpendicular = VerticalLineGradient;
            }

            if (_gradientPerpendicular != 0)
            {
                _gradient = -1 / _gradientPerpendicular;   
            }
            else
            {
                _gradient = VerticalLineGradient;
            }

            _yIntercept = pointOnLine.y - _gradient * pointOnLine.x;

            _pointOnLine1 = pointOnLine;
            _pointOnLine2 = pointOnLine + new Vector2(1, _gradient);

            _approachSide = false;
            _approachSide = GetSide(pointPerpendicularToLine);
        }

        private bool GetSide(Vector2 p)
        {
            return (p.x - _pointOnLine1.x) * (_pointOnLine2.y - _pointOnLine1.y) >
                   (p.y - _pointOnLine1.y) * (_pointOnLine2.x - _pointOnLine1.x);
        }

        public bool HasCrossedLine(Vector2 p)
        {
            return GetSide(p) != _approachSide;
        }

        public float DistanceFromPoint(Vector2 point)
        {
            var yInterceptPerpendicular = point.y - _gradientPerpendicular * point.x;
            var intersectX = (yInterceptPerpendicular - _yIntercept) / (_gradient - _gradientPerpendicular);
            var intersectY = _gradient * intersectX + _yIntercept;
            return Vector2.Distance(point, new Vector2(intersectX, intersectY));
        }

        public void DrawWithGizmos(int i)
        {
            var lineDir = new Vector3(1, 0, _gradient).normalized;
            var lineCentre = new Vector3(_pointOnLine1.x, 0, _pointOnLine1.y) + Vector3.up;
            Gizmos.DrawLine(lineCentre - lineDir * i / 2f, lineCentre + lineDir * i / 2f);
        }
    }
}