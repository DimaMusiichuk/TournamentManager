using TournemantManager.Contracts;
using TournemantManager.Core.Models;

namespace TournemantManager.Core.Logic;

public class GroupStage
{
    public List<Match> Matches { get; set; } = new();
    private TournamentSettings _settings;

    public List<Match> GruopCreator(IList<IParticipant>  participants, int numberOfGroups)
    {
        List<List<IParticipant>> groups = new();
        for (int i = 0; i < numberOfGroups; i++)
        {
            groups.Add(new List<IParticipant>());
        }
        
        for (int i = 0; i < participants.Count; i++)
        {
            int groupIndex = i % numberOfGroups;
            groups[groupIndex].Add(participants[i]);
        }

        foreach (var group in groups)
        {
            for (int i = 0; i < group.Count; i++)
            {
                for (int j = i + 1; j < group.Count; j++)
                {
                    var match = new Match();
                    match.FirstParticipant = group[i];
                    match.SecondParticipant = group[j];
                    Matches.Add(match);
                }   
            }
        }
        return Matches;
    }

    public GroupStage(TournamentSettings settings)
    {
        _settings = settings;
    }

    public Dictionary<IParticipant, ParticipantStats> CalculateStandings(IList<IParticipant> participants)
    {
        var standings = new Dictionary<IParticipant, ParticipantStats>();
        foreach (IParticipant p in participants)
        {
            standings.Add(p, new ParticipantStats());
        }

        foreach (Match match in Matches)
        {
            if (match.IsCompleted == true)
            {
                var score = match.Scores[0];
                if (score.FirstScore > score.SecondScore)
                {
                    standings[match.FirstParticipant].Points += _settings.WinPoint;
                    standings[match.FirstParticipant].Wins++;
                    standings[match.SecondParticipant].Points += _settings.LosePoint;
                    standings[match.SecondParticipant].Losses++;
                }
                else if (score.SecondScore > score.FirstScore)
                {
                    standings[match.FirstParticipant].Points += _settings.LosePoint;
                    standings[match.FirstParticipant].Losses++;
                    standings[match.SecondParticipant].Points += _settings.WinPoint;
                    standings[match.SecondParticipant].Wins++;
                }
                else
                {
                    standings[match.FirstParticipant].Points += _settings.DrawPoint;
                    standings[match.FirstParticipant].Draws++;
                    standings[match.SecondParticipant].Points += _settings.DrawPoint;
                    standings[match.SecondParticipant].Draws++;
                }
            }
        }
        return standings;
    }
}