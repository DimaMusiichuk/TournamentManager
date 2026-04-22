using TournemantManager.Contracts;
using TournemantManager.Core.Models;

namespace TournemantManager.Core.Logic;

public class GroupStage
{
    public List<Match> Matches { get; set; } = new();

    public List<Match> GruopCreator(IList<IParticipant>  participants, int numberOfGroups)
    {
        List<List<IParticipant>> groups = new();
        for (int i = 0; i < numberOfGroups; i++)
        {
            groups.Add(new List<IParticipant>());
        }
        
        for (int i = 0; i < participants.Count; i++)
        {
            int groupIndex = i % numberOfGroups;
            groups[groupIndex].Add(participants[i]);
        }

        foreach (var group in groups)
        {
            for (int i = 0; i < group.Count; i++)
            {
                for (int j = i + 1; j < group.Count; j++)
                {
                    var match = new Match();
                    match.FirstParticipant = group[i];
                    match.SecondParticipant = group[j];
                    Matches.Add(match);
                }   
            }
        }
        return Matches;
    }
}