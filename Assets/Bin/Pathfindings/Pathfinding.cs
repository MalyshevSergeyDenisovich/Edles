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

            var openSet = new Heep<Node>(_grid.MaxSize);
            var closedSet = new HashSet<Node>();
            
            openSet.Add(startNode);

            while (openSet.Count > 0)
            {
                var currentNode = openSet.RemoveFirst();
                closedSet.Add(currentNode);

                if (currentNode == targetNode)
                {
                    RetracePath(startNode, targetNode);
                    return;
                }

                foreach (var neighbour in _grid.GetNeighbours(currentNode))
                {
                    if (!neighbour.Walkable || closedSet.Contains(neighbour))
                    {
                        continue;
                    }

                    var newMovementCostToNeighbour = currentNode.gCost + GetDistance(currentNode, neighbour);
                    if (newMovementCostToNeighbour < neighbour.gCost || !openSet.Contains(neighbour))
                    {
                        neighbour.gCost = newMovementCostToNeighbour;
                        neighbour.hCost = GetDistance(neighbour, targetNode);
                        neighbour.Parent = currentNode;
                        
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
                currentNode = currentNode.Parent;
            }
            path.Reverse();

            _grid.path = path;
        }

        private int GetDistance(Node nodeA, Node nodeB)
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