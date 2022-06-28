using System.Data;
using ReliaCoat.Common;

var exposureTimes = new double[] { 25, 50, 100 };
var gains = new double[] { 1, 5, 10 };
var injection = new string[] { "under", "optimized", "over" };

var permute1 = exposureTimes.SelectMany(x => gains, (ex, g) => new { ex, g });
var permute2 = permute1.SelectMany(x => injection, (p1, i) => new { p1.ex, p1.g, i });

var parameterList = permute2.shuffle().ToArray();

var table = new DataTable("Design of Experiments");

table.Columns.Add("Exposure Times", typeof(double));
table.Columns.Add("Exposure Gains", typeof(double));
table.Columns.Add("Injection", typeof(string));

foreach (var t in parameterList)
{
    var row = table.NewRow();

    row[0] = t.ex;
    row[1] = t.g;
    row[2] = t.i;

    table.Rows.Add(row);
}

foreach (var item in table.Rows)
{

}