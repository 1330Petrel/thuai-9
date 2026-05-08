namespace Thuai.GameLogic.StrategyCards;

public class StopLossBlade : StrategyCard
{
    public override string Name => "止损名刀";
    public override CardCategory Category => CardCategory.RiskControl;
    public override string Description => "撤销全部挂单，并在接下来3天将盘面下跌造成的净值亏损降为20%";
    public override bool IsPassive => false;

    public bool UsedThisMonth { get; private set; }
    public int ImmuneUntilDay { get; private set; } = -1;
    public long ReferenceMidPrice { get; private set; }

    public long ActivationCost => 10_000;

    public void Activate(Player player, int currentDay, long referenceMidPrice)
    {
        UsedThisMonth = true;
        ReferenceMidPrice = referenceMidPrice;
        ImmuneUntilDay = currentDay + 2;
        player.IsImmune = true;
        player.ImmuneUntilTick = ImmuneUntilDay;
        player.ProtectedMidPrice = referenceMidPrice;
    }

    public override void OnTick(Player player, int currentDay)
    {
        if (ImmuneUntilDay >= 0 && currentDay > ImmuneUntilDay)
        {
            player.IsImmune = false;
            player.ProtectedMidPrice = 0;
        }
    }

    public void ResetMonthly()
    {
        UsedThisMonth = false;
        ImmuneUntilDay = -1;
        ReferenceMidPrice = 0;
        CurrentCooldown = 0;
    }
}

public class TargetedPurchase : StrategyCard
{
    public override string Name => "定向增发";
    public override CardCategory Category => CardCategory.RiskControl;
    public override string Description => "以买一价2%折扣直接购买100单位黄金，锁定10天";
    public override bool IsPassive => false;

    private bool _usedThisMonth;

    public long ActivationCost => 0;
    public bool IsUsed => _usedThisMonth;
    public int LockDuration => 10;
    public int PurchaseQuantity => 100;
    public double DiscountRate => 0.02;

    public void MarkUsed()
    {
        _usedThisMonth = true;
    }

    public void ResetMonthly()
    {
        _usedThisMonth = false;
        CurrentCooldown = 0;
    }
}
