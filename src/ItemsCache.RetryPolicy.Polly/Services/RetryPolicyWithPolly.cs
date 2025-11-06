using Microsoft.Extensions.Logging;
using Polly;
using ItemsCache.RetryPolicy.Abstraction.Interfaces;
using ItemsCache.RetryPolicy.Abstraction.Models;

namespace ItemsCache.RetryPolicy.Polly.Services;

internal sealed class RetryPolicyWithPolly : IRetryPolicy
{
    private readonly IAsyncPolicy _policy;
    private readonly ILogger _logger;
    private readonly string _operationType;

    public RetryPolicyWithPolly(RetryOptions options, ILogger logger, string operationType)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _operationType = operationType ?? throw new ArgumentNullException(nameof(operationType));
        _policy = CreatePolicy(options);
    }

    public async Task<TResult> ExecuteAsync<TResult>(Func<CancellationToken, Task<TResult>> operation, CancellationToken cancellationToken = default)
    {
        return await _policy.ExecuteAsync(async (ct) =>
        {
            _logger.LogDebug("ItemsCache{OperationType}Executing operation", _operationType);
            return await operation(ct);
        }, cancellationToken);
    }

    private IAsyncPolicy CreatePolicy(RetryOptions options)
    {
        var retryPolicy = Policy
            .Handle<Exception>()
            .WaitAndRetryAsync(
                retryCount: options.MaxRetryAttempts,
                sleepDurationProvider: retryAttempt =>
                {
                    var delay = TimeSpan.FromMilliseconds(
                        options.InitialRetryDelay.TotalMilliseconds * Math.Pow(2, retryAttempt - 1));
                    return delay > options.MaxRetryDelay ? options.MaxRetryDelay : delay;
                },
                onRetry: (outcome, timespan, retryCount, context) =>
                {
                    _logger.LogWarning(outcome, "ItemsCache{OperationType}Retry attempt {RetryCount} after {Delay}ms",
                        _operationType, retryCount, timespan.TotalMilliseconds);
                });

        var timeoutPolicy = Policy.TimeoutAsync(options.OperationTimeout);

        return Policy.WrapAsync(timeoutPolicy, retryPolicy);
    }
}