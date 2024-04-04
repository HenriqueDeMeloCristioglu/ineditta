import { ApiService } from "../../../../core/index"
import { DocSindService, EstruturaClausulaService, SinonimosService } from "../../../../services"
import DatepickerWrapper from "../../../../utils/datepicker/datepicker-wrapper"
import { closeModal } from "../../../../utils/modals/modal-wrapper"
import SelectWrapper from "../../../../utils/selects/select-wrapper"
import { obterSinonimos } from "../../../clausula"
import $ from 'jquery'
import { handleChangeEstruturaClausula } from "../handlers"

const docSindService = new DocSindService(new ApiService())
const estruturaClausulaService = new EstruturaClausulaService(new ApiService())
const sinonimosService = new SinonimosService(new ApiService())

export function configurarFormularioModalEditarClausula(context) {
  const { selects, datePickers } = context.inputs

  selects.documentoSindicalSelect = new SelectWrapper('#documento_sindical', {
    onOrdenable: (data) => data.sort((a, b) => a.id - b.id),
    onOpened: async () => await docSindService.obterSelect(),
    options: {
      allowEmpty: true
    }
  })
  selects.documentoSindicalSelect.disable()

  selects.estruturaClausulaSelect = new SelectWrapper('#lista_clausula', {
    onOpened: async () => (await estruturaClausulaService.obterSelect()).value,
    onChange: async data => handleChangeEstruturaClausula(data, context),
    options: {
      allowEmpty: true
    }
  })

  selects.sinonimoSelect = new SelectWrapper('#sinonimo_select', {
    onOpened: async () => (await sinonimosService.obterSelect()).value,
    onSelected: async (value) => await obterSinonimos(value.id),
    options: { allowEmpty: true }
  })

  datePickers.dataInicialDt = new DatepickerWrapper('#vigencia_inicial')
  datePickers.dataFinalDt = new DatepickerWrapper('#vigencia_final')

  $("#informacao_adicional_grupo_painel").hide()
  $("#table-grupo-add").hide()
  $("#btn_voltar_modal_upsert").on('click', () => closeModal({ id: 'clausulaModal' }))
}
