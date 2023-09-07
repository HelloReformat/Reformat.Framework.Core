namespace Reformat.Framework.Core.Common.Domain;

public class BatchResult
{
    /// <summary>
    /// 执行总数
    /// </summary>
    public int totle { get; set; } = 0;

    /// <summary>
    /// 成功条目
    /// </summary>
    public int successCount { get; set; } = 0;

    /// <summary>
    /// 成功条目
    /// </summary>
    public List<SuccessInfo> successMsg { get; set; } = new();

    /// <summary>
    /// 失败总数
    /// </summary>
    public int errorCount { get; set; } = 0;
    /// <summary>
    /// 失败信息
    /// </summary>
    public List<ErrorInfo> errorMsg { get; set; } = new();


    public void Success(string type,object info)
    {
        totle++;
        successCount++;
        successMsg.Add(new SuccessInfo(){type = type,info = info});
    }

    public void Error(string type,object info)
    {
        totle++;
        errorCount++;
        errorMsg.Add(new ErrorInfo(){type = type,info = info});
    }
}


public class SuccessInfo
{
    public string type { get; set; }
    public object info { get; set; }
}

public class ErrorInfo
{
    public string type { get; set; }
    public object info { get; set; }
}