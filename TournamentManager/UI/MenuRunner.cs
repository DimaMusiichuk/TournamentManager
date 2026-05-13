using TournemantManager.Core.Logic;
using TournemantManager.Core.Models;
using TournemantManager.Infrastructure;

namespace TournemantManager.UI;

public class MenuRunner
{
    private Tournament _tournament = new Tournament();
    private TournamentSettings _settings;
    private GroupStage _groupStage;
    private PlayOff _playOff;

    private TournamentSetupUI _setupUI = new TournamentSetupUI();
    private TournamentMatchUI _matchUI = new TournamentMatchUI();

    public void Run()
    {
        while (true)
        {
            if (_settings != null && !string.IsNullOrEmpty(_settings.TournamentName))
            {
                Console.WriteLine($"\n=== {_settings.TournamentName} | Дисципліна: {_settings.Discipline} ===");
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
            Console.WriteLine("10 - Зберегти турнір у файл");
            Console.WriteLine("11 - Завантажити турнір з файлу");
            Console.WriteLine("12 - Зіграти переігровки груп");
            Console.WriteLine("0 - Вихід");
            Console.Write("Оберіть дію: ");

            try
            {
                int choice = int.Parse(Console.ReadLine()!);

                switch (choice)
                {
                    case 1:
                    {
                        _settings = _setupUI.ConfigureTournament();
                        break;
                    }
                    case 2:
                    {
                        _setupUI.AddTeam(_tournament, _settings);
                        break;
                    }
                    case 3:
                    {
                        _groupStage = _matchUI.GenerateGroups(_tournament, _settings);
                        break;
                    }
                    case 4:
                    {
                        _matchUI.UpdateGroupScores(_groupStage, _settings);
                        break;
                    }
                    case 5:
                    {
                        _matchUI.ShowGroupStandings(_groupStage);
                        break;
                    }
                    case 6:
                    {
                        _playOff = _matchUI.StartPlayOff(_groupStage, _settings);
                        break;
                    }
                    case 7:
                    {
                        _matchUI.ShowPlayOff(_playOff, _settings);
                        break;
                    }
                    case 8:
                    {
                        _setupUI.ShowAllTeams(_tournament);
                        break;
                    }
                    case 9:
                    {
                        _matchUI.UpdatePlayOffScores(_playOff, _settings);
                        break;
                    }
                    case 10:
                    {
                        SaveTournament();
                        break;
                    }
                    case 11:
                    {
                        LoadTournament();
                        break;
                    }
                    case 12:
                    {
                        _matchUI.PlayTiebreakers(_groupStage, _settings);
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
                Console.WriteLine("Помилка вводу, введіть числове значення");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Сталася помилка: {ex.Message}");
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