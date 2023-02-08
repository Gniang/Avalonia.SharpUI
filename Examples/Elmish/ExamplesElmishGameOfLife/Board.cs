using Avalonia.Controls;
using Avalonia.Controls.Documents;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Shapes;
using Avalonia.Media;
using Avalonia.SharpUI;
using Avalonia.SharpUI.Elmish;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static ExamplesElmishGameOfLife.Board;

namespace ExamplesElmishGameOfLife;

public class Board : IView<Msg, State>
{
    public record State(BoardMatrix Board);

    public static State Init() => new(BoardMatrix.ConstructBasic(50, 50));

    public record Msg
    {
        private Msg() { }
        public record Evolve() : Msg;
        public record KillCell(BoardPosition Pos) : Msg;
        public record ReviveCell(BoardPosition Pos) : Msg;
    }

    public static State Update(Msg msg, State s)
        => msg switch
        {
            Msg.Evolve _ => new(BoardMatrix.Evolve(s.Board)),
            Msg.KillCell k => new(BoardMatrix.PlaceDeadCell(s.Board, k.Pos)),
            Msg.ReviveCell r => new(BoardMatrix.PlaceAliveCell(s.Board, r.Pos)),
            _ => throw new Exception($"unexperimental msg.{msg}"),
        };

    public Control View(State state, IViewUpdater<Msg> viewUpdater)
    {
        return new UniformGrid()
        {
            Columns = state.Board.Width,
            Rows = state.Board.Height,
        }
            .Children(
                state.Board.Cells
                    .Flatten()
                    .Select(item =>
                    {
                        var (x, y, cell) = item;
                        return cell switch
                        {
                            Cell.Alive
                                => new Button()
                                {
                                    Background = Brush.Parse("green"),
                                }
                                .OnClick((s, e) => { }),
                            Cell.Dead
                                => new Button()
                                {
                                    Background = Brush.Parse("gray"),
                                }
                                .OnClick((s, e) => { }),
                            _ => throw new Exception($"unexpect. {cell}"),
                        };
                    })
                    .ToArray()
                );
    }
}


public record class BoardMatrix(int Width, int Height, Cell[,] Cells)
{
    public static BoardMatrix ConstructBasic(int width, int height)
    {
        return new BoardMatrix(width, height, new Cell[width, height])
            .SetCell(new(2, 1), Cell.Alive)
            .SetCell(new(3, 2), Cell.Alive)
            .SetCell(new(1, 3), Cell.Alive)
            .SetCell(new(2, 3), Cell.Alive)
            .SetCell(new(3, 3), Cell.Alive);
    }


    private Cell[] GetNeighbors(BoardPosition pos)
    {
        var board = this;
        var neighborPositions =
            new[]{
                pos.Above(),
                pos.Above().LeftOf(),
                pos.Above().RightOf(),
                pos.LeftOf(),
                pos.RightOf(),
                pos.Below(),
                pos.Below().LeftOf(),
                pos.Below().RightOf(),
            };

        return neighborPositions
            .Select(pos => board.TryGetCell(pos))
            .Select(x => x ?? Cell.Dead)
            .ToArray();
    }

    private Cell? TryGetCell(BoardPosition pos)
    {
        var board = this;
        bool xInRange = pos.X >= 0 && pos.X < board.Width;
        bool yInRange = pos.Y >= 0 && pos.Y < board.Height;
        return (xInRange, yInRange) switch
        {
            (true, true) => board.Cells[pos.X, pos.Y],
            _ => null
        };
    }

    private Cell EvolveCell(BoardPosition pos, Cell cell)
    {
        var board = this;
        var neighbors = board.GetNeighbors(pos);
        return cell == Cell.Dead
            ? EvolveCellDead(neighbors)
            : EvolveCellAlive(neighbors);
    }

    private Cell EvolveCellDead(Cell[] neighbors)
    {
        int aliveNeighbors =
            neighbors.Count(x => x == Cell.Alive);

        return aliveNeighbors switch
        {
            3 => Cell.Alive,
            _ => Cell.Dead,
        };
    }
    private Cell EvolveCellAlive(Cell[] neighbors)
    {
        int aliveNeighbors =
            neighbors.Count(x => x == Cell.Alive);

        return aliveNeighbors switch
        {
            0 or 1 => Cell.Dead,
            2 or 3 => Cell.Alive,
            _ => Cell.Dead,
        };
    }
    private BoardMatrix SetCell(BoardPosition pos, Cell cell)
    {
        var cells = this.Cells.Map2D(v => v.val);
        cells[pos.X, pos.Y] = cell;
        return this with { Cells = cells };
    }

    public static BoardMatrix Evolve(BoardMatrix board)
    {
        var cells = board.Cells
            .Map2D(v => board.EvolveCell(new(v.x, v.y), v.val));
        return board with { Cells = cells };
    }

    public static BoardMatrix PlaceAliveCell(BoardMatrix board, BoardPosition pos)
        => board.SetCell(pos, Cell.Alive);

    public static BoardMatrix PlaceDeadCell(BoardMatrix board, BoardPosition pos)
        => board.SetCell(pos, Cell.Dead);
}

public enum Cell
{
    Alive,
    Dead,
}

public record struct BoardPosition(int X, int Y)
{
    public BoardPosition LeftOf()
        => this with { X = this.X - 1 };
    public BoardPosition RightOf()
        => this with { X = this.X + 1 };
    public BoardPosition Below()
        => this with { Y = this.Y - 1 };
    public BoardPosition Above()
        => this with { Y = this.Y + 1 };
}

public static class Array2D
{
    public static IEnumerable<(int x, int y, T val)> Flatten<T>(this T[,] array2d)
    {
        var d1 = array2d.GetUpperBound(0);
        var d2 = array2d.GetUpperBound(1);
        for (int x = 0; x < d1; ++x)
        {
            for (int y = 0; y < d2; ++y)
            {
                yield return (x, y, array2d[x, y]);
            }
        }
    }
    public static R[,] Map2D<T, R>(this T[,] array2d, Func<(int x, int y, T val), R> mapper)
    {
        var d1 = array2d.GetUpperBound(0);
        var d2 = array2d.GetUpperBound(1);
        var ret = new R[d1, d2];
        foreach (var v in Flatten(array2d))
        {
            ret[v.x, v.y] = mapper(v);
        }
        return ret;
    }
}
