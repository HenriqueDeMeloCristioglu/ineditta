import SelectWrapper from "../../../../utils/selects/select-wrapper"
import {
  empresaPorUsuarioSelect,
  estabelecimentoPorUsuarioSelect,
  estruturaClausulasSelect,
  grupoClausulasSelect,
  grupoEconomicoPorUsuarioSelect,
  localizacaoEstabelecimentosPorUsuarioSelect,
  tipoDocumentoSelect
} from "../../../core"
import {
  useClausulaService,
  useCnaeService,
  useSindicatoLaboralService,
  useSindicatoPatronalService
} from "../../../core/hooks"
import $ from 'jquery'
import { obterOpcoesSindicatos } from "../obter-dados"

const cnaeService = useCnaeService()
const clausulaService = useClausulaService()
const sindicatoPatronalService = useSindicatoPatronalService()
const sindicatoLaboralService = useSindicatoLaboralService()

export async function configurarSelects(context) {
  const { datas, inputs: { selects } } = context 

  const isIneditta = datas.isIneditta
  const isGrupoEconomico = datas.isGrupoEconomico
  const isEstabelecimento = datas.isEstabelecimento
  const markOptionAsSelectable = datas.isCliente

  selects.gruposEconomicosSelect = grupoEconomicoPorUsuarioSelect({
    isIneditta,
    onChange: async () => {
      if (selects.cnaeSelect) await selects.cnaeSelect.reload()
      if (selects.sindLaboralSelect) await selects.sindLaboralSelect.reload()
      if (selects.sindPatronalSelect) await selects.sindPatronalSelect.reload()
      if (selects.dataBaseSelect) await selects.dataBaseSelect.reload()
    }
  })
  if (isIneditta) {
    selects.gruposEconomicosSelect.enable()
  } else {
    selects.gruposEconomicosSelect.disable()
    await selects.gruposEconomicosSelect.loadOptions()
  }

  selects.empresasSelect = empresaPorUsuarioSelect({
    isGrupoEconomico,
    isIneditta,
    onChange: async () => {
      if (selects.cnaeSelect) {
        await selects.cnaeSelect.reload()
      }
      if (selects.sindLaboralSelect) {
        await selects.sindLaboralSelect.reload()
      }
      if (selects.sindPatronalSelect) {
        await selects.sindPatronalSelect.reload()
      }
    }
  })
  if (isIneditta || isGrupoEconomico) {
    selects.empresasSelect.enable()
  } else {
    const options = await selects.empresasSelect.loadOptions()
    if (!(options instanceof Array && options.length > 1) || isEstabelecimento) {
      selects.empresasSelect.disable()
    }
    else {
      selects.empresasSelect.config.markOptionAsSelectable = () => false
      selects.empresasSelect.clear()
      selects.empresasSelect.enable()
    }
  }

  selects.estabelecimentosSelect = estabelecimentoPorUsuarioSelect({
    isEstabelecimento,
    onChange: async () => {
      if (selects.cnaeSelect) {
        await selects.cnaeSelect.reload()
      }
      if (selects.sindLaboralSelect) {
        await selects.sindLaboralSelect.reload()
      }
      if (selects.sindPatronalSelect) {
        await selects.sindPatronalSelect.reload()
      }
    }
  })
  if (isEstabelecimento) {
    const options = await selects.estabelecimentosSelect.loadOptions()

    if (!(options instanceof Array && options.length > 1)) {
      selects.estabelecimentosSelect.disable()
    }
    else {
      selects.estabelecimentosSelect.config.markOptionAsSelectable = () => false
      selects.estabelecimentosSelect?.clear()
      selects.estabelecimentosSelect.enable()
    }
  } else {
    selects.estabelecimentosSelect.enable()
  }

  selects.localizacoesSelect = localizacaoEstabelecimentosPorUsuarioSelect({
    markOptionAsSelectable,
    onChange: async () => {
      await selects.sindLaboralSelect.reload()
      await selects.sindPatronalSelect.reload()
    }
  })

  selects.cnaeSelect = new SelectWrapper("#cnaes", {
    options: { placeholder: "Selecione", multiple: true },
    onOpened: async () => {
      const options = {
        gruposEconomicosIds: $("#grupo").val() ?? null,
        matrizesIds: $("#matriz").val() ?? null,
        clientesUnidadesIds: $("#unidade").val() ?? null,
      }
      return await cnaeService.obterSelectPorUsuario(options)
    },
    onChange: async () => {
      await selects.sindLaboralSelect.reload()
      await selects.sindPatronalSelect.reload()
    },
    markOptionAsSelectable,
  })

  selects.sindPatronalSelect = new SelectWrapper("#sind_patronal", {
    options: { placeholder: "Selecione", multiple: true },
    onOpened: async () => {
      const localidades = $("#localidade").val()
      const options = obterOpcoesSindicatos(localidades)

      return await sindicatoPatronalService.obterSelectPorUsuario(options)
    },
    markOptionAsSelectable,
  })

  selects.sindLaboralSelect = new SelectWrapper("#sind_laboral", {
    options: { placeholder: "Selecione", multiple: true },
    onOpened: async () => {
      const localidades = $("#localidade").val()
      const options = obterOpcoesSindicatos(localidades)

      return await sindicatoLaboralService.obterSelectPorUsuario(options)
    },
    onChange: async () => {
      if (selects.dataBaseSelect) {
        await selects.dataBaseSelect.reload()
      }
    },
    markOptionAsSelectable,
  })

  selects.tipoDocSelect = tipoDocumentoSelect()
  selects.grupoClausulaSelect = grupoClausulasSelect()
  selects.estruturaClausulaSelect = estruturaClausulasSelect({ filter: { clausulaResumivel: true, clausulaNaoConsta: true } })
  selects.estruturaClausulaSelect.enable()

  selects.dataBaseSelect = new SelectWrapper("#data_base", {
    options: { allowEmpty: true },
    onOpened: async () => {
      let grupos
      if ($("#grupo").val()) {
        grupos = [$("#grupo").val()]
      }

      const requestData = {
        sindLaboralIds: $("#sind_laboral").val(),
        sindPatronalIds: $("#sind_patronal").val(),
        grupoEconomicoIds: grupos
      }

      return await clausulaService.obterDatasBases(requestData)
    },
  })
}