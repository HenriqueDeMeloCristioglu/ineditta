import { renderizarModal } from "../../../../utils/modals/modal-wrapper";
import NotificationService from "../../../../utils/notifications/notification.service";

export class NotificarModal {
    constructor(pageContext) {
        this.pageContext = pageContext;
    }

    configurarModal() {
        const pageCtn = document.getElementById('pageCtn');
        const configuracao = this._gerarConfiguracao();
        renderizarModal(pageCtn, configuracao);
    }

    _gerarConfiguracao() {
        const notificarModalHidden = document.getElementById('anuenciaModalHidden')
        const notificarModalContent = document.getElementById('anuenciaModalContent')

        const configuracaoModal = [{
            id: 'anuenciaModal',
            modal_hidden: notificarModalHidden,
            content: notificarModalContent,
            btnsConfigs: [],
            onOpen: async () => await this._onOpenModal(),
            onClose: () => this._onCloseModal()
        }];

        return configuracaoModal;
    }

    async _onOpenModal() {
        const { usuariosTb } = this.pageContext.formulario.datatables;

        const existeCarregarAsync = !!usuariosTb.carregarAsync;

        if (existeCarregarAsync) {
            usuariosTb.carregarAsync();
        }
        else {
            NotificationService.error({
                title: "Datatable 'usuariosTb' não inicializado! Somente datatables inicializados podem ser carregados"
            });
        }
    }

    _onCloseModal() {
        return null;
    }
}