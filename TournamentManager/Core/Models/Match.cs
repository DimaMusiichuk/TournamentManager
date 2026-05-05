using TournemantManager.Contracts;
using TournemantManager.Core.Base;
using TournemantManager.Core.Enums;

namespace TournemantManager.Core.Models;

public class Match : EntityBase
{
   public IParticipant FirstParticipant { get; set; }
   public IParticipant SecondParticipant { get; set; }
   public List<Score> Scores { get; set; } = new();
   public bool IsCompleted { get; set; }
   public MatchStatus Status { get; set; } = MatchStatus.Pending;
}