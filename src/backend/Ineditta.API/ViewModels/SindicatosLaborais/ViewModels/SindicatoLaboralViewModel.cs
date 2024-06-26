﻿using Ineditta.BuildingBlocks.Core.Web.API.Datatables.Attributes;

namespace Ineditta.API.ViewModels.SindicatosLaborais.ViewModels
{
    public class SindicatoLaboralViewModel
    {
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        public int Id { get; set; }
        public string Cnpj { get; set; }
        public string RazaoSocial { get; set; }
        public string Municipio { get; set; }
        public string Uf { get; set; }
        public string Telefone { get; set; }
        public string Email { get; set; }
        public string Site { get; set; }
        public string Logradouro { get; set; }
        [NotSearchableDataTable]
        public string Sigla { get; set; }
        [NotSearchableDataTable]
        public string Codigo { get; set; }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    }
}
