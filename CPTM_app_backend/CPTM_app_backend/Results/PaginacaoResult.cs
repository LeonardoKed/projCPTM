namespace CPTM_app_backend.Results
{
    public class PaginacaoResult<T>
    {
        public List<T> Itens { get; set; }           // Os dados da página atual
        public int PaginaAtual { get; set; }         // Ex: 2
        public int TotalPaginas { get; set; }        // Ex: 10
        public int TotalRegistros { get; set; }      // Ex: 95 formulários
        public int ItensPorPagina { get; set; }      // Ex: 10
        public bool TemPaginaAnterior => PaginaAtual > 1;
        public bool TemProximaPagina => PaginaAtual < TotalPaginas;

        
    }
}
