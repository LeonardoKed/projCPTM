namespace CPTM_app_backend.Results
{
    public class FiltroFormularioRequest
    {
        public int? UsuarioId { get; set; }
        public string? NomeContratada { get; set; }
        public string? NumeroContrato { get; set; }
        public int? SiglaArea { get; set; }
        //public int? Municipio { get; set; }
        public DateTime? DataInicio { get; set; }
        //public DateTime? DataFim { get; set; }
    }
}
