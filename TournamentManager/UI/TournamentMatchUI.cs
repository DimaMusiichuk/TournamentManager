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
        while (true)
        {
            Console.WriteLine($"\nМатч: {match.FirstParticipant.Name} VS {match.SecondParticipant.Name}");
            Console.Write($"Рахунок команди {match.FirstParticipant.Name}: ");
            score.FirstScore = int.Parse(Console.ReadLine()!);
            Console.Write($"Рахунок команди {match.SecondParticipant.Name}: ");
            score.SecondScore = int.Parse(Console.ReadLine()!);

            if (format == 2 || score.FirstScore != score.SecondScore) 
            {
                break; 
            }
            
            Console.WriteLine("Помилка, в форматі матчів BO1, BO3, BO5 нічия не можлива спробуйте ще раз");
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
}