## ADDED Requirements

### Requirement: WASM build with correct base path

The project SHALL build to static WASM/JS/HTML assets with Trunk. The production
build SHALL target the GitHub Pages project-page base path `/uttt/` so that all
assets resolve when served from `https://gabema.github.io/uttt/`. Local
development builds SHALL serve from `/` without the production base path.

#### Scenario: Production assets resolve under the project path

- **WHEN** the app is built for release with the production base path and served
  from `https://gabema.github.io/uttt/`
- **THEN** the WASM, JS, and CSS assets load without 404s and the board renders

#### Scenario: Local dev serves from root

- **WHEN** a developer runs the local Trunk dev server
- **THEN** the app is served from `/` without needing the `/uttt/` base path

### Requirement: Continuous integration gate

CI SHALL run on pull requests to any branch and on pushes to `main`, using a Rust
toolchain with the `wasm32-unknown-unknown` target. CI SHALL run the engine
tests and verify the WASM app builds. Dependency compilation SHALL be cached to
keep runs fast.

#### Scenario: PR runs tests and build

- **WHEN** a pull request is opened
- **THEN** CI runs `cargo test` for the engine and builds the WASM app, failing
  the check if either fails

### Requirement: GitHub Pages deployment

On push to `main`, the project SHALL build the release WASM app and deploy it to
GitHub Pages via the official GitHub Actions Pages artifact flow
(`upload-pages-artifact` + `deploy-pages`), with the workflow permissions and
concurrency the flow requires. The deployment SHALL NOT depend on a `gh-pages`
branch.

#### Scenario: Push to main deploys the site

- **WHEN** a commit is pushed to `main`
- **THEN** the workflow builds the release app and publishes it to GitHub Pages
  through the Pages artifact flow

#### Scenario: One-time Pages source setup is documented

- **WHEN** the deployment is set up
- **THEN** the required manual repository setting (Pages source set to GitHub
  Actions) is documented as a prerequisite, since it cannot be automated
