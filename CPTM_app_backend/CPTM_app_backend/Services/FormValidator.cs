using CPTM_app_backend.Entities;
using System.Security.Cryptography.X509Certificates;

namespace CPTM_app_backend.Services
{
    public class FormValidator
    {
        private readonly Formulario _formulario;

        public FormValidator(Formulario formulario)
        {
            _formulario = formulario;
        }

        public bool ValidarFormulario()
        {
            // Verificar se os campos obrigatórios estão preenchidos
            if (string.IsNullOrEmpty(_formulario.nomeContratada) ||
                string.IsNullOrEmpty(_formulario.numeroContrato) ||
                string.IsNullOrEmpty(_formulario.localEscopo) ||
                string.IsNullOrEmpty(_formulario.representante))
            {
                return false; // Formulário inválido
            }

            // Adicione outras validações específicas conforme necessário
            
            return true; // Formulário válido
        }
        

    }
}
