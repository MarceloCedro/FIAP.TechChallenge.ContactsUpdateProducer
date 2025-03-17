using FIAP.TechChallenge.ContactsUpdateProducer.Domain.Interfaces.Integrations;
using FIAP.TechChallenge.ContactsUpdateProducer.Integrations;
using Microsoft.Extensions.DependencyInjection;

namespace FIAP.TechChallenge.ContactsUpdateProducer.Infrastructure
{
    public static class IntegrationsDependency
    {
        public static IServiceCollection AddIntegrationsDependency(this IServiceCollection service)
        {
            service.AddScoped<IContactConsultManager, ContactConsultManager>();

            return service;
        }
    }
}
