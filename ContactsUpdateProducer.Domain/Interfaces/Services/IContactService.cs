using FIAP.TechChallenge.ContactsUpdateProducer.Domain.Entities;

namespace FIAP.TechChallenge.ContactsUpdateProducer.Domain.Interfaces.Services
{
    public interface IContactService
    {
        Task UpdateAsync(Contact contact);
    }
}