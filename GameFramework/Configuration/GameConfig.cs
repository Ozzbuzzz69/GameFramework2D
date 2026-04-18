using System.Xml.Linq;
using GameFramework.Logging;

namespace GameFramework.Configuration
{
    /// <summary>
    /// Represents the difficulty level of the game.
    /// </summary>
    public enum GameLevel
    {
        /// <summary>Easy difficulty – suitable for beginners.</summary>
        Beginner,
        /// <summary>Normal difficulty.</summary>
        Normal,
        /// <summary>Hard difficulty for experienced players.</summary>
        Expert
    }

    /// <summary>
    /// Reads and stores game configuration from an XML file.
    /// <para>
    /// Expected XML format:
    /// <code>
    /// &lt;GameConfig&gt;
    ///   &lt;World MaxX="20" MaxY="20" /&gt;
    ///   &lt;Level&gt;Normal&lt;/Level&gt;
    /// &lt;/GameConfig&gt;
    /// </code>
    /// </para>
    /// </summary>
    public class GameConfig
    {
        /// <summary>Gets the maximum X dimension of the world.</summary>
        public int MaxX { get; private set; } = 20;

        /// <summary>Gets the maximum Y dimension of the world.</summary>
        public int MaxY { get; private set; } = 20;

        /// <summary>Gets the selected difficulty level.</summary>
        public GameLevel Level { get; private set; } = GameLevel.Normal;

        /// <summary>
        /// Loads configuration from the specified XML file.
        /// Falls back to default values if the file is missing or malformed.
        /// </summary>
        /// <param name="filePath">Path to the XML configuration file.</param>
        /// <returns>A populated <see cref="GameConfig"/> instance.</returns>
        public static GameConfig Load(string filePath)
        {
            var config = new GameConfig();

            if (!File.Exists(filePath))
            {
                MyLogger.Instance.Warn($"Config file '{filePath}' not found – using defaults.");
                return config;
            }

            try
            {
                var doc = XDocument.Load(filePath);
                var root = doc.Root!;

                var worldElem = root.Element("World");
                if (worldElem is not null)
                {
                    config.MaxX = int.Parse(worldElem.Attribute("MaxX")?.Value ?? "20");
                    config.MaxY = int.Parse(worldElem.Attribute("MaxY")?.Value ?? "20");
                }

                var levelText = root.Element("Level")?.Value;
                if (Enum.TryParse<GameLevel>(levelText, true, out var level))
                    config.Level = level;

                MyLogger.Instance.Log($"Config loaded: World={config.MaxX}x{config.MaxY}, Level={config.Level}");
            }
            catch (Exception ex)
            {
                MyLogger.Instance.Error($"Failed to parse config '{filePath}': {ex.Message}");
            }

            return config;
        }

        /// <summary>
        /// Saves the current configuration to an XML file.
        /// </summary>
        /// <param name="filePath">Destination file path.</param>
        public void Save(string filePath)
        {
            var doc = new XDocument(
                new XElement("GameConfig",
                    new XElement("World",
                        new XAttribute("MaxX", MaxX),
                        new XAttribute("MaxY", MaxY)),
                    new XElement("Level", Level.ToString())));
            doc.Save(filePath);
            MyLogger.Instance.Log($"Config saved to '{filePath}'.");
        }
    }
}
