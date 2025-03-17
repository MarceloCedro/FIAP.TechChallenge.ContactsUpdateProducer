using FIAP.TechChallenge.ContactsUpdateProducer.Domain.Entities;
using FIAP.TechChallenge.ContactsUpdateProducer.Domain.Enumerators;

namespace FIAP.TechChallenge.ContactsUpdateProducer.Api.Middleware
{
    public class UserListMiddleware(RequestDelegate next)
    {
        private readonly RequestDelegate _next = next;

        public Task Invoke(HttpContext httpContext)
        {
            UserList.Users = [
                                 new User { Id = 1, Username = "admin", Password = "admin", TypePermission = TypePermission.Admin },
                                 new User { Id = 2, Username = "user", Password = "user", TypePermission = TypePermission.User },
                                 new User { Id = 3, Username = "guest", Password = "guest", TypePermission = TypePermission.Guest }
                             ];

            return _next(httpContext);
        }
    }

    public static class UserListMiddlewareExtensions
    {
        public static IApplicationBuilder UseListaUserMiddleware(this IApplicationBuilder builder)
            => builder.UseMiddleware<UserListMiddleware>();
    }
}
