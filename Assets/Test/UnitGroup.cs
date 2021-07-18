using System.Collections.Generic;
using Bin.Units.UnitConstructor;


public struct UnitGroup
{

    private List<BaseUnit> _units;
    
    
    public UnitGroup(params BaseUnit[] units)
    {
        _units = new List<BaseUnit>();
        _units.AddRange(units);
    }

    public void AddUnit(BaseUnit unit)
    {
        _units.Add(unit);
    }

    public void RemoveUnit(BaseUnit unit)
    {
        _units.Remove(unit);
    }

    private bool IsUnitInGroup(BaseUnit unit)
    {
        return _units.Contains(unit);
    }

    private int GetSize()
    {
        return _units.Count;
    }

}
