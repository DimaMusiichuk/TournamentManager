namespace TournemantManager.Core.Exceptions;

public class InvalidNameFormatException : Exception
{
    public InvalidNameFormatException(string message) : base(message) 
    { 
    }
}

public class InvalidAgeException : Exception
{
    public InvalidAgeException(string message) : base(message) 
    { 
    }
}