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

        if (CheckIfTournamentFinished())
        {
            return;
        }
        
        List<IParticipant> upperWinners = new();
        List<IParticipant> upperLosers = new();
        
        ProcessUpperBracket(upperWinners, upperLosers);
        ProcessLowerBracket(upperWinners, upperLosers);
    }
    
    private bool CheckIfTournamentFinished()
    {
        if (UpperMatches.Count == 1 && (!_settings.HasLowerBracket || LowerMatches.Count == 0))
        {
            TournamentWinner = GetMatchWinner(UpperMatches[0]);
            IsFinished = true;
            UpperMatches.Clear();
            return true;
        }
        return false;
    }
    
    private void ProcessUpperBracket(List<IParticipant> upperWinners, List<IParticipant> upperLosers)
    {
        foreach (var match in UpperMatches)
        {
            upperWinners.Add(GetMatchWinner(match));
            if (_settings.HasLowerBracket)
            {
                upperLosers.Add(GetMatchLoser(match));
            }
        }

        if (upperWinners.Count > 1)
        {
            GenerateNextRound(upperWinners, UpperMatches);
        }
        else if (upperWinners.Count == 1 && !_settings.HasLowerBracket)
        {
            TournamentWinner = upperWinners[0];
            IsFinished = true;
            UpperMatches.Clear();
        }
    }
    
    private void ProcessLowerBracket(List<IParticipant> upperWinners, List<IParticipant> upperLosers)
    {
        if (!_settings.HasLowerBracket)
        {
            return;
        }

        List<IParticipant> nextLowerParticipants = new();
        
        foreach (var match in LowerMatches)
        {
            nextLowerParticipants.Add(GetMatchWinner(match));
        }
        
        nextLowerParticipants.AddRange(upperLosers);

        if (upperWinners.Count == 1 && nextLowerParticipants.Count == 1)
        {
            UpperMatches.Clear();
            LowerMatches.Clear();
            
            var grandFinal = new Match();
            grandFinal.FirstParticipant = upperWinners[0];
            grandFinal.SecondParticipant = nextLowerParticipants[0];
            
            UpperMatches.Add(grandFinal);
        }
        else if (nextLowerParticipants.Count > 1)
        {
            GenerateNextRound(nextLowerParticipants, LowerMatches);
        }
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
    
    private int GetTeamScore(Match match, bool isFirstTeam)
    {
        if (isFirstTeam)
        {
            return match.Scores.Sum(s => s.FirstScore);
        }
        else
        {
            return match.Scores.Sum(s => s.SecondScore);
        }
    }
    
    private IParticipant GetMatchResult(Match match, bool isLookingForWinner)
    {
        int firstScore = GetTeamScore(match, true);
        int secondScore = GetTeamScore(match, false);

        if (firstScore > secondScore)
        {
            if (isLookingForWinner)
            {
                return match.FirstParticipant; 
            }
            else
            {
                return match.SecondParticipant; 
            }
        }
        else
        {
            if (isLookingForWinner)
            {
                return match.SecondParticipant;
            }
            else
            {
                return match.FirstParticipant;
            }
        }
    }

    private IParticipant GetMatchWinner(Match match)
    {
        return GetMatchResult(match, true);
    }
    
    private IParticipant GetMatchLoser(Match match)
    {
        return GetMatchResult(match, false);
    }
}