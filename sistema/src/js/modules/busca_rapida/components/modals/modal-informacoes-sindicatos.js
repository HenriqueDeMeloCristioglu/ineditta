import { renderizarModal } from "../../../../utils/modals/modal-wrapper"
import { handleOpenInformacoesSindicatosModal, handleCloseInformacoesSindicatosModal } from "../../features/handlers/modals/informacoes-sindicatos"

export class ModalInformacoesSindicatos {
  constructor({ pageContainer }) {
    this.pageContainer = pageContainer
  }

  render(context) {
    const modal_hidden = document.getElementById("infoSindModalHidden")
    const content = document.getElementById("infoSindModalHiddenContent")

    renderizarModal(this.pageContainer, [
      {
        id: "infoSindModal",
        modal_hidden,
        content,
        btnsConfigs: [],
        onOpen: async () => await handleOpenInformacoesSindicatosModal(context),
        onClose: () => handleCloseInformacoesSindicatosModal(),
      }
    ])
  }
}
