using Avalonia.Controls;
using System.Collections.Generic;
using System.Linq;

namespace Avalonia.SharpUI;

public static class GridExtensions
{


    public static T Columns<T>(this T grid, IEnumerable<DataGridColumn> columns) where T : DataGrid
    {
        foreach (var col in columns)
        {
            grid.Columns.Add(col);
        }
        return grid;
    }
    public static T Columns<T>(this T grid, params DataGridColumn[] columns) where T : DataGrid
    {
        return Columns(grid, columns.AsEnumerable());
    }


    /// <summary>set grid row/column index/span.</summary>
    /// <param name="control">controls that to aligment to grid.</param>
    /// <param name="rowIndex">row index of grid.</param>
    /// <param name="columnIndex">column index of grid.</param>
    /// <param name="rowSpan">row span of grid.</param>
    /// <param name="columnSpan">column span of grid.</param>
    /// <returns></returns>
    public static T SetGrid<T>(this T control,
                                int? rowIndex = null,
                                int? columnIndex = null,
                                int? rowSpan = null,
                                int? columnSpan = null)
    where T : Control
    {
        if (rowIndex != null)
            Grid.SetRow(control, rowIndex.Value);

        if (columnIndex != null)
            Grid.SetColumn(control, columnIndex.Value);

        if (rowSpan != null)
            Grid.SetRowSpan(control, rowSpan.Value);

        if (columnSpan != null)
            Grid.SetColumnSpan(control, columnSpan.Value);

        return control;
    }


    /// <summary>
    /// set column definitions to grid.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="grid">grid</param>
    /// <param name="s">column definitions. eg. "1*,30,Auto".</param>
    /// <returns></returns>
    public static T ColumnDefinitions<T>(this T grid, string s)
    where T : Grid
    {
        grid.ColumnDefinitions = new ColumnDefinitions(s);
        return grid;
    }

    /// <summary>
    /// set row definitions to grid.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="grid">grid.</param>
    /// <param name="s">row definitions. eg. "1*,30,Auto".</param>
    /// <returns></returns>
    public static T RowDefinitions<T>(this T grid, string s)
    where T : Grid
    {
        grid.RowDefinitions = new RowDefinitions(s);
        return grid;
    }

}
