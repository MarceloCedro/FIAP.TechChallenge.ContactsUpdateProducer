using FIAP.TechChallenge.ContactsUpdateProducer.Domain.Entities;

namespace FIAP.TechChallenge.ContactsUpdateProducer.Domain.Interfaces.Applications
{
    public interface ITokenApplication
    {
        public string GetToken(User usuario);
    }
}
