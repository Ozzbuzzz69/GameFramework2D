namespace GameFramework.Items
{
    /// <summary>
    /// Defines the contract for all attack items a creature can carry.
    /// </summary>
    public interface IAttackItem
    {
        /// <summary>Gets the display name of the item.</summary>
        string Name { get; }

        /// <summary>Gets the base hit-point damage this item deals.</summary>
        int Hit { get; }

        /// <summary>Gets the effective attack range (in world cells).</summary>
        int Range { get; }

        /// <summary>Gets the weight of the item (affects carry capacity).</summary>
        double Weight { get; }
    }

    /// <summary>
    /// Defines the contract for all defence items a creature can equip.
    /// </summary>
    public interface IDefenceItem
    {
        /// <summary>Gets the display name of the item.</summary>
        string Name { get; }

        /// <summary>Gets the number of hit-points reduced when receiving a blow.</summary>
        int ReduceHitPoint { get; }
    }
}
