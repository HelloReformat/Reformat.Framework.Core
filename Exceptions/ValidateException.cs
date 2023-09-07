using Reformat.Framework.Core.Validator.Domains;

namespace Reformat.Framework.Core.Exceptions;

public class ValidateException : Exception
{
    public int code { get; set; } = 500;
    public string message { get; set; } = "DTO参数校验不通过";
    public ValidResult details { get; set; }
    
    public ValidateException(string message): base()
    {
        this.message = message;
    }

    public ValidateException(ValidResult details): base()
    {
        this.details = details;
    }

    // public ValidateException(string message): base(message)
    // {
    //     this.message = message;
    // }
    //
    // public ValidateException(string message,object details): base(message)
    // {
    //     this.message = message;
    //     this.details = details;
    // }
    //
    // public ValidateException(int code, string message, object details): base(message)
    // {
    //     this.code = code;
    //     this.message = message;
    //     this.details = details;
    // }
}