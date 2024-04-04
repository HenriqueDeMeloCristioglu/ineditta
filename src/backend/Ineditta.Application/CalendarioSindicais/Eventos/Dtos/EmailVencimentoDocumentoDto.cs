using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

using Ineditta.BuildingBlocks.Core.Web.API.Datatables.Attributes;

namespace Ineditta.Application.CalendarioSindicais.Eventos.Dtos
{
    public class EmailVencimentoDocumentoDto
    {
        public string? Abrangencia { get; set; }
        public string? NomeDocumento { get; set; } = null!;
        public string? AtividadesEconomicas { get; set; } = null!;
        public DateOnly? DataVencimento { get; set; }
        public string? SindicatoLaboral { get; set; }
        public string? SindicatoPatronal { get; set; }

    }
}
