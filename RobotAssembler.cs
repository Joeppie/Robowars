using Robowars.Robots;

namespace Robowars
{
    /// <summary>Represents logic regarding how specific robots may be produced. </summary>
    public abstract class RobotAssembler
    {
        /// <summary>Creates a new robot.</summary>
        public Robots.Robot AssembleRobot()
        {
            return CreateRobot();
        }
        /// <summary>Creates weaponry for the robot.</summary>
        protected abstract Robots.Robot CreateRobot();
    }
}