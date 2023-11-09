
using CandleStickPlotter.DataTypes;
using CandleStickPlotter.Studies.Momentum;
using CandleStickPlotter.Studies.MovingAverages;
using CandleStickPlotter.Studies.Volatility;
using CandleStickPlotter.Utils;

namespace CandleStickPlotter.Strategies
{
    public enum StudyType
    {
        AverageTrueRange,
        BollingerBands,
        DonchianChannel,
        ExponentialMovingAverage,
        KeltnerChannels,
        LinearRegression,
        RelativeStrengthIndex,
        SimpleMovingAverage,
        StandardDeviation,
        TrueRange,
        TTMSqueeze,
        WeigthedMovingAverage,
        WildersMovingAverage
    }
    public enum BandTypes : byte { Lower, Middle, Upper }
    public enum LinearRegressionValueTypes : byte { Intercept, Predicted, R2, Slope }
    public enum PositionType : byte { Long, Short }
    public enum TTMSqueezeValueTypes : byte { Histogram, Signal}
    readonly struct OpeningCondition
    {
        public readonly string PostfixExpression { get; }
        public readonly double? StopLoss { get; }
        public readonly double? StopGain { get; }
        public readonly PositionType PositionType { get; }
        public OpeningCondition(string postfixExpression, PositionType positionType, double? stopLoss, double? stopGain)
        {
            PostfixExpression = postfixExpression;
            PositionType = positionType;
            StopLoss = stopLoss;
            StopGain = stopGain;
        }
    }
    readonly struct ClosingCondition
    {
        public readonly string PostfixExpression { get; }
        public ClosingCondition(string postfixExpression)
        {
            PostfixExpression = postfixExpression;
        }
    }

