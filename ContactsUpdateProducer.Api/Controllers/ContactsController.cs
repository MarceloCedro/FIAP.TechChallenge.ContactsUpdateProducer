using FIAP.TechChallenge.ContactsUpdateProducer.Api.Logging;
using FIAP.TechChallenge.ContactsUpdateProducer.Domain.DTOs.EntityDTOs;
using FIAP.TechChallenge.ContactsUpdateProducer.Domain.Interfaces.Applications;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FIAP.TechChallenge.ContactsUpdateProducer.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ContactsController(IContactApplication contactService, ILogger<ContactsController> logger) : ControllerBase
    {
        private readonly IContactApplication _contactService = contactService;
        private readonly ILogger<ContactsController> _logger = logger;

        /// <summary>
        /// Método para atualizar um contato de forma assíncrona.
        /// </summary>
        /// <param name="contact">Objeto com os dados do contato a ser atualizado
        /// em formato Json com os dados do contato:ID, Nome, DDD, Telefone e Email</param>
        /// <returns>Não retorna nenhum valor, atualiza os dados no banco</returns>
        [HttpPut]
        [Authorize(Roles = "Admin,User")]
        public async Task<IActionResult> UpdateContact(ContactUpdateDto contact)
        {
            _logger.LogInformation("Atualizando contato de ID {ID}", contact.Id);
            var updatedObject = await _contactService.UpdateContactAsync(contact);

            if (updatedObject.Success)
                return Ok(updatedObject.Message);
            else
                return BadRequest(updatedObject.Message);
        }
    }
}
