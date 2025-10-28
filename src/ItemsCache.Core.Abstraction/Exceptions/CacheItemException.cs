namespace ItemsCache.Core.Abstraction.Exceptions;

public class CacheItemException : Exception
{
    public CacheItemException(string message) : base(message)
    {
    }
}