namespace GameFramework.Items
{
    /// <summary>
    /// A concrete attack item such as a sword, axe, bow or magic spell.
    /// Operator <c>+</c> is overloaded to combine two items into a <see cref="CompositeAttackItem"/>.
    /// </summary>
    public class AttackItem : IAttackItem
    {
        /// <inheritdoc/>
        public string Name { get; }

        /// <inheritdoc/>
        public int Hit { get; }

        /// <inheritdoc/>
        public int Range { get; }

        /// <inheritdoc/>
        public double Weight { get; }

        /// <summary>
        /// Initialises a new <see cref="AttackItem"/>.
        /// </summary>
        /// <param name="name">Display name.</param>
        /// <param name="hit">Base hit-point damage.</param>
        /// <param name="range">Attack range in world cells.</param>
        /// <param name="weight">Weight of the item.</param>
        public AttackItem(string name, int hit, int range, double weight = 1.0)
        {
            Name   = name;
            Hit    = hit;
            Range  = range;
            Weight = weight;
        }

        // ── Operator overload (required by assignment) ────────────────────────
        /// <summary>
        /// Combines two <see cref="IAttackItem"/> instances into a <see cref="CompositeAttackItem"/>.
        /// </summary>
        /// <param name="a">First item.</param>
        /// <param name="b">Second item.</param>
        /// <returns>A composite that sums the damage of both items.</returns>
        public static CompositeAttackItem operator +(AttackItem a, AttackItem b) =>
            new CompositeAttackItem($"{a.Name}+{b.Name}") { a, b };

        /// <inheritdoc/>
        public override string ToString() =>
            $"[AttackItem: {Name}, Hit={Hit}, Range={Range}, Weight={Weight}]";
    }

    /// <summary>
    /// A concrete defence item such as a shield, helmet, boots or armour.
    /// </summary>
    public class DefenceItem : IDefenceItem
    {
        /// <inheritdoc/>
        public string Name { get; }

        /// <inheritdoc/>
        public int ReduceHitPoint { get; }

        /// <summary>
        /// Initialises a new <see cref="DefenceItem"/>.
        /// </summary>
        /// <param name="name">Display name.</param>
        /// <param name="reduceHitPoint">Number of hit-points absorbed per incoming blow.</param>
        public DefenceItem(string name, int reduceHitPoint)
        {
            Name           = name;
            ReduceHitPoint = reduceHitPoint;
        }

        /// <inheritdoc/>
        public override string ToString() =>
            $"[DefenceItem: {Name}, Reduce={ReduceHitPoint}]";
    }
}
