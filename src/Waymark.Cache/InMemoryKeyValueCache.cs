namespace Waymark.Cache;

/// <summary>
/// Provides a thread-safe, process-local cache with no persistence or eviction.
/// </summary>
public sealed class InMemoryKeyValueCache : IKeyValueCache
{
    private readonly object operationLock = new();
    private readonly Dictionary<string, byte[]> values =
        new(StringComparer.Ordinal);

    /// <inheritdoc />
    public Task<CacheLookupResult> GetAsync(
        string key,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        ArgumentException.ThrowIfNullOrEmpty(key);

        lock (operationLock)
        {
            cancellationToken.ThrowIfCancellationRequested();
            var result = values.TryGetValue(key, out var value)
                ? CacheLookupResult.FromValue((byte[])value.Clone())
                : CacheLookupResult.Missing;

            return Task.FromResult(result);
        }
    }

    /// <inheritdoc />
    public Task PutAsync(
        string key,
        byte[] value,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        ArgumentException.ThrowIfNullOrEmpty(key);
        ArgumentNullException.ThrowIfNull(value);

        var storedValue = (byte[])value.Clone();

        lock (operationLock)
        {
            cancellationToken.ThrowIfCancellationRequested();
            values[key] = storedValue;
            return Task.CompletedTask;
        }
    }

    /// <inheritdoc />
    public Task<bool> DeleteAsync(
        string key,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        ArgumentException.ThrowIfNullOrEmpty(key);

        lock (operationLock)
        {
            cancellationToken.ThrowIfCancellationRequested();
            return Task.FromResult(values.Remove(key));
        }
    }
}
