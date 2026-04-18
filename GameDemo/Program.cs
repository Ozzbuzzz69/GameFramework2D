using System.Diagnostics;
using GameDemo;
using GameDemo.Creatures;
using GameFramework.Configuration;
using GameFramework.Items;
using GameFramework.Logging;
using GameFramework.Strategies;
using GameFramework.World;
using GameFramework.WorldObjects;

// ════════════════════════════════════════════════════════════════════════════
//  GAME DEMO – Program.cs
//  Demonstrates the GameFramework2D library. This is NOT a full game;
//  it is a showcase of every framework feature.
// ════════════════════════════════════════════════════════════════════════════

Console.OutputEncoding = System.Text.Encoding.UTF8;

// ── 1. Logging setup ─────────────────────────────────────────────────────────
// Add a console TraceListener so we can see log output while running.
MyLogger.Instance.AddListener(new ConsoleTraceListener());

// Optionally also log to a file:
// MyLogger.Instance.AddListener(new TextWriterTraceListener("game.log"));

Console.WriteLine("════════════════════════════════════════════════════════");
Console.WriteLine("   GameFramework2D – Feature Demo");
Console.WriteLine("════════════════════════════════════════════════════════\n");

// ── 2. Configuration from XML ────────────────────────────────────────────────
// Write a default config file if it doesn't exist, then load it.
const string configPath = "game.xml";
if (!File.Exists(configPath))
{
    var defaultConfig = new GameConfig();
    defaultConfig.Save(configPath);
}

GameConfig config = GameConfig.Load(configPath);
Console.WriteLine($"[Config] World size: {config.MaxX} x {config.MaxY},  Level: {config.Level}\n");

// ── 3. World creation ────────────────────────────────────────────────────────
var world = new World(config.MaxX, config.MaxY);

// ── 4. Create items ──────────────────────────────────────────────────────────
// Plain attack items
var sword       = new AttackItem("Iron Sword",    hit: 15, range: 1, weight: 5.0);
var axe         = new AttackItem("Battle Axe",    hit: 20, range: 1, weight: 8.0);
var bow         = new AttackItem("Long Bow",      hit: 10, range: 5, weight: 3.0);
var magicStaff  = new AttackItem("Magic Staff",   hit: 25, range: 3, weight: 4.0);

// Defence items
var shield      = new DefenceItem("Iron Shield",  reduceHitPoint: 8);
var helmet      = new DefenceItem("Steel Helmet", reduceHitPoint: 4);
var boots       = new DefenceItem("Speed Boots",  reduceHitPoint: 2);

Console.WriteLine("── Items created ──────────────────────────────────────");
Console.WriteLine(sword);
Console.WriteLine(axe);
Console.WriteLine(bow);
Console.WriteLine(shield);
Console.WriteLine(helmet);
Console.WriteLine();

// ── 5. DECORATOR pattern ─────────────────────────────────────────────────────
Console.WriteLine("── Decorator Pattern ──────────────────────────────────");
IAttackItem enchantedSword  = new BoostedAttackDecorator(sword,   multiplier: 2.0);
IAttackItem cursedAxe       = new WeakenedAttackDecorator(axe,    factor: 0.4);

Console.WriteLine($"Original sword hit : {sword.Hit}");
Console.WriteLine($"Enchanted sword hit: {enchantedSword.Hit}  ({enchantedSword.Name})");
Console.WriteLine($"Original axe hit   : {axe.Hit}");
Console.WriteLine($"Cursed axe hit     : {cursedAxe.Hit}  ({cursedAxe.Name})");
Console.WriteLine();

// ── 6. COMPOSITE pattern ─────────────────────────────────────────────────────
Console.WriteLine("── Composite Pattern ──────────────────────────────────");
var battleKit = new CompositeAttackItem("Hero Battle Kit");
battleKit.Add(enchantedSword);
battleKit.Add(bow);
Console.WriteLine($"Composite item: {battleKit}");
Console.WriteLine($"  Total Hit    : {battleKit.Hit}");
Console.WriteLine($"  Total Weight : {battleKit.Weight}");
Console.WriteLine();

// ── 7. OPERATOR OVERLOAD (+) ─────────────────────────────────────────────────
Console.WriteLine("── Operator Overload (AttackItem + AttackItem) ────────");
CompositeAttackItem combined = sword + axe;   // uses the overloaded + operator
Console.WriteLine($"sword + axe = {combined.Name}, Hit={combined.Hit}, Weight={combined.Weight}");
Console.WriteLine();

