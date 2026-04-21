using Tournemant_Manager.Domain.Models;

namespace Tournemant_Manager.Contracts;

public interface ITournament
{
    string Title { get; set; }
    string Description { get; set; }
    IList<IParticipant> Participants { get; set; }
    void AddParticipant(IParticipant participant);
    void RemoveParticipant(IParticipant participant);
}