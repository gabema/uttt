# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## What this is

A playable Ultimate Tic Tac Toe browser game, built as a **Rust → WebAssembly** app using the **Leptos** framework, built with **Trunk**, and deployed as static files to GitHub Pages.

> Migrated from an earlier .NET 8 Blazor WebAssembly implementation (see ADR 0006). No C#/.NET remains in the tree.

## Commands

A Cargo workspace at the repo root ties the crates together.

```bash
cargo build                     # build the workspace
cargo test -p uttt-core         # run the engine tests (native, fast, no browser)
cargo clippy -p uttt-core --all-targets -- -D warnings
cargo fmt --all
```

Run a single test by name:

```bash
cargo test -p uttt-core small_diagonal_x_wins
```

Run the app locally (hot-reload dev server at http://localhost:8080/):

```bash
cd crates/uttt-web && trunk serve
```

Produce the static publish output GitHub Pages serves (mirrors `pages.yml`):

```bash
cd crates/uttt-web && trunk build --release --public-url "/uttt/"
# static site lands in crates/uttt-web/dist
```

> Windows/Git Bash gotcha: MSYS path conversion mangles the `/uttt/` argument into a Windows path. Locally prefix with `MSYS_NO_PATHCONV=1`, or omit `--public-url` for local builds (they serve from `/`). CI runs on Linux and is unaffected. Trunk under PowerShell also trips over an inherited `NO_COLOR=1`; prefer Git Bash locally.

## Architecture

Two crates, split so game rules stay independent of the UI:

- **`crates/uttt-core`** — pure game-logic library. **No UI/WASM dependencies** (no `leptos`, `wasm-bindgen`, or `web-sys`). This is a hard rule: it keeps `cargo test` native and fast, and keeps the crate reusable under a different UI framework (the cross-platform hedge in ADR 0006). Holds the board model, win/draw detection, the `Game` state machine, and the `BoardView` projection.
- **`crates/uttt-web`** — Leptos WebAssembly frontend. The **disposable view**: it renders entirely from `uttt_core`'s `BoardView`, forwards clicks to `Game::play`, and owns only the transient capture animation. It computes no game logic and stores no authoritative game state.

### Key invariants

These are load-bearing — preserve them when changing code (see `openspec/` design.md and ADR 0006):

1. **`uttt-core` stays framework-free.** Never add a UI/WASM dependency to it.
2. **The view is disposable.** All game facts and all derived display facts come from the engine projection. The permanent winner backface is *derived* from small-board status; only the in-flight pulse is view-local state.
3. **Draw semantics.** Three *drawn* small boards do **not** win the large board — draws are excluded when evaluating lines, at both levels. Covered by `uttt-core` tests; keep it that way.

### Domain model (`crates/uttt-core`)

- `Player` (`X | O`), `Cell` (`Empty | Mark(Player)`), `SquareStatus` (`InPlay | Won(Player) | Draw`).
- `SmallBoard { cells: [Cell; 9] }` and `Board { boards: [SmallBoard; 9] }`, indexed `0..8` (row-major). Win detection is centralized in `resolve(...)` over a `const LINES: [[usize; 3]; 8]` table, used for both the small and large square — the Rust equivalent of the old `SpotStateUtils.ToSpot`.
- `Game { board, current, constraint }` owns the interactive rules. `play(cell)` (flat `0..81`) returns `Result<MoveOutcome, MoveError>`; `view()` returns the `BoardView { cells: [Cell; 81], small_status: [SquareStatus; 9], playable: [bool; 9], overall, next_player }` projection.

When changing game rules, edit `crates/uttt-core/src/game.rs` and cover it in tests. When changing what counts as a win, edit `resolve`/`LINES` in `crates/uttt-core/src/board.rs`. The view (`crates/uttt-web/src/main.rs`) must not encode rules.

## CI/CD

- `.github/workflows/ci.yml` — on PRs (any branch) and pushes to `main`: `cargo fmt --check`, clippy + `cargo test` on `uttt-core`, then a `trunk build` of the wasm app. Uses `Swatinem/rust-cache` and a prebuilt Trunk binary.
- `.github/workflows/pages.yml` — on push to `main` (or manual dispatch): `trunk build --release --public-url "/uttt/"`, then the official GitHub Pages artifact flow (`actions/upload-pages-artifact` + `actions/deploy-pages`). No `gh-pages` branch.
- **One-time manual prerequisite** (cannot be automated): repo *Settings → Pages → Build and deployment → Source: "GitHub Actions"*. Until set, the deploy job fails.

## Docs / architecture records

`doc/` holds C4 model sources (`workspace.dsl`, `model.dsl`) rendered with **Structurizr Lite**, and Architecture Decision Records under `doc/adr/` (managed with `adr-tools`). ADR 0006 records the choice of Leptos over Dioxus (superseding the Blazor framework choice in ADR 0003); ADR 0007 records the GitHub Pages artifact deploy flow.
