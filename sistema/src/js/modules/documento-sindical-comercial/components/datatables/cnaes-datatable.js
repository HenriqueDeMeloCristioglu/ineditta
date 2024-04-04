import DataTableWrapper from "../../../../utils/datatables/datatable-wrapper";
import NotificationService from "../../../../utils/notifications/notification.service";

export class CnaesTb {
  constructor(pageContext, cnaeService) {
    const { cnaesTb } = pageContext.formulario.datatables;

    this.pageContext = pageContext;
    this.cnaeService = cnaeService;

    this.carregarAsync = this.carregarAsync.bind(this);
    cnaesTb.carregarAsync = this.carregarAsync.bind(this);
  }

  async carregarAsync() {
    const { cnaesTb } = this.pageContext.formulario.datatables;

    if (cnaesTb.content) return;

    cnaesTb.content = new DataTableWrapper("#cnaesTb", {
      columns: [
        { data: "id", orderable: false, title: "Selecione" },
        { data: "subclasse", title: "Subclasse" },
        { data: "descricaoSubClasse", title: "Subclasse CNAE" },
      ],
      ajax: async (requestData) => {
        requestData.Columns = "id,subclasse,descricaoSubClasse";
        requestData.porGrupoDoUsuario = true;
        return await this.cnaeService.obterDatatable(requestData);
      },
      rowCallback: (row, data) => {
        const checkbox = $("<input>")
          .attr("type", "checkbox")
          .attr("data-id", data?.id)
          .addClass("cnaes");

        $("td:eq(0)", row).html(checkbox);

        if (cnaesTb.checkboxsSelecionados) {
          const ids = cnaesTb.checkboxsSelecionados.split(" ");
          const isChecked = ids.indexOf("" + data?.id);
          if (isChecked >= 0) {
            $(row).find(".cnaes").prop("checked", true);
          }
        }

        this._handlerCheckboxChange(row);
      },
    });

    await cnaesTb.content.initialize();
  }

  _handlerCheckboxChange(row) {
    const { cnaesTb } = this.pageContext.formulario.datatables;

    $(row).find('.cnaes').on('change', function () {
      const dataId = $(this).data("id")
  
      if ($(this).is(":checked")) {
        cnaesTb.checkboxsSelecionados += " " + dataId
  
        return NotificationService.success({ title: 'Atividade econômica selecionada!' })
      }
  
      cnaesTb.checkboxsSelecionados = (cnaesTb.checkboxsSelecionados + "").replace(' ' + dataId, '')
  
      NotificationService.success({ title: 'Atividade econômica desselecionada!' })
    })
  }
}
