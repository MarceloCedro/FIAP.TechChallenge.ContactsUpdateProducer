using FIAP.TechChallenge.ContactsUpdateProducer.Domain.Entities;

namespace FIAP.TechChallenge.ContactsUpdateProducer.Domain.Interfaces.Repositories
{
    public interface IContactRepository
    {
        Task UpdateAsync(Contact contact);
    }
}
