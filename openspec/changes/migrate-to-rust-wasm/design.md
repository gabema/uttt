## Context

The app is today a .NET 8 Blazor WebAssembly game deployed as static files to
GitHub Pages. Two facts drive this design:

1. **The interactive rules leaked into the view.** `src/uttt.game` only scores a
   square (`ToSpot`). Turn order, the forced-board constraint, and the "reset to
   play anywhere" rule live in the Blazor component `UttBoard.razor`. They are
   not unit-tested.
2. **The domain model fights the language.** The board is modeled recursively
   with named positional fields (`TopLeft…BottomRight`), forcing ~150 lines of
   `switch`-based accessors to translate a flat `0..80` index into nested
   records.

The migration's motivation is "all of the above": shed the multi-megabyte .NET
runtime (bundle size, startup performance), learn Rust idiomatically, and — as
an aspirational, not committed, goal — keep cross-platform reach possible. The
repository deploys to a GitHub **project** page (`github.com/gabema/uttt`, no
CNAME), so the site serves from `https://gabema.github.io/uttt/` — base path
`/uttt/`.

Framework selection (Leptos over Dioxus) is recorded in
[ADR 0006](../../../doc/adr/0006-use-leptos-for-rust-wasm-ui.md).

## Goals / Non-Goals

**Goals:**
- Replace the .NET/Blazor stack entirely with a Rust/WASM implementation.
- Put **all** game logic — rules, state, win/draw detection, and derived display
  facts — into a pure, framework-agnostic, unit-tested `uttt-core` crate.
- Keep the frontend **view-only and disposable**: it maps an engine projection
  to the DOM, forwards clicks, and owns nothing but presentation animation.
- Preserve exact game behavior, including the subtle rule that three *drawn*
  small boards do not win the large board.
- Ship to GitHub Pages via GitHub Actions with a fast, cached build.

**Non-Goals:**
- Building native desktop/mobile targets now. Cross-platform is preserved
  architecturally (via the reusable core), not delivered in this change.
- Using a Tauri webview as the cross-platform path — native rendering is
  preferred, so that path is explicitly rejected (see ADR 0006).
- Adding AI opponents, multiplayer, persistence, or any feature beyond parity
  with the current game.
- Changing the visual design beyond what the framework port requires.

## Decisions

### D1: Two-crate workspace — `uttt-core` (pure) + `uttt-web` (Leptos)
The workspace splits pure logic from UI. `uttt-core` has **no** `leptos`,
`wasm-bindgen`, or `web-sys` dependency — enforced as a hard rule — so its tests
run as fast native `cargo test` with no browser, and so it can be reused
unchanged under a different UI framework later.
*Alternative considered:* a single crate mixing logic and UI (rejected: kills
native testing and the cross-platform hedge).

### D2: Leptos + Trunk
Chosen over Dioxus per ADR 0006: best-in-class docs, smallest bundle, and its
fine-grained signal model teaches distinctively-Rust patterns, while Trunk +
`--public-url` is the documented GitHub Pages path.
*Alternative considered:* Dioxus (stronger native cross-platform, but that is
only aspirational here and its `dx` toolchain fits Pages less cleanly);
hand-written `wasm-bindgen`/`web-sys` (rejected: writing DOM plumbing for 81
animated cells buries the Rust-learning goal).

### D3: Idiomatic flat-array board model
Replace recursive `TopLeft…BottomRight` records with flat `[Cell; 9]` arrays
indexed `0..8`, and centralize win detection over a `const` line table
(`[[usize; 3]; 8]`). This deletes the `switch` accessor boilerplate and leans on
exhaustive `match`.
*Alternative considered:* a faithful 1:1 port of the named-field structure
(rejected by the user in favor of an idiomatic rewrite).

### D4: Disposable-view contract — projection out, intent in
`uttt-core` exposes a `Game` that computes every game fact and every derived
display fact as a **projection** (per-cell marks, per-small-board status,
playable-board set, overall status, current player). The view calls
`play(cell)` (the only intent) and re-renders from the projection; the move
operation returns which boards became won so the view can animate. The view
stores **no** authoritative game state.
- Litmus test for "disposable": a second frontend (CLI, `<canvas>`, or Dioxus)
  can reuse 100% of `uttt-core` and re-derive nothing.
