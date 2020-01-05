namespace Robowars.Robots
{
    /// <summary>Indicates that the <see cref="Robot"/> is currently <see cref="Attacking"/>.</summary>
    internal class Attacking : Active
    {
        public Attacking(Robot context, string feedback = "") : base(context, feedback) { }

        public override bool Ready() { Context.Situation = new Active(Context); return true; }

        public override bool Attack() { Feedback = "Cannot attack twice."; return false; }
    }
}