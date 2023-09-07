namespace Reformat.Framework.Core.IOC.Exceptions;

public class IocAutowiredException : IocException
{
    public IocAutowiredException()
    {
    }

    public IocAutowiredException(string message) : base(message)
    {
    }

    public IocAutowiredException(string message, Exception e) : base(message, e)
    {
    }
}