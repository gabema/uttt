## Why

The app is a Blazor WebAssembly (.NET 8) game that ships the entire .NET runtime
to the browser before it can run, and its interactive game rules live inside the
UI component (`UttBoard.razor`) rather than in the testable game library.
Migrating to Rust/WASM lets us shed the multi-megabyte runtime (smaller bundle,
faster startup), learn Rust on a real project, and re-layer the design so all
game logic lives in a pure, unit-tested core with a thin, disposable frontend.
Cross-platform reach is an aspirational goal preserved architecturally, not a
committed near-term target (see design.md and ADR 0006).

## What Changes

- **BREAKING**: Remove the entire .NET/Blazor stack — `src/uttt.game`,
  `src/uttt.app`, `test/utt.game.test`, and `uttt.sln`. No C# remains.
- Introduce a Cargo workspace with two crates:
  - `uttt-core` — pure game logic (board, win detection, and the full move-rule
    state machine), with native `cargo test` coverage. **No UI/WASM
    dependencies.**
  - `uttt-web` — a Leptos frontend compiled to WASM, built and served with Trunk.
- Move the interactive game rules (turn order, forced-board constraint, reset to
  "play anywhere") out of the view and into `uttt-core`.
- Rewrite the board model idiomatically: flat `[Cell; 9]` arrays indexed `0..8`
  in place of the recursive `TopLeft…BottomRight` named fields, eliminating the
  `switch`-based accessor boilerplate.
- Establish a disposable-view contract: `uttt-core` computes every game fact and
  every derived display fact (a projection); `uttt-web` only maps that
  projection to the DOM, forwards clicks, and owns presentation-only animation.
- **BREAKING**: Replace CI/CD. Swap the .NET GitHub Actions for a Rust
  toolchain: `cargo test` in CI and a cached `trunk build --release` deployed to
  GitHub Pages via the official Pages artifact flow (with `--public-url "/uttt/"`
  for the project-page base path).
- Update architecture docs: supersede the framework portion of ADR 0003, accept
  ADR 0006 (Leptos), and add an ADR recording the Pages deploy flow.

## Capabilities

### New Capabilities
- `game-engine`: The pure game domain — board representation, win/draw
  detection, legal-move rules, turn and forced-board state, and the projection
  the UI renders from. Framework-agnostic; lives in `uttt-core`.
- `game-ui`: The interactive Leptos frontend — rendering the board from the
  engine's projection, forwarding player moves, highlighting playable boards,
  and the capture (pulse → flip) animation.
- `web-deployment`: Building the WASM app with Trunk and shipping it to GitHub
  Pages via GitHub Actions, including the project-page base path and the CI
  test/build gate.

### Modified Capabilities
<!-- None. There are no existing OpenSpec specs; this is a greenfield capture. -->

## Impact

- **Removed**: `src/uttt.game/`, `src/uttt.app/`, `test/utt.game.test/`,
  `uttt.sln`, and all `.csproj` files.
- **Added**: `Cargo.toml` (workspace), `crates/uttt-core/`, `crates/uttt-web/`,
  `index.html`, `Trunk.toml`.
- **Toolchain**: .NET 8 SDK → Rust toolchain with the `wasm32-unknown-unknown`
  target, plus Trunk. Contributors need `rustup` and `trunk` instead of
  `dotnet`.
- **CI/CD**: `.github/workflows/ci.yml` and `pages.yml` rewritten. The official
  Pages artifact flow requires a one-time manual repo setting
  (Settings → Pages → Source: GitHub Actions) that cannot be automated.
- **Docs**: `doc/adr/` (0003 superseded in part, 0006 accepted, new deploy ADR),
  and the C4 model under `doc/` describing the old Blazor structure.
- **Behavior parity**: The subtle win/draw rule — three *drawn* small boards do
  not win the large board — must be preserved and covered by ported tests.
