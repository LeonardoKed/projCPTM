namespace CPTM_app_backend.Results
{
    public class FormularioResult
    {
        public int Id { get; set; }
        public string NomeContratada { get; set; }
        public string NumeroContrato { get; set; }
        public string LocalEscopo { get; set; }
        public string Representante { get; set; }
        
        public int? Usuario_id { get; set; }

        
    }
}
