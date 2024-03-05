using CandleStickPlotter.DataTypes;
using CandleStickPlotter.Tools;

namespace CandleStickPlotter.Studies
{

    public class Plot : ICloneable
    {
        public enum PlotType : byte { Line, Histogram, Dot, Square, Dash, ArrowUp, ArrowDown }
        public string Name { get; set; } = string.Empty;
        public SubPlot[] SubPlots { get; set; } = [];
        public Column<double> Values { get; set; } = new Column<double>();
        public bool IsBoolean { get; set; } = false;
        public bool Visible { get; set; } = true;
        public PlotType Type { get; set; }
        public double MinValue
        {
            get { return Values.MinValue; }
        }
        public double MaxValue
        {
            get { return Values.MinValue; }
        }
        public Plot() { }
        public Plot(SubPlot[] subPlots)
        {
            SubPlots = subPlots;
        }
        public object Clone() => MemberwiseClone();
    }
    public struct SubPlot
    {
        public SubPlot(bool plotEveryIndex)
        {
            PlotEveryIndex = plotEveryIndex;
        }
        public SubPlot(bool plotEveryIndex, Color color)
        {
            PlotEveryIndex = plotEveryIndex;
            Color = color;
        }
        public SubPlot(string name, Color color)
        {
            Name = name;
            Color = color;
        }
        public SubPlot(string name, bool plotEveryIndex, Color color)
        {
            Name = name;
            PlotEveryIndex = plotEveryIndex;
            Color = color;
        }
        public SubPlot(string name, bool plotEveryIndex, int[] indices, Color color)
        {
            Name = name;
            PlotEveryIndex = plotEveryIndex;
            Indices = indices;
            Color = color;
        }
        public string Name { get; set; } = string.Empty;
        public bool PlotEveryIndex { set; get; } = false;
        public int[] Indices { get; set; } = [];
        public Color Color { get; set; } = Color.Cyan;
    }
    public abstract class Study : ICloneable
    {
        public enum PlotLocation : byte { Price, Lower }
        public int Displace { get; set; }
        public string Name => GetType().Name;
        public Plot[] Plots { get; set; } = [];
        public PlotLocation DefaultPlotLocation { get; set; }
        public virtual void Calculate(Column<double> column) { }
        public virtual void Calculate(Table<double> table) { }
        public virtual void Calculate(Table<double> table, MarketDataType marketDataType) { }
        public virtual string NameString() => "<empty>";
        public object Clone() => MemberwiseClone();
    }
}
