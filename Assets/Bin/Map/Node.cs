using UnityEngine;

namespace Bin.Map
{
    public class Node : IHeapItem<Node>
    {
        public bool Walkable { get; }
        public Vector3 WorldPosition;
        public readonly int GridX;
        public readonly int GridY;
        public int MovementPenalty; 
        
        public int GCost;
        public int HCost;

        public Node Parent;

        public Node(bool walkable, Vector3 worldPosition, int gridX, int gridY, int movementPenalty)
        {
            Walkable = walkable;
            WorldPosition = worldPosition;
            GridX = gridX;
            GridY = gridY;
            MovementPenalty = movementPenalty;
        }

        private int FCost => GCost + HCost;
        public int CompareTo(Node other)
        {
            var compare = FCost.CompareTo(other.FCost);
            if (compare == 0)
            {
                compare = HCost.CompareTo(other.HCost);
            }

            return -compare;
        }
        
        public int HeapIndex { get; set; }
    }
}