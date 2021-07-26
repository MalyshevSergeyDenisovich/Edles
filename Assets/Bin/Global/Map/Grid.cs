using UnityEngine;

namespace Bin.Global.Map
{
    public class Grid
    {
        private int _width;
        private int _height;
        private float _sellSize;
        private int[,] _gridArray;

        public Grid(int width, int height, float sellSize)
        {
            _width = width;
            _height = height;
            _sellSize = sellSize;

            _gridArray = new int[width, height];
            
            Debug.Log($"width = {width}, height = {height}");

            for (var x = 0; x < _gridArray.GetLength(0); x++)
            {
                for (var y = 0; y < _gridArray.GetLength(1); y++)
                {
                    var go = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                    go.transform.position = GetWorldPosition(x, y);
                    go.transform.localScale *= .1f;

                    Debug.DrawLine(GetWorldPosition(x, y), GetWorldPosition(x, y + 1), Color.cyan, 10f);
                    Debug.DrawLine(GetWorldPosition(x, y), GetWorldPosition(x + 1, y), Color.cyan, 10f);
                    
                }
            }
            Debug.DrawLine(GetWorldPosition(0, height), GetWorldPosition(width, height), Color.cyan, 10f);
            Debug.DrawLine(GetWorldPosition(width, 0), GetWorldPosition(width, height), Color.cyan, 10f);
            
        }

        private Vector3 GetWorldPosition(int x, int y)
        {
            return new Vector3(x, 0, y) * _sellSize;
        }

    }
}