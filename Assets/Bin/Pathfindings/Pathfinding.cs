using System;
using System.Collections.Generic;
using Bin.Map;
using UnityEngine;
using Grid = Bin.Map.Grid;

namespace Bin.Pathfindings
{
    public class Pathfinding : MonoBehaviour
    {
        public Transform seeker, target;
        
        
        private Grid _grid;

        private void Awake()
        {
            _grid = GetComponent<Grid>();
        }

        private void Update()
        {
            FindPath(seeker.position, target.position);
        }

        private void FindPath(Vector3 startPos, Vector3 targetPos)
        {
            var startNode = _grid.GetNodeFromWorldPoint(startPos);
            var targetNode = _grid.GetNodeFromWorldPoint(targetPos);

            var openSet = new List<Node>();
            var closedSet = new HashSet<Node>();
            
            openSet.Add(startNode);

            while (openSet.Count > 0)
            {
                var currentNode = openSet[0];
                for (var i = 1; i < openSet.Count; i++)
                {
                    if (openSet[i].fCost < currentNode.fCost || openSet[i].fCost ==currentNode.fCost && openSet[i].hCost < currentNode.hCost)
                    {
                        currentNode = openSet[i];
                    }
                }

                openSet.Remove(currentNode);
                closedSet.Add(currentNode);

                if (currentNode == targetNode)
                {
                    RetracePath(startNode, targetNode);
                    return;
                }

                foreach (var neighbour in _grid.GetNeighbours(currentNode))
                {
                    if (!neighbour.walkable || closedSet.Contains(neighbour))
                    {
                        continue;
                    }

                    var newMovementCostToNeighbour = currentNode.gCost + GetDistance(currentNode, neighbour);
                    if (newMovementCostToNeighbour < neighbour.gCost || !openSet.Contains(neighbour))
                    {
                        neighbour.gCost = newMovementCostToNeighbour;
                        neighbour.hCost = GetDistance(neighbour, targetNode);
                        neighbour.parent = currentNode;
                        
                        if (!openSet.Contains(neighbour))
                            openSet.Add(neighbour);
                    }
                }
            }
        }

        private void RetracePath(Node startNode, Node endNode)
        {
            var path = new List<Node>();
            var currentNode = endNode;

            while (currentNode != startNode)
            {
                path.Add(currentNode);
                currentNode = currentNode.parent;
            }
            path.Reverse();

            _grid.path = path;
        }

        private int GetDistance(Node nodeA, Node nodeB)
        {
            var dstX = Mathf.Abs(nodeA.gridX - nodeB.gridX);
            var dstY = Mathf.Abs(nodeA.gridY - nodeB.gridY);

            if (dstX > dstY)
            {
                return 14 * dstY + 10 * (dstX - dstY);
            }
            return 14 * dstX + 10 * (dstY - dstX);
        }
    }
}