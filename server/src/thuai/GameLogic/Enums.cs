namespace Thuai.GameLogic;

public enum GameStage
{
    Waiting,
    PreparingGame,
    StrategySelection,
    TradingDay,
    Settlement,
    Finished
}

public enum OrderSide { Buy, Sell }
public enum OrderStatus { Pending, PartiallyFilled, Filled, Cancelled }
public enum OrderIntent { Immediate, Resting }
public enum NewsSentiment { Bullish, Bearish }
public enum Prediction { Long, Short, Hold }
