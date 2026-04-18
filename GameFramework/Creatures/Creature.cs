using GameFramework.Items;
using GameFramework.Logging;
using GameFramework.Strategies;
using GameFramework.World;

namespace GameFramework.Creatures
{
    // ═══════════════════════════════════════════════════════════════════════════
    // TEMPLATE METHOD PATTERN
    // Creature defines the skeleton (Hit, ReceiveHit, Loot).
    // Subclasses override the protected hook methods to customise behaviour
    // without changing the overall algorithm structure.
    //
    // OBSERVER PATTERN
    // Creature maintains a list of ICreatureObserver subscribers and notifies
    // them on every hit and on death.
    //
    // STRATEGY PATTERN
    // The hit-calculation algorithm is delegated to an IHitStrategy that can be
    // replaced at runtime (Dependency Inversion Principle).
    // ═══════════════════════════════════════════════════════════════════════════

    /// <summary>
    /// Abstract base class for all creatures in the game world.
    /// <para>
    /// Implements the Template Method, Observer and Strategy design patterns.
    /// Concrete creatures must inherit from this class and override at least
    /// <see cref="GetBaseHitPoints"/> to set starting health.
    /// </para>
    /// </summary>
    public abstract class Creature
    {
        // ── Fields ───────────────────────────────────────────────────────────
        private readonly List<ICreatureObserver> _observers  = new();
        private readonly List<IAttackItem>        _attackItems  = new();
        private readonly List<IDefenceItem>       _defenceItems = new();
        private IHitStrategy                      _hitStrategy;

        // ── Construction ─────────────────────────────────────────────────────

        /// <summary>
        /// Initialises a creature with a name, optional hit strategy and max carry weight.
        /// </summary>
        /// <param name="name">Display name of the creature.</param>
        /// <param name="hitStrategy">
        /// The strategy used to calculate outgoing damage.
        /// Defaults to <see cref="SumHitStrategy"/> if <c>null</c>.
        /// </param>
        /// <param name="maxCarryWeight">
        /// Maximum total weight of attack items the creature can carry.
        /// Set to <see cref="double.MaxValue"/> for unlimited.
        /// </param>
        protected Creature(string name, IHitStrategy? hitStrategy = null, double maxCarryWeight = 100.0)
        {
            Name            = name;
            _hitStrategy    = hitStrategy ?? new SumHitStrategy();
            MaxCarryWeight  = maxCarryWeight;
            HitPoints       = GetBaseHitPoints();
            Position        = new Position(0, 0);
        }

        // ── Properties ───────────────────────────────────────────────────────

        /// <summary>Gets the creature's display name.</summary>
        public string Name { get; }

        /// <summary>Gets or sets the current hit-points. Death occurs when this reaches ≤ 0.</summary>
        public int HitPoints { get; protected set; }

        /// <summary>Gets whether the creature is still alive.</summary>
        public bool IsAlive => HitPoints > 0;

        /// <summary>Gets or sets the creature's position in the world.</summary>
        public Position Position { get; set; }

        /// <summary>Gets the maximum total weight of attack items this creature can carry.</summary>
        public double MaxCarryWeight { get; }

        /// <summary>Gets the current total weight of all equipped attack items.</summary>
        public double CurrentCarryWeight => _attackItems.Sum(a => a.Weight);

        /// <summary>Read-only view of the creature's attack items.</summary>
        public IReadOnlyList<IAttackItem> AttackItems => _attackItems.AsReadOnly();

        /// <summary>Read-only view of the creature's defence items.</summary>
        public IReadOnlyList<IDefenceItem> DefenceItems => _defenceItems.AsReadOnly();

        // ── Template Method hooks ─────────────────────────────────────────────

        /// <summary>
        /// Returns the starting hit-points for this creature type.
        /// Override in subclasses to set species-specific health.
        /// </summary>
        protected abstract int GetBaseHitPoints();

        /// <summary>
        /// Hook called before the creature deals damage. Override to add pre-attack logic.
        /// </summary>
        protected virtual void OnBeforeHit(Creature target) { }

        /// <summary>
        /// Hook called after the creature deals damage. Override to add post-attack logic.
        /// </summary>
        protected virtual void OnAfterHit(Creature target, int damage) { }

        /// <summary>
        /// Hook called when this creature dies. Override for death animations / drops.
        /// </summary>
        protected virtual void OnDying() { }

        // ── Strategy ─────────────────────────────────────────────────────────

        /// <summary>Replaces the active hit strategy at runtime.</summary>
        /// <param name="strategy">The new strategy to use.</param>
        public void SetHitStrategy(IHitStrategy strategy)
        {
            ArgumentNullException.ThrowIfNull(strategy);
            _hitStrategy = strategy;
            MyLogger.Instance.Log($"{Name}: hit strategy changed to {strategy.GetType().Name}");
        }

        // ── Observer management ───────────────────────────────────────────────

