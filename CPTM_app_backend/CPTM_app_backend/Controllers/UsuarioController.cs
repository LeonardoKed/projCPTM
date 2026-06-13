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
    [Route("api/usuariocontroller")]
    [ApiController]
    public class UsuarioController : ControllerBase
    {
        private readonly Services_ _services;
        private readonly string _key;

        public UsuarioController(Services_ services, IConfiguration configuration)
        {
            _services = services;
            _key = configuration["JwtSettings:SecretKey"];

            Console.WriteLine("Hello World - Entrei Agora");
        }



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

                    Expires = DateTime.UtcNow.AddHours(1), // Token válido por 1 hora
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
        [Authorize]
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

        /*
        //Delete: api/formulario/{id}
        [HttpDelete("formulario/{id}")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> DeletarFormulario(int id)
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
                var sucesso = await _services.DeletarFormularioAsync(id, usuarioId);
                if (!sucesso)
                    return NotFound(new ApiResponse<object>
                    {
                        Sucesso = false,
                        Mensagem = "Formulário não encontrado ou você não tem permissão para deletá-lo"
                    });
                return Ok(new ApiResponse<object>
                {
                    Sucesso = true,
                    Mensagem = "Formulário deletado com sucesso"
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Erro ao deletar formulário: {ex.Message}");
                return StatusCode(500, new ApiResponse<object>
                {
                    Sucesso = false,
                    Mensagem = "Erro ao deletar formulário"
                });
            }
        }
        */

        //Delete usuarios
        [HttpDelete("usuario/{id}")]
        [Authorize(Roles = "admin")]
        public IActionResult DeletarUsuario(int id)
        {
            var usuario = _services.GetUsuarioById(id);
            if (usuario == null)
            {
                return NotFound("Usuário não encontrado.");
            }
            _services.DeleteUsuario(id);
            return NoContent();
        }

        // GET: api/usuarios
        [HttpGet]
        [Route("getallusuarios")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> GetUsuarios(
            [FromQuery] AdminUserRequest filtro,
            [FromQuery] int pagina = 1,
            [FromQuery] int itensPorPagina = 10)
        {
            try
            {
                filtro ??= new AdminUserRequest();

                var resultado = await _services.GetUsuariosPaginadoAsync(filtro, pagina, itensPorPagina);
                return Ok(resultado);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Erro ao listar usuários: {ex.Message}");
                return StatusCode(500, new ApiResponse<object>
                {
                    Sucesso = false,
                    Mensagem = "Erro ao buscar usuários"
                });
            }
            ;
        }


        // GET: api/usuarios/{id}
        [HttpGet("{id}")]
        public IActionResult GetUsuario(int id)
        {
            var usuario = _services.GetUsuarioById(id);
            if (usuario == null)
            {
                return NotFound("Usuário não encontrado.");
            }

            return Ok(usuario);
        }


        // PUT: api/usuarios/{id}
        [HttpPut("{id}")]
        public IActionResult PutUsuario(int id, [FromBody] string nomeUsuario)
        {
            var usuario = _services.GetUsuarioById(id);
            {
                return NotFound("Usuário não encontrado.");
            }

            if (string.IsNullOrWhiteSpace(nomeUsuario))
            {
                return BadRequest("O nome do usuário não pode ser vazio.");
            }

            usuario.Nome = nomeUsuario;
            return NoContent();
        }
              
        /*
        [HttpPost("cadastrarUsuario")]
        [Authorize(Roles = "admin")]
        public IActionResult CadastrarUsuario([FromBody] Usuario novoUsuario)
        {
            _services.AddUsuario(novoUsuario);
            return Ok("Usuário cadastrado com sucesso.");
        }
        */



    }
}
