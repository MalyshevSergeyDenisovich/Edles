using Bin.Units.Items;

namespace Bin.Units.Staff
{
    public interface IInventors
    {
        public void Take(Item item);
        public void Drop(Item item);
    }
}