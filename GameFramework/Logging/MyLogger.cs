using System.Diagnostics;

namespace GameFramework.Logging
{
    /// <summary>
    /// Singleton logger that wraps <see cref="System.Diagnostics.Trace"/>.
    /// Supports adding and removing <see cref="TraceListener"/> instances at runtime.
    /// All framework classes must use this instead of Console.WriteLine.
    /// </summary>
    /// <remarks>
    /// Usage: <c>MyLogger.Instance.Log("message");</c>
    /// </remarks>
    public sealed class MyLogger
    {
        // ── Singleton ────────────────────────────────────────────────────────────
        private static readonly Lazy<MyLogger> _instance =
            new(() => new MyLogger(), LazyThreadSafetyMode.ExecutionAndPublication);

        /// <summary>Gets the single <see cref="MyLogger"/> instance.</summary>
        public static MyLogger Instance => _instance.Value;

        private MyLogger()
        {
            // Remove default listeners so the library is not polluted by console output.
            Trace.Listeners.Clear();
        }

        // ── Listener management ──────────────────────────────────────────────────

        /// <summary>Adds a <see cref="TraceListener"/> to the logging pipeline.</summary>
        /// <param name="listener">The listener to add.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="listener"/> is null.</exception>
        public void AddListener(TraceListener listener)
        {
            ArgumentNullException.ThrowIfNull(listener);
            Trace.Listeners.Add(listener);
        }

        /// <summary>Removes a <see cref="TraceListener"/> from the logging pipeline.</summary>
        /// <param name="listener">The listener to remove.</param>
        public void RemoveListener(TraceListener listener)
        {
            Trace.Listeners.Remove(listener);
        }

        /// <summary>Removes all registered <see cref="TraceListener"/> instances.</summary>
        public void ClearListeners() => Trace.Listeners.Clear();

        // ── Logging ──────────────────────────────────────────────────────────────

        /// <summary>Logs an informational message.</summary>
        /// <param name="message">The message to log.</param>
        public void Log(string message) =>
            Trace.TraceInformation($"[INFO]  {DateTime.Now:HH:mm:ss} – {message}");

        /// <summary>Logs a warning message.</summary>
        /// <param name="message">The message to log.</param>
        public void Warn(string message) =>
            Trace.TraceWarning($"[WARN]  {DateTime.Now:HH:mm:ss} – {message}");

        /// <summary>Logs an error message.</summary>
        /// <param name="message">The message to log.</param>
        public void Error(string message) =>
            Trace.TraceError($"[ERROR] {DateTime.Now:HH:mm:ss} – {message}");
    }
}
