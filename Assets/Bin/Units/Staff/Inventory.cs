using System.Collections.Generic;
using Bin.Units.Items;

namespace Bin.Units.Staff
{
    public class Inventory
    {
        private readonly List<BaseItem> _items;

        protected Inventory()
        {
            _items = new List<BaseItem>();
        }


        public void Put(BaseItem item)
        {
            _items.Add(item);
        }

        public void Remove(BaseItem item)
        {
            _items.Remove(item);
        }
    }
}