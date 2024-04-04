import "bootstrap"
import JQuery from "jquery"

import "../../js/utils/masks/jquery-mask-extensions.js"
import "../../js/utils/util.js"

import "datatables.net-bs5/css/dataTables.bootstrap5.css"
import "datatables.net-responsive-bs5/css/responsive.bootstrap5.css"
import "datatables.net-bs5"
import "datatables.net-responsive-bs5"

// Css libs
import "bootstrap/dist/css/bootstrap.min.css"
import "mark.js/dist/jquery.mark.es6.js"

// Core
import { AuthService } from "../../js/core/index.js"

import "../../js/main.js"
import { Menu } from "../../components/menu/menu.js"

import {
  configurarSelects,
  configurarFormulario,
  configurarPermissoesPagina,
  configurarConsultaUrl,
  configurarModal,
  buscaRapidaContext
} from "../../js/modules"

JQuery(async () => {
  new Menu()

  await AuthService.initialize()

  await configurarPermissoesPagina(buscaRapidaContext)
  await configurarSelects(buscaRapidaContext)
  configurarModal(buscaRapidaContext)
  configurarFormulario(buscaRapidaContext)
  await configurarConsultaUrl(buscaRapidaContext)
})
