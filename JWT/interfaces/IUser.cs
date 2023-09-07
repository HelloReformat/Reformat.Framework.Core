namespace Reformat.Framework.Core.JWT.interfaces;

public interface IUser
{
    /// <summary>
    /// ID
    /// </summary>
    public int Id { get; set; }
    /// <summary>
    /// 姓名
    /// </summary>
    public string Name { get; set; }
    /// <summary>
    /// 手机
    /// </summary>
    public string phone { get; set; }
}