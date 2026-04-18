using GameFramework.Creatures;
using GameFramework.Items;
using GameFramework.Logging;
using GameFramework.Strategies;

namespace GameDemo.Creatures
{
    // ── Concrete Hero ─────────────────────────────────────────────────────────

    /// <summary>
    /// A player-controlled hero creature.
    /// Demonstrates Template Method by overriding the OnBeforeHit hook to shout
    /// a battle cry.
    /// </summary>
    public class Hero : Creature
    {
        public Hero(string name, double maxCarryWeight = 50.0)
            : base(name, new SumHitStrategy(), maxCarryWeight) { }

        protected override int GetBaseHitPoints() => 100;

        protected override void OnBeforeHit(Creature target)
        {
            MyLogger.Instance.Log($"{Name} shouts: 'For glory!' before striking {target.Name}!");
        }

        protected override void OnDying()
        {
            MyLogger.Instance.Log($"{Name} whispers: 'Tell my story...' as they fall.");
        }
    }

    // ── Concrete Monster ──────────────────────────────────────────────────────

    /// <summary>
    /// An enemy monster that uses a random hit strategy and starts with fewer HP.
    /// </summary>
    public class Monster : Creature
    {
        public Monster(string name, double maxCarryWeight = 30.0)
            : base(name, new RandomHitStrategy(), maxCarryWeight) { }

        protected override int GetBaseHitPoints() => 60;

        protected override void OnAfterHit(Creature target, int damage)
        {
            MyLogger.Instance.Log($"{Name} growls after hitting {target.Name} for {damage}.");
        }
    }
}
