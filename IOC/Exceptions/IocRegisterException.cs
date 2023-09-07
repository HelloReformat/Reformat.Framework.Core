namespace Reformat.Framework.Core.IOC.Exceptions;

public class IocRegisterException : IocException
{
    public IocRegisterException()
    {
    }

    public IocRegisterException(string message) : base(message)
    {
    }

    public IocRegisterException(string message, Exception e) : base(message, e)
    {
    }
}