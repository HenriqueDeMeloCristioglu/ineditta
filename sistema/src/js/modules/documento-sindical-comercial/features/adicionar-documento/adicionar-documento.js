import Result from "../../../../core/result";
import NotificationService from "../../../../utils/notifications/notification.service";
import { DocSindService } from "../../../../services";
import { ApiService } from "../../../../core/index";
import { uploadFile, incluirDocumentoNoBancoDados } from "./services";

const apiService = new ApiService();
const docSindService = new DocSindService(apiService);

export async function addDocumento(pageContext) {
  const { formulario } = pageContext;

  const prosseguirAdicao = await usuarioDesejaProsseguirAdicao();
  if (!prosseguirAdicao) return;

  const isPreenchimentoFormularioValido = formulario.instancia.isPreenchimentoValido();
  if (!isPreenchimentoFormularioValido) return;

  const uploadResult = await uploadFile(docSindService);
  const fileName = uploadResult.value.fileName;
  if (uploadResult.isFailure()) return;

  const incluirDocumentoNoBancoDadosResult = await incluirDocumentoNoBancoDados(pageContext, fileName, docSindService);
  if (incluirDocumentoNoBancoDadosResult.isFailure()) {
    NotificationService.error({
      title: "Falha ao realizar o cadastro do documento.", 
      message: incluirDocumentoNoBancoDadosResult.error
    });
    return Result.failure();
  }

  NotificationService.success({ title: "Cadastro realizado com sucesso!" });
  return Result.success();
}

async function usuarioDesejaProsseguirAdicao() {
  const desejaProsseguir =
    await new Promise
    ((resolve) => {
      NotificationService.success({
        title: "Tem certeza?",
        message:
          "Verifique se o documento disponibilizado está com as permissões adequadas para visualização dos demais usuários. Importante que sejam validados os perfis de usuários para compartilhamento de informações sensíveis ou confidenciais da empresa. (TAC, PLR, entre outras).",
        showConfirmButton: true,
        showCancelButton: true,
        confirmButtonColor: "#3085d6",
        cancelButtonColor: "#d33",
        confirmButtonText: "Sim, cadastrar",
        then: async (result) => {
          if (result.isConfirmed) {
            resolve(true);
          } else {
            resolve(false);
          }
        },
      });
    });

  return desejaProsseguir;
}

