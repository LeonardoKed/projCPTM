namespace CPTM_app_backend.Results
{
    public class AdminUserRequest
    {
        public string? Nome { get; set; }
        public string? Email { get; set; }
        public string? Perfil { get; set; }
        public bool? Ativo { get; set; }
        // mais filtros podem ser adicionados conforme necessário
    }
}
