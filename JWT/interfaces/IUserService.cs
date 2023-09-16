using Reformat.Framework.Core.Aspects;

namespace Reformat.Framework.Core.JWT.interfaces;

public interface IUserService
{
    public IUser GetCurrentUser();

    public bool CheckPermissions(string permissions);

    //public Task<List<IUser>> GetUserByIds(long[] ids = null);
}