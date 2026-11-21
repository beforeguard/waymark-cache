# Waymark Cache

Waymark is an **educational distributed-systems project** that builds a key/value
cache in C#/.NET. It prioritizes explicit correctness guarantees, bounded resources,
reproducible failures, and learning each systems concept in small reviewable steps.

> **Learning-focused, production-minded, not production-ready.** Waymark is a place
> to study routing, replication, consistency, failure handling, observability, and
> performance with careful engineering discipline. It should not be used as an
> authoritative data store or as a production Redis replacement.

## Prerequisites

- .NET SDK 10.0.301 or a later patch release
- Docker Engine or Docker Desktop

## Build and test

```bash
dotnet restore Waymark.Cache.slnx
dotnet build Waymark.Cache.slnx --no-restore
dotnet test Waymark.Cache.slnx --no-build
```

To run the same validation in a container:

```bash
docker compose run --rm test
```

## Chapter 1: minimal in-memory cache

`IKeyValueCache` is the API boundary for a single-node, process-local cache. It
stores non-null byte arrays under non-null, non-empty string keys. The string
comparison is ordinal.

- `GetAsync` returns a `CacheLookupResult`. A missing entry has `Found == false`
  and `Value == null`; a found value is never null.
- `PutAsync` stores a new defensive copy and replaces any existing value.
- `DeleteAsync` returns whether a value existed before the deletion.
- `PutAsync` rejects a null value. Every method rejects a null key with
  `ArgumentNullException` and an empty key with `ArgumentException`.
- Every method checks its `CancellationToken` before validation and immediately
  before changing or reading cache state. A canceled operation has no effect.
- Values are copied both on put and on get. Mutating a caller's original array or
  a returned result therefore cannot mutate cache state.

The asynchronous method signatures preserve a client-compatible shape for later
chapters, although the in-memory implementation currently completes without I/O.

Access is thread-safe. Mutating and lookup operations are serialized in completion
order: the state change occurs before its already-completed `Task` is returned, so
the last completed operation on a key wins. A later overlapping `PutAsync` can
replace an earlier one, while a later overlapping `DeleteAsync` can remove it.

This chapter deliberately has no TTL, expiration, capacity, eviction, memory
accounting, metrics, networking, persistence, replication, membership, routing, or
distribution.

## Learning workflow

Each chapter follows:

1. Learn the concept and its failure modes.
2. Record the design, invariants, and alternatives.
3. Implement the smallest bounded vertical slice.
4. Test normal, boundary, concurrent, and failure behavior.
5. Review the code and explain the result before continuing.

## Project structure

- `src/Waymark.Cache` - cache implementation and public contracts
- `tests/Waymark.Cache.Tests` - unit and invariant tests

Waymark does not target Redis command, protocol, or cluster compatibility unless a
later optional chapter explicitly adds that subset.

