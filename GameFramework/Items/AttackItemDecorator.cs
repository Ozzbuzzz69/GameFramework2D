namespace GameFramework.Items
{
    // ═══════════════════════════════════════════════════════════════════════════
    // DECORATOR PATTERN
    // AttackItemDecorator wraps any IAttackItem and delegates to it, allowing
    // subclasses to selectively override properties without changing the original.
    // ═══════════════════════════════════════════════════════════════════════════

    /// <summary>
    /// Abstract base decorator for <see cref="IAttackItem"/>.
    /// Subclass this to add boosting or weakening effects without modifying
    /// the original item (Open/Closed Principle).
    /// </summary>
    public abstract class AttackItemDecorator : IAttackItem
    {
        /// <summary>The wrapped attack item.</summary>
        protected readonly IAttackItem _inner;

        /// <summary>Initialises the decorator with an inner item to wrap.</summary>
        /// <param name="inner">The item being decorated.</param>
        protected AttackItemDecorator(IAttackItem inner)
        {
            ArgumentNullException.ThrowIfNull(inner);
            _inner = inner;
        }

        /// <inheritdoc/>
        public virtual string Name   => _inner.Name;
        /// <inheritdoc/>
        public virtual int    Hit    => _inner.Hit;
        /// <inheritdoc/>
        public virtual int    Range  => _inner.Range;
        /// <inheritdoc/>
        public virtual double Weight => _inner.Weight;
    }

    /// <summary>
    /// Decorator that boosts the hit damage of an attack item by a multiplier.
    /// </summary>
    /// <example>
    /// <code>
    /// IAttackItem sword = new AttackItem("Sword", 10, 1);
    /// IAttackItem enchanted = new BoostedAttackDecorator(sword, 1.5);  // +50% damage
    /// </code>
    /// </example>
    public class BoostedAttackDecorator : AttackItemDecorator
    {
        private readonly double _multiplier;

        /// <summary>
        /// Initialises a boost decorator.
        /// </summary>
        /// <param name="inner">The item to boost.</param>
        /// <param name="multiplier">Damage multiplier (e.g. 1.5 = 50 % bonus).</param>
        public BoostedAttackDecorator(IAttackItem inner, double multiplier = 1.5)
            : base(inner)
        {
            _multiplier = multiplier;
        }

        /// <inheritdoc/>
        public override string Name => $"{_inner.Name} (Boosted)";

        /// <inheritdoc/>
        public override int Hit => (int)(_inner.Hit * _multiplier);
    }

    /// <summary>
    /// Decorator that weakens the hit damage of an attack item (e.g. cursed weapon).
    /// </summary>
    public class WeakenedAttackDecorator : AttackItemDecorator
    {
        private readonly double _factor;

        /// <summary>
        /// Initialises a weakening decorator.
        /// </summary>
        /// <param name="inner">The item to weaken.</param>
        /// <param name="factor">Reduction factor between 0 and 1 (e.g. 0.5 = half damage).</param>
        public WeakenedAttackDecorator(IAttackItem inner, double factor = 0.5)
            : base(inner)
        {
            _factor = Math.Clamp(factor, 0.0, 1.0);
        }

        /// <inheritdoc/>
        public override string Name => $"{_inner.Name} (Weakened)";

        /// <inheritdoc/>
        public override int Hit => (int)(_inner.Hit * _factor);
    }
}
