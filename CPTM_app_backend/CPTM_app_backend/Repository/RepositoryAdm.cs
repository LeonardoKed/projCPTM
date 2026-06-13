using CPTM_app_backend.Entities;
using CPTM_app_backend.Results;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Razor;
using Oracle.ManagedDataAccess.Client;

namespace CPTM_app_backend.Repository
{
    public class RepositoryAdm
    {
        private string connectionString;
        public RepositoryAdm(IConfiguration configuration)
        {
            connectionString = configuration.GetConnectionString("cptm");
        }

        public async Task<PaginacaoResult<AdminUserResult>> GetUsuariosPaginadoAsync(AdminUserRequest filtro, int pagina, int itensPorPagina)
        {
            var usuarios = new List<AdminUserResult>();
            int totalRegistros = 0;

            try
            {
                using (var connection = new Oracle.ManagedDataAccess.Client.OracleConnection(connectionString))
                {
                    await connection.OpenAsync();

                    var queryCount = "SELECT COUNT(*) FROM Usuarios WHERE 1=1";
                    var querySelect = @"SELECT Id, Nome, Email, Perfil , Ativo
                                FROM Usuarios
                                WHERE 1=1";

                    var where = "";
                    var parametrosCount = new List<Oracle.ManagedDataAccess.Client.OracleParameter>();
                    var parametrosSelect = new List<Oracle.ManagedDataAccess.Client.OracleParameter>();

                    void AddParam(string name, Oracle.ManagedDataAccess.Client.OracleDbType type, object value)
                    {
                        var v = value ?? (object)DBNull.Value;
                        var pCount = new Oracle.ManagedDataAccess.Client.OracleParameter(name, type) { Value = v };
                        var pSelect = new Oracle.ManagedDataAccess.Client.OracleParameter(name, type) { Value = v };
                        parametrosCount.Add(pCount);
                        parametrosSelect.Add(pSelect);
                    }

                    if (!string.IsNullOrEmpty(filtro?.Nome))
                    {
                        where += " AND Nome LIKE :Nome";
                        AddParam("Nome", Oracle.ManagedDataAccess.Client.OracleDbType.Varchar2, $"%{filtro.Nome}%");
                    }

                    if (!string.IsNullOrEmpty(filtro?.Email))
                    {
                        where += " AND Email LIKE :Email";
                        AddParam("Email", Oracle.ManagedDataAccess.Client.OracleDbType.Varchar2, $"%{filtro.Email}%");
                    }

                    if (!string.IsNullOrEmpty(filtro?.Perfil))
                    {
                        where += " AND Perfil = :Perfil";
                        AddParam("Perfil", Oracle.ManagedDataAccess.Client.OracleDbType.Varchar2, filtro.Perfil);
                    }

                    if (filtro?.Ativo.HasValue == true)
                    {
                        where += " AND Ativo = :Ativo";
                        // Armazena booleano como NUMBER(1): 1 = true, 0 = false
                        AddParam("Ativo", Oracle.ManagedDataAccess.Client.OracleDbType.Int32, filtro.Ativo.Value ? 1 : 0);
                    }

                    // Conta total
                    queryCount += where;
                    using (var commandCount = new Oracle.ManagedDataAccess.Client.OracleCommand(queryCount, connection))
                    {
                        commandCount.BindByName = true;
                        if (parametrosCount.Count > 0) commandCount.Parameters.AddRange(parametrosCount.ToArray());
                        var scalar = await commandCount.ExecuteScalarAsync();
                        totalRegistros = Convert.ToInt32(scalar);
                    }

                    // Seleção paginada (Oracle 12c+)
                    var offset = (pagina - 1) * itensPorPagina;
                    querySelect += where + " ORDER BY Id DESC OFFSET :Offset ROWS FETCH NEXT :Limit ROWS ONLY";

                    parametrosSelect.Add(new Oracle.ManagedDataAccess.Client.OracleParameter("Offset", Oracle.ManagedDataAccess.Client.OracleDbType.Int32) { Value = offset });
                    parametrosSelect.Add(new Oracle.ManagedDataAccess.Client.OracleParameter("Limit", Oracle.ManagedDataAccess.Client.OracleDbType.Int32) { Value = itensPorPagina });

                    using (var command = new Oracle.ManagedDataAccess.Client.OracleCommand(querySelect, connection))
                    {
                        command.BindByName = true;
                        if (parametrosSelect.Count > 0) command.Parameters.AddRange(parametrosSelect.ToArray());

                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                usuarios.Add(new AdminUserResult
                                {
                                    Id = reader["Id"] != DBNull.Value ? Convert.ToInt32(reader["Id"]) : 0,
                                    Nome = reader["Nome"] != DBNull.Value ? reader["Nome"].ToString() : string.Empty,
                                    Email = reader["Email"] != DBNull.Value ? reader["Email"].ToString() : string.Empty,
                                    Perfil = reader["Perfil"] != DBNull.Value ? reader["Perfil"].ToString() : string.Empty,
                                    Ativo = reader["Ativo"] != DBNull.Value ? Convert.ToInt32(reader["Ativo"]) == 1 : false
                                });
                            }
                        }
                    }
                }

