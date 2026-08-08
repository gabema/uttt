//! Pure Ultimate Tic Tac Toe game logic.
//!
//! This crate has no UI or WASM dependencies. It owns the board model, win/draw
//! detection, and the interactive rules (turn order and the forced-board
//! constraint), and exposes a [`BoardView`] projection the UI renders from.

mod board;
mod game;

pub use board::{Board, Cell, Player, SmallBoard, SquareStatus};
pub use game::{BoardView, Constraint, Game, MoveError, MoveOutcome};
