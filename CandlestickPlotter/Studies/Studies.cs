using CandleStickPlotter.DataTypes;

namespace CandleStickPlotter.Studies
{
    public abstract class Plot
    {
        public string[] Names { get; set; }
        public int[][] Indices { get; set; }
        public Color[] Colors { get; set; }
        public Plot()
        {
            Names = Array.Empty<string>();
            Indices = Array.Empty<int[]>();
            Colors = Array.Empty<Color>();
        }
        public Plot(string[] names, int[][] indices, Color[] colors)
        {
            Names = names;
            Indices = indices;
            Colors = colors;
        }
    }
    public class PricePlot : Plot
    {
        public enum PlotType : byte { Line, Histogram, Dot, Square, Dash, ArrowUp, ArrowDown }
    }
    public class StudyPlot : Plot
    {
        public enum PlotType : byte { Line, Histogram, Dot, Square, Dash, ArrowUp, ArrowDown }
    }
    public abstract class Study
    {
        public Plot[] Plots { get; set; } = Array.Empty<Plot>();
        public virtual void Calculate(Column<double> column) { }
        public virtual void Calculate(Table<double> table) { }
        public abstract string NameString();
    }
}
