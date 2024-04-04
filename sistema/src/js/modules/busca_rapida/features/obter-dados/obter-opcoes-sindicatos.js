import $ from 'jquery'

export function obterOpcoesSindicatos(localidades) {
  const localizacoesIds = localidades.map((loc) => {
    if (loc.includes("municipio")) {
      return Number(loc.split(":")[1])
    }

    return null
  })
  .filter((x) => x !== null)

  const ufs = localidades.map((loc) => {
    if (loc.includes("uf")) {
      return loc.split(":")[1]
    }

    return null
  })
  .filter((x) => x !== null)

  const options = {
    gruposEconomicosIds: $("#grupo").val() ?? null,
    matrizesIds: $("#matriz").val() ?? null,
    clientesUnidadesIds: $("#unidade").val() ?? null,
    cnaesIds: $("#categoria").val() ?? null,
    localizacoesIds,
    ufs,
  }

  return options
}