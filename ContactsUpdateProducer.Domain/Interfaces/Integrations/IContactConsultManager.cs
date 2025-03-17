using FIAP.TechChallenge.ContactsUpdateProducer.Domain.Entities;

namespace FIAP.TechChallenge.ContactsUpdateProducer.Domain.Interfaces.Integrations
{
    public interface IContactConsultManager
    {
        Task<string> GetToken();
        Task<Contact> GetContactById(int id, string token);
    }
}
