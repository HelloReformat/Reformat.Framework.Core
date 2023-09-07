namespace Reformat.Framework.Core.JWT.interfaces;

public interface IUserService
{
    public IUser GetUserByToken(string token);

    public Task<IUser> GetUserById(long id);
    
    public Task<List<IUser>> GetUserByIds(long[] ids = null);

    //public Task<List<long>> GetAllUserIds();
}