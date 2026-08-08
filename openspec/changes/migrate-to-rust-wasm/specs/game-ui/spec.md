## ADDED Requirements

### Requirement: Board rendering from the engine projection

The frontend SHALL render the full board — 81 cells, per-small-board winner
backfaces, the current player, and game-over state — entirely from the engine's
projection. The frontend SHALL NOT compute win/draw status, legal moves, or
derived display facts itself, and SHALL NOT store the authoritative game state.

#### Scenario: Cells reflect engine marks

- **WHEN** the projection reports a cell as X or O
- **THEN** the rendered cell shows that symbol; open cells render blank

#### Scenario: Overall winner is shown

- **WHEN** the projection's overall status is a winner or draw
- **THEN** the UI displays a game-over indication naming the result

### Requirement: Move input forwarding

The frontend SHALL forward a player's cell click to the engine as a move intent
and re-render from the resulting projection. It SHALL NOT enforce the move rules
itself; rejected moves SHALL simply leave the display unchanged.

#### Scenario: Click applies a move

- **WHEN** the player clicks an open, legal cell
- **THEN** the frontend calls the engine's move operation and re-renders from the
  new projection

#### Scenario: Rejected click is a no-op

- **WHEN** the player clicks a cell the engine rejects
- **THEN** the displayed state does not change

### Requirement: Playable-board highlighting

The frontend SHALL highlight the small board(s) the current player may play in,
using the playable set from the projection.

#### Scenario: Forced board is highlighted

- **WHEN** the projection restricts play to a single small board
- **THEN** only that board is highlighted

#### Scenario: Free choice highlights all open boards

- **WHEN** the projection permits play in any board
- **THEN** every not-yet-resolved small board is highlighted

### Requirement: Capture animation

The frontend SHALL play a presentation-only capture animation (a pulse followed
by a flip revealing the winner backface) for each small board reported as newly
won by a move outcome. The permanent "show winner backface" state SHALL be
derived from the engine's per-small-board status; only the transient in-flight
animation SHALL be frontend-local state.

#### Scenario: Newly-won board animates then shows its winner

- **WHEN** a move outcome reports a small board as newly won
- **THEN** that board plays the pulse-then-flip animation and afterward
  persistently shows its winner backface

#### Scenario: Winner backface survives without animation state

- **WHEN** the board is re-rendered with no animation in progress
- **THEN** every won small board still shows its winner backface, derived from
  the engine status rather than from stored animation flags
