namespace Reformat.Framework.Core.Exceptions;

public class BusinessException : Exception
{
    public int code { get; set; } = 503;
    public string message { get; set; }
    public object details { get; set; }

    public BusinessException(string message): base(message)
    {
        this.message = message;
    }
    
    public BusinessException(string message,object details): base(message)
    {
        this.message = message;
        this.details = details;
    }

    public BusinessException(int code, string message, object details): base(message)
    {
        this.code = code;
        this.message = message;
        this.details = details;
    }
}