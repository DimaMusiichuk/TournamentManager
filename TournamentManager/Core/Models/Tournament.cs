using TournemantManager.Contracts;
using TournemantManager.Core.Base;

namespace TournemantManager.Core.Models;

public class Tournament : EntityBase, ITournament
{
    public string Title { get; set; }
    public string Description { get; set; }
    public IList<IParticipant> Participants { get; set; } = new List<IParticipant>();
    public void AddParticipant(IParticipant participant)
    {   
        Participants.Add(participant);
    }

    public void RemoveParticipant(IParticipant participant)
    {
        Participants.Remove(participant);
    }

    public IList<Match> Matches { get; set; } =  new List<Match>();
}