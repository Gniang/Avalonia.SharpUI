using Avalonia.Controls;
using Avalonia.Media;
using static System.Net.Mime.MediaTypeNames;
using static Avalonia.SharpUI.ObservableState;
using Avalonia.SharpUI;
using Avalonia.Layout;
using System.ComponentModel;
using System.Reactive.Linq;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using System;
using Avalonia.Controls.Documents;
using Avalonia.Data.Converters;
using System.Globalization;
using Avalonia.Media.Immutable;

namespace ExamplesCounterApp
{
    internal class MainView : UserControl
    {
        private IBrush redBrush = new ImmutableSolidColorBrush(Color.FromUInt32(0x0ffB2474Du));
        private IBrush blueBrush = new ImmutableSolidColorBrush(Color.FromUInt32(0x0ff47B2A6u));
        public MainView()
        {
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
                    new RichTextBlock()
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
                                new Bold().Inlines(new Run("Oh, so bold!") ),
                                new LineBreak(),
                                new Italic().Inlines(new Run("Although, ") ),
                                new Run("I always wanted to be "),
                                new Underline().Inlines(new Run("underlined") ),
                            }),
                    })
                    ,
                }
            );
        }

    }


}