using NUnit.Framework;
using System.Collections.Generic;
using TournemantManager.Contracts;
using TournemantManager.Core.Logic;
using TournemantManager.Core.Models;

namespace TournemantManager.TESTS;

[TestFixture]
public class PlayOffTests
{
    private TournamentSettings _settings;
    private PlayOff _playOff;
    private Team _team1;
    private Team _team2;

    [SetUp]
    public void Setup()
    {
        _settings = new TournamentSettings { HasLowerBracket = false };
        _playOff = new PlayOff(_settings);

        _team1 = new Team { Id = 1, Name = "NAVI" };
        _team2 = new Team { Id = 2, Name = "Team Liquid" };
    }

    [Test]
    public void AdvanceToNextRound_WithoutLowerBracket_ShouldDeclareWinner()
    {
        var match = new Match { FirstParticipant = _team1, SecondParticipant = _team2 };
        match.Scores.Add(new Score { FirstScore = 2, SecondScore = 1 });
        match.IsCompleted = true;
        
        _playOff.UpperMatches.Add(match);
        _playOff.AdvanceToNextRound();

        Assert.That(_playOff.IsFinished, Is.True, "Турнір має завершитися");
        Assert.That(_playOff.TournamentWinner.Name, Is.EqualTo("NAVI"), "NAVI мають бути переможцями");
    }

    [Test]
    public void AdvanceToNextRound_WithLowerBracket_ShouldDropLosersToLowerBracket()
    {
        _settings.HasLowerBracket = true;
        
        var match1 = new Match { FirstParticipant = _team1, SecondParticipant = _team2 };
        match1.Scores.Add(new Score { FirstScore = 2, SecondScore = 1 });
        match1.IsCompleted = true;

        var team3 = new Team { Id = 3, Name = "Team Falcons" };
        var team4 = new Team { Id = 4, Name = "Tundra" };
        var match2 = new Match { FirstParticipant = team3, SecondParticipant = team4 };
        match2.Scores.Add(new Score { FirstScore = 2, SecondScore = 0 });
        match2.IsCompleted = true;
        
        _playOff.UpperMatches.Add(match1);
        _playOff.UpperMatches.Add(match2);

        _playOff.AdvanceToNextRound();

        Assert.That(_playOff.LowerMatches.Count, Is.EqualTo(1), "Дві команди, що програли, мають утворити 1 матч у Нижній сітці");
        Assert.That(_playOff.IsFinished, Is.False, "Турнір ще не має завершитися");
    }

    [Test]
    public void AdvanceToNextRound_UpperFinalAndLowerSemiFinal_ShouldCreateLowerFinal()
    {
        _settings.HasLowerBracket = true;
        var team3 = new Team { Id = 3, Name = "Team Falcons" };
        var team4 = new Team { Id = 4, Name = "Tundra" };

        var upperMatch = new Match { FirstParticipant = _team1, SecondParticipant = _team2 };
        upperMatch.Scores.Add(new Score { FirstScore = 2, SecondScore = 0 });
        upperMatch.IsCompleted = true;
        
        var lowerMatch = new Match { FirstParticipant = team3, SecondParticipant = team4 };
        lowerMatch.Scores.Add(new Score { FirstScore = 2, SecondScore = 1 });
        lowerMatch.IsCompleted = true;

        _playOff.UpperMatches.Add(upperMatch);
        _playOff.LowerMatches.Add(lowerMatch);

        _playOff.AdvanceToNextRound();

        Assert.That(_playOff.LowerMatches.Count, Is.EqualTo(1), "Має створитися Фінал Нижньої сітки");
        Assert.That(_playOff.IsFinished, Is.False, "Турнір ще не завершено");
    }

    [Test]
    public void AdvanceToNextRound_GrandFinalCompleted_ShouldDeclareAbsoluteWinner()
    {
        _settings.HasLowerBracket = true;
        
        var grandFinal = new Match { FirstParticipant = _team1, SecondParticipant = new Team { Id = 3, Name = "Team Falcons" } };
        grandFinal.Scores.Add(new Score { FirstScore = 3, SecondScore = 1 }); 
        grandFinal.IsCompleted = true;

        _playOff.UpperMatches.Add(grandFinal);
        _playOff.LowerMatches.Clear(); 

        _playOff.AdvanceToNextRound();

        Assert.That(_playOff.IsFinished, Is.True, "Після Гранд-Фіналу турнір має завершитися");
        Assert.That(_playOff.TournamentWinner, Is.Not.Null, "Переможець має бути визначений");
        Assert.That(_playOff.TournamentWinner.Name, Is.EqualTo("NAVI"), "NAVI виграли Гранд-Фінал і стали чемпіонами");
    }
    
    [Test]
    public void AdvanceToNextRound_WhenTournamentIsFinished_ShouldNotDoAnything()
    {
        var match = new Match { FirstParticipant = _team1, SecondParticipant = _team2 };
        match.Scores.Add(new Score { FirstScore = 2, SecondScore = 0 });
        match.IsCompleted = true;
        
        _playOff.UpperMatches.Add(match);
        _playOff.AdvanceToNextRound();

        var upperMatchesCountBefore = _playOff.UpperMatches.Count;

        _playOff.AdvanceToNextRound(); 

        Assert.That(_playOff.IsFinished, Is.True);
        Assert.That(_playOff.UpperMatches.Count, Is.EqualTo(upperMatchesCountBefore), "Кількість матчів не має змінюватися після завершення турніру");
    }
    
    [Test]
    public void StartPlayOff_WithEmptyStandings_ShouldNotCreateAnyMatches()
    {
        var emptyStandings = new Dictionary<string, Dictionary<IParticipant, ParticipantStats>>();

        _playOff.StartPlayOff(emptyStandings);

        Assert.That(_playOff.UpperMatches.Count, Is.EqualTo(0));
        Assert.That(_playOff.LowerMatches.Count, Is.EqualTo(0));
    }
    
    [Test]
    public void PlayOff_StartWithDifferentSizes_ShouldGenerateCorrectInitialMatches()
    {
        _settings.UpperBracketSlots = 4;
        _settings.LowerBracketSlots = 2;
        _settings.HasLowerBracket = true;

        var groupStandings = new Dictionary<IParticipant, ParticipantStats>();
        for (int i = 1; i <= 6; i++) 
            groupStandings.Add(new Team { Id = i, Name = $"T{i}" }, new ParticipantStats());

        var allStandings = new Dictionary<string, Dictionary<IParticipant, ParticipantStats>> 
        { 
            { "Група A", groupStandings } 
        };

        _playOff.StartPlayOff(allStandings);

        Assert.That(_playOff.UpperMatches.Count, Is.EqualTo(2), "4 команди у верхній сітці = 2 матчі");
        Assert.That(_playOff.LowerMatches.Count, Is.EqualTo(1), "2 команди у нижній сітці = 1 матч");
    }
    
    [Test]
    public void AreAllMatchesCompleted_WhenNotAllFinished_ShouldReturnFalse()
    {
        var match1 = new Match { IsCompleted = true };
        var match2 = new Match { IsCompleted = false };
        
        _playOff.UpperMatches.Add(match1);
        _playOff.UpperMatches.Add(match2);

        Assert.That(_playOff.AreAllMatchesCompleted(), Is.False, "Якщо хоч один матч не завершено, метод має повертати false");
    }

    [Test]
    public void AreAllMatchesCompleted_WhenAllFinished_ShouldReturnTrue()
    {
        _settings.HasLowerBracket = true;
        _playOff.UpperMatches.Add(new Match { IsCompleted = true });
        _playOff.LowerMatches.Add(new Match { IsCompleted = true });

        Assert.That(_playOff.AreAllMatchesCompleted(), Is.True, "Коли всі матчі завершені, метод має давати дозвіл на наступний раунд");
    }
    
    [Test]
    public void StartPlayOff_WithZeroUpperSlots_ShouldNotGenerateMatches()
    {
        _settings.UpperBracketSlots = 0;
        var groupStandings = new Dictionary<IParticipant, ParticipantStats>
        {
            { new Team { Id = 1, Name = "NaVi" }, new ParticipantStats() },
            { new Team { Id = 2, Name = "Team Falcons" }, new ParticipantStats() }
        };
        var allStandings = new Dictionary<string, Dictionary<IParticipant, ParticipantStats>> { { "Група A", groupStandings } };

        _playOff.StartPlayOff(allStandings);

        Assert.That(_playOff.UpperMatches.Count, Is.EqualTo(0), "Якщо слотів для виходу 0, сітка має бути порожньою");
    }

    [Test]
    public void AdvanceToNextRound_WhenWinnerAlreadySet_ShouldNotChangeWinner()
    {
        var match = new Match { FirstParticipant = _team1, SecondParticipant = _team2 };
        match.Scores.Add(new Score { FirstScore = 1, SecondScore = 0 });
        match.IsCompleted = true;
        _playOff.UpperMatches.Add(match);
        
        _playOff.AdvanceToNextRound();
        var initialWinner = _playOff.TournamentWinner;

        _playOff.AdvanceToNextRound();
        
        Assert.That(_playOff.TournamentWinner, Is.EqualTo(initialWinner), "Переможець не має перезаписуватися після завершення");
    }
}