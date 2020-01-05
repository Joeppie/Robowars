using System;

namespace Robowars.Robots
{
    /// <summary>
    /// Represents a Situation a <see cref="Robot"/> can be in; 
    /// Each method is a transition to a different <see cref="RobotSituation"/>, false is returned if the transition cannot be completed.
    /// </summary>
    public abstract class RobotSituation
    {
        private RobotSituation() { }
        public RobotSituation(Robot context, string feedback = "")
        {
            Context = context;
            Feedback = feedback;
        }

        /// <summary>Returns the <see cref="Robot"/> to which this RobotSituation applies.</summary>
        public Robot Context { get; private set; }

        /// <summary>Attempts a transition to the <see cref="Active"/> Situation</summary>
        /// <returns>True when succesful, false when the <see cref="Robot"/>'s <see cref="RobotSituation"/> was not changed.</returns>
        public abstract bool Ready();

        /// <summary>Attempts a transition to the <see cref="Attacking"/> Situation</summary>
        /// <returns>True when succesful, false when the <see cref="Robot"/>'s <see cref="RobotSituation"/> was not changed.</returns>
        public abstract bool Attack();

        /// <summary>Attempts a transition to the <see cref="Standby"/> Situation</summary>
        /// <returns>True when succesful, false when the <see cref="Robot"/>'s <see cref="RobotSituation"/> was not changed.</returns>
        public abstract bool Deactivate();

        /// <summary>Attempts a transition to the <see cref="Destroyed"/> Situation</summary>
        /// <returns>True when succesful, false when the <see cref="Robot"/>'s <see cref="RobotSituation"/> was not changed.</returns>
        protected abstract bool Destroy(string reason);

        /// <summary>
        /// Handles the logic for a <see cref="Robot"/> taking damage, and transitions the <see cref="Situation"/> to <see cref="Destroyed"/> when appropriate. </summary>
        /// <param name="damage">The amount of damage taken</param>
        public void TakeDamage(int damage)
        {
            Context.PowerLevel = Math.Max(0, Context.PowerLevel - damage); //Prevent negative health.
            if (Context.PowerLevel == 0)
            {
                Destroy($",Damaged beyond repair!!");
            }
        }
        /// <summary>
        /// A text description of what happened.
        /// </summary>
        public string Feedback { get; protected set; }

        /// <summary>
        /// Returns the description for the <see cref="RobotSituation"/>, typically the name of the situation.
        /// </summary>
        public virtual string Description => (this.GetType().Name);
    }
}