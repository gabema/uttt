//! The interactive game: turn order, the forced-board rule, and the projection
//! the UI renders from. This is the logic that used to live in the Blazor
//! `UttBoard.razor` component; it now lives here, fully unit-tested.

use crate::board::{Board, Cell, Player, SquareStatus};

/// Where the current player is allowed to move.
#[derive(Clone, Copy, PartialEq, Eq, Debug)]
pub enum Constraint {
    /// Must play in this specific small board (`0..8`).
    Board(usize),
    /// May play in any still-playable small board.
    Anywhere,
}

/// Why a move was rejected. A rejected move leaves game state unchanged.
#[derive(Clone, Copy, PartialEq, Eq, Debug)]
pub enum MoveError {
    /// The game is already won or drawn.
    GameOver,
    /// The cell index was not in `0..81`.
    OutOfRange,
    /// The targeted cell is already occupied.
    CellTaken,
    /// The forced-board constraint forbids playing in that board.
    WrongBoard,
}

/// The result of a successful move.
#[derive(Clone, PartialEq, Eq, Debug, Default)]
pub struct MoveOutcome {
    /// Small boards (`0..8`) that became won as a direct result of this move.
    /// At most one board can be captured per move.
    pub captured: Vec<usize>,
}

/// Everything a frontend needs to render — computed here so the view derives
/// no game facts of its own.
#[derive(Clone, PartialEq, Eq, Debug)]
pub struct BoardView {
    /// Flat `0..80` cell contents (`cell = big * 9 + small`).
    pub cells: [Cell; 81],
    /// Resolved status of each small board (`0..8`).
    pub small_status: [SquareStatus; 9],
    /// Which small boards the current player may play in right now.
    pub playable: [bool; 9],
    /// Overall game status.
    pub overall: SquareStatus,
    /// Whose turn it is.
    pub next_player: Player,
}

/// Authoritative game state. The view holds one of these (or a handle to it)
/// and never stores game state of its own.
#[derive(Clone, Copy, PartialEq, Eq, Debug)]
pub struct Game {
    board: Board,
    current: Player,
    constraint: Constraint,
}

impl Game {
    /// A new game: empty board, X to move, free choice of board.
    pub fn new() -> Self {
        Game {
            board: Board::new(),
            current: Player::X,
            constraint: Constraint::Anywhere,
        }
    }

    /// The player to move.
    pub fn current_player(&self) -> Player {
        self.current
    }

    /// The overall game status.
    pub fn status(&self) -> SquareStatus {
        self.board.status()
    }

    /// The current move constraint.
    pub fn constraint(&self) -> Constraint {
        self.constraint
    }

    /// Whether small board `big` is playable under the current constraint.
    fn is_allowed(&self, big: usize) -> bool {
        match self.constraint {
            Constraint::Board(b) => b == big,
            Constraint::Anywhere => self.board.boards[big].is_playable(),
        }
    }

    /// Attempt to play the cell at flat index `0..81`.
    ///
    /// On success the mark is placed, the opponent's forced board is set to the
    /// local index just played (or `Anywhere` if that board is already
    /// won/full), and the turn passes. On failure the state is unchanged.
    pub fn play(&mut self, cell: usize) -> Result<MoveOutcome, MoveError> {
        if self.status() != SquareStatus::InPlay {
            return Err(MoveError::GameOver);
        }
        if cell >= 81 {
            return Err(MoveError::OutOfRange);
        }
        let (big, small) = (cell / 9, cell % 9);
        if !self.is_allowed(big) {
            return Err(MoveError::WrongBoard);
        }
        if self.board.boards[big].cells[small] != Cell::Empty {
            return Err(MoveError::CellTaken);
        }

        // Place the mark, noting whether this move captured the small board.
        let before = self.board.boards[big].status();
        self.board.boards[big].cells[small] = Cell::Mark(self.current);
        let after = self.board.boards[big].status();

        let mut captured = Vec::new();
        if before == SquareStatus::InPlay {
            if let SquareStatus::Won(_) = after {
                captured.push(big);
            }
        }

        // The local index just played dictates the opponent's target board;
        // if that board is already resolved/full, the opponent plays anywhere.
        self.constraint = if self.board.boards[small].is_playable() {
            Constraint::Board(small)
        } else {
            Constraint::Anywhere
        };

        self.current = self.current.other();
        Ok(MoveOutcome { captured })
    }

    /// Build the projection the UI renders from.
    pub fn view(&self) -> BoardView {
        let cells = std::array::from_fn(|i| self.board.boards[i / 9].cells[i % 9]);
        let small_status = std::array::from_fn(|i| self.board.boards[i].status());
        let overall = self.board.status();

        // No board is playable once the game is over.
        let playable =
            std::array::from_fn(|big| overall == SquareStatus::InPlay && self.is_allowed(big));

        BoardView {
            cells,
            small_status,
            playable,
            overall,
            next_player: self.current,
        }
    }
}

