namespace MinAPI.Auth.Interfaces
{
    public interface ITokenService
    {
        string BuildToken(string key, string issuer, UserDto user);
    }
}
