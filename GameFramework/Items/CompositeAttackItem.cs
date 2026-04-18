using System.Collections;

namespace GameFramework.Items
{
    // ═══════════════════════════════════════════════════════════════════════════
    // COMPOSITE PATTERN
    // CompositeAttackItem treats a collection of IAttackItem instances as a
    // single IAttackItem whose Hit/Weight are the sums of its children.
    // The creature's max-weight check can therefore be done on the composite.
    // ═══════════════════════════════════════════════════════════════════════════

    /// <summary>
    /// Composite attack item that aggregates a collection of <see cref="IAttackItem"/>
    /// instances.  <see cref="Hit"/> and <see cref="Weight"/> return the sums of all
    /// children; <see cref="Range"/> returns the maximum child range.
    /// <para>
    /// Implements <see cref="IEnumerable{T}"/> so collection-initialiser syntax works:
    /// </para>
    /// <code>
    /// var combo = new CompositeAttackItem("Battle Kit") { sword, axe, magicStaff };
    /// </code>
    /// </summary>
    public class CompositeAttackItem : IAttackItem, IEnumerable<IAttackItem>
    {
        private readonly List<IAttackItem> _items = new();

        /// <inheritdoc/>
        public string Name { get; }

        /// <summary>
        /// Returns the total hit damage (sum of all children).
        /// </summary>
        public int Hit => _items.Sum(i => i.Hit);

        /// <summary>
        /// Returns the maximum range across all children.
        /// </summary>
        public int Range => _items.Count > 0 ? _items.Max(i => i.Range) : 0;

        /// <summary>
        /// Returns the total weight (sum of all children).
        /// Used to validate a creature's <see cref="Creatures.Creature.MaxCarryWeight"/>.
        /// </summary>
        public double Weight => _items.Sum(i => i.Weight);

        /// <summary>Gets the number of items in this composite.</summary>
        public int Count => _items.Count;

        /// <summary>Creates a new composite with the given display name.</summary>
        /// <param name="name">Display name for the group (e.g. "Battle Kit").</param>
        public CompositeAttackItem(string name)
        {
            Name = name;
        }

        /// <summary>Adds an attack item to the composite.</summary>
        /// <param name="item">Item to add.</param>
        public void Add(IAttackItem item)
        {
            ArgumentNullException.ThrowIfNull(item);
            _items.Add(item);
        }

        /// <summary>Removes an attack item from the composite.</summary>
        /// <param name="item">Item to remove.</param>
        /// <returns><c>true</c> if the item was found and removed.</returns>
        public bool Remove(IAttackItem item) => _items.Remove(item);

        /// <inheritdoc/>
        public IEnumerator<IAttackItem> GetEnumerator() => _items.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        /// <inheritdoc/>
        public override string ToString() =>
            $"[Composite: {Name}, Items={Count}, TotalHit={Hit}, TotalWeight={Weight}]";
    }
}
