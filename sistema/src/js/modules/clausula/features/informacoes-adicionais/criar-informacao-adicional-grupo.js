import { stringButton, stringTd, stringTr } from "../../../../utils/components/string-elements"

export function criarInformacaoAdicionalGrupo(infoAdicional) {
  const { id, nomeTipoInformacao } = infoAdicional

  return stringTr({
    content: stringTd({
      content: stringButton({
        className: 'btn btn-secondary btn_informacao_adicional',
        config: "data-id=" + `${id}` + "",
        content: 'Selecionar'
      })
    }) + stringTd({
      content: nomeTipoInformacao
    })
  })
}