using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BookStore535.API.Areas.Admin.Controllers;

[Area(CD.ADMIN_AREA)]
[Route("api/[area]/[controller]")]
[ApiController]
[Authorize(Roles = AppRoles.AdminAccess)]
public sealed class DiscountCodesController(IRepository<DiscountCode> discountCodes) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        var items = await discountCodes.GetAllAsync(isTracked: false, cancellationToken: cancellationToken);
        return Ok(ApiResponseFactory.Success<IReadOnlyList<DiscountCodeResponse>>(
            items.OrderBy(item => item.Code).Select(ToResponse).ToList(), "Discount codes returned successfully."));
    }

    [HttpPost]
    public async Task<IActionResult> Create(DiscountCodeRequest request, CancellationToken cancellationToken)
    {
        var code = Normalize(request.Code);
        if (request.ExpiresUtc.HasValue && request.StartsUtc.HasValue && request.ExpiresUtc <= request.StartsUtc)
            return BadRequest(ApiResponseFactory.Failure<object>("Expiry must be after the start date."));
        if (await discountCodes.AnyAsync(item => item.Code == code, cancellationToken))
            return Conflict(ApiResponseFactory.Failure<object>("Discount code already exists."));
        var item = request.Adapt<DiscountCode>();
        item.Code = code;
        await discountCodes.AddAsync(item, cancellationToken);
        await discountCodes.CommitAsync(cancellationToken);
        return StatusCode(StatusCodes.Status201Created,
            ApiResponseFactory.Success(ToResponse(item), "Discount code created successfully."));
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, DiscountCodeRequest request, CancellationToken cancellationToken)
    {
        var item = await discountCodes.GetOneAsync(code => code.Id == id, cancellationToken: cancellationToken);
        if (item is null) return NotFound(ApiResponseFactory.Failure<object>("Discount code not found."));
        var code = Normalize(request.Code);
        if (request.ExpiresUtc.HasValue && request.StartsUtc.HasValue && request.ExpiresUtc <= request.StartsUtc)
            return BadRequest(ApiResponseFactory.Failure<object>("Expiry must be after the start date."));
        if (await discountCodes.AnyAsync(existing => existing.Id != id && existing.Code == code, cancellationToken))
            return Conflict(ApiResponseFactory.Failure<object>("Discount code already exists."));
        request.Adapt(item);
        item.Code = code;
        await discountCodes.CommitAsync(cancellationToken);
        return NoContent();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        var item = await discountCodes.GetOneAsync(code => code.Id == id, cancellationToken: cancellationToken);
        if (item is null) return NotFound(ApiResponseFactory.Failure<object>("Discount code not found."));
        discountCodes.Remove(item);
        await discountCodes.CommitAsync(cancellationToken);
        return NoContent();
    }

    private static string Normalize(string code) => code.Trim().ToUpperInvariant();

    private static DiscountCodeResponse ToResponse(DiscountCode item) => new()
    {
        Id = item.Id,
        Code = item.Code,
        PercentOff = item.PercentOff,
        MinimumOrderAmount = item.MinimumOrderAmount,
        StartsUtc = item.StartsUtc,
        ExpiresUtc = item.ExpiresUtc,
        MaxUses = item.MaxUses,
        UsesCount = item.UsesCount,
        IsActive = item.IsActive
    };
}
