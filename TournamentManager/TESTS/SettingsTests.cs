using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using TournemantManager.Contracts;
using TournemantManager.Core.Logic;
using TournemantManager.Core.Models;

namespace TournemantManager.TESTS;

[TestFixture]
public class SettingsTests
{
    private TournamentSettings _settings;
    private GroupStage _groupStage;
    private List<IParticipant> _teams;

    [SetUp]
    public void Setup()
    {
        _settings = new TournamentSettings
        {
            WinPoint = 3,
            DrawPoint = 1,
            LosePoint = 0,
            NumberOfGroups = 1,
            MatchesPerOpponent = 1
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
    public void Settings_ExtremePoints_ShouldHandleLargeValues()
    {
        _settings.WinPoint = 1000000;
        
        _groupStage.GroupCreator(_teams, 1);
        var match = _groupStage.GroupMatches["Група A"][0];
        match.Scores.Add(new Score { FirstScore = 1, SecondScore = 0 });
        match.IsCompleted = true;

        var standings = _groupStage.CalculateStandings()["Група A"];
        var stats = standings.First(t => t.Key.Name == "NAVI").Value;

        Assert.That(stats.Points, Is.EqualTo(1000000), "Система має коректно працювати з великими числами");
    }

    [Test]
    public void GroupCreator_MoreGroupsThanTeams_ShouldHandleGracefully()
    {
        _settings.NumberOfGroups = 10;

        var matches = _groupStage.GroupCreator(_teams, _settings.NumberOfGroups);

        Assert.That(_groupStage.GroupMatches.Count, Is.GreaterThanOrEqualTo(4), "Кількість груп має відповідати кількості команд, якщо команд мало");
    }

    [Test]
    public void Settings_NegativePoints_ShouldThrowExceptionOrHandle()
    {
        _settings.LosePoint = -5;
        _groupStage.GroupCreator(_teams, 1);
        var match = _groupStage.GroupMatches["Група A"][0];
        match.Scores.Add(new Score { FirstScore = 0, SecondScore = 2 });
        match.IsCompleted = true;

        var stats = _groupStage.CalculateStandings()["Група A"].First(t => t.Key.Name == "NAVI").Value;

        Assert.That(stats.Points, Is.EqualTo(-5), "Система має підтримувати від'ємні очки, якщо це задано налаштуваннями");
    }
    
    [Test]
    public void Settings_DrawPointGreaterThanWinPoint_ShouldCalculateCorrectly()
    {
        _settings.WinPoint = 1;
        _settings.DrawPoint = 5; 
        
        _groupStage.GroupCreator(_teams, 1);
        var match = _groupStage.GroupMatches["Група A"][0];
        
        match.Scores.Add(new Score { FirstScore = 1, SecondScore = 1 });
        match.IsCompleted = true;

        var standings = _groupStage.CalculateStandings()["Група A"];
        var stats = standings.First(t => t.Key.Name == "NAVI").Value;

        Assert.That(stats.Points, Is.EqualTo(5), "Система має нараховувати очки строго за налаштуваннями, навіть якщо нічия дає більше очок, ніж перемога");
    }
    
    [Test]
    public void Settings_ChangePointsAfterMatchCompletion_ShouldAffectStandings()
    {
        _settings.WinPoint = 3;
        _groupStage.GroupCreator(_teams, 1);
        var match = _groupStage.GroupMatches["Група A"][0];
        
        match.Scores.Add(new Score { FirstScore = 2, SecondScore = 0 }); 
        match.IsCompleted = true;

        _settings.WinPoint = 5; 

        var standings = _groupStage.CalculateStandings()["Група A"];
        var winnerStats = standings.First(t => t.Key == match.FirstParticipant).Value;

        Assert.That(winnerStats.Points, Is.EqualTo(5), "Метод CalculateStandings має завжди використовувати найактуальніші налаштування на момент свого виклику");
    }
    
    [Test]
    public void Settings_DefaultInitialization_ShouldHaveExpectedValues()
    {
        var defaultSettings = new TournamentSettings();
        
        Assert.That(defaultSettings.WinPoint, Is.EqualTo(0), "Числа за замовчуванням мають бути 0");
        Assert.That(defaultSettings.HasLowerBracket, Is.False, "Логічні змінні за замовчуванням мають бути false");
        Assert.That(defaultSettings.TournamentName, Is.Null, "Рядки (string) за замовчуванням мають бути null");
    }
}