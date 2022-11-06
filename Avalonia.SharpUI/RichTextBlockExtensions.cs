using Avalonia.Controls;
using Avalonia.Controls.Documents;
using System;
using System.Collections.Generic;
using System.Text;

namespace Avalonia.SharpUI;

public static class RichTextBlockExtensions
{
    public static RichTextBlock Inlines(this RichTextBlock block, InlineCollection inlines)
    {
        block.Inlines = inlines;
        return block;
    }
    public static RichTextBlock Inlines(this RichTextBlock block, params Inline[] inlines)
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
}
