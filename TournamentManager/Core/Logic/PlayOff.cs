using System.Collections.Generic;
using System.Linq;
using TournemantManager.Contracts;
using TournemantManager.Core.Models;

namespace TournemantManager.Core.Logic;

public class PlayOff
{
    private TournamentSettings _settings;

    public List<Match> UpperMatches { get; set; } = new();
    public List<Match> LowerMatches { get; set; } = new();
    public IParticipant TournamentWinner { get; private set; }
    public bool IsFinished { get; private set; } = false;

    public PlayOff(TournamentSettings settings)
    {
        _settings = settings;
    }

    public void StartPlayOff(Dictionary<string, Dictionary<IParticipant, ParticipantStats>> groupStandings)
    {
        List<IParticipant> upperBracketTeams = new();
        List<IParticipant> lowerBracketTeams = new();

        foreach (var group in groupStandings)
        {
            ExtractTeamsFromGroup(group.Value, upperBracketTeams, lowerBracketTeams);
        }

        GenerateMatches(upperBracketTeams, UpperMatches);

        if (_settings.HasLowerBracket)
        {
            GenerateMatches(lowerBracketTeams, LowerMatches);
        }
    }

    private void ExtractTeamsFromGroup(Dictionary<IParticipant, ParticipantStats> standings, List<IParticipant> upper, List<IParticipant> lower)
    {
        List<IParticipant> teamsInGroup = new List<IParticipant>(standings.Keys);

        for (int i = 0; i < teamsInGroup.Count; i++)
        {
            if (i < _settings.UpperBracketSlots)
            {
                upper.Add(teamsInGroup[i]);
            }
            else if (_settings.HasLowerBracket && i < (_settings.UpperBracketSlots + _settings.LowerBracketSlots))
            {
                lower.Add(teamsInGroup[i]);
            }
        }
    }

    public void GenerateMatches(IList<IParticipant> participants, List<Match> targetBracket)
    {
        targetBracket.Clear();
        for (int i = 0; i < participants.Count / 2; i++)
        {
            var match = new Match();
            match.FirstParticipant = participants[i];
            match.SecondParticipant = participants[participants.Count - 1 - i];
            
            targetBracket.Add(match);
        }
    }


    public bool AreMatchesCompleted(List<Match> bracket)
    {
        foreach (var match in bracket)
        {
            if (!match.IsCompleted)
            {
                return false;
            }
        }
        return true;
    }

    public bool AreAllMatchesCompleted()
    {
        if (!AreMatchesCompleted(UpperMatches))
        {
            return false;
        }
        
        if (_settings.HasLowerBracket && !AreMatchesCompleted(LowerMatches))
        {
            return false;
        }
        
        return true;
    }

    public void AdvanceToNextRound()
    {
        if (IsFinished)
        {
            return;
        }

        bool upperFinished = AdvanceBracket(UpperMatches, true);
        bool lowerFinished = true;

        if (_settings.HasLowerBracket)
        {
            lowerFinished = AdvanceBracket(LowerMatches, false);
        }

        if (upperFinished && lowerFinished)
        {
            IsFinished = true;
        }
    }

    private bool AdvanceBracket(List<Match> bracket, bool isUpper)
    {
        if (bracket.Count == 0)
        {
            return true; 
        }

        if (bracket.Count == 1)
        {
            var winner = GetMatchWinner(bracket[0]);
            if (isUpper)
            {
                TournamentWinner = winner;
            }
            bracket.Clear(); 
            return true; 
        }

        List<IParticipant> winners = new();
        foreach (var match in bracket)
        {
            winners.Add(GetMatchWinner(match));
        }

        GenerateNextRound(winners, bracket);
        return false; 
    }

    private void GenerateNextRound(List<IParticipant> winners, List<Match> targetBracket)
    {
        targetBracket.Clear();
        for (int i = 0; i < winners.Count; i += 2)
        {
            var match = new Match();
            match.FirstParticipant = winners[i];
            match.SecondParticipant = winners[i + 1];
            
            targetBracket.Add(match);
        }
    }

    private IParticipant GetMatchWinner(Match match)
    {
        int firstTeamTotal = match.Scores.Sum(s => s.FirstScore);
        int secondTeamTotal = match.Scores.Sum(s => s.SecondScore);
        
        if (firstTeamTotal > secondTeamTotal)
        {
            return match.FirstParticipant;
        }
        else
        {
            return match.SecondParticipant;
        }
    }
}