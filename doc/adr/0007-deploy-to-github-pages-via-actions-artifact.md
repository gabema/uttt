# 7. Deploy to GitHub Pages via the Actions artifact flow

Date: 2026-08-08

## Status

Accepted

Refines [5. Deploy via Github Pages](0005-deploy-via-github-pages.md) for the
Rust/WASM stack, and follows the migration recorded in
[6. Use Leptos over Dioxus for the Rust/WASM UI](0006-use-leptos-for-rust-wasm-ui.md).

## Context

The migration to Rust/WASM (ADR 0006) replaced `dotnet publish` with a Trunk
build, so the deployment pipeline had to be rebuilt. The app is served from a
GitHub **project** page (`https://gabema.github.io/uttt/`), so assets must
resolve under the `/uttt/` base path.

Two deployment mechanisms were available:

- The previous flow pushed the built site to a `gh-pages` branch via
  `peaceiris/actions-gh-pages`. It works but keeps an orphan branch and runs
  Jekyll (which ignores `_`-prefixed paths unless `.nojekyll` is present).
- The first-party flow uploads a Pages artifact (`actions/upload-pages-artifact`)
  and deploys it (`actions/deploy-pages`). It needs no branch, bypasses Jekyll,
  and is GitHub's current recommendation.

## Decision

Deploy with the first-party GitHub Actions Pages artifact flow. On push to
`main`, a build job runs `trunk build --release --public-url "/uttt/"` and
uploads `crates/uttt-web/dist` as the Pages artifact; a deploy job publishes it
with `actions/deploy-pages`. CI installs the Rust toolchain with the
`wasm32-unknown-unknown` target, caches with `Swatinem/rust-cache`, and installs
Trunk from a prebuilt binary (not `cargo install`, which compiles from source).

## Consequences

1. No `gh-pages` branch and no Jekyll considerations.
2. A one-time manual setting is required that cannot be automated:
   repo Settings -> Pages -> Source: "GitHub Actions". Until it is set, the
   deploy job fails.
3. The `/uttt/` base path lives only in the release build command, so local
   `trunk serve` continues to serve from `/`.
4. The workflow needs `pages: write` and `id-token: write` permissions and a
   `pages` concurrency group, per the artifact flow's requirements.
