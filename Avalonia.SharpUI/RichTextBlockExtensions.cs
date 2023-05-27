using Avalonia.Controls;
using Avalonia.Controls.Documents;
using System;
using System.Collections.Generic;
using System.Text;

namespace Avalonia.SharpUI;

public static class RichTextBlockExtensions
{
    public static SelectableTextBlock Inlines(this SelectableTextBlock block, InlineCollection inlines)
    {
        block.Inlines = inlines;
        return block;
    }
    public static SelectableTextBlock Inlines(this SelectableTextBlock block, params Inline[] inlines)
    {
        var i = new InlineCollection();
        i.AddRange(inlines);
        return Inlines(block, i);
    }

    public static TSpan Inlines<TSpan>(this TSpan span, params Inline[] inlines)
    where TSpan : Span
    {
        span.Inlines.AddRange(inlines);
        return span;
    }

    public static TSpan Text<TSpan>(this TSpan span, string text)
    where TSpan : Span
    {
        span.Inlines.Add(new Run(text));
        return span;
    }
}
public static class RunExtensions
{
    public static TRun Text<TRun>(this TRun run, string text)
    where TRun : Run
    {
        run.Text = text;
        return run;
    }
}
