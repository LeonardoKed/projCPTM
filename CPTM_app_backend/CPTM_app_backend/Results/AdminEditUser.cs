namespace CPTM_app_backend.Results
{
    public class AdminEditUser
    {        
        public string? Nome { get; set; }
        public string? Email { get; set; }
        public string? Senha { get; set; }
        public string? SenhaAntiga { get; set; }
        public string? Perfil { get; set; }
        public bool? Ativo { get; set; }
    }
}
