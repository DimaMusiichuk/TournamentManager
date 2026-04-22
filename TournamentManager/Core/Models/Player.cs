using TournemantManager.Contracts;
using TournemantManager.Core.Base;

namespace TournemantManager.Core.Models;

public class Player : EntityBase, IParticipant
{
    public int Age { get; set; }
    public string SecondName { get; set; }
    public string Name { get; set; }
    public string Team { get; set; }
}