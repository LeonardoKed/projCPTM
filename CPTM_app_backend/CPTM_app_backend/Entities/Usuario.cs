namespace CPTM_app_backend.Entities
{
    public class Usuario
    {
        public int Id { get; set; }
        public string UsuarioNomeApp { get; set; }
        public string Nome { get; set; }
        public string Sobrenome { get; set; }
        public string Email { get; set; }
        public string Senha { get; set; }
        public bool Ativo { get; set; }
        public string Perfil { get; set; }
    }
}
