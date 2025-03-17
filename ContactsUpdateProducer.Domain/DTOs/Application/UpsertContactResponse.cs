using System.Diagnostics.CodeAnalysis;

namespace FIAP.TechChallenge.ContactsUpdateProducer.Domain.DTOs.Application
{
    [ExcludeFromCodeCoverage]
    public class UpsertContactResponse
    {
        public bool Success { get; set; }

        public string Message { get; set; }
    }
}
