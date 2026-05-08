namespace Thuai.GameLogic.StrategyCards;

/// <summary>
/// Manages the strategy card draft process between months.
/// Each month: randomly picks 1 card from each category (no repeats across months),
/// and both players blindly choose within the strategy phase.
/// </summary>
public class StrategyCardManager
{
    private readonly Random _rng = new();

    private static readonly List<Func<IStrategyCard>> InfrastructureFactory =
    [
        () => new InsiderInfo(),
        () => new FlashTrading()
    ];

    private static readonly List<Func<IStrategyCard>> RiskControlFactory =
    [
        () => new StopLossBlade(),
        () => new TargetedPurchase()
    ];

    private static readonly List<Func<IStrategyCard>> FinTechFactory =
    [
        () => new NetworkStorm(),
        () => new PublicOpinionAttack()
    ];

    /// <summary>Current draft option from the Infrastructure category.</summary>
    public IStrategyCard? CurrentInfrastructure { get; private set; }

    /// <summary>Current draft option from the RiskControl category.</summary>
    public IStrategyCard? CurrentRiskControl { get; private set; }

    /// <summary>Current draft option from the FinTech category.</summary>
    public IStrategyCard? CurrentFinTech { get; private set; }

    /// <summary>
    /// Generate new draft options: one random card from each category.
    /// Players cannot select the same named card twice, but the public offer set
    /// can repeat across months so month three still has valid draft choices.
    /// </summary>
    public bool GenerateDraftOptions()
    {
        CurrentInfrastructure = PickRandom(InfrastructureFactory);
        CurrentRiskControl = PickRandom(RiskControlFactory);
        CurrentFinTech = PickRandom(FinTechFactory);

        return CurrentInfrastructure != null
            || CurrentRiskControl != null
            || CurrentFinTech != null;
    }

    /// <summary>
    /// Player selects a card by name from the current draft options.
    /// The card is acquired (OnAcquire called) and added to the player's ActiveCards.
    /// Both players receive independent card instances — each player gets their own copy.
    /// Returns the card if valid, null if the card name doesn't match any current option.
    /// </summary>
    public IStrategyCard? SelectCard(Player player, string cardName)
    {
        // Find the matching draft option
        IStrategyCard? template = null;
        if (CurrentInfrastructure?.Name == cardName) template = CurrentInfrastructure;
        else if (CurrentRiskControl?.Name == cardName) template = CurrentRiskControl;
        else if (CurrentFinTech?.Name == cardName) template = CurrentFinTech;

        if (template == null) return null;

        // Check the player doesn't already have a card with this exact name
        if (player.ActiveCards.Any(c => c.Name == cardName))
            return null;

        // Create a fresh instance for this player (so each player has independent state)
        IStrategyCard card = CreateFreshInstance(template);
        card.OnAcquire(player);
        player.ActiveCards.Add(card);
        return card;
    }

    /// <summary>
    /// Find an active card on a player by name, for activation during trading.
    /// </summary>
    public static IStrategyCard? FindActiveCard(Player player, string cardName)
    {
        return player.ActiveCards.FirstOrDefault(c => c.Name == cardName);
    }

    /// <summary>
    /// Get a list of the current draft option names (for sending to players).
    /// </summary>
    public List<string> GetCurrentDraftOptionNames()
    {
        var names = new List<string>(3);
        if (CurrentInfrastructure != null) names.Add(CurrentInfrastructure.Name);
        if (CurrentRiskControl != null) names.Add(CurrentRiskControl.Name);
        if (CurrentFinTech != null) names.Add(CurrentFinTech.Name);
        return names;
    }

    /// <summary>
    /// Reset all monthly card state for a player.
    /// Card ownership persists for the whole game; only month-scoped state resets.
    /// </summary>
    public static void ResetMonthlyCardState(Player player)
    {
        foreach (var card in player.ActiveCards)
        {
            switch (card)
            {
                case InsiderInfo insider:
                    insider.ResetMonthly();
                    break;
                case FlashTrading flash:
                    flash.ResetMonthly();
                    break;
                case StopLossBlade stopLoss:
                    stopLoss.ResetMonthly();
                    break;
                case TargetedPurchase targeted:
                    targeted.ResetMonthly();
                    break;
                case NetworkStorm storm:
                    storm.CurrentCooldown = 0;
                    break;
                case PublicOpinionAttack attack:
                    attack.CurrentCooldown = 0;
                    break;
            }
        }
    }

    /// <summary>
    /// Tick all active cards for a player (decrement cooldowns, check timed effects).
    /// </summary>
    public static void TickCards(Player player, int currentTick)
    {
        foreach (var card in player.ActiveCards)
        {
            card.OnTick(player, currentTick);
        }
    }

    /// <summary>
    /// Reset the manager entirely (for a new game).
    /// </summary>
    public void Reset()
    {
        CurrentInfrastructure = null;
        CurrentRiskControl = null;
        CurrentFinTech = null;
    }

    private IStrategyCard? PickRandom(List<Func<IStrategyCard>> factories)
    {
        if (factories.Count == 0) return null;
        return factories[_rng.Next(factories.Count)]();
    }

    /// <summary>
    /// Create a fresh instance of a card based on its type, so each player gets
    /// independent mutable state.
    /// </summary>
    private static IStrategyCard CreateFreshInstance(IStrategyCard template)
    {
        return template switch
        {
            InsiderInfo => new InsiderInfo(),
            FlashTrading => new FlashTrading(),
            StopLossBlade => new StopLossBlade(),
            TargetedPurchase => new TargetedPurchase(),
            NetworkStorm => new NetworkStorm(),
            PublicOpinionAttack => new PublicOpinionAttack(),
            _ => throw new InvalidOperationException($"Unknown card type: {template.GetType().Name}")
        };
    }
}
