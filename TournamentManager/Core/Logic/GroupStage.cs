using System;
using System.Collections.Generic;
using System.Linq;
using TournemantManager.Contracts;
using TournemantManager.Core.Models;

namespace TournemantManager.Core.Logic;

public class GroupStage
{
    private TournamentSettings _settings;

    public Dictionary<string, List<Match>> GroupMatches { get; set; } = new();

    public GroupStage(TournamentSettings settings)
    {
        _settings = settings;
    }

    public List<Match> GroupCreator(IList<IParticipant> participants, int numberOfGroups)
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
        
        List<Match> allMatchesToReturn = new();
        char groupLetter = 'A';

        foreach (var group in groups)
        {
            string groupName = $"Група {groupLetter}";
            GroupMatches.Add(groupName, new List<Match>());
            
            for (int i = 0; i < group.Count; i++)
            {
                for (int j = i + 1; j < group.Count; j++)
                {
                    GenerateMatchesBetweenOpponents(group[i], group[j], groupName, allMatchesToReturn);
                }
            }
            groupLetter++;
        }
        return allMatchesToReturn;
    }

    private void GenerateMatchesBetweenOpponents(IParticipant first, IParticipant second, string groupName, List<Match> allMatches)
    {
        for (int matchCount = 0; matchCount < _settings.MatchesPerOpponent; matchCount++)
        {
            var match = new Match();
            match.FirstParticipant = first;
            match.SecondParticipant = second;
            
            GroupMatches[groupName].Add(match);
            allMatches.Add(match);
        }
    }

    public Dictionary<string, Dictionary<IParticipant, ParticipantStats>> CalculateStandings()
    {
       var allStandings = new Dictionary<string, Dictionary<IParticipant, ParticipantStats>>();

        foreach (var groupPair in GroupMatches)
        {
            string groupName = groupPair.Key;
            List<Match> matchesInGroup = groupPair.Value;
            
            var groupStandings = new Dictionary<IParticipant, ParticipantStats>();

            foreach (Match match in matchesInGroup)
            {
                if (!groupStandings.ContainsKey(match.FirstParticipant))
                {
                    groupStandings.Add(match.FirstParticipant, new ParticipantStats());
                }

                if (!groupStandings.ContainsKey(match.SecondParticipant))
                {
                    groupStandings.Add(match.SecondParticipant, new ParticipantStats());
                }
            }

            foreach (Match match in matchesInGroup)
            {
                if (match.IsCompleted)
                {
                    UpdateStatsForCompletedMatch(match, groupStandings);
                }
            }

            var list = groupStandings.ToList();
            list.Sort((team1, team2) => team2.Value.Points.CompareTo(team1.Value.Points));
            
            var sortedStandings = new Dictionary<IParticipant, ParticipantStats>();
            foreach (var team in list)
            {
                sortedStandings.Add(team.Key, team.Value);
            }

            allStandings.Add(groupName, sortedStandings);
        }
        
        return allStandings;
    }

    private void UpdateStatsForCompletedMatch(Match match, Dictionary<IParticipant, ParticipantStats> groupStandings)
    {
        int firstTeamTotalScore = match.Scores.Sum(s => s.FirstScore);
        int secondTeamTotalScore = match.Scores.Sum(s => s.SecondScore);
        
        if (firstTeamTotalScore > secondTeamTotalScore)
        {
            groupStandings[match.FirstParticipant].Points += _settings.WinPoint;
            groupStandings[match.FirstParticipant].Wins++;
            groupStandings[match.SecondParticipant].Points += _settings.LosePoint;
            groupStandings[match.SecondParticipant].Losses++;
        }
        else if (secondTeamTotalScore > firstTeamTotalScore)
        {
            groupStandings[match.FirstParticipant].Points += _settings.LosePoint;
            groupStandings[match.FirstParticipant].Losses++;
            groupStandings[match.SecondParticipant].Points += _settings.WinPoint;
            groupStandings[match.SecondParticipant].Wins++;
        }
        else
        {
            groupStandings[match.FirstParticipant].Points += _settings.DrawPoint;
            groupStandings[match.FirstParticipant].Draws++;
            groupStandings[match.SecondParticipant].Points += _settings.DrawPoint;
            groupStandings[match.SecondParticipant].Draws++;
        }
    }
}