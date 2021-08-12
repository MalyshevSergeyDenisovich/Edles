using System;
using System.Collections.Generic;
using UnityEngine;

namespace Bin.Map
{
    public class Grid : MonoBehaviour
    {
        [SerializeField] private bool displayGridGizmos;
        [SerializeField] private LayerMask unwalkableMask;
        [SerializeField] private Vector2 gridWorldSize;
        [SerializeField] private float nodeRadius;
        [SerializeField] private TerrainType[] walkableRegions;
        private LayerMask _walkableLayerMask;
        private Dictionary<int, int> _walkableRegionsDictionary = new Dictionary<int, int>();
        
        private Node[,] _grid;

        private float _nodeDiameter;
        private int _gridSizeX, _gridSizeY;
        private void Awake()
        {
            _nodeDiameter = nodeRadius * 2;
            _gridSizeX = Mathf.RoundToInt(gridWorldSize.x / _nodeDiameter);
            _gridSizeY = Mathf.RoundToInt(gridWorldSize.y / _nodeDiameter);

            foreach (var region in walkableRegions)
            {
                _walkableLayerMask.value = _walkableLayerMask |= region.TerrainMask.value;
                _walkableRegionsDictionary.Add((int)Mathf.Log(region.TerrainMask.value, 2), region.TerrainPenalty);
            }
            
            CreateGrid();
        }

        public int MaxSize => _gridSizeX * _gridSizeY;

        private void CreateGrid()
        {
            _grid = new Node[_gridSizeX, _gridSizeY];
            var worldBottomLeft = transform.position - Vector3.right * gridWorldSize.x / 2 -
                                  Vector3.forward * gridWorldSize.y / 2; 
            
            for (var x = 0; x < _gridSizeX; x++)
            {
                for (var y = 0; y < _gridSizeY; y++)
                {
                    var worldPoint = worldBottomLeft + Vector3.right * (x * _nodeDiameter + nodeRadius) +
                                     Vector3.forward * (y * _nodeDiameter + nodeRadius);

                    var walkable = !Physics.CheckSphere(worldPoint, nodeRadius, unwalkableMask);
                    var movementPenalty = 0;

                    if (walkable)
                    {
                        var ray = new Ray(worldPoint + Vector3.up *50, Vector3.down);
                        RaycastHit hit;
                        if (Physics.Raycast(ray, out hit, 100, _walkableLayerMask))
                        {
                            _walkableRegionsDictionary.TryGetValue(hit.collider.gameObject.layer, out movementPenalty);
                        }
                    }

                    _grid[x, y] = new Node(walkable, worldPoint, x, y, movementPenalty);
                }
            }
        }

        public List<Node> GetNeighbours(Node node)
        {
            var neighbours = new List<Node>();
            
            for (var x = -1; x <= 1; x++)
            {
                for (var y = -1; y <= 1; y++)
                {
                    if (x == 0 && y == 0)
                        continue;

                    var checkX = node.GridX + x;
                    var checkY = node.GridY + y;

                    if (checkX >= 0 && checkX <= _gridSizeX && checkY >= 0 && checkY <= _gridSizeY)
                    {
                        neighbours.Add(_grid[checkX, checkY]);
                    }
                }    
            }

            return neighbours;
        }

        public Node GetNodeFromWorldPoint(Vector3 worldPosition)
        {
            var percentX = (worldPosition.x + gridWorldSize.x / 2) / gridWorldSize.x;
            var percentY = (worldPosition.z + gridWorldSize.y / 2) / gridWorldSize.y;
            percentX = Mathf.Clamp01(percentX);
            percentY = Mathf.Clamp01(percentY);

            var x = Mathf.RoundToInt((_gridSizeX - 1) * percentX);
            var y = Mathf.RoundToInt((_gridSizeY - 1) * percentY);
            return _grid[x,y];
        }


        private void OnDrawGizmos()
        {
            Gizmos.DrawWireCube(transform.position, new Vector3(gridWorldSize.x, 1, gridWorldSize.y));

            if (_grid != null && displayGridGizmos)
            {
                foreach (var node in _grid)
                {
                    Gizmos.color = node.Walkable ? Color.white : Color.red;
                    Gizmos.DrawCube(node.WorldPosition, Vector3.one * (_nodeDiameter - .1f));
                }
            }
        }
        
        [Serializable]
        public class TerrainType
        {
            public LayerMask TerrainMask;
            public int TerrainPenalty;
        }
    }
}