using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace BookStore535.API.Areas.Identity.Controllers;

[Area(CD.IDENTITY_AREA)]
[Route("api/[area]/[controller]")]
[ApiController]
public sealed class AuthController(
    UserManager<CustomerAccount> users,
    SignInManager<CustomerAccount> signInManager) : ControllerBase
{
    [HttpPost("Register")]
    [ProducesResponseType(typeof(ApiResponse<UserResponse>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Register(RegisterRequest request)
    {
        var user = new CustomerAccount
        {
            Name = $"{request.FirstName.Trim()} {request.LastName.Trim()}",
            Email = request.Email.Trim(),
            UserName = request.Email.Trim(),
            PhoneNumber = request.PhoneNumber?.Trim(),
            Address = request.Address?.Trim(),
            CreatedUtc = DateTime.UtcNow
        };

        var result = await users.CreateAsync(user, request.Password);
        if (!result.Succeeded)
            return BadRequest(ApiResponseFactory.Failure<object>("Could not register the account.", result.Errors.Select(error => error.Description)));

        var roleResult = await users.AddToRoleAsync(user, AppRoles.Customer);
        if (!roleResult.Succeeded)
        {
            await users.DeleteAsync(user);
            return BadRequest(ApiResponseFactory.Failure<object>("Could not assign the customer role.", roleResult.Errors.Select(error => error.Description)));
        }

        var response = new UserResponse { Id = user.Id, Name = user.Name, Email = user.Email, PhoneNumber = user.PhoneNumber };
        return StatusCode(StatusCodes.Status201Created,
            ApiResponseFactory.Success(response, "Account created successfully."));
    }

    [HttpPost("Login")]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Login(LoginRequest request)
    {
        var user = await users.FindByEmailAsync(request.Email.Trim());
        if (user is null)
            return Unauthorized(ApiResponseFactory.Failure<object>("Invalid email or password."));

        var result = await signInManager.PasswordSignInAsync(user, request.Password, isPersistent: false, lockoutOnFailure: true);
        if (!result.Succeeded)
            return Unauthorized(ApiResponseFactory.Failure<object>("Invalid email or password."));

        return Ok(ApiResponseFactory.Success<object?>(null, "Login successful."));
    }

    [HttpPost("Logout")]
    public async Task<IActionResult> Logout()
    {
        await signInManager.SignOutAsync();
        return Ok(ApiResponseFactory.Success<object?>(null, "Logout successful."));
    }
}

