namespace Reformat.Framework.Core.Validator.Domains;


/// <summary>
/// 错误成员对象
/// </summary>
public class ErrorMember
{
    /// <summary>
    /// 错误信息
    /// </summary>
    public string? ErrorMessage { get; set; }
    
    /// <summary>
    /// 错误成员名称
    /// </summary>
    public string? ErrorMemberName { get; set; }

    public override string ToString()
    {
        return $"{nameof(ErrorMessage)}: {ErrorMessage}, {nameof(ErrorMemberName)}: {ErrorMemberName}";
    }
}