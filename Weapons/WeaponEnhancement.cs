namespace Robowars.Weapons
{
    /// <summary>Represents an enchancement of a <see cref="Weapon"/>, by transparently wrapping the weapon while also implementing its interface.</summary>
    public abstract class WeaponEnhancement : Weapon
    {
        public Weapon EnhancedWeapon { get; set; }

        public override int GetDamage()
        {
            return EnhancedWeapon.GetDamage();
        }
    }
}