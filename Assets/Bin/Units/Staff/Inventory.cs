using System.Collections.Generic;
using System.Linq;
using Bin.Units.Items;

namespace Bin.Units.Staff
{
    public class Inventory
    {
        private readonly List<Item> _items;
        private Item _selectedItem;

        
        public Inventory(IWeapon weapon, Item selectedItem = null)
        {
            _items = new List<Item>();

            
            _selectedItem = selectedItem;
        }


        public void Put(Item item)
        {
            _items.Add(item);
        }

        public void Remove(Item item)
        {
            _items.Remove(item);
        }

        public float GetInventoryWeight()
        {
            return _items.Sum(item => item.Weight);
        }
    }
}