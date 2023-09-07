using Reformat.Framework.Core.JWT.interfaces;

namespace Reformat.Framework.Core.JWT.Domain;

public class DebugUser: IUser
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string phone { get; set; }

    public static DebugUser GetDebugUser()
    {
        return new DebugUser()
        {
            Id = -1,
            Name = "dev",
            phone = "18688888888"
        };
    }
}


