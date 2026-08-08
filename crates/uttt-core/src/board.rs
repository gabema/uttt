//! Board representation and win/draw detection.
//!
//! Both the small square (3x3 of cells) and the large square (3x3 of small
//! boards) are scored by the same [`resolve`] function, so the win rule lives
//! in exactly one place — mirroring the original `SpotStateUtils.ToSpot`.

/// A player: X or O.
#[derive(Clone, Copy, PartialEq, Eq, Debug, Hash)]
pub enum Player {
    X,
    O,
}

impl Player {
    /// The other player — whose turn it becomes after this one moves.
    pub fn other(self) -> Player {
        match self {
            Player::X => Player::O,
            Player::O => Player::X,
        }
    }
}

/// The contents of a single spot on a small board.
#[derive(Clone, Copy, PartialEq, Eq, Debug, Default)]
pub enum Cell {
    #[default]
    Empty,
    Mark(Player),
}

impl Cell {
    /// Map a cell to the sub-square status it contributes for line evaluation.
    fn as_status(self) -> SquareStatus {
        match self {
            Cell::Empty => SquareStatus::InPlay,
            Cell::Mark(p) => SquareStatus::Won(p),
        }
    }
}

/// The resolved state of a 3x3 square, at either level.
#[derive(Clone, Copy, PartialEq, Eq, Debug)]
pub enum SquareStatus {
    /// Has at least one open spot and no winner yet.
    InPlay,
    /// Won by a player (three in a row).
    Won(Player),
    /// Full / fully resolved with no winner.
    Draw,
}

/// The eight winning lines, as index triples into a flat `0..8` square.
const LINES: [[usize; 3]; 8] = [
    [0, 1, 2], // rows
    [3, 4, 5],
    [6, 7, 8],
    [0, 3, 6], // columns
    [1, 4, 7],
    [2, 5, 8],
    [0, 4, 8], // diagonals
    [2, 4, 6],
];

/// Score nine sub-square statuses into one.
///
/// A line of three matching `Won(p)` resolves to `Won(p)`. `Draw` never
/// completes a line — three drawn sub-squares do NOT win the parent, matching
/// the original `includeDraw: false` at both levels. With no winning line, the
/// square is `InPlay` if any sub-square is still `InPlay`, otherwise `Draw`.
fn resolve(squares: [SquareStatus; 9]) -> SquareStatus {
    for [a, b, c] in LINES {
        let first = squares[a];
        if matches!(first, SquareStatus::Won(_)) && first == squares[b] && first == squares[c] {
            return first;
        }
    }
    if squares.contains(&SquareStatus::InPlay) {
        SquareStatus::InPlay
    } else {
        SquareStatus::Draw
    }
}

/// A single 3x3 board of cells, indexed `0..8` (row-major).
#[derive(Clone, Copy, PartialEq, Eq, Debug)]
pub struct SmallBoard {
    pub cells: [Cell; 9],
}

impl SmallBoard {
    /// An empty small board.
    pub fn new() -> Self {
        SmallBoard {
            cells: [Cell::Empty; 9],
        }
    }

    /// The resolved status of this small board.
    pub fn status(&self) -> SquareStatus {
        resolve(self.cells.map(Cell::as_status))
    }

    /// Whether this board can still be played in (open and unresolved).
    pub fn is_playable(&self) -> bool {
        self.status() == SquareStatus::InPlay
    }
}

impl Default for SmallBoard {
    fn default() -> Self {
        Self::new()
    }
}

/// The full Ultimate board: a 3x3 of [`SmallBoard`]s, indexed `0..8`.
#[derive(Clone, Copy, PartialEq, Eq, Debug)]
pub struct Board {
    pub boards: [SmallBoard; 9],
}

impl Board {
    /// A new, fully-open board.
    pub fn new() -> Self {
        Board {
            boards: [SmallBoard::new(); 9],
        }
    }

