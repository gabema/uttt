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
