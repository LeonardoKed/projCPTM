using CPTM_app_backend.Entities;
using CPTM_app_backend.Results;
using MySql.Data.MySqlClient;
using System.Globalization;

namespace CPTM_app_backend.Repository
{
    public class UsuarioRepository
    {
        //private readonly List<Usuario> _usuarios = new List<Usuario>();
        private readonly string _connectionString;

        public UsuarioRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
            // Inicialização do repositório, se necessário
        }




        /// <summary>
        /// Busca formulários com filtros e paginação
        /// </summary>
        public async Task<PaginacaoResult<FormularioResult>> BuscarComFiltrosPaginadoAsync(
            FiltroFormularioRequest filtro,
            int pagina,
            int itensPorPagina)
        {
            var formularios = new List<FormularioResult>();
            int totalRegistros = 0;

            try
            {
                using (var connection = new MySqlConnection(_connectionString))
                {
                    await connection.OpenAsync();

                    // Query base
                    var queryCount = "SELECT COUNT(*) FROM formulario WHERE 1=1";
                    var querySelect = @"
                        SELECT Id, nomeContratada, numeroContrato, localEscopo, representante,
                               siglaArea, areaGestora, supervisoraAmbiental, QuantidadeEfluente,
                               latitude, longitude, dataCadastramento, observacoesGerais
                        FROM formulario 
                        WHERE 1=1";

                    var parametros = new List<MySqlParameter>();
                    var whereClauses = "";

                    // Filtros dinâmicos
                    if (filtro.UsuarioId.HasValue)
                    {
                        whereClauses += " AND usuario_id = @UsuarioId";
                        parametros.Add(new MySqlParameter("@UsuarioId", filtro.UsuarioId.Value));
                    }

                    if (!string.IsNullOrEmpty(filtro.NomeContratada))
                    {
                        whereClauses += " AND nomeContratada LIKE @NomeContratada";
                        parametros.Add(new MySqlParameter("@NomeContratada", $"%{filtro.NomeContratada}%"));
                    }

                    if (!string.IsNullOrEmpty(filtro.NumeroContrato))
                    {
                        whereClauses += " AND numeroContrato = @NumeroContrato";
                        parametros.Add(new MySqlParameter("@NumeroContrato", filtro.NumeroContrato));
                    }

                    if (filtro.SiglaArea.HasValue)
                    {
                        whereClauses += " AND siglaArea = @SiglaArea";
                        parametros.Add(new MySqlParameter("@SiglaArea", filtro.SiglaArea.Value));
                    }

                    //if (filtro.Municipio.HasValue)
                    //{
                    //    whereClauses += " AND municipio = @Municipio";
                    //    parametros.Add(new MySqlParameter("@Municipio", filtro.Municipio.Value));
                    //}

                    if (filtro.DataInicio.HasValue)
                    {
                        whereClauses += " AND dataCadastramento >= @DataInicio";
                        parametros.Add(new MySqlParameter("@DataInicio", filtro.DataInicio.Value));
                    }

                    //if (filtro.DataFim.HasValue)
                    //{
                    //    whereClauses += " AND dataCadastramento <= @DataFim";
                    //    parametros.Add(new MySqlParameter("@DataFim", filtro.DataFim.Value));
                    //}

                    // Conta total de registros
                    queryCount += whereClauses;
                    using (var commandCount = new MySqlCommand(queryCount, connection))
                    {
                        commandCount.Parameters.AddRange(parametros.ToArray());
                        totalRegistros = Convert.ToInt32(await commandCount.ExecuteScalarAsync());
                    }

                    // Busca dados paginados
                    querySelect += whereClauses + " ORDER BY Id DESC LIMIT @Limit OFFSET @Offset";

                    var offset = (pagina - 1) * itensPorPagina;

                    using (var commandSelect = new MySqlCommand(querySelect, connection))
                    {
                        commandSelect.Parameters.AddRange(parametros.ToArray());
                        commandSelect.Parameters.AddWithValue("@Limit", itensPorPagina);
                        commandSelect.Parameters.AddWithValue("@Offset", offset);

                        using (var reader = await commandSelect.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                formularios.Add(new FormularioResult
                                {
                                    Id = Convert.ToInt32(reader["Id"]),
                                    NomeContratada = reader["nomeContratada"].ToString(),
                                    NumeroContrato = reader["numeroContrato"].ToString(),
                                    LocalEscopo = reader["localEscopo"].ToString(),
                                    Representante = reader["representante"].ToString(),                                    
                                });
                            }
                        }
                    }
                }

                var totalPaginas = (int)Math.Ceiling(totalRegistros / (double)itensPorPagina);

                return new PaginacaoResult<FormularioResult>
                {
                    Itens = formularios,
                    PaginaAtual = pagina,
                    TotalPaginas = totalPaginas,
                    TotalRegistros = totalRegistros,
                    ItensPorPagina = itensPorPagina
                };
            }
            catch (MySqlException ex)
            {
                Console.WriteLine($"❌ Erro ao buscar formulários: {ex.Message}");
                throw;
            }
        }
    



        public Usuario GetUsuarioByEmail(string email)
        {
            Usuario usuario = null;

            using (var connection = new MySqlConnection(_connectionString))
            {
                connection.Open();

                using (var command = new MySqlCommand("SELECT Id, Nome, Email, Senha, Perfil FROM usuario WHERE Email = @Email", connection))
                {
                    command.Parameters.AddWithValue("@Email", email);

                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            usuario = new Usuario
                            {
                                Id = Convert.ToInt32(reader["Id"]),
                                Nome = reader["Nome"].ToString(),
                                Email = reader["Email"].ToString(),
                                Senha = reader["Senha"].ToString(),
                                Perfil = reader["Perfil"].ToString()
                            };
                        }
                    }
                }
            }

            return usuario;
        }

        public async Task<Formulario> SalvarFormulario(Formulario formulario)
        {

            try
            {
                using (var connection = new MySqlConnection(_connectionString))
                {
                    await connection.OpenAsync();

                    using (var command = new MySqlCommand("INSERT INTO formulario (nomeContratada, numeroContrato, localEscopo, representante,siglaArea,areaGestora,supervisoraAmbiental,QuantidadeEfluente,latitude,longitude, usuario_id) VALUES (@NomeContratada, @NumeroContrato, @LocalEscopo, @Representante,@SiglaArea,@AreaGestora,@SupervisoraAmbiental, @QuantidadeEfluente,@Latitude,@Longitude,@UsuarioId)", connection))
                    {
                        command.Parameters.AddWithValue("@NomeContratada", formulario.nomeContratada);
                        command.Parameters.AddWithValue("@NumeroContrato", formulario.numeroContrato);
                        command.Parameters.AddWithValue("@LocalEscopo", formulario.localEscopo);
                        command.Parameters.AddWithValue("@Representante", formulario.representante);

                        command.Parameters.AddWithValue("@SiglaArea", formulario.siglaArea);
                        command.Parameters.AddWithValue("@AreaGestora", formulario.areaGestora);
                        command.Parameters.AddWithValue("@SupervisoraAmbiental", formulario.supervisoraAmbiental);

                        command.Parameters.AddWithValue("@QuantidadeEfluente", formulario.QuantidadeEfluente);

                        command.Parameters.AddWithValue("@Latitude", formulario.latitude);
                        command.Parameters.AddWithValue("@Longitude", formulario.longitude);

                        command.Parameters.AddWithValue("@UsuarioId", formulario.usuario_id);

                        await command.ExecuteNonQueryAsync();

                        Console.WriteLine("Consulta SQL executada com sucesso.");
                    }
                }
            }

            catch (MySqlException ex)
            {
                Console.WriteLine($"Erro ao executar consulta SQL: {ex.Message}");
                throw; // Re-lança a exceção para análise adicional
            }

            return formulario;
        }
        

        public Usuario cadastrarUsuario(Usuario usuario)
        {
            using (var connection = new MySqlConnection(_connectionString))
            {
                connection.Open();

                using (var command = new MySqlCommand("INSERT INTO usuario (UsuarioNomeApp, Nome, Sobrenome, Email, Senha, Ativo, Perfil) VALUES (@UsuarioNomeApp, @Nome, @Sobrenome, @Email, @Senha, @Ativo, @Perfil)", connection))
                {
                    command.Parameters.AddWithValue("@UsuarioNomeApp", usuario.UsuarioNomeApp);
                    command.Parameters.AddWithValue("@Nome", usuario.Nome);
                    command.Parameters.AddWithValue("@Sobrenome", usuario.Sobrenome);
                    command.Parameters.AddWithValue("@Email", usuario.Email);
                    command.Parameters.AddWithValue("@Senha", usuario.Senha);
                    command.Parameters.AddWithValue("@Ativo", usuario.Ativo);
                    command.Parameters.AddWithValue("@Perfil", usuario.Perfil);

                    command.ExecuteNonQuery();
                }
            }
            return usuario;
        }

        public Usuario GetUsuarioById(int id)
        {
            Usuario usuario = null;
            using (var connection = new MySqlConnection(_connectionString))
            {
                connection.Open();
                using (var command = new MySqlCommand("SELECT Id, Nome, Email FROM usuario WHERE Id = @Id", connection))
                {
                    command.Parameters.AddWithValue("@Id", id);
                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            usuario = new Usuario
                            {
                                Id = Convert.ToInt32(reader["Id"]),
                                Nome = reader["Nome"].ToString(),
                                Email = reader["Email"].ToString()
                            };
                        }
                    }
                }
            }
            return usuario;
        }


        public async Task<PaginacaoResult<AdminUserResult>> GetUsuariosPaginadoAsync(AdminUserRequest filtro,int pagina,int itensPorPagina)
        {
            var usuarios = new List<AdminUserResult>();
            int totalRegistros = 0;

            try
            {
                using (var connection = new MySqlConnection(_connectionString))
                {
                    await connection.OpenAsync();

                    var queryCount = "SELECT COUNT(*) FROM usuario WHERE 1=1";
                    var querySelect = @"SELECT Id, Nome, Email, Perfil, Ativo
                                FROM usuario
                                WHERE 1=1";

                    var parametros = new List<MySqlParameter>();
                    var where = "";

                    if (!string.IsNullOrEmpty(filtro.Nome))
                    {
                        where += " AND Nome LIKE @Nome";
                        parametros.Add(new MySqlParameter("@Nome", $"%{filtro.Nome}%"));
                    }

                    if (!string.IsNullOrEmpty(filtro.Email))
                    {
                        where += " AND Email LIKE @Email";
                        parametros.Add(new MySqlParameter("@Email", $"%{filtro.Email}%"));
                    }

                    if (!string.IsNullOrEmpty(filtro.Perfil))
                    {
                        where += " AND Perfil = @Perfil";
                        parametros.Add(new MySqlParameter("@Perfil", filtro.Perfil));
                    }

                    if (filtro.Ativo.HasValue)
                    {
                        where += " AND Ativo = @Ativo";
                        parametros.Add(new MySqlParameter("@Ativo", filtro.Ativo.Value));
                    }

                    // Conta total
                    queryCount += where;
                    using (var commandCount = new MySqlCommand(queryCount, connection))
                    {
                        commandCount.Parameters.AddRange(parametros.ToArray());
                        totalRegistros = Convert.ToInt32(await commandCount.ExecuteScalarAsync());
                    }

                    // Seleção paginada
                    var offset = (pagina - 1) * itensPorPagina;
                    querySelect += where + " ORDER BY Id DESC LIMIT @Limit OFFSET @Offset";

                    using (var command = new MySqlCommand(querySelect, connection))
                    {
                        command.Parameters.AddRange(parametros.ToArray());
                        command.Parameters.AddWithValue("@Limit", itensPorPagina);
                        command.Parameters.AddWithValue("@Offset", offset);

                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                usuarios.Add(new AdminUserResult
                                {
                                    Id = Convert.ToInt32(reader["Id"]),
                                    Nome = reader["Nome"]?.ToString(),
                                    Email = reader["Email"]?.ToString(),
                                    Perfil = reader["Perfil"]?.ToString(),
                                    Ativo = reader["Ativo"] != DBNull.Value ? Convert.ToBoolean(reader["Ativo"]) : false,
                                    //DataCriacao = reader["dataCriacao"] != DBNull.Value ? Convert.ToDateTime(reader["dataCriacao"]) : (DateTime?)null
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
            catch (MySqlException ex)
            {
                Console.WriteLine($"❌ Erro ao buscar usuários: {ex.Message}");
                throw;
            }
        }



    }
}
