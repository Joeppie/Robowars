namespace Robowars.Robots
{
    /// <summary>Indicates that the <see cref="Robot"/> has been <see cref="Destroyed"/>.</summary>
    internal class Destroyed : RobotSituation
    {
        public Destroyed(Robot context, string feedback) : base(context, feedback) { }

        public override bool Ready() { Feedback = "Destroyed, cannot reactivate."; return false; }

        public override bool Deactivate() { Feedback = "Destroyed, cannot deactivate."; return false; }

        public override bool Attack() { Feedback = "Destroyed, cannot attack."; return false; }

        protected override bool Destroy(string reason) { Feedback = "Already Destroyed."; return false; }
    }
}