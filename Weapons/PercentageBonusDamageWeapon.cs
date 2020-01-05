using System;

namespace Robowars.Weapons
{
    public class PercentageBonusDamageWeapon : WeaponEnhancement
    {
        public double BonusFactor { get; private set; }

        /// <summary>Powerdrain increases slightly exponentially with stronger weapons.</summary>
        public override int PowerDrain => (int)Math.Round(EnhancedWeapon.PowerDrain * Math.Pow(BonusFactor, 1.2));

        public PercentageBonusDamageWeapon(Weapon weapontToEnhance, int percentBonus)
        {
            EnhancedWeapon = weapontToEnhance;
            BonusFactor = 1 + percentBonus / 100d;
        }
        public override int GetDamage()
        {
            return (int)Math.Round(base.GetDamage() * BonusFactor);
        }
    }
}