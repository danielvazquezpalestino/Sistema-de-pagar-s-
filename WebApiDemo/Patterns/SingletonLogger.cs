namespace WebApiDemo.Patterns
{
    /// <summary>
    /// Classic Singleton Pattern implementation for logging API operations.
    /// Thread-safe implementation using Lazy<T> for lazy initialization.
    /// Stores recent API operations in memory (max 100 entries).
    /// </summary>
    public class SingletonLogger
    {
        private static readonly Lazy<SingletonLogger> _instance = new Lazy<SingletonLogger>(() => new SingletonLogger());
        
        private readonly List<string> _logs;
        private readonly object _lock = new object();
        private const int MaxLogs = 100;

        /// <summary>
        /// Private constructor to prevent direct instantiation.
        /// </summary>
        private SingletonLogger()
        {
            _logs = new List<string>();
        }

        /// <summary>
        /// Gets the singleton instance of the logger.
        /// Lazy<T> ensures thread-safe lazy initialization.
        /// </summary>
        public static SingletonLogger Instance => _instance.Value;

        /// <summary>
        /// Logs a message to the in-memory log store.
        /// Thread-safe operation using lock to prevent race conditions.
        /// Maintains maximum of 100 entries by removing oldest when limit is reached.
        /// </summary>
        /// <param name="message">The message to log.</param>
        public void Log(string message)
        {
            lock (_lock)
            {
                var logEntry = $"[{DateTime.UtcNow:yyyy-MM-dd HH:mm:ss.fff}] {message}";
                
                if (_logs.Count >= MaxLogs)
                {
                    _logs.RemoveAt(0);
                }
                
                _logs.Add(logEntry);
            }
        }

        /// <summary>
        /// Gets all recent log entries.
        /// Thread-safe operation using lock to ensure consistent reads.
        /// </summary>
        /// <returns>A list of recent log entries.</returns>
        public List<string> GetRecentLogs()
        {
            lock (_lock)
            {
                return new List<string>(_logs);
            }
        }

        /// <summary>
        /// Clears all log entries.
        /// Thread-safe operation using lock.
        /// </summary>
        public void ClearLogs()
        {
            lock (_lock)
            {
                _logs.Clear();
            }
        }
    }
}
