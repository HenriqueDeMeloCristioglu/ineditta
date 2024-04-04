import { stringButton, stringI, stringTd, stringTr } from "../../../../../utils/components/string-elements";

export class InformacaoAdicionalGrupoSelecionado {
  constructor() {}
  
  criar ({ nomeTipoInformacao, id }) {
    return stringTr({
      id,
      className: 'infoGrupo',
      content: stringTd({ content: nomeTipoInformacao }) + stringTd({}) + stringTd({
        style: 'height: 55px; display: flex; justify-content:center; align-items:center;',
        content: stringButton({
          style: 'color: red; border:none; background-color: transparent;',
          className: 'btn_remover_clausula_adicional',
          config: 'data-id="' + id + '"',
          content: stringI({ className: 'fa fa-times', config: 'data-id="' + id + '"' })
        })
      })
    })
  }
}