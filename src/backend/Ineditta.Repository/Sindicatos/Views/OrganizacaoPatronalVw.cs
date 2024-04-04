using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Ineditta.BuildingBlocks.Core.Domain.ValueObjects;

namespace Ineditta.Repository.Sindicatos.Views
{
    public class OrganizacaoPatronalVw
    {
        public int SindicatoPatronalId { get; set; }
        public string? Municipio { get; set; }
        public string? Sigla { get; set; }
        public string? Cnpj { get; set; }
        public string? NomeConfederacao { get; set; }
        public string? NomeFederacao { get; set; }
        public string? Associado { get; set; }
    }
}
