import DataTableWrapper from "../../../../utils/datatables/datatable-wrapper"
import { useSindicatoService } from "../../../core/hooks"

const sindicatoService = useSindicatoService()

export async function dataTableInfoDiretoriaTb({
  context,
  params: {
    sindicatoId,
    tipoSindicato
  }
}) {
  const { dataTables } = context

  if (dataTables.diretoriaInfoSindTb) {
    dataTables.diretoriaInfoSindTb.reload()
    return
  }

  dataTables.diretoriaInfoSindTb = new DataTableWrapper("#diretoriainfosindtb", {
    ajax: async (requestData) =>
      await sindicatoService.obterInfoDiretoriaSindDatatable(
        requestData,
        sindicatoId,
        tipoSindicato
      ),
    columns: [
      { title: "Dirigente", data: "nome" },
      {
        title: "Início Mandato",
        data: "inicioMandato",
      },
      { title: "Fim Mandato", data: "fimMandato" },
      { title: "Função", data: "funcao" },
    ],
    columnDefs: [
      {
        targets: [1, 2],
        render: (data) => DataTableWrapper.formatDate(data),
      },
      {
        targets: "_all",
        defaultContent: "",
      },
    ],
  })

  await dataTables.diretoriaInfoSindTb.initialize()
}