import $ from "jquery";
import jQuery from "jquery";

import 'bootstrap';
import 'datatables.net-bs5/css/dataTables.bootstrap5.css';
import 'datatables.net-responsive-bs5/css/responsive.bootstrap5.css';
import 'bootstrap/dist/css/bootstrap.min.css';

import '../../js/main.js'

// Core
import { UserInfoService } from "../../js/core/index.js"
import { ApiLegadoService } from "../../js/core/api-legado"

import NotificationService from "../../js/utils/notifications/notification.service";


const apiLegadoService = new ApiLegadoService();

jQuery(() => {
  $("#aceitarBtn").on("click", async () => {
    const dataRequest = {
      module: "usuario",
      action: "acceptTerms",
      user: UserInfoService.getDataUser(),
    };

    const result = await apiLegadoService.post("includes/php/ajax.php", dataRequest, {
      "Content-Type": "application/x-www-form-urlencoded",
    });

    if (result.isFailure()) {
      NotificationService.error({
        title: "Não foi possível realizar o cadastro! Erro: ",
        message: result.error,
      });

      return result;
    }

    NotificationService.success({
      title: "Confirmado com sucesso!",
      message: "",
      timer: 1500,
    });

    if (result.value.response_data) {
      let path = result.value.response_data.path;

      setTimeout(() => {
        window.location.pathname = `/${path}`;
      }, 1510);
    } else {
      NotificationService.error({
        title: "Usuário não autenticado",
        showConfirmButton: false,
        timer: 1500,
      })


      setTimeout(() => {
        window.location.pathname = "/index.php";
      }, 1510);
    }
  });
});
