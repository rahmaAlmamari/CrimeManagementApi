using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;

namespace CrimeManagementApi.Middleware
{
    /// <summary>
    /// Restricts number of comment posts per user per minute.
    /// </summary>
    public class RateLimitingMiddleware : IMiddleware
    {
        private readonly IMemoryCache _cache;
        private const int MAX_COMMENTS_PER_MINUTE = 5;

        public RateLimitingMiddleware(IMemoryCache cache) => _cache = cache;

        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            if (context.Request.Path.StartsWithSegments("/api/comment", StringComparison.OrdinalIgnoreCase)
                && context.Request.Method == HttpMethods.Post)
            {
                string key = $"RateLimit_{context.Connection.RemoteIpAddress}";
                if (_cache.TryGetValue(key, out int count))
                {
                    if (count >= MAX_COMMENTS_PER_MINUTE)
                    {
                        context.Response.StatusCode = StatusCodes.Status429TooManyRequests;
                        await context.Response.WriteAsJsonAsync(new
                        {
                            message = "Rate limit exceeded. Maximum 5 comments per minute."
                        });
                        return;
                    }
                    _cache.Set(key, count + 1, TimeSpan.FromMinutes(1));
                }
                else
                {
                    _cache.Set(key, 1, TimeSpan.FromMinutes(1));
                }
            }
            await next(context);
        }
    }
}
