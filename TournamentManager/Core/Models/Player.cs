using Tournemant_Manager.Contracts;
using Tournemant_Manager.Domain.Base;

namespace Tournemant_Manager.Domain.Models;

public class Player : EntityBase, IParticipant
{
    public int Age { get; set; }
    public string SecondName { get; set; }
    public string Name { get; set; }
}