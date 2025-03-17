using FIAP.TechChallenge.ContactsUpdateProducer.Domain.DTOs.Application;
using FIAP.TechChallenge.ContactsUpdateProducer.Domain.Entities;
using FIAP.TechChallenge.ContactsUpdateProducer.Domain.Interfaces.Integrations;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace FIAP.TechChallenge.ContactsUpdateProducer.Integrations
{
    public class ContactConsultManager(ILogger<ContactConsultManager> logger, IConfiguration configuration) : IContactConsultManager
    {
        private readonly ILogger<ContactConsultManager> _logger = logger;

        private readonly IConfiguration _configuration = configuration;

        public async Task<Contact> GetContactById(int id, string token)
        {
            try
            {
                var url = $"{_configuration.GetSection("Integrations:ContactConsult")["BasePath"]}api/Contacts/{id}";

                HttpClient cliente = new HttpClient();
                cliente.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");
                var resultado = await cliente.GetAsync(url);

                if (resultado != null && resultado.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    var responseString = JsonConvert.DeserializeObject<Contact>(await resultado.Content.ReadAsStringAsync());
                    return responseString;
                }
                else
                    return null;
            }
            catch (Exception e)
            {
                _logger.LogError($"Um erro aconteceu ao obter o Contato. Erro: {e.Message}.", e);
                return null; 
            }
        }

        public async Task<string> GetToken()
        {
            try
            {
                var url = _configuration.GetSection("Integrations:ContactConsult")["BasePath"]+"api/Token";
                var body = new CredentialDTO
                {
                    Username = _configuration.GetSection("Credentials")["Username"],
                    Password = _configuration.GetSection("Credentials")["Password"]
                };

                HttpClient cliente = new();
                var json = JsonConvert.SerializeObject(body);
                StringContent httpContent = new(json, System.Text.Encoding.UTF8, "application/json");

                var resultado = await cliente.PostAsync(url, httpContent);

                if (resultado != null && resultado.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    var responseString = await resultado.Content.ReadAsStringAsync();
                    return responseString;
                }
                else
                    return string.Empty;
            }
            catch (Exception e)
            {
                _logger.LogError($"Um erro aconteceu ao obter o token. Erro: {e.Message}.", e);
                return string.Empty;
            }
        }
    }
}
