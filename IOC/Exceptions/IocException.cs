namespace Reformat.Framework.Core.IOC.Exceptions;

public class IocException : SystemException
{
    public IocException(): base()
    {

    }
    
    public IocException(string message) : base(message)
    {
        
    }
    
    public IocException(string message,Exception e) : base(message,e)
    {
        
    }
}