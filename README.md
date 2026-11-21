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

