using Microsoft.AspNetCore.Mvc;
using PostsBlogApi.DTOs;
using Microsoft.AspNetCore.Identity;
using PostsBlogApi.Services;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

[Route("API/[controller]")]
[ApiController]
public class UsersController : ControllerBase
{
    private readonly UserManager<User> _userManager;
    private readonly SignInManager<User> _signInManager;

    public UsersController(UserManager<User> userManager, SignInManager<User> signInManager)
    {
        _userManager = userManager;
        _signInManager = signInManager;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterDto dto)
    {
        var user = new User
        {
            UserName = dto.UserName,
            FirstName = dto.FirstName,
            LastName = dto.LastName
        };

        var result = await _userManager.CreateAsync(user, dto.Password);

        if (!result.Succeeded)
            return BadRequest(result.Errors);

        return Ok(new { message = "User registered successfully" });
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginDto dto, [FromServices] JwtService jwtService)
    {
        var user = await _userManager.FindByNameAsync(dto.UserName);
        if (user == null)
            return Unauthorized(new { message = "Invalid credentials" });

        var result = await _signInManager.CheckPasswordSignInAsync(user, dto.Password, false);
        if (!result.Succeeded)
            return Unauthorized(new { message = "Invalid credentials" });

        var token = jwtService.GenerateToken(user);
        return Ok(new { token });
    }

    [Authorize]
    [HttpGet("me")]
    public async Task<IActionResult> GetProfile()
    {
        var userId = User?.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId))
            return Unauthorized(new { message = "No valid user ID in token" });
        var user = await _userManager.FindByNameAsync(userId);
        if (user == null)
            return NotFound(new { message = "User not found" });
        return Ok(new { user.UserName, user.Email, user.FirstName, user.LastName });
    }
}
