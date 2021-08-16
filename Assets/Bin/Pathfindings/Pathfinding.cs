using System;
using System.Collections.Generic;
using Bin.Map;
using UnityEngine;
using Grid = Bin.Map.Grid;

namespace Bin.Pathfindings
{
    public class Pathfinding
    {
        private readonly Grid _grid;

        public Pathfinding(Grid grid)
        {
            _grid = grid;
        }
        
        public void FindPath(PathRequest request, Action<PathResult>callback)
        { 
            var waypoints = new Vector3[0]; 
            var pathSuccess = false;
            
            var startNode = _grid.GetNodeFromWorldPoint(request.PathStart);
            var targetNode = _grid.GetNodeFromWorldPoint(request.PathEnd);

            if (startNode.Walkable && targetNode.Walkable)
            {
                var openSet = new Heep<Node>(_grid.MaxSize);
                var closedSet = new HashSet<Node>();

                openSet.Add(startNode);

                while (openSet.Count > 0)
                {
                    var currentNode = openSet.RemoveFirst();
                    closedSet.Add(currentNode);

                    if (currentNode == targetNode)
                    {
                        pathSuccess = true;
                        break;
                    }

                    foreach (var neighbour in _grid.GetNeighbours(currentNode))
                    {
                        if (!neighbour.Walkable || closedSet.Contains(neighbour))
                        {
                            continue;
                        }

                        var newMovementCostToNeighbour = currentNode.GCost + GetDistance(currentNode, neighbour) +
                                                         neighbour.MovementPenalty;
                        
                        if (newMovementCostToNeighbour < neighbour.GCost || !openSet.Contains(neighbour))
                        {
                            neighbour.GCost = newMovementCostToNeighbour;
                            neighbour.HCost = GetDistance(neighbour, targetNode);
                            neighbour.Parent = currentNode;

                            if (!openSet.Contains(neighbour))
                            {
                                openSet.Add(neighbour);
                            }
                            else
                            {
                                openSet.UpdateItem(neighbour);
                            }
                        }
                    }
                }
            }
            if (pathSuccess)
            {
                waypoints = RetracePath(startNode, targetNode);
                pathSuccess = waypoints.Length > 0;
            }

            callback(new PathResult(waypoints, pathSuccess, request.Callback));
        }

        private Vector3[] RetracePath(Node startNode, Node endNode)
        {
            var path = new List<Node>();
            var currentNode = endNode;

            while (currentNode != startNode)
            {
                path.Add(currentNode);
                currentNode = currentNode.Parent;
            }

            var waypoints = SimplifyPath(path);
            Array.Reverse(waypoints);
            
            return waypoints;
        }

        private static Vector3[] SimplifyPath(IReadOnlyList<Node> path)
        {
            var waypoints = new List<Vector3>();
            var directionOld = Vector2.zero;

            for (var i = 1; i < path.Count; i++)
            {
                var directionNew = new Vector2(path[i - 1].GridX - path[i].GridX, path[i - 1].GridY - path[i].GridY);
                if (directionNew != directionOld)
                {
                    waypoints.Add(path[i].WorldPosition);
                }
                directionOld = directionNew;
            }

            return waypoints.ToArray();
        }

        private static int GetDistance(Node nodeA, Node nodeB)
        {
            var dstX = Mathf.Abs(nodeA.GridX - nodeB.GridX);
            var dstY = Mathf.Abs(nodeA.GridY - nodeB.GridY);

            if (dstX > dstY)
            {
                return 14 * dstY + 10 * (dstX - dstY);
            }
            return 14 * dstX + 10 * (dstY - dstX);
        }
    }
}