using TournemantManager.Core.Exceptions;
using TournemantManager.Core.Models;

namespace TournemantManager.UI;

public class TournamentSetupUI
{
    public TournamentSettings ConfigureTournament()
    {
        Console.WriteLine("\n--- Налаштування турніру ---");
        var settings = new TournamentSettings();

        Console.Write("Введіть кіберспортивну дисципліну (наприклад, Dota 2, CS2): ");
        settings.Discipline = Console.ReadLine()!;

        Console.Write("Введіть назву турніру: ");
        settings.TournamentName = Console.ReadLine()!;

        Console.Write("Введіть кількість команд для турніру (наприклад: 8, 12, 16): ");
        settings.TotalTeams = int.Parse(Console.ReadLine()!);

        Console.Write("Скільки гравців має бути в одній команді? (наприклад: 5): ");
        settings.TeamSize = int.Parse(Console.ReadLine()!);

        Console.Write("Формат матчів у групі (1 - Bo1, 2 - Bo2, 3 - Bo3): ");
        settings.MatchesPerOpponent = int.Parse(Console.ReadLine()!);

        while (true)
        {
            Console.Write("Введіть кількість очок за перемогу (рекомендовано 2): ");
            settings.WinPoint = int.Parse(Console.ReadLine()!);

            Console.Write("Введіть кількість очок за нічию (рекомендовано 1): ");
            settings.DrawPoint = int.Parse(Console.ReadLine()!);

            Console.Write("Введіть кількість очок за поразку (рекомендовано 0): ");
            settings.LosePoint = int.Parse(Console.ReadLine()!);

            if (settings.WinPoint > settings.DrawPoint && settings.DrawPoint >= settings.LosePoint)
            {
                break;
            }

            Console.WriteLine("Помилка: очки за перемогу повинні бути більші за нічию, а нічия більша або рівна поразці");
        }

        Console.Write("Введіть кількість груп (наприклад: 1, 2, 4): ");
        settings.NumberOfGroups = int.Parse(Console.ReadLine()!);
        
        Console.Write("Чи буде турнір мати нижню сітку? (1 - Так, 0 - Ні): ");
        settings.HasLowerBracket = (Console.ReadLine() == "1");

        Console.Write("Скільки команд виходять у верхню сітку Плей-оф?: ");
        settings.UpperBracketSlots = int.Parse(Console.ReadLine()!);

        if (settings.HasLowerBracket)
        {
            Console.Write("Скільки команд виходять у нижню сітку Плей-оф?: ");
            settings.LowerBracketSlots = int.Parse(Console.ReadLine()!);
        }

        Console.WriteLine("Налаштування збережено");
        return settings;
    }

