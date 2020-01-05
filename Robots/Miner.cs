using Robowars.Weapons;
using System;
using System.Collections.Generic;

namespace Robowars.Robots
{
    /// <summary>
    /// Represents a robot designed to perform mining tasks.
    /// </summary>
    public class Miner : Robot
    {
        /// <summary>The color of the miner, after which it is named.</summary>
        public ConsoleColor Color { get; private set; }
        /// <summary>The day of the miner, after which it is named.</summary>
        public DayOfWeek day { get; private set; }

        /// <summary>
        /// Constructor that can only be called from this Assembly itself; it is not meant to be used directly or publicly.
        /// </summary>
        internal Miner(IEnumerable<Weapon> weapons, ConsoleColor color, DayOfWeek day) : base(weapons)
        {
            Color = color;
            Name = $"Mining Robot({color}-{day})";
        }

        /// <inheritdoc/>
        public override void Report()
        {
            ConsoleColor previous = Console.ForegroundColor;
            Console.ForegroundColor = Color;
            Console.WriteLine($" `{Name}`, Power: {PowerLevel}, status: {Situation.Description} {Situation.Feedback}");
            Console.ForegroundColor = previous;
        }
    }
}