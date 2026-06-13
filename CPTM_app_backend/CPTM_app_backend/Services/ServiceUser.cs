using CPTM_app_backend.Entities;
using CPTM_app_backend.Repository;
using CPTM_app_backend.Results;

namespace CPTM_app_backend.Services
{
    public class ServiceUser
    {
        private readonly RepositoryUser _repositoryUser;
        public ServiceUser(RepositoryUser repositoryUser)
        {
            _repositoryUser = repositoryUser;
        }

        
        public Usuario Login(string email, string senha)
        {
            var usuario = _repositoryUser.GetUsuarioByEmail(email);

            if (usuario == null)
            {
                return null; // Retorna null se o usuário não for encontrado ou a senha estiver incorreta
            }

            // Verifica o hash com BCrypt
            var senhaValida = BCrypt.Net.BCrypt.Verify(senha ?? string.Empty, usuario.Senha ?? string.Empty);
            if (!senhaValida)
            {
                return null;
            }

            return usuario;
        }

        public async Task<Formulario> EnviarFormulario(Formulario formulario)
        {
           

            var form = await _repositoryUser.SalvarFormulario(formulario);

            return form;
        }

        public async Task<PaginacaoResult<FormularioResult>> BuscarPaginadoAsync(FiltroFormularioRequest filtro, int pagina, int itensPorPagina)
        {
            // Validações
            if (pagina < 1) pagina = 1;
            if (itensPorPagina < 1 || itensPorPagina > 100) itensPorPagina = 10;

            return await _repositoryUser.BuscarComFiltrosPaginadoAsync(filtro, pagina, itensPorPagina);
        }

        public async Task<Usuario> ObterPerfilUsuarioAsync(int usuarioId)
        {
            return await _repositoryUser.GetUsuarioById(usuarioId);

        }

        //Editar Usuario 
        public async Task<bool> EditarUsuarioById(int id, AdminEditUser usuarioEditado)
        {
            if (usuarioEditado == null) return false;

            var usuarioExistente = await _repositoryUser.GetUsuarioByIdAsync(id);
            if (usuarioExistente == null) return false;

            //aplicar somente campos enviados
            if (usuarioEditado.Nome != null) usuarioExistente.Nome = usuarioEditado.Nome;
            if (usuarioEditado.Email != null) usuarioExistente.Email = usuarioEditado.Email;
            if (usuarioEditado.Senha != null)
            {
                usuarioExistente.Senha = BCrypt.Net.BCrypt.HashPassword(usuarioEditado.Senha);
            }

            try
            {
                return await _repositoryUser.EditarUsuarioById(usuarioExistente);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao atualizar usuário no serviço: {ex.Message}");
                return false;
            }

        }

        // Pegar formulario por id e validar propriedade do usuário
        public async Task<Formulario> GetFormularioById(int id, int usuarioId)
        {
            // possível lugar para auditoria/validações adicionais
            return await _repositoryUser.GetFormularioByIdAsync(id, usuarioId);
        }

        // Retorna bytes da foto (index 1..4)
        public async Task<byte[]> GetFotoBytesAsync(int id, int usuarioId, int index)
        {
            return await _repositoryUser.GetFotoBytesAsync(id, usuarioId, index);
        }