                var totalPaginas = (int)Math.Ceiling(totalRegistros / (double)itensPorPagina);

                return new PaginacaoResult<AdminUserResult>
                {
                    Itens = usuarios,
                    PaginaAtual = pagina,
                    TotalPaginas = totalPaginas,
                    TotalRegistros = totalRegistros,
                    ItensPorPagina = itensPorPagina
                };
            }
            catch (Oracle.ManagedDataAccess.Client.OracleException ex)
            {
                Console.WriteLine($"❌ Erro ao buscar usuários (Oracle): {ex.Message}");
                throw;
            }
        }

        //Criar Usuario
        public async Task AddUsuarioAsync(AdminCreateUser novoUsuario)
        {
            try
            {
                using (var connection = new Oracle.ManagedDataAccess.Client.OracleConnection(connectionString))
                {
                    await connection.OpenAsync();
                    var query = @"INSERT INTO Usuarios (Nome, Email, Senha, Perfil) 
                                  VALUES (:Nome, :Email, :Senha, :Perfil)";
                    using (var command = new Oracle.ManagedDataAccess.Client.OracleCommand(query, connection))
                    {
                        command.BindByName = true;

                        command.Parameters.Add(new Oracle.ManagedDataAccess.Client.OracleParameter("Nome", Oracle.ManagedDataAccess.Client.OracleDbType.Varchar2) { Value = novoUsuario.Nome });

                        command.Parameters.Add(new Oracle.ManagedDataAccess.Client.OracleParameter("Email", Oracle.ManagedDataAccess.Client.OracleDbType.Varchar2) { Value = novoUsuario.Email });

                        // Hash da senha com BCrypt
                        var hashed = BCrypt.Net.BCrypt.HashPassword(novoUsuario.Senha ?? string.Empty);
                        command.Parameters.Add(new Oracle.ManagedDataAccess.Client.OracleParameter("Senha", Oracle.ManagedDataAccess.Client.OracleDbType.Varchar2) { Value = hashed });

                        command.Parameters.Add(new Oracle.ManagedDataAccess.Client.OracleParameter("Perfil", Oracle.ManagedDataAccess.Client.OracleDbType.Varchar2) { Value = novoUsuario.Perfil });

                        await command.ExecuteNonQueryAsync();
                    }
                }
            }
            catch (Oracle.ManagedDataAccess.Client.OracleException ex)
            {
                Console.WriteLine($"❌ Erro ao adicionar usuário (Oracle): {ex.Message}");
                throw;
            }
        }

        //Pegar o id do usuario para editar
        public async Task<Usuario> GetUsuarioByIdAsync(int id)
        {
            try
            {
                using var connection = new Oracle.ManagedDataAccess.Client.OracleConnection(connectionString);
                await connection.OpenAsync();

                var query = @"SELECT Id, Nome, Email,Senha, Perfil, Ativo FROM Usuarios WHERE Id = :id";

                var cmd = new OracleCommand(query, connection) { BindByName = true };

                cmd.Parameters.Add(new Oracle.ManagedDataAccess.Client.OracleParameter("id", Oracle.ManagedDataAccess.Client.OracleDbType.Int32) { Value = id });

                using var reader = await cmd.ExecuteReaderAsync();

                if (!await reader.ReadAsync()) return null;

                return new Usuario
                {
                    Id = reader["Id"] != DBNull.Value ? Convert.ToInt32(reader["Id"]) : 0,
                    Nome = reader["Nome"] != DBNull.Value ? reader["Nome"].ToString() : string.Empty,
                    Email = reader["Email"] != DBNull.Value ? reader["Email"].ToString() : string.Empty,
                    Senha = reader["Senha"] != DBNull.Value ? reader["Senha"].ToString() : string.Empty,
                    Perfil = reader["Perfil"] != DBNull.Value ? reader["Perfil"].ToString() : string.Empty,
                    Ativo = reader["Ativo"] != DBNull.Value ? Convert.ToInt32(reader["Ativo"]) == 1 : false
                };

            }
            catch (OracleException ex)
            {
                Console.WriteLine($"Erro ao editar o usuario: {ex.Message}");
                return null;
            }

        }


        public async Task<bool> EditarUsuarioById(Usuario usuarioEditado)
        {
            try
            {
                using (var connection = new Oracle.ManagedDataAccess.Client.OracleConnection(connectionString))
                {
                    await connection.OpenAsync();
                    var query = @"UPDATE Usuarios SET Nome = :Nome, Email = :Email, Senha = :Senha, Perfil = :Perfil, Ativo = :Ativo WHERE Id = :Id";

                    using (var command = new Oracle.ManagedDataAccess.Client.OracleCommand(query, connection))

                    {
                        command.Parameters.Add(new Oracle.ManagedDataAccess.Client.OracleParameter("Nome", Oracle.ManagedDataAccess.Client.OracleDbType.Varchar2) { Value = usuarioEditado.Nome ?? (object)DBNull.Value });
                        command.Parameters.Add(new Oracle.ManagedDataAccess.Client.OracleParameter("Email", Oracle.ManagedDataAccess.Client.OracleDbType.Varchar2) { Value = usuarioEditado.Email ?? (object)DBNull.Value });
                        command.Parameters.Add(new Oracle.ManagedDataAccess.Client.OracleParameter("Senha", Oracle.ManagedDataAccess.Client.OracleDbType.Varchar2) { Value = usuarioEditado.Senha ?? (object)DBNull.Value });
                        command.Parameters.Add(new Oracle.ManagedDataAccess.Client.OracleParameter("Perfil", Oracle.ManagedDataAccess.Client.OracleDbType.Varchar2) { Value = usuarioEditado.Perfil ?? (object)DBNull.Value });
                        command.Parameters.Add(new Oracle.ManagedDataAccess.Client.OracleParameter("Ativo", Oracle.ManagedDataAccess.Client.OracleDbType.Int32) { Value = usuarioEditado.Ativo ? 1 : 0 });
                        command.Parameters.Add(new Oracle.ManagedDataAccess.Client.OracleParameter("Id", Oracle.ManagedDataAccess.Client.OracleDbType.Int32) { Value = usuarioEditado.Id });

                        var rows = await command.ExecuteNonQueryAsync();
                        return rows > 0;
                    }
                }
            }
            catch (Oracle.ManagedDataAccess.Client.OracleException ex)
            {
                Console.WriteLine($"❌ Erro ao editar usuário (Oracle): {ex.Message}");
                return false;
            }

        }

        public async Task<bool> DeletarUsuarioById(int id)
        {
            try
            {
                using (var connection = new Oracle.ManagedDataAccess.Client.OracleConnection(connectionString))
                {
                    await connection.OpenAsync();
                    var query = @"DELETE FROM Usuarios WHERE Id = :Id";
                    using (var command = new Oracle.ManagedDataAccess.Client.OracleCommand(query, connection))
                    {
                        command.Parameters.Add(new Oracle.ManagedDataAccess.Client.OracleParameter("Id", Oracle.ManagedDataAccess.Client.OracleDbType.Int32) { Value = id });
                        var rows = await command.ExecuteNonQueryAsync();
                        return rows > 0;
                    }
                }
            }
            catch (Oracle.ManagedDataAccess.Client.OracleException ex)
            {
                Console.WriteLine($"❌ Erro ao deletar usuário (Oracle): {ex.Message}");
                return false;
            }


        }

        //Verificar se o formulario existe
        public async Task<Formulario> GetFormularioByIdAsync(int id)
        {
            try
            {
                using var connection = new Oracle.ManagedDataAccess.Client.OracleConnection(connectionString);
                await connection.OpenAsync();
                var query = @"SELECT Id FROM Formulario WHERE Id = :id";
                var cmd = new OracleCommand(query, connection) { BindByName = true };
                cmd.Parameters.Add(new Oracle.ManagedDataAccess.Client.OracleParameter("id", Oracle.ManagedDataAccess.Client.OracleDbType.Int32) { Value = id });
                using var reader = await cmd.ExecuteReaderAsync();
                if (!await reader.ReadAsync()) return null;
                return new Formulario
                {
                    Id = reader["Id"] != DBNull.Value ? Convert.ToInt32(reader["Id"]) : 0
                };
            }
            catch (OracleException ex)
            {
                Console.WriteLine($"Erro ao verificar o formulário: {ex.Message}");
                return null;
            }
        }

        //Deletar formulario
        public async Task<bool> DeletarFormularioById(int id)
        {
            try
            {
                using (var connection = new Oracle.ManagedDataAccess.Client.OracleConnection(connectionString))
                {
                    await connection.OpenAsync();
                    var query = @"DELETE FROM Formulario WHERE Id = :Id";
                    using (var command = new Oracle.ManagedDataAccess.Client.OracleCommand(query, connection))
                    {
                        command.Parameters.Add(new Oracle.ManagedDataAccess.Client.OracleParameter("Id", Oracle.ManagedDataAccess.Client.OracleDbType.Int32) { Value = id });
                        var rows = await command.ExecuteNonQueryAsync();
                        return rows > 0;
                    }
                }
            }
            catch (Oracle.ManagedDataAccess.Client.OracleException ex)
            {
                Console.WriteLine($"❌ Erro ao deletar formulário (Oracle): {ex.Message}");
                return false;
            }

        }

        //Pegar todos usuarios
        public async Task<List<Usuario>> GetUsuarios()
        {
            var usuarios = new List<Usuario>();
            try
            {
                using (var connection = new Oracle.ManagedDataAccess.Client.OracleConnection(connectionString))
                {
                    await connection.OpenAsync();
                    var query = @"SELECT Id, Nome, Email, Senha, Perfil, Ativo FROM Usuarios";
                    using (var command = new Oracle.ManagedDataAccess.Client.OracleCommand(query, connection))
                    {
                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                usuarios.Add(new Usuario
                                {
                                    Id = reader["Id"] != DBNull.Value ? Convert.ToInt32(reader["Id"]) : 0,
                                    Nome = reader["Nome"] != DBNull.Value ? reader["Nome"].ToString() : string.Empty,
                                    Email = reader["Email"] != DBNull.Value ? reader["Email"].ToString() : string.Empty,
                                    Senha = reader["Senha"] != DBNull.Value ? reader["Senha"].ToString() : string.Empty,
                                    Perfil = reader["Perfil"] != DBNull.Value ? reader["Perfil"].ToString() : string.Empty,
                                    Ativo = reader["Ativo"] != DBNull.Value ? Convert.ToInt32(reader["Ativo"]) == 1 : false
                                });
                            }
                        }
                    }
                }
            }
            catch (Oracle.ManagedDataAccess.Client.OracleException ex)
            {
                Console.WriteLine($"❌ Erro ao buscar usuários (Oracle): {ex.Message}");
                throw;
            }
            return usuarios;

        }




    }
}
