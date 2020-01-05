using Robowars.Weapons;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Robowars.Robots
{
    /// <summary>An interface and abstract implementation for basic Robot functionality</summary>
    public abstract class Robot
    {
        #region constructors
        internal Robot(IEnumerable<Weapon> weapons)
        {
            _weapons = weapons.ToList();

            Situation = new Standby(this);
            PowerLevel = 100;
        }
        #endregion

        #region properties
        /// <summary>Returns the actual status of the <see cref="Robot"/></summary>
        public RobotSituation Situation
        {
            get;
            internal set;
        }

        /// <summary>Whether the <see cref="Robot"/ >is <see cref="Destroyed"/>.</summary>
        public bool IsDestroyed => Situation is Destroyed;

        /// <summary>Whether the <see cref="Robot"/ >is <see cref="Active"/>.</summary>
        /// <remarks><see cref="Attacking"/> is a sub-state of <see cref="Active"/></remarks>
        public bool IsActive => Situation is Active;

        /// <summary>Whether the <see cref="Robot"/ >is <see cref="Standby"/>.</summary>
        public bool IsStandby => Situation is Standby;

        /// <summary>Whether the <see cref="Robot"/ >is <see cref="Attacking"/>.</summary>
        public bool IsMoving => Situation is Attacking;

        /// <summary>Returns the Name of this Robot.</summary>
        public string Name { get; protected set; }

        /// <summary>Is both the health and ability to attack for a robot.</summary>
        public int PowerLevel { get; internal set; }

        private List<Weapon> _weapons = new List<Weapon>(); //backing field.

        /// <summary>Returns a list containing all the <see cref="Weapon"/>s of this Robot.</summary>
        public List<Weapon> Weapons { get { return _weapons.ToList(); } }

        #endregion

        /// <summary>Instructs the robot to take damage for a certain reason</summary>
        public void TakeDamage(int damage, string description)
        {
            Console.WriteLine($"{Name} {description}.");
            Situation.TakeDamage(damage);
        }

        /// <summary>Writes the status of the robot to the console.</summary>
        public abstract void Report();

        /// <summary>Robot attacks the other robot with a random weapon, if the situation allows.</summary>
        public void Attack(Robot other)
        {
            if (Situation.Attack() && _weapons.Any())
            {
                var weapon = _weapons.ElementAt(new Random().Next(_weapons.Count()));

                this.PowerLevel -= weapon.PowerDrain;
                int damage = weapon.GetDamage();
                other.TakeDamage(damage, $", sustained {damage} damage from a {weapon.Name}.");

                if (this.PowerLevel <= 0)
                    this.Situation.Deactivate(); //No power left.
            }
        }

        /// <summary>Attempts to restore the robot back to its <see cref="Active"/> situation, if possible.</summary>
        public void ActivateIfPossible()
        {
            Situation.Ready();
        }
    }
}