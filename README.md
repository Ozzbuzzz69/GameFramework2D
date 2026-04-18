# GameFramework2D

A mini-framework for **turn-based 2D games** written in **C# / .NET 8**.

---

## Project structure

```
GameFramework.sln
├── GameFramework/          ← The library (NuGet package)
│   ├── Configuration/
│   │   └── GameConfig.cs           XML config reader/writer
│   ├── Logging/
│   │   └── MyLogger.cs             Singleton logger (Trace-based)
│   ├── Items/
│   │   ├── IItems.cs               IAttackItem / IDefenceItem interfaces
│   │   ├── Items.cs                AttackItem, DefenceItem (+ operator overload)
│   │   ├── AttackItemDecorator.cs  Decorator pattern (Boosted / Weakened)
│   │   └── CompositeAttackItem.cs  Composite pattern
│   ├── Strategies/
│   │   └── IHitStrategy.cs         Strategy pattern (Sum / Aggressive / Random)
│   ├── Creatures/
│   │   ├── ICreatureObserver.cs    Observer interface
│   │   └── Creature.cs             Abstract Template + Observer + Strategy
│   └── World/
│       ├── WorldObjects.cs         Position struct, WorldObject, TreasureChest, Wall, BonusBox
│       └── World.cs                World (2D grid)
│
└── GameDemo/               ← Console app showing the framework in use
    ├── Creatures/
    │   └── ConcreteCreatures.cs    Hero, Monster (concrete Creature subclasses)
    ├── BattleLogger.cs             ICreatureObserver implementation
    ├── Program.cs                  Full feature demo
    └── game.xml                    Default configuration file
```

---

## Design patterns implemented

| Pattern           | Where |
|-------------------|-------|
| **Singleton**     | `MyLogger` |
| **Template Method** | `Creature` – `Hit`, `ReceiveHit`, `Loot`; hooks `OnBeforeHit`, `OnAfterHit`, `OnDying` |
| **Observer**      | `Creature` notifies `ICreatureObserver` on hit and on death |
| **Strategy**      | `IHitStrategy` – pluggable hit-calculation: `SumHitStrategy`, `AggressiveHitStrategy`, `RandomHitStrategy` |
| **Decorator**     | `AttackItemDecorator`, `BoostedAttackDecorator`, `WeakenedAttackDecorator` |
| **Composite**     | `CompositeAttackItem` – sums `Hit` and `Weight` of child items |

---

## SOLID principles

| Principle | Where |
|-----------|-------|
| **S**ingle Responsibility | Each class has one job: `MyLogger` only logs, `GameConfig` only reads XML, `World` only manages position/containment |
| **O**pen/Closed | `AttackItemDecorator` extends behaviour without modifying `AttackItem` |
| **L**iskov Substitution | Any `IAttackItem` can be passed where an `IAttackItem` is expected (`AttackItem`, `CompositeAttackItem`, any decorator) |
| **I**nterface Segregation | `IAttackItem` and `IDefenceItem` are separate; `ICreatureObserver` is separate from the creature |
| **D**ependency Inversion | `Creature` depends on `IHitStrategy` (abstraction), not on `SumHitStrategy` (concrete class) |

---

## Operator overload

`AttackItem` overloads the `+` operator to combine two items into a `CompositeAttackItem`:

```csharp
var sword = new AttackItem("Iron Sword", hit: 15, range: 1, weight: 5.0);
var axe   = new AttackItem("Battle Axe", hit: 20, range: 1, weight: 8.0);

CompositeAttackItem combined = sword + axe;
Console.WriteLine(combined.Hit);    // 35
Console.WriteLine(combined.Weight); // 13.0
```

---

## Configuration file (game.xml)

```xml
<?xml version="1.0" encoding="utf-8"?>
<GameConfig>
  <World MaxX="20" MaxY="20" />
  <Level>Normal</Level>   <!-- Beginner | Normal | Expert -->
</GameConfig>
```

Load with:

```csharp
GameConfig config = GameConfig.Load("game.xml");
var world = new World(config.MaxX, config.MaxY);
```

---

## Logging

```csharp
// Add listeners before using the framework
MyLogger.Instance.AddListener(new ConsoleTraceListener());
MyLogger.Instance.AddListener(new TextWriterTraceListener("game.log"));

// Remove one later
MyLogger.Instance.RemoveListener(someListener);
```

No `Console.WriteLine` exists anywhere in the library — all output goes through `MyLogger`.

---

## Running the demo

```bash
cd GameFramework
dotnet run --project GameDemo
```

---

## Generating documentation (Doxygen)

```bash
# Install Doxygen: https://www.doxygen.nl/download.html
cd GameFramework
doxygen Doxyfile
# Output at: docs/html/index.html
```

---


