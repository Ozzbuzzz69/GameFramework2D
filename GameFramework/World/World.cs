using GameFramework.Creatures;
using GameFramework.Logging;
using GameFramework.WorldObjects;

namespace GameFramework.World
{
    /// <summary>
    /// Represents the 2D game world.  Holds all <see cref="Creature"/> and
    /// <see cref="WorldObject"/> instances and enforces boundary rules.
    /// </summary>
    /// <remarks>
    /// World size is loaded from <see cref="Configuration.GameConfig"/>:
    /// <code>
    /// var config = GameConfig.Load("game.xml");
    /// var world  = new World(config.MaxX, config.MaxY);
    /// </code>
    /// </remarks>
    public class World
    {
        private readonly List<Creature>     _creatures    = new();
        private readonly List<WorldObject>  _worldObjects = new();

        // ── Construction ─────────────────────────────────────────────────────

        /// <summary>
        /// Initialises the world with the given dimensions.
        /// </summary>
        /// <param name="maxX">Width of the world grid (columns, 1-based).</param>
        /// <param name="maxY">Height of the world grid (rows, 1-based).</param>
        public World(int maxX, int maxY)
        {
            if (maxX <= 0 || maxY <= 0)
                throw new ArgumentOutOfRangeException("World dimensions must be positive.");

            MaxX = maxX;
            MaxY = maxY;
            MyLogger.Instance.Log($"World created: {MaxX}x{MaxY}.");
        }

        // ── Properties ───────────────────────────────────────────────────────

        /// <summary>Gets the maximum X boundary of the world.</summary>
        public int MaxX { get; }

        /// <summary>Gets the maximum Y boundary of the world.</summary>
        public int MaxY { get; }

        /// <summary>Read-only view of all creatures currently in the world.</summary>
        public IReadOnlyList<Creature> Creatures => _creatures.AsReadOnly();

        /// <summary>Read-only view of all world objects currently in the world.</summary>
        public IReadOnlyList<WorldObject> WorldObjects => _worldObjects.AsReadOnly();

        // ── Creature management ───────────────────────────────────────────────

        /// <summary>Adds a creature to the world at its current position.</summary>
        /// <param name="creature">The creature to add.</param>
        /// <exception cref="ArgumentException">Thrown if the position is out of bounds.</exception>
        public void AddCreature(Creature creature)
        {
            ArgumentNullException.ThrowIfNull(creature);
            ValidatePosition(creature.Position);
            _creatures.Add(creature);
            MyLogger.Instance.Log($"Creature '{creature.Name}' added at {creature.Position}.");
        }

        /// <summary>Removes a creature from the world (e.g. after death).</summary>
        /// <param name="creature">The creature to remove.</param>
        public void RemoveCreature(Creature creature) => _creatures.Remove(creature);

        // ── World-object management ───────────────────────────────────────────

        /// <summary>Places a world object into the world.</summary>
        /// <param name="obj">The object to place.</param>
        public void AddWorldObject(WorldObject obj)
        {
            ArgumentNullException.ThrowIfNull(obj);
            ValidatePosition(obj.Position);
            _worldObjects.Add(obj);
            MyLogger.Instance.Log($"WorldObject '{obj.Name}' placed at {obj.Position}.");
        }

        /// <summary>Removes all world objects that have been marked as removed.</summary>
        public void CleanupRemovedObjects()
        {
            var toRemove = _worldObjects.Where(o => o.IsRemoved).ToList();
            foreach (var obj in toRemove)
                _worldObjects.Remove(obj);
        }

        // ── Queries ───────────────────────────────────────────────────────────

        /// <summary>Returns the creature at the given position, or <c>null</c>.</summary>
        public Creature? GetCreatureAt(Position pos) =>
            _creatures.FirstOrDefault(c => c.Position == pos && c.IsAlive);

        /// <summary>Returns all world objects at the given position.</summary>
        public IEnumerable<WorldObject> GetObjectsAt(Position pos) =>
            _worldObjects.Where(o => o.Position == pos && !o.IsRemoved);

        /// <summary>Checks whether a position is within bounds.</summary>
        /// <param name="pos">Position to test.</param>
        public bool IsInBounds(Position pos) =>
            pos.X >= 0 && pos.X < MaxX && pos.Y >= 0 && pos.Y < MaxY;

        // ── Helper ────────────────────────────────────────────────────────────
        private void ValidatePosition(Position pos)
        {
            if (!IsInBounds(pos))
                throw new ArgumentException($"Position {pos} is outside the world bounds ({MaxX}x{MaxY}).");
        }
    }
}
