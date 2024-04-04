using System.Text.Json.Serialization;

using Newtonsoft.Json;

namespace Ineditta.Application.Documentos.Sindicais.Dtos
{
    public class Estabelecimento
    {
        private Estabelecimento()
        {

        }
        public Estabelecimento(int id, string? nome, int grupoEconomicoId, int empresaId)
        {
            Id = id;
            Nome = nome;
            GrupoEconomicoId = grupoEconomicoId;
            EmpresaId = empresaId;
        }

        [JsonPropertyName("u")]
        [JsonProperty("u")]
        public int Id { get; private set; }
        [JsonPropertyName("nome_unidade")]
        [JsonProperty("nome_unidade")]
        public string? Nome { get; private set; }
        [JsonPropertyName("g")]
        [JsonProperty("g")]
        public int GrupoEconomicoId { get; private set; }
        [JsonPropertyName("m")]
        [JsonProperty("m")]
        public int EmpresaId { get; private set; }
    }
}
