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
}