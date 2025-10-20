namespace uttt.game;

public enum SpotState {
    Open,
    X,
    O,
    Draw
}

public interface ISquare<T>
{
    public T TopLeft { get; }
    public T TopMiddle { get; }
    public T TopRight { get; }
    public T MiddleLeft { get; }
    public T MiddleMiddle { get; }
    public T MiddleRight { get; }
    public T BottomLeft { get; }
    public T BottomMiddle { get; }
    public T BottomRight { get; }

    public SpotState ToSpot();
}

static class SpotStateUtils
{
    public static SpotState ToSpot(Func<SpotState> topLeft, Func<SpotState> topMiddle, Func<SpotState> topRight,
    Func<SpotState> middleLeft, Func<SpotState> middleMiddle, Func<SpotState> middleRight,
    Func<SpotState> bottomLeft, Func<SpotState> bottomMiddle, Func<SpotState> bottomRight, bool includeDraw)
    {
        var (a, b, c) = (topLeft(), topMiddle(), topRight());
        var (d, e, f) = (middleLeft(), middleMiddle(), middleRight());
        var (g, h, i) = (bottomLeft(), bottomMiddle(), bottomRight());

        var hasOpenSpots =
            a == SpotState.Open ||
            b == SpotState.Open ||
            c == SpotState.Open ||
            d == SpotState.Open ||
            e == SpotState.Open ||
            f == SpotState.Open ||
            g == SpotState.Open ||
            h == SpotState.Open ||
            i == SpotState.Open;

        return UnionEvaluator(a, b, c, includeDraw) ??
            UnionEvaluator(d, e, f, includeDraw) ??
            UnionEvaluator(g, h, i, includeDraw) ??
            UnionEvaluator(a, d, g, includeDraw) ??
            UnionEvaluator(b, e, h, includeDraw) ??
            UnionEvaluator(c, f, i, includeDraw) ??
            UnionEvaluator(a, e, i, includeDraw) ??
            UnionEvaluator(c, e, g, includeDraw) ??
            (hasOpenSpots ? SpotState.Open : SpotState.Draw);
    }

    private static SpotState? UnionEvaluator(SpotState a, SpotState b, SpotState c, bool includeDraw)
    {
        if (a == b && b == c && c != SpotState.Open)
        {
            if (!includeDraw && c == SpotState.Draw)
            {
                return null;
            }
            return a;
        }
        return null;
    }
}

public record struct SmallSquare(SpotState TopLeft, SpotState TopMiddle, SpotState TopRight,
    SpotState MiddleLeft, SpotState MiddleMiddle, SpotState MiddleRight,
    SpotState BottomLeft, SpotState BottomMiddle, SpotState BottomRight) : ISquare<SpotState>
{
    public SpotState ToSpot()
    {
        var local = this;
        return SpotStateUtils.ToSpot(
            () => local.TopLeft,
            () => local.TopMiddle,
            () => local.TopRight,
            () => local.MiddleLeft,
            () => local.MiddleMiddle,
            () => local.MiddleRight,
            () => local.BottomLeft,
            () => local.BottomMiddle,
            () => local.BottomRight,
            includeDraw: false
        );
    }
}

public record struct LargeSquare(
    SmallSquare TopLeft, SmallSquare TopMiddle, SmallSquare TopRight,
    SmallSquare MiddleLeft, SmallSquare MiddleMiddle, SmallSquare MiddleRight,
    SmallSquare BottomLeft, SmallSquare BottomMiddle, SmallSquare BottomRight) : ISquare<SmallSquare>
{
    public static LargeSquare NewBoard() => new(
            new(SpotState.Open, SpotState.Open, SpotState.Open, SpotState.Open, SpotState.Open, SpotState.Open, SpotState.Open, SpotState.Open, SpotState.Open),
            new(SpotState.Open, SpotState.Open, SpotState.Open, SpotState.Open, SpotState.Open, SpotState.Open, SpotState.Open, SpotState.Open, SpotState.Open),
            new(SpotState.Open, SpotState.Open, SpotState.Open, SpotState.Open, SpotState.Open, SpotState.Open, SpotState.Open, SpotState.Open, SpotState.Open),

            new(SpotState.Open, SpotState.Open, SpotState.Open, SpotState.Open, SpotState.Open, SpotState.Open, SpotState.Open, SpotState.Open, SpotState.Open),
            new(SpotState.Open, SpotState.Open, SpotState.Open, SpotState.Open, SpotState.Open, SpotState.Open, SpotState.Open, SpotState.Open, SpotState.Open),
            new(SpotState.Open, SpotState.Open, SpotState.Open, SpotState.Open, SpotState.Open, SpotState.Open, SpotState.Open, SpotState.Open, SpotState.Open),

            new(SpotState.Open, SpotState.Open, SpotState.Open, SpotState.Open, SpotState.Open, SpotState.Open, SpotState.Open, SpotState.Open, SpotState.Open),
            new(SpotState.Open, SpotState.Open, SpotState.Open, SpotState.Open, SpotState.Open, SpotState.Open, SpotState.Open, SpotState.Open, SpotState.Open),
            new(SpotState.Open, SpotState.Open, SpotState.Open, SpotState.Open, SpotState.Open, SpotState.Open, SpotState.Open, SpotState.Open, SpotState.Open)
        );

    public SpotState ToSpot()
    {
        var spot = SpotStateUtils.ToSpot(
            TopLeft.ToSpot,
            TopMiddle.ToSpot,
            TopRight.ToSpot,
            MiddleLeft.ToSpot,
            MiddleMiddle.ToSpot,
            MiddleRight.ToSpot,
            BottomLeft.ToSpot,
            BottomMiddle.ToSpot,
            BottomRight.ToSpot,
            includeDraw: false
        );
        return spot;
    }
}

public enum Player {
    One,
    Two
}

public record Game(LargeSquare Square, Player NextPlayer, int SquareToPlay);