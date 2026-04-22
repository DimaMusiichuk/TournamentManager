using TournemantManager.Core.Base;

namespace TournemantManager.Core.Models;

public class Score : EntityBase
{
    public int FirstScore { get; set; }
    public int SecondScore { get; set; }
}