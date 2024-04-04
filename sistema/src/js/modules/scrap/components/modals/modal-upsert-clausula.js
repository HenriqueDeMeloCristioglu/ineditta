import { renderizarModal } from "../../../../utils/modals/modal-wrapper"
import {
  handleClickSalvarClausulaModal,
  handleCloseModalUpsertClausula,
  handleOpenModalUpsertClausula
} from "../../features"

export class ModalUpsertClausula {
  constructor({ pageContainer }) {
    this.pageContainer = pageContainer
  }

  render(context) {
    const clausulaModalHidden = document.getElementById('clausulaModalHidden')
    const clausulaModalContent = document.getElementById('clausulaModalContent')

    const modalsConfig = [
      {
        id: 'clausulaModal',
        modal_hidden: clausulaModalHidden,
        content: clausulaModalContent,
        btnsConfigs: [
          {
            id: 'btn_salvar_clausula',
            onClick: async (_, modalContainer) => await handleClickSalvarClausulaModal({ modalContainer, context })
          }
        ],
        onOpen: async () => await handleOpenModalUpsertClausula(context),
        onClose: () => handleCloseModalUpsertClausula(context),
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
    ]

    renderizarModal(this.pageContainer, modalsConfig)
  }
}