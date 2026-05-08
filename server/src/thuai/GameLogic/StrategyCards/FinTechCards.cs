namespace Thuai.GameLogic.StrategyCards;

public class NetworkStorm : StrategyCard
{
    public override string Name => "网络风暴";
    public override CardCategory Category => CardCategory.FinTech;
    public override string Description => "全局最多使用3次，使目标的下一次下单额外滞后1天";
    public override bool IsPassive => false;

    public int UsesUsedThisGame { get; private set; }
    public int MaxUses => 3;
    public long ActivationCost => (UsesUsedThisGame + 1) * 1_000L;

    public bool CanUse => UsesUsedThisGame < MaxUses;

    public void MarkUsed()
    {
        UsesUsedThisGame++;
    }
}

public class PublicOpinionAttack : StrategyCard
{
    public override string Name => "舆情打击";
    public override CardCategory Category => CardCategory.FinTech;
    public override string Description => "全局仅可使用1次，伪造一条快讯并污染他人的下一条广播";
    public override bool IsPassive => false;

    public bool UsedThisGame { get; private set; }
    public long ActivationCost => 20_000;
    public bool CanUse => !UsedThisGame;

    public void MarkUsed()
    {
        UsedThisGame = true;
    }
}
