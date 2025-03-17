using FIAP.TechChallenge.ContactsUpdateProducer.Api.Controllers;
using FIAP.TechChallenge.ContactsUpdateProducer.Application.Applications;
using FIAP.TechChallenge.ContactsUpdateProducer.Domain.Interfaces.Applications;
using FIAP.TechChallenge.ContactsUpdateProducer.Domain.Interfaces.Integrations;
using FIAP.TechChallenge.ContactsUpdateProducer.Domain.Interfaces.Repositories;
using FIAP.TechChallenge.ContactsUpdateProducer.Domain.Interfaces.Services;
using FIAP.TechChallenge.ContactsUpdateProducer.Domain.Services;
using FIAP.TechChallenge.ContactsUpdateProducer.IntegrationTests.Config;
using MassTransit;
using MassTransit.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;

namespace FIAP.TechChallenge.ContactsConsult.IntegrationTest.Validations
{
    public class ContactControllerTest : BaseServiceTests
    {
        private readonly ContactsController _contactsController;
        private readonly IContactApplication _contactApplication;
        private readonly IContactService _contactService;
        private readonly IConfiguration configuration;
        private Mock<ILogger<ContactService>> _loggerServiceMock;
        private Mock<ILogger<ContactApplication>> _loggerApplicationMock;
        private Mock<ILogger<ContactsController>> _loggerControllerMock;
        private readonly IContactRepository _contactRepository;
        private readonly Mock<IContactConsultManager> _contactConsultManagerMock;
        private readonly Mock<IBus> _busMock;

        public readonly Random RandomId;

        public ContactControllerTest()
        {            
            _loggerServiceMock = new Mock<ILogger<ContactService>>();
            _loggerApplicationMock = new Mock<ILogger<ContactApplication>>();
            configuration = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory())
                                                      .AddJsonFile(
                                                           path: "appsettings.json",
                                                           optional: false,
                                                           reloadOnChange: true)
                                                      .Build();
            _loggerControllerMock = new Mock<ILogger<ContactsController>>();
            _contactService = new ContactService(_contactRepository, _loggerServiceMock.Object);
            _busMock = new Mock<IBus>();
            _contactConsultManagerMock = new Mock<IContactConsultManager>();
            _contactApplication = new ContactApplication(_contactService, 
                                                         _contactConsultManagerMock.Object,
                                                         _loggerApplicationMock.Object, 
                                                         configuration, 
                                                         _busMock.Object);
            
            _contactsController = new ContactsController(_contactApplication, _loggerControllerMock.Object);
            RandomId = new Random();
        }

        private async Task SetupTokenAsync(bool validReturn)
        {
            if (validReturn)
                _contactConsultManagerMock.Setup(u => u.GetToken()).ReturnsAsync("TOKEN-TEST");
            else
                _contactConsultManagerMock.Setup(u => u.GetToken());
        }

        private async Task SetupBusAsync(bool exception = false)
        {
            if (!exception)
            {
                await using var provider = new ServiceCollection()
                .AddMassTransitInMemoryTestHarness(cfg =>
                {
                })
                .BuildServiceProvider(true);

                var harness = provider.GetRequiredService<InMemoryTestHarness>();

                await harness.Start();
                var bus = provider.GetRequiredService<IBus>();
                var endpoint = await bus.GetSendEndpoint(new Uri($"queue:Teste"));

                _busMock.Setup(u => u.GetSendEndpoint(It.IsAny<Uri>())).ReturnsAsync(endpoint);
            }
            else
                _busMock.Setup(u => u.GetSendEndpoint(It.IsAny<Uri>())).ThrowsAsync(new Exception());
        }

        [Fact]
        public async Task InsertContactSuccessAsync()
        {
            await SetupTokenAsync(true);
            await SetupBusAsync();

            var contact = ContactFixtures.CreateFakeContactCreateDto();
            var insertResult = await _contactsController.UpdateContact (contact);
            Assert.NotNull(insertResult);
        }
    }
}
