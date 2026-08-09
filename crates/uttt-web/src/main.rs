//! Ultimate Tic Tac Toe — Leptos frontend.
//!
//! This crate is the disposable view layer: it holds the authoritative game
//! state in a single signal, renders entirely from `uttt_core`'s projection,
//! forwards clicks to `Game::play`, and owns only the transient capture
//! animation. It computes no game logic of its own.

use std::collections::HashSet;

use leptos::prelude::*;
use uttt_core::{Cell, Game, Player, SquareStatus};

fn main() {
    // Panic-message formatting is a dev aid; under `panic = "abort"` in release
    // it adds bytes and compile time for no runtime benefit. Keep it in debug.
    #[cfg(debug_assertions)]
    console_error_panic_hook::set_once();
    mount();
}

/// Mount the live app into the `#app` container, replacing the static pre-boot
/// shell that `index.html` paints before the wasm boots. `mount_to_body` would
/// *append* — leaving the shell rendered alongside a duplicate board — so we
/// mount into the container and clear the shell out of it first.
fn mount() {
    use leptos::wasm_bindgen::JsCast;
    let app = leptos::web_sys::window()
        .and_then(|w| w.document())
        .and_then(|d| d.get_element_by_id("app"))
        .expect("#app mount container is present in index.html");
    // Drop the static shell so exactly one board exists after mount.
    app.set_inner_html("");
    let app: leptos::web_sys::HtmlElement = app.unchecked_into();
    leptos::mount::mount_to(app, || view! { <App /> }).forget();
}

#[component]
fn App() -> impl IntoView {
    // The only authoritative state. `Game` is `Copy`, so we read a copy, mutate
    // it, and write it back.
    let game = RwSignal::new(Game::new());
    // Transient, view-only animation state: boards currently mid-pulse.
    let pulsing = RwSignal::new(HashSet::<usize>::new());

    view! {
        <div class="utt-container">
            <div class="next-player">
                {move || match game.get().current_player() {
                    Player::X => "Next: X",
                    Player::O => "Next: O",
                }}
            </div>
            {move || {
                let status = game.get().status();
                (status != SquareStatus::InPlay)
                    .then(|| view! { <div class="game-over">"Game Over — " {status_label(status)}</div> })
            }}
            <div class="utt-grid">
                {(0..9).map(|big| small_board(big, game, pulsing)).collect_view()}
            </div>
        </div>
    }
}

/// Render one small board, driven entirely by the engine projection.
fn small_board(
    big: usize,
    game: RwSignal<Game>,
    pulsing: RwSignal<HashSet<usize>>,
) -> impl IntoView {
    view! {
        <div
            class="small-board"
            class:highlight=move || game.get().view().playable[big]
            // Permanent backface visibility is DERIVED from engine status; the
            // pulse briefly holds the flip so the animation reads pulse -> flip.
            class:flipped=move || {
                let resolved = game.get().view().small_status[big] != SquareStatus::InPlay;
                resolved && !pulsing.get().contains(&big)
            }
            class:pulse=move || pulsing.get().contains(&big)
        >
            <div class="flip-inner">
                <div class="front">
                    {(0..9)
                        .map(|i| {
                            let global = big * 9 + i;
                            view! {
                                <div
                                    class="cell"
                                    class:x=move || {
                                        matches!(game.get().view().cells[global], Cell::Mark(Player::X))
                                    }
                                    class:o=move || {
                                        matches!(game.get().view().cells[global], Cell::Mark(Player::O))
                                    }
                                    on:click=move |_| play(game, pulsing, global)
                                >
                                    {move || symbol(game.get().view().cells[global])}
                                </div>
                            }
                        })
                        .collect_view()}
                </div>
                <div class="back">
                    {move || backface(game.get().view().small_status[big])}
                </div>
            </div>
        </div>
    }
}

/// Forward a click to the engine and, on a capture, run the pulse animation.
fn play(game: RwSignal<Game>, pulsing: RwSignal<HashSet<usize>>, global: usize) {
    let mut g = game.get_untracked();
    // Rejected moves are a no-op: state is only written on success.
    if let Ok(outcome) = g.play(global) {
        game.set(g);
        for b in outcome.captured {
            pulsing.update(|p| {
                p.insert(b);
            });
            leptos::task::spawn_local(async move {
                gloo_timers::future::TimeoutFuture::new(400).await;
                pulsing.update(|p| {
                    p.remove(&b);
                });
            });
        }
    }
}

fn symbol(cell: Cell) -> &'static str {
    match cell {
        Cell::Mark(Player::X) => "X",
        Cell::Mark(Player::O) => "O",
        Cell::Empty => "",
    }
}

fn status_label(status: SquareStatus) -> &'static str {
    match status {
        SquareStatus::Won(Player::X) => "Winner: X",
        SquareStatus::Won(Player::O) => "Winner: O",
        _ => "Draw",
    }
}

fn backface(status: SquareStatus) -> impl IntoView {
    let (class, mark) = match status {
        SquareStatus::Won(Player::X) => ("big-winner x", "X"),
        SquareStatus::Won(Player::O) => ("big-winner o", "O"),
        SquareStatus::Draw => ("big-winner tie", "—"),
        SquareStatus::InPlay => ("big-winner blank", ""),
    };
    view! { <div class=class>{mark}</div> }
}
