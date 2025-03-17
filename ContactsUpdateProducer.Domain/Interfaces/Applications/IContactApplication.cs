using FIAP.TechChallenge.ContactsUpdateProducer.Domain.DTOs.Application;
using FIAP.TechChallenge.ContactsUpdateProducer.Domain.DTOs.EntityDTOs;

namespace FIAP.TechChallenge.ContactsUpdateProducer.Domain.Interfaces.Applications
{
    public interface IContactApplication
    {
        Task<UpsertContactResponse> UpdateContactAsync(ContactUpdateDto contactUpdateDto);
    }
}