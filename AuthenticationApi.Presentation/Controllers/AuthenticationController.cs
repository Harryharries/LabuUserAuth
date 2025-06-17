using Microsoft.AspNetCore.Mvc;
using AuthenticationApi.Application.Interfaces;
using AuthenticationApi.Application.DTOs;
using eCommerce.SharedLibrary.Responses;

namespace AuthenticationApi.Presentation.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthenticationController(IUser userInterface) : ControllerBase
    {
        [HttpPost("register")]
        public async Task<ActionResult<Response>> RegisterAsync(AppUserDTO userDTO)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var result = await userInterface.RegisterAsync(userDTO);
            return result.Flag ? Ok(result) : BadRequest(Request);
        }

        [HttpPost("login")]
        public async Task<IActionResult> LoginAsync(LoginDTO loginDTO)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var result = await userInterface.LoginAsync(loginDTO);
            return result.Flag ? Ok(result) : BadRequest(Request);
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<GetUserDTO>> GetUserAsync(int id)
        {
            if (id <= 0)
            {
                return BadRequest("Invalid id");
            }
            var result = await userInterface.GetAppUserAsync(id);
            return result.Id > 0 ? Ok(result) : NotFound();
        }
    }
}
