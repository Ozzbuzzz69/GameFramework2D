namespace GameFramework.Creatures
{
    // ═══════════════════════════════════════════════════════════════════════════
    // OBSERVER PATTERN
    // ICreatureObserver is the subscriber interface.  Any object that wants to
    // react to creature events implements this interface and registers itself
    // with Creature.AddObserver().
    // ═══════════════════════════════════════════════════════════════════════════

    /// <summary>
    /// Observer interface for creature lifecycle events.
    /// Register instances via <see cref="Creature.AddObserver"/>.
    /// </summary>
    public interface ICreatureObserver
    {
        /// <summary>Called every time the observed creature receives a hit.</summary>
        /// <param name="creature">The creature that was hit.</param>
        /// <param name="damageDealt">The final damage that was applied (after defence reduction).</param>
        void OnHit(Creature creature, int damageDealt);

        /// <summary>Called once when the observed creature's hit-points reach zero or below.</summary>
        /// <param name="creature">The creature that died.</param>
        void OnDeath(Creature creature);
    }
}
