using Avalonia.Controls;
using Avalonia.Media;
using static Avalonia.SharpUI.ObservableState;
using Avalonia.SharpUI;
using Avalonia.Layout;
using Avalonia.Controls.Documents;
using Avalonia.Media.Immutable;

namespace ExamplesInlineText;

internal class MainView : UserControl
{
    private IBrush redBrush = new ImmutableSolidColorBrush(Color.FromUInt32(0x0ffB2474Du));
    private IBrush blueBrush = new ImmutableSolidColorBrush(Color.FromUInt32(0x0ff47B2A6u));
    public MainView()
    {
        this.FontFamily = "Calibri";
        ObservableState<int> colorMode = UseState(0);
        this.Content = new StackPanel
        {
            VerticalAlignment = VerticalAlignment.Center,
            HorizontalAlignment = HorizontalAlignment.Center
        }
            .Children(new Control[]
            {
                new Button()
                {
                    Content = "Invert color!",
                }
                .OnClick((s, e) => {
                    if( colorMode.Value == 0 )
                    {
                        colorMode.Value = 1;
                    }
                    else
                    {
                        colorMode.Value = 0;
                    }
                 })
                ,
                new SelectableTextBlock()
                {
                    FontSize = 48.0,
                    HorizontalAlignment = HorizontalAlignment.Center
                }
                .DockTop()
                .Inlines(new Inline[] {
                    new Run("You"),
                    new Run()
                    {
                        Text = "Inline"
                    }
                    .SetBind(Run.BackgroundProperty, colorMode, converter: ValueConverterSimple.OneWay(
                        (v) =>colorMode.Value switch
                                {
                                    0 => redBrush,
                                    _ => blueBrush,
                                })
                    ),
                    new LineBreak(),
                    new Span()
                        .Inlines(new Inline[]
                        {
                            new Bold().Text("Oh, so bold!"),
                            new LineBreak(),
                            new Italic().Text("Although, "),
                            new Run("I always wanted to be "),
                            new Underline(){ Background = blueBrush }.Text("underlined"),
                            new LineBreak(),
                            new Bold().Inlines(new Run("text "), new Italic().Text("additional.")),
                        }),
                })
                ,
            }
        );
    }

}