// ── 8. Create creatures ───────────────────────────────────────────────────────
Console.WriteLine("── Creatures ──────────────────────────────────────────");
var hero    = new Hero("Aldric",    maxCarryWeight: 60.0);
var monster = new Monster("Goblin", maxCarryWeight: 20.0);

hero.Position    = new Position(0, 0);
monster.Position = new Position(1, 0);

// Equip hero (Template Method hook fires when Hit is called later)
hero.AddAttackItem(enchantedSword);
hero.AddAttackItem(bow);
hero.AddDefenceItem(shield);
hero.AddDefenceItem(helmet);

// Equip monster
monster.AddAttackItem(axe);
monster.AddDefenceItem(boots);

world.AddCreature(hero);
world.AddCreature(monster);

Console.WriteLine(hero);
Console.WriteLine(monster);
Console.WriteLine();

// ── 9. OBSERVER pattern ───────────────────────────────────────────────────────
Console.WriteLine("── Observer Pattern ───────────────────────────────────");
var battleLogger = new BattleLogger();
hero.AddObserver(battleLogger);
monster.AddObserver(battleLogger);
Console.WriteLine("BattleLogger observer registered for both creatures.\n");

// ── 10. STRATEGY pattern – swap hit strategy at runtime ───────────────────────
Console.WriteLine("── Strategy Pattern ───────────────────────────────────");
Console.WriteLine($"Hero uses SumHitStrategy by default.");
hero.SetHitStrategy(new AggressiveHitStrategy(bonus: 10));
Console.WriteLine($"Hero strategy swapped to AggressiveHitStrategy (+10 bonus).\n");

// ── 11. World objects + Loot ──────────────────────────────────────────────────
Console.WriteLine("── World Objects & Loot ───────────────────────────────");
var chest = new TreasureChest("Ancient Chest", new Position(2, 0));
chest.AddLootItem(magicStaff);
chest.AddLootItem(new DefenceItem("Dragon Plate", reduceHitPoint: 12));

var bonusBox = new BonusBox("Bonus Box", new Position(3, 0));
bonusBox.AddLootItem(new AttackItem("Dagger", hit: 5, range: 1, weight: 1.0));

world.AddWorldObject(chest);
world.AddWorldObject(bonusBox);

// Move hero next to chest and loot it
hero.Position = new Position(2, 0);
hero.Loot(chest);
Console.WriteLine($"Hero after looting chest: {hero}");
Console.WriteLine();

// ── 12. COMBAT ROUND ─────────────────────────────────────────────────────────
Console.WriteLine("── Combat ─────────────────────────────────────────────");
Console.WriteLine($"Before combat:  {hero}");
Console.WriteLine($"Before combat:  {monster}");
Console.WriteLine();

// Hero attacks monster (AggressiveHitStrategy + enchantedSword + bow + magicStaff)
Console.WriteLine("Round 1 – Hero attacks Goblin:");
hero.Hit(monster);
Console.WriteLine();

// Monster attacks hero (RandomHitStrategy + axe)
Console.WriteLine("Round 2 – Goblin attacks Aldric:");
monster.Hit(hero);
Console.WriteLine();

// Keep fighting until something dies
int round = 3;
while (hero.IsAlive && monster.IsAlive && round <= 20)
{
    Console.WriteLine($"Round {round} – Hero attacks Goblin:");
    hero.Hit(monster);
    if (monster.IsAlive)
    {
        Console.WriteLine($"Round {round} – Goblin retaliates:");
        monster.Hit(hero);
    }
    round++;
    Console.WriteLine();
}

// ── 13. Carry-weight limit demonstration ─────────────────────────────────────
Console.WriteLine("── Carry-Weight Limit ─────────────────────────────────");
var heavyMace = new AttackItem("Giant Mace", hit: 50, range: 1, weight: 200.0);
bool added = hero.AddAttackItem(heavyMace);
Console.WriteLine($"Try adding Giant Mace (200 kg) to hero → {(added ? "SUCCESS" : "REFUSED (overweight)")}");
Console.WriteLine();

// ── 14. World cleanup ─────────────────────────────────────────────────────────
world.CleanupRemovedObjects();
Console.WriteLine($"Active world objects after cleanup: {world.WorldObjects.Count}");
Console.WriteLine();

Console.WriteLine("════════════════════════════════════════════════════════");
Console.WriteLine("   Demo complete.");
Console.WriteLine("════════════════════════════════════════════════════════");
