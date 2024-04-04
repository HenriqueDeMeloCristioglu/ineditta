import { stringButton, stringTd, stringTr } from "../../../../../../utils/components/string-elements";

export class InformacaoAdicionalGrupo {
  constructor() {}
  
  criar({ id, nomeTipoInformacao }) {
    return stringTr({
      content: stringTd({
        content: stringButton({
          className: 'btn btn-secondary btn_selecionar_informacao_adicional_grupo',
          config: "data-id=" + `${id}` + "",
          content: 'Selecionar'
        })
      }) + stringTd({ content: nomeTipoInformacao })
    })
  }
}