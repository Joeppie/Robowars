namespace Robowars.Weapons
{
    public class Hammer : Weapon
    {
        public Hammer()
        {
            Damage = new DamageRating(10, 30);
            Name = "Construction Hammer";
        }
        public override int PowerDrain => 2;
    }
}