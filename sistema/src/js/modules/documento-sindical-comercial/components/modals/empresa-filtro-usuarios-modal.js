import { renderizarModal } from "../../../../utils/modals/modal-wrapper";
import NotificationService from "../../../../utils/notifications/notification.service";

export class EmpresaFiltroUsuarioModal {
    constructor(pageContext) {
        this.pageContext = pageContext;
    }

    configurarModal() {
        const pageCtn = document.getElementById('pageCtn');
        const configuracao = this._gerarConfiguracao();
        renderizarModal(pageCtn, configuracao);
    }

    _gerarConfiguracao() {
        const empresaFiltroUsuarioModalHidden = document.getElementById('empresaFiltroUsuarioModalHidden')
        const empresaFiltroUsuarioModalContent = document.getElementById('empresaFiltroUsuarioModalContent')

        const configuracaoModal = [{
            id: 'empresaFiltroUsuarioModal',
            modal_hidden: empresaFiltroUsuarioModalHidden,
            content: empresaFiltroUsuarioModalContent,
            btnsConfigs: [],
            styles: {
                container: {
                    zIndex: 50000
                },
                modal: {
                    zIndex: 50001
                }
            },
            onOpen: async () => await this._onOpenModal(),
            onClose: () => this._onCloseModal()
        }];

        return configuracaoModal;
    }

    async _onOpenModal() {
        const { empresaFiltroUsuarioTb } = this.pageContext.formulario.datatables;

        const existeCarregarAsync = !!empresaFiltroUsuarioTb.carregarAsync;

        if (existeCarregarAsync) {
            empresaFiltroUsuarioTb.carregarAsync();
        }
        else {
            NotificationService.error({
                title: "Datatable 'EmpresaFiltroUsuarioTb' não inicializado! Somente datatables inicializados podem ser carregados"
            });
        }
    }

    _onCloseModal() {
        return null;
    }
}