    /// The resolved status of the overall game.
    pub fn status(&self) -> SquareStatus {
        resolve(std::array::from_fn(|i| self.boards[i].status()))
    }
}

impl Default for Board {
    fn default() -> Self {
        Self::new()
    }
}

#[cfg(test)]
mod tests {
    use super::*;

    // Readable board literals — these mirror the 3x3 visually in each test.
    const E: Cell = Cell::Empty;
    const X: Cell = Cell::Mark(Player::X);
    const O: Cell = Cell::Mark(Player::O);

    fn small(cells: [Cell; 9]) -> SmallBoard {
        SmallBoard { cells }
    }

    // A full small board with no line — resolves to Draw. (Ported from
    // BoardTests.NewDrawSquare.)
    fn draw_square() -> SmallBoard {
        small([X, X, O, O, O, X, X, O, X])
    }

    fn win_square(p: Player) -> SmallBoard {
        let m = Cell::Mark(p);
        small([m, m, m, E, E, E, E, E, E])
    }

    fn open_square() -> SmallBoard {
        SmallBoard::new()
    }

    // --- Small-square scenarios (ported from BoardTests.SmallSquareTests) ---

    #[test]
    fn small_all_open_is_in_play() {
        assert_eq!(open_square().status(), SquareStatus::InPlay);
    }

    #[test]
    fn small_no_line_is_in_play() {
        assert_eq!(
            small([O, E, O, X, X, E, E, E, E]).status(),
            SquareStatus::InPlay
        );
    }

    #[test]
    fn small_diagonal_x_wins() {
        assert_eq!(
            small([X, O, X, O, X, E, E, E, X]).status(),
            SquareStatus::Won(Player::X)
        );
    }

    #[test]
    fn small_top_row_o_wins() {
        assert_eq!(
            small([O, O, O, O, O, E, X, E, X]).status(),
            SquareStatus::Won(Player::O)
        );
    }

    #[test]
    fn small_right_column_o_wins() {
        assert_eq!(
            small([O, E, O, O, E, O, X, E, O]).status(),
            SquareStatus::Won(Player::O)
        );
    }

    #[test]
    fn small_full_no_line_is_draw() {
        assert_eq!(
            small([X, O, X, X, O, O, O, X, X]).status(),
            SquareStatus::Draw
        );
    }

    // --- Large-square scenarios (ported from BoardTests.LargeSquareTests) ---

    #[test]
    fn large_new_board_is_in_play() {
        assert_eq!(Board::new().status(), SquareStatus::InPlay);
    }

    #[test]
    fn large_top_row_x_wins() {
        let board = Board {
            boards: [
                win_square(Player::X),
                win_square(Player::X),
                win_square(Player::X),
                open_square(),
                open_square(),
                open_square(),
                open_square(),
                open_square(),
                open_square(),
            ],
        };
        assert_eq!(board.status(), SquareStatus::Won(Player::X));
    }

    #[test]
    fn large_right_column_o_wins() {
        let board = Board {
            boards: [
                open_square(),
                open_square(),
                win_square(Player::O),
                open_square(),
                open_square(),
                win_square(Player::O),
                open_square(),
                open_square(),
                win_square(Player::O),
            ],
        };
        assert_eq!(board.status(), SquareStatus::Won(Player::O));
    }

    #[test]
    fn large_line_of_drawn_boards_does_not_win() {
        // A middle row of drawn small boards must NOT win the large board.
        let board = Board {
            boards: [
                open_square(),
                open_square(),
                open_square(),
                draw_square(),
                draw_square(),
                draw_square(),
                open_square(),
                open_square(),
                open_square(),
            ],
        };
        assert_eq!(board.status(), SquareStatus::InPlay);
    }

    #[test]
    fn large_all_drawn_boards_is_draw() {
        let board = Board {
            boards: [draw_square(); 9],
        };
        assert_eq!(board.status(), SquareStatus::Draw);
    }
}
