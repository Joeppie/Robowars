using System;

namespace Robowars.Robots
{

    /// <summary>Indicates that the <see cref="Robot"/> is <see cref="Active"/> and is ready to perform actions.</summary>
    internal class Active : RobotSituation
    {

        public Active(Robot context, string feedback = "") : base(context, feedback) { }

        public override bool Ready() { Feedback = "Already Activated."; return true; }

        public override bool Deactivate() { Context.Situation = new Standby(Context); return true; }

        protected override bool Destroy(String reason) { Context.Situation = new Destroyed(Context, reason); return true; }

        public override bool Attack() { Context.Situation = new Attacking(Context); return true; }

    }
}