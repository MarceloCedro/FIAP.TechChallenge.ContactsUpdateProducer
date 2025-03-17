using FIAP.TechChallenge.ContactsUpdateProducer.Domain.Entities;
using FIAP.TechChallenge.ContactsUpdateProducer.Domain.Interfaces.Applications;
using Microsoft.AspNetCore.Mvc;

namespace FIAP.TechChallenge.ContactsUpdateProducer.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TokenController(ITokenApplication tokenService) : Controller
    {
        private readonly ITokenApplication _tokenService = tokenService;

        /// <summary>
        /// Metodo para fazer o login do usuario e retornar o token para a autenticação
        /// </summary>
        /// <remarks>
        /// Exemplo de Requisição: Não é ncessário informar o ID e o Tipo da Permissão
        /// {
        /// "username": "admin",
        /// "password": "admin"
        /// }
        /// ou
        /// {
        /// "username": "guest",
        /// "password": "guest"
        /// }
        /// </remarks>
        /// <param name="usuario">Realizar o Login com os usários admin,user ou guest</param>
        /// <returns>Irá retornar o token para realizar o login no Swagger</returns>
        /// <response code="200">Sucesso na execução - pode serguir com o Token</response>
        [HttpPost]
        public IActionResult GetToken([FromBody] User usuario)
        {
            var token = _tokenService.GetToken(usuario);

            if (string.IsNullOrWhiteSpace(token))
                return Unauthorized();

            return Ok(token);
        }

    }
}
