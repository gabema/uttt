using Xunit;
using uttt.game;
using System;

namespace utt.game.test;

public class BoardTests
{
    [Fact]
    public void CanCreateBoard()
    {
        Assert.Equal(LargeSquare.NewBoard(), LargeSquare.NewBoard());
    }

    [Fact]
    public void SetPiece()
    {
        var board = LargeSquare.NewBoard();
        var updatedBoard = board with {MiddleMiddle = board.MiddleMiddle with {MiddleMiddle = SpotState.X}};
        Console.WriteLine(updatedBoard);
        Assert.NotEqual(board, updatedBoard);
    }

    [Fact]
    public void SmallSquare_ToSpot_ReturnsOpenWhenEmpty()
    {
        var s = new SmallSquare(SpotState.Open, SpotState.Open, SpotState.Open, SpotState.Open, SpotState.Open, SpotState.Open, SpotState.Open, SpotState.Open, SpotState.Open);
        Assert.Equal(SpotState.Open, s.ToSpot());
    }

    [Fact]
    public void LargeSquare_Detects_Win_When_ThreeSmallBoardsInRow()
    {
        // create three small squares that are X winners
        var xWon = new SmallSquare(SpotState.X, SpotState.X, SpotState.X, SpotState.Open, SpotState.Open, SpotState.Open, SpotState.Open, SpotState.Open, SpotState.Open);
    var open = new SmallSquare(SpotState.Open, SpotState.Open, SpotState.Open, SpotState.Open, SpotState.Open, SpotState.Open, SpotState.Open, SpotState.Open, SpotState.Open);
    var board = new LargeSquare(xWon, xWon, xWon,
                   open, open, open,
                   open, open, open);
        // Expect large board ToSpot to be X since top row small boards are X
        Assert.Equal(SpotState.X, board.ToSpot());
    }
}