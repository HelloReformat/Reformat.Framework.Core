namespace Reformat.Framework.Core.JWT.interfaces;

public interface IUserService
{
    public IUser GetCurrentUser();

    //public Task<List<IUser>> GetUserByIds(long[] ids = null);
}