- The permanent "show winner backface" is derived from engine status; only the
  transient pulse→flip transition is view-local. This removes the current
  Blazor bug where `flippedIndices` is a stored source of truth that duplicates
  "this board is won."

### D5: Official GitHub Pages artifact deploy
Replace the `peaceiris`/`gh-pages`-branch flow with the first-party
`upload-pages-artifact` + `deploy-pages` actions (no orphan branch, bypasses
Jekyll). CI installs the Rust toolchain + `wasm32-unknown-unknown`, uses
`Swatinem/rust-cache`, installs Trunk from a **prebuilt binary** (not
`cargo install`, which compiles from source), and builds with
`trunk build --release --public-url "/uttt/"`. Base path lives only in the
release workflow, so local `trunk serve` stays at `/`.
*Alternative considered:* keep the `gh-pages` branch + `peaceiris` (rejected:
requires `.nojekyll` care and an orphan branch; the first-party flow is
GitHub's current recommendation and the user asked for the GitHub build/deploy
actions).

## Risks / Trade-offs

- **[Wrong base path → white page of 404s]** → Pass `--public-url "/uttt/"` in
  the release build only; make Phase 5 acceptance a live load of
  `https://gabema.github.io/uttt/` with a working move, not just a green action.
- **[Losing the drawn-boards-don't-win rule in the rewrite]** → Port the
  existing `BoardTests.cs` theory cases into `cargo test` in Phase 1 before any
  UI work, so parity is verified mechanically.
- **[Rules migrating from view to core introduce behavior drift]** → Add engine
  tests for the move rule (forced board, reset-to-anywhere, rejection) — logic
  that is currently untested — so the new behavior is pinned.
- **[Slow CI from `cargo install trunk` / cold builds]** → Prebuilt Trunk binary
  + `Swatinem/rust-cache`; turns multi-minute runs into ~30s.
- **[Aspirational cross-platform never materializes cheaply]** → Accept it: the
  hedge is the framework-free `uttt-core`, which bounds a future native port to
  rewriting only the thin `uttt-web` layer. Not eliminated, but bounded.
- **[One-time manual Pages setting cannot be automated]** → Document
  "Settings → Pages → Source: GitHub Actions" as a task and a deployment
  prerequisite.

## Migration Plan

Phased; each phase runs and is independently shippable. Phases 1–2 (pure core)
deliver the biggest Rust-learning value before any browser is involved.

1. **`uttt-core` — board + win detection**, with `BoardTests.cs` cases ported to
   `cargo test`. Prove scoring parity first.
2. **`uttt-core` — `Game` state machine**: move rule, forced-board constraint,
   turn tracking, projection + move outcome. Fully unit-tested.
3. **`uttt-web` (Leptos)**: render the board from the projection, wire clicks to
   `play`, port highlighting.
4. **Capture animation**: pulse→flip via signals + `spawn_local` and a timer
   future; backface derived from engine status.
5. **CI/CD**: Rust toolchain, cached `trunk build --release --public-url
   "/uttt/"`, official Pages artifact deploy. Plus the manual Pages-source
   toggle.
6. **Retire .NET**: delete `src/`, `test/`, `uttt.sln`; update docs — supersede
   ADR 0003's framework portion, accept ADR 0006, add a deploy-flow ADR; refresh
   the C4 model and `CLAUDE.md`.

**Rollback:** `main` still serves the Blazor build until Phase 5 flips the Pages
source. If the Rust deploy misbehaves, revert the Pages workflow and source
setting to restore the .NET deployment; the old code remains in history (and in
the tree until Phase 6).

## Open Questions

- Which Trunk-install action/binary to pin in CI (e.g. a maintained
  `trunk-action` vs. a curled release tarball) — decide during Phase 5.
- Exact projection shape (a single `BoardView` struct vs. a few accessor methods
  on `Game`) — settle in Phase 2 as the engine API firms up.
- Whether to keep the on-page Blazor status/error overlay concept from the old
  `index.html` in some Rust-appropriate form, or drop it entirely (lean: drop).
