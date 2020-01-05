using System;

namespace Robowars.Weapons
{

    /// <summary>Defines a top level interface for a weapon.</summary>
    public abstract class Weapon
    {
        /// <summary>Using the weapon drains this amount of power.</summary>
        public abstract int PowerDrain { get; }

        public string Name { get; set; }

        protected internal DamageRating Damage { get; protected set; }

        virtual public int GetDamage()
        {
            return Damage.Get();
        }

        /// <summary>Defines a random damage between <see cref="Min"/> and <see cref="Max"/>.</summary>
        public class DamageRating
        {
            private Random random = new Random();

            public DamageRating(int min, int max)
            {
                if (min > max || min < 0 || max < 0)
                    throw new ArgumentException("min must be smaller than max, and both larger than zero.");
                Min = min;
                Max = max;
            }

            public int Min { get; private set; }
            public int Max { get; private set; }

            /// <summary>Gets a new random damage between <see cref="Min"/> and <see cref="Max"/>.</summary>
            internal int Get()
            {
                return random.Next(Min, Max);
            }
        }
    }
}