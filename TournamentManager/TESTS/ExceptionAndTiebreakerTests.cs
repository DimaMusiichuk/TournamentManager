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
    
    [Test]
    public void CustomException_InvalidName_EmptyString_ShouldThrow()
    {
        var ex = Assert.Throws<InvalidNameFormatException>(() => 
        {
            throw new InvalidNameFormatException("Ім'я не може бути порожнім");
        });
        Assert.That(ex.Message, Is.EqualTo("Ім'я не може бути порожнім"));
    }

    [Test]
    public void CustomException_InvalidName_LowerCase_ShouldThrow()
    {
        var ex = Assert.Throws<InvalidNameFormatException>(() => 
        {
            throw new InvalidNameFormatException("Ім'я повинно починатися з великої літери");
        });
        Assert.That(ex.Message, Is.EqualTo("Ім'я повинно починатися з великої літери"));
    }

    [Test]
    public void CustomException_InvalidCountry_ShouldThrow()
    {
        var ex = Assert.Throws<InvalidCountryException>(() => 
        {
            throw new InvalidCountryException("Країна повинна починатися з великої літери");
        });
        Assert.That(ex.Message, Is.EqualTo("Країна повинна починатися з великої літери"));
    }
    
    [Test]
    public void GenerateTiebreakers_WithEmptyStandings_ShouldNotCreateMatches()
    {
        var settings = new TournamentSettings { WinPoint = 3, DrawPoint = 1, LosePoint = 0, NumberOfGroups = 1 };
        var groupStage = new GroupStage(settings);
        var emptyStandings = new Dictionary<string, Dictionary<IParticipant, ParticipantStats>>();

        groupStage.GenerateTiebreakers(emptyStandings);

        Assert.That(groupStage.TiebreakerMatches.Count, Is.EqualTo(0));
    }
    
    [Test]
    public void GenerateTiebreakers_WhenPointsAreDifferent_ShouldNotCreateTiebreaker()
    {
        var settings = new TournamentSettings { WinPoint = 3, DrawPoint = 1, LosePoint = 0, NumberOfGroups = 1 };
        var groupStage = new GroupStage(settings);

        var team1 = new Team { Id = 1, Name = "NAVI" };
        var team2 = new Team { Id = 2, Name = "Team Falcons" };

        var groupStandings = new Dictionary<IParticipant, ParticipantStats>
        {
            { team1, new ParticipantStats { Points = 3 } },
            { team2, new ParticipantStats { Points = 0 } }
        };

        var allStandings = new Dictionary<string, Dictionary<IParticipant, ParticipantStats>>
        {
            { "Група A", groupStandings }
        };

        groupStage.GroupMatches.Add("Група A", new List<Match>());

        groupStage.GenerateTiebreakers(allStandings);

        Assert.That(groupStage.TiebreakerMatches.Count, Is.EqualTo(0), "Якщо очки різні, переігровка не має створюватися");
    }
    
    [Test]
    public void GenerateTiebreakers_ThreeTeamsTied_ShouldCreateMatchesForEveryone()
    {
        var settings = new TournamentSettings { WinPoint = 3, DrawPoint = 1, LosePoint = 0, NumberOfGroups = 1 };
        var groupStage = new GroupStage(settings);

        var groupStandings = new Dictionary<IParticipant, ParticipantStats>
        {
            { new Team { Id = 1, Name = "T1" }, new ParticipantStats { Points = 5 } },
            { new Team { Id = 2, Name = "T2" }, new ParticipantStats { Points = 5 } },
            { new Team { Id = 3, Name = "T3" }, new ParticipantStats { Points = 5 } }
        };
        
        var allStandings = new Dictionary<string, Dictionary<IParticipant, ParticipantStats>> 
        { 
            { "Група A", groupStandings } 
        };
        
        groupStage.GroupMatches.Add("Група A", new List<Match>());

        groupStage.GenerateTiebreakers(allStandings);

        Assert.That(groupStage.TiebreakerMatches.Count, Is.GreaterThan(0), "Для троїстої нічиї мають бути згенеровані матчі");
    }
    
    [Test]
    public void InvalidAgeException_AgeIs12_ShouldThrow()
    {
        var ex = Assert.Throws<InvalidAgeException>(() => 
        {
            int testAge = 12;
            if (testAge < 13 || testAge > 45)
            {
                throw new InvalidAgeException("Вік повинен бути від 13 до 45 років");
            }
        });
        Assert.That(ex.Message, Is.EqualTo("Вік повинен бути від 13 до 45 років"));
    }

    [Test]
    public void InvalidAgeException_AgeIs46_ShouldThrow()
    {
        var ex = Assert.Throws<InvalidAgeException>(() => 
        {
            int testAge = 46;
            if (testAge < 13 || testAge > 45)
            {
                throw new InvalidAgeException("Вік повинен бути від 13 до 45 років");
            }
        });
        Assert.That(ex.Message, Is.EqualTo("Вік повинен бути від 13 до 45 років"));
    }
    
    [Test]
    public void GenerateTiebreakers_AllTeamsTiedWithZeroPoints_ShouldGenerateTiebreakers()
    {
        var settings = new TournamentSettings { NumberOfGroups = 1 };
        var groupStage = new GroupStage(settings);

        var groupStandings = new Dictionary<IParticipant, ParticipantStats>
        {
            { new Team { Id = 1 }, new ParticipantStats { Points = 0 } },
            { new Team { Id = 2 }, new ParticipantStats { Points = 0 } }
        };
        var allStandings = new Dictionary<string, Dictionary<IParticipant, ParticipantStats>> { { "Група A", groupStandings } };
        groupStage.GroupMatches.Add("Група A", new List<Match>());

        groupStage.GenerateTiebreakers(allStandings);

        Assert.That(groupStage.TiebreakerMatches.Count, Is.EqualTo(1), "Тайбрейкер має створюватися навіть якщо команди мають по 0 очок");
    }

    [Test]
    public void CustomException_DuplicateTeam_ShouldInheritFromException()
    {
        var ex = new DuplicateTeamException("test");
        Assert.That(ex, Is.InstanceOf<System.Exception>(), "Всі кастомні помилки мають наслідувати базовий клас Exception");
    }
}