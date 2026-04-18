namespace GameFramework.World
{
    /// <summary>
    /// Represents an (X, Y) coordinate in the 2D world grid.
    /// </summary>
    public struct Position : IEquatable<Position>
    {
        /// <summary>Gets the X coordinate.</summary>
        public int X { get; }

        /// <summary>Gets the Y coordinate.</summary>
        public int Y { get; }

        /// <summary>Initialises a new position.</summary>
        /// <param name="x">X coordinate.</param>
        /// <param name="y">Y coordinate.</param>
        public Position(int x, int y) { X = x; Y = y; }

        /// <summary>Returns a new position offset by (dx, dy).</summary>
        public Position Translate(int dx, int dy) => new(X + dx, Y + dy);

        /// <inheritdoc/>
        public bool Equals(Position other) => X == other.X && Y == other.Y;

        /// <inheritdoc/>
        public override bool Equals(object? obj) => obj is Position p && Equals(p);

        /// <inheritdoc/>
        public override int GetHashCode() => HashCode.Combine(X, Y);

        /// <summary>Equality operator for two positions.</summary>
        public static bool operator ==(Position a, Position b) => a.Equals(b);

        /// <summary>Inequality operator for two positions.</summary>
        public static bool operator !=(Position a, Position b) => !a.Equals(b);

        /// <inheritdoc/>
        public override string ToString() => $"({X}, {Y})";
    }
}

namespace GameFramework.WorldObjects
{
    using GameFramework.Items;
    using GameFramework.Logging;
    using GameFramework.World;

    /// <summary>
    /// Base class for all fixed or removable objects placed in the game world,
    /// such as walls, treasure chests or potions.
    /// </summary>
    public abstract class WorldObject
    {
        private readonly List<object> _lootItems = new();
        private bool _removed;

        /// <summary>Gets the display name of this world object.</summary>
        public string Name { get; }

        /// <summary>Gets the current world position of this object.</summary>
        public Position Position { get; set; }

        /// <summary>
        /// Gets whether creatures can loot items from this object.
        /// </summary>
        public bool Lootable { get; protected set; }

        /// <summary>
        /// Gets whether this object can be removed from the world once looted.
        /// </summary>
        public bool Removable { get; protected set; }

        /// <summary>Gets whether the object has been removed from the world.</summary>
        public bool IsRemoved => _removed;

        /// <summary>
        /// Initialises a world object at the given position.
        /// </summary>
        /// <param name="name">Display name.</param>
        /// <param name="position">Starting position.</param>
        /// <param name="lootable">Whether the object can be looted.</param>
        /// <param name="removable">Whether the object disappears after being looted.</param>
        protected WorldObject(string name, Position position, bool lootable = true, bool removable = true)
        {
            Name     = name;
            Position = position;
            Lootable = lootable;
            Removable = removable;
        }

        /// <summary>
        /// Returns the items available for looting (attack and defence items).
        /// </summary>
        public IReadOnlyList<object> GetLootableItems() => _lootItems.AsReadOnly();

        /// <summary>
        /// Adds an item to this object's loot pool.
        /// </summary>
        /// <param name="item">An <see cref="IAttackItem"/> or <see cref="IDefenceItem"/>.</param>
        public void AddLootItem(object item) => _lootItems.Add(item);

        /// <summary>
        /// Marks this object as removed from the world.
        /// </summary>
        public void Remove()
        {
            _removed = true;
            MyLogger.Instance.Log($"WorldObject '{Name}' removed from world.");
        }
    }

    // ── Concrete world objects ────────────────────────────────────────────────

    /// <summary>A treasure chest that can be looted for items.</summary>
    public class TreasureChest : WorldObject
    {
        /// <summary>Creates a treasure chest at the given position.</summary>
        public TreasureChest(string name, Position position)
            : base(name, position, lootable: true, removable: true) { }
    }

    /// <summary>An impassable wall block.</summary>
    public class Wall : WorldObject
    {
        /// <summary>Creates a wall at the given position.</summary>
        public Wall(string name, Position position)
            : base(name, position, lootable: false, removable: false) { }
    }

    /// <summary>A bonus box that grants items when looted, then disappears.</summary>
    public class BonusBox : WorldObject
    {
        /// <summary>Creates a bonus box at the given position.</summary>
        public BonusBox(string name, Position position)
            : base(name, position, lootable: true, removable: true) { }
    }
}
