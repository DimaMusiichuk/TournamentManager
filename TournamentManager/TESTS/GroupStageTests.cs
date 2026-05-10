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
}