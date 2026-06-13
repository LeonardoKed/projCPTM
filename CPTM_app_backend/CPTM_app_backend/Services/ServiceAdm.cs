using CPTM_app_backend.Entities;
using CPTM_app_backend.Repository;
using CPTM_app_backend.Results;
using System.Text.RegularExpressions;

namespace CPTM_app_backend.Services
{
    public class ServiceAdm
    {
        private readonly RepositoryAdm _repositoryAdm;
        public ServiceAdm(RepositoryAdm repositoryAdm)
        {
            _repositoryAdm = repositoryAdm;
        }


        public async Task<PaginacaoResult<AdminUserResult>> GetUsuariosPaginadoAsync(AdminUserRequest filtro, int pagina, int itensPorPagina)
        {
            if (pagina < 1) pagina = 1;
            if (itensPorPagina < 1 || itensPorPagina > 200) itensPorPagina = 10;

            return await _repositoryAdm.GetUsuariosPaginadoAsync(filtro ?? new AdminUserRequest(), pagina, itensPorPagina);
        }

        public async Task<bool> AddUsuarioAsync(AdminCreateUser novoUsuario)
        {
            // Aqui você pode implementar a lógica para adicionar um novo usuário

            if (novoUsuario == null || string.IsNullOrEmpty(novoUsuario.Nome) || string.IsNullOrEmpty(novoUsuario.Email) || string.IsNullOrEmpty(novoUsuario.Senha) || string.IsNullOrEmpty(novoUsuario.Perfil))
            {
                // Validação básica para garantir que os campos necessários estão preenchidos
                return false;
            }

            await _repositoryAdm.AddUsuarioAsync(novoUsuario);
            // Por exemplo, validar os dados, criar um objeto de usuário e chamar o repositório para salvar no banco de dados
            // Exemplo:
            // var usuario = new Usuario { Nome = novoUsuario.Nome, Email = novoUsuario.Email, Perfil = novoUsuario.Perfil, Ativo = novoUsuario.Ativo };
            // return await _repositoryAdm.AddUsuarioAsync(usuario);
            // Por enquanto, vamos apenas retornar true para indicar que o usuário foi adicionado com sucesso
            return true;
        }

        public async Task<Usuario> GetUserById(int id)
        {
            return await _repositoryAdm.GetUsuarioByIdAsync(id);
        }

        //Editar Usuario 
        public async Task<bool> EditarUsuarioById(int id, AdminEditUser usuarioEditado)
        {
            if (usuarioEditado == null) return false;

            var usuarioExistente = await _repositoryAdm.GetUsuarioByIdAsync(id);
            if (usuarioExistente == null) return false;

            //aplicar somente campos enviados
            if (usuarioEditado.Nome != null) usuarioExistente.Nome = usuarioEditado.Nome;
            if (usuarioEditado.Email != null) usuarioExistente.Email = usuarioEditado.Email;
            if (usuarioEditado.Senha != null)
            {
                usuarioExistente.Senha = BCrypt.Net.BCrypt.HashPassword(usuarioEditado.Senha);
            }

            if (usuarioEditado.Perfil != null) usuarioExistente.Perfil = usuarioEditado.Perfil;
            if (usuarioEditado.Ativo.HasValue) usuarioExistente.Ativo = usuarioEditado.Ativo.Value;
            try
            {
                return await _repositoryAdm.EditarUsuarioById(usuarioExistente);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao atualizar usuário no serviço: {ex.Message}");
                return false;
            }

        }

        //Deletar Usuario
        public async Task<bool> DeletarUsuarioById(int id)
        {
            var usuarioExistente = await _repositoryAdm.GetUsuarioByIdAsync(id);

            if (usuarioExistente == null) return false;
            try
            {
                return await _repositoryAdm.DeletarUsuarioById(id);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao deletar usuário no serviço: {ex.Message}");
                return false;
            }
        }

        //Deletar Formulario
        public async Task<bool> DeletarFormularioById(int id)
        {
            var formularioExistente = await _repositoryAdm.GetFormularioByIdAsync(id);
            if (formularioExistente == null) return false;
            try
            {
                return await _repositoryAdm.DeletarFormularioById(id);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao deletar formulário no serviço: {ex.Message}");
                return false;
            }
        }

        //Pegar todos os usuarios
        public async Task<IEnumerable<Usuario>> GetUsuarios()
        {
            return await _repositoryAdm.GetUsuarios();
        }


    }
}
