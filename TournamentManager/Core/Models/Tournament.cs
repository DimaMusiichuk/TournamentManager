using TournemantManager.Contracts;
using TournemantManager.Core.Base;

namespace TournemantManager.Core.Models;

public class Tournament : EntityBase, ITournament
{
    public string Title { get; set; }
    public string Description { get; set; }
    public TournamentSettings Settings { get; set; }
    public IList<IParticipant> Participants { get; set; } = new List<IParticipant>();
    public bool IsStarted { get; set; } = false;
    public List<Match> AllMatches { get; set; } = new();
    
    public void AddParticipant(IParticipant participant)
    {   
        Participants.Add(participant);
    }

    public void RemoveParticipant(IParticipant participant)
    {
        if (this.IsStarted) 
        {
            throw new InvalidOperationException("Неможливо видалити учасника: турнір вже розпочався");
        }

        if (!Participants.Remove(participant))
        {
            throw new ArgumentException("Учасника не знайдено у списку");
        }
    }

    public IList<Match> Matches { get; set; } =  new List<Match>();
}