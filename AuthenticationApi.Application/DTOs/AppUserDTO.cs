using System.ComponentModel.DataAnnotations;

namespace AuthenticationApi.Application.DTOs
{
    public record AppUserDTO(
        int Id,
        [Required] string UserName,
        [Required, DataType(DataType.EmailAddress)] string Email,
        [Required] string Address,
        [Required] string City,
        [Required] string Province,
        [Required] string ZipCode,
        [Required] string Country,
        [Required] string TelephoneNumber,
        [Required] string Password,
        [Required] string Role
        );
}
