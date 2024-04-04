import { renderizarModal } from "../../../../utils/modals/modal-wrapper";
import NotificationService from "../../../../utils/notifications/notification.service";

export class AbrangenciaModal {
    constructor(pageContext) {
        this.pageContext = pageContext;
    }

    configurarModal() {
        const { abrangenciasTb } = this.pageContext.formulario.datatables;

        const pageCtn = document.getElementById('pageCtn');
        const configuracao = this._gerarConfiguracao();

        renderizarModal(pageCtn, configuracao);
        $('#uf_input').on('change', async () => await abrangenciasTb?.content?.reload())
    }

    _gerarConfiguracao() {
        const abrangenciaModalHidden = document.getElementById('abrangenciaModalHidden')
        const abrangenciaModalContent = document.getElementById('abrangenciaModalContent')

        const buttonsModalConfig = [{
            id: "btn_reset_abrangencia",
            onClick: () => {
              $('#btn_add_abrangencia').attr('disabled', false)
            },
          }]

        const configuracaoModal = [{
            id: 'abrangenciaModal',
            modal_hidden: abrangenciaModalHidden,
            content: abrangenciaModalContent,
            btnsConfigs: buttonsModalConfig,
            onOpen: async () => await this._onOpenModal(),
            onClose: () => this._onCloseModal()
        }];

        return configuracaoModal;
    }

    async _onOpenModal() {
        const { abrangenciasTb } = this.pageContext.formulario.datatables;

        const existeCarregarAsync = !!abrangenciasTb.carregarAsync;

        if (existeCarregarAsync) {
            abrangenciasTb.carregarAsync();
        }
        else {
            NotificationService.error({
                title: "Datatable 'AbrangenciasTb' não inicializado! Somente datatables inicializados podem ser carregados"
            });
        }
    }

    _onCloseModal() {
        return null;
    }
}