impl Default for Game {
    fn default() -> Self {
        Self::new()
    }
}

#[cfg(test)]
mod tests {
    use super::*;

    fn cell(big: usize, small: usize) -> usize {
        big * 9 + small
    }

    #[test]
    fn new_game_is_x_to_move_anywhere() {
        let g = Game::new();
        assert_eq!(g.current_player(), Player::X);
        assert_eq!(g.constraint(), Constraint::Anywhere);
        assert_eq!(g.status(), SquareStatus::InPlay);
    }

    #[test]
    fn legal_move_places_mark_and_passes_turn() {
        let mut g = Game::new();
        let outcome = g.play(cell(4, 0)).expect("legal move");
        assert!(outcome.captured.is_empty());
        assert_eq!(g.view().cells[cell(4, 0)], Cell::Mark(Player::X));
        assert_eq!(g.current_player(), Player::O);
    }

    #[test]
    fn move_dictates_opponents_forced_board() {
        let mut g = Game::new();
        // X plays local index 7 of board 4 -> O is forced into board 7.
        g.play(cell(4, 7)).expect("legal move");
        assert_eq!(g.constraint(), Constraint::Board(7));
        // O may only play in board 7.
        assert_eq!(g.play(cell(3, 0)), Err(MoveError::WrongBoard));
        g.play(cell(7, 0)).expect("legal move in forced board");
    }

    #[test]
    fn occupied_cell_is_rejected() {
        let mut g = Game::new();
        g.play(cell(4, 4)).expect("legal move");
        // O is forced into board 4; playing the same occupied cell is rejected.
        assert_eq!(g.play(cell(4, 4)), Err(MoveError::CellTaken));
    }

    #[test]
    fn out_of_range_is_rejected() {
        let mut g = Game::new();
        assert_eq!(g.play(81), Err(MoveError::OutOfRange));
    }

    #[test]
    fn rejected_move_leaves_state_unchanged() {
        let mut g = Game::new();
        let before = g;
        assert!(g.play(200).is_err());
        assert_eq!(g, before);
    }

    #[test]
    fn winning_a_small_board_is_reported_as_captured() {
        // Drive X to win board 0 across the top row, keeping O's replies legal.
        let mut g = Game::new();
        // X: board0 idx0  -> O forced to board0
        g.play(cell(0, 0)).unwrap();
        // O: board0 idx3  -> X forced to board3
        g.play(cell(0, 3)).unwrap();
        // X: board3 idx1  -> O forced to board1
        g.play(cell(3, 1)).unwrap();
        // O: board1 idx3  -> X forced to board3
        g.play(cell(1, 3)).unwrap();
        // X: board3 idx2  -> O forced to board2
        g.play(cell(3, 2)).unwrap();
        // O: board2 idx3  -> X forced to board3
        g.play(cell(2, 3)).unwrap();
        // X: board3 idx0 completes top row of board 3 -> capture board 3
        let outcome = g.play(cell(3, 0)).unwrap();
        assert_eq!(outcome.captured, vec![3]);
        assert_eq!(g.view().small_status[3], SquareStatus::Won(Player::X));
    }

    #[test]
    fn winning_move_pointing_at_the_won_board_resets_to_anywhere() {
        // X wins board 1 with a final move at local index 1. That local index
        // would force the opponent back into board 1 — but board 1 just became
        // won, so the constraint must reset to Anywhere.
        let mut g = Game::new();
        g.play(cell(1, 0)).unwrap(); // X: board1 idx0 -> O forced to board0
        g.play(cell(0, 1)).unwrap(); // O: board0 idx1 -> X forced to board1
        g.play(cell(1, 2)).unwrap(); // X: board1 idx2 -> O forced to board2
        g.play(cell(2, 1)).unwrap(); // O: board2 idx1 -> X forced to board1
        let outcome = g.play(cell(1, 1)).unwrap(); // X completes board1 top row

        assert_eq!(outcome.captured, vec![1]);
        assert_eq!(g.view().small_status[1], SquareStatus::Won(Player::X));
        assert_eq!(g.constraint(), Constraint::Anywhere);
        assert_eq!(g.current_player(), Player::O);
    }

    #[test]
    fn view_marks_playable_boards() {
        let mut g = Game::new();
        // Free choice: every board playable.
        assert!(g.view().playable.iter().all(|&p| p));
        g.play(cell(4, 2)).unwrap(); // O forced into board 2
        let v = g.view();
        assert!(v.playable[2]);
        assert_eq!(v.playable.iter().filter(|&&p| p).count(), 1);
    }
}
