using FIAP.TechChallenge.ContactsUpdateProducer.Domain.DTOs.EntityDTOs;
using FIAP.TechChallenge.ContactsUpdateProducer.IntegrationTests.Config;
using System.ComponentModel.DataAnnotations;

namespace FIAP.TechChallenge.ContactsUpdateProducer.IntegrationTests.Validations
{
    public class BaseContactDtoValidation
    {
        public readonly Random RandomId;
        public BaseContactDtoValidation()
        {
            RandomId = new Random();
        }

        public static List<ValidationResult> ValidateModel(object model)
        {
            var validationResults = new List<ValidationResult>();
            var context = new ValidationContext(model, null, null);
            Validator.TryValidateObject(model, context, validationResults, true);

            return validationResults;
        }

        [Fact]
        public void ContractDtoValidationFormatSuccess()
        {
            var basicContact = ContactFixtures.CreateFakeContactDto(RandomId.Next(999999999));

            var validationResults = ValidateModel(basicContact);

            Assert.Empty(validationResults);
        }

        public static IEnumerable<object[]> GetBasicContactInvalidData()
        {
            yield return new object[] { ContactFixtures.CreateContractDtoInvalidName() };
            yield return new object[] { ContactFixtures.CreateContractDtoInvalidEmail() };
            yield return new object[] { ContactFixtures.CreateContractDtoInvalidPhoneNumber() };
            yield return new object[] { ContactFixtures.CreateContractDtoInvalidAreaCode() };
        }

        [Theory]
        [MemberData(nameof(GetBasicContactInvalidData))]
        public void ContractDtoValidationEachDataFaill(ContactDto basicContact)
        {
            var validationResults = ValidateModel(basicContact);

            Assert.NotEmpty(validationResults);
        }
    }
}
