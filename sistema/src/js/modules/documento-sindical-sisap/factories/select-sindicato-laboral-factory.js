import SelectWrapper from "../../../utils/selects/select-wrapper";

export class SelectSindicatoLaboralFactory {
    static Criar(sindicatoLaboralService, selectId) {
        const sindicatosLaboraisSelect = new SelectWrapper('#'+selectId, { onOpened: async () => obterSindicatosLaboraisSelect() });

        async function obterSindicatosLaboraisSelect() {
            const result = await sindicatoLaboralService.obterTodosComBaseExistente()
          
            if(result.isFailure()) return
          
            const options = []
          
            const data = result.value
            data.map(({ id, sigla, denominacao, cnpj }) => {
              options.push({
                id,
                description: `${id}-${sigla} / ${denominacao} / ${cnpj}`
              })
            })
          
            return options
          }

        return sindicatosLaboraisSelect;
    }
}