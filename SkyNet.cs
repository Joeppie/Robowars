using Robowars.Robots;
using Robowars.Weapons;
using System.Collections.Generic;

namespace Robowars
{
    /// <summary>Creates terminator robots</summary>
    public class SkyNet : RobotAssembler
    {
        private static readonly SkyNet _instance = new SkyNet(); //There is only one SkyNet.
        const int start = 2000;
        int _mark = start;

        private SkyNet() { } //Do not allow direct instantiation.

        public static SkyNet Instance => _instance;

        /// <returns>A new <see cref="Terminator"/> instance</returns>
        protected override Robots.Robot CreateRobot()
        {
            int upgrades = _mark - start; //Every successive generation of terminator is more powerful.

            var turboLaser = WeaponUpgradeSpecifier.FromWeapon(new Laser()).UpgradeByPercentage(20+ upgrades).Named("TurboLaser").Build();
            var laser = WeaponUpgradeSpecifier.FromWeapon(new Laser()).UpgradeByAmount(upgrades).Named("LaserShotgun").Build();

            return new Terminator(new List<Weapon>() { turboLaser, laser },_mark++);
        }
    }
}