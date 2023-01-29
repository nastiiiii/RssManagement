using API.Enteties;

namespace API.Interfaces;

public interface ITokenService
{
    string CreateToken(User user);
}