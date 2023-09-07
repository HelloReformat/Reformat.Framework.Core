namespace Reformat.Framework.Core.Exceptions;

public class PermissionException : Exception
{
    public int code { get; set; } = 403;
    public string message { get; set; }

    public PermissionException()
    {
        
    }
    
    public PermissionException(string message): base(message)
    {
        this.message = message;
    }

    public PermissionException(int code, string message): base(message)
    {
        this.code = code;
        this.message = message;
    }
}