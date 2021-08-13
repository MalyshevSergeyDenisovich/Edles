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
        [SerializeField] private int obstacleProximityPenalty = 10;
        private LayerMask _walkableLayerMask;
        private readonly Dictionary<int, int> _walkableRegionsDictionary = new Dictionary<int, int>();
        
        private Node[,] _grid;

        private float _nodeDiameter;
        private int _gridSizeX, _gridSizeY;
        
        
        private int _penaltyMin = int.MaxValue;
        private int _penaltyMax = int.MinValue;
        private void Awake()
        {
            _nodeDiameter = nodeRadius * 2;
            _gridSizeX = Mathf.RoundToInt(gridWorldSize.x / _nodeDiameter);
            _gridSizeY = Mathf.RoundToInt(gridWorldSize.y / _nodeDiameter);

            foreach (var region in walkableRegions)
            {
                _walkableLayerMask.value = _walkableLayerMask |= region.terrainMask.value;
                _walkableRegionsDictionary.Add((int)Mathf.Log(region.terrainMask.value, 2), region.terrainPenalty);
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


                    var ray = new Ray(worldPoint + Vector3.up * 50, Vector3.down);
                    if (Physics.Raycast(ray, out var hit, 100, _walkableLayerMask))
                    {
                        _walkableRegionsDictionary.TryGetValue(hit.collider.gameObject.layer, out movementPenalty);
                    }

                    if (!walkable)
                    {
                        movementPenalty += obstacleProximityPenalty;
                    }

                    _grid[x, y] = new Node(walkable, worldPoint, x, y, movementPenalty);
                }
            }

            BlurPenaltyMap(3);
        }

        private void BlurPenaltyMap(int blurSize)
        {
            var kernelSize = blurSize * 2 + 1;
            var kernelExtents = (kernelSize - 1) / 2;
            var penaltyHorizontalPass = new int[_gridSizeX, _gridSizeY];
            var penaltyVerticalPass = new int[_gridSizeX, _gridSizeY];

            for (var y = 0; y < _gridSizeY; y++)
            {
                for (var x = -kernelExtents; x <= kernelExtents; x++)
                {
                    var sampleX = Mathf.Clamp(x, 0, kernelExtents);
                    penaltyHorizontalPass[0, y] += _grid[sampleX, y].MovementPenalty;
                }

                for (var x = 1; x < _gridSizeX; x++)
                {
                    var removeIndex = Mathf.Clamp(x - kernelExtents - 1, 0, _gridSizeX);
                    var addIndex = Mathf.Clamp(x + kernelExtents, 0, _gridSizeX - 1);

                    penaltyHorizontalPass[x, y] = penaltyHorizontalPass[x - 1, y] -
                        _grid[removeIndex, y].MovementPenalty + _grid[addIndex, y].MovementPenalty;
                }
            }
            
            for (var x = 0; x < _gridSizeY; x++)
            {
                for (var y = -kernelExtents; y <= kernelExtents; y++)
                {
                    var sampleY = Mathf.Clamp(y, 0, kernelExtents);
                    penaltyVerticalPass[x, 0] += penaltyHorizontalPass[x, sampleY];
                }

                var blurredPenalty = Mathf.RoundToInt((float)penaltyVerticalPass[x, 0] / (kernelSize * kernelSize));
                _grid[x, 0].MovementPenalty = blurredPenalty;
                
                for (var y = 1; y < _gridSizeY; y++)
                {
                    var removeIndex = Mathf.Clamp(y - kernelExtents - 1, 0, _gridSizeY);
                    var addIndex = Mathf.Clamp(y + kernelExtents, 0, _gridSizeY - 1);

                    penaltyVerticalPass[x, y] = penaltyVerticalPass[x, y - 1] -
                        penaltyHorizontalPass[x, removeIndex] + penaltyHorizontalPass[x, addIndex];
                    blurredPenalty = Mathf.RoundToInt((float)penaltyVerticalPass[x, y] / (kernelSize * kernelSize));
                    _grid[x, y].MovementPenalty = blurredPenalty;

                    if (blurredPenalty > _penaltyMax)
                    {
                        _penaltyMax = blurredPenalty;
                    }
                    if (blurredPenalty < _penaltyMin)
                    {
                        _penaltyMin = blurredPenalty;
                    }
                }
            }
            
        }

        public IEnumerable<Node> GetNeighbours(Node node)
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
                    Gizmos.color = Color.Lerp(Color.white, Color.black,
                        Mathf.InverseLerp(_penaltyMin, _penaltyMax, node.MovementPenalty));
                    
                    Gizmos.color = node.Walkable ? Gizmos.color : Color.red;
                    Gizmos.DrawCube(node.WorldPosition, Vector3.one * (_nodeDiameter));
                }
            }
        }
        
        [Serializable]
        public class TerrainType
        {
            public LayerMask terrainMask;
            public int terrainPenalty;
        }
    }
}