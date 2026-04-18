namespace GameFramework.Strategies
{
    // ═══════════════════════════════════════════════════════════════════════════
    // STRATEGY PATTERN
    // IHitStrategy defines how a creature calculates its outgoing hit value.
    // Different strategies can be swapped in at runtime (e.g. aggressive,
    // defensive or random) without touching the Creature class.
    // ═══════════════════════════════════════════════════════════════════════════

    /// <summary>
    /// Strategy interface for computing the total hit damage a creature delivers.
    /// </summary>
    public interface IHitStrategy
    {
        /// <summary>
        /// Calculates the hit damage from a collection of attack items.
        /// </summary>
        /// <param name="attackItems">The creature's equipped attack items.</param>
        /// <returns>The final damage value (≥ 0).</returns>
        int CalculateHit(IEnumerable<Items.IAttackItem> attackItems);
    }

    /// <summary>
    /// Default strategy: sums the <see cref="Items.IAttackItem.Hit"/> of all carried items.
    /// </summary>
    public class SumHitStrategy : IHitStrategy
    {
        /// <inheritdoc/>
        public int CalculateHit(IEnumerable<Items.IAttackItem> attackItems) =>
            attackItems.Sum(a => a.Hit);
    }

    /// <summary>
    /// Aggressive strategy: sums all items but adds a fixed bonus.
    /// </summary>
    public class AggressiveHitStrategy : IHitStrategy
    {
        private readonly int _bonus;

        /// <summary>Creates an aggressive strategy with the given damage bonus.</summary>
        /// <param name="bonus">Extra hit-points added on top of normal damage.</param>
        public AggressiveHitStrategy(int bonus = 5) => _bonus = bonus;

        /// <inheritdoc/>
        public int CalculateHit(IEnumerable<Items.IAttackItem> attackItems) =>
            attackItems.Sum(a => a.Hit) + _bonus;
    }

    /// <summary>
    /// Random strategy: each item's damage is multiplied by a random factor [0.5, 1.5].
    /// </summary>
    public class RandomHitStrategy : IHitStrategy
    {
        private static readonly Random _rng = new();

        /// <inheritdoc/>
        public int CalculateHit(IEnumerable<Items.IAttackItem> attackItems)
        {
            double total = 0;
            foreach (var item in attackItems)
                total += item.Hit * (_rng.NextDouble() + 0.5);  // factor 0.5 – 1.5
            return (int)total;
        }
    }
}
