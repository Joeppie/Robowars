using Robowars.Robots;
using Robowars.Weapons;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Robowars
{
    /// <summary>Creates friendly mining robots</summary>
    public class MiningRobotMaker : RobotAssembler
    {
        /// <returns>A new <see cref="Miner"/> instance</returns>
        protected override Robots.Robot CreateRobot()
        {
            Array colors = Enum.GetValues(typeof(ConsoleColor));
            ConsoleColor color = (ConsoleColor)colors.GetValue(new Random().Next(colors.Length));
            color = (color == ConsoleColor.Black) ? ConsoleColor.White : color;

            Array days = Enum.GetValues(typeof(DayOfWeek));
            DayOfWeek day = (DayOfWeek)days.GetValue(new Random().Next(days.Length));

            return new Miner(new List<Weapon>() { new Hammer(), new Hammer(), new Laser() }, color, day);
        }
    }
}