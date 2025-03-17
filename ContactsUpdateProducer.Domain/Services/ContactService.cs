using FIAP.TechChallenge.ContactsUpdateProducer.Domain.Entities;
using FIAP.TechChallenge.ContactsUpdateProducer.Domain.Interfaces.Repositories;
using FIAP.TechChallenge.ContactsUpdateProducer.Domain.Interfaces.Services;
using Microsoft.Extensions.Logging;

namespace FIAP.TechChallenge.ContactsUpdateProducer.Domain.Services
{
    public class ContactService(IContactRepository contactRepository, ILogger<ContactService> logger) : IContactService
    {
        private readonly IContactRepository _contactRepository = contactRepository;
        private readonly ILogger<ContactService> _logger = logger;

        public async Task UpdateAsync(Contact contactUpdate)
        {
            try
            {
                await _contactRepository.UpdateAsync(contactUpdate);
            }
            catch (Exception)
            {
                var message = "Some error occour when trying to update a Contact.";
                _logger.LogError(message);
                throw new Exception(message);
            }
        }
    }
}