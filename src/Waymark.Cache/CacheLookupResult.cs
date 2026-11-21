namespace Waymark.Cache;

/// <summary>
/// Represents either an explicitly missing cache entry or a found byte sequence.
/// </summary>
public sealed class CacheLookupResult
{
    private CacheLookupResult(byte[]? value)
    {
        Value = value;
    }

    /// <summary>
    /// Gets a result representing a missing cache entry.
    /// </summary>
    public static CacheLookupResult Missing { get; } = new(null);

    /// <summary>
    /// Gets a value indicating whether the cache entry was found.
    /// </summary>
    public bool Found => Value is not null;

    /// <summary>
    /// Gets a copy of the cached value, or <see langword="null"/> when the entry
    /// was not found.
    /// </summary>
    public byte[]? Value { get; }

    internal static CacheLookupResult FromValue(byte[] value) => new(value);
}
