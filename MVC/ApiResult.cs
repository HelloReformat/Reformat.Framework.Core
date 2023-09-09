namespace Reformat.Framework.Core.MVC;

public class ApiResult
{
    public ApiResult()
    {
    }

    public ApiResult(int code, string message, object data)
    {
        this.code = code;
        this.message = message;
        this.data = data;
    }

    /// <summary>
    /// 错误编号 当为0时为正确结果
    /// </summary>
    public int code { get; set; }

    /// <summary>
    /// 运行结果
    /// </summary>
    public string message { get; set; }
        
    /// <summary>
    /// 返回数据  
    /// </summary>
    public Object data { get; set; }
}


public class ApiResult<T> : ApiResult
{
    /// <summary>
    /// 返回数据  
    /// </summary>
    public T data { get; set; }
}