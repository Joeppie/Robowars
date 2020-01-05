using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace Robowars
{
    /// <summary>Contains all the information required to organize the Robowars contest</summary>
    public class Contest
    {
        //properties
        Robots.Robot Contestant1 { get; set; }
        Robots.Robot Contestant2 { get; set; }

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
                Console.WriteLine("2: Make Contestant 1 a mining robot.");
                Console.WriteLine("3: Make Contestant 2 a terminator.");
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
                        Contestant1 = MiningFacility.AssembleRobot();
                        break;
                    case ConsoleKey.D3:
                        Contestant2 = TerminatorFacility.AssembleRobot();  
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

        /// <summary>Logic that implements everything a fight between two robots involves</summary>
        public void Fight()
        {
            if (Contestant1 == null || Contestant2 == null)
                throw new ArgumentException("There must be two contestants.");
            Console.WriteLine($"Our contestants are {Contestant1.Name} and {Contestant2.Name}");

            List<Robots.Robot> robots = new List<Robots.Robot> { Contestant1, Contestant2 };

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
}