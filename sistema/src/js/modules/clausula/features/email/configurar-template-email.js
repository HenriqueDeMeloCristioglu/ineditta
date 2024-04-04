import { TemplateEmailDocumentoLiberado } from "../../../../utils/components/emails/templates"

export function configurarTemplateEmail({ documentoSelecionado, nomeGrupoEconomico, uri, gestaoDeChamadosUrl }) {
  const cnaes = []
  documentoSelecionado?.cnaeDoc.map(({ subclasse }) => {
    cnaes.push(subclasse)
  })

  const sindicatosLaborais = []
  documentoSelecionado?.sindLaboral.map(({ sigla, denominacao }) => {
    sindicatosLaborais.push(sigla + " / " + denominacao)
  })

  const sindicatosPatronais = []
  if (documentoSelecionado.sindPatronal && documentoSelecionado.sindPatronal.length > 0) {
    documentoSelecionado?.sindPatronal.map(({ sigla, denominacao }) => {
      sindicatosPatronais.push(sigla + " / " + denominacao)
    })
  }

  const abrangencia = []
  documentoSelecionado?.abrangencia.map(({ uf, municipio }) => {
    abrangencia.push(`${municipio}/${uf}`)
  })

  const templateEmailDocumentoLiberado = new TemplateEmailDocumentoLiberado({
    nomeGrupoEconomico,
    documentoSelecionado,
    cnaes,
    sindicatosLaborais,
    sindicatosPatronais,
    pdfLink: uri,
    chamadoLink: gestaoDeChamadosUrl,
    abrangencia: abrangencia.join(', ')
  })

  return templateEmailDocumentoLiberado.criar()
}