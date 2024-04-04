import 'bootstrap/dist/css/bootstrap.min.css';
import 'bootstrap';

import JQuery from 'jquery';

import { AuthService } from '../../js/core/auth.js';
import { renderizarModal } from '../../js/utils/modals/modal-wrapper.js';

import '../../js/main.js'
import { Menu } from '../../components/menu/menu.js'

JQuery(async function () {
  new Menu()

  await AuthService.initialize()

  configurarModal()
})

function configurarModal() {
  const pageCtn = document.getElementById('pageCtn');

  // Add Task
  const addTaskModalHidden = document.getElementById('addTaskModalHidden');
  const addTaskModalHiddenContent = document.getElementById('addTaskModalContent');

  // Comentario
  const comentarioModalHidden = document.getElementById('comentarioModalHidden');
  const comentarioModalHiddenContent = document.getElementById('comentarioModalContent');

  const modalsConfig = [
    {
      id: 'addTaskModal',
      modal_hidden: addTaskModalHidden,
      content: addTaskModalHiddenContent,
      btnsConfigs: [],
      onOpen: null,
      onClose: null
    },
    {
      id: 'comentarioModal',
      modal_hidden: comentarioModalHidden,
      content: comentarioModalHiddenContent,
      btnsConfigs: [],
      onOpen: null,
      onClose: null
    }
  ];

  renderizarModal(pageCtn, modalsConfig);
}

