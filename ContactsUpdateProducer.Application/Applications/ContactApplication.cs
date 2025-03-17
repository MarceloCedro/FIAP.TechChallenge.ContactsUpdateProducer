using FIAP.TechChallenge.ContactsUpdateProducer.Domain.DTOs.Application;
using FIAP.TechChallenge.ContactsUpdateProducer.Domain.DTOs.EntityDTOs;
using FIAP.TechChallenge.ContactsUpdateProducer.Domain.Entities;
using FIAP.TechChallenge.ContactsUpdateProducer.Domain.Interfaces.Applications;
using FIAP.TechChallenge.ContactsUpdateProducer.Domain.Interfaces.Integrations;
using FIAP.TechChallenge.ContactsUpdateProducer.Domain.Interfaces.Services;
using MassTransit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace FIAP.TechChallenge.ContactsUpdateProducer.Application.Applications
{
    public class ContactApplication(IContactService contactService,
                                    IContactConsultManager contactConsultManager,
                                    ILogger<ContactApplication> logger,
                                    IConfiguration configuration,
                                    IBus bus) : IContactApplication
    {
        private readonly IContactService _contactService = contactService;
        private readonly ILogger<ContactApplication> _logger = logger;
        private readonly IContactConsultManager _contactConsultManager = contactConsultManager;
        private readonly IBus _bus = bus;
        private readonly IConfiguration _configuration = configuration;

        public async Task<UpsertContactResponse> UpdateContactAsync(ContactUpdateDto contactUpdateDto)
        {
            var insertResult = new UpsertContactResponse();
            var token = await _contactConsultManager.GetToken();
            if (token == null)
            {
                insertResult.Success = false;
                insertResult.Message = $"Houve um problema ao se obter o token para chamada externa.";
            }
            else
            {
                var contactObject = await _contactConsultManager.GetContactById(contactUpdateDto.Id, token);
                                
                if (contactObject == null)
                {
                    insertResult.Success = false;
                    insertResult.Message = $"Contato não encontrado na base para atualizacao.";
                }
                else
                {
                    try
                    {
                        var massTransitObject = ChargeMassTransitObject();

                        var endpoint = await _bus.GetSendEndpoint(new Uri($"queue:{massTransitObject.QueueName}"));

                        await endpoint.Send<ContactDto>(new ContactDto
                        {
                            Id = contactUpdateDto.Id,
                            Name = contactUpdateDto.Name,
                            Email = contactUpdateDto.Email,
                            AreaCode = contactUpdateDto.AreaCode,
                            Phone = contactUpdateDto.Phone
                        });

                        insertResult.Success = true;
                        insertResult.Message = "Contato inserido na FILA para atualizacao com sucesso.";
                    }
                    catch (Exception e)
                    {
                        insertResult.Success = false;
                        insertResult.Message = $"Ocorreu um problema ao tentar inserir o registro na fila para atualizacao.";
                        _logger.LogError(insertResult.Message, e);
                    }
                }
            }

            return insertResult;
        }

        private MassTransitDTO ChargeMassTransitObject()
        {
            return new MassTransitDTO()
            {
                QueueName = _configuration.GetSection("MassTransit:QueueName").Value ?? string.Empty,

                Server = _configuration.GetSection("MassTransit:Server").Value ?? string.Empty,

                User = _configuration.GetSection("MassTransit:User").Value ?? string.Empty,

                Password = _configuration.GetSection("MassTransit:Password").Value ?? string.Empty
            };
        }
    }
}