namespace Waymark.Cache;

/// <summary>
/// Defines the single-node byte-oriented cache contract.
/// </summary>
public interface IKeyValueCache
{
    /// <summary>
    /// Looks up a value by key.
    /// </summary>
    /// <exception cref="ArgumentNullException"><paramref name="key"/> is
    /// <see langword="null"/>.</exception>
    /// <exception cref="ArgumentException"><paramref name="key"/> is empty.</exception>
    /// <exception cref="OperationCanceledException"><paramref name="cancellationToken"/>
    /// is canceled.</exception>
    Task<CacheLookupResult> GetAsync(
        string key,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Stores a copy of a value for a key, replacing any current value.
    /// </summary>
    /// <exception cref="ArgumentNullException"><paramref name="key"/> or
    /// <paramref name="value"/> is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentException"><paramref name="key"/> is empty.</exception>
    /// <exception cref="OperationCanceledException"><paramref name="cancellationToken"/>
    /// is canceled.</exception>
    Task PutAsync(
        string key,
        byte[] value,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes the value for a key and reports whether a value existed.
    /// </summary>
    /// <exception cref="ArgumentNullException"><paramref name="key"/> is
    /// <see langword="null"/>.</exception>
    /// <exception cref="ArgumentException"><paramref name="key"/> is empty.</exception>
    /// <exception cref="OperationCanceledException"><paramref name="cancellationToken"/>
    /// is canceled.</exception>
    Task<bool> DeleteAsync(
        string key,
        CancellationToken cancellationToken = default);
}
