using System.Data;

namespace uttt.game;

public enum SpotState {
    Open,
    X,
    O,
    Closed,
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
              BottomLeft == SpotState.Open || BottomMiddle == SpotState.Open || BottomRight == SpotState.Open ? SpotState.Open : SpotState.Closed;
}

public record struct LargeSquare(SmallSquare TopLeft, SmallSquare TopMiddle, SmallSquare TopRight,
SmallSquare MiddleLeft, SmallSquare MiddleMiddle, SmallSquare MiddleRight,
SmallSquare BottomLeft, SmallSquare BottomMiddle, SmallSquare BottomRight) : ISquare<SmallSquare>
{
    public static LargeSquare NewBoard() => new LargeSquare(
            new SmallSquare(SpotState.Open, SpotState.Open, SpotState.Open, SpotState.Open, SpotState.Open, SpotState.Open, SpotState.Open, SpotState.Open, SpotState.Open),
            new SmallSquare(SpotState.Open, SpotState.Open, SpotState.Open, SpotState.Open, SpotState.Open, SpotState.Open, SpotState.Open, SpotState.Open, SpotState.Open),
            new SmallSquare(SpotState.Open, SpotState.Open, SpotState.Open, SpotState.Open, SpotState.Open, SpotState.Open, SpotState.Open, SpotState.Open, SpotState.Open),

            new SmallSquare(SpotState.Open, SpotState.Open, SpotState.Open, SpotState.Open, SpotState.Open, SpotState.Open, SpotState.Open, SpotState.Open, SpotState.Open),
            new SmallSquare(SpotState.Open, SpotState.Open, SpotState.Open, SpotState.Open, SpotState.Open, SpotState.Open, SpotState.Open, SpotState.Open, SpotState.Open),
            new SmallSquare(SpotState.Open, SpotState.Open, SpotState.Open, SpotState.Open, SpotState.Open, SpotState.Open, SpotState.Open, SpotState.Open, SpotState.Open),

            new SmallSquare(SpotState.Open, SpotState.Open, SpotState.Open, SpotState.Open, SpotState.Open, SpotState.Open, SpotState.Open, SpotState.Open, SpotState.Open),
            new SmallSquare(SpotState.Open, SpotState.Open, SpotState.Open, SpotState.Open, SpotState.Open, SpotState.Open, SpotState.Open, SpotState.Open, SpotState.Open),
            new SmallSquare(SpotState.Open, SpotState.Open, SpotState.Open, SpotState.Open, SpotState.Open, SpotState.Open, SpotState.Open, SpotState.Open, SpotState.Open)
        );

    public SpotState ToSpot() => SpotState.Open;
}

public enum Player {
    One,
    Two
}

public record Game(LargeSquare Square, Player NextPlayer, int SquareToPlay);