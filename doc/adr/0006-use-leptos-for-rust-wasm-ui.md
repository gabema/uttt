# 6. Use Leptos over Dioxus for the Rust/WASM UI

Date: 2026-08-08

## Status

Proposed

Relates to [3. Use Blazor for progressive web app framework](0003-use-blazor-for-cross-platform-deployability.md). This decision is contingent on the planned migration off Blazor/.NET to Rust/WASM; if and when that migration is accepted, this ADR partially supersedes the framework portion of ADR 0003.

## Context

We are planning to migrate the Ultimate Tic Tac Toe app from Blazor/.NET to
Rust/WASM. The primary driver is learning Rust "for the craft" and producing an
idiomatic result, while remaining shippable to GitHub Pages via GitHub Actions.

The app is a component-style UI (a board of 81 clickable cells with dynamic
highlighting and an async flip/pulse capture animation), so a Rust UI framework
with a component model is a natural fit. The two leading candidates are:

- **Leptos** — fine-grained reactivity with signals, no virtual DOM; views wire
  directly to the DOM nodes they touch. Web-first. Smallest bundle in the
  ecosystem. Standard build path is Trunk. Documentation (the Leptos Book) is
  among the best in the Rust ecosystem.
- **Dioxus** — virtual-DOM, React-like model (`rsx!`, hooks). Its headline is
  cross-platform reach (web + desktop + mobile + TUI from one codebase). Its
  idiomatic build tool is the `dx` CLI rather than Trunk.

Evaluated against our stated priorities:

- **Learning Rust / craft.** Leptos's signal + `move`-closure model forces
  direct engagement with ownership, `Copy` semantics, and borrow rules — the
  most distinctively-Rust concepts. Dioxus deliberately smooths these over to
  feel like React, so it teaches Rust syntax more than the language's model.
- **Idiomatic result.** Fine-grained, no-VDOM is the clean modern Rust-web
  shape. Dioxus is idiomatic chiefly for a React background.
- **GitHub Pages deploy.** Leptos + Trunk with `--public-url "/uttt/"` is the
  documented, well-trodden path and matches our planned deploy design. Dioxus
  web works, but its idiomatic `dx` toolchain has a less standardized Pages
  base-path story, working against the plan.
- **Docs.** The Leptos Book is a strong advantage for a learning-focused effort.

Cross-platform reach (web + desktop + mobile) is Dioxus's main edge and is the
same rationale ADR 0003 used to choose Blazor. It is an acknowledged goal here,
but an **aspirational** one: the committed near-term target is the web, with
desktop/mobile a door we want to keep open rather than a roadmap item. A future
native build is preferred over a webview wrapper, which rules out the otherwise
tempting "Leptos web app inside a Tauri shell" path as the cross-platform
answer.

Because cross-platform is aspirational rather than committed, we optimize for the
real near-term target (web), where every concrete priority — craft, bundle size,
performance, and GitHub Pages fit — favors Leptos. The aspiration is protected
not by the framework but by the architecture: `uttt-core` is a pure,
framework-agnostic Rust crate (types, win detection, game state machine, all
unit-tested), so it can be reused unchanged under a different UI framework. If a
committed native cross-platform requirement ever emerges, only the thin
`uttt-web` layer (essentially one Leptos component plus its animation) would be
rewritten — most plausibly in Dioxus — while `uttt-core` is reused verbatim.

## Decision

Use **Leptos** as the Rust/WASM UI framework for the migrated app. Build and
serve with **Trunk**.

## Consequences

1. The signal/closure ownership model gives high learning value but a steeper
   initial curve than Dioxus's React-like hooks — which is acceptable, and
   arguably desirable, given the learning goal.
2. The async flip/pulse capture animation maps cleanly onto signals plus
   `spawn_local` and a timer future (e.g. `gloo-timers`).
3. Trunk is the build tool, aligning with the planned GitHub Pages deploy that
   passes `--public-url "/uttt/"` for the project-page base path.
4. The result is web-first. A native desktop/mobile target is not turnkey the
   way it would be under Dioxus, and a webview wrapper (Tauri) is explicitly
   rejected as the path since native rendering is preferred. The mitigation is
   the core/UI seam: a future native build reuses the framework-agnostic
   `uttt-core` crate and rewrites only the small `uttt-web` layer (likely in
   Dioxus). This keeps the cost of the aspiration bounded and low rather than
   eliminating it.
5. Smallest-in-class bundle size, which supports the broader migration goal of
   shedding the multi-megabyte .NET runtime that Blazor ships.
