using GameFramework.Creatures;

namespace GameDemo
{
    /// <summary>
    /// A simple console-printing observer that tracks creature events.
    /// Registered via Creature.AddObserver() in Program.cs.
    /// </summary>
    public class BattleLogger : ICreatureObserver
    {
        public void OnHit(Creature creature, int damageDealt)
        {
            Console.WriteLine($"  [Observer] {creature.Name} took {damageDealt} damage. HP remaining: {creature.HitPoints}");
        }

        public void OnDeath(Creature creature)
        {
            Console.WriteLine($"  [Observer] *** {creature.Name} has been SLAIN! ***");
        }
    }
}
