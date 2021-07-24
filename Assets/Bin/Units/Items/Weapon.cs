namespace Bin.Units.Items
{
    public interface IWeapon
    {
        void TakeDamage();
        int WeaponType { get; }
    }
}