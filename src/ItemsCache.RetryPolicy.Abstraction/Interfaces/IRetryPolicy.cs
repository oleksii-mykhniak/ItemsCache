namespace ItemsCache.RetryPolicy.Abstraction.Interfaces
{
    public interface IRetryPolicy
    {
        Task<TResult> ExecuteAsync<TResult>(Func<CancellationToken, Task<TResult>> operation, CancellationToken cancellationToken = default);
    }
}
