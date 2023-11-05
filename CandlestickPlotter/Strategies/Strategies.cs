
using CandleStickPlotter.DataTypes;
using CandleStickPlotter.Studies.Momentum;
using CandleStickPlotter.Studies.MovingAverages;
using CandleStickPlotter.Studies.Volatility;
using CandleStickPlotter.Utils;
using System;

namespace CandleStickPlotter.Strategies
{
    public enum AvailableStudies
    {
        AverageTrueRange,
        BollingerBand,
        DonchianChannel,
        ExponentialMovingAverage,
        KeltnerChannel,
        LinearRegression,
        RelativeStrengthIndex,
        SimpleMovingAverage,
        StandardDeviation,
        TrueRange,
        TTMSqueeze,
        WeigthedMovingAverage,
        WildersMovingAverage
    }
    public enum BandsAvailableValues : byte { Lower, Middle, Upper }
    public enum LinearRegressionAvailableValues : byte { Intercept, Predicted, R2, Slope }
    public enum PositionType : byte { Long, Short }
    public enum TTMSqueezeAvailableValues : byte { Histogram, Signal}
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
        public Strategy(OpeningCondition openingCondition, ClosingCondition closingCondition, OhlcData ohlcData, int id = 0)
        {
            OpeningCondition = openingCondition;
            ClosingCondition = closingCondition;
            Result = new StrategyResult(id, EvaluateExpression(OpeningCondition.PostfixExpression, ohlcData).GetTrueIndices(),
            EvaluateExpression(ClosingCondition.PostfixExpression, ohlcData).GetTrueIndices(), Array.Empty<char>(), Array.Empty<double>());
            Evaluate(ohlcData);
        }
        private void Evaluate(OhlcData ohlcData)
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
                    if (closingConditionIndex == ohlcData.Length) break;
                    openingPrice = ohlcData.Open[openingConditionIndex];
                    k = closingConditionIndex;
                    reason = 'C';
                    closingPrice = ohlcData.Open[closingConditionIndex];
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
                                if (ohlcData.Low[k] <= stopLossPrice)
                                {
                                    stopReached = true;
                                    reason = 'L';
                                    closingPrice = stopLossPrice;
                                }
                                else if (ohlcData.High[k] >= stopGainPrice)
                                {
                                    stopReached = true;
                                    reason = 'G';
                                    closingPrice = stopGainPrice;
                                }
                            }
                            else
                            {
                                if (ohlcData.High[k] >= stopLossPrice)
                                {
                                    stopReached = true;
                                    reason = 'L';
                                    closingPrice = stopLossPrice;
                                }
                                else if (ohlcData.Low[k] <= stopGainPrice)
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
        private static ColumnBool EvaluateExpression(string postfixExpression, OhlcData ohlcData)
        {
            postfixExpression = postfixExpression.Trim();
            string[] tokens = postfixExpression.Split(' ');
            Stack<ColumnDouble> doubleStack = new();
            Stack<ColumnBool> booleanStack = new();
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
                        booleanStack.Push(GetBooleanStudy(studyName, specificStudyName, parameters, displaceAmount, ohlcData));
                    }
                    else
                    {
                        nextOperandType.Push('D');
                        doubleStack.Push(GetStudy(studyName, specificStudyName, parameters, displaceAmount, ohlcData));
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
                            ColumnDouble operand2 = doubleStack.Pop();
                            ColumnDouble operand1 = doubleStack.Pop();
                            if (IsBooleanOperator(token[0]))
                            {
                                booleanStack.Push(CalculateBooleanColumn(operand1, operand2, token[0]));
                                nextOperandType.Push('B');
                            }
                            else
                            {
                                doubleStack.Push(CalculateColumnDouble(operand1, operand2, token[0]));
                                nextOperandType.Push('D');
                            }
                        }
                        else if (nextOperand2 == 'C')
                        {
                            ColumnDouble operand1 = doubleStack.Pop();
                            double operand2 = constantStack.Pop();
                            if (IsBooleanOperator(token[0]))
                            {
                                booleanStack.Push(CalculateBooleanColumn(operand1, operand2, token[0]));
                                nextOperandType.Push('B');
                            }
                            else
                            {
                                doubleStack.Push(CalculateColumnDouble(operand1, operand2, token[0]));
                                nextOperandType.Push('D');
                            }
                        }
                        else if (nextOperand2 == 'B')
                            throw new ArithmeticException("Error. Operations between ColumnDouble objects and ColumnBool objects are not valid.");
                    }
                    else if (nextOperand1 == 'C')
                    {
                        if (nextOperand2 == 'D')
                        {
                            ColumnDouble operand2 = doubleStack.Pop();
                            double operand1 = constantStack.Pop();
                            if (IsBooleanOperator(token[0]))
                            {
                                booleanStack.Push(CalculateBooleanColumn(operand1, operand2, token[0]));
                                nextOperandType.Push('B');
                            }
                            else
                            {
                                doubleStack.Push(CalculateColumnDouble(operand1, operand2, token[0]));
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
                            throw new ArithmeticException("Error. Operations between constants and ColumnBool objects are not valid.");
                    }
                    else if (nextOperand1 == 'B')
                    {
                        if (nextOperand2 == 'D')
                            throw new ArithmeticException("Error. Operations between ColumnDouble objects and ColumnBool objects are not valid.");
                        else if(nextOperand2 == 'C')
                            throw new ArithmeticException("Error. Operations between constants and ColumnBool objects are not valid.");
                        else if (nextOperand2 == 'B')
                        {
                            ColumnBool operand2 = booleanStack.Pop();
                            ColumnBool operand1 = booleanStack.Pop();
                            booleanStack.Push(CalculateBooleanColumn(operand1, operand2, token[0]));
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
            AvailableStudies studyType = Enum.Parse<AvailableStudies>(studyName);
            if (studyType == AvailableStudies.TTMSqueeze)
            {
                TTMSqueezeAvailableValues specificStudyType = Enum.Parse<TTMSqueezeAvailableValues>(specificStudyName);
                isBoolean = specificStudyType == TTMSqueezeAvailableValues.Signal;
            }
        }
        private static ColumnDouble GetStudy(string studyName, string specificStudyName, List<string> parameters, int displaceAmount, OhlcData data)
        {
            AvailableStudies study = Enum.Parse<AvailableStudies>(studyName);
            switch (study)
            {
                case AvailableStudies.AverageTrueRange:
                    ColumnDouble atr = AverageTrueRange.Calculate(data, Convert.ToInt32(parameters[0]), Enum.Parse<MovingAverageType>(parameters[1]));
                    atr.Displace(displaceAmount);
                    return atr;
                case AvailableStudies.BollingerBand:
                    { 
                        MarketDataType marketDataType = Enum.Parse<MarketDataType>(parameters[1]);
                        TableDouble bollingerBands = BollingerBands.Calculate(Utils.Utils.GetMarketData(data, marketDataType),
                            Convert.ToInt32(parameters[0]), Convert.ToDouble(parameters[2]), Enum.Parse<MovingAverageType>(parameters[3]));
                        switch (Enum.Parse<BandsAvailableValues>(specificStudyName))
                        {
                            case BandsAvailableValues.Lower:
                                bollingerBands.Columns[0].Displace(displaceAmount);
                                return bollingerBands.Columns[0];
                            case BandsAvailableValues.Middle:
                                bollingerBands.Columns[1].Displace(displaceAmount);
                                return bollingerBands.Columns[1];
                            case BandsAvailableValues.Upper:
                                bollingerBands.Columns[2].Displace(displaceAmount);
                                return bollingerBands.Columns[2];
                        }
                    }
                    break;
                case AvailableStudies.DonchianChannel:
                    {
                        TableDouble donchianChannels = DonchianChannels.Calculate(data, Convert.ToInt32(parameters[0]));
                        switch (Enum.Parse<BandsAvailableValues>(specificStudyName))
                        {
                            case BandsAvailableValues.Lower:
                                donchianChannels.Columns[0].Displace(displaceAmount);
                                return donchianChannels.Columns[0];
                            case BandsAvailableValues.Middle:
                                donchianChannels.Columns[1].Displace(displaceAmount);
                                return donchianChannels.Columns[1];
                            case BandsAvailableValues.Upper:
                                donchianChannels.Columns[2].Displace(displaceAmount);
                                return donchianChannels.Columns[2];
                        }
                    }
                    break;
                case AvailableStudies.ExponentialMovingAverage:
                    {
                        MarketDataType marketDataType = Enum.Parse<MarketDataType>(parameters[1]);
                        ColumnDouble ema = ExponentialMovingAverage.Calculate(Utils.Utils.GetMarketData(data, marketDataType),
                            Convert.ToInt32(parameters[0]));
                        ema.Displace(displaceAmount);
                        return ema;
                    }
                case AvailableStudies.KeltnerChannel:
                    {
                        TableDouble keltnerChannels = KeltnerChannels.Calculate(data, Convert.ToInt32(parameters[0]), Convert.ToDouble(parameters[2]),
                            Enum.Parse<MovingAverageType>(parameters[3]), Enum.Parse<MovingAverageType>(parameters[4]));
                        switch (Enum.Parse<BandsAvailableValues>(specificStudyName))
                        {
                            case BandsAvailableValues.Lower:
                                keltnerChannels.Columns[0].Displace(displaceAmount);
                                return keltnerChannels.Columns[0];
                            case BandsAvailableValues.Middle:
                                keltnerChannels.Columns[1].Displace(displaceAmount);
                                return keltnerChannels.Columns[1];
                            case BandsAvailableValues.Upper:
                                keltnerChannels.Columns[2].Displace(displaceAmount);
                                return keltnerChannels.Columns[2];
                        }
                    }
                    break;
                case AvailableStudies.LinearRegression:
                    {
                        MarketDataType marketDataType = Enum.Parse<MarketDataType>(parameters[1]);
                        TableDouble linearRegression = LeastSquaresLinearRegression.Calculate(Utils.Utils.GetMarketData(data, marketDataType), Convert.ToInt32(parameters[0]));
                        switch (Enum.Parse<LinearRegressionAvailableValues>(specificStudyName))
                        {
                            case LinearRegressionAvailableValues.Slope:
                                linearRegression[0].Displace(displaceAmount);
                                return linearRegression[0];
                            case LinearRegressionAvailableValues.Intercept:
                                linearRegression[1].Displace(displaceAmount);
                                return linearRegression[1];
                            case LinearRegressionAvailableValues.R2:
                                linearRegression[2].Displace(displaceAmount);
                                return linearRegression[2];
                            case LinearRegressionAvailableValues.Predicted:
                                linearRegression[3].Displace(displaceAmount);
                                return linearRegression[3];
                        }
                    }
                    break;
                case AvailableStudies.RelativeStrengthIndex:
                    {
                        MarketDataType marketDataType = Enum.Parse<MarketDataType>(parameters[1]);
                        ColumnDouble rsi = RelativeStrenghtIndex.Calculate(Utils.Utils.GetMarketData(data, marketDataType),
                            Convert.ToInt32(parameters[0]), Enum.Parse<MovingAverageType>(parameters[2]));
                        rsi.Displace(displaceAmount);
                        return rsi;
                    }
                case AvailableStudies.SimpleMovingAverage:
                    {
                        MarketDataType marketDataType = Enum.Parse<MarketDataType>(parameters[1]);
                        ColumnDouble sma = SimpleMovingAverage.Calculate(Utils.Utils.GetMarketData(data, marketDataType), Convert.ToInt32(parameters[0]));
                        sma.Displace(displaceAmount);
                        return sma;
                    }
                case AvailableStudies.StandardDeviation:
                    {
                        MarketDataType marketDataType = Enum.Parse<MarketDataType>(parameters[1]);
                        ColumnDouble stDev = StandardDeviation.Calculate(Utils.Utils.GetMarketData(data, marketDataType), Convert.ToInt32(parameters[0]));
                        stDev.Displace(displaceAmount);
                        return stDev;
                    }
                case AvailableStudies.TrueRange:
                    {
                        ColumnDouble tr = TrueRange.Calculate(data);
                        tr.Displace(displaceAmount);
                        return tr;
                    }
                case AvailableStudies.TTMSqueeze:
                    {
                        TableDouble ttmSqueeze = TTMSqueeze.Calculate(data, Convert.ToInt32(parameters[0]),
                            Convert.ToDouble(parameters[3]), Convert.ToDouble(parameters[2]), Enum.Parse<MarketDataType>(parameters[1]));
                        TTMSqueezeAvailableValues specificColumn = Enum.Parse<TTMSqueezeAvailableValues>(specificStudyName);
                        if (specificColumn == TTMSqueezeAvailableValues.Histogram)
                        {
                            ttmSqueeze[0].Displace(displaceAmount);
                            return ttmSqueeze[0];
                        }
                        break;
                    }
                case AvailableStudies.WeigthedMovingAverage:
                    {
                        MarketDataType marketDataType = Enum.Parse<MarketDataType>(parameters[1]);
                        ColumnDouble wma = WeightedMovingAverage.Calculate(Utils.Utils.GetMarketData(data, marketDataType), Convert.ToInt32(parameters[0]));
                        wma.Displace(displaceAmount);
                        return wma;
                    }
                case AvailableStudies.WildersMovingAverage:
                    {
                        MarketDataType marketDataType = Enum.Parse<MarketDataType>(parameters[1]);
                        ColumnDouble wma = WeightedMovingAverage.Calculate(Utils.Utils.GetMarketData(data, marketDataType), Convert.ToInt32(parameters[0]));
                        wma.Displace(displaceAmount);
                        return wma;
                    }
            }
            return new ColumnDouble("Null", 0);
        }
        private static ColumnBool GetBooleanStudy(string studyName, string specificStudyName, List<string> parameters, int displaceAmount, OhlcData data)
        {
            AvailableStudies study = Enum.Parse<AvailableStudies>(studyName);
            switch (study)
            {
                case AvailableStudies.TTMSqueeze:
                    {
                        TableDouble ttmSqueeze = TTMSqueeze.Calculate(data, Convert.ToInt32(parameters[0]),
                            Convert.ToDouble(parameters[3]), Convert.ToDouble(parameters[2]), Enum.Parse<MarketDataType>(parameters[1]));
                        TTMSqueezeAvailableValues specificColumn = Enum.Parse<TTMSqueezeAvailableValues>(specificStudyName);
                        if (specificColumn == TTMSqueezeAvailableValues.Histogram)
                        {
                            ttmSqueeze.BooleanColumns![0].Displace(displaceAmount);
                            return ttmSqueeze.BooleanColumns[0];
                        }
                    }
                    break;
            }
            return new ColumnBool();
        }
        private static ColumnDouble CalculateColumnDouble(ColumnDouble column1, ColumnDouble column2, char op)
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
            throw new ArithmeticException($"The operator '{op}' is not supported between two ColumnDouble objects.");
        }
        private static ColumnDouble CalculateColumnDouble(ColumnDouble column, double value, char op)
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
            throw new ArithmeticException($"The operator '{op}' is not supported between a ColumnDouble object and a constant.");
        }
        private static ColumnDouble CalculateColumnDouble(double value, ColumnDouble column, char op)
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
            throw new ArithmeticException($"The operator '{op}' is not supported between a ColumnDouble object and a constant.");
        }
        private static ColumnBool CalculateBooleanColumn(ColumnDouble column1, ColumnDouble column2, char op)
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
            throw new ArithmeticException($"The operator '{op}' is not supported between two ColumnDouble objects.");
        }
        private static ColumnBool CalculateBooleanColumn(ColumnDouble column, double value, char op)
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
            throw new ArithmeticException($"The operator '{op}' is not supported between a ColumnDouble object and a constant.");
        }
        private static ColumnBool CalculateBooleanColumn(double value, ColumnDouble column, char op)
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
            throw new ArithmeticException($"The operator '{op}' is not supported between a ColumnDouble object and a constant.");
        }
        private static ColumnBool CalculateBooleanColumn(ColumnBool column1, ColumnBool column2, char op)
        {
            switch (op)
            {
                case '&':
                    return column1 & column2;
                case '|':
                    return column1 | column2;
                case '!':
                    return column1 != column2;
                case '=':
                    return column1 == column2;
                default:
                    break;
            }
            throw new ArithmeticException($"The operator '{op}' is not supported between two ColumnBool objects.");
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
