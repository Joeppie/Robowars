using System;

namespace Robowars.Robots
{
    /// <summary>Indicates that the <see cref="Robot"/> is inactive, on <see cref="Standby"/>.</summary>
    internal class Standby : RobotSituation
    {
        public Standby(Robot context, string feedback = "") : base(context, feedback) { }

        public override bool Ready() { Context.Situation = new Active(Context); return true; }

        public override bool Deactivate() { Feedback = "Already deactivated."; return false; }

        protected override bool Destroy(String reason) { Context.Situation = new Destroyed(Context, reason); return true; }

        public override bool Attack() { Feedback = "Cannot move when in standby"; return true; }

    }
}