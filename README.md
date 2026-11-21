# Waymark Cache

Waymark is an educational distributed key/value cache built in C#/.NET. The project
prioritizes explicit correctness guarantees, bounded resources, reproducible
failures, and learning each systems concept in small reviewable steps.

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

The cache is not an authoritative data store and does not target Redis command,
protocol, or cluster compatibility unless a later optional chapter explicitly adds
that subset.

