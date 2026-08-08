## 1. Workspace scaffolding

- [ ] 1.1 Add a root `Cargo.toml` defining a workspace with members `crates/uttt-core` and `crates/uttt-web`
- [ ] 1.2 Create the `uttt-core` crate (library, no UI/WASM dependencies)
- [ ] 1.3 Add `rust-toolchain.toml` (or document the pinned toolchain) and the `wasm32-unknown-unknown` target
- [ ] 1.4 Update `.gitignore` for Rust (`/target`, Trunk `dist/`) and confirm `cargo build` succeeds

## 2. uttt-core: board + win detection (Phase 1)

- [ ] 2.1 Define the cell/mark type (open, X, O) and a `SquareStatus` (open, X, O, draw)
- [ ] 2.2 Represent small and large boards with flat `[Cell; 9]` arrays and a "new empty board" constructor
- [ ] 2.3 Implement win/draw/open detection over a `const` line table `[[usize; 3]; 8]`, excluding draws when evaluating lines at both levels
- [ ] 2.4 Port every `BoardTests.cs` case (small-square and large-square scenarios) to `cargo test`
- [ ] 2.5 Add a test asserting three drawn small boards do NOT win the large board; run `cargo test` green

## 3. uttt-core: Game state machine (Phase 2)

- [ ] 3.1 Define `Game` holding board state, current player, and the forced-board constraint (`-1` = anywhere)
- [ ] 3.2 Implement `play(cell)`: reject illegal moves (occupied, game over, forced-board violation); otherwise place mark, set next forced board, reset to "anywhere" when the target is won/full, and pass the turn
- [ ] 3.3 Return a move outcome that reports rejection and which small boards became newly won
- [ ] 3.4 Implement the rendering projection (per-cell marks, per-small-board status, playable-board set, overall status, current player)
- [ ] 3.5 Unit-test the move rule: forced-board redirect, reset-to-anywhere, rejection cases, turn alternation, and game-over lockout
- [ ] 3.6 Unit-test that the projection and move outcome match expected values for a scripted game

## 4. uttt-web: Leptos board rendering (Phase 3)

- [ ] 4.1 Create the `uttt-web` crate with Leptos, an `index.html`, and `Trunk.toml`; get `trunk serve` running at `/`
- [ ] 4.2 Render the 9×9 board and current-player / game-over indicators entirely from the engine projection
- [ ] 4.3 Wire cell clicks to `Game::play` and re-render from the new projection; rejected clicks are no-ops
- [ ] 4.4 Implement playable-board highlighting from the projection's playable set
- [ ] 4.5 Port `site.css` (grid, cells, highlight, winner faces) and confirm no game logic lives in the view

## 5. uttt-web: capture animation (Phase 4)

- [ ] 5.1 Derive each small board's winner backface from engine status (not stored flags)
- [ ] 5.2 Implement the pulse→flip animation via signals + `spawn_local` and a timer future, triggered by the move outcome's newly-won boards
- [ ] 5.3 Verify a re-render with no animation in progress still shows every won board's backface

## 6. CI/CD and GitHub Pages (Phase 5)

- [ ] 6.1 Rewrite `.github/workflows/ci.yml`: Rust toolchain + `wasm32` target, `Swatinem/rust-cache`, `cargo test`, and a WASM build check on PRs and pushes to `main`
- [ ] 6.2 Rewrite `.github/workflows/pages.yml`: build `trunk build --release --public-url "/uttt/"` and deploy via `upload-pages-artifact` + `deploy-pages` with the required `permissions` and `concurrency`
- [ ] 6.3 Install Trunk from a prebuilt binary (not `cargo install`) in both workflows
- [ ] 6.4 Document and perform the one-time manual step: repo Settings → Pages → Source: GitHub Actions
- [ ] 6.5 Deploy from `main` and verify a live load of `https://gabema.github.io/uttt/` renders the board and a move works (no asset 404s)

## 7. Retire .NET and update docs (Phase 6)

- [ ] 7.1 Delete `src/uttt.game/`, `src/uttt.app/`, `test/utt.game.test/`, `uttt.sln`, and all `.csproj` files
- [ ] 7.2 Flip ADR 0006 status to Accepted; add a new ADR recording the GitHub Pages artifact deploy flow
- [ ] 7.3 Mark ADR 0003 as superseded (framework portion) and update the C4 model under `doc/` to reflect the Rust/Leptos structure
- [ ] 7.4 Rewrite `CLAUDE.md` (and `readme.md` as needed) for the Rust/Trunk stack: commands, architecture, and the disposable-view / framework-free-core invariants
- [ ] 7.5 Confirm a clean checkout builds, tests, and deploys with no .NET remaining
