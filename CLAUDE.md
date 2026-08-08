# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## What this is

A playable Ultimate Tic Tac Toe browser game, built as a **.NET 8 Blazor WebAssembly** app and deployed as static files to GitHub Pages.

> The current branch is `migrateToRustWasm`, but there is no Rust in the tree yet — all code is C#/.NET 8. Treat the Blazor stack below as the source of truth until Rust actually lands.

## Commands

Run from the repo root (`uttt.sln` ties the projects together).

```bash
dotnet restore
dotnet build                    # build the whole solution
dotnet test                     # run all xUnit tests
```

Run the app locally (hot-reload dev server):

```bash
dotnet run --project src/uttt.app/uttt.app.csproj
```

Run a single test by name (xUnit via `dotnet test --filter`):

```bash
dotnet test --filter "FullyQualifiedName~BoardTests.CanCreateBoard"
dotnet test --filter "DisplayName~TestSmallSquareScenarios"   # a [Theory] and all its cases
```

Produce the static publish output that GitHub Pages serves (mirrors `pages.yml`):

```bash
dotnet publish src/uttt.app/uttt.app.csproj -c Release -o publish
# static site lands in ./publish/wwwroot
```

## Architecture

Three projects, split so game rules stay independent of the UI:

- **`src/uttt.game`** — pure game-logic library, no UI or framework deps. All domain types live in `Records.cs`.
- **`src/uttt.app`** — Blazor WebAssembly UI. References `uttt.game`. Entry point `Program.cs` mounts `App` at `#app`.
- **`test/utt.game.test`** — xUnit tests against `uttt.game` (win/draw detection for small and large boards).

### Domain model (`src/uttt.game/Records.cs`)

The board is modeled recursively with immutable `record struct`s:

- `SpotState` — `Open | X | O | Draw`.
- `SmallSquare` — nine `SpotState`s (one 3×3 board), fields named by position (`TopLeft` … `BottomRight`).
- `LargeSquare` — nine `SmallSquare`s (the outer 3×3), same positional field names. `LargeSquare.NewBoard()` builds an empty game.
- Both implement `ISquare<T>.ToSpot()`, which reports the winner/draw/open state of that square.

Win detection is centralized in `SpotStateUtils.ToSpot(...)`: it takes nine `Func<SpotState>` accessors plus an `includeDraw` flag and checks all 8 lines via `UnionEvaluator`. `SmallSquare` passes its own cells; `LargeSquare` passes each child's `ToSpot`, so the same logic evaluates both levels. Note `includeDraw: false` is used at both levels — a line of three *drawn* small boards does **not** win the large board; a full-but-unwon square resolves to `Draw`.

`Game(LargeSquare Square, Player NextPlayer, int SquareToPlay)` is a defined record but is **not currently wired into the UI** — the live game state is held in the component instead (see below).

### UI and interactive game rules (`src/uttt.app/Shared/UttBoard.razor`)

This component is the most important file to understand: the **interactive rules of Ultimate Tic Tac Toe live here, not in `uttt.game`**. The library only knows how to score a square. `UttBoard.razor` owns:

- Mutable game state as component fields: `boardState` (a `LargeSquare`), `CurrentPlayer`, and `NextBoardToPlay` (`-1` = play anywhere, else the forced small-board index).
- The move rule in `OnCellClick`: the small index just played dictates the next player's target board; if that target is already won/full, the constraint resets to "anywhere" (`-1`).
- Immutable updates via `with` expressions — `SetSpot`/`GetSpot`/`GetSmallSquare` translate a flat `0..80` cell index into the nested positional record fields with `switch` expressions.
- Presentation state: highlighting the playable board(s), and the capture animation (`justCaptured` pulse → `flippedIndices` flip) driven by `TriggerFlipAsync`.

When changing game rules, edit `UttBoard.razor`. When changing what counts as a win, edit `SpotStateUtils`/`ToSpot` in `Records.cs` and cover it in `BoardTests.cs`.

## CI/CD

- `.github/workflows/ci.yml` — on PRs (any branch) and pushes to `main`: restore → `dotnet test` → Release build.
- `.github/workflows/pages.yml` — on push to `main` (or manual dispatch): publishes the Blazor app and deploys `./publish/wwwroot` to the `gh-pages` branch.

## Docs / architecture records

`doc/` holds C4 model sources (`workspace.dsl`, `model.dsl`) rendered with **Structurizr Lite**, and Architecture Decision Records under `doc/adr/` (managed with `adr-tools`). The `readme.md` documents the Podman-based Structurizr setup. ADR 0003 records the choice of Blazor for cross-platform deployability; 0005 records GitHub Pages deployment.
