import { renderizarModal } from "../../../../utils/modals/modal-wrapper";
import NotificationService from "../../../../utils/notifications/notification.service";

export class AtividadeEconomicaModal {
    constructor(pageContext) {
        this.pageContext = pageContext;
    }

    configurarModal() {
        const pageCtn = document.getElementById('pageCtn');
        const configuracao = this._gerarConfiguracao();
        renderizarModal(pageCtn, configuracao);
    }

    _gerarConfiguracao() {
        const atividadeEconomicaModalHidden = document.getElementById('atividadeEconomicaModalHidden')
        const atividadeEconomicaModalContent = document.getElementById('atividadeEconomicaModalContent')

        const configuracaoModal = [{
            id: 'atividadeEconomicaModal',
            modal_hidden: atividadeEconomicaModalHidden,
            content: atividadeEconomicaModalContent,
            btnsConfigs: [],
            onOpen: async () => await this._onOpenModal(),
            onClose: () => this._onCloseModal()
        }];

        return configuracaoModal;
    }

    async _onOpenModal() {
        const { cnaesTb } = this.pageContext.formulario.datatables;

        const existeCarregarAsync = !!cnaesTb.carregarAsync;

        if (existeCarregarAsync) {
            cnaesTb.carregarAsync();
        }
        else {
            NotificationService.error({
                title: "Datatable 'cnaesTb' não inicializado! Somente datatables inicializados podem ser carregados"
            });
        }
    }

    _onCloseModal() {
        return null;
    }
}