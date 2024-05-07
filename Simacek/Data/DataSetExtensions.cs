using System.Data;

namespace Simacek.Data
{
    public static partial class DataSetExtensions
    {
        public static string ToHtmlTable(this DataSet source)
        {
            var htmlString = "";

            foreach (DataTable table in source.Tables)
            {
                htmlString += "<table class='table table-hover table-striped table-condensed'> <thead> <tr>";

                var cols = table.Columns;
                for (int i = 0; i < cols.Count; i++)
                {
                    htmlString += ("<th>" + cols[i].ColumnName + "</th>");
                }
                htmlString += "</tr> </thead> <tbody>";

                var rows = table.Rows;
                for (int i = 0; i < rows.Count; i++)
                {
                    htmlString += "<tr>";
                    for (int j = 0; j < cols.Count; j++)
                    {
                        htmlString += ("<td>" + table.Rows[i][j].ToString() + "</td>");
                    }
                    htmlString += "</tr>";
                }
                htmlString += "</tbody> </table>";
            }

            return htmlString;
        }
    }
}