    public class StrategyResult
    {
        public StrategyResult(int id, int[] openingIndices, int[] closingIndices, char[] closingReasons, double[] gainLoss)
        {
            Id = id;
            OpeningIndices = openingIndices;
            ClosingIndices = closingIndices;
            ClosingReasons = closingReasons;
            GainLoss = gainLoss;
        }
        public int Id { get; private set; }
        public int[] OpeningIndices { get; set; }
        public int[] ClosingIndices { get; set; }
        public char[] ClosingReasons { get; set; }
        public double[] GainLoss { get; set; }
    }
    class Strategy
    {
        private OpeningCondition OpeningCondition { get; }
        private ClosingCondition ClosingCondition { get; }
        public StrategyResult Result { get; private set; }
        public Strategy(OpeningCondition openingCondition, ClosingCondition closingCondition, Table<double> ohlcvData, int id = 0)
        {
            OpeningCondition = openingCondition;
            ClosingCondition = closingCondition;
            Result = new StrategyResult(id, EvaluateExpression(OpeningCondition.PostfixExpression, ohlcvData),
            EvaluateExpression(ClosingCondition.PostfixExpression, ohlcvData), Array.Empty<char>(), Array.Empty<double>());
            Evaluate(ohlcvData);
        }
        private void Evaluate(Table<double> ohlcvData)
        {
            int openingConditionIndex;
            int closingConditionIndex;
            int previousClosingIndex = -1;
            double openingPrice, stopLossPrice, stopGainPrice, closingPrice;
            List<int> trueOpeningIndices = new();
            List<int> trueClosingIndices = new();
            List<char> closingReason = new();
            List<double> gainLoss = new();
            char reason;
            int k;
            bool stopReached;
            for (int i = 0, j = 0; i < Result.OpeningIndices.Length || j < Result.ClosingIndices.Length;)
            {
                openingConditionIndex = Result.OpeningIndices[i];
                closingConditionIndex = Result.ClosingIndices[j];
                if (openingConditionIndex <= previousClosingIndex)
                {
                    i++;
                    continue;
                }
                if (openingConditionIndex < closingConditionIndex)
                {
                    openingConditionIndex++;
                    closingConditionIndex++;
                    if (closingConditionIndex == ohlcvData.RowCount) break;
                    openingPrice = ohlcvData["Open"][openingConditionIndex];
                    k = closingConditionIndex;
                    reason = 'C';
                    closingPrice = ohlcvData["Open"][closingConditionIndex];
                    stopReached = false;
                    if (OpeningCondition.StopLoss != null || OpeningCondition.StopGain != null)
                    {
                        k = openingConditionIndex;
                        if (OpeningCondition.PositionType == PositionType.Long)
                        {
                            stopLossPrice = OpeningCondition.StopLoss == null ? double.NaN : (1 - OpeningCondition.StopLoss.Value) * openingPrice;
                            stopGainPrice = OpeningCondition.StopGain == null ? double.NaN : (1 + OpeningCondition.StopGain.Value) * openingPrice;
                        }
                        else
                        {
                            stopLossPrice = OpeningCondition.StopLoss == null ? double.NaN : (1 + OpeningCondition.StopLoss.Value) * openingPrice;
                            stopGainPrice = OpeningCondition.StopGain == null ? double.NaN : (1 - OpeningCondition.StopGain.Value) * openingPrice;
                        }
                        for (; k < closingConditionIndex && !stopReached; k++)
                        {
                            if (OpeningCondition.PositionType == PositionType.Long)
                            {
                                if (ohlcvData["Low"][k] <= stopLossPrice)
                                {
                                    stopReached = true;
                                    reason = 'L';
                                    closingPrice = stopLossPrice;
                                }
                                else if (ohlcvData["High"][k] >= stopGainPrice)
                                {
                                    stopReached = true;
                                    reason = 'G';
                                    closingPrice = stopGainPrice;
                                }
                            }
                            else
                            {
                                if (ohlcvData["High"][k] >= stopLossPrice)
                                {
                                    stopReached = true;
                                    reason = 'L';
                                    closingPrice = stopLossPrice;
                                }
                                else if (ohlcvData["Low"][k] <= stopGainPrice)
                                {
                                    stopReached = true;
                                    reason = 'G';
                                    closingPrice = stopLossPrice;
                                }
                            }
                        }
                    }
                    trueOpeningIndices.Add(openingConditionIndex);
                    trueClosingIndices.Add(k);
                    closingReason.Add(reason);
                    gainLoss.Add(OpeningCondition.PositionType == PositionType.Long ? (closingPrice - openingPrice) / openingPrice : (openingPrice - closingPrice) / openingPrice);
                    previousClosingIndex = k;
                    i++;
                }
                j++;
            }
            Result.OpeningIndices = trueOpeningIndices.ToArray();
            Result.ClosingIndices = trueClosingIndices.ToArray();
            Result.ClosingReasons = closingReason.ToArray();
            Result.GainLoss = gainLoss.ToArray();
        }
        private static int[] EvaluateExpression(string postfixExpression, Table<double> ohlcvData)
        {
            postfixExpression = postfixExpression.Trim();
            string[] tokens = postfixExpression.Split(' ');
            Stack<Column<double>> doubleStack = new();
            Stack<int[]> booleanStack = new();
            Stack<double> constantStack = new();
            // D = double, B = boolean, C = constant
            Stack<char> nextOperandType = new();
            foreach (string  token in tokens)
            {
                // It's a study
                if (char.IsLetter(token[0]))
                {
                    GetStudyParameters(token, out string studyName, out string specificStudyName, out List<string> parameters, out int displaceAmount, out bool isBoolean);
                    if (isBoolean)
                    {
                        nextOperandType.Push('B');
                        booleanStack.Push(GetBooleanStudyTrueIndices(studyName, specificStudyName, parameters, displaceAmount, ohlcvData));
                    }
                    else
                    {
                        nextOperandType.Push('D');
                        doubleStack.Push(GetStudy(studyName, specificStudyName, parameters, displaceAmount, ohlcvData));
                    }
                }
                // It's a constant
                else if (char.IsDigit(token[0]))
                {
                    nextOperandType.Push('C');
                    constantStack.Push(Convert.ToDouble(token));
                }
                // It's an operator
                else
                {
                    char nextOperand2 = nextOperandType.Pop();
                    char nextOperand1 = nextOperandType.Pop();
                    if (nextOperand1 == 'D')
                    {
                        if (nextOperand2 == 'D')
                        {
                            Column<double> operand2 = doubleStack.Pop();
                            Column<double> operand1 = doubleStack.Pop();
                            if (IsBooleanOperator(token[0]))
                            {
                                booleanStack.Push(GetTrueIndices(operand1, operand2, token[0]));
                                nextOperandType.Push('B');
                            }
                            else
                            {
                                doubleStack.Push(CalculateColumn(operand1, operand2, token[0]));
                                nextOperandType.Push('D');
                            }
                        }
                        else if (nextOperand2 == 'C')
                        {
                            Column<double> operand1 = doubleStack.Pop();
                            double operand2 = constantStack.Pop();
                            if (IsBooleanOperator(token[0]))
                            {
                                booleanStack.Push(GetTrueIndices(operand1, operand2, token[0]));
                                nextOperandType.Push('B');
                            }
                            else
                            {
                                doubleStack.Push(CalculateColumn(operand1, operand2, token[0]));
                                nextOperandType.Push('D');
                            }
                        }
                        else if (nextOperand2 == 'B')
                            throw new ArithmeticException("Error. Operations between Column<double> objects and Column<byte> objects are not valid.");
                    }
                    else if (nextOperand1 == 'C')
                    {
                        if (nextOperand2 == 'D')
                        {
                            Column<double> operand2 = doubleStack.Pop();
                            double operand1 = constantStack.Pop();
                            if (IsBooleanOperator(token[0]))
                            {
                                booleanStack.Push(GetTrueIndices(operand1, operand2, token[0]));
                                nextOperandType.Push('B');
                            }
                            else
                            {
                                doubleStack.Push(CalculateColumn(operand1, operand2, token[0]));
                                nextOperandType.Push('D');
                            }
                        }
                        else if (nextOperand2 == 'C')
                        {
                            double operand2 = constantStack.Pop();
                            double operand1 = constantStack.Pop();
                            constantStack.Push(CalculateConstant(operand1, operand2, token[0]));
                            nextOperandType.Push('C');
                        }
                        else if (nextOperand2 == 'B')
                            throw new ArithmeticException("Error. Operations between constants and Column<byte> objects are not valid.");
                    }
                    else if (nextOperand1 == 'B')
                    {
                        if (nextOperand2 == 'D')
                            throw new ArithmeticException("Error. Operations between Column<double> objects and Column<byte> objects are not valid.");
                        else if(nextOperand2 == 'C')
                            throw new ArithmeticException("Error. Operations between constants and Column<byte> objects are not valid.");
                        else if (nextOperand2 == 'B')
                        {
                            int[] operand2 = booleanStack.Pop();
                            int[] operand1 = booleanStack.Pop();
                            booleanStack.Push(GetTrueIndices(operand1, operand2, token[0], ohlcvData.RowCount));
                            nextOperandType.Push('B');
                        }
                    }
                }
            }
            return booleanStack.Pop();
        }
        private static bool IsBooleanOperator(char op)
        {
            return !(op == '+' || op == '-' || op == '*' || op == '/');
        }
        private static void GetStudyParameters(string studyString, out string studyName, out string specificStudyName, out List<string> parameters,
            out int displaceAmount, out bool isBoolean)
        {
            parameters = new();
            int i = 0;
            int state = 0;
            studyName = "";
            specificStudyName = "";
            string displaceString = "";
            string tempParam = "";
            isBoolean = false;
            while (i < studyString.Length)
            {
                if (state == 0)
                {
                    if (studyString[i] == '(')
                        state = 1;
                    else
                        studyName += studyString[i];
                }
                else if (state == 1)
                {
                    if (studyString[i] == ',' || studyString[i] == ')')
                    {
                        parameters.Add(tempParam);
                        tempParam = "";
                    }
                    else
                        tempParam += studyString[i];

                    if (studyString[i] == ')')
                        state = 2;
                }
                else if (state == 2)
                {
                    if (studyString[i] == '.')
                        state = 3;
                    else if (studyString[i] == '[')
                        state = 4;
                }
                else if (state == 3)
                {
                    if (studyString[i] == '[')
                        state = 4;
                    else
                        specificStudyName += studyString[i];
                }
                else
                {
                    if (studyString[i] == ']')
                        break;
                    else
                        displaceString += studyString[i];
                }
                i++;
            }
            displaceAmount = Convert.ToInt32(displaceString);
            StudyType studyType = Enum.Parse<StudyType>(studyName);
            if (studyType == StudyType.TTMSqueeze)
            {
                TTMSqueezeValueTypes specificStudyType = Enum.Parse<TTMSqueezeValueTypes>(specificStudyName);
                isBoolean = specificStudyType == TTMSqueezeValueTypes.Signal;
            }
        }
        private static Column<double> GetStudy(string studyName, string specificStudyName, List<string> parameters, int displaceAmount, Table<double> ohlcvData)
        {
            StudyType study = Enum.Parse<StudyType>(studyName);
            switch (study)
            {
                case StudyType.AverageTrueRange:
                    Column<double> atr = AverageTrueRange.Calculate(ohlcvData, Convert.ToInt32(parameters[0]), Enum.Parse<MovingAverageType>(parameters[1]));
                    atr.Displace(displaceAmount);
                    return atr;
                case StudyType.BollingerBands:
                    { 
                        MarketDataType marketDataType = Enum.Parse<MarketDataType>(parameters[1]);
                        Table<double> bollingerBands = BollingerBands.Calculate(Utils.Utils.GetMarketData(ohlcvData, marketDataType),
                            Convert.ToInt32(parameters[0]), Convert.ToDouble(parameters[2]), Enum.Parse<MovingAverageType>(parameters[3]));
                        switch (Enum.Parse<BandTypes>(specificStudyName))
                        {
                            case BandTypes.Lower:
                                bollingerBands.Columns[0].Displace(displaceAmount);
                                return bollingerBands.Columns[0];
                            case BandTypes.Middle:
                                bollingerBands.Columns[1].Displace(displaceAmount);
                                return bollingerBands.Columns[1];
                            case BandTypes.Upper:
                                bollingerBands.Columns[2].Displace(displaceAmount);
                                return bollingerBands.Columns[2];
                        }
                    }
                    break;
                case StudyType.DonchianChannel:
                    {
                        Table<double> donchianChannels = DonchianChannels.Calculate(ohlcvData, Convert.ToInt32(parameters[0]));
                        switch (Enum.Parse<BandTypes>(specificStudyName))
                        {
                            case BandTypes.Lower:
                                donchianChannels.Columns[0].Displace(displaceAmount);
                                return donchianChannels.Columns[0];
                            case BandTypes.Middle:
                                donchianChannels.Columns[1].Displace(displaceAmount);
                                return donchianChannels.Columns[1];
                            case BandTypes.Upper:
                                donchianChannels.Columns[2].Displace(displaceAmount);
                                return donchianChannels.Columns[2];
                        }
                    }
                    break;
                case StudyType.ExponentialMovingAverage:
                    {
                        MarketDataType marketDataType = Enum.Parse<MarketDataType>(parameters[1]);
                        Column<double> ema = ExponentialMovingAverage.Calculate(Utils.Utils.GetMarketData(ohlcvData, marketDataType),
                            Convert.ToInt32(parameters[0]));
                        ema.Displace(displaceAmount);
                        return ema;
                    }
                case StudyType.KeltnerChannels:
                    {
                        Table<double> keltnerChannels = KeltnerChannels.Calculate(ohlcvData, Convert.ToInt32(parameters[0]), Convert.ToDouble(parameters[2]),
                            Enum.Parse<MovingAverageType>(parameters[3]), Enum.Parse<MovingAverageType>(parameters[4]));
                        switch (Enum.Parse<BandTypes>(specificStudyName))
                        {
                            case BandTypes.Lower:
                                keltnerChannels.Columns[0].Displace(displaceAmount);
                                return keltnerChannels.Columns[0];
                            case BandTypes.Middle:
                                keltnerChannels.Columns[1].Displace(displaceAmount);
                                return keltnerChannels.Columns[1];
                            case BandTypes.Upper:
                                keltnerChannels.Columns[2].Displace(displaceAmount);
                                return keltnerChannels.Columns[2];
                        }
                    }
                    break;
                case StudyType.LinearRegression:
                    {
                        MarketDataType marketDataType = Enum.Parse<MarketDataType>(parameters[1]);
                        Table<double> linearRegression = LeastSquaresLinearRegression.Calculate(Utils.Utils.GetMarketData(ohlcvData, marketDataType), Convert.ToInt32(parameters[0]));
                        switch (Enum.Parse<LinearRegressionValueTypes>(specificStudyName))
                        {
                            case LinearRegressionValueTypes.Slope:
                                linearRegression[0].Displace(displaceAmount);
                                return linearRegression[0];
                            case LinearRegressionValueTypes.Intercept:
                                linearRegression[1].Displace(displaceAmount);
                                return linearRegression[1];
                            case LinearRegressionValueTypes.R2:
                                linearRegression[2].Displace(displaceAmount);
                                return linearRegression[2];
                            case LinearRegressionValueTypes.Predicted:
                                linearRegression[3].Displace(displaceAmount);
                                return linearRegression[3];
                        }
                    }
                    break;
                case StudyType.RelativeStrengthIndex:
                    {
                        MarketDataType marketDataType = Enum.Parse<MarketDataType>(parameters[1]);
                        Column<double> rsi = RelativeStrenghtIndex.Calculate(Utils.Utils.GetMarketData(ohlcvData, marketDataType),
                            Convert.ToInt32(parameters[0]), Enum.Parse<MovingAverageType>(parameters[2]));
                        rsi.Displace(displaceAmount);
                        return rsi;
                    }
                case StudyType.SimpleMovingAverage:
                    {
                        MarketDataType marketDataType = Enum.Parse<MarketDataType>(parameters[1]);
                        Column<double> sma = SimpleMovingAverage.Calculate(Utils.Utils.GetMarketData(ohlcvData, marketDataType), Convert.ToInt32(parameters[0]));
                        sma.Displace(displaceAmount);
                        return sma;
                    }
                case StudyType.StandardDeviation:
                    {
                        MarketDataType marketDataType = Enum.Parse<MarketDataType>(parameters[1]);
                        Column<double> stDev = StandardDeviation.Calculate(Utils.Utils.GetMarketData(ohlcvData, marketDataType), Convert.ToInt32(parameters[0]));
                        stDev.Displace(displaceAmount);
                        return stDev;
                    }
                case StudyType.TrueRange:
                    {
                        Column<double> tr = TrueRange.Calculate(ohlcvData);
                        tr.Displace(displaceAmount);
                        return tr;
                    }
                case StudyType.TTMSqueeze:
                    {
                        Column<double> histogram;
                        (histogram, _) = TTMSqueeze.Calculate(ohlcvData, Convert.ToInt32(parameters[0]),
                            Convert.ToDouble(parameters[3]), Convert.ToDouble(parameters[2]), Enum.Parse<MarketDataType>(parameters[1]));
                        TTMSqueezeValueTypes specificColumn = Enum.Parse<TTMSqueezeValueTypes>(specificStudyName);
                        if (specificColumn == TTMSqueezeValueTypes.Histogram)
                        {
                            histogram.Displace(displaceAmount);
                            return histogram;
                        }
                        break;
                    }
                case StudyType.WeigthedMovingAverage:
                    {
                        MarketDataType marketDataType = Enum.Parse<MarketDataType>(parameters[1]);
                        Column<double> wma = WeightedMovingAverage.Calculate(Utils.Utils.GetMarketData(ohlcvData, marketDataType), Convert.ToInt32(parameters[0]));
                        wma.Displace(displaceAmount);
                        return wma;
                    }
                case StudyType.WildersMovingAverage:
                    {
                        MarketDataType marketDataType = Enum.Parse<MarketDataType>(parameters[1]);
                        Column<double> wma = WeightedMovingAverage.Calculate(Utils.Utils.GetMarketData(ohlcvData, marketDataType), Convert.ToInt32(parameters[0]));
                        wma.Displace(displaceAmount);
                        return wma;
                    }
            }
            return new Column<double>("Null", 0);
        }
        private static int[] GetBooleanStudyTrueIndices(string studyName, string specificStudyName, List<string> parameters, int displaceAmount, Table<double> ohlcvData)
        {
            StudyType study = Enum.Parse<StudyType>(studyName);
            switch (study)
            {
                case StudyType.TTMSqueeze:
                    {
                        Column<byte> signal;
                        (_, signal) = TTMSqueeze.Calculate(ohlcvData, Convert.ToInt32(parameters[0]),
                            Convert.ToDouble(parameters[3]), Convert.ToDouble(parameters[2]), Enum.Parse<MarketDataType>(parameters[1]));
                        TTMSqueezeValueTypes specificColumn = Enum.Parse<TTMSqueezeValueTypes>(specificStudyName);
                        if (specificColumn == TTMSqueezeValueTypes.Histogram)
                        {
                            signal.Displace(displaceAmount);
                            return signal == 1;
                        }
                    }
                    break;
            }
            return Array.Empty<int>();
        }
        private static Column<double> CalculateColumn(Column<double> column1, Column<double> column2, char op)
        {
            switch (op)
            {
                case '+':
                    return column1 + column2;
                case '-':
                    return column1 - column2;
                case '*':
                    return column1 * column2;
                case '/':
                    return column1 / column2;
                default:
                    break;
            }
            throw new ArithmeticException($"The operator '{op}' is not supported between two Column<double> objects.");
        }
        private static Column<double> CalculateColumn(Column<double> column, double value, char op)
        {
            switch (op)
            {
                case '+':
                    return column + value;
                case '-':
                    return column - value;
                case '*':
                    return column * value;
                case '/':
                    return column / value;
                default:
                    break;
            }
            throw new ArithmeticException($"The operator '{op}' is not supported between a Column<double> object and a constant.");
        }
        private static Column<double> CalculateColumn(double value, Column<double> column, char op)
        {
            switch (op)
            {
                case '+':
                    return value + column;
                case '-':
                    return value - column;
                case '*':
                    return value * column;
                case '/':
                    return value / column;
                default:
                    break;
            }
            throw new ArithmeticException($"The operator '{op}' is not supported between a Column<double> object and a constant.");
        }
        private static int[] GetTrueIndices(Column<double> column1, Column<double> column2, char op)
        {
            switch (op)
            {
                case '«':
                    return column1 <= column2;
                case '<':
                    return column1 < column2;
                case '»':
                    return column1 >= column2;
                case '>':
                    return column1 > column2;
                case '!':
                    return column1 != column2;
                case '=':
                    return column1 == column2;
                case '}':
                    return column1.CrossesAbove(column2);
                case '{':
                    return column1.CrossesBelow(column2);
                default:
                    break;
            }
            throw new ArithmeticException($"The operator '{op}' is not supported between two Column<double> objects.");
        }
        private static int[] GetTrueIndices(Column<double> column, double value, char op)
        {
            switch (op)
            {
                case '«':
                    return column <= value;
                case '<':
                    return column < value;
                case '»':
                    return column >= value;
                case '>':
                    return column > value;
                case '!':
                    return column != value;
                case '=':
                    return column == value;
                case '}':
                    return column.CrossesAbove(value);
                case '{':
                    return column.CrossesBelow(value);
                default:
                    break;
            }
            throw new ArithmeticException($"The operator '{op}' is not supported between a Column<double> object and a constant.");
        }
        private static int[] GetTrueIndices(double value, Column<double> column, char op)
        {
            switch (op)
            {
                case '«':
                    return value <= column;
                case '<':
                    return value < column;
                case '»':
                    return value >= column;
                case '>':
                    return value > column;
                case '!':
                    return value != column;
                case '=':
                    return value == column;
                case '}':
                    return column.CrossesBelow(value);
                case '{':
                    return column.CrossesAbove(value);
                default:
                    break;
            }
            throw new ArithmeticException($"The operator '{op}' is not supported between a Column<double> object and a constant.");
        }
        private static int[] GetTrueIndices(int[] indices1, int[] indices2, char op, int originalColumnLength)
        {
            switch (op)
            {
                case '&':
                    return indices1.Intersect(indices2).ToArray();
                case '|':
                    return indices1.Union(indices2).ToArray();
                // This solution will give the wrong result if the indices passed in come from Column<T>.GetTrueIndices() applied on
                // a Column<double> with NaNs in it. However, this function will never receive such an input in this implementation.
                case '!':
                    {
                        Column<byte> column1 = Column.FromTrueIndices(indices1, originalColumnLength);
                        Column<byte> column2 = Column.FromTrueIndices(indices2, originalColumnLength);
                        return column1 != column2;
                    }
                case '=':
                    {
                        Column<byte> column1 = Column.FromTrueIndices(indices1, originalColumnLength);
                        Column<byte> column2 = Column.FromTrueIndices(indices2, originalColumnLength);
                        return column1 == column2;
                    }
                default:
                    break;
            }
            throw new ArithmeticException($"The operator '{op}' is not supported between two Column<byte> objects.");
        }
        private static double CalculateConstant(double value1, double value2, char op)
        {
            switch (op)
            {
                case '+': return value1 + value2;
                case '-': return value1 - value2;
                case '*': return value1 * value2;
                case '/': return value1 / value2;
                default: break;
            }
            throw new ArithmeticException($"The operator '{op}' is not supported between two constants.");
        }
    }
}
