namespace Reformat.Framework.Core.MVC;

public class ApiResult
{
    public ApiResult()
    {
    }

    public ApiResult(bool success,string message)
    {
        this.Success = success;
        this.Message = message;
    }

    public ApiResult(bool success,int errorCode, string message, object data)
    {
        this.Success = success;
        this.ErrorCode = errorCode;
        this.Message = message;
        this.Data = data;
    }

    /// <summary>
    /// 是否成功
    /// </summary>
    public bool Success { get; set; }

    /// <summary>
    /// 错误编号 当为0时为正确结果
    /// </summary>
    public int ErrorCode { get; set; }

    /// <summary>
    /// 运行结果
    /// </summary>
    public string Message { get; set; }
        
    /// <summary>
    /// 返回数据  
    /// </summary>
    public Object Data { get; set; }
}


public class ApiResult<T> : ApiResult
{
    /// <summary>
    /// 返回数据  
    /// </summary>
    public T Data { get; set; }

    public ApiResult()
    {
    }

    public ApiResult(bool success, string message) : base(success, message)
    {
    }

    public ApiResult(bool success, int errorCode, string message, object data) : base(success, errorCode, message, data)
    {
    }
}