        //Atualizar Formulario
        public async Task<bool> EditarFormularioById(int id, Formulario formularioEditado)
        {
            if (formularioEditado == null) return false;
            var formularioExistente = await _repositoryUser.GetFormularioByIdOnlyAsync(id);
            if (formularioExistente == null) return false;
            //aplicar somente campos enviados
            if (formularioEditado.nomeContratada != null) formularioExistente.nomeContratada = formularioEditado.nomeContratada;
            if (formularioEditado.numeroContrato != null) formularioExistente.numeroContrato = formularioEditado.numeroContrato;
            if (formularioEditado.localEscopo != null) formularioExistente.localEscopo = formularioEditado.localEscopo;
            if (formularioEditado.representante != null) formularioExistente.representante = formularioEditado.representante;
            if(formularioEditado.siglaArea != null) formularioExistente.siglaArea = formularioEditado.siglaArea;
            if (formularioEditado.areaGestora != null) formularioExistente.areaGestora = formularioEditado.areaGestora;
            if(formularioEditado.identificadorAreaGestora != null) formularioExistente.identificadorAreaGestora = formularioEditado.identificadorAreaGestora;
            if(formularioEditado.siglaAreaGestora != null) formularioExistente.siglaAreaGestora = formularioEditado.siglaAreaGestora;
            if(formularioEditado.supervisoraAmbiental != null) formularioExistente.supervisoraAmbiental = formularioEditado.supervisoraAmbiental;

            if (formularioEditado.autorCadastramento != null) formularioExistente.autorCadastramento = formularioEditado.autorCadastramento;
            if (formularioEditado.responsavelTecnico != null) formularioExistente.responsavelTecnico = formularioEditado.responsavelTecnico;
            if (formularioEditado.registroProfissional != null) formularioExistente.registroProfissional = formularioEditado.registroProfissional;
            if (formularioEditado.DocRespTec != null) formularioExistente.DocRespTec = formularioEditado.DocRespTec;
            if (formularioEditado.naturezaPGA != null) formularioExistente.naturezaPGA = formularioEditado.naturezaPGA;
            if (formularioEditado.tipoFormulario != null) formularioExistente.tipoFormulario = formularioEditado.tipoFormulario;
            if (formularioEditado.dataEmissaoFormulario != null) formularioExistente.dataEmissaoFormulario = formularioEditado.dataEmissaoFormulario;
            if (formularioEditado.numeroFormulario != null) formularioExistente.numeroFormulario = formularioEditado.numeroFormulario;
            if (formularioEditado.autorFormulario != null) formularioExistente.autorFormulario = formularioEditado.autorFormulario;
            if (formularioEditado.arquivoFDC != null) formularioExistente.arquivoFDC = formularioEditado.arquivoFDC;
            if (formularioEditado.codigoFDC != null) formularioExistente.codigoFDC = formularioEditado.codigoFDC;
            if (formularioEditado.dataCadastramento != null) formularioExistente.dataCadastramento = formularioEditado.dataCadastramento;
            if (formularioEditado.horaCadastramento != null) formularioExistente.horaCadastramento = formularioEditado.horaCadastramento;
            if (formularioEditado.chavePrimaria != null) formularioExistente.chavePrimaria = formularioEditado.chavePrimaria;
            if (formularioEditado.elementoMonitoramentoNumero != null) formularioExistente.elementoMonitoramentoNumero = formularioEditado.elementoMonitoramentoNumero;
            if (formularioEditado.elementoMonitoramentoNome != null) formularioExistente.elementoMonitoramentoNome = formularioEditado.elementoMonitoramentoNome;
            if (formularioEditado.municipio != null) formularioExistente.municipio = formularioEditado.municipio;
            if (formularioEditado.linhaCPTM != null) formularioExistente.linhaCPTM = formularioEditado.linhaCPTM;
            if (formularioEditado.nomeEstacaoCPTM != null) formularioExistente.nomeEstacaoCPTM = formularioEditado.nomeEstacaoCPTM;
            if (formularioEditado.numeroViaLinha != null) formularioExistente.numeroViaLinha = formularioEditado.numeroViaLinha;
            if (formularioEditado.trechoSentidoLinha != null) formularioExistente.trechoSentidoLinha = formularioEditado.trechoSentidoLinha;
            if (formularioEditado.quilometroPoste != null) formularioExistente.quilometroPoste = formularioEditado.quilometroPoste;
            if (formularioEditado.latitude != null) formularioExistente.latitude = formularioEditado.latitude;
            if (formularioEditado.longitude != null) formularioExistente.longitude = formularioEditado.longitude;
            if (formularioEditado.atividade != null) formularioExistente.atividade = formularioEditado.atividade;
            if (formularioEditado.atividadeNaoListada != null) formularioExistente.atividadeNaoListada = formularioEditado.atividadeNaoListada;
            if (formularioEditado.draListado != null) formularioExistente.draListado = formularioEditado.draListado;
            if (formularioEditado.draNaoListada != null) formularioExistente.draNaoListada = formularioEditado.draNaoListada;
            if (formularioEditado.codigoDRA != null) formularioExistente.codigoDRA = formularioEditado.codigoDRA;
            if (formularioEditado.dataValidadeDRA != null) formularioExistente.dataValidadeDRA = formularioEditado.dataValidadeDRA;
            if (formularioEditado.TipoAtividadeCPTM != null) formularioExistente.TipoAtividadeCPTM = formularioEditado.TipoAtividadeCPTM;
            if (formularioEditado.NomeEdificacao != null) formularioExistente.NomeEdificacao = formularioEditado.NomeEdificacao;
            if (formularioEditado.NomeEdificacaoComplemento != null) formularioExistente.NomeEdificacaoComplemento = formularioEditado.NomeEdificacaoComplemento;
            if (formularioEditado.OrigemEfluente != null) formularioExistente.OrigemEfluente = formularioEditado.OrigemEfluente;
            if (formularioEditado.FonteGeradoraEfluente != null) formularioExistente.FonteGeradoraEfluente = formularioEditado.FonteGeradoraEfluente;
            if (formularioEditado.QuantidadeEfluente != null) formularioExistente.QuantidadeEfluente = formularioEditado.QuantidadeEfluente;
            if (formularioEditado.TipoDestinacaoEfluente != null) formularioExistente.TipoDestinacaoEfluente = formularioEditado.TipoDestinacaoEfluente;
            if (formularioEditado.TipoVeiculo != null) formularioExistente.TipoVeiculo = formularioEditado.TipoVeiculo;
            if (formularioEditado.IdentificadorVeiculo != null) formularioExistente.IdentificadorVeiculo = formularioEditado.IdentificadorVeiculo;
            if (formularioEditado.codigoGuiaRemessa != null) formularioExistente.codigoGuiaRemessa = formularioEditado.codigoGuiaRemessa;
            if (formularioEditado.distanciaViaCPTM != null) formularioExistente.distanciaViaCPTM = formularioEditado.distanciaViaCPTM;
            if (formularioEditado.observacoesGerais != null) formularioExistente.observacoesGerais = formularioEditado.observacoesGerais;

            if (formularioEditado.Fotografia1 != null) formularioExistente.Fotografia1 = formularioEditado.Fotografia1;
            if (formularioEditado.Fotografia2 != null) formularioExistente.Fotografia2 = formularioEditado.Fotografia2;
            if (formularioEditado.Fotografia3 != null) formularioExistente.Fotografia3 = formularioEditado.Fotografia3;
            if (formularioEditado.Fotografia4 != null) formularioExistente.Fotografia4 = formularioEditado.Fotografia4;


            // ... continue para os outros campos conforme necessário
            try
            {
                return await _repositoryUser.AtualizarFormulario(formularioExistente);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao atualizar formulário no serviço: {ex.Message}");
                return false;
            }

        }


    }



        
}
