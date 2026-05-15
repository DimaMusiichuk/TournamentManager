using TournemantManager.Core.Enums;
using TournemantManager.Core.Logic;
using TournemantManager.Core.Models;

namespace TournemantManager.UI;

public class TournamentMatchUI
{
    public GroupStage GenerateGroups(Tournament tournament, TournamentSettings settings)
    {
        if (settings == null || tournament.Participants.Count == 0)
        {
            throw new InvalidOperationException("Спочатку налаштуйте турнір і додайте команди");
        }

        Console.WriteLine("\n--- Жеребкування груп ---");
        var groupStage = new GroupStage(settings);
        tournament.Matches = groupStage.GroupCreator(tournament.Participants, settings.NumberOfGroups);
        
        Console.WriteLine($"Згенеровано {tournament.Matches.Count} матчів групового етапу");
        return groupStage;
    }

    public void UpdateGroupScores(GroupStage groupStage, TournamentSettings settings)
    {
        if (groupStage == null || groupStage.GroupMatches.Count == 0)
        {
            throw new InvalidOperationException("Спочатку згенеруйте групи");
        }

        Console.WriteLine("\nЯк хочете ввести результати?");
        Console.WriteLine("1 - Вручну");
        Console.WriteLine("2 - Автоматично (випадкові рахунки)");
        Console.Write("Вибір: ");
        string choice = Console.ReadLine()!;

        foreach (var groupPair in groupStage.GroupMatches)
        {
            if (choice == "1") 
            {
                Console.WriteLine($"\n--- Оновлення результатів: {groupPair.Key} ---");
            }
            
            ProcessMatches(groupPair.Value, choice, settings.MatchesPerOpponent);
        }
        Console.WriteLine("Всі матчі групового етапу оновлено");
    }

    public void ShowGroupStandings(GroupStage groupStage)
    {
        if (groupStage == null) 
        {
            throw new InvalidOperationException("Спочатку згенеруйте групи");
        }

        Console.WriteLine("\n--- Турнірна таблиця ---");
        ConsolePrinter.PrintGroupStage(groupStage.CalculateStandings());
    }

    public PlayOff StartPlayOff(GroupStage groupStage, TournamentSettings settings)
    {
        if (groupStage == null) 
        {
            throw new InvalidOperationException("Спочатку потрібно зіграти груповий етап");
        }

        Console.WriteLine("\n--- Формування сітки Плей-оф ---");
        var playOff = new PlayOff(settings);
        playOff.StartPlayOff(groupStage.CalculateStandings());
        return playOff;
    }

    public void ShowPlayOff(PlayOff playOff, TournamentSettings settings)
    {
        if (playOff == null) 
        {
            throw new InvalidOperationException("Спочатку запустіть Плей-оф");
        }

        Console.WriteLine("\n--- Матчі верхньої сітки Плей-оф ---");
        ConsolePrinter.PrintPlayOff(playOff.UpperMatches);
        
        if (settings.HasLowerBracket && playOff.LowerMatches.Count > 0)
        {
            Console.WriteLine("\n--- Матчі нижньої сітки Плей-оф ---");
            ConsolePrinter.PrintPlayOff(playOff.LowerMatches);
        }
    }

    public void UpdatePlayOffScores(PlayOff playOff, TournamentSettings settings)
    {
        if (playOff == null || playOff.IsFinished)
        {
            throw new InvalidOperationException("Плей-оф ще не запущено або турнір вже завершено");
        }

        Console.WriteLine("\nЯк хочете ввести результати Плей-оф?");
        Console.WriteLine("1 - Вручну");
        Console.WriteLine("2 - Автоматично (випадкові рахунки)");
        Console.Write("Вибір: ");
        string choice = Console.ReadLine()!;
        
        bool isUpperFinal = playOff.UpperMatches.Count == 1;

        Console.WriteLine("\n--- Граємо Верхню сітку ---");
        ProcessMatches(playOff.UpperMatches, choice, isUpperFinal ? 5 : 3);

        if (settings.HasLowerBracket && playOff.LowerMatches.Count > 0)
        {
            bool isLowerFinal = playOff.LowerMatches.Count == 1;
            Console.WriteLine("\n--- Граємо Нижню сітку ---");
            ProcessMatches(playOff.LowerMatches, choice, isLowerFinal ? 5 : 3);
        }

        if (playOff.AreAllMatchesCompleted())
        {
            playOff.AdvanceToNextRound();

            if (playOff.IsFinished) 
            {
                Console.WriteLine($"🏆 ТУРНІР ЗАВЕРШЕНО! ПЕРЕМОЖЕЦЬ: {playOff.TournamentWinner.Name} 🏆");
            }
            else 
            {
                Console.WriteLine("\nУсі матчі поточного раунду зіграно, сформовано наступний етап Плей-оф.");
            }
        }
    }

