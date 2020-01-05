using Robowars.Weapons;
using System;
using System.Collections.Generic;

namespace Robowars.Robots
{
    /// <summary>Represents a killer-robot designed to 'terminate' others</summary>
    public class Terminator : Robot
    {
        /// <summary>The build number of the Terminator, the higher the stronger the robot tends to be.</summary>
        public int Mark { get; private set; }

        /// <summary>
        /// Constructor that can only be called from this Assembly itself; it is not meant to be used directly or publicly.
        /// </summary>
        internal Terminator(IEnumerable<Weapon> weapons, int mark) : base(weapons)
        {
            Mark = mark;
            Name = $"T{mark} unit";
        }

        /// <inheritdoc/>
        public override void Report()
        {
            ConsoleColor previousForeground = Console.ForegroundColor;
            ConsoleColor previousBackground = Console.BackgroundColor;
            Console.ForegroundColor = ConsoleColor.Red;
            Console.BackgroundColor = ConsoleColor.DarkRed;
            Console.WriteLine($" `{Name}`, Power: {PowerLevel}, objective: {Situation.Description} {Situation.Feedback}");

            Console.ForegroundColor = previousForeground;
            Console.BackgroundColor = previousBackground;
        }
    }
}