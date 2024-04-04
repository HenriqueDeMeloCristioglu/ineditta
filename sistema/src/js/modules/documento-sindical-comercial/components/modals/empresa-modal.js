import { renderizarModal } from "../../../../utils/modals/modal-wrapper";
import NotificationService from "../../../../utils/notifications/notification.service";

export class EmpresaModal {
    constructor(pageContext) {
        this.pageContext = pageContext;
    }

    configurarModal() {
        const pageCtn = document.getElementById('pageCtn');
        const configuracao = this._gerarConfiguracao();
        renderizarModal(pageCtn, configuracao);
    }

    _gerarConfiguracao() {
        const empresaModalHidden = document.getElementById('empresaModalHidden');
        const empresaModalContent = document.getElementById('empresaModalContent');

        const configuracaoModal = [{
            id: 'empresaModal',
            modal_hidden: empresaModalHidden,
            content: empresaModalContent,
            btnsConfigs: [],
            onOpen: async () => await this._onOpenModal(),
            onClose: () => this._onCloseModal()
        }];

        return configuracaoModal;
    }

    async _onOpenModal() {
        const { empresaTb } = this.pageContext.formulario.datatables;

        const existeEmpresaTbCarregarAsync = !!empresaTb.carregarAsync;

        if (existeEmpresaTbCarregarAsync) {
            empresaTb.carregarAsync();
        }
        else {
            NotificationService.error({
                title: "Datatable 'EmpresaTb' não inicializado! Somente datatables inicializados podem ser carregados"
            });
        }
    }

    _onCloseModal() {
        return null;
    }
}