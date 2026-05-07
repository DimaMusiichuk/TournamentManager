using System;
using System.Collections.Generic;
using TournemantManager.Contracts;
using TournemantManager.Core.Enums;
using TournemantManager.Core.Logic;
using TournemantManager.Core.Models;
using TournemantManager.Core.Exceptions;
using TournemantManager.Infrastructure;

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
            if (_settings != null && string.IsNullOrEmpty(_settings.TournamentName))
            {
                Console.WriteLine($"{_settings.TournamentName} | Дисципліна: {_settings.Discipline}");
            }
            else
            {
                Console.WriteLine("\n=== МЕНЮ ===");
            }
            Console.WriteLine("1 - Налаштувати формат турніру");
            Console.WriteLine("2 - Додати команду");
            Console.WriteLine("3 - Згенерувати групи");
            Console.WriteLine("4 - Оновити рахунок групового етапу");
            Console.WriteLine("5 - Переглянути таблицю груп");
            Console.WriteLine("6 - Запустити Плей-оф");
            Console.WriteLine("7 - Переглянути сітку Плей-оф");
            Console.WriteLine("8 - Переглянути список усіх команд та гравців");
            Console.WriteLine("9 - Зіграти матчі Плей-оф");
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
                    case 9:
                    {
                        UpdatePlayOffScores();
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
        
        Console.Write("Введіть кіберспортивну дисципліну (наприклад, Dota 2, CS2): ");
        _settings.Discipline = Console.ReadLine()!;
        
        Console.Write("Введіть назву турніру: ");
        _settings.TournamentName = Console.ReadLine()!;

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
            
            Console.Write("Країна кіберспортивної команди:");
            string teamCountry = Console.ReadLine()!;

            var newTeam = new Team 
            { 
                Id = _tournament.Participants.Count + 1, 
                Name = teamName,
                TeamCountry = teamCountry
            };

            Console.WriteLine($"Команду '{teamName}' створено, потрібно додати {_settings.TeamSize} гравців");
            
            for (int i = 0; i < _settings.TeamSize; i++)
            {
                Console.WriteLine($"\nГравець {i + 1} з {_settings.TeamSize}:");
                
                Console.Write("Нікнейм: ");
                string nickname = Console.ReadLine()!;
                
                string firstName = GetValidFirstName();
                string secondName = GetValidSecondName();
                int age = GetValidAge();
                
                Console.Write("Країна гравця: ");
                string playerCountry = Console.ReadLine()!;

                var player = new Player
                {
                    Id = (newTeam.Id * 100) + (i + 1), 
                    Nickname = nickname,
                    Name = firstName,
                    SecondName = secondName,
                    Age = age,
                    Country = playerCountry,
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
            Console.WriteLine($"\nКоманда: {team.Name} [{team.TeamCountry}] (ID: {team.Id})");
            Console.WriteLine($"Капітан: {team.Captain.Nickname}👑");
            if (!string.IsNullOrEmpty(team.Coach))
            {
                Console.WriteLine($"Тренер: {team.Coach}©️");
            }
            Console.WriteLine("Склад:");
            foreach (var player in team.Players)
            {
                Console.WriteLine($"- [{player.Nickname}] {player.Name} {player.SecondName}, {player.Age} років, {player.Country}");
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
        
        _playOff = new PlayOff(_settings);
        _playOff.StartPlayOff(standings);
        
    }
    
    private void ProcessPlayOffMatches(List<Match> matches, string choice)
    {
        Random rnd = new Random();
        foreach (var match in matches)
        {
            if (match.IsCompleted) 
            {
                continue; 
            }

            var score = new Score();
            
            if (choice == "1")
            {
                EnterManualScore(match, score, 1); 
            }
            else
            {
                GenerateAutoScore(match, score, rnd, 1); 
            }

            match.Scores.Add(score);
            match.Status = MatchStatus.Finished;
            match.IsCompleted = true;
        }
    }

    private void ShowPlayOff()
    {
        if (_playOff == null)
        {
            Console.WriteLine("Спочатку запустіть Плей-оф");
            return;
        }

        Console.WriteLine("\n--- Матчі верхньої сітки Плей-оф ---");
        ConsolePrinter.PrintPlayOff(_playOff.UpperMatches);
        
        if (_settings.HasLowerBracket && _playOff.LowerMatches.Count > 0)
        {
            Console.WriteLine("\n--- Матчі нижньої сітки Плей-оф ---");
            ConsolePrinter.PrintPlayOff(_playOff.LowerMatches);
        }
    }
    
    private string GetValidFirstName()
    {
        while (true)
        {
            try
            {
                Console.Write("Ім'я: ");
                string input = Console.ReadLine()!;
                
                if (string.IsNullOrWhiteSpace(input))
                {
                    throw new InvalidNameFormatException("Поле не може бути порожнім!");
                }

                if (!char.IsUpper(input[0]))
                {
                    throw new InvalidNameFormatException("Ім'я повинно починатися з великої літери");
                }
                
                return input;
            }
            catch (InvalidNameFormatException ex)
            {
                Console.WriteLine($"Помилка: {ex.Message}");
            }
        }
    }
    
    private string GetValidSecondName()
    {
        while (true)
        {
            try
            {
                Console.Write("Прізвище: ");
                string input = Console.ReadLine()!;
                
                if (string.IsNullOrWhiteSpace(input))
                {
                    throw new InvalidNameFormatException("Поле не може бути порожнім");
                }

                if (!char.IsUpper(input[0]))
                {
                    throw new InvalidNameFormatException("Прізвище повинно починатися з великої літери");
                }
                
                return input;
            }
            catch (InvalidNameFormatException ex)
            {
                Console.WriteLine($"Помилка: {ex.Message}");
            }
        }
    }
    
    private int GetValidAge()
    {
        while (true)
        {
            try
            {
                Console.Write("Вік: ");
                int age = int.Parse(Console.ReadLine()!);
                
                if (age < 13 || age > 45)
                {
                    throw new InvalidAgeException("Вік повинен бути від 13 до 45 років");
                }
                
                return age;
            }
            catch (FormatException)
            {
                Console.WriteLine("Помилка: введіть число");
            }
            catch (InvalidAgeException ex)
            {
                Console.WriteLine($"Помилка: {ex.Message}");
            }
        }
    }
    
    private void UpdatePlayOffScores()
    {
        if (_playOff == null || _playOff.IsFinished)
        {
            Console.WriteLine("Плей-оф ще не запущено або турнір вже завершено");
            return;
        }

        Console.WriteLine("\nЯк хочете ввести результати Плей-оф?");
        Console.WriteLine("1 - Вручну");
        Console.WriteLine("2 - Автоматично (випадкові рахунки)");
        Console.Write("Вибір: ");
        string choice = Console.ReadLine()!;

        Console.WriteLine("\n--- Граємо Верхню сітку ---");
        ProcessPlayOffMatches(_playOff.UpperMatches, choice);

        if (_settings.HasLowerBracket && _playOff.LowerMatches.Count > 0)
        {
            Console.WriteLine("\n--- Граємо Нижню сітку ---");
            ProcessPlayOffMatches(_playOff.LowerMatches, choice);
        }

        if (_playOff.AreAllMatchesCompleted())
        {
            _playOff.AdvanceToNextRound();
            
            if (_playOff.IsFinished)
            {
                Console.WriteLine($"🏆 ТУРНІР ЗАВЕРШЕНО! ПЕРЕМОЖЕЦЬ: {_playOff.TournamentWinner.Name} 🏆");
            }
            else
            {
                Console.WriteLine("\nУсі матчі поточного раунду зіграно, сформовано наступний етап Плей-оф.");
            }
        }
    }
    
    private void SaveTournament()
    {
        try
        {
            var storage = new FileStorage<Tournament>("tournament_data.json");
            
            storage.Save(_tournament);
            
            Console.WriteLine("Дані турніру збережено у файл tournament_data.json");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Помилка при збереженні: {ex.Message}");
        }
    }

    private void LoadTournament()
    {
        try
        {
            var storage = new FileStorage<Tournament>("tournament_data.json");
            
            var loadedTournament = storage.Load();
            
            if (loadedTournament != null)
            {
                _tournament = loadedTournament;
                Console.WriteLine("Дані турніру завантажено");
                Console.WriteLine($"Кількість команд у турнірі: {_tournament.Participants.Count}");
            }
            else
            {
                Console.WriteLine("Файл збереження не знайдено, спочатку збережіть турнір");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Помилка при завантаженні: {ex.Message}");
        }
    }
}