namespace ItemsCache.RetryPolicy.Abstraction.Models
{
    public class RetryOptions
    {
        public int MaxRetryAttempts { get; set; } = 3;
        
        public TimeSpan InitialRetryDelay { get; set; } = TimeSpan.FromSeconds(1);

        public TimeSpan MaxRetryDelay { get; set; } = TimeSpan.FromSeconds(30);
        
        public TimeSpan OperationTimeout { get; set; } = TimeSpan.FromSeconds(30);
    }
}
