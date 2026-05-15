using TournemantManager.Core.Models;

namespace TournemantManager.UI;

public class TeamEditorUI
{
    public void EditTeam(Tournament tournament)
    {
        Console.WriteLine("\n--- Редагування команди ---");
        Console.Write("Введіть назву команди, яку хочете виправити (або '0' для виходу): ");
        string name = Console.ReadLine()!;
        
        if (name == "0") 
        {
            return;
        }

        Team teamToEdit = null;
        foreach (var participant in tournament.Participants)
        {
            if (participant.Name == name && participant is Team)
            {
                teamToEdit = (Team)participant;
                break;
            }
        }

        if (teamToEdit == null)
        {
            Console.WriteLine("Команду з такою назвою не знайдено");
            return;
        }

        Console.Write($"Нова назва команди (зараз '{teamToEdit.Name}'). Введіть нову або натисніть Enter, щоб залишити як є: ");
        string newName = Console.ReadLine()!;
        
        if (!string.IsNullOrWhiteSpace(newName))
        {
            teamToEdit.Name = newName;
            Console.WriteLine("Назву команди оновлено");
        }

        Console.Write($"Нова країна команди (зараз '{teamToEdit.TeamCountry}'). Введіть нову або натисніть Enter, щоб залишити як є: ");
        string newTeamCountry = Console.ReadLine()!;
        
        if (!string.IsNullOrWhiteSpace(newTeamCountry))
        {
            teamToEdit.TeamCountry = newTeamCountry;
            Console.WriteLine("Країну команди оновлено");
        }

        Console.Write("\nХочете відредагувати гравців цієї команди? (1 - Так, 0 - Ні): ");
        if (Console.ReadLine() == "1")
        {
            EditTeamPlayers(teamToEdit);
        }
    }

    private void EditTeamPlayers(Team team)
    {
        while (true)
        {
            Console.WriteLine("\n--- Список гравців ---");
            for (int i = 0; i < team.Players.Count; i++)
            {
                Console.WriteLine($"{i + 1} - {team.Players[i].Nickname} ({team.Players[i].Name} {team.Players[i].SecondName}, {team.Players[i].Age} р., {team.Players[i].Country})");
            }
            
            Console.Write("Оберіть номер гравця для редагування (або '0' для виходу): ");
            string choice = Console.ReadLine()!;
            
            if (choice == "0")
            {
                break;
            }
            
            if (int.TryParse(choice, out int playerIndex) && playerIndex > 0 && playerIndex <= team.Players.Count)
            {
                Player playerToEdit = team.Players[playerIndex - 1];
                
                Console.Write($"Новий нікнейм (зараз '{playerToEdit.Nickname}'). Введіть новий або натисніть Enter: ");
                string newNick = Console.ReadLine()!;
                if (!string.IsNullOrWhiteSpace(newNick))
                {
                    playerToEdit.Nickname = newNick;
                }
                
                Console.Write($"Нове ім'я (зараз '{playerToEdit.Name}'). Введіть нове або натисніть Enter: ");
                string newName = Console.ReadLine()!;
                if (!string.IsNullOrWhiteSpace(newName))
                {
                    playerToEdit.Name = newName;
                }

                Console.Write($"Нове прізвище (зараз '{playerToEdit.SecondName}'). Введіть нове або натисніть Enter: ");
                string newSecondName = Console.ReadLine()!;
                if (!string.IsNullOrWhiteSpace(newSecondName))
                {
                    playerToEdit.SecondName = newSecondName;
                }

                Console.Write($"Новий вік (зараз '{playerToEdit.Age}'). Введіть новий або натисніть Enter: ");
                string newAgeStr = Console.ReadLine()!;
                if (!string.IsNullOrWhiteSpace(newAgeStr))
                {
                    if (int.TryParse(newAgeStr, out int newAge))
                    {
                        playerToEdit.Age = newAge;
                    }
                    else
                    {
                        Console.WriteLine("Вік має бути числом.");
                    }
                }

                Console.Write($"Нова країна (зараз '{playerToEdit.Country}'). Введіть нову або натисніть Enter: ");
                string newCountry = Console.ReadLine()!;
                if (!string.IsNullOrWhiteSpace(newCountry))
                {
                    playerToEdit.Country = newCountry;
                }

                Console.WriteLine("Дані гравця оновлено");
            }
            else
            {
                Console.WriteLine("Невірний номер гравця");
            }
        }
    }
}