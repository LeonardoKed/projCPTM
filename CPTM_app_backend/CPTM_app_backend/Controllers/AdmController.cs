using CPTM_app_backend.Results;
using CPTM_app_backend.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Cryptography.X509Certificates;

namespace CPTM_app_backend.Controllers
{
    [Route("api/adm")]
    [ApiController]
    public class AdmController : ControllerBase
    {
        private readonly ServiceAdm _services;
        private readonly string _key;
        public AdmController(ServiceAdm services, IConfiguration configuration)
        {
            Console.WriteLine("Adm entrou");
            _services = services;
            _key = configuration["JwtSettings:SecretKey"];
        }

        // GET: api/usuarios
        [HttpGet]
        [Route("getallusuarios")]
        //[Authorize(Roles = "admin")]
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

        [HttpGet]
        [Route("getusuarios")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> GetFormularios()         
        {
            var usuarios = await _services.GetUsuarios();

            return Ok(usuarios);
        }


        //Adicionar Usuario
        [HttpPost]
        [Route("addusuario")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> AddUsuario([FromBody] AdminCreateUser novoUsuario)
        {
            try
            {
                LoginResults login = new LoginResults();

                var resultado = await _services.AddUsuarioAsync(novoUsuario);

                login.Mensagem = "Usuário criado com sucesso";

                return Ok(login);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Erro ao adicionar usuário: {ex.Message}");
                return StatusCode(500, new ApiResponse<object>
                {
                    Sucesso = false,
                    Mensagem = "Erro ao adicionar usuário"
                });
            }
        }

        [HttpGet]
        [Route("{id}")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> GetUsuarioById(int id)
        {
            var usuario = await _services.GetUserById(id);
            if (usuario==null) return NotFound("Usuário não encontrado");
            return Ok(usuario);
        }

        //Editar Usuario por id
        [HttpPatch]
        [Route("edituser/{id}")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> EditarUsuarioById([FromRoute] int id, [FromBody] AdminEditUser adminEditUser)
        {
            if (adminEditUser == null) return BadRequest();

            var sucesso = await _services.EditarUsuarioById(id, adminEditUser);

            if (!sucesso) return NotFound("Não encontrado");

            return Ok($"Usuário do {id} foi modificado");
        }


        //Deletar Usuario
        [HttpDelete]
        [Route("deleteuser/{id}")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> DeletarUsuarioById([FromRoute] int id)
        {
            var sucesso = await _services.DeletarUsuarioById(id);

            if (!sucesso) return NotFound("Não encontrado");

            return Ok($"Usuário do {id} foi deletado");
        }

        //Deletar Formulario
        [HttpDelete]
        [Route("deleteform/{id}")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> DeletarFormularioById([FromRoute] int id)
        {
            var sucesso = await _services.DeletarFormularioById(id);

            if (!sucesso) return NotFound("Não encontrado");

            return Ok($"Formulário do {id} foi deletado");
        }


        //Listar usuários

    }

}
