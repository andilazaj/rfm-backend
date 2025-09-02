using System.Security.Claims;
using Microsoft.AspNetCore.Http;

namespace Rfm.Api.Auth; 

public static class HttpContextExtensions
{
    public static bool IsAdmin(this HttpContext? ctx)
        => ctx?.User?.IsInRole("Admin") == true;

    public static string? GetUserId(this HttpContext? ctx)
        => ctx?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value
           ?? ctx?.User?.FindFirst("sub")?.Value;
}
