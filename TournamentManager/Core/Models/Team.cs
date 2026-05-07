using TournemantManager.Contracts;
using TournemantManager.Core.Base;

namespace TournemantManager.Core.Models;

public class Team : EntityBase, IParticipant
{
    public string Name { get; set; }
    public string Coach { get; set; }
    public Player Captain { get; set; }
    public List<Player> Players { get; set; } = new();
    public string TeamCountry { get; set; }
}