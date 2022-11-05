using System.Data;

public static class DataTableExtensions
{

    public static DataTable ToDataTable(this double[,] items)
    {
        var rowSize = items.GetLength(0);
        var colSize = items.GetLength(1);
        DataTable dt = new DataTable();
        dt.BeginLoadData();
        for (int i = 0; i < colSize; ++i)
        {
            dt.Columns.Add("Column" + (i + 1), typeof(double));
        }

        for (var i = 0; i < rowSize; ++i)
        {
            var ds = new object[colSize];
            for (var j = 0; j < colSize; ++j)
            {
                ds[j] = items[i, j];
            }
            dt.Rows.Add(ds);
        }
        dt.EndLoadData();
        return dt;
    }
}

