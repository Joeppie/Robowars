namespace Robowars.Weapons
{
    public class Laser : Weapon
    {
        public Laser()
        {
            Damage = new DamageRating(20, 30);
            Name = "Basic Laser";
        }

        public override int PowerDrain => 5;
    }
}