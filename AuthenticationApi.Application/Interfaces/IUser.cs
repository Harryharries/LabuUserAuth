using AuthenticationApi.Application.DTOs;
using eCommerce.SharedLibrary.Responses;
namespace AuthenticationApi.Application.Interfaces
{
    public interface IUser
    {
        Task<Response> RegisterAsync(AppUserDTO userDTO);
        Task<Response> LoginAsync(LoginDTO loginDTO);
        Task<GetUserDTO> GetAppUserAsync(int id);
    }
}
