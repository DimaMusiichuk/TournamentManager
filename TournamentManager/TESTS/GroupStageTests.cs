using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using TournemantManager.Contracts;
using TournemantManager.Core.Logic;
using TournemantManager.Core.Models;

namespace TournemantManager.TESTS;

[TestFixture]
public class GroupStageTests
{
    private TournamentSettings _settings;
    private GroupStage _groupStage;
    private List<IParticipant> _teams;

    [SetUp]
    public void Setup()
    {
        _settings = new TournamentSettings
        {
            MatchesPerOpponent = 1,
            WinPoint = 2,
            DrawPoint = 1,
            LosePoint = 0,
            NumberOfGroups = 1
        };

        _groupStage = new GroupStage(_settings);

        _teams = new List<IParticipant>
        {
            new Team { Id = 1, Name = "NAVI" },
            new Team { Id = 2, Name = "Team Liquid" },
            new Team { Id = 3, Name = "Team Falcons" },
            new Team { Id = 4, Name = "Tundra Esports" }
        };
    }

    [Test]
    public void GroupCreator_ShouldGenerateCorrectNumberOfMatches()
    {
        var matches = _groupStage.GroupCreator(_teams, _settings.NumberOfGroups);

        Assert.That(matches.Count, Is.EqualTo(6), "Для 4 команд має бути згенеровано 6 матчів");
        Assert.That(_groupStage.GroupMatches.ContainsKey("Група A"), Is.True, "Група A має бути створена");
    }

    [Test]
    public void CalculateStandings_ShouldCalculatePointsCorrectly()
    {
        _groupStage.GroupCreator(_teams, _settings.NumberOfGroups);
        var matches = _groupStage.GroupMatches["Група A"];

        var match = matches.First(m => m.FirstParticipant.Name == "NAVI" && m.SecondParticipant.Name == "Team Liquid");
        match.Scores.Add(new Score { FirstScore = 2, SecondScore = 0 });
        match.IsCompleted = true;

        var standings = _groupStage.CalculateStandings()["Група A"];
        var naviStats = standings.First(t => t.Key.Name == "NAVI").Value;
        var vitalityStats = standings.First(t => t.Key.Name == "Team Liquid").Value;

        Assert.That(naviStats.Points, Is.EqualTo(2), "Переможець має отримати 2 очки");
        Assert.That(vitalityStats.Points, Is.EqualTo(0), "Той, хто програв, отримує 0 очок");
    }
    
    [Test]
    public void GroupCreator_EmptyParticipantsList_ShouldReturnEmptyMatchList()
    {
        var emptyTeams = new List<IParticipant>();
        var matches = _groupStage.GroupCreator(emptyTeams, _settings.NumberOfGroups);
        
        Assert.That(matches.Count, Is.EqualTo(0), "Для порожнього списку команд має повертатися 0 матчів");
        Assert.That(_groupStage.GroupMatches["Група A"].Count, Is.EqualTo(0));
    }

    [Test]
    public void CalculateStandings_AllDraws_ShouldCalculateCorrectly()
    {
        _groupStage.GroupCreator(_teams, _settings.NumberOfGroups);
        var matches = _groupStage.GroupMatches["Група A"];

        foreach (var match in matches)
        {
            match.Scores.Add(new Score { FirstScore = 1, SecondScore = 1 });
            match.IsCompleted = true;
        }

        var standings = _groupStage.CalculateStandings()["Група A"];
        
        foreach (var stat in standings.Values)
        {
            Assert.That(stat.Points, Is.EqualTo(3), "За 3 нічиї має бути 3 очки");
            Assert.That(stat.Draws, Is.EqualTo(3), "Кількість нічиїх має бути 3");
            Assert.That(stat.Wins, Is.EqualTo(0), "Кількість перемог має бути 0");
            Assert.That(stat.Losses, Is.EqualTo(0), "Кількість поразок має бути 0");
        }
    }

    [Test]
    public void GroupCreator_MultipleGroups_ShouldDistributeTeamsCorrectly()
    {
        _settings.NumberOfGroups = 2;
        
        var matches = _groupStage.GroupCreator(_teams, _settings.NumberOfGroups);

        Assert.That(matches.Count, Is.EqualTo(2), "Має бути 2 матчі загалом (по 1 в кожній групі)");
        Assert.That(_groupStage.GroupMatches.ContainsKey("Група A"), Is.True, "Має створитися Група A");
        Assert.That(_groupStage.GroupMatches.ContainsKey("Група B"), Is.True, "Має створитися Група B");
        Assert.That(_groupStage.GroupMatches["Група A"].Count, Is.EqualTo(1));
        Assert.That(_groupStage.GroupMatches["Група B"].Count, Is.EqualTo(1));
    }
    
    [Test]
    public void GroupCreator_ZeroGroups_ShouldThrowDivideByZeroException()
    {
        _settings.NumberOfGroups = 0;

        Assert.Throws<System.DivideByZeroException>(() => _groupStage.GroupCreator(_teams, _settings.NumberOfGroups));
    }
    
    [Test]
    public void CalculateStandings_BeforeGroupCreator_ShouldReturnEmptyDictionary()
    {

        var standings = _groupStage.CalculateStandings();

        Assert.That(standings.Count, Is.EqualTo(0));
    }
}