namespace Waymark.Cache.Tests;

public sealed class InMemoryKeyValueCacheContractTests
{
    [Fact]
    public async Task GetAsyncWhenKeyIsMissingReturnsExplicitMiss()
    {
        var cache = CreateCache();
        var result = await cache.GetAsync("missing", CancellationToken.None);

        Assert.False(result.Found);
        Assert.Null(result.Value);
    }

    [Fact]
    public async Task PutAsyncThenGetAsyncReturnsEquivalentValue()
    {
        var cache = CreateCache();
        await cache.PutAsync("key", [1, 2, 3], CancellationToken.None);

        var result = await cache.GetAsync("key", CancellationToken.None);

        Assert.True(result.Found);
        Assert.Equal([1, 2, 3], result.Value);
    }

    [Fact]
    public async Task PutAsyncWhenKeyExistsOverwritesValue()
    {
        var cache = CreateCache();
        await cache.PutAsync("key", [1], CancellationToken.None);
        await cache.PutAsync("key", [2], CancellationToken.None);

        var result = await cache.GetAsync("key", CancellationToken.None);

        Assert.True(result.Found);
        Assert.Equal([2], result.Value);
    }

    [Fact]
    public async Task DeleteAsyncReportsWhetherTheValueExisted()
    {
        var cache = CreateCache();
        Assert.False(await cache.DeleteAsync("missing", CancellationToken.None));

        await cache.PutAsync("key", [1], CancellationToken.None);

        Assert.True(await cache.DeleteAsync("key", CancellationToken.None));
        Assert.False((await cache.GetAsync("key", CancellationToken.None)).Found);
    }

    [Fact]
    public async Task PutAsyncCopiesTheValue()
    {
        var cache = CreateCache();
        byte[] value = [1, 2, 3];
        await cache.PutAsync("key", value, CancellationToken.None);

        value[1] = 9;

        Assert.Equal([1, 2, 3], (await cache.GetAsync("key", CancellationToken.None)).Value);
    }

    [Fact]
    public async Task GetAsyncReturnsACopyOfTheStoredValue()
    {
        var cache = CreateCache();
        await cache.PutAsync("key", [1, 2, 3], CancellationToken.None);
        var first = await cache.GetAsync("key", CancellationToken.None);

        first.Value![1] = 9;
        var second = await cache.GetAsync("key", CancellationToken.None);

        Assert.Equal([1, 2, 3], second.Value);
    }

    [Fact]
    public async Task GetAsyncRejectsInvalidKeys()
    {
        var cache = CreateCache();
        await Assert.ThrowsAsync<ArgumentNullException>(
            () => cache.GetAsync(null!, CancellationToken.None));
        await Assert.ThrowsAsync<ArgumentException>(
            () => cache.GetAsync(string.Empty, CancellationToken.None));
    }

    [Fact]
    public async Task PutAsyncRejectsInvalidArguments()
    {
        var cache = CreateCache();
        await Assert.ThrowsAsync<ArgumentNullException>(
            () => cache.PutAsync(null!, [1], CancellationToken.None));
        await Assert.ThrowsAsync<ArgumentException>(
            () => cache.PutAsync(string.Empty, [1], CancellationToken.None));
        await Assert.ThrowsAsync<ArgumentNullException>(
            () => cache.PutAsync("key", null!, CancellationToken.None));
    }

    [Fact]
    public async Task DeleteAsyncRejectsInvalidKeys()
    {
        var cache = CreateCache();
        await Assert.ThrowsAsync<ArgumentNullException>(
            () => cache.DeleteAsync(null!, CancellationToken.None));
        await Assert.ThrowsAsync<ArgumentException>(
            () => cache.DeleteAsync(string.Empty, CancellationToken.None));
    }

    [Fact]
    public async Task PublicMethodsRespectCancellation()
    {
        var cache = CreateCache();
        using var cancellation = new CancellationTokenSource();
        cancellation.Cancel();

        await Assert.ThrowsAnyAsync<OperationCanceledException>(
            () => cache.GetAsync("key", cancellation.Token));
        await Assert.ThrowsAnyAsync<OperationCanceledException>(
            () => cache.PutAsync("key", [1], cancellation.Token));
        await Assert.ThrowsAnyAsync<OperationCanceledException>(
            () => cache.DeleteAsync("key", cancellation.Token));
    }

    [Fact]
    public async Task ConcurrentAccessDoesNotCorruptIndependentKeys()
    {
        const int workerCount = 16;
        const int operationCount = 100;
        var cache = CreateCache();

        var operations = Enumerable.Range(0, workerCount).Select(async worker =>
        {
            var key = $"key-{worker}";

            for (var operation = 0; operation < operationCount; operation++)
            {
                byte[] value = [(byte)worker, (byte)operation];
                await cache.PutAsync(key, value, CancellationToken.None);

                var result = await cache.GetAsync(key, CancellationToken.None);
                Assert.True(result.Found);
                Assert.Equal(value, result.Value);
            }

            return key;
        });

        var keys = await Task.WhenAll(operations);

        foreach (var key in keys)
        {
            Assert.True((await cache.DeleteAsync(key, CancellationToken.None)));
        }
    }

    private static InMemoryKeyValueCache CreateCache() => new();
}
