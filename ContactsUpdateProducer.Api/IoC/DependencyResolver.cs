using FIAP.TechChallenge.ContactsUpdateProducer.Application;
using FIAP.TechChallenge.ContactsUpdateProducer.Domain;
using FIAP.TechChallenge.ContactsUpdateProducer.Infrastructure;

namespace FIAP.TechChallenge.ContactsUpdateProducer.Api.IoC
{
    public static class DependencyResolver
    {
        public static void AddDependencyResolver(this IServiceCollection services, string connectionString)
        {
            services.AddRepositoriesDependency();
            services.AddDbContextDependency(connectionString);
            services.AddServicesDependency();
            services.AddApplicationDependency();
            services.AddAuthenticationDependency();
            services.AddIntegrationsDependency();
        }
    }
}
