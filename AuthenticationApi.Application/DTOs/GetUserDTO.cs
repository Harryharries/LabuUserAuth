using System.ComponentModel.DataAnnotations;

namespace AuthenticationApi.Application.DTOs
{
    public record GetUserDTO(
        int Id,
        [Required] string UserName,
        [Required, DataType(DataType.EmailAddress)] string Email,
        [Required] string Address,
        [Required] string City,
        [Required] string Province,
        [Required] string ZipCode,
        [Required] string Country,
        [Required] string PhoneNumber
        );
}
