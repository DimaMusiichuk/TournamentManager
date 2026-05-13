using TournemantManager.Contracts;

namespace TournemantManager.Core.Base;

public abstract class EntityBase : IIdentifiable
{
    public int Id { get; set; }
    
    public override bool Equals(object? obj)
    {
        if (obj is EntityBase other)
        {
            if (this.GetType() != other.GetType())
            {
                return false;
            }

            return this.Id == other.Id;
        }
    
        return false;
    }

    public override int GetHashCode()
    {
        return  Id.GetHashCode();
    }
}