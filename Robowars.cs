using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;

namespace Robowars
{
    class Program
    {
        static void Main()
        {
            Contest contest = new Contest();
            contest.ShowMenu();
        }
    }

    /// <summary>Contains all the information required to organize the Robowars contest</summary>
    public class Contest
    {
        //properties
        Robot Contestant1 { get; set; }
        Robot Contestant2 { get; set; }

        public RobotAssembler TerminatorFacility => SkyNet.Instance;

        private MiningRobotMaker _miningRobotMaker = new MiningRobotMaker();
        public RobotAssembler MiningFacility => _miningRobotMaker;

        /// <summary>Shows the status of the two contestants</summary>
        public void ShowStatus()
        {
            Console.Write("Contestant #1: ");
            if (Contestant1 == null)
                Console.WriteLine("not present");
            else
                Contestant1.Report();

            Console.Write("Contestant #2: ");
            if (Contestant2 == null)
                Console.WriteLine("not present");
            else
                Contestant2.Report();
        }

        /// <summary>Shows a menu where the user can select a command to organize and initiate a fight</summary>
        public void ShowMenu()
        {
            Console.WriteLine("Welcome to Robowars, provide input:");


            ConsoleKeyInfo key;
            while (true)
            {
                Console.WriteLine("1: Make Contestant 1 a terminator.");
                Console.WriteLine("2: Make Contestant 2 a terminator.");
                Console.WriteLine("3: Make Contestant 1 a mining robot.");
                Console.WriteLine("4: Make Contestant 2 a mining robot.");
                Console.WriteLine("5: FIGHT robowar");
                Console.WriteLine("6: Exit");

                key = Console.ReadKey();

                switch (key.Key)
                {
                    case ConsoleKey.D1:
                        Contestant1 = TerminatorFacility.AssembleRobot();
                        break;
                    case ConsoleKey.D2:
                        Contestant2 = TerminatorFacility.AssembleRobot();
                        break;
                    case ConsoleKey.D3:
                        Contestant1 = MiningFacility.AssembleRobot();
                        break;
                    case ConsoleKey.D4:
                        Contestant2 = MiningFacility.AssembleRobot();
                        break;
                    case ConsoleKey.D5:
                        Console.Clear();
                        if (Contestant1 == null || Contestant2 == null)
                        {
                            Console.WriteLine("Cannot fight without two contestants.");
                            continue;
                        }
                        Fight();
                        break;
                    case ConsoleKey.Escape:
                    case ConsoleKey.D6:
                        return;
                    default:
                        Console.WriteLine("Command not supported.");
                        Thread.Sleep(500);
                        continue;
                }
                ShowStatus();
            }

        }


        public void Fight()
        {
            if (Contestant1 == null || Contestant2 == null)
                throw new ArgumentException("There must be two contestants.");
            Console.WriteLine($"Our contestants are {Contestant1.Name} and {Contestant2.Name}");

            List<Robot> robots = new List<Robot> { Contestant1, Contestant2 };

            //Initial status report.
            robots.ForEach(r => r.Report());

            for (int i = 1; i <= 10; i++)
            {
                Console.WriteLine($"Round {i}..");
                Thread.Sleep(1000);

                //Reset the robots so they can attack again (if they are still alive!)
                robots.First().ActivateIfPossible();
                robots.Last().ActivateIfPossible();

                //Have the robots attack eachother
                robots.First().Attack(robots.Last());
                robots.Last().Attack(robots.First());

                robots.ForEach(r => r.Report());

                //Check for victory
                if (robots.First().IsDestroyed)
                {
                    Console.WriteLine($"{robots.Last().Name} was victorious after {i} rounds.");
                    break;
                }
                else if (robots.Last().IsDestroyed)
                {
                    Console.WriteLine($"{robots.First().Name} was victorious after {i} rounds.");
                    break;
                }

                //Swap places for fairness.
                robots.Reverse();
                Thread.Sleep(500);
            }

            robots.First().ActivateIfPossible();
            robots.Last().ActivateIfPossible();

            Console.WriteLine("Robowars has concluded.");
        }
    }

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
        public RobotSituation Situation
        {
            get;
            internal set;
        }

        public bool IsDestroyed => Situation is Destroyed;
        public bool IsActive => Situation is Active; //Attacking inherits from Active.
        public bool IsStandby => Situation is Standby;
        public bool IsMoving => Situation is Attacking;

        public string Name { get; protected set; }

        /// <summary>Is both the health and ability to attack for a robot.</summary>
        public int PowerLevel { get; internal set; }

        private List<Weapon> _weapons = new List<Weapon>(); //backing field.
        public List<Weapon> Weapons { get { return _weapons.ToList(); } }

        #endregion

        #region methods
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

        public void ActivateIfPossible()
        {
            Situation.Ready();
        }
    }

    public class Terminator : Robot
    {
        public int Mark { get; private set; }
        internal Terminator(IEnumerable<Weapon> weapons, int mark) : base(weapons)
        {
            Mark = mark;
            Name = $"T{mark} unit";
        }

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

    public class Miner : Robot
    {
        public ConsoleColor Color { get; private set; }
        public DayOfWeek day { get; private set; }
        internal Miner(IEnumerable<Weapon> weapons, ConsoleColor color, DayOfWeek day) : base(weapons)
        {
            Color = color;
            Name = $"Mining Robot({color}-{day})";
        }

        public override void Report()
        {
            ConsoleColor previous = Console.ForegroundColor;
            Console.ForegroundColor = Color;
            Console.WriteLine($" `{Name}`, Power: {PowerLevel}, status: {Situation.Description} {Situation.Feedback}");
            Console.ForegroundColor = previous;
        }
    }


    #endregion

    #region Situation
    /// <summary>
    /// Represents a Situation a robot can have; 
    /// Each method is a transition to a different situation, false is returned if the transition cannot be completed.
    /// </summary>
    public abstract class RobotSituation
    {
        private RobotSituation() { }
        public RobotSituation(Robot context, string feedback = "")
        {
            Context = context;
            Feedback = feedback;
        }
        public Robot Context { get; private set; }
        public abstract bool Ready();
        public abstract bool Attack();
        public abstract bool Deactivate();
        public void TakeDamage(int damage)
        {
            Context.PowerLevel = Math.Max(0, Context.PowerLevel - damage); //Prevent negative health.
            if (Context.PowerLevel == 0)
            {
                Destroy($",Damaged beyond repair!!");
            }
        }
        protected abstract bool Destroy(string reason);
        public string Feedback { get; protected set; }

        public virtual string Description => (this.GetType().Name);
    }


    internal class Active : RobotSituation
    {

        public Active(Robot context, string feedback = "") : base(context, feedback) { }

        public override bool Ready() { Feedback = "Already Activated."; return true; }

        public override bool Deactivate() { Context.Situation = new Standby(Context); return true; }

        protected override bool Destroy(String reason) { Context.Situation = new Destroyed(Context, reason); return true; }

        public override bool Attack() { Context.Situation = new Attacking(Context); return true; }

    }

    internal class Attacking : Active
    {
        public Attacking(Robot context, string feedback = "") : base(context, feedback) { }

        public override bool Ready() { Context.Situation = new Active(Context); return true; }

        public override bool Attack() { Feedback = "Cannot attack twice."; return false; }
    }

    internal class Standby : RobotSituation
    {
        public Standby(Robot context, string feedback = "") : base(context, feedback) { }

        public override bool Ready() { Context.Situation = new Active(Context); return true; }

        public override bool Deactivate() { Feedback = "Already deactivated."; return false; }

        protected override bool Destroy(String reason) { Context.Situation = new Destroyed(Context, reason); return true; }

        public override bool Attack() { Feedback = "Cannot move when in standby"; return true; }

    }

    internal class Destroyed : RobotSituation
    {
        public Destroyed(Robot context, string feedback) : base(context, feedback) { }

        public override bool Ready() { Feedback = "Destroyed, cannot reactivate."; return false; }

        public override bool Deactivate() { Feedback = "Destroyed, cannot deactivate."; return false; }

        public override bool Attack() { Feedback = "Destroyed, cannot attack."; return false; }

        protected override bool Destroy(string reason) { Feedback = "Already Destroyed."; return false; }
    }
    #endregion




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

    public class Laser : Weapon
    {
        public Laser()
        {
            Damage = new DamageRating(20, 30);
            Name = "Basic Laser";
        }

        public override int PowerDrain => 5;
    }

    public class Hammer : Weapon
    {
        public Hammer()
        {
            Damage = new DamageRating(10, 30);
            Name = "Construction Hammer";
        }
        public override int PowerDrain => 2;
    }

    /// <summary>Represents an enchancement of a <see cref="Weapon"/>, by transparently wrapping the weapon while also implementing its interface.</summary>
    public abstract class WeaponEnhancement : Weapon
    {
        public Weapon EnhancedWeapon { get; set; }

        public override int GetDamage()
        {
            return EnhancedWeapon.GetDamage();
        }
    }

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

    /// <summary>Represents logic regarding how specific robots may be produced. </summary>
    public abstract class RobotAssembler
    {
        /// <summary>Creates a new robot.</summary>
        public Robot AssembleRobot()
        {
            return CreateRobot();
        }
        /// <summary>Creates weaponry for the robot.</summary>
        protected abstract Robot CreateRobot();
    }


    /// <summary>Creates terminator robots</summary>
    public class SkyNet : RobotAssembler
    {
        private static readonly SkyNet _instance = new SkyNet(); //There is only one SkyNet.
        const int start = 2000;
        int _mark = start;

        private SkyNet() { } //Do not allow direct instantiation.

        public static SkyNet Instance => _instance;

        protected override Robot CreateRobot()
        {
            int upgrades = _mark - start; //Every successive generation of terminator is more powerful.

            var turboLaser = WeaponUpgradeSpecifier.FromWeapon(new Laser()).UpgradeByPercentage(20+ upgrades).Named("TurboLaser").Build();
            var laser = WeaponUpgradeSpecifier.FromWeapon(new Laser()).UpgradeByAmount(upgrades).Named("LaserShotgun").Build();

            return new Terminator(new List<Weapon>() { turboLaser, laser },_mark++);
        }
    }


    /// <summary>Creates friendly mining robots</summary>
    public class MiningRobotMaker : RobotAssembler
    {
        protected override Robot CreateRobot()
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