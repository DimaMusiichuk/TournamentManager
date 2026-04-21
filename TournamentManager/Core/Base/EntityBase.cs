using Tournemant_Manager.Contracts;

namespace Tournemant_Manager.Domain.Base;

public abstract class EntityBase : IIdentifiable
{
    public int Id { get; set; }
    
    public override bool Equals(object? obj)
    {
        if (obj is EntityBase other)
        {
            return this.Id ==  other.Id;
        }
        else
        {
            return false;
        }
    }

    public override int GetHashCode()
    {
        return  Id.GetHashCode();
    }
}