using Avalonia.Controls;
using static Avalonia.SharpUI.ObservableState;
using Avalonia.SharpUI;
using Avalonia.Layout;
using Microsoft.VisualBasic;
using System.Text.RegularExpressions;
using System;
using static ExamplesElmishGameOfLife.MainView;
using Avalonia.SharpUI.Elmish;
using Avalonia.Media;
using Avalonia.Controls.Shapes;
using Avalonia.Threading;
using static ExamplesElmishGameOfLife.MainView.Msg;

namespace ExamplesElmishGameOfLife;

internal class MainView : IView<Msg, State>
{
    public record State(BoardMatrix Board, bool EvolutionRunning);

    public static State Init()
        => new(BoardMatrix.ConstructBasic(50, 50), false);


    public abstract record Msg
    {
        private Msg() { }
        public sealed record BoardMsg(Board.Msg Msg) : Msg;
        public sealed record StartEvolution : Msg;
        public sealed record StopEvolution : Msg;
    }

    public static State Update(Msg msg, State s)
    {
        State boardUpdate(Board.Msg m)
            => m switch
            {
                Board.Msg.Evolve _ when s.EvolutionRunning
                    => s with { Board = Board.Update(m, new(s.Board)).Board },
                Board.Msg.Evolve _ => s,
                _ => s with { Board = Board.Update(m, new(s.Board)).Board }
            };

        return msg switch
        {
            Msg.StartEvolution _ => s with { EvolutionRunning = true },
            Msg.StopEvolution _ => s with { EvolutionRunning = false },
            Msg.BoardMsg t => boardUpdate(t.Msg),
            _ => throw new Exception($"unexperimental msg.{msg}"),
        };
    }

    public Control View(State state, IViewUpdater<Msg> v)
    {
        return new DockPanel()
            .Children(new Control[]
            {
                new Button()
                {
                    HorizontalAlignment= HorizontalAlignment.Stretch,
                    IsVisible = !state.EvolutionRunning,
                    Background = Brush.Parse("#16a085"),
                    Content = "start",
                }
                .DockBottom()
                .OnClick((s, e) => v.Invoke(new StartEvolution()) )
                ,

                new Button()
                {
                    HorizontalAlignment= HorizontalAlignment.Stretch,
                    IsVisible = state.EvolutionRunning,
                    Background = Brush.Parse("#d35400"),
                    Content = "stop",
                }
                .DockBottom()
                .OnClick((s, e) => v.Invoke(new StopEvolution()) )
                ,
                 new Board().Create<Board, Board.State, Board.Msg>(Board.Update, new Board.State(state.Board)),
            })
            ;
    }
}
