using FIAP.TechChallenge.ContactsUpdateProducer.Domain.Entities;
using FIAP.TechChallenge.ContactsUpdateProducer.Domain.Enumerators;
using FIAP.TechChallenge.ContactsUpdateProducer.Domain.Interfaces.Applications;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Text;

namespace FIAP.TechChallenge.ContactsUpdateProducer.Application.Applications
{
    public class TokenApplication(IConfiguration configuration) : ITokenApplication
    {
        private readonly IConfiguration _configuration = configuration;

        public string GetToken(User usuario)
        {
            int usuarioExiste = UserList.Users?.Any(u => u.Username == usuario.Username && u.Password == usuario.Password) ?? false ? 1 : 0;

            TypePermission typePermission;

            if (usuarioExiste == 0)
                return string.Empty;
            else
            {
                var user = UserList.Users?.FirstOrDefault(u => u.Username == usuario.Username && u.Password == usuario.Password);
                typePermission = user?.TypePermission ?? TypePermission.Admin;
            }

            var tokeHandler = new JwtSecurityTokenHandler();

            var chaveCriptografia = Encoding.ASCII.GetBytes(_configuration.GetValue<string>("SecretJWT") ?? string.Empty);

            var tokenPropriedades = new SecurityTokenDescriptor()
            {
                Subject = new System.Security.Claims.ClaimsIdentity(new[]
                {
                    new System.Security.Claims.Claim(System.Security.Claims.ClaimTypes.Name, usuario.Username ?? string.Empty),
                    new System.Security.Claims.Claim(System.Security.Claims.ClaimTypes.Role, typePermission.ToString())

                }),

                //tempo de expiração do token
                Expires = DateTime.UtcNow.AddHours(1),

                //chave de criptografia
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(chaveCriptografia),
                                                            SecurityAlgorithms.HmacSha256Signature)
            };

            //cria o token
            var token = tokeHandler.CreateToken(tokenPropriedades);

            //retorna o token criado
            return tokeHandler.WriteToken(token);
        }
    }
}
