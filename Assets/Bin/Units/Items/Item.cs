namespace Bin.Units.Items
{
    public abstract class Item
    {
        protected Item(float weight)
        {
            Weight = weight;
        }

        public float Weight { get; }
    }
}