using TournemantManager.Contracts;
using TournemantManager.Core.Base;

namespace TournemantManager.Core.Models;

public class Match : EntityBase
{
   public IParticipant FirstParticipant { get; set; }
   public IParticipant SecondParticipant { get; set; }
   public List<Score> Scores { get; set; } = new();
   public bool IsCompleted { get; set; }
}