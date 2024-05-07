using System.Data;
using System.Text;

namespace Simacek.Data
{
    public static partial class DataTableExtensions
    {
        public static byte[] ToCsvBytes(this DataTable source)
        {
            var sb = new StringBuilder();

            var tab = "";
            foreach (DataColumn col in source.Columns)
            {
                sb.Append(tab + col.ColumnName.Replace(",", " "));
                tab = ",";
            }
            sb.Append("\n");

            var count = source.Columns.Count;
            foreach (DataRow row in source.Rows)
            {
                tab = "";
                for (int i = 0; i < count; i++)
                {
                    sb.Append(tab + row[i].ToString().Replace(",", " "));
                    tab = ",";
                }
                sb.Append("\n");
            }

            return Encoding.Default.GetBytes(sb.ToString());
        }
    }
}
