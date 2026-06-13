using CPTM_app_backend.Entities;
using CPTM_app_backend.Results;
using MySql.Data.MySqlClient;
using Oracle.ManagedDataAccess.Client;
using Oracle.ManagedDataAccess.Types;
using System.Configuration;
using System.Data;

namespace CPTM_app_backend.Repository
{
    public class RepositoryUser
    {
        private string connectionString;
        public RepositoryUser(IConfiguration configuration)
        {
            connectionString = configuration.GetConnectionString("cptm");
        }

        public Usuario GetUsuarioByEmail(string email)
        {
            Usuario usuario = null;

            using (var connection = new OracleConnection(connectionString))
            {
                connection.Open();

                using (var command = new OracleCommand("SELECT Id, Nome, Email, Senha, Perfil  FROM Usuarios WHERE Email = :Email", connection))
                {
                    command.BindByName = true;
                    command.Parameters.Add("Email", OracleDbType.Varchar2).Value = email;

                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            usuario = new Usuario
                            {
                                Id = !reader.IsDBNull(reader.GetOrdinal("Id")) ? Convert.ToInt32(reader["Id"]) : 0,
                                Nome = !reader.IsDBNull(reader.GetOrdinal("Nome")) ? reader.GetString(reader.GetOrdinal("Nome")) : null,
                                Email = !reader.IsDBNull(reader.GetOrdinal("Email")) ? reader.GetString(reader.GetOrdinal("Email")) : null,
                                Senha = !reader.IsDBNull(reader.GetOrdinal("Senha")) ? reader.GetString(reader.GetOrdinal("Senha")) : null,
                                Perfil = !reader.IsDBNull(reader.GetOrdinal("Perfil")) ? reader.GetString(reader.GetOrdinal("Perfil")) : null
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
                using (var connection = new OracleConnection(connectionString))
                {
                    await connection.OpenAsync();

                    var sql = @"INSERT INTO Formulario (nomeContratada, numeroContrato, localEscopo, representante, siglaArea, areaGestora, identificadorAreaGestora, siglaAreaGestora,supervisoraAmbiental, autorCadastramento, responsavelTecnico, registroProfissional,documentoResponsabilidadeTecnica, naturezaPGA, tipoFormulario, dataEmissaoFormulario,numeroFormulario, autorFormulario, arquivoFDC, codigoFDC, dataCadastramento, horaCadastramento, chavePrimaria, elementoMonitoramentoNumero,elementoMonitoramentoNome, municipio, linhaCPTM, nomeEstacaoCPTM, numeroViaLinha, trechoSentidoLinha, quilometroPoste, latitude, longitude, atividade, atividadeNaoListada, draListado, draNaoListada,
    codigoDRA, dataValidadeDRA, TipoAtividadeCPTM, NomeEdificacao, NomeEdificacaoComplemento, OrigemEfluente, FonteGeradoraEfluente, QuantidadeEfluente,TipoDestinacaoEfluente, TipoVeiculo,IdentificadorVeiculo, codigoGuiaRemessa, distanciaViaCPTM, observacoesGerais,fotografia1, fotografia2, fotografia3, fotografia4, usuario_id
) VALUES (:NomeContratada, :NumeroContrato, :LocalEscopo, :Representante,:SiglaArea, :AreaGestora, :IdentificadorAreaGestora, :siglaAreaGestora,:SupervisoraAmbiental, :autorCadastramento, :responsavelTecnico, :registroProfissional,:DocRespTec, :naturezaPGA, :tipoFormulario, :dataEmissaoFormulario,:numeroFormulario, :autorFormulario, :arquivoFDC, :codigoFDC,:dataCadastramento, :horaCadastramento, :chavePrimaria, :elementoMonitoramentoNumero,:elementoMonitoramentoNome, :municipio, :linhaCPTM, :nomeEstacaoCPTM,:numeroViaLinha, :trechoSentidoLinha, :quilometroPoste, :Latitude, :Longitude, :atividade, :atividadeNaoListada, :draListado, :draNaoListada,:codigoDRA, :dataValidadeDRA, :TipoAtividadeCPTM, :NomeEdificacao, :NomeEdificacaoComplemento,:OrigemEfluente, :FonteGeradoraEfluente,:QuantidadeEfluente, :TipoDestinacaoEfluente, :TipoVeiculo,:IdentificadorVeiculo, :codigoGuiaRemessa, :distanciaViaCPTM, :observacoesGerais,:Fotografia1, :Fotografia2, :Fotografia3, :Fotografia4, :UsuarioId)";

                    using (var command = new OracleCommand(sql, connection))
                    {
                        command.BindByName = true;

                        command.Parameters.Add("NomeContratada", OracleDbType.Varchar2).Value = formulario.nomeContratada ?? (object)DBNull.Value;

                        command.Parameters.Add("NumeroContrato", OracleDbType.Varchar2).Value = formulario.numeroContrato ?? (object)DBNull.Value;

                        command.Parameters.Add("LocalEscopo", OracleDbType.Varchar2).Value = formulario.localEscopo ?? (object)DBNull.Value;

                        command.Parameters.Add("Representante", OracleDbType.Varchar2).Value = formulario.representante ?? (object)DBNull.Value;

                        command.Parameters.Add("SiglaArea", OracleDbType.Int32).Value = formulario.siglaArea ?? (object)DBNull.Value;

                        command.Parameters.Add("AreaGestora", OracleDbType.Int32).Value = formulario.areaGestora ?? (object)DBNull.Value;

                        command.Parameters.Add("IdentificadorAreaGestora", OracleDbType.Varchar2).Value = formulario.identificadorAreaGestora ?? (object)DBNull.Value;

                        command.Parameters.Add("siglaAreaGestora", OracleDbType.Varchar2).Value = formulario.siglaAreaGestora ?? (object)DBNull.Value;

                        command.Parameters.Add("SupervisoraAmbiental", OracleDbType.Varchar2).Value = formulario.supervisoraAmbiental ?? (object)DBNull.Value;

                        command.Parameters.Add("autorCadastramento", OracleDbType.Varchar2).Value = formulario.autorCadastramento ?? (object)DBNull.Value;

                        command.Parameters.Add("responsavelTecnico", OracleDbType.Varchar2).Value = formulario.responsavelTecnico ?? (object)DBNull.Value;

                        command.Parameters.Add("registroProfissional", OracleDbType.Varchar2).Value = formulario.registroProfissional ?? (object)DBNull.Value;

                        command.Parameters.Add("DocRespTec", OracleDbType.Varchar2).Value = formulario.DocRespTec ?? (object)DBNull.Value;

                        command.Parameters.Add("naturezaPGA", OracleDbType.Int32).Value = formulario.naturezaPGA ?? (object)DBNull.Value;

                        command.Parameters.Add("tipoFormulario", OracleDbType.Varchar2).Value = formulario.tipoFormulario ?? (object)DBNull.Value;

                        command.Parameters.Add("dataEmissaoFormulario", OracleDbType.Date).Value = formulario.dataEmissaoFormulario ?? (object)DBNull.Value;

                        command.Parameters.Add("numeroFormulario", OracleDbType.Int32).Value = formulario.numeroFormulario ?? (object)DBNull.Value;

                        command.Parameters.Add("autorFormulario", OracleDbType.Varchar2).Value = formulario.autorFormulario ?? (object)DBNull.Value;

                        command.Parameters.Add("arquivoFDC", OracleDbType.Varchar2).Value = formulario.arquivoFDC ?? (object)DBNull.Value;

                        command.Parameters.Add("codigoFDC", OracleDbType.Int32).Value = formulario.codigoFDC ?? (object)DBNull.Value;

                        command.Parameters.Add("dataCadastramento", OracleDbType.Date).Value = formulario.dataCadastramento ?? (object)DBNull.Value;

                        command.Parameters.Add("horaCadastramento", OracleDbType.Varchar2, 8).Value = formulario.horaCadastramento.HasValue ? formulario.horaCadastramento.Value.ToString("HH:mm:ss") : (object)DBNull.Value;

                        command.Parameters.Add("chavePrimaria", OracleDbType.Varchar2).Value = formulario.chavePrimaria ?? (object)DBNull.Value;

                        command.Parameters.Add("elementoMonitoramentoNumero", OracleDbType.Int32).Value = formulario.elementoMonitoramentoNumero ?? (object)DBNull.Value;

                        command.Parameters.Add("elementoMonitoramentoNome", OracleDbType.Varchar2).Value = formulario.elementoMonitoramentoNome ?? (object)DBNull.Value;

                        command.Parameters.Add("municipio", OracleDbType.Int32).Value = formulario.municipio ?? (object)DBNull.Value;

                        command.Parameters.Add("linhaCPTM", OracleDbType.Int32).Value = formulario.linhaCPTM ?? (object)DBNull.Value;

                        command.Parameters.Add("nomeEstacaoCPTM", OracleDbType.Int32).Value = formulario.nomeEstacaoCPTM ?? (object)DBNull.Value;

                        command.Parameters.Add("numeroViaLinha", OracleDbType.Int32).Value = formulario.numeroViaLinha ?? (object)DBNull.Value;

                        command.Parameters.Add("trechoSentidoLinha", OracleDbType.Int32).Value = formulario.trechoSentidoLinha ?? (object)DBNull.Value;

                        command.Parameters.Add("quilometroPoste", OracleDbType.Varchar2).Value = formulario.quilometroPoste ?? (object)DBNull.Value;


                        // Latitude / Longitude (decimal) - OracleDbType.Decimal
                        command.Parameters.Add("Latitude", OracleDbType.Decimal).Value = formulario.latitude ?? (object)DBNull.Value;
                        command.Parameters.Add("Longitude", OracleDbType.Decimal).Value = formulario.longitude ?? (object)DBNull.Value;


                        command.Parameters.Add("atividade", OracleDbType.Varchar2).Value = formulario.atividade ?? (object)DBNull.Value;

                        command.Parameters.Add("atividadeNaoListada", OracleDbType.Varchar2).Value = formulario.atividadeNaoListada ?? (object)DBNull.Value;

                        command.Parameters.Add("draListado", OracleDbType.Int32).Value = formulario.draListado ?? (object)DBNull.Value;

                        command.Parameters.Add("draNaoListada", OracleDbType.Varchar2).Value = formulario.draNaoListada ?? (object)DBNull.Value;

                        command.Parameters.Add("codigoDRA", OracleDbType.Int32).Value = formulario.codigoDRA ?? (object)DBNull.Value;

                        command.Parameters.Add("dataValidadeDRA", OracleDbType.Date).Value = formulario.dataValidadeDRA ?? (object)DBNull.Value;

                        command.Parameters.Add("TipoAtividadeCPTM", OracleDbType.Int32).Value = formulario.TipoAtividadeCPTM ?? (object)DBNull.Value;

                        command.Parameters.Add("NomeEdificacao", OracleDbType.Int32).Value = formulario.NomeEdificacao ?? (object)DBNull.Value;

                        command.Parameters.Add("NomeEdificacaoComplemento", OracleDbType.Varchar2).Value = formulario.NomeEdificacaoComplemento ?? (object)DBNull.Value;

                        command.Parameters.Add("OrigemEfluente", OracleDbType.Int32).Value = formulario.OrigemEfluente ?? (object)DBNull.Value;

                        command.Parameters.Add("FonteGeradoraEfluente", OracleDbType.Int32).Value = formulario.FonteGeradoraEfluente ?? (object)DBNull.Value;

                        command.Parameters.Add("QuantidadeEfluente", OracleDbType.Double).Value = formulario.QuantidadeEfluente ?? (object)DBNull.Value;

                        command.Parameters.Add("TipoDestinacaoEfluente", OracleDbType.Int32).Value = formulario.TipoDestinacaoEfluente ?? (object)DBNull.Value;

                        command.Parameters.Add("TipoVeiculo", OracleDbType.Int32).Value = formulario.TipoVeiculo ?? (object)DBNull.Value;

                        command.Parameters.Add("IdentificadorVeiculo", OracleDbType.Varchar2).Value = formulario.IdentificadorVeiculo ?? (object)DBNull.Value;

                        command.Parameters.Add("codigoGuiaRemessa", OracleDbType.Varchar2).Value = formulario.codigoGuiaRemessa ?? (object)DBNull.Value;

                        command.Parameters.Add("distanciaViaCPTM", OracleDbType.Double).Value = formulario.distanciaViaCPTM ?? (object)DBNull.Value;

                        command.Parameters.Add("observacoesGerais", OracleDbType.Varchar2).Value = formulario.observacoesGerais ?? (object)DBNull.Value;


                        command.Parameters.Add("Fotografia1", OracleDbType.Blob).Value = formulario.Fotografia1 ?? (object)DBNull.Value;
                        command.Parameters.Add("Fotografia2", OracleDbType.Blob).Value = formulario.Fotografia2 ?? (object)DBNull.Value;
                        command.Parameters.Add("Fotografia3", OracleDbType.Blob).Value = formulario.Fotografia3 ?? (object)DBNull.Value;
                        command.Parameters.Add("Fotografia4", OracleDbType.Blob).Value = formulario.Fotografia4 ?? (object)DBNull.Value;


                        command.Parameters.Add("UsuarioId", OracleDbType.Int32).Value = formulario.usuario_id;

                        await command.ExecuteNonQueryAsync();


                        Console.WriteLine("Consulta SQL executada com sucesso (Oracle).");
                    }
                }
            }
            catch (OracleException ex)
            {
                Console.WriteLine($"Erro ao executar consulta Oracle: {ex.Message}");
                throw;
            }

            return formulario;
        }



        public async Task<PaginacaoResult<FormularioResult>> BuscarComFiltrosPaginadoAsync(
            FiltroFormularioRequest filtro,
            int pagina,
            int itensPorPagina)
        {
            var formularios = new List<FormularioResult>();
            int totalRegistros = 0;

            try
            {
                using (var connection = new OracleConnection(connectionString))
                {
                    await connection.OpenAsync();

                    var queryCount = "SELECT COUNT(*) FROM formulario WHERE 1=1";
                    var querySelect = @"
                        SELECT Id, nomeContratada, numeroContrato, localEscopo, representante,
                               siglaArea, areaGestora, supervisoraAmbiental, QuantidadeEfluente,
                               latitude, longitude, dataCadastramento, observacoesGerais, usuario_id
                        FROM formulario
                        WHERE 1=1";

                    var whereClauses = "";
                    var parametrosCount = new List<OracleParameter>();
                    var parametrosSelect = new List<OracleParameter>();

                    void AddParam(string name, OracleDbType type, object value)
                    {
                        var v = value ?? (object)DBNull.Value;
                        parametrosCount.Add(new OracleParameter(name, type) { Value = v });
                        parametrosSelect.Add(new OracleParameter(name, type) { Value = v });
                    }

                    if (filtro?.UsuarioId.HasValue == true)
                    {
                        whereClauses += " AND usuario_id = :UsuarioId";
                        AddParam("UsuarioId", OracleDbType.Int32, filtro.UsuarioId.Value);
                    }

                    if (!string.IsNullOrEmpty(filtro?.NomeContratada))
                    {
                        whereClauses += " AND nomeContratada LIKE :NomeContratada";
                        AddParam("NomeContratada", OracleDbType.Varchar2, $"%{filtro.NomeContratada}%");
                    }

                    if (!string.IsNullOrEmpty(filtro?.NumeroContrato))
                    {
                        whereClauses += " AND numeroContrato = :NumeroContrato";
                        AddParam("NumeroContrato", OracleDbType.Varchar2, filtro.NumeroContrato);
                    }

                    if (filtro?.SiglaArea.HasValue == true)
                    {
                        whereClauses += " AND siglaArea = :SiglaArea";
                        AddParam("SiglaArea", OracleDbType.Int32, filtro.SiglaArea.Value);
                    }

                    //if (filtro?.Municipio.HasValue == true)
                    //{
                    //    whereClauses += " AND municipio = :Municipio";
                    //    AddParam("Municipio", OracleDbType.Int32, filtro.Municipio.Value);
                    //}

                    if (filtro?.DataInicio.HasValue == true)
                    {
                        whereClauses += " AND dataCadastramento >= :DataInicio";
                        AddParam("DataInicio", OracleDbType.Date, filtro.DataInicio.Value);
                    }

                    //if (filtro?.DataFim.HasValue == true)
                    //{
                    //    whereClauses += " AND dataCadastramento <= :DataFim";
                    //    AddParam("DataFim", OracleDbType.Date, filtro.DataFim.Value);
                    //}

                    // Conta total
                    queryCount += whereClauses;
                    using (var commandCount = new OracleCommand(queryCount, connection))
                    {
                        commandCount.BindByName = true;
                        if (parametrosCount.Count > 0) commandCount.Parameters.AddRange(parametrosCount.ToArray());
                        var scalar = await commandCount.ExecuteScalarAsync();
                        totalRegistros = Convert.ToInt32(scalar);
                    }

                    // Paginação Oracle 12c+
                    var offset = (pagina - 1) * itensPorPagina;
                    querySelect += whereClauses + " ORDER BY Id DESC OFFSET :Offset ROWS FETCH NEXT :Limit ROWS ONLY";

                    // adiciona parâmetros de paginação
                    parametrosSelect.Add(new OracleParameter("Offset", OracleDbType.Int32) { Value = offset });
                    parametrosSelect.Add(new OracleParameter("Limit", OracleDbType.Int32) { Value = itensPorPagina });

                    using (var commandSelect = new OracleCommand(querySelect, connection))
                    {
                        commandSelect.BindByName = true;
                        if (parametrosSelect.Count > 0) commandSelect.Parameters.AddRange(parametrosSelect.ToArray());

                        using (var reader = await commandSelect.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                formularios.Add(new FormularioResult
                                {
                                    Id = reader["Id"] != DBNull.Value ? Convert.ToInt32(reader["Id"]) : 0,
                                    NomeContratada = reader["nomeContratada"] != DBNull.Value ? reader["nomeContratada"].ToString() : string.Empty,
                                    NumeroContrato = reader["numeroContrato"] != DBNull.Value ? reader["numeroContrato"].ToString() : string.Empty,
                                    LocalEscopo = reader["localEscopo"] != DBNull.Value ? reader["localEscopo"].ToString() : string.Empty,
                                    Representante = reader["representante"] != DBNull.Value ? reader["representante"].ToString() : string.Empty,
                                    Usuario_id = reader["usuario_id"] != DBNull.Value ? Convert.ToInt32(reader["usuario_id"]) : (int?)null

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
            catch (OracleException ex)
            {
                Console.WriteLine($"❌ Erro ao buscar formulários (Oracle): {ex.Message}");
                throw;
            }
        }

        //obter dados usuario e adm
        public async Task<Usuario> GetUsuarioById(int id)
        {
            Usuario usuario = null;
            try
            {
                using (var connection = new OracleConnection(connectionString))
                {
                    await connection.OpenAsync();
                    var sql = "SELECT Id, Nome, Email FROM Usuarios WHERE Id = :Id";
                    using (var command = new OracleCommand(sql, connection))
                    {
                        command.BindByName = true;
                        command.Parameters.Add("Id", OracleDbType.Int32).Value = id;
                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            if (await reader.ReadAsync())
                            {
                                usuario = new Usuario
                                {
                                    Id = !reader.IsDBNull(reader.GetOrdinal("Id")) ? Convert.ToInt32(reader["Id"]) : 0,
                                    Nome = !reader.IsDBNull(reader.GetOrdinal("Nome")) ? reader.GetString(reader.GetOrdinal("Nome")) : null,
                                    Email = !reader.IsDBNull(reader.GetOrdinal("Email")) ? reader.GetString(reader.GetOrdinal("Email")) : null

                                };
                            }
                        }
                    }
                }
            }
            catch (OracleException ex)
            {
                Console.WriteLine($"Erro ao obter usuário por ID (Oracle): {ex.Message}");
                throw;
            }

            return usuario;

        }


        //Pegar o id do usuario para editar
        public async Task<Usuario> GetUsuarioByIdAsync(int id)
        {
            try
            {
                using var connection = new Oracle.ManagedDataAccess.Client.OracleConnection(connectionString);
                await connection.OpenAsync();

                var query = @"SELECT Id, Nome, Email,Senha FROM Usuarios WHERE Id = :id";

                var cmd = new OracleCommand(query, connection) { BindByName = true };

                cmd.Parameters.Add(new Oracle.ManagedDataAccess.Client.OracleParameter("id", Oracle.ManagedDataAccess.Client.OracleDbType.Int32) { Value = id });

                using var reader = await cmd.ExecuteReaderAsync();

                if (!await reader.ReadAsync()) return null;

                return new Usuario
                {
                    Id = reader["Id"] != DBNull.Value ? Convert.ToInt32(reader["Id"]) : 0,
                    Nome = reader["Nome"] != DBNull.Value ? reader["Nome"].ToString() : string.Empty,
                    Email = reader["Email"] != DBNull.Value ? reader["Email"].ToString() : string.Empty,
                    Senha = reader["Senha"] != DBNull.Value ? reader["Senha"].ToString() : string.Empty

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
                    var query = @"UPDATE Usuarios SET Nome = :Nome, Email = :Email, Senha = :Senha WHERE Id = :Id";

                    using (var command = new Oracle.ManagedDataAccess.Client.OracleCommand(query, connection))

                    {


                        command.Parameters.Add(new Oracle.ManagedDataAccess.Client.OracleParameter("Nome", Oracle.ManagedDataAccess.Client.OracleDbType.Varchar2) { Value = usuarioEditado.Nome ?? (object)DBNull.Value });

                        command.Parameters.Add(new Oracle.ManagedDataAccess.Client.OracleParameter("Email", Oracle.ManagedDataAccess.Client.OracleDbType.Varchar2) { Value = usuarioEditado.Email ?? (object)DBNull.Value });

                        command.Parameters.Add(new Oracle.ManagedDataAccess.Client.OracleParameter("Senha", Oracle.ManagedDataAccess.Client.OracleDbType.Varchar2) { Value = usuarioEditado.Senha ?? (object)DBNull.Value });

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


        public async Task<Formulario> GetFormularioByIdAsync(int id, int usuarioId)
        {
            Formulario formulario = null;
            try
            {
                using var connection = new OracleConnection(connectionString);
                await connection.OpenAsync();

                var query = @"SELECT * FROM Formulario WHERE Id = :id AND usuario_id = :usuarioId";
                using var cmd = new OracleCommand(query, connection) { BindByName = true };
                cmd.Parameters.Add(new OracleParameter("id", OracleDbType.Int32) { Value = id });
                cmd.Parameters.Add(new OracleParameter("usuarioId", OracleDbType.Int32) { Value = usuarioId });

                using var reader = await cmd.ExecuteReaderAsync(CommandBehavior.SingleRow);
                if (!await reader.ReadAsync()) return null;

                formulario = new Formulario
                {
                    Id = reader["Id"] != DBNull.Value ? Convert.ToInt32(reader["Id"]) : 0,
                    usuario_id = reader["usuario_id"] != DBNull.Value ? Convert.ToInt32(reader["usuario_id"]) : 0,
                    nomeContratada = reader["nomeContratada"] != DBNull.Value ? reader["nomeContratada"].ToString() : null,
                    numeroContrato = reader["numeroContrato"] != DBNull.Value ? reader["numeroContrato"].ToString() : null,
                    localEscopo = reader["localEscopo"] != DBNull.Value ? reader["localEscopo"].ToString() : null,
                    representante = reader["representante"] != DBNull.Value ? reader["representante"].ToString() : null,
                    // Exemplos adicionais - adaptar conforme sua entidade Formulario
                    dataCadastramento = reader["dataCadastramento"] != DBNull.Value ? (DateTime?)Convert.ToDateTime(reader["dataCadastramento"]) : null,
                    horaCadastramento = reader["horaCadastramento"] != DBNull.Value

                    ? (
                    reader["horaCadastramento"] is TimeSpan ts
                        ? TimeOnly.FromTimeSpan(ts)
                        : reader["horaCadastramento"] is OracleTimeStamp ots
                            ? TimeOnly.FromTimeSpan(ots.Value.TimeOfDay)
                            : TimeOnly.Parse(reader["horaCadastramento"].ToString())
                  )

                : (TimeOnly?)null,
                    latitude = reader["latitude"] != DBNull.Value ? Convert.ToDecimal(reader["latitude"]) : (decimal?)null,
                    longitude = reader["longitude"] != DBNull.Value ? Convert.ToDecimal(reader["longitude"]) : (decimal?)null,
                    observacoesGerais = reader["observacoesGerais"] != DBNull.Value ? reader["observacoesGerais"].ToString() : null
                };

                // Ler BLOBs para byte[] (se sua entidade usar byte[] para FotografiaN)
                //formulario.Fotografia1 = ReadBlob(reader, "fotografia1");
                //formulario.Fotografia2 = ReadBlob(reader, "fotografia2");
                //formulario.Fotografia3 = ReadBlob(reader, "fotografia3");
                //formulario.Fotografia4 = ReadBlob(reader, "fotografia4");
            }
            catch (OracleException ex)
            {
                Console.WriteLine($"Erro ao obter formulário por ID (Oracle): {ex.Message}");
                throw;
            }
            return formulario;
        }

        // Método que retorna apenas bytes da foto (para endpoint /form/{id}/foto/{index})
        public async Task<byte[]> GetFotoBytesAsync(int id, int usuarioId, int index)
        {
            if (index < 1 || index > 4) return null;

            var columnName = index switch
            {
                1 => "fotografia1",
                2 => "fotografia2",
                3 => "fotografia3",
                4 => "fotografia4",
                _ => null
            };

            try
            {
                using var connection = new OracleConnection(connectionString);
                await connection.OpenAsync();

                var query = $"SELECT {columnName} FROM Formulario WHERE Id = :id AND usuario_id = :usuarioId";
                using var cmd = new OracleCommand(query, connection) { BindByName = true };
                cmd.Parameters.Add(new OracleParameter("id", OracleDbType.Int32) { Value = id });
                cmd.Parameters.Add(new OracleParameter("usuarioId", OracleDbType.Int32) { Value = usuarioId });

                using var reader = await cmd.ExecuteReaderAsync(CommandBehavior.SingleRow);
                if (!await reader.ReadAsync()) return null;

                return ReadBlob(reader, columnName);
            }
            catch (OracleException ex)
            {
                Console.WriteLine($"Erro ao obter foto (Oracle): {ex.Message}");
                throw;
            }
        }

        // Helper para ler BLOB/RAW do reader de forma robusta
        private static byte[] ReadBlob(IDataRecord reader, string columnName)
        {
            try
            {
                if (reader == null) return null;
                var ordinal = reader.GetOrdinal(columnName);
                if (reader.IsDBNull(ordinal)) return null;
                var val = reader.GetValue(ordinal);
                if (val is byte[] bytes) return bytes;
                if (val is OracleBlob ob) return ob.Value;
                // fallback: tentar converter para byte[]
                return val as byte[] ?? null;
            }
            catch
            {
                return null;
            }
        }



        //Pegar formulario somente com o id
        public async Task<Formulario> GetFormularioByIdOnlyAsync(int id)
        {
            Formulario formulario = null;
            try
            {
                using var connection = new OracleConnection(connectionString);
                await connection.OpenAsync();
                var query = @"SELECT * FROM Formulario WHERE Id = :id";
                using var cmd = new OracleCommand(query, connection) { BindByName = true };
                cmd.Parameters.Add(new OracleParameter("id", OracleDbType.Int32) { Value = id });
                using var reader = await cmd.ExecuteReaderAsync(CommandBehavior.SingleRow);
                if (!await reader.ReadAsync()) return null;
                formulario = new Formulario
                {
                    Id = reader["Id"] != DBNull.Value ? Convert.ToInt32(reader["Id"]) : 0,
                    nomeContratada = reader["nomeContratada"] != DBNull.Value ? reader["nomeContratada"].ToString() : null,
                    numeroContrato = reader["numeroContrato"] != DBNull.Value ? reader["numeroContrato"].ToString() : null,
                    localEscopo = reader["localEscopo"] != DBNull.Value ? reader["localEscopo"].ToString() : null,
                    representante = reader["representante"] != DBNull.Value ? reader["representante"].ToString() : null,
                    // Exemplos adicionais - adaptar conforme sua entidade Formulario
                    dataCadastramento = reader["dataCadastramento"] != DBNull.Value ? (DateTime?)Convert.ToDateTime(reader["dataCadastramento"]) : null,
                    horaCadastramento = reader["horaCadastramento"] != DBNull.Value
                    ? (
                    reader["horaCadastramento"] is TimeSpan ts
                        ? TimeOnly.FromTimeSpan(ts)
                        : reader["horaCadastramento"] is OracleTimeStamp ots
                            ? TimeOnly.FromTimeSpan(ots.Value.TimeOfDay)
                            : TimeOnly.Parse(reader["horaCadastramento"].ToString())
                  )
                : (TimeOnly?)null,



                    latitude = reader["latitude"] != DBNull.Value ? Convert.ToDecimal(reader["latitude"]) : (decimal?)null,
                    longitude = reader["longitude"] != DBNull.Value ? Convert.ToDecimal(reader["longitude"]) : (decimal?)null,
                    observacoesGerais = reader["observacoesGerais"] != DBNull.Value ? reader["observacoesGerais"].ToString() : null
                };
                // Ler BLOBs para           


            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao obter formulário por ID (Oracle): {ex.Message}");
                throw;
            }

            return formulario;


        }


        public async Task<bool> AtualizarFormulario(Formulario formularioUpdateRequest)
        {
            if (formularioUpdateRequest == null) return false;

            try
            {
                using var connection = new OracleConnection(connectionString);
                await connection.OpenAsync();

                var query = @"
UPDATE Formulario SET
    nomeContratada = :NomeContratada,
    numeroContrato = :NumeroContrato,
    localEscopo = :LocalEscopo,
    representante = :Representante,
    siglaArea = :SiglaArea,
    areaGestora = :AreaGestora,
    identificadorAreaGestora = :IdentificadorAreaGestora,
    siglaAreaGestora = :siglaAreaGestora,
    supervisoraAmbiental = :SupervisoraAmbiental,
    autorCadastramento = :autorCadastramento,
    responsavelTecnico = :responsavelTecnico,
    registroProfissional = :registroProfissional,
    documentoResponsabilidadeTecnica = :DocRespTec,
    naturezaPGA = :naturezaPGA,
    tipoFormulario = :tipoFormulario,
    dataEmissaoFormulario = :dataEmissaoFormulario,
    numeroFormulario = :numeroFormulario,
    autorFormulario = :autorFormulario,
    arquivoFDC = :arquivoFDC,
    codigoFDC = :codigoFDC,
    dataCadastramento = :dataCadastramento,
    horaCadastramento = :horaCadastramento,
    chavePrimaria = :chavePrimaria,
    elementoMonitoramentoNumero = :elementoMonitoramentoNumero,
    elementoMonitoramentoNome = :elementoMonitoramentoNome,
    municipio = :municipio,
    linhaCPTM = :linhaCPTM,
    nomeEstacaoCPTM = :nomeEstacaoCPTM,
    numeroViaLinha = :numeroViaLinha,
    trechoSentidoLinha = :trechoSentidoLinha,
    quilometroPoste = :quilometroPoste,
    latitude = :Latitude,
    longitude = :Longitude,
    atividade = :atividade,
    atividadeNaoListada = :atividadeNaoListada,
    draListado = :draListado,
    draNaoListada = :draNaoListada,
    codigoDRA = :codigoDRA,
    dataValidadeDRA = :dataValidadeDRA,
    TipoAtividadeCPTM = :TipoAtividadeCPTM,
    NomeEdificacao = :NomeEdificacao,
    NomeEdificacaoComplemento = :NomeEdificacaoComplemento,
    OrigemEfluente = :OrigemEfluente,
    FonteGeradoraEfluente = :FonteGeradoraEfluente,
    QuantidadeEfluente = :QuantidadeEfluente,
    TipoDestinacaoEfluente = :TipoDestinacaoEfluente,
    TipoVeiculo = :TipoVeiculo,
    IdentificadorVeiculo = :IdentificadorVeiculo,
    codigoGuiaRemessa = :codigoGuiaRemessa,
    distanciaViaCPTM = :distanciaViaCPTM,
    observacoesGerais = :observacoesGerais,
    fotografia1 = :Fotografia1,
    fotografia2 = :Fotografia2,
    fotografia3 = :Fotografia3,
    fotografia4 = :Fotografia4
   
    
WHERE Id = :Id";

                using var cmd = new OracleCommand(query, connection) { BindByName = true };

                cmd.Parameters.Add(new OracleParameter("NomeContratada", OracleDbType.Varchar2) { Value = formularioUpdateRequest.nomeContratada ?? (object)DBNull.Value });
                cmd.Parameters.Add(new OracleParameter("NumeroContrato", OracleDbType.Varchar2) { Value = formularioUpdateRequest.numeroContrato ?? (object)DBNull.Value });
                cmd.Parameters.Add(new OracleParameter("LocalEscopo", OracleDbType.Varchar2) { Value = formularioUpdateRequest.localEscopo ?? (object)DBNull.Value });
                cmd.Parameters.Add(new OracleParameter("Representante", OracleDbType.Varchar2) { Value = formularioUpdateRequest.representante ?? (object)DBNull.Value });
                cmd.Parameters.Add(new OracleParameter("SiglaArea", OracleDbType.Int32) { Value = formularioUpdateRequest.siglaArea ?? (object)DBNull.Value });
                cmd.Parameters.Add(new OracleParameter("AreaGestora", OracleDbType.Int32) { Value = formularioUpdateRequest.areaGestora ?? (object)DBNull.Value });
                cmd.Parameters.Add(new OracleParameter("IdentificadorAreaGestora", OracleDbType.Varchar2) { Value = formularioUpdateRequest.identificadorAreaGestora ?? (object)DBNull.Value });
                cmd.Parameters.Add(new OracleParameter("siglaAreaGestora", OracleDbType.Varchar2) { Value = formularioUpdateRequest.siglaAreaGestora ?? (object)DBNull.Value });
                cmd.Parameters.Add(new OracleParameter("SupervisoraAmbiental", OracleDbType.Varchar2) { Value = formularioUpdateRequest.supervisoraAmbiental ?? (object)DBNull.Value });
                cmd.Parameters.Add(new OracleParameter("autorCadastramento", OracleDbType.Varchar2) { Value = formularioUpdateRequest.autorCadastramento ?? (object)DBNull.Value });
                cmd.Parameters.Add(new OracleParameter("responsavelTecnico", OracleDbType.Varchar2) { Value = formularioUpdateRequest.responsavelTecnico ?? (object)DBNull.Value });
                cmd.Parameters.Add(new OracleParameter("registroProfissional", OracleDbType.Varchar2) { Value = formularioUpdateRequest.registroProfissional ?? (object)DBNull.Value });
                cmd.Parameters.Add(new OracleParameter("DocRespTec", OracleDbType.Varchar2) { Value = formularioUpdateRequest.DocRespTec ?? (object)DBNull.Value });
                cmd.Parameters.Add(new OracleParameter("naturezaPGA", OracleDbType.Int32) { Value = formularioUpdateRequest.naturezaPGA ?? (object)DBNull.Value });
                cmd.Parameters.Add(new OracleParameter("tipoFormulario", OracleDbType.Varchar2) { Value = formularioUpdateRequest.tipoFormulario ?? (object)DBNull.Value });
                cmd.Parameters.Add(new OracleParameter("dataEmissaoFormulario", OracleDbType.Date) { Value = formularioUpdateRequest.dataEmissaoFormulario ?? (object)DBNull.Value });
                cmd.Parameters.Add(new OracleParameter("numeroFormulario", OracleDbType.Int32) { Value = formularioUpdateRequest.numeroFormulario ?? (object)DBNull.Value });
                cmd.Parameters.Add(new OracleParameter("autorFormulario", OracleDbType.Varchar2) { Value = formularioUpdateRequest.autorFormulario ?? (object)DBNull.Value });
                cmd.Parameters.Add(new OracleParameter("arquivoFDC", OracleDbType.Varchar2) { Value = formularioUpdateRequest.arquivoFDC ?? (object)DBNull.Value });
                cmd.Parameters.Add(new OracleParameter("codigoFDC", OracleDbType.Int32) { Value = formularioUpdateRequest.codigoFDC ?? (object)DBNull.Value });
                cmd.Parameters.Add(new OracleParameter("dataCadastramento", OracleDbType.Date) { Value = formularioUpdateRequest.dataCadastramento ?? (object)DBNull.Value });
                cmd.Parameters.Add(new OracleParameter("horaCadastramento", OracleDbType.Varchar2, 8) { Value = formularioUpdateRequest.horaCadastramento.HasValue ? formularioUpdateRequest.horaCadastramento.Value.ToString("HH:mm:ss") : (object)DBNull.Value });
                cmd.Parameters.Add(new OracleParameter("chavePrimaria", OracleDbType.Varchar2) { Value = formularioUpdateRequest.chavePrimaria ?? (object)DBNull.Value });
                cmd.Parameters.Add(new OracleParameter("elementoMonitoramentoNumero", OracleDbType.Int32) { Value = formularioUpdateRequest.elementoMonitoramentoNumero ?? (object)DBNull.Value });
                cmd.Parameters.Add(new OracleParameter("elementoMonitoramentoNome", OracleDbType.Varchar2) { Value = formularioUpdateRequest.elementoMonitoramentoNome ?? (object)DBNull.Value });
                cmd.Parameters.Add(new OracleParameter("municipio", OracleDbType.Int32) { Value = formularioUpdateRequest.municipio ?? (object)DBNull.Value });
                cmd.Parameters.Add(new OracleParameter("linhaCPTM", OracleDbType.Int32) { Value = formularioUpdateRequest.linhaCPTM ?? (object)DBNull.Value });
                cmd.Parameters.Add(new OracleParameter("nomeEstacaoCPTM", OracleDbType.Int32) { Value = formularioUpdateRequest.nomeEstacaoCPTM ?? (object)DBNull.Value });
                cmd.Parameters.Add(new OracleParameter("numeroViaLinha", OracleDbType.Int32) { Value = formularioUpdateRequest.numeroViaLinha ?? (object)DBNull.Value });
                cmd.Parameters.Add(new OracleParameter("trechoSentidoLinha", OracleDbType.Int32) { Value = formularioUpdateRequest.trechoSentidoLinha ?? (object)DBNull.Value });
                cmd.Parameters.Add(new OracleParameter("quilometroPoste", OracleDbType.Varchar2) { Value = formularioUpdateRequest.quilometroPoste ?? (object)DBNull.Value });
                cmd.Parameters.Add(new OracleParameter("Latitude", OracleDbType.Decimal) { Value = formularioUpdateRequest.latitude ?? (object)DBNull.Value });
                cmd.Parameters.Add(new OracleParameter("Longitude", OracleDbType.Decimal) { Value = formularioUpdateRequest.longitude ?? (object)DBNull.Value });
                cmd.Parameters.Add(new OracleParameter("atividade", OracleDbType.Varchar2) { Value = formularioUpdateRequest.atividade ?? (object)DBNull.Value });
                cmd.Parameters.Add(new OracleParameter("atividadeNaoListada", OracleDbType.Varchar2) { Value = formularioUpdateRequest.atividadeNaoListada ?? (object)DBNull.Value });
                cmd.Parameters.Add(new OracleParameter("draListado", OracleDbType.Int32) { Value = formularioUpdateRequest.draListado ?? (object)DBNull.Value });
                cmd.Parameters.Add(new OracleParameter("draNaoListada", OracleDbType.Varchar2) { Value = formularioUpdateRequest.draNaoListada ?? (object)DBNull.Value });
                cmd.Parameters.Add(new OracleParameter("codigoDRA", OracleDbType.Int32) { Value = formularioUpdateRequest.codigoDRA ?? (object)DBNull.Value });
                cmd.Parameters.Add(new OracleParameter("dataValidadeDRA", OracleDbType.Date) { Value = formularioUpdateRequest.dataValidadeDRA ?? (object)DBNull.Value });
                cmd.Parameters.Add(new OracleParameter("TipoAtividadeCPTM", OracleDbType.Int32) { Value = formularioUpdateRequest.TipoAtividadeCPTM ?? (object)DBNull.Value });
                cmd.Parameters.Add(new OracleParameter("NomeEdificacao", OracleDbType.Int32) { Value = formularioUpdateRequest.NomeEdificacao ?? (object)DBNull.Value });
                cmd.Parameters.Add(new OracleParameter("NomeEdificacaoComplemento", OracleDbType.Varchar2) { Value = formularioUpdateRequest.NomeEdificacaoComplemento ?? (object)DBNull.Value });
                cmd.Parameters.Add(new OracleParameter("OrigemEfluente", OracleDbType.Int32) { Value = formularioUpdateRequest.OrigemEfluente ?? (object)DBNull.Value });
                cmd.Parameters.Add(new OracleParameter("FonteGeradoraEfluente", OracleDbType.Int32) { Value = formularioUpdateRequest.FonteGeradoraEfluente ?? (object)DBNull.Value });
                cmd.Parameters.Add(new OracleParameter("QuantidadeEfluente", OracleDbType.Double) { Value = formularioUpdateRequest.QuantidadeEfluente ?? (object)DBNull.Value });
                cmd.Parameters.Add(new OracleParameter("TipoDestinacaoEfluente", OracleDbType.Int32) { Value = formularioUpdateRequest.TipoDestinacaoEfluente ?? (object)DBNull.Value });
                cmd.Parameters.Add(new OracleParameter("TipoVeiculo", OracleDbType.Int32) { Value = formularioUpdateRequest.TipoVeiculo ?? (object)DBNull.Value });
                cmd.Parameters.Add(new OracleParameter("IdentificadorVeiculo", OracleDbType.Varchar2) { Value = formularioUpdateRequest.IdentificadorVeiculo ?? (object)DBNull.Value });
                cmd.Parameters.Add(new OracleParameter("codigoGuiaRemessa", OracleDbType.Varchar2) { Value = formularioUpdateRequest.codigoGuiaRemessa ?? (object)DBNull.Value });
                cmd.Parameters.Add(new OracleParameter("distanciaViaCPTM", OracleDbType.Double) { Value = formularioUpdateRequest.distanciaViaCPTM ?? (object)DBNull.Value });
                cmd.Parameters.Add(new OracleParameter("observacoesGerais", OracleDbType.Varchar2) { Value = formularioUpdateRequest.observacoesGerais ?? (object)DBNull.Value });

                cmd.Parameters.Add(new OracleParameter("Fotografia1", OracleDbType.Blob) { Value = formularioUpdateRequest.Fotografia1 ?? (object)DBNull.Value });
                cmd.Parameters.Add(new OracleParameter("Fotografia2", OracleDbType.Blob) { Value = formularioUpdateRequest.Fotografia2 ?? (object)DBNull.Value });
                cmd.Parameters.Add(new OracleParameter("Fotografia3", OracleDbType.Blob) { Value = formularioUpdateRequest.Fotografia3 ?? (object)DBNull.Value });
                cmd.Parameters.Add(new OracleParameter("Fotografia4", OracleDbType.Blob) { Value = formularioUpdateRequest.Fotografia4 ?? (object)DBNull.Value });

                cmd.Parameters.Add(new OracleParameter("Id", OracleDbType.Int32) { Value = formularioUpdateRequest.Id });



                var rowsAffected = await cmd.ExecuteNonQueryAsync();
                return rowsAffected > 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao atualizar formulário (Oracle): {ex.Message}");
                return false;
            }
        }


    }
            
}
