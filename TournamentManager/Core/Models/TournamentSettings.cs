using TournemantManager.Core.Enums;

namespace TournemantManager.Core.Models;

public class TournamentSettings
{
    public int WinPoint { get; set; }
    public int LosePoint { get; set; }
    public int NumberOfGroups { get; set; }
    public TournamentType Format { get; set; }
    public int UpperBracketSlots  { get; set; }
    public int LowerBracketSlots  { get; set; }
    public int DrawPoint { get; set; }
    public bool HasLowerBracket { get; set; }
    public int AdvancingTeamsPerGroup { get; set; }
}