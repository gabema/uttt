# web-startup-performance

## Purpose

Fast perceived startup for the client-side-rendered WASM app: a size- and
compile-tuned wasm build, inlined critical CSS with no render-blocking request,
a static pre-boot app shell that paints before the wasm boots, and a mount
handoff that swaps the shell for the live board without double-rendering. The
shell is presentational only, preserving the disposable-view and
framework-free-engine invariants (see `game-ui`, `web-deployment`).

## Requirements

### Requirement: Size- and compile-tuned release build

The workspace release profile SHALL be tuned for small wasm output and faster
client-side compilation rather than Cargo's speed-optimized defaults. The
release build SHALL optimize for size, enable link-time optimization, use a
single codegen unit, abort on panic, and strip symbols. The tuning SHALL NOT
change game behavior or any engine test outcome.

#### Scenario: Release wasm is smaller than the default-profile build

- **WHEN** the app is built with `trunk build --release`
- **THEN** the emitted `*_bg.wasm` is smaller than the same build without a tuned
  `[profile.release]`, and the app still loads and plays correctly

#### Scenario: Engine tests are unaffected

- **WHEN** `cargo test -p uttt-core` runs
- **THEN** all engine tests pass, unchanged by the release-profile tuning

### Requirement: No render-blocking stylesheet request in the boot path

The critical CSS SHALL be inlined into the served `index.html` so that the boot
path contains no separate render-blocking stylesheet request, and any static
pre-boot shell is styled the instant the HTML is parsed.

#### Scenario: No external stylesheet request before first paint

- **WHEN** the production `index.html` is served and loaded
- **THEN** the document contains an inline `<style>` with the app's critical CSS
  and issues no blocking `<link rel="stylesheet">` request for it

#### Scenario: Shell is styled without waiting on a network round-trip

- **WHEN** the HTML is parsed but the wasm has not yet booted
- **THEN** the pre-boot shell renders with its intended layout and colors

### Requirement: Static pre-boot app shell paints before wasm

The served `index.html` body SHALL contain static, inert shell markup — a board
skeleton and title — that produces a meaningful First Contentful Paint and
Largest Contentful Paint before the wasm downloads and compiles. The shell SHALL
be presentational only: it SHALL NOT encode game rules, game state, or any
derived display fact, preserving the disposable-view and framework-free-engine
invariants.

#### Scenario: Meaningful frame paints before wasm boot

- **WHEN** the HTML is parsed but the wasm has not finished downloading or
  compiling
- **THEN** a styled board skeleton (and title) is visible on screen

#### Scenario: Shell carries no game truth

- **WHEN** the shell markup is inspected
- **THEN** it contains no marks, no win/draw status, no legal-move highlighting,
  and no game logic — only inert placeholder structure

### Requirement: Shell-to-live-board mount handoff

When the wasm boots, the Leptos app SHALL replace the static shell with the live
board rather than rendering alongside it, so the shell is never visible
simultaneously with, or duplicated by, the mounted app.

#### Scenario: Live board replaces the shell

- **WHEN** the wasm boots and the Leptos app mounts
- **THEN** the static shell is removed or replaced and the live, interactive
  board is the only board rendered

#### Scenario: No duplicated board after mount

- **WHEN** the app has finished mounting
- **THEN** exactly one board is present in the DOM (no leftover shell alongside
  the mounted app)
