using FIAP.TechChallenge.ContactsUpdateProducer.Domain.Interfaces.Services;
using FIAP.TechChallenge.ContactsUpdateProducer.Domain.Services;
using Microsoft.Extensions.DependencyInjection;

namespace FIAP.TechChallenge.ContactsUpdateProducer.Domain
{
    public static class ServicesDependency
    {
        public static IServiceCollection AddServicesDependency(this IServiceCollection service)
        {
            service.AddScoped<IContactService, ContactService>();

            return service;
        }
    }
}
