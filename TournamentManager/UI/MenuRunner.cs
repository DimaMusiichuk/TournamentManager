using System;
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
    private TeamEditorUI _teamEditorUI = new TeamEditorUI();
    private TournamentSettingsUI _settingsUI = new TournamentSettingsUI();

    private readonly string[] _menuOptions = new string[]
    {
        "Налаштувати формат турніру",
        "Додати команду",
        "Видалити команду",
        "Згенерувати групи",
        "Оновити рахунок групового етапу",
        "Переглянути таблицю груп",
        "Запустити Плей-оф",
        "Переглянути сітку Плей-оф",
        "Переглянути список усіх команд та гравців",
        "Зіграти матчі Плей-оф",
        "Зберегти турнір у файл",
        "Завантажити турнір з файлу",
        "Зіграти переігровки груп",
        "Редагувати існуючу команду",
        "Редагувати параметри турніру",
        "Видалити турнір",
        "Пошук та фільтрація матчів",
        "Вихід"
    };

    public void Run()
    {
        while (true)
        {
            int selectedIndex = RunInteractiveMenu();

            Console.Clear();

            try
            {
                switch (selectedIndex)
                {
                    case 0:
                        _settings = _setupUI.ConfigureTournament();
                        break;
                    case 1:
                        _setupUI.AddTeam(_tournament, _settings);
                        break;
                    case 2:
                        _setupUI.HandleRemoveParticipant(_tournament);
                        break;
                    case 3:
                        _groupStage = _matchUI.GenerateGroups(_tournament, _settings);
                        break;
                    case 4:
                        _matchUI.UpdateGroupScores(_groupStage, _settings);
                        break;
                    case 5:
                        _matchUI.ShowGroupStandings(_groupStage);
                        break;
                    case 6:
                        _playOff = _matchUI.StartPlayOff(_groupStage, _settings);
                        break;
                    case 7:
                        _matchUI.ShowPlayOff(_playOff, _settings);
                        break;
                    case 8:
                        _setupUI.ShowAllTeams(_tournament);
                        break;
                    case 9:
                        _matchUI.UpdatePlayOffScores(_playOff, _settings);
                        break;
                    case 10:
                        SaveTournament();
                        break;
                    case 11:
                        LoadTournament();
                        break;
                    case 12:
                        _matchUI.PlayTiebreakers(_groupStage, _settings);
                        break;
                    case 13:
                        _teamEditorUI.EditTeam(_tournament);
                        break;
                    case 14:
                        _settings = _settingsUI.EditSettings(_settings);
                        _groupStage = new GroupStage(_settings);
                        break;
                    case 15:
                        DeleteTournament();
                        break;
                    case 16:
                        _matchUI.SearchAndFilterMatches(_groupStage, _playOff);
                        break;
                    case 17:
                        Console.WriteLine("Вихід з програми");
                        return; 
                }
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"\nПомилка: {ex.Message}");
                Console.ResetColor();
            }

            Console.WriteLine("\nНатисніть будь-яку клавішу для повернення до меню");
            Console.ReadKey(true);
        }
    }

    private int RunInteractiveMenu()
    {
        int selectedIndex = 0;
        ConsoleKey keyPressed;

        Console.CursorVisible = false;
        
        Console.Clear();

        while (true)
        {
            Console.SetCursorPosition(0, 0); 
            
            Console.ForegroundColor = ConsoleColor.Green;
            if (_settings != null && !string.IsNullOrEmpty(_settings.TournamentName))
            {
                Console.WriteLine($"=== {_settings.TournamentName} | Дисципліна: {_settings.Discipline} ===\n".PadRight(50));
            }
            else
            {
                Console.WriteLine("=== ТУРНІРНИЙ МЕНЕДЖЕР ===\n".PadRight(50));
            }
            Console.ResetColor();

            for (int i = 0; i < _menuOptions.Length; i++)
            {
                if (i == selectedIndex)
                {
                    Console.ForegroundColor = ConsoleColor.Black;
                    Console.BackgroundColor = ConsoleColor.White;
                    Console.WriteLine($" > {_menuOptions[i]} ".PadRight(50));
                    Console.ResetColor(); 
                }
                else
                {
                    Console.WriteLine($"   {_menuOptions[i]} ".PadRight(50));
                }
            }

            ConsoleKeyInfo keyInfo = Console.ReadKey(true);
            keyPressed = keyInfo.Key;

            if (keyPressed == ConsoleKey.Enter)
            {
                break;
            }

            if (keyPressed == ConsoleKey.UpArrow)
            {
                selectedIndex--;
                if (selectedIndex < 0)
                {
                    selectedIndex = _menuOptions.Length - 1;
                } 
            }
            else if (keyPressed == ConsoleKey.DownArrow)
            {
                selectedIndex++;
                if (selectedIndex >= _menuOptions.Length)
                {
                    selectedIndex = 0;
                } 
            }
        }

        Console.CursorVisible = true;

        return selectedIndex;
    }

    private void SaveTournament()
    {
        try
        {
            _tournament.Settings = _settings;
            
            string baseDir = AppDomain.CurrentDomain.BaseDirectory;
            string fullPath = Path.Combine(baseDir, "tournament_data.json");
            
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
            
            string baseDir = AppDomain.CurrentDomain.BaseDirectory;
            string filePath = Path.Combine(baseDir, "tournament_data.json");
            
            if (loadedTournament != null)
            {
                _tournament = loadedTournament;
                _settings = _tournament.Settings;
                
                if (_settings != null)
                {
                    _groupStage = new GroupStage(_settings);
                    _playOff = new PlayOff(_settings);
                }
                
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
    
    private void DeleteTournament()
    {
        Console.Clear();
        Console.WriteLine("=== ВИДАЛЕННЯ ТУРНІРУ ===");
        Console.WriteLine("Це очистить всі дані турніру з пам'яті та файлу");
        Console.Write("Ви впевнені? (y/n): ");

        if (Console.ReadLine()?.ToLower() == "y")
        {
            string baseDir = AppDomain.CurrentDomain.BaseDirectory;
            string filePath = Path.Combine(baseDir, "tournament_data.json");
            string backupPath = Path.Combine(baseDir, $"tournament_backup_{DateTime.Now:yyyyMMdd_HHmmss}.json");

            if (File.Exists(filePath))
            {
                File.Copy(filePath, backupPath); 
                File.Delete(filePath);
            
                Console.WriteLine($"\n файл видалено.");
                Console.WriteLine($"Історія збережена у файл: {Path.GetFileName(backupPath)} 😉");
            }

            _tournament = new Tournament();
            _settings = null;
            _groupStage = null;
            _playOff = null;

            Console.WriteLine("\nТурнір видалено з системи");
        }
        else
        {
            Console.WriteLine("\nВидалення скасовано");
        }
    }
}