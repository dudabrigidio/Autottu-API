namespace AutoTTU.Middleware
{
    public class ApiKeyMiddleware
    {
        private readonly RequestDelegate _next;
        private const string ApiKeyHeaderName = "X-API-Key";
        private readonly IConfiguration _configuration;
        
        // Rotas públicas que não precisam de API KEY
        private static readonly string[] PublicPaths = new[]
        {
            "/health",
            "/swagger",
            "/swagger/index.html",
            "/swagger/v1/swagger.json"
        };

        public ApiKeyMiddleware(RequestDelegate next, IConfiguration configuration)
        {
            _next = next;
            _configuration = configuration;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var path = context.Request.Path.Value?.ToLower() ?? string.Empty;
            
            // Verifica se a rota é pública (não precisa de API KEY)
            if (IsPublicPath(path))
            {
                await _next(context);
                return;
            }

            // Tenta pegar a chave do header
            if (!context.Request.Headers.TryGetValue(ApiKeyHeaderName, out var extractApiKey))
            {
                context.Response.StatusCode = 401;
                await context.Response.WriteAsync("API Key ausente");
                return;
            }

            var apiKey = _configuration.GetValue<string>("ApiSettings:ApiKey");

            // Validação de segurança: verifica se a API KEY está configurada
            if (string.IsNullOrWhiteSpace(apiKey))
            {
                context.Response.StatusCode = 500;
                await context.Response.WriteAsync("API Key não configurada no servidor");
                return;
            }

            // Comparação segura usando StringComparison.Ordinal para evitar timing attacks
            if (!string.Equals(apiKey, extractApiKey.ToString(), StringComparison.Ordinal))
            {
                context.Response.StatusCode = 401;
                await context.Response.WriteAsync("API Key inválida");
                return;
            }

            await _next(context);
        }

        private static bool IsPublicPath(string path)
        {
            return PublicPaths.Any(publicPath => path.StartsWith(publicPath, StringComparison.OrdinalIgnoreCase));
        }
    }
}
