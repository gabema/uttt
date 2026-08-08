## ADDED Requirements

### Requirement: Board representation

The engine SHALL model the game as a large 3×3 board of nine small 3×3 boards,
using flat arrays indexed `0..8` (not recursive named positional fields). Each
cell SHALL hold one of: open, X, O. The engine SHALL provide a way to construct
a new, fully-open board.

#### Scenario: New board is empty

- **WHEN** a new board is created
- **THEN** all 81 cells are open and the game's overall status is open

#### Scenario: Cells are addressed by flat index

- **WHEN** the engine is asked for the state of a cell by an index in `0..80`
- **THEN** it returns that cell's state without the caller performing any
  positional field translation

### Requirement: Win and draw detection

The engine SHALL determine the winner, draw, or open status of any 3×3 square
(small or large) by evaluating all eight lines (three rows, three columns, two
diagonals). A square with three matching non-open marks in any line SHALL
resolve to that mark's winner. A fully-occupied square with no winning line
SHALL resolve to draw. A square with at least one open cell and no winning line
SHALL resolve to open.

#### Scenario: Small board win by a line

- **WHEN** a small board has three X marks completing any row, column, or diagonal
- **THEN** that small board's status is X

#### Scenario: Full small board with no line is a draw

- **WHEN** a small board is fully occupied with no three-in-a-row
- **THEN** that small board's status is draw

#### Scenario: Large board win from small-board winners

- **WHEN** three small boards won by O complete a line on the large board
- **THEN** the large board's status is O

#### Scenario: Drawn small boards do not win the large board

- **WHEN** three drawn small boards occupy a line on the large board
- **THEN** that line does not produce a large-board win (draws are excluded when
  evaluating lines)

#### Scenario: Fully-resolved large board with no line is a draw

- **WHEN** every small board is resolved (won or drawn) with no winning line of
  matching winners on the large board
- **THEN** the large board's status is draw

### Requirement: Move rules and turn state

The engine SHALL own the interactive rules. It SHALL track whose turn it is and
which small board the current player is forced to play in (or that any board is
allowed). Applying a move SHALL: reject the move if the target cell is not open,
if the game is over, or if it violates the forced-board constraint; otherwise
place the current player's mark, set the next forced board to the small-board
index corresponding to the played cell, reset the constraint to "play anywhere"
when that target board is already won or full, and pass the turn to the other
player.

#### Scenario: Legal move places a mark and passes the turn

- **WHEN** the current player plays an open cell that satisfies the forced-board
  constraint
- **THEN** the cell holds the current player's mark and it becomes the other
  player's turn

#### Scenario: Move dictates the opponent's forced board

- **WHEN** a player plays the cell at small-board-local index `k`
- **THEN** the opponent is forced to play in small board `k`, unless board `k` is
  already won or full, in which case the opponent may play anywhere

#### Scenario: Illegal move is rejected

- **WHEN** a move targets a non-open cell, violates the forced-board constraint,
  or is attempted after the game is over
- **THEN** the engine rejects it and the game state is unchanged

### Requirement: Rendering projection

The engine SHALL expose a projection containing every fact needed to render the
game, so that a frontend computes no game logic of its own. The projection SHALL
include: each cell's mark, each small board's resolved status, which small
boards are currently playable, the overall game status, and whose turn it is.
Applying a move SHALL report which small boards became won as a result of that
move.

#### Scenario: Projection exposes derived display facts

- **WHEN** a frontend requests the projection
- **THEN** it receives per-cell marks, per-small-board status, the playable-board
  set, the overall status, and the current player — with no further computation
  required

#### Scenario: Move outcome reports captured boards

- **WHEN** a move causes one or more small boards to become won
- **THEN** the move outcome identifies exactly those newly-won small boards
