import NotificationService from "../../../../../utils/notifications/notification.service";
import Result from "../../../../../core/result";
import DateParser from "../../../../../utils/date/date-parser";
import { converterNumSeparadosPorEspacosParaArray } from "../../../converters/converter-num-separados-por-espacos-para-array";

export async function incluirDocumentoNoBancoDados(pageContext, fileName, docSindService) {
  const requestData = _criarAdicionarDocumentoNoBancoDadosRequest(pageContext, fileName);

  const inclusaoResult = await docSindService.incluirDocumentoComercial(requestData);

  if (inclusaoResult.isFailure()) {
    NotificationService.error({ title: inclusaoResult.error });
    return Result.failure();
  }

  const documentoCadastradoId = inclusaoResult?.value?.result;

  return Result.success({ documentoCadastradoId });
}

function _criarAdicionarDocumentoNoBancoDadosRequest(pageContext, fileName) {
  const { usuariosTb, empresaTb, cnaesTb, abrangenciasTb } = pageContext.formulario.datatables;
  const { sindicatoLaboralSelect, sindPatronalSelect, assuntoSelect } = pageContext.formulario.selects;
  const { vigenciaInicialDate,  vigenciaFinalDate } = pageContext.formulario.datePickers;

  const usuariosIds = converterNumSeparadosPorEspacosParaArray(usuariosTb.checkboxsSelecionados);
  const empresasIds = converterNumSeparadosPorEspacosParaArray(empresaTb.checkboxsSelecionados);
  const cnaesIds = converterNumSeparadosPorEspacosParaArray(cnaesTb.checkboxsSelecionados);
  const abrangenciasIds = converterNumSeparadosPorEspacosParaArray(abrangenciasTb.checkboxsSelecionados);
  const arquivo = document.getElementById("file").files[0];

  const deveEnviarEmailsDeNotificacao = $("#anuencia").val() != "nao";
  const usuariosParaNotificarIds = usuariosTb.checkboxsSelecionados.split(" ").filter((id) => !!id);

  const requestData = {
    module: "documentos",
    action: "addDocumentos",
    origem: $("#origem_doc").val(),
    tipo: $("#nome_doc").val(),
    descricao: null,
    numeroLei: $("#numero_lei").val(),
    anuencia: $("#anuencia").val(),
    usuarios: usuariosIds,
    restrito: $("#restrito").val() == 'sim',
    clienteEstabelecimento: empresasIds,
    cnaesIds: cnaesIds,
    sindLaboral: sindicatoLaboralSelect?.getValue(),
    sindPatronal: sindPatronalSelect?.getValue(),
    validadeInicial: DateParser.toString(vigenciaInicialDate?.getValue()),
    validadeFinal: DateParser.toString(vigenciaFinalDate?.getValue()),
    ano: $("#year").val(),
    fonteLei: $("#fonte_site").val(),
    status: $("#status").val(),
    observacao: $("#comentarios").val(),
    usuario_cad: sessionStorage.getItem("iduser"),
    referencia: assuntoSelect?.getValue(),
    abrangencia: abrangenciasIds,
    caminhoArquivo: fileName,
    nomeArquivo: arquivo.name,
    usuariosParaNotificarIds: deveEnviarEmailsDeNotificacao ? usuariosParaNotificarIds : []
  };

  return requestData;
}