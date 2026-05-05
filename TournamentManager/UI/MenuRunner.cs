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

        Console.Write("Введіть кількість очок за перемогу (рекомендовано 2): ");
        _settings.WinPoint = int.Parse(Console.ReadLine()!);

        Console.Write("Введіть кількість очок за нічию (рекомендовано 1): ");
        _settings.DrawPoint = int.Parse(Console.ReadLine()!);

        Console.Write("Введіть кількість очок за поразку (рекомендовано 0): ");
        _settings.LosePoint = int.Parse(Console.ReadLine()!);

        Console.Write("Введіть кількість груп (наприклад: 1, 2, 4): ");
        _settings.NumberOfGroups = int.Parse(Console.ReadLine()!);

        Console.Write("Скільки команд виходять у верхню сітку Плей-оф?: ");
        _settings.UpperBracketSlots = int.Parse(Console.ReadLine()!);

        Console.Write("Чи буде турнір мати нижню сітку? (1 - Так, 0 - Ні): ");
        string hasLower = Console.ReadLine()!;
        _settings.HasLowerBracket = (hasLower == "1");

        if (_settings.HasLowerBracket)
        {
            Console.Write("Скільки команд виходять у НИЖНЮ сітку Плей-оф?: ");
            _settings.LowerBracketSlots = int.Parse(Console.ReadLine()!);
        }

        Console.WriteLine("Налаштування збережено");
    }

    private void AddTeam()
    {
        Console.WriteLine("\n--- Додавання команд та гравців ---");
        Console.WriteLine("Введіть '0', щоб повернутися в головне меню");

        while (true)
        {
            Console.Write("\nВведіть назву команди (або '0' для виходу): ");
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

            Console.WriteLine($"Команду '{teamName}' створено, додамо гравців");
            Console.WriteLine("Введіть '0', щоб завершити склад цієї команди");

            int playerIdCounter = 1;
            while (true)
            {
                Console.Write($"\nВведіть ім'я {playerIdCounter}-го гравця (або '0' щоб вийти): ");
                string playerName = Console.ReadLine()!;
                
                if (playerName == "0") 
                {
                    break;
                }

                Console.Write("Введіть прізвище гравця: ");
                string secondName = Console.ReadLine()!;

                Console.Write("Введіть вік гравця: ");
                int age = int.Parse(Console.ReadLine()!);

                var player = new Player
                {
                    Id = (newTeam.Id * 100) + playerIdCounter, 
                    Name = playerName,
                    SecondName = secondName,
                    Age = age,
                    Team = teamName
                };

                newTeam.Players.Add(player);
                playerIdCounter++;
                Console.WriteLine($"[+] Гравця {playerName} {secondName} додано до команди");
            }

            _tournament.AddParticipant(newTeam);
            Console.WriteLine($"\nКоманду '{teamName}' збережено, гравців у складі: {newTeam.Players.Count}");
            Console.WriteLine($"Всього команд у турнірі: {_tournament.Participants.Count}");
            
            if (_settings != null && _tournament.Participants.Count == _settings.TotalTeams)
            {
                Console.WriteLine("\nДосягнуто максимальну кількість команд для цього турніру");
                break;
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
        if (_groupStage == null || _groupStage.Matches.Count == 0)
        {
            Console.WriteLine("Спочатку згенеруйте групи");
            return;
        }

        Console.WriteLine("\nЯк хочете ввести результати?");
        Console.WriteLine("1 - Вручну");
        Console.WriteLine("2 - Автоматично (випадкові рахунки)");
        Console.Write("Вибір: ");
        string choice = Console.ReadLine()!;

        Random rnd = new Random();

        foreach (var match in _groupStage.Matches)
        {
            if (match.IsCompleted) continue; 

            var score = new Score();

            if (choice == "1")
            {
                Console.WriteLine($"\nМатч: {match.FirstParticipant.Name} VS {match.SecondParticipant.Name}");
                Console.Write($"Поінти команди {match.FirstParticipant.Name}: ");
                score.FirstScore = int.Parse(Console.ReadLine()!);
                Console.Write($"Поінти команди {match.SecondParticipant.Name}: ");
                score.SecondScore = int.Parse(Console.ReadLine()!);
            }
            else
            {
                score.FirstScore = rnd.Next(0, 6);
                score.SecondScore = rnd.Next(0, 6);
            }

            match.Scores.Add(score);
            match.Status = MatchStatus.Finished;
            match.IsCompleted = true;
        }
        
        Console.WriteLine("Всі матчі групового етапу оновлено");
    }

    private void ShowGroupStandings()
    {
        if (_groupStage == null)
        {
            Console.WriteLine("Спочатку згенеруйте групи");
            return;
        }

        Console.WriteLine("\n--- Турнірна таблиця ---");
        var standings = _groupStage.CalculateStandings(_tournament.Participants);
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
        var standings = _groupStage.CalculateStandings(_tournament.Participants);
        
        _playOff = new PlayOff(_settings);
        _playOff.StartPlayOff(standings);
        
        Console.WriteLine("Сітку Плей-оф сформовано");
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