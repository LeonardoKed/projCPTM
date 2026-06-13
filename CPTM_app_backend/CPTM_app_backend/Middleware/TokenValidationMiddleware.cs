using System.IdentityModel.Tokens.Jwt;

namespace CPTM_app_backend.Middleware
{
    public class TokenValidationMiddleware
    {
        private readonly RequestDelegate _next;

        public TokenValidationMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var authHeader = context.Request.Headers["Authorization"].FirstOrDefault();
            if (authHeader != null && authHeader.StartsWith("Bearer "))
            {
                var token = authHeader.Substring("Bearer ".Length);

                try
                {
                    var jwtHandler = new JwtSecurityTokenHandler();
                    if (jwtHandler.CanReadToken(token))
                    {
                        var jwtToken = jwtHandler.ReadJwtToken(token);
                        var usuarioId = jwtToken.Claims.FirstOrDefault(c => c.Type == "usuario_id")?.Value;

                        if (!string.IsNullOrEmpty(usuarioId))
                        {
                            context.Items["UsuarioId"] = usuarioId;
                        }
                    }
                }
                catch
                {
                    context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                    await context.Response.WriteAsync("Token inválido.");
                    return;
                }
            }

            await _next(context);
        }
    }
}
