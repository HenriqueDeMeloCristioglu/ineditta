import SelectWrapper from "../../../utils/selects/select-wrapper";

export class SelectSindicatoPatronalFactory {
    static Criar(sindicatoPatronalService, selectId) {
        const sindicatosPatronaisSelect = new SelectWrapper('#'+selectId, { onOpened: async () => obterSindicatosPatronaisSelect() });

        async function obterSindicatosPatronaisSelect() {
            const result = await sindicatoPatronalService.obterTodosComBaseExistente()
          
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

        return sindicatosPatronaisSelect;
    }
}