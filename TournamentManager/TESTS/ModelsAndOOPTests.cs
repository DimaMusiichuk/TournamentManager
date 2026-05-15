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
    
    [Test]
    public void Tournament_FullLifecycle_AddAndRemoveParticipants()
    {
        var tournament = new Tournament { Title = "KPI Open", Description = "CyberSport Tournament" };
        var team = new Team { Id = 1, Name = "NAVI" };

        tournament.AddParticipant(team);
        Assert.That(tournament.Participants.Count, Is.EqualTo(1));

        tournament.RemoveParticipant(team);
        Assert.That(tournament.Participants.Count, Is.EqualTo(0));
    }
    
    [Test]
    public void Match_InitialStatus_ShouldBePending()
    {
        var match = new Match();
        Assert.That(match.Status, Is.EqualTo(TournemantManager.Core.Enums.MatchStatus.Pending));
    }
    
    [Test]
    public void EntityBase_Equality_SameIdShouldBeEqual()
    {
        var team1 = new Team { Id = 10, Name = "Team A" };
        var team2 = new Team { Id = 10, Name = "Team B" };

        Assert.That(team1, Is.EqualTo(team2), "Об'єкти з однаковим ID мають вважатися рівними");
    }
    
    [Test]
    public void Team_AddPlayersAndSetCaptain_ShouldStoreCorrectly()
    {
        var team = new Team { Id = 1, Name = "NAVI" };
        var player1 = new Player { Id = 10, Name = "Artem", Nickname = "Niku" };
        var player2 = new Player { Id = 11, Name = "Tamir", Nickname = "daze" };

        team.Players.Add(player1);
        team.Players.Add(player2);
        team.Captain = player2;

        Assert.That(team.Players.Count, Is.EqualTo(2), "В команді має бути 2 гравці");
        Assert.That(team.Players.Contains(player1), Is.True, "Список має містити гравця Niku");
        Assert.That(team.Captain.Nickname, Is.EqualTo("daze"), "Капітаном має бути daze");
    }
    
    [Test]
    public void EntityBase_GetHashCode_ShouldReturnSameHashForSameId()
    {
        var player1 = new Player { Id = 99, Name = "SonicFox" };
        var player2 = new Player { Id = 99, Name = "Ninjakiller" };

        int hash1 = player1.GetHashCode();
        int hash2 = player2.GetHashCode();

        Assert.That(hash1, Is.EqualTo(hash2), "Хеш-коди різних об'єктів з однаковим ID мають збігатися для коректної роботи Dictionary");
    }
    
    [Test]
    public void Team_Properties_ShouldSetAndGetCorrectly()
    {
        var team = new Team { Name = "Liquid", Coach = "Blitz", TeamCountry = "Netherlands" };
        Assert.That(team.Coach, Is.EqualTo("Blitz"));
        Assert.That(team.TeamCountry, Is.EqualTo("Netherlands"));
    }

    [Test]
    public void Player_Properties_ShouldSetAndGetCorrectly()
    {
        var player = new Player { Age = 20, SecondName = "Mulyarchuk", Country = "Ukraine" };
        Assert.That(player.SecondName, Is.EqualTo("Mulyarchuk"));
        Assert.That(player.Country, Is.EqualTo("Ukraine"));
    }

    [Test]
    public void Score_DefaultValues_ShouldBeZero()
    {
        var score = new Score();
        Assert.That(score.FirstScore, Is.EqualTo(0), "За замовчуванням рахунок має бути 0");
        Assert.That(score.SecondScore, Is.EqualTo(0), "За замовчуванням рахунок має бути 0");
    }

    [Test]
    public void Tournament_Properties_ShouldSetAndGetCorrectly()
    {
        var tournament = new Tournament { Title = "The International", Description = "Dota2 World Championship" };
        Assert.That(tournament.Title, Is.EqualTo("The International"));
        Assert.That(tournament.Description, Is.EqualTo("Dota2 World Championship"));
        Assert.That(tournament.Matches, Is.Not.Null, "Список матчів має бути ініціалізований");
    }

    [Test]
    public void ParticipantStats_DefaultValues_ShouldBeZero()
    {
        var stats = new ParticipantStats();
        Assert.That(stats.Wins, Is.EqualTo(0));
        Assert.That(stats.Losses, Is.EqualTo(0));
        Assert.That(stats.Draws, Is.EqualTo(0));
        Assert.That(stats.Points, Is.EqualTo(0));
    }

    [Test]
    public void EntityBase_Equals_DifferentIds_ShouldReturnFalse()
    {
        var team1 = new Team { Id = 1, Name = "NAVI" };
        var team2 = new Team { Id = 2, Name = "NAVI" };
        Assert.That(team1, Is.Not.EqualTo(team2), "Об'єкти з різними ID не можуть бути рівними");
    }
    
    [Test]
    public void EntityBase_Equals_NullObject_ShouldReturnFalse()
    {
        var player = new Player { Id = 1, Name = "Illia" };
        Assert.That(player.Equals(null), Is.False, "Порівняння об'єкта з null має повертати false, а не викликати помилку");
    }

    [Test]
    public void EntityBase_Equals_DifferentType_ShouldReturnFalse()
    {
        var player = new Player { Id = 1, Name = "Illia" };
        var team = new Team { Id = 1, Name = "Ammar" };
        
        Assert.That(player.Equals(team), Is.False, "Об'єкти різних типів не можуть бути рівними, навіть якщо їх ID збігаються");
    }

    [Test]
    public void TournamentSettings_Properties_ShouldStoreCorrectly()
    {
        var settings = new TournamentSettings 
        { 
            Format = TournemantManager.Core.Enums.TournamentType.SingleElimination,
            Discipline = "Dota2",
            TeamSize = 5
        };
        
        Assert.That(settings.Discipline, Is.EqualTo("Dota2"));
        Assert.That(settings.TeamSize, Is.EqualTo(5));
    }
    
    [Test]
    public void Tournament_AddPlayersWithSameNicknameInDifferentTeams_ShouldDetectConflict()
    {
        var tournament = new Tournament();
    
        var team1 = new Team { Name = "NAVI" };
        team1.Players.Add(new Player { Id = 1, Nickname = "Niku" });
        tournament.AddParticipant(team1);

        var team2 = new Team { Name = "Team Falcons" };
        string attemptedNickname = "Niku";

        bool alreadyExists = false;

        foreach (var participant in tournament.Participants) 
        {
            if (participant is Team t) 
            {
                foreach (var p in t.Players) 
                {
                    if (p.Nickname.ToLower() == attemptedNickname.ToLower()) 
                    {
                        alreadyExists = true;
                        break;
                    }
                }
            }
            if (alreadyExists) break;
        }

        Assert.That(alreadyExists, Is.True);
    }
}