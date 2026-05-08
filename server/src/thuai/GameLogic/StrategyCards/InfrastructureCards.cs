namespace Thuai.GameLogic.StrategyCards;

public class InsiderInfo : StrategyCard
{
    public override string Name => "内幕消息";
    public override CardCategory Category => CardCategory.Infrastructure;
    public override string Description => "花费摩拉提前3天获取下一条快讯，或以低价赌一半概率的伪消息";
    public override bool IsPassive => false;

    public bool UsedThisMonth { get; private set; }
    public int PreviewNewsDay { get; private set; }
    public int PreviewDeliveryDay { get; private set; }
    public bool PreviewIsFake { get; private set; }

    public long PremiumCost => 10_000;
    public long CheapCost => 500;

    public bool CanActivate(int currentDay, int nextNewsDay) =>
        !UsedThisMonth && nextNewsDay > currentDay + 3;

    public void Activate(Player player, int currentDay, int nextNewsDay, bool cheapMode, bool previewIsFake)
    {
        UsedThisMonth = true;
        PreviewNewsDay = nextNewsDay;
        PreviewDeliveryDay = nextNewsDay - 3;
        PreviewIsFake = previewIsFake;
        player.SetInsiderPriorityDay(nextNewsDay);
    }

    public bool TryConsumePreview(int currentDay, out bool isFake)
    {
        isFake = false;
        if (!UsedThisMonth || currentDay != PreviewDeliveryDay)
            return false;

        isFake = PreviewIsFake;
        return true;
    }

    public void ResetMonthly()
    {
        UsedThisMonth = false;
        PreviewNewsDay = 0;
        PreviewDeliveryDay = 0;
        PreviewIsFake = false;
        CurrentCooldown = 0;
    }
}

public class FlashTrading : StrategyCard
{
    public override string Name => "闪电交易";
    public override CardCategory Category => CardCategory.Infrastructure;
    public override string Description => "接下来的3天每天可多进行一次即时交易";
    public override bool IsPassive => false;

    private bool _usedThisMonth;
    private int _activeFromDay = -1;
    private int _activeUntilDay = -1;

    public long ActivationCost => 1_000;

    public bool CanActivateThisMonth => !_usedThisMonth;

    public override void OnActivate(Player player, int currentTick)
    {
        _usedThisMonth = true;
        _activeFromDay = currentTick + 1;
        _activeUntilDay = currentTick + 3;
    }

    public override void OnTick(Player player, int currentTick)
    {
        if (_activeFromDay >= 0 && currentTick >= _activeFromDay && currentTick <= _activeUntilDay)
        {
            player.BonusImmediateOrdersToday = 1;
        }
    }

    public bool IsActive(int currentDay) =>
        _activeFromDay >= 0 && currentDay >= _activeFromDay && currentDay <= _activeUntilDay;

    public void ResetMonthly()
    {
        _usedThisMonth = false;
        _activeFromDay = -1;
        _activeUntilDay = -1;
        CurrentCooldown = 0;
    }
}