        /// <summary>Registers an observer to receive creature events.</summary>
        /// <param name="observer">Observer to add.</param>
        public void AddObserver(ICreatureObserver observer)
        {
            ArgumentNullException.ThrowIfNull(observer);
            _observers.Add(observer);
        }

        /// <summary>Removes a previously registered observer.</summary>
        /// <param name="observer">Observer to remove.</param>
        public void RemoveObserver(ICreatureObserver observer) =>
            _observers.Remove(observer);

        // ── Core methods (Template pattern) ──────────────────────────────────

        /// <summary>
        /// Attacks the <paramref name="target"/> creature.
        /// The damage is computed via the active <see cref="IHitStrategy"/>.
        /// <para>Template steps: OnBeforeHit → strategy → target.ReceiveHit → OnAfterHit.</para>
        /// </summary>
        /// <param name="target">The creature to attack.</param>
        /// <returns>The raw damage value before the target's defence reduction.</returns>
        public int Hit(Creature target)
        {
            if (!IsAlive)
            {
                MyLogger.Instance.Warn($"{Name} cannot attack – it is dead.");
                return 0;
            }

            OnBeforeHit(target);
            int damage = _hitStrategy.CalculateHit(_attackItems);
            MyLogger.Instance.Log($"{Name} hits {target.Name} for {damage} damage.");
            target.ReceiveHit(damage);
            OnAfterHit(target, damage);
            return damage;
        }

        /// <summary>
        /// Receives an incoming hit, reduces it by equipped defence items, applies
        /// the remainder to <see cref="HitPoints"/> and notifies observers.
        /// </summary>
        /// <param name="incomingDamage">Raw damage from the attacker.</param>
        public void ReceiveHit(int incomingDamage)
        {
            int reduction   = _defenceItems.Sum(d => d.ReduceHitPoint);
            int finalDamage = Math.Max(0, incomingDamage - reduction);
            HitPoints      -= finalDamage;

            MyLogger.Instance.Log(
                $"{Name} receives {incomingDamage} dmg, reduced by {reduction} → {finalDamage}. HP: {HitPoints}");

            NotifyHit(finalDamage);

            if (!IsAlive)
            {
                MyLogger.Instance.Log($"{Name} has died.");
                OnDying();
                NotifyDeath();
            }
        }

        /// <summary>
        /// Picks up (loots) a <see cref="WorldObject"/> from the world.
        /// Attack / defence items are added if carry-weight allows.
        /// </summary>
        /// <param name="worldObject">The world object to loot.</param>
        public void Loot(WorldObjects.WorldObject worldObject)
        {
            if (!worldObject.Lootable)
            {
                MyLogger.Instance.Warn($"{Name} tried to loot {worldObject.Name} but it is not lootable.");
                return;
            }

            foreach (var item in worldObject.GetLootableItems())
            {
                switch (item)
                {
                    case IAttackItem attack:
                        AddAttackItem(attack);
                        break;
                    case IDefenceItem defence:
                        AddDefenceItem(defence);
                        break;
                }
            }

            if (worldObject.Removable)
                worldObject.Remove();

            MyLogger.Instance.Log($"{Name} looted {worldObject.Name}.");
        }

        // ── Item management ───────────────────────────────────────────────────

        /// <summary>
        /// Adds an attack item to the creature's inventory if it fits within
        /// <see cref="MaxCarryWeight"/>.
        /// </summary>
        /// <param name="item">The item to add.</param>
        /// <returns><c>true</c> if the item was added; <c>false</c> if overweight.</returns>
        public bool AddAttackItem(IAttackItem item)
        {
            if (CurrentCarryWeight + item.Weight > MaxCarryWeight)
            {
                MyLogger.Instance.Warn(
                    $"{Name}: cannot carry {item.Name} – weight limit exceeded " +
                    $"({CurrentCarryWeight + item.Weight} > {MaxCarryWeight}).");
                return false;
            }
            _attackItems.Add(item);
            MyLogger.Instance.Log($"{Name} equipped attack item: {item.Name}.");
            return true;
        }

        /// <summary>Adds a defence item to the creature's inventory.</summary>
        /// <param name="item">The item to add.</param>
        public void AddDefenceItem(IDefenceItem item)
        {
            _defenceItems.Add(item);
            MyLogger.Instance.Log($"{Name} equipped defence item: {item.Name}.");
        }

        // ── Observer notification ─────────────────────────────────────────────
        private void NotifyHit(int damage)
        {
            foreach (var obs in _observers)
                obs.OnHit(this, damage);
        }

        private void NotifyDeath()
        {
            foreach (var obs in _observers)
                obs.OnDeath(this);
        }

        /// <inheritdoc/>
        public override string ToString() =>
            $"[{GetType().Name}: {Name}, HP={HitPoints}, Alive={IsAlive}, " +
            $"Attack={_attackItems.Count}, Defence={_defenceItems.Count}]";
    }
}