    public void AddTeam(Tournament tournament, TournamentSettings settings)
    {
        if (settings == null)
        {
            throw new InvalidOperationException("Спочатку налаштуйте турнір");
        }
        
        Console.WriteLine("\n--- Додавання команд та гравців ---");
        Console.WriteLine("Введіть '0', щоб повернутися в головне меню");

        while (true)
        {
            if (tournament.Participants.Count >= settings.TotalTeams)
            {
                Console.WriteLine("Досягнуто максимальну кількість команд для цього турніру");
                break;
            }
            
            Console.Write("Введіть назву команди: ");
            string teamName = GetValidTeamName(tournament);

            if (teamName == "0") 
            {
                break;
            }
            
            string teamCountry = GetValidCountry("кіберспортивної команди");

            var newTeam = new Team 
            { 
                Id = tournament.Participants.Count + 1, 
                Name = teamName,
                TeamCountry = teamCountry
            };

            Console.WriteLine($"Команду '{teamName}' створено, потрібно додати {settings.TeamSize} гравців");
            
            for (int i = 0; i < settings.TeamSize; i++)
            {
                Console.WriteLine($"\nГравець {i + 1} з {settings.TeamSize}:");
                Console.Write("Нікнейм: ");
                string nickname = GetValidNickname(tournament);
                string firstName = GetValidFirstName();
                string secondName = GetValidSecondName();
                int age = GetValidAge();
                string playerCountry = GetValidCountry("гравця");

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
            
            Console.WriteLine("\nЧи є у команди капітан? (1 - Так, 0 - Ні): ");
            if (Console.ReadLine() == "1")
            {
                Console.WriteLine("\nХто з гравців є капітаном?");
                for (int i = 0; i < newTeam.Players.Count; i++)
                {
                    Console.WriteLine($"{i + 1} - {newTeam.Players[i].Nickname}");
                }
                
                Console.Write("Оберіть номер капітана: ");
                int captIndex = int.Parse(Console.ReadLine()!) - 1;
                newTeam.Captain = newTeam.Players[captIndex];
            }

            tournament.AddParticipant(newTeam);
            Console.WriteLine($"\nКоманду '{teamName}' збережено, гравців у складі: {newTeam.Players.Count}");
            Console.WriteLine($"Всього команд у турнірі: {tournament.Participants.Count}");
        }
    }
    
        public void HandleRemoveParticipant(Tournament tournament)
    {
        if (tournament.Participants == null || tournament.Participants.Count == 0)
        {
            Console.WriteLine("\nСписок учасників порожній, немає кого видаляти");
            return;
        }

        Console.WriteLine("\n--- Список учасників ---");
        for (int i = 0; i < tournament.Participants.Count; i++)
        {
            Console.WriteLine($"{i + 1}. {tournament.Participants[i].Name}"); 
        }

        Console.Write("\nВведіть номер команди для видалення (або 0 для відміни): ");
    
        string input = Console.ReadLine();
    
        if (int.TryParse(input, out int selectedIndex))
        {
            if (selectedIndex == 0) 
            {
                Console.WriteLine("Відміна операції");
                return;
            }

            if (selectedIndex > 0 && selectedIndex <= tournament.Participants.Count)
            {
                var participantToRemove = tournament.Participants[selectedIndex - 1];

                try
                {
                    tournament.RemoveParticipant(participantToRemove);
                    Console.WriteLine($"\nКоманду '{participantToRemove.Name}' видалено");
                }
                catch (InvalidOperationException ex)
                {
                    Console.WriteLine($"\nПомилка Логіки {ex.Message}");
                }
                catch (ArgumentException ex)
                {
                    Console.WriteLine($"\nПомилка Даних {ex.Message}");
                }
            }
            else
            {
                Console.WriteLine("\nПомилка: Команди з таким номером не існує");
            }
        }
        else
        {
            Console.WriteLine("\nПомилка: Будь ласка, введіть коректне число");
        }
    }

    public void ShowAllTeams(Tournament tournament)
    {
        if (tournament.Participants.Count == 0)
        {
            Console.WriteLine("У турнірі ще немає команд");
            return;
        }

        Console.WriteLine("\nСПИСОК КОМАНД УЧАСНИКІВ");
        foreach (Team team in tournament.Participants)
        {
            Console.WriteLine($"\nКоманда: {team.Name} [{team.TeamCountry}] (ID: {team.Id})");
            
            if (team.Captain != null)
            {
                Console.WriteLine($"Капітан: {team.Captain.Nickname}👑");
            }
            
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
    
    private string GetValidNickname(Tournament tournament)
    {
        while (true)
        {
            Console.Write("Нікнейм: ");
            string nickname = Console.ReadLine();

            if (string.IsNullOrWhiteSpace(nickname))
            {
                Console.WriteLine("Нікнейм не може бути порожнім");
                continue;
            }

            bool isGlobalDuplicate = false;
            foreach (var participant in tournament.Participants)
            {
                if (participant is Team team)
                {
                    if (team.Players.Any(p => p.Nickname.Equals(nickname, StringComparison.OrdinalIgnoreCase)))
                    {
                        isGlobalDuplicate = true;
                        break;
                    }
                }
            }

            if (isGlobalDuplicate)
            {
                Console.WriteLine($"Гравець з нікнеймом '{nickname}' вже записаний");
                continue;
            }

            return nickname;
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
                    throw new InvalidNameFormatException("Поле не може бути порожнім");
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
    
    private string GetValidCountry(string target)
    {
        while (true)
        {
            try
            {
                Console.Write($"Країна {target}: ");
                string input = Console.ReadLine()!;
                
                if (string.IsNullOrWhiteSpace(input)) 
                {
                    throw new InvalidCountryException("Поле країни не може бути порожнім");
                }

                if (!char.IsUpper(input[0])) 
                {
                    throw new InvalidCountryException("Назва країни повинна починатися з великої літери");
                }
                
                return input;
            }
            catch (InvalidCountryException ex) 
            { 
                Console.WriteLine($"Помилка: {ex.Message}"); 
            }
        }
    }
    
    private string GetValidTeamName(Tournament tournament)
    {
        while (true)
        {
            try
            {
                string teamName = Console.ReadLine()!;
                
                if (teamName == "0") 
                {
                    return teamName;
                }
                
                if (string.IsNullOrWhiteSpace(teamName))
                {
                    Console.WriteLine("Помилка: Назва команди не може бути порожньою");
                    Console.Write("Введіть назву команди: ");
                    continue;
                }

                if (tournament.Participants.Any(p => p.Name.Equals(teamName, StringComparison.OrdinalIgnoreCase)))
                {
                    throw new DuplicateTeamException($"Команда з назвою '{teamName}' вже зареєстрована в турнірі");
                }
                
                return teamName;
            }
            catch (DuplicateTeamException ex)
            {
                Console.WriteLine($"Помилка: {ex.Message}");
                Console.Write("Введіть назву команди: ");
            }
        }
    }
}