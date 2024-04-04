import { renderizarModal } from "../../../../utils/modals/modal-wrapper"
import { handleCloseModalListaClausula, handleOpenModalListaClausula } from "../../features"

export class ModalListaClausula {
  constructor({ pageContainer }) {
    this.pageContainer = pageContainer
  }

  render(context) {
    const listaClausulasModalHidden = document.getElementById('listaClausulasModalHidden')
    const listaClausulasModalContent = document.getElementById('listaClausulasModalContent')

    renderizarModal(this.pageContainer, [
      {
        id: 'listaClausulasModal',
        modal_hidden: listaClausulasModalHidden,
        content: listaClausulasModalContent,
        btnsConfigs: [],
        onOpen: async () => await handleOpenModalListaClausula(context),
        onClose: () => handleCloseModalListaClausula(context),
        styles: {
          container: {
            paddingRight: '30px',
            paddingLeft: '30px'
          },
          modal: {
            maxWidth: '95%',
            width: '100%'
          }
        }
      }
    ])
  }
}