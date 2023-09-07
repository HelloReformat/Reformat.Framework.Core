using System.Text;

namespace Reformat.Framework.Core.Validator.Domains;

/// <summary>
/// 验证结果类
/// </summary>
public class ValidResult
{
    /// <summary>
    /// 错误成员列表
    /// </summary>
    public List<ErrorMember> ErrorMembers { get; set; } = new List<ErrorMember>();
    /// <summary>
    /// 验证结果
    /// </summary>
    public bool IsVaild { get; set; }

    public override string ToString()
    {
        StringBuilder sb = new StringBuilder();
        ErrorMembers.ForEach(item =>
        {
            sb.AppendLine(item.ErrorMemberName + " : " + item.ErrorMessage + ";");
        });
        return sb.ToString();
    }
}