    public void PlayTiebreakers(GroupStage groupStage, TournamentSettings settings)
    {
        if (groupStage == null) 
        {
            throw new InvalidOperationException("Спочатку згенеруйте групи та зіграйте матчі");
        }

        groupStage.GenerateTiebreakers(groupStage.CalculateStandings());

        if (groupStage.TiebreakerMatches.Count == 0)
        {
            Console.WriteLine("Команди не мають однакової кількості очок в групі, переігровки не потрібні");
            return;
        }

        Console.WriteLine($"Знайдено {groupStage.TiebreakerMatches.Count} матчів-переігровок");
        Console.WriteLine("Як хочете зіграти ці тайбрейкери (формат Bo1)?");
        Console.WriteLine("1 - Вручну\n2 - Автоматично");
        Console.Write("Вибір: ");
        string choice = Console.ReadLine()!;

        ProcessMatches(groupStage.TiebreakerMatches, choice, 1);
        Console.WriteLine("Переігровки завершено, оновлена таблиця:");
        ShowGroupStandings(groupStage);
    }

    private void ProcessMatches(List<Match> matches, string choice, int format)
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
    Console.WriteLine($"\nМатч: {match.FirstParticipant.Name} VS {match.SecondParticipant.Name}");
    Console.WriteLine($"Формат матчу: BO{format}");

    int winnerChoice = GetWinnerChoice(match, format);
    int mapsPlayed = 0;

    if (format == 1)
    {
        mapsPlayed = ProcessBo1Score(score, winnerChoice);
    }
    else if (format == 2)
    {
        mapsPlayed = ProcessBo2Score(score, winnerChoice);
    }
    else
    {
        mapsPlayed = ProcessBestOfSeriesScore(score, winnerChoice, format);
    }

    Console.WriteLine($"\nРахунок по картах: {score.FirstScore}:{score.SecondScore}");

    CollectMapStatistics(match, mapsPlayed);
}


    private int GetWinnerChoice(Match match, int format)
    {
        while (true)
        {
            Console.WriteLine("\nХто переміг у цій серії?");
            Console.WriteLine($"1 - {match.FirstParticipant.Name}");
            Console.WriteLine($"2 - {match.SecondParticipant.Name}");
            if (format == 2) Console.WriteLine("3 - Нічия (Тільки для Bo2)");
            Console.Write("Ваш вибір: ");

            if (int.TryParse(Console.ReadLine(), out int choice) && (choice == 1 || choice == 2 || (format == 2 && choice == 3)))
            {
                return choice;
            }
            Console.WriteLine("Помилка: оберіть правильний варіант.");
        }
    }

    private int ProcessBo1Score(Score score, int winnerChoice)
    {
    if (winnerChoice == 1)
    {
        score.FirstScore = 1;
        score.SecondScore = 0;
    }
    else
    {
        score.FirstScore = 0;
        score.SecondScore = 1;
    }
    
    return 1;
    }

    private int ProcessBo2Score(Score score, int winnerChoice)
    {
        if (winnerChoice == 3)
        {
            score.FirstScore = 1; 
            score.SecondScore = 1; 
        } 
        else if (winnerChoice == 1)
        {
            score.FirstScore = 2;
            score.SecondScore = 0;
        }
        else
        {
            score.FirstScore = 0;
            score.SecondScore = 2;
        }
    
        return 2;
    }

    private int ProcessBestOfSeriesScore(Score score, int winnerChoice, int format)
    {
        int maxWinsNeeded;
        if (format == 3)
        {
            maxWinsNeeded = 2;
        }
        else
        {
            maxWinsNeeded = 3;
        }
    
        Console.Write($"\nСкільки карт змогла виграти команда, що програла? (Від 0 до {maxWinsNeeded - 1}): ");
        int loserScore = int.Parse(Console.ReadLine()!); 
        
        if (winnerChoice == 1)
        {
            score.FirstScore = maxWinsNeeded;
            score.SecondScore = loserScore;
        }
        else
        {
            score.FirstScore = loserScore;
            score.SecondScore = maxWinsNeeded;
        }
    
        return maxWinsNeeded + loserScore;
    }

    private void CollectMapStatistics(Match match, int mapsPlayed)
    {
    Console.WriteLine($"\n--- Детальна статистика за {mapsPlayed} зіграних мап ---");
        for (int i = 1; i <= mapsPlayed; i++)
        {
            Console.WriteLine($"\nМапа {i}:");
            Console.Write($"Статистика команди [{match.FirstParticipant.Name}]: ");
            string stats1 = Console.ReadLine()!;
        
            Console.Write($"Статистика команди [{match.SecondParticipant.Name}]: ");
            string stats2 = Console.ReadLine()!;
        
            Console.WriteLine($"=> Результат Мапи {i} збережено: {stats1} : {stats2}");
        }
    }

    private void GenerateAutoScore(Match match, Score score, Random rnd, int format)
    {
        score.FirstScore = rnd.Next(0, format + 1);
        score.SecondScore = rnd.Next(0, format + 1);

        if (format == 1 && score.FirstScore == score.SecondScore)
        {
            score.FirstScore = 1;
            score.SecondScore = 0;
        }

        if (format == 3 && score.FirstScore == score.SecondScore)
        {
            score.FirstScore = 2;
            score.SecondScore = rnd.Next(0, 2);
        }
    }
}