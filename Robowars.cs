using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;

namespace Robowars
{
    class Robowars
    {
        static void Main(string[] args)
        {
            new Contest().ShowMenu();
        }

       
    }


    public class Contest
    {

        Robot Contestant1 { get; set; }
        Robot Contestant2 { get; set; }

        public RobotAssembler TerminatorFacility => SkyNet.Instance;

        private MiningRobotMaker _miningRobotMaker = new MiningRobotMaker();
        public RobotAssembler MiningFacility  => _miningRobotMaker;


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


        public void ShowMenu()
        {
            Console.WriteLine("Welcome to Robowars, provide input:");


            ConsoleKeyInfo key;
            do
            {

                Console.WriteLine("1: Make Contestant 1 a terminator.");
                Console.WriteLine("2: Make Contestant 2 a terminator.");
                Console.WriteLine("3: Make Contestant 2 a mining robot.");
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
                        if(Contestant1 == null || Contestant2 == null)
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
            while (key.KeyChar != 6);
        }


        void Fight()
        {
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


    public class Robot
    {
        #region constructors
        internal Robot(string name, IEnumerable<Weapon> weapons)
        {
            Name = name;
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

        public string Name { get; private set; }

        /// <summary>Is both the health and ability to attack for a robot.</summary>
        public int PowerLevel { get; internal set; }

        private List<Weapon> _weapons = new List<Weapon>(); //backing field.
        public IEnumerable<Weapon> Weapons { get { return _weapons.ToList(); } }

        #endregion

        #region methods
        public void TakeDamage(int damage, string description)
        {
            Console.WriteLine($"{Name} {description}.");
            Situation.TakeDamage(damage);
        }


        public void Report()
        {
            Console.WriteLine($" `{Name}`, Power: {PowerLevel}, Situation: {Situation.Description} {Situation.Feedback}");
        }


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
            public String Feedback { get; protected set; }

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


    }


    public abstract class Weapon
    {
        public abstract int PowerDrain { get; }

        public string Name { get; set; }

        protected internal DamageRating Damage { get; protected set; }

        virtual public int GetDamage()
        {
            return Damage.Get();
        }

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


    public abstract class WeaponEnhancement : Weapon
    {

        public Weapon enhancedWeapon { get; protected set; }

        public override int GetDamage()
        {
            return enhancedWeapon.GetDamage();
        }
    }

    public class FixedDamageBonusWeapon : WeaponEnhancement
    {
        public int Bonus { get; private set; }

        public override int PowerDrain => enhancedWeapon.PowerDrain + Bonus / 2;

        public FixedDamageBonusWeapon(Weapon weapontToEnhance, int bonus)
        {
            enhancedWeapon = weapontToEnhance;
            Bonus = bonus;
        }
        public override int GetDamage()
        {
            return enhancedWeapon.GetDamage() + Bonus;
        }
    }

    public class PercentageBonusDamageWeapon : WeaponEnhancement
    {
        public double BonusFactor { get; private set; }

        public override int PowerDrain => (int)Math.Round(enhancedWeapon.PowerDrain * Math.Pow(BonusFactor, 1.2));

        public PercentageBonusDamageWeapon(Weapon weapontToEnhance, int percentBonus)
        {
            enhancedWeapon = weapontToEnhance;
            BonusFactor = 1 + percentBonus / 100d;
        }
        public override int GetDamage()
        {
            return (int)Math.Round(base.GetDamage() * BonusFactor);
        }
    }



    /// <summary>Specifies how a weapon should be upgraded and named.</summary>
    public class WeaponUpgradeSpecifier
    {
        private Weapon _weapon { get;  set; }
        private string _name {  get;  set; }

        private WeaponUpgradeSpecifier(Weapon weapon)
        {
            _weapon = Activator.CreateInstance(weapon.GetType()) as Weapon;
            //Create a fresh version of the weapon.
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


    public abstract class RobotAssembler
    {
        public Robot AssembleRobot()
        {
            Robot robot = new Robot(Name(), CreateWeapons());
            return robot;
        }

        protected abstract string Name();

        protected abstract List<Weapon> CreateWeapons();
    }


    public class SkyNet : RobotAssembler
    {
        static SkyNet _instance = new SkyNet();
        int _mark = 2000;

        private SkyNet(){} //Do not allow direct instantiation.

        public static SkyNet Instance { get { return _instance; } }

        protected override List<Weapon> CreateWeapons()
        {
            var turboLaser = WeaponUpgradeSpecifier.FromWeapon(new Laser()).UpgradeByPercentage(50).Named("TurboLaser").Build();
            var laser = WeaponUpgradeSpecifier.FromWeapon(new Laser()).UpgradeByAmount(5).Named("LaserShotgun").Build();

            return new List<Weapon>()
            {
                turboLaser,
                laser
            };
        }

        protected override string Name()
        {
            return $"T{_mark++} terminator";
        }
    }

    public class MiningRobotMaker : RobotAssembler
    {
        protected override List<Weapon> CreateWeapons()
        {
            return new List<Weapon>() { new Hammer(), new Hammer(), new Laser() };
        }

        protected override string Name()
        {
            Array days = Enum.GetValues(typeof(DayOfWeek));
            string day = days.GetValue(new Random().Next(days.Length)).ToString();

            Array colors = Enum.GetValues(typeof(System.Drawing.KnownColor));
            string color = colors.GetValue(new Random().Next(colors.Length)).ToString();

            return $"MiningRobot({color}-{day})";
        }
    }
}
