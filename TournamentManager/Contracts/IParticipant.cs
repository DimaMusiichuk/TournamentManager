using System.Text.Json.Serialization;
using TournemantManager.Core.Models;

namespace TournemantManager.Contracts;

[JsonDerivedType(typeof(Team), typeDiscriminator: "Team")]
public interface IParticipant
{
    string Name { get; set; }
}