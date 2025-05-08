using System.ComponentModel.DataAnnotations;

namespace DeviceManager.Web.Models
{
    public class Dispositivo
    {
        public string Id { get; set; }

        [Required(ErrorMessage = "A descrição é obrigatória")]
        [StringLength(100, ErrorMessage = "A descrição deve ter no máximo 100 caracteres")]
        public string Descricao { get; set; }

        [Required(ErrorMessage = "O código de referência é obrigatório")]
        [StringLength(50, ErrorMessage = "O código de referência deve ter no máximo 50 caracteres")]
        public string CodigoReferencia { get; set; }

        public DateTime DataCriacao { get; set; }

        public DateTime? DataAtualizacao { get; set; }
    }
}
