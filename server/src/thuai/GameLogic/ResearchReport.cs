namespace Thuai.GameLogic;

public class ResearchReport
{
    public string PlayerToken { get; init; } = "";
    public int NewsId { get; init; }
    public Prediction Prediction { get; init; }
    public int SubmitTick { get; init; }
    public int SubmissionRank { get; set; }
    public int SubmitDay { get; init; }
    public int SettlementDay { get; init; }
    public long ActualChange { get; set; }

    public bool? IsCorrect { get; set; }
    public long Reward { get; set; }
}
