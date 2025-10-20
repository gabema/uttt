namespace utt.game.test;

using Xunit;
using uttt.game;
using System;

public class BoardTests
{
    private static SmallSquare NewDrawSquare() => new(SpotState.X, SpotState.X, SpotState.O,
                                    SpotState.O, SpotState.O, SpotState.X,
                                    SpotState.X, SpotState.O, SpotState.X);

    private static SmallSquare NewWinSquare(SpotState winner) => new(winner, winner, winner,
                                    SpotState.Open, SpotState.Open, SpotState.Open,
                                    SpotState.Open, SpotState.Open, SpotState.Open);
    private static SmallSquare NewOpenSquare() => new(SpotState.Open, SpotState.Open, SpotState.Open,
                                    SpotState.Open, SpotState.Open, SpotState.Open,
                                    SpotState.Open, SpotState.Open, SpotState.Open);

    public static readonly object[] LargeSquareTests =
    [
        new object[]
        {
            LargeSquare.NewBoard(),
            SpotState.Open
        },
        // Expect large board ToSpot to be X since top row small boards are X
        new object[]
        {
            new LargeSquare(
                NewWinSquare(SpotState.X), NewWinSquare(SpotState.X), NewWinSquare(SpotState.X),
                NewOpenSquare(), NewOpenSquare(), NewOpenSquare(),
                NewOpenSquare(), NewOpenSquare(), NewOpenSquare()),
            SpotState.X
        },
        // Expect large board ToSpot to be O since right row in small boards are O
        new object[]
        {
            new LargeSquare(
                NewOpenSquare(), NewOpenSquare(), NewWinSquare(SpotState.O),
                NewOpenSquare(), NewOpenSquare(), NewWinSquare(SpotState.O),
                NewOpenSquare(), NewOpenSquare(), NewWinSquare(SpotState.O)),
            SpotState.O
        },
        // Expect large board to be Open since no three in a row (not counting 3 in a row draws)
        new object[]
        {
            new LargeSquare(
                NewOpenSquare(), NewOpenSquare(), NewOpenSquare(),
                NewDrawSquare(), NewDrawSquare(), NewDrawSquare(),
                NewOpenSquare(), NewOpenSquare(), NewOpenSquare()),
            SpotState.Open
        },
        new object[]
        {
            new LargeSquare(
                NewDrawSquare(), NewDrawSquare(), NewDrawSquare(),
                NewDrawSquare(), NewDrawSquare(), NewDrawSquare(),
                NewDrawSquare(), NewDrawSquare(), NewDrawSquare()),
            SpotState.Draw
        }
    ];

    public static readonly object[] SmallSquareTests =
    [
        new object[]
        {
            new SmallSquare(
                SpotState.Open, SpotState.Open, SpotState.Open,
                SpotState.Open, SpotState.Open, SpotState.Open,
                SpotState.Open, SpotState.Open, SpotState.Open),
            SpotState.Open
        },
        new object[]
        {
            new SmallSquare(
                SpotState.O, SpotState.Open, SpotState.O,
                SpotState.X, SpotState.X, SpotState.Open,
                SpotState.Open, SpotState.Open, SpotState.Open),
            SpotState.Open
        },
        new object[]
        {
            new SmallSquare(
                SpotState.X, SpotState.O, SpotState.X,
                SpotState.O, SpotState.X, SpotState.Open,
                SpotState.Open, SpotState.Open, SpotState.X),
            SpotState.X
        },
        new object[]
        {
            new SmallSquare(
                SpotState.O, SpotState.O, SpotState.O,
                SpotState.O, SpotState.O, SpotState.Open,
                SpotState.X, SpotState.Open, SpotState.X),
            SpotState.O
        },
        new object[]
        {
            new SmallSquare(
                SpotState.O, SpotState.Open, SpotState.O,
                SpotState.O, SpotState.Open, SpotState.O,
                SpotState.X, SpotState.Open, SpotState.O),
            SpotState.O
        },
        new object[]
        {
            new SmallSquare(
                SpotState.X, SpotState.O, SpotState.X,
                SpotState.X, SpotState.O, SpotState.O,
                SpotState.O, SpotState.X, SpotState.X),
            SpotState.Draw
        }
    ];

    [Theory]
    [MemberData(nameof(SmallSquareTests))]
    public void TestSmallSquareScenarios(SmallSquare board, SpotState expected)
    {
        Assert.Equal(expected, board.ToSpot());
    }

    [Theory]
    [MemberData(nameof(LargeSquareTests))]
    public void TestLargeSquareScenarios(LargeSquare board, SpotState expected)
    {
        Assert.Equal(expected, board.ToSpot());
    }

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