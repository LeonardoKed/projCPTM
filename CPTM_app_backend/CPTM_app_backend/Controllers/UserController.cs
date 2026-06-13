using CPTM_app_backend.Entities;
using CPTM_app_backend.Results;
using CPTM_app_backend.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace CPTM_app_backend.Controllers
{
    [Route("api/")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly ServiceUser _services;
        private readonly string _key;
        public UserController(ServiceUser services, IConfiguration configuration)
        {
            Console.WriteLine("Entrei no sistema");
            _services = services;
            _key = configuration["JwtSettings:SecretKey"];
        }

        //Login Usuario

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest loginRequest)
        {
            if (string.IsNullOrWhiteSpace(loginRequest.Email) || string.IsNullOrWhiteSpace(loginRequest.Senha))
            {
                return BadRequest(new LoginResults
                {
                    Sucesso = false,
                    Mensagem = "E-mail e senha são obrigatórios."
                });
            }

            var usuario = _services.Login(loginRequest.Email, loginRequest.Senha);

            if (usuario != null)
            {
                // Gerar o token JWT
                var tokenHandler = new JwtSecurityTokenHandler();
                var tokenDescriptor = new SecurityTokenDescriptor
                {
                    Subject = new ClaimsIdentity(new[]
                    {
                        new Claim(ClaimTypes.Name, usuario.Nome),
                        new Claim(ClaimTypes.Email, usuario.Email),
                        new Claim("usuario_id", usuario.Id.ToString()),// Adiciona o ID do usuário como claim
                        new Claim(ClaimTypes.Role, usuario.Perfil) // Exemplo de claim de função, se necessário
                    }),

                    Expires = DateTime.UtcNow.AddDays(30), // Token válido por 30 dias
                    SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_key)), SecurityAlgorithms.HmacSha256Signature)
                };

                var token = tokenHandler.CreateToken(tokenDescriptor);
                var tokenString = tokenHandler.WriteToken(token);

                return Ok(new LoginResults
                {
                    Sucesso = true,
                    Mensagem = $"Seja bem-vindo, {usuario.Nome}.",
                    Token = tokenString
                });
            }

            return Unauthorized(new LoginResults
            {
                Sucesso = false,
                Mensagem = "Credenciais inválidas."
            });
        }

        //Enviar Formulario
        [HttpPost("enviarForm")]
        [Authorize]
        public async Task<IActionResult> EnviarForm([FromForm] Formulario formulario)
        {
            if (formulario == null)
            {
                return BadRequest("O formulário não pode ser nulo.");
            }

            // Recupera o UsuarioId do token JWT
            var usuarioIdClaim = User.Claims.FirstOrDefault(c => c.Type == "usuario_id");
            if (usuarioIdClaim == null)
            {
                return Unauthorized("Usuário não autenticado.");
            }

            for (int i = 1; i <= 4; i++)
            {
                var file = Request.Form.Files[$"Fotografia{i}"];

                if (file != null && file.Length > 0)
                {
                    using var ms = new MemoryStream();
                    await file.CopyToAsync(ms);

                    formulario.GetType()
                              .GetProperty($"Fotografia{i}")
                              .SetValue(formulario, ms.ToArray());
                }
            }
            

            formulario.usuario_id = int.Parse(usuarioIdClaim.Value); // Certifique-se que o campo existe no Formulario

            await _services.EnviarFormulario(formulario);


            return Ok(new LoginResults
            {
                Sucesso = true,
                Mensagem = "Formulario enviado com sucesso",

            });
        }

        /// <summary>
        /// Busca formulários com filtros e paginação para administradores
        /// GET /api/formulario/buscar?nomeContratada=ACME&pagina=1&itensPorPagina=10
        /// </summary>
        [HttpGet("buscar")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> BuscarPaginado(
            [FromQuery] FiltroFormularioRequest filtro,
            [FromQuery] int pagina = 1,
            [FromQuery] int itensPorPagina = 10)
        {
            try
            {
                var resultado = await _services.BuscarPaginadoAsync(filtro, pagina, itensPorPagina);

                return Ok(resultado);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Erro ao buscar formulários: {ex.Message}");
                return StatusCode(500, new ApiResponse<object>
                {
                    Sucesso = false,
                    Mensagem = "Erro ao buscar formulários"
                });
            }
        }

        //Atualizar Formulario
        /// <summary>
        /// Editar formulário por id
        /// GET /api/formulario/edit/{id}
        /// </summary>
        [HttpGet]
        [Route("form/{id}")]
        [Authorize]
        public async Task<IActionResult> EditarFormularioById([FromRoute] int id)
        {
            try
            {
                var usuarioIdClaim = User.FindFirst("usuario_id")?.Value;
                if (string.IsNullOrEmpty(usuarioIdClaim))
                    return Unauthorized(new ApiResponse<object> { Sucesso = false, Mensagem = "Usuário não autenticado" });

                if (!int.TryParse(usuarioIdClaim, out var usuarioId))
                    return Unauthorized(new ApiResponse<object> { Sucesso = false, Mensagem = "Claim de usuário inválido" });

                var resultado = await _services.GetFormularioById(id, usuarioId);
                if (resultado == null)
                    return NotFound(new ApiResponse<object> { Sucesso = false, Mensagem = "Formulário não encontrado" });

                return Ok(resultado);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Erro ao editar formulário: {ex.Message}");
                return StatusCode(500, new ApiResponse<object> { Sucesso = false, Mensagem = "Erro ao editar formulário" });
            }
        }

        [HttpGet]
        [Route("form/{id}/foto")]
        [Authorize]
        public async Task<IActionResult> GetFotoFormularioById([FromRoute] int id, [FromQuery] int index)
        {
            try
            {
                var usuarioIdClaim = User.FindFirst("usuario_id")?.Value;
                if (string.IsNullOrEmpty(usuarioIdClaim))
                    return Unauthorized(new ApiResponse<object> { Sucesso = false, Mensagem = "Usuário não autenticado" });
                if (!int.TryParse(usuarioIdClaim, out var usuarioId))
                    return Unauthorized(new ApiResponse<object> { Sucesso = false, Mensagem = "Claim de usuário inválido" });
                var formulario = await _services.GetFotoBytesAsync(id, usuarioId,index);

                if (formulario == null || formulario.Length == 0)
                    return NotFound(new ApiResponse<object> { Sucesso = false, Mensagem = "Foto não encontrada" });

                var mimeType = DetectImageMimeType(formulario);
                return File(formulario, mimeType);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Erro ao obter foto do formulário: {ex.Message}");
                return StatusCode(500, new ApiResponse<object> { Sucesso = false, Mensagem = "Erro ao obter foto do formulário" });
            }
        }

        // helper para detectar MIME por magic bytes
        private static string DetectImageMimeType(byte[] bytes)
        {
            if (bytes == null || bytes.Length < 4) return "application/octet-stream";
            if (bytes[0] == 0xFF && bytes[1] == 0xD8) return "image/jpeg";
            if (bytes[0] == 0x89 && bytes[1] == 0x50 && bytes[2] == 0x4E && bytes[3] == 0x47) return "image/png";
            if (bytes[0] == 0x47 && bytes[1] == 0x49 && bytes[2] == 0x46) return "image/gif";
            if (bytes.Length >= 12 &&
                bytes[0] == 0x52 && bytes[1] == 0x49 && bytes[2] == 0x46 && bytes[3] == 0x46 &&
                bytes[8] == 0x57 && bytes[9] == 0x45 && bytes[10] == 0x42 && bytes[11] == 0x50) return "image/webp";
            if (bytes[0] == 0x42 && bytes[1] == 0x4D) return "image/bmp";
            return "application/octet-stream";
        }




        //Listar Formulario do Usuario
        /// <summary>
        /// Busca formulários do usuário logado
        /// GET /api/formulario/meus?pagina=1
        /// </summary>
        [HttpGet("meus")]
        [Authorize]
        public async Task<IActionResult> BuscarMeusFormularios(
            [FromQuery] FiltroFormularioRequest filtro,
            [FromQuery] int pagina = 1,
            [FromQuery] int itensPorPagina = 10)
        {
            try
            {
                // Pega o ID do usuário do token
                var usuarioIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                                  ?? User.FindFirst("usuario_id")?.Value;

                if (string.IsNullOrEmpty(usuarioIdClaim))
                    return Unauthorized(new ApiResponse<object>
                    {
                        Sucesso = false,
                        Mensagem = "Usuário não autenticado"
                    });

                var usuarioId = int.Parse(usuarioIdClaim);

                // Se nenhum filtro foi enviado, inicializa
                filtro ??= new FiltroFormularioRequest();

                // Segurança: força o filtro para o usuário logado (não permite consultar outros usuários)
                filtro.UsuarioId = usuarioId;

                var resultado = await _services.BuscarPaginadoAsync(filtro, pagina, itensPorPagina);

                return Ok(resultado);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Erro ao buscar formulários do usuário: {ex.Message}");
                return StatusCode(500, new ApiResponse<object>
                {
                    Sucesso = false,
                    Mensagem = "Erro ao buscar formulários"
                });
            }
        }

        //Painel de configurações do usuario e adm
        /// <summary>
        /// Página de configurações do usuario
        /// GET /api/perfil
        /// </summary>
        [HttpGet]
        [Authorize]
        [Route("perfil")]
        public async Task<IActionResult> PaginaPerfil()
        {
            // Pega o ID do usuário do token
            var usuarioIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value?? User.FindFirst("usuario_id")?.Value;

            if (string.IsNullOrEmpty(usuarioIdClaim))
                return Unauthorized(new ApiResponse<object>
                {
                    Sucesso = false,
                    Mensagem = "Usuário não autenticado"
                });

            var usuarioId = int.Parse(usuarioIdClaim);

            var resultado = await _services.ObterPerfilUsuarioAsync(usuarioId);

            return Ok(resultado);
        }


        //Editar Usuario por id
        [HttpPatch]
        [Route("edituser/{id}")]
        [Authorize]
        public async Task<IActionResult> EditarUsuarioById([FromRoute] int id, [FromBody] AdminEditUser adminEditUser)
        {
            if (adminEditUser == null) return BadRequest();

            var sucesso = await _services.EditarUsuarioById(id, adminEditUser);

            if (!sucesso) return NotFound("Não encontrado");

            return Ok(sucesso);
        }


        //Atualizar formulario
        [HttpPatch]
        [Route("updateform/{id}")]
        [Authorize]
        public async Task<IActionResult> AtualizarFormulario([FromRoute] int id, [FromForm] Formulario formularioUpdateRequest)
        {
            if (formularioUpdateRequest == null) return BadRequest();

            //Processar imagem
            for (int i = 1; i <= 4; i++)
            {
                var file = Request.Form.Files[$"Fotografia{i}"];
                if (file != null && file.Length > 0)
                {
                    using var ms = new MemoryStream();
                    await file.CopyToAsync(ms);
                    var bytes = ms.ToArray();

                    // seta byte[] na propriedade Fotografia{i}
                    var fotoProp = formularioUpdateRequest.GetType().GetProperty($"Fotografia{i}");
                    if (fotoProp != null && fotoProp.PropertyType == typeof(byte[]))
                        fotoProp.SetValue(formularioUpdateRequest, bytes);

                    // seta ContentType na propriedade Fotografia{i}ContentType (se existir)
                    var ctProp = formularioUpdateRequest.GetType().GetProperty($"Fotografia{i}ContentType");
                    if (ctProp != null && ctProp.PropertyType == typeof(string))
                        ctProp.SetValue(formularioUpdateRequest, file.ContentType);

                    // seta FileName na propriedade Fotografia{i}FileName (se existir)
                    var nameProp = formularioUpdateRequest.GetType().GetProperty($"Fotografia{i}FileName");
                    if (nameProp != null && nameProp.PropertyType == typeof(string))
                        nameProp.SetValue(formularioUpdateRequest, file.FileName);
                }
            }

            var sucesso = await _services.EditarFormularioById(id, formularioUpdateRequest);
            
            if (!sucesso) return NotFound("Não encontrado");

            return Ok(formularioUpdateRequest);

        }






    }
}
