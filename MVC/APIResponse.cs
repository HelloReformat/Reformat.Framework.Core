namespace Reformat.Framework.Core.MVC;

public class APIResponse<T>
{
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
    public T data { get; set; }
}