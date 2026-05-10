using NUnit.Framework;
using System.Collections.Generic;
using TournemantManager.Core.Logic;
using TournemantManager.Core.Models;
using TournemantManager.Core.Exceptions;
using TournemantManager.Contracts;

namespace TournemantManager.TESTS;

[TestFixture]
public class ExceptionAndTiebreakerTests
{
    [Test]
    public void GenerateTiebreakers_WhenPointsAreEqual_ShouldCreateTiebreakerMatch()
    {
        var settings = new TournamentSettings { WinPoint = 3, DrawPoint = 1, LosePoint = 0, NumberOfGroups = 1 };
        var groupStage = new GroupStage(settings);

        var team1 = new Team { Id = 1, Name = "NAVI" };
        var team2 = new Team { Id = 2, Name = "Team Falcons" };

        var stats1 = new ParticipantStats { Points = 3 };
        var stats2 = new ParticipantStats { Points = 3 };

        var groupStandings = new Dictionary<IParticipant, ParticipantStats>
        {
            { team1, stats1 },
            { team2, stats2 }
        };

        var allStandings = new Dictionary<string, Dictionary<IParticipant, ParticipantStats>>
        {
            { "Група A", groupStandings }
        };

        groupStage.GroupMatches.Add("Група A", new List<Match>());
        groupStage.GenerateTiebreakers(allStandings);

        Assert.That(groupStage.TiebreakerMatches.Count, Is.EqualTo(1), "Має створитися 1 додатковий матч-переігровка");
        Assert.That(groupStage.TiebreakerMatches[0].FirstParticipant.Name, Is.EqualTo("NAVI"), "Першим учасником має бути NAVI");
    }

    [Test]
    public void CustomException_InvalidAge_ShouldStoreMessageCorrectly()
    {
        var ex = Assert.Throws<InvalidAgeException>(() => 
        {
            throw new InvalidAgeException("Вік повинен бути від 13 до 45 років");
        });

        Assert.That(ex.Message, Is.EqualTo("Вік повинен бути від 13 до 45 років"), "Повідомлення про помилку має зберігатися правильно");
    }

    [Test]
    public void CustomException_DuplicateTeam_ShouldThrowCorrectly()
    {
        var ex = Assert.Throws<DuplicateTeamException>(() => 
        {
            throw new DuplicateTeamException("Команда вже існує");
        });

        Assert.That(ex.Message, Is.EqualTo("Команда вже існує"));
    }
}