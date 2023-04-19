namespace MinAPI.Auth.Interfaces
{
    public interface IUserRepository
    {
        UserDto GetUser(UserModel userModel);
    }
}
