using TournemantManager.Contracts;
using TournemantManager.Core.Models;

namespace TournemantManager.Core.Logic;

public class PlayOff
{
    private TournamentSettings _settings;

    public List<IParticipant> UpperBracketTeams { get; set; } = new();
    public List<IParticipant> LowerBracketTeams { get; set; } = new();
    public List<Match> UpperMatches { get; set; } = new();
    public List<Match> LowerMatches { get; set; } = new();

    public PlayOff(TournamentSettings settings)
    {
        _settings = settings;
    }

    public void StartPlayOff(Dictionary<IParticipant, ParticipantStats> groupStandings)
    {
        ProcessGroupResults(groupStandings);
        
        UpperMatches = GenerateMatches(UpperBracketTeams);
        
        if (_settings.HasLowerBracket)
        {
            LowerMatches = GenerateMatches(LowerBracketTeams);
        }
    }

    public void ProcessGroupResults(Dictionary<IParticipant, ParticipantStats> groupStandings)
    {
        var teams = groupStandings.Keys.ToList();
        
        for (int i = 0; i < _settings.UpperBracketSlots + _settings.LowerBracketSlots; i++)
        {
            if (i < _settings.UpperBracketSlots)
            {
                UpperBracketTeams.Add(teams[i]);
            }
            else
            {
                LowerBracketTeams.Add(teams[i]);
            }
        }
    }

    public List<Match> GenerateMatches(IList<IParticipant> participants)
    {
        var roundMatches = new List<Match>();
        
        for (int i = 0; i < participants.Count / 2; i++)
        {
            var match = new Match();
            match.FirstParticipant = participants[i];
            match.SecondParticipant = participants[participants.Count - 1 - i];
            
            roundMatches.Add(match);
        }
        
        return roundMatches;
    }
}