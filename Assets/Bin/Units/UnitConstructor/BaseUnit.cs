using System.Collections.Generic;
using Bin.Units.Items;
using Bin.Units.Skills;
using Bin.Units.Staff;

namespace Bin.Units.UnitConstructor
{
    public abstract class BaseUnit
    {        
        private Inventory _inventory;
        private HealthState _curHealthState;
        private List<Skill> _skills;
        
        
        protected BaseUnit(HealthState healthState)
        {
            _inventory = new Inventory();
            
            _curHealthState = healthState;
            
            _skills = new List<Skill>();
        }


        public void Take(Item item)
        {
            _inventory.Put(item);
        }

        public void Drop(Item item)
        {
            _inventory.Remove(item);
        }
    }
}
