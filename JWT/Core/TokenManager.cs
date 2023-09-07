using Microsoft.AspNetCore.Http;
using Reformat.Framework.Core.IOC.Attributes;
using Reformat.Framework.Core.Common.Extensions.lang;

namespace Reformat.Framework.Core.JWT.Core;

[ScopedService]
public class TokenManager
{
    private IHttpContextAccessor _accessor;

    public TokenManager(IHttpContextAccessor _accessor)
    {
        this._accessor = _accessor;
    }

    public string GetAuthorization(bool bearerToken = true)
    {
        var value = _accessor.HttpContext.Request.Headers["Authorization"].FirstOrDefault();
        // remove "Bearer " prefix
        return bearerToken ? value : value.Substring(7);
    }

    public string GetToken()
    {
        string token = "";
#if DEBUG
        if (token.IsNullOrEmpty()) return "dev";
#endif
        return _accessor.HttpContext.Request.Headers["token"].FirstOrDefault("");
    }
}