using System.ComponentModel.DataAnnotations;

namespace CPTM_app_backend.Entities
{
    public class Formulario
    {
        public int Id { get; set; }
        [Required]
        public string nomeContratada { get; set; }// Pergunta 1
        [Required]
        public string numeroContrato { get; set; } // Pergunta 2
        [Required]
        public string localEscopo { get; set; } // Pergunta 3        
        [Required]
        public string representante { get; set; } // Pergunta 4        
        public int? siglaArea { get; set; } // Pergunta 5
        public int? areaGestora { get; set; } // Pergunta 6
        public string? identificadorAreaGestora { get; set; } // Pergunta 7
        public string? siglaAreaGestora { get; set; } // Pergunta 8
        public string? supervisoraAmbiental { get; set; } // Pergunta 9
        public string? autorCadastramento { get; set; } // Pergunta 10
        public string? responsavelTecnico { get; set; } // Pergunta 11
        public string? registroProfissional { get; set; } // Pergunta 12
        public string? DocRespTec { get; set; } // Pergunta 13
        public int? naturezaPGA { get; set; } // Pergunta 14
        public string? tipoFormulario { get; set; } // Pergunta 15
        public DateTime? dataEmissaoFormulario { get; set; } // Pergunta 16
        public int? numeroFormulario { get; set; } // Pergunta 17
        public string? autorFormulario { get; set; } // Pergunta 18
        public string? arquivoFDC { get; set; } // Pergunta 19
        public int? codigoFDC { get; set; } // Pergunta 20
        public DateTime? dataCadastramento { get; set; } // Pergunta 21
        public TimeOnly? horaCadastramento { get; set; } // Pergunta 22
        public string? chavePrimaria { get; set; } // Pergunta 23
        public int? elementoMonitoramentoNumero { get; set; } // Pergunta 24
        public string? elementoMonitoramentoNome { get; set; } // Pergunta 25
        public int? municipio { get; set; } // Pergunta 26
        public int? linhaCPTM { get; set; } // Pergunta 27
        public int? nomeEstacaoCPTM { get; set; } // Pergunta 28
        public int? numeroViaLinha { get; set; } // Pergunta 29
        public int? trechoSentidoLinha { get; set; } // Pergunta 30
        public string? quilometroPoste { get; set; } // Pergunta 31
        public decimal? latitude { get; set; } // Pergunta 32
        public decimal? longitude { get; set; } // Pergunta 33
        public string? atividade { get; set; } // Pergunta 34
        public string? atividadeNaoListada { get; set; } // Pergunta 35
        public int? draListado { get; set; } // Pergunta 36
        public string? draNaoListada { get; set; } // Pergunta 37
        public int? codigoDRA { get; set; } // Pergunta 38
        public DateTime? dataValidadeDRA { get; set; } // Pergunta 39
        public int? TipoAtividadeCPTM { get; set; } // Pergunta 40
        public int? NomeEdificacao { get; set; } // Pergunta 41
        public string? NomeEdificacaoComplemento { get; set; } // Pergunta 42
        public int? OrigemEfluente { get; set; } // Pergunta 43
        public int? FonteGeradoraEfluente { get; set; } // Pergunta 44
        public double? QuantidadeEfluente { get; set; } // Pergunta 45
        public int? TipoDestinacaoEfluente { get; set; } // Pergunta 46
        public int? TipoVeiculo { get; set; } // Pergunta 47
        public string? IdentificadorVeiculo { get; set; } // Pergunta 48
        public string? codigoGuiaRemessa { get; set; } // Pergunta 49
        public double? distanciaViaCPTM { get; set; } // Pergunta 50
        public string? observacoesGerais { get; set; } // Pergunta 51
        public byte[]? Fotografia1 { get; set; } // Pergunta 52
        public byte[]? Fotografia2 { get; set; } // Pergunta 53
        public byte[]? Fotografia3 { get; set; } // Pergunta 54
        public byte[]? Fotografia4 { get; set; } // Pergunta 55        
        public int? usuario_id { get; set; }


        // Getters e Setters
        // Construtores
    }
}
