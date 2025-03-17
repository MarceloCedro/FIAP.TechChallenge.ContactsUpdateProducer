using FIAP.TechChallenge.ContactsUpdateProducer.Application.Applications;
using FIAP.TechChallenge.ContactsUpdateProducer.Domain.DTOs.EntityDTOs;
using FIAP.TechChallenge.ContactsUpdateProducer.Domain.Entities;
using FIAP.TechChallenge.ContactsUpdateProducer.Domain.Interfaces.Integrations;
using FIAP.TechChallenge.ContactsUpdateProducer.Domain.Interfaces.Services;
using MassTransit;
using MassTransit.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;

namespace FIAP.TechChallenge.ContactsUpdateProducer.UnitTest
{
    public class ContactApplicaitonTest
    {
        private readonly Mock<IContactService> _contactServiceMock;
        private readonly Mock<ILogger<ContactApplication>> _loggerMock;
        private readonly Mock<IContactConsultManager> _contactConsultManagerMock;
        private readonly Mock<IConfiguration> _configurationMock;
        private readonly Mock<IBus> _busMock;
        private readonly ContactApplication contactApplication;

        public ContactApplicaitonTest()
        {
            _contactServiceMock = new Mock<IContactService>();
            _loggerMock = new Mock<ILogger<ContactApplication>>();
            _contactConsultManagerMock = new Mock<IContactConsultManager>();
            _configurationMock = new Mock<IConfiguration>();
            _busMock = new Mock<IBus>();

            contactApplication = new ContactApplication(_contactServiceMock.Object,
                                                        _contactConsultManagerMock.Object,
                                                        _loggerMock.Object,
                                                        _configurationMock.Object,
                                                        _busMock.Object);
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

        private async Task SetupGetContactByEmailAsync(bool validReturn)
        {
            if (validReturn)
            {
                var contato = new Contact()
                {
                    Id = 1,
                    Name = "Marcelo Cedro",
                    Email = "marcel1234ocedro@gmail.com",
                    AreaCode = "11",
                    Phone = "982840611"
                };
                _contactConsultManagerMock.Setup(u => u.GetContactById(It.IsAny<int>(), It.IsAny<string>())).ReturnsAsync(contato);
            }
            else
                _contactConsultManagerMock.Setup(u => u.GetContactById(It.IsAny<int>(), It.IsAny<string>()));
        }

        [Fact]
        public async Task ValidateFakeTokenAsync()
        {
            await SetupTokenAsync(false);
            await SetupGetContactByEmailAsync(false);

            var retorno = await contactApplication.UpdateContactAsync(null);

            await VerifyTokenAsync(Times.Once());
            await VerifyGetContactByEmailAsync(Times.Never());
            Assert.False(retorno.Success);
            Assert.Equal("Houve um problema ao se obter o token para chamada externa.", retorno.Message);
        }

        [Fact]
        public async Task ValidateContactAlreadyExistsAsync()
        {
            await SetupTokenAsync(true);
            await SetupGetContactByEmailAsync(false);
            var contactDto = new ContactUpdateDto
            {
                Name = "Marcelo Cedro",
                Email = "marcel1234ocedro@gmail.com",
                AreaCode = "11",
                Phone = "982840611"
            };
            var retorno = await contactApplication.UpdateContactAsync(contactDto);

            await VerifyTokenAsync(Times.Once());
            await VerifyGetContactByEmailAsync(Times.Once());
            Assert.False(retorno.Success);
            Assert.Equal("Contato não encontrado na base para atualizacao.", retorno.Message);
        }

        [Fact]
        public async Task ValidateContactSendingContactExceptionAsync()
        {
            await SetupTokenAsync(true);
            await SetupGetContactByEmailAsync(true);
            SetupConfigurationAsync();
            await SetupBusAsync(true);
            var contactDto = new ContactUpdateDto
            {
                Name = "Marcelo Cedro",
                Email = "marcel1234ocedro@gmail.com",
                AreaCode = "11",
                Phone = "982840611"
            };
            var retorno = await contactApplication.UpdateContactAsync(contactDto);

            await VerifyTokenAsync(Times.Once());
            await VerifyGetContactByEmailAsync(Times.Once());
            await VerifyBusAsync(Times.Once());
            Assert.False(retorno.Success);
            Assert.Equal("Ocorreu um problema ao tentar inserir o registro na fila para atualizacao.", retorno.Message);
        }

        [Fact]
        public async Task ValidateContactSendingContactAsync()
        {
            await SetupTokenAsync(true);
            await SetupGetContactByEmailAsync(true);
            SetupConfigurationAsync();
            await SetupBusAsync();
            var contactDto = new ContactUpdateDto
            {
                Name = "Marcelo Cedro",
                Email = "marcel1234ocedro@gmail.com",
                AreaCode = "11",
                Phone = "982840611"
            };
            var retorno = await contactApplication.UpdateContactAsync(contactDto);

            await VerifyTokenAsync(Times.Once());
            await VerifyGetContactByEmailAsync(Times.Once());
            await VerifyBusAsync(Times.Once());
            Assert.True(retorno.Success);
            Assert.Equal("Contato inserido na FILA para atualizacao com sucesso.", retorno.Message);
        }

        private async Task VerifyTokenAsync(Times times)
        {
            _contactConsultManagerMock.Verify(u => u.GetToken(), times);
        }

        private async Task VerifyGetContactByEmailAsync(Times times)
        {
            _contactConsultManagerMock.Verify(u => u.GetContactById(It.IsAny<int>(), It.IsAny<string>()), times);
        }

        private async Task VerifyBusAsync(Times times)
        {
            _busMock.Verify(u => u.GetSendEndpoint(It.IsAny<Uri>()), times);
        }

        private void SetupConfigurationAsync()
        {
            var mockQueueName = new Mock<IConfigurationSection>();
            mockQueueName.Setup(x => x.Value).Returns("Teste");

            var mockServerName = new Mock<IConfigurationSection>();
            mockServerName.Setup(x => x.Value).Returns("localhost");

            var mockUserName = new Mock<IConfigurationSection>();
            mockUserName.Setup(x => x.Value).Returns("User");

            var mockPassword = new Mock<IConfigurationSection>();
            mockPassword.Setup(x => x.Value).Returns("Password");

            _configurationMock.Setup(x => x.GetSection("MassTransit:QueueName")).Returns(mockQueueName.Object);
            _configurationMock.Setup(x => x.GetSection("MassTransit:Server")).Returns(mockServerName.Object);
            _configurationMock.Setup(x => x.GetSection("MassTransit:User")).Returns(mockUserName.Object);
            _configurationMock.Setup(x => x.GetSection("MassTransit:Password")).Returns(mockPassword.Object);
        }
    }
}