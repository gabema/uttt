# 3. Use Blazor for progressive web app framework

Date: 2025-10-17

## Status

Superseded by [6. Use Leptos over Dioxus for the Rust/WASM UI](0006-use-leptos-for-rust-wasm-ui.md)

The framework choice recorded here (Blazor/.NET) was replaced by a Rust/WASM
(Leptos) implementation. See ADR 0006 for the rationale.

## Context

The issue motivating this decision, and any context that influences or constrains the decision.
We need to decide on a framework for developing the Progress web app.

## Decision

[Blazor](https://dotnet.microsoft.com/en-us/apps/aspnet/web-apps/blazor) will be used to generate the static Progressive Web App.

## Consequences

What becomes easier or more difficult to do and any risks introduced by the change that will need to be mitigated.
1. We can leverage our knowledge and expertise with .NET.
1. We can grow our skills with Progressive Web Apps built with .NET.
1. Since this is the first Blazor web assembly based project there are new skills and understanding that must be attained.