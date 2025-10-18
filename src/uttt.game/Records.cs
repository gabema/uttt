using System.Data;

namespace uttt.game;

public enum SpotState {
    Open,
    X,
    O,
    Draw,
}

public record struct Spot(SpotState S) {
    public SpotState ToSpot() => S;
}

public interface ISquare<T> where T : struct
{
    public T TopLeft {get;}
    public T TopMiddle {get;}
    public T TopRight {get;}
    public T MiddleLeft {get;}
    public T MiddleMiddle{get;}
    public T MiddleRight {get;}
    public T BottomLeft {get;}
    public T BottomMiddle {get;}
    public T BottomRight {get;}

    public SpotState ToSpot();
}



public record struct SmallSquare(SpotState TopLeft, SpotState TopMiddle, SpotState TopRight,
SpotState MiddleLeft, SpotState MiddleMiddle, SpotState MiddleRight,
SpotState BottomLeft, SpotState BottomMiddle, SpotState BottomRight) : ISquare<SpotState>
{
    public SpotState ToSpot() =>
            TopLeft == TopMiddle && TopMiddle == TopRight && TopRight != SpotState.Open ? TopRight
            : MiddleLeft == MiddleMiddle && MiddleMiddle == MiddleRight && MiddleRight != SpotState.Open ? MiddleLeft
            : BottomLeft == BottomMiddle && BottomRight == BottomRight && BottomLeft != SpotState.Open ? BottomLeft
            : TopLeft == MiddleLeft && MiddleLeft == BottomLeft && BottomLeft != SpotState.Open ? BottomLeft
            : TopMiddle == MiddleMiddle && MiddleMiddle == BottomMiddle && BottomMiddle != SpotState.Open ? BottomMiddle
            : TopRight == MiddleRight && MiddleRight == BottomRight && BottomRight != SpotState.Open ? BottomRight
            : TopLeft == MiddleMiddle && MiddleMiddle == BottomRight && BottomRight != SpotState.Open ? BottomRight
            : TopRight == MiddleMiddle && MiddleMiddle == BottomLeft && BottomLeft != SpotState.Open ? BottomLeft
            : TopLeft == SpotState.Open || TopMiddle == SpotState.Open || TopRight == SpotState.Open ||
              MiddleLeft == SpotState.Open || MiddleMiddle == SpotState.Open || MiddleRight == SpotState.Open ||
              BottomLeft == SpotState.Open || BottomMiddle == SpotState.Open || BottomRight == SpotState.Open ? SpotState.Open : SpotState.Draw;
}

public record struct LargeSquare(
    SmallSquare TopLeft, SmallSquare TopMiddle, SmallSquare TopRight,
    SmallSquare MiddleLeft, SmallSquare MiddleMiddle, SmallSquare MiddleRight,
    SmallSquare BottomLeft, SmallSquare BottomMiddle, SmallSquare BottomRight) : ISquare<SmallSquare>
{
    public static LargeSquare NewBoard() => new(
            new(SpotState.Open, SpotState.Open, SpotState.Open, SpotState.Open, SpotState.Open, SpotState.Open, SpotState.Open, SpotState.Open, SpotState.Open),
            new(SpotState.Open, SpotState.Open, SpotState.Open, SpotState.Open, SpotState.Open, SpotState.Open, SpotState.Open, SpotState.Open, SpotState.Open),
            new SmallSquare(SpotState.Open, SpotState.Open, SpotState.Open, SpotState.Open, SpotState.Open, SpotState.Open, SpotState.Open, SpotState.Open, SpotState.Open),

            new(SpotState.Open, SpotState.Open, SpotState.Open, SpotState.Open, SpotState.Open, SpotState.Open, SpotState.Open, SpotState.Open, SpotState.Open),
            new(SpotState.Open, SpotState.Open, SpotState.Open, SpotState.Open, SpotState.Open, SpotState.Open, SpotState.Open, SpotState.Open, SpotState.Open),
            new SmallSquare(SpotState.Open, SpotState.Open, SpotState.Open, SpotState.Open, SpotState.Open, SpotState.Open, SpotState.Open, SpotState.Open, SpotState.Open),

            new(SpotState.Open, SpotState.Open, SpotState.Open, SpotState.Open, SpotState.Open, SpotState.Open, SpotState.Open, SpotState.Open, SpotState.Open),
            new(SpotState.Open, SpotState.Open, SpotState.Open, SpotState.Open, SpotState.Open, SpotState.Open, SpotState.Open, SpotState.Open, SpotState.Open),
            new(SpotState.Open, SpotState.Open, SpotState.Open, SpotState.Open, SpotState.Open, SpotState.Open, SpotState.Open, SpotState.Open, SpotState.Open)
        );

    public SpotState ToSpot()
    {
        // Determine the winner of the large square based on each small square's ToSpot()
        var a = TopLeft.ToSpot();
        var b = TopMiddle.ToSpot();
        var c = TopRight.ToSpot();
        var d = MiddleLeft.ToSpot();
        var e = MiddleMiddle.ToSpot();
        var f = MiddleRight.ToSpot();
        var g = BottomLeft.ToSpot();
        var h = BottomMiddle.ToSpot();
        var i = BottomRight.ToSpot();

        // Check rows
        if (a == b && b == c && a != SpotState.Open) return a;
        if (d == e && e == f && d != SpotState.Open) return d;
        if (g == h && h == i && g != SpotState.Open) return g;

        // Check columns
        if (a == d && d == g && a != SpotState.Open) return a;
        if (b == e && e == h && b != SpotState.Open) return b;
        if (c == f && f == i && c != SpotState.Open) return c;

        // Check diagonals
        if (a == e && e == i && a != SpotState.Open) return a;
        if (c == e && e == g && c != SpotState.Open) return c;

        // If any small square is still Open, large board is still open
        if (a == SpotState.Open || b == SpotState.Open || c == SpotState.Open || d == SpotState.Open || e == SpotState.Open || f == SpotState.Open || g == SpotState.Open || h == SpotState.Open || i == SpotState.Open)
            return SpotState.Open;

        // Otherwise it's closed (draw)
        return SpotState.Draw;
    }
}

public enum Player {
    One,
    Two
}

public record Game(LargeSquare Square, Player NextPlayer, int SquareToPlay);