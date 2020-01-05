using System;

namespace Robowars.Weapons
{
    /// <summary>Used to create weapons so no detail knowledge is required about bonuses.</summary>
    public class WeaponUpgradeSpecifier
    {
        private Weapon _weapon { get; set; }
        private string _name { get; set; }

        private WeaponUpgradeSpecifier(Weapon weapon)
        {
            _weapon = weapon;
        }

        public static WeaponUpgradeSpecifier FromWeapon(Weapon baseWeapon)
        {
            return new WeaponUpgradeSpecifier(baseWeapon);
        }

        public WeaponUpgradeSpecifier UpgradeByPercentage(int percentBonus)
        {
            if (_weapon == null)
                throw new InvalidOperationException("Must first have a weapon to upgrade.");
            _weapon = new PercentageBonusDamageWeapon(_weapon, percentBonus);
            return this;
        }

        public WeaponUpgradeSpecifier UpgradeByAmount(int bonus)
        {
            if (_weapon == null)
                throw new InvalidOperationException("Must first have a weapon to upgrade.");
            _weapon = new FixedDamageBonusWeapon(_weapon, bonus);
            return this;
        }

        public WeaponUpgradeSpecifier Named(string name)
        {
            _name = name;
            return this;
        }

        public Weapon Build()
        {
            _weapon.Name = _name;
            return _weapon;
        }
    }
}