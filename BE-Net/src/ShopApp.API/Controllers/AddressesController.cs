using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShopApp.API.Auth;
using ShopApp.Application.Users.Commands.AddAddress;
using ShopApp.Application.Users.Commands.DeleteAddress;
using ShopApp.Application.Users.Commands.SetDefaultAddress;
using ShopApp.Application.Users.Commands.UpdateAddress;
using ShopApp.Application.Users.Queries.GetAddressesByUserId;

namespace ShopApp.API.Controllers;

[ApiController]
[Route("api/users/{userId:guid}/addresses")]
public sealed class AddressesController(ISender sender) : ControllerBase
{
    [HttpGet]
    [Authorize(Policy = AuthorizationPolicies.CustomerOrAdmin)]
    public async Task<IActionResult> GetAll(Guid userId, CancellationToken ct)
    {
        if (!User.CanAccessUser(userId))
            return Forbid();

        var result = await sender.Send(new GetAddressesByUserIdQuery(userId), ct);
        return Ok(result);
    }

    [HttpPost]
    [Authorize(Policy = AuthorizationPolicies.CustomerOrAdmin)]
    public async Task<IActionResult> Add(Guid userId, [FromBody] AddAddressRequest request, CancellationToken ct)
    {
        if (!User.CanAccessUser(userId))
            return Forbid();

        var result = await sender.Send(new AddAddressCommand(
            userId,
            request.FullName,
            request.PhoneNumber,
            request.AddressLine,
            request.City,
            request.Province,
            request.Country,
            request.PostalCode), ct);
        return Ok(result);
    }

    [HttpPut("{addressId:guid}")]
    [Authorize(Policy = AuthorizationPolicies.CustomerOrAdmin)]
    public async Task<IActionResult> Update(Guid userId, Guid addressId,
        [FromBody] UpdateAddressRequest request, CancellationToken ct)
    {
        if (!User.CanAccessUser(userId))
            return Forbid();

        var result = await sender.Send(new UpdateAddressCommand(
            addressId,
            request.FullName,
            request.PhoneNumber,
            request.AddressLine,
            request.City,
            request.Province,
            request.Country,
            request.PostalCode), ct);
        return Ok(result);
    }

    [HttpPatch("{addressId:guid}/set-default")]
    [Authorize(Policy = AuthorizationPolicies.CustomerOrAdmin)]
    public async Task<IActionResult> SetDefault(Guid userId, Guid addressId, CancellationToken ct)
    {
        if (!User.CanAccessUser(userId))
            return Forbid();

        var result = await sender.Send(new SetDefaultAddressCommand(userId, addressId), ct);
        return Ok(result);
    }

    [HttpDelete("{addressId:guid}")]
    [Authorize(Policy = AuthorizationPolicies.CustomerOrAdmin)]
    public async Task<IActionResult> Delete(Guid userId, Guid addressId, CancellationToken ct)
    {
        if (!User.CanAccessUser(userId))
            return Forbid();

        await sender.Send(new DeleteAddressCommand(addressId), ct);
        return NoContent();
    }
}

public record AddAddressRequest(
    string FullName,
    string PhoneNumber,
    string AddressLine,
    string City,
    string Province,
    string Country,
    string? PostalCode = null);

public record UpdateAddressRequest(
    string FullName,
    string PhoneNumber,
    string AddressLine,
    string City,
    string Province,
    string Country,
    string? PostalCode = null);
