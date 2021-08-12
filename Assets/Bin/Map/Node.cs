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
        
        public int gCost;
        public int hCost;

        public Node Parent;

        private int _heapIndex;
        public Node(bool walkable, Vector3 worldPosition, int gridX, int gridY, int movementPenalty)
        {
            Walkable = walkable;
            WorldPosition = worldPosition;
            GridX = gridX;
            GridY = gridY;
            MovementPenalty = movementPenalty;
        }

        public int fCost => gCost + hCost;
        public int CompareTo(Node other)
        {
            var compare = fCost.CompareTo(other.fCost);
            if (compare == 0)
            {
                compare = hCost.CompareTo(other.hCost);
            }

            return -compare;
        }
        
        public int HeapIndex
        {
            get => _heapIndex;
            set => _heapIndex = value;
        }
    }
}