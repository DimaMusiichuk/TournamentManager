using System;
using System.Collections.Generic;
using TournemantManager.Contracts;
using TournemantManager.Core.Enums;
using TournemantManager.Core.Logic;
using TournemantManager.Core.Models;

namespace TournemantManager.UI;

public class MenuRunner
{
    private Tournament _tournament = new Tournament();
    private TournamentSettings _settings;
    private GroupStage _groupStage;
    private PlayOff _playOff;

    public void Run()
    {
        while (true)
        {
            Console.WriteLine("\n=== МЕНЮ ===");
            Console.WriteLine("1 - Налаштувати формат турніру");
            Console.WriteLine("2 - Додати команду");
            Console.WriteLine("3 - Згенерувати групи");
            Console.WriteLine("4 - Оновити рахунок групового етапу");
            Console.WriteLine("5 - Переглянути таблицю груп");
            Console.WriteLine("6 - Запустити Плей-оф");
            Console.WriteLine("7 - Переглянути сітку Плей-оф");
            Console.WriteLine("8 - Переглянути список усіх команд та гравців");
            Console.WriteLine("0 - Вихід");
            Console.Write("Оберіть дію: ");

            try
            {
                int choice = int.Parse(Console.ReadLine()!);

                switch (choice)
                {
                    case 1:
                    {
                        ConfigureTournament();
                        break;
                    }
                    case 2:
                    {
                        AddTeam();
                        break;
                    }
                    case 3:
                    {
                        GenerateGroups();
                        break;
                    }
                    case 4:
                    {
                        UpdateGroupScores();
                        break;
                    }
                    case 5:
                    {
                        ShowGroupStandings();
                        break;
                    }
                    case 6:
                    {
                        StartPlayOff();
                        break;
                    }
                    case 7:
                    {
                        ShowPlayOff();
                        break;
                    }
                    case 8:
                    {
                        ShowAllTeams();
                        break;
                    }
                    case 0:
                    {
                        Console.WriteLine("Вихід з програми");
                        return;
                    }
                    default:
                    {
                        Console.WriteLine("Невідома команда, спробуйте ще раз");
                        break;
                    }
                }
            }
            catch (FormatException)
            {
                Console.WriteLine("Помилка вводу! Будь ласка, введіть числове значення");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Сталася помилка: {ex.Message}");
            }
        }
    }

    private void ConfigureTournament()
    {
        Console.WriteLine("\n--- Налаштування турніру ---");
        _settings = new TournamentSettings();

        Console.Write("Введіть кількість команд для турніру (наприклад: 8, 12, 16): ");
        _settings.TotalTeams = int.Parse(Console.ReadLine()!);
        
        Console.Write("Скільки гравців має бути в одній команді? (наприклад: 5): ");
        _settings.TeamSize = int.Parse(Console.ReadLine()!);
        
        Console.Write("Формат матчів у групі (1 - Bo1, 2 - Bo2, 3 - Bo3): ");
        _settings.MatchesPerOpponent = int.Parse(Console.ReadLine()!);

        Console.Write("Введіть кількість очок за перемогу (рекомендовано 2): ");
        _settings.WinPoint = int.Parse(Console.ReadLine()!);

        Console.Write("Введіть кількість очок за нічию (рекомендовано 1): ");
        _settings.DrawPoint = int.Parse(Console.ReadLine()!);

        Console.Write("Введіть кількість очок за поразку (рекомендовано 0): ");
        _settings.LosePoint = int.Parse(Console.ReadLine()!);

        Console.Write("Введіть кількість груп (наприклад: 1, 2, 4): ");
        _settings.NumberOfGroups = int.Parse(Console.ReadLine()!);
        
        Console.Write("Чи буде турнір мати нижню сітку? (1 - Так, 0 - Ні): ");
        _settings.HasLowerBracket = (Console.ReadLine() == "1");

        Console.Write("Скільки команд виходять у верхню сітку Плей-оф?: ");
        _settings.UpperBracketSlots = int.Parse(Console.ReadLine()!);

        if (_settings.HasLowerBracket)
        {
            Console.Write("Скільки команд виходять у НИЖНЮ сітку Плей-оф?: ");
            _settings.LowerBracketSlots = int.Parse(Console.ReadLine()!);
        }

        Console.WriteLine("Налаштування збережено");
    }

    private void AddTeam()
    {
        
        if (_settings == null)
        {
            throw new InvalidOperationException("Спочатку налаштуйте турнір");
        }
        
        Console.WriteLine("\n--- Додавання команд та гравців ---");
        Console.WriteLine("Введіть '0', щоб повернутися в головне меню");

        while (true)
        {
            if (_tournament.Participants.Count >= _settings.TotalTeams)
            {
                Console.WriteLine("\nДосягнуто максимальну кількість команд для цього турніру!");
                break;
            }
            
            Console.Write("\nВведіть назву команди: ");
            string teamName = Console.ReadLine()!;

            if (teamName == "0") 
            {
                break;
            }

            var newTeam = new Team 
            { 
                Id = _tournament.Participants.Count + 1, 
                Name = teamName 
            };

            Console.WriteLine($"Команду '{teamName}' створено, потрібно додати {_settings.TeamSize} гравців");
            
            for (int i = 0; i < _settings.TeamSize; i++)
            {
                Console.WriteLine($"\nГравець {i + 1} з {_settings.TeamSize}:");
                
                Console.Write("Нікнейм: ");
                string nickname = Console.ReadLine()!;
                
                Console.Write("Ім'я: ");
                string firstName = Console.ReadLine()!;

                Console.Write("Прізвище: ");
                string secondName = Console.ReadLine()!;

                Console.Write("Вік: ");
                int age = int.Parse(Console.ReadLine()!);

                var player = new Player
                {
                    Id = (newTeam.Id * 100) + (i + 1), 
                    Nickname = nickname,
                    Name = firstName,
                    SecondName = secondName,
                    Age = age,
                    Team = teamName
                };

                newTeam.Players.Add(player);
            }
            
            Console.Write("\nЧи є у команди тренер? (1 - Так, 0 - Ні): ");
            if (Console.ReadLine() == "1")
            {
                Console.Write("Введіть нікнейм тренера: ");
                newTeam.Coach = Console.ReadLine()!;
            }
            
            Console.WriteLine("\nХто з гравців є капітаном?");
            for (int i = 0; i < newTeam.Players.Count; i++)
            {
                Console.WriteLine($"{i + 1} - {newTeam.Players[i].Nickname}");
            }
            Console.Write("Оберіть номер капітана: ");
            int captIndex = int.Parse(Console.ReadLine()!) - 1;
            newTeam.Captain = newTeam.Players[captIndex];

            _tournament.AddParticipant(newTeam);
            Console.WriteLine($"\nКоманду '{teamName}' збережено, гравців у складі: {newTeam.Players.Count}");
            Console.WriteLine($"Всього команд у турнірі: {_tournament.Participants.Count}");
        }
    }
    
    private void ShowAllTeams()
    {
        if (_tournament.Participants.Count == 0)
        {
            Console.WriteLine("У турнірі ще немає команд");
            return;
        }

        Console.WriteLine("\nСПИСОК КОМАНД УЧАСНИКІВ");
        foreach (Team team in _tournament.Participants)
        {
            Console.WriteLine($"\nКоманда: {team.Name} (ID: {team.Id})");
            Console.WriteLine($"Капітан: {team.Captain.Nickname}");
            if (!string.IsNullOrEmpty(team.Coach))
            {
                Console.WriteLine($"Тренер: {team.Coach}");
            }
            Console.WriteLine("Склад:");
            foreach (var player in team.Players)
            {
                Console.WriteLine($"- [{player.Nickname}] {player.Name} {player.SecondName}, {player.Age} років");
            }
        }
    }

    private void GenerateGroups()
    {
        if (_settings == null || _tournament.Participants.Count == 0)
        {
            Console.WriteLine("Спочатку налаштуйте турнір і додайте команди");
            return;
        }

        Console.WriteLine("\n--- Жеребкування груп ---");
        _groupStage = new GroupStage(_settings);
        
        _tournament.Matches = _groupStage.GroupCreator(_tournament.Participants, _settings.NumberOfGroups);
        
        Console.WriteLine($"Згенеровано {_tournament.Matches.Count} матчів групового етапу.");
    }

    private void UpdateGroupScores()
    {
        if (_groupStage == null || _groupStage.GroupMatches.Count == 0)
        {
            Console.WriteLine("Спочатку згенеруйте групи");
            return;
        }

        Console.WriteLine("\nЯк хочете ввести результати?");
        Console.WriteLine("1 - Вручну");
        Console.WriteLine("2 - Автоматично (випадкові рахунки)");
        Console.Write("Вибір: ");
        string choice = Console.ReadLine()!;

        foreach (var groupPair in _groupStage.GroupMatches)
        {
            if (choice == "1")
            {
                Console.WriteLine($"\n--- Оновлення результатів: {groupPair.Key} ---");
            }

            ProcessGroupMatches(groupPair.Value, choice, _settings.MatchesPerOpponent);
        }
        
        Console.WriteLine("Всі матчі групового етапу оновлено");
    }

    private void ProcessGroupMatches(List<Match> groupMatchesList, string choice, int format)
    {
        Random rnd = new Random();
        foreach (var match in groupMatchesList)
        {
            if (match.IsCompleted) 
            {
                continue; 
            }

            var score = new Score();
            
            if (choice == "1")
            {
                EnterManualScore(match, score, format);
            }
            else
            {
                GenerateAutoScore(match, score, rnd, format);
            }

            match.Scores.Add(score);
            match.Status = MatchStatus.Finished;
            match.IsCompleted = true;
        }
    }

    private void EnterManualScore(Match match, Score score, int format)
    {
        while (true)
        {
            Console.WriteLine($"\nМатч: {match.FirstParticipant.Name} VS {match.SecondParticipant.Name}");
            Console.Write($"Рахунок команди {match.FirstParticipant.Name}: ");
            score.FirstScore = int.Parse(Console.ReadLine()!);
            Console.Write($"Рахунок команди {match.SecondParticipant.Name}: ");
            score.SecondScore = int.Parse(Console.ReadLine()!);

            if (format == 2)
            {
                break;
            }

            if (score.FirstScore != score.SecondScore)
            {
                break; 
            }
            
            Console.WriteLine("Помилка, в форматі матчів BO1 та BO3 нічия не можлива! Спробуйте ще раз.");
        }
    }

    private void GenerateAutoScore(Match match, Score score, Random rnd, int format)
    {
        score.FirstScore = rnd.Next(0, 6);
        score.SecondScore = rnd.Next(0, 6);
        
        if (format != 2)
        {
            while (score.FirstScore == score.SecondScore)
            {
                score.SecondScore = rnd.Next(0, 6);
            }
        }
    }

    private void ShowGroupStandings()
    {
        if (_groupStage == null)
        {
            Console.WriteLine("Спочатку згенеруйте групи");
            return;
        }

        Console.WriteLine("\n--- Турнірна таблиця ---");
        var standings = _groupStage.CalculateStandings(); 
        ConsolePrinter.PrintGroupStage(standings);
    }

    private void StartPlayOff()
    {
        if (_groupStage == null)
        {
            Console.WriteLine("Спочатку потрібно зіграти груповий етап");
            return;
        }

        Console.WriteLine("\n--- Формування сітки Плей-оф ---");
        var standings = _groupStage.CalculateStandings(); 
        
        Console.WriteLine("Логіка генерації Плей-оф тимчасово вимкнена. Переходимо до Етапу 3!");
    }

    private void ShowPlayOff()
    {
        if (_playOff == null)
        {
            Console.WriteLine("Спочатку запустіть Плей-оф");
            return;
        }

        Console.WriteLine("\n--- Матчі Верхньої сітки Плей-оф ---");
        ConsolePrinter.PrintPlayOff(_playOff.UpperMatches);
    }
}