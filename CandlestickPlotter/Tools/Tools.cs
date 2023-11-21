using CandleStickPlotter.DataTypes;
using CandleStickPlotter.Strategies;
using CandleStickPlotter.Studies.Momentum;
using CandleStickPlotter.Studies.MovingAverages;
using CandleStickPlotter.Studies.Volatility;
using CandleStickPlotter.Studies.Tools;
using System.Globalization;

namespace CandleStickPlotter.Tools
{
    public enum MarketDataType : byte { Close, High, Low, Open, HL2, HLC3, OHLC4, Volume }
    public static class Tools
    {
        public static Column<double> GetMarketData(Table<double> ohlcvData, MarketDataType dataType)
        {
            Column<double> data = dataType switch
            {
                MarketDataType.Close => ohlcvData["Close"],
                MarketDataType.High => ohlcvData["High"],
                MarketDataType.Low => ohlcvData["Low"],
                MarketDataType.Open => ohlcvData["Open"],
                MarketDataType.HL2 => (ohlcvData["Low"] + ohlcvData["High"]) / 2,
                MarketDataType.HLC3 => (ohlcvData["Low"] + ohlcvData["High"] + ohlcvData["Close"]) / 3,
                MarketDataType.OHLC4 => (ohlcvData["Open"] + ohlcvData["High"] + ohlcvData["Low"] + ohlcvData["Close"]) / 4,
                MarketDataType.Volume => ohlcvData["Volume"],
                _ => ohlcvData["Close"]
            };
            return data;
        }
        public static Type GetSudyType(StudyType studyType)
        {
            return studyType switch
            {
                StudyType.AverageTrueRange => typeof(AverageTrueRange),
                StudyType.BollingerBands => typeof(BollingerBands),
                StudyType.DonchianChannel => typeof(DonchianChannels),
                StudyType.ExponentialMovingAverage => typeof(ExponentialMovingAverage),
                StudyType.KeltnerChannels => typeof(KeltnerChannels),
                StudyType.LinearRegression => typeof(LeastSquaresLinearRegression),
                StudyType.RelativeStrengthIndex => typeof(RelativeStrenghtIndex),
                StudyType.SimpleMovingAverage => typeof(SimpleMovingAverage),
                StudyType.StandardDeviation => typeof(StandardDeviation),
                StudyType.TrueRange => typeof(TrueRange),
                StudyType.TTMSqueeze => typeof(TTMSqueeze),
                StudyType.WeigthedMovingAverage => typeof(WeightedMovingAverage),
                StudyType.WildersMovingAverage => typeof(WildersMovingAverage),
                _ => typeof(object),
            };
        }

        public static (DateTime[], Table<double>, long[]) GetOHLCVTFromCSV(string filename)
        {
            using StreamReader reader = new(filename);
            string[] lines = reader.ReadToEnd().Split('\n');
            string[] vals = lines[0].Split(',');
            DateTime[] dates = new DateTime[lines.Length - 1];
            Column<double>[] ohlcv = new Column<double>[vals.Length - 1];
            long[] volume = new long[lines.Length - 1];
            int i = 0;
            for (; i < ohlcv.Length; i++)
                ohlcv[i] = new Column<double>(vals[i + 1], lines.Length - 1);
            i = 0;
            int stop = lines.Length - 1;
            for (; i < stop; i++)
            {
                vals = lines[i + 1].Split(',');
                dates[i] = DateTime.ParseExact(vals[0], "yyyy-MM-dd", CultureInfo.InvariantCulture);
                for (int j = 1; j < vals.Length; j++)
                    ohlcv[j - 1][i] = Convert.ToDouble(vals[j]);
                volume[i] = Convert.ToInt64(vals[^1]);
            }
            return (dates, new Table<double>(ohlcv), volume);
        }
    }
}
