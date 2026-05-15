using TournemantManager.Core.Models;

namespace TournemantManager.UI;

public class TournamentSettingsUI
{
    public TournamentSettings EditSettings(TournamentSettings currentSettings)
    {
        Console.Clear();
        Console.WriteLine("=== РЕДАГУВАННЯ НАЛАШТУВАНЬ ТУРНІРУ ===");
        Console.WriteLine($"Поточна назва: {currentSettings.TournamentName}");
        Console.WriteLine($"Поточна дисципліна: {currentSettings.Discipline}");

        Console.Write("Введіть нову назву (Enter, щоб залишити поточну): ");
        string name = Console.ReadLine();
        if (!string.IsNullOrWhiteSpace(name)) currentSettings.TournamentName = name;

        Console.Write("Введіть нову дисципліну (Enter, щоб залишити поточну): ");
        string game = Console.ReadLine();
        if (!string.IsNullOrWhiteSpace(game)) currentSettings.Discipline = game;

        currentSettings.WinPoint = ReadIntWithDefault("Очки за перемогу", currentSettings.WinPoint);
        currentSettings.DrawPoint = ReadIntWithDefault("Очки за нічию", currentSettings.DrawPoint);
        currentSettings.LosePoint = ReadIntWithDefault("Очки за поразку", currentSettings.LosePoint);

        Console.WriteLine("\nНалаштування оновлено, натисніть будь-яку клавішу");
        Console.ReadKey();
        
        return currentSettings;
    }

    private int ReadIntWithDefault(string prompt, int defaultValue)
    {
        Console.Write($"{prompt} [поточне: {defaultValue}]: ");
        string input = Console.ReadLine();
        if (int.TryParse(input, out int result))
        {
            return result;
        }
        return defaultValue;
    }
}