# uttt.app (Prototype)

This is a small Blazor WebAssembly prototype that demonstrates a kanban column and an Ultimate Tic Tac Toe board layout based on the project mockup and ADRs in `doc/adr`.

Key decisions implemented:
- Uses .NET (per ADR #3) and is scaffolded as a Blazor WebAssembly PWA candidate.

To build and run locally:

1. From the repository root, run the tests and build existing projects:

   dotnet build

2. To run the Blazor app (development server):

   dotnet run --project src/uttt.app/uttt.app.csproj

This prototype is intentionally minimal: it focuses on layout and wiring. Next steps: wire game logic from `src/uttt.game`, add drag/drop interactions, unit tests, and PWA manifest.

Continuous Integration
----------------------
This repo contains a GitHub Actions workflow at `.github/workflows/ci.yml` that runs on pull requests and pushes to `main`. It restores, builds and runs tests on Ubuntu using .NET 8.

To run the same checks locally:

dotnet restore
dotnet build --configuration Release
dotnet test


Publishing (GitHub Pages)
-------------------------
The repository includes a workflow `.github/workflows/pages.yml` which publishes the Blazor WebAssembly app to the `gh-pages` branch whenever commits are pushed to `main` (or when manually triggered). It uses `dotnet publish` to produce the static files and `peaceiris/actions-gh-pages` to push them to the `gh-pages` branch. The published directory is `publish/wwwroot` from the `dotnet publish` output.

Configure GitHub Pages to serve the published site
-------------------------------------------------
1. Go to your repository on GitHub (e.g. https://github.com/<owner>/<repo>).
2. Open `Settings` -> `Pages` (left-hand menu).
3. Under "Build and deployment" -> "Source", choose "Deploy from a branch" and select the branch `gh-pages` and the folder `/` (root). Save.
4. After the next push to `main` the workflow will publish to `gh-pages` and GitHub Pages will serve the site. The site URL will be shown in the Pages settings panel (usually https://<owner>.github.io/<repo>/).

Optional settings:
- Enforce HTTPS: in Pages settings, toggle "Enforce HTTPS" if you want HTTPS-only access (recommended).
- Custom domain: set a custom domain in the Pages settings; the workflow will still publish to `gh-pages` — you'll need to point DNS to GitHub Pages as documented by GitHub.

Troubleshooting
---------------
- If Pages does not show updates after a publish, verify the Actions run completed successfully and that the `gh-pages` branch contains the static files (check the `publish/wwwroot` output in the action logs).
- If you prefer the Pages source to be `gh-pages` with a `/docs` folder instead, update the `pages.yml` workflow's `publish_dir` and the Pages settings accordingly.

