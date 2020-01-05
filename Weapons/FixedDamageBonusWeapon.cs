namespace Robowars.Weapons
{
    public class FixedDamageBonusWeapon : WeaponEnhancement
    {
        public int Bonus { get; private set; }

        public override int PowerDrain => EnhancedWeapon.PowerDrain + Bonus / 2;

        public FixedDamageBonusWeapon(Weapon weapontToEnhance, int bonus)
        {
            EnhancedWeapon = weapontToEnhance;
            Bonus = bonus;
        }
        public override int GetDamage()
        {
            return EnhancedWeapon.GetDamage() + Bonus;
        }
    }
}