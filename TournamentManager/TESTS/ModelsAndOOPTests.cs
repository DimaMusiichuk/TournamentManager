using NUnit.Framework;
using System.Collections.Generic;
using TournemantManager.Contracts;
using TournemantManager.Core.Logic;
using TournemantManager.Core.Models;

namespace TournemantManager.TESTS;

[TestFixture]
public class ModelsAndOOPTests
{
    [Test]
    public void OOP_Polymorphism_ShouldRunTournamentWithPlayersInsteadOfTeams()
    {
        var settings = new TournamentSettings { NumberOfGroups = 1, MatchesPerOpponent = 1 };
        var groupStage = new GroupStage(settings);

        var players = new List<IParticipant>
        {
            new Player { Id = 1, Name = "SonicFox" },
            new Player { Id = 2, Name = "NinjaKiller" }
        };

        var matches = groupStage.GroupCreator(players, settings.NumberOfGroups);

        Assert.That(matches.Count, Is.EqualTo(1), "Турнір має успішно створитися для гравців (1x1)");
        Assert.That(matches[0].FirstParticipant.Name, Is.EqualTo("SonicFox"));
        Assert.That(matches[0].SecondParticipant.Name, Is.EqualTo("NinjaKiller"));
    }

    [Test]
    public void Match_MultipleScores_ShouldCalculateTotalCorrectly()
    {
        var match = new Match 
        { 
            FirstParticipant = new Team { Name = "NAVI" }, 
            SecondParticipant = new Team { Name = "Team Falcons" } 
        };

        match.Scores.Add(new Score { FirstScore = 1, SecondScore = 0 });
        match.Scores.Add(new Score { FirstScore = 0, SecondScore = 1 });
        match.Scores.Add(new Score { FirstScore = 1, SecondScore = 0 });
        match.IsCompleted = true;

        int naviTotal = 0;
        int fazeTotal = 0;
        foreach (var score in match.Scores)
        {
            naviTotal += score.FirstScore;
            fazeTotal += score.SecondScore;
        }

        Assert.That(naviTotal, Is.EqualTo(2), "NAVI виграли 2 карти");
        Assert.That(fazeTotal, Is.EqualTo(1), "FaZe виграли 1 карту");
        Assert.That(naviTotal > fazeTotal, Is.True, "NAVI є переможцями матчу");
    }
    
    [Test]
    public void Match_WhenCreated_DefaultValuesShouldBeCorrect()
    {
        var match = new Match();

        Assert.That(match.IsCompleted, Is.False, "Новий матч за замовчуванням не може бути завершеним");
        Assert.That(match.Scores, Is.Not.Null, "Список рахунків має бути ініціалізований");
        Assert.That(match.Scores.Count, Is.EqualTo(0), "У нового матчу ще не має бути жодних рахунків");
    }

    [Test]
    public void Player_Initialization_ShouldStorePropertiesCorrectly()
    {
        var player = new Player { Id = 777, Name = "Niku" };

        Assert.That(player.Id, Is.EqualTo(777));
        Assert.That(player.Name, Is.EqualTo("Niku"));
    }
}