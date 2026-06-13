using CPTM_app_backend.Entities;
using CPTM_app_backend.Repository;
using CPTM_app_backend.Results;

namespace CPTM_app_backend.Services
{
    public class Services_
    {

        private readonly List<Usuario> _usuarios = new List<Usuario>();        
        private readonly UsuarioRepository _usuarioRepository;

        public Services_(UsuarioRepository usuarioRepository)
        {
            _usuarioRepository = usuarioRepository;
            // Adiciona alguns usuários de exemplo            
        }

        // Método para autenticação (login)
        public Usuario Login(string email, string senha)
        {
            var usuario = _usuarioRepository.GetUsuarioByEmail(email);

            if (usuario == null || usuario.Senha != senha)
            {
                return null; // Retorna null se o usuário não for encontrado ou a senha estiver incorreta
            }

            return usuario;
        }

        /// <summary>
        /// Busca formulários com filtros e paginação
        /// </summary>
        public async Task<PaginacaoResult<FormularioResult>> BuscarPaginadoAsync(FiltroFormularioRequest filtro, int pagina, int itensPorPagina)
        {
            // Validações
            if (pagina < 1) pagina = 1;
            if (itensPorPagina < 1 || itensPorPagina > 100) itensPorPagina = 10;

            return await _usuarioRepository.BuscarComFiltrosPaginadoAsync(filtro, pagina, itensPorPagina);
        }

        public async Task<Formulario> EnviarFormulario(Formulario formulario)
        {
            // Aqui você pode implementar a lógica para processar o formulário, como salvar no banco de dados
            // Por enquanto, vamos apenas retornar o formulário recebido como resposta

            
            var form = await _usuarioRepository.SalvarFormulario(formulario);

            return form;
        }


        public async Task<PaginacaoResult<AdminUserResult>> GetUsuariosPaginadoAsync(AdminUserRequest filtro, int pagina, int itensPorPagina)
        {
            if (pagina < 1) pagina = 1;
            if (itensPorPagina < 1 || itensPorPagina > 200) itensPorPagina = 10;

            return await _usuarioRepository.GetUsuariosPaginadoAsync(filtro ?? new AdminUserRequest(), pagina, itensPorPagina);
        }

        

        public Usuario GetUsuarioById(int id)
        {
            return _usuarios.FirstOrDefault(u => u.Id == id);
        }        

        public bool UpdateUsuario(int id, Usuario usuario)
        {
            var existingUsuario = GetUsuarioById(id);
            if (existingUsuario == null)
            {
                return false;
            }

            existingUsuario.Nome = usuario.Nome;
            existingUsuario.Email = usuario.Email;
            return true;
        }

        public bool DeleteUsuario(int id)
        {
            var usuario = GetUsuarioById(id);
            if (usuario == null)
            {
                return false;
            }

            _usuarios.Remove(usuario);
            return true;
        }
    }
}
