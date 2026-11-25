using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace BookStore535.API.Areas.Identity.Controllers;

[Area(CD.IDENTITY_AREA)]
[Route("api/[area]/[controller]")]
[ApiController]
[Authorize]
public sealed class ProfileController(UserManager<CustomerAccount> users) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<UserResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetData()
    {
        var user = await users.GetUserAsync(User);
        if (user is null) return NotFound(ApiResponseFactory.Failure<object>("User not found."));
        return Ok(ApiResponseFactory.Success(ToResponse(user), "Profile returned successfully."));
    }

    [HttpPut("UpdateProfile")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateProfile(UpdateProfileRequest request)
    {
        var user = await users.GetUserAsync(User);
        if (user is null) return NotFound(ApiResponseFactory.Failure<object>("User not found."));

        user.Name = request.Name.Trim();
        user.PhoneNumber = request.PhoneNumber?.Trim();
        var result = await users.UpdateAsync(user);
        if (!result.Succeeded)
            return BadRequest(ApiResponseFactory.Failure<object>("Could not update the profile.", result.Errors.Select(error => error.Description)));
        return NoContent();
    }

    private static UserResponse ToResponse(CustomerAccount user) => new()
    {
        Id = user.Id,
        Name = user.Name,
        Email = user.Email,
        PhoneNumber = user.PhoneNumber
    };
}

