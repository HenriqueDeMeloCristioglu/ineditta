import $ from 'jquery'
import SelectWrapper from "../../../utils/selects/select-wrapper";
import DatepickerWrapper from "../../../utils/datepicker/datepicker-wrapper";
import NotificationService from "../../../utils/notifications/notification.service";
import { addDocumento } from "../features/adicionar-documento";

export class FormularioInclusao {
    constructor(
        pageContext, 
        estruturaClausulaService, 
        sindicatoPatronalService, 
        sindicatoLaboralService,
        tipoDocService
    ) {
        this.pageContext = pageContext;
        this.estruturaClausulaService = estruturaClausulaService;
        this.sindicatoPatronalService = sindicatoPatronalService;
        this.sindicatoLaboralService = sindicatoLaboralService;
        this.tipoDocService = tipoDocService;
    }

    inicializar() {
        this._configurarSelects();
        this._configurarDatepicker();

        $("#year").mask('0000');
        $("#vigencia_inicial").maskDate();
        $("#vigencia_final").maskDate();
      
        $("#numero_lei").attr("disabled", true);
        $("#fonte_site").attr("disabled", true);
      
        $('#btn-cadastrar-doc').on('click', async () => await this._submitHandler());
    }

    _configurarSelects() {
        const { selects } = this.pageContext.formulario;

        selects.sindPatronalSelect = new SelectWrapper('#sind_patronal', { 
          options: { placeholder: 'Selecione', multiple: true }, 
          onOpened: async () => await this.sindicatoPatronalService.obterSelectPorUsuario()
        });

        selects.sindicatoLaboralSelect = new SelectWrapper('#sind_laboral', { options: { placeholder: 'Selecione', multiple: true }, onOpened: async () => await this.sindicatoLaboralService.obterSelectPorUsuario() });
        
        selects.tipoDocSelect = new SelectWrapper('#tipo_doc', { 
          onOpened: async () => {
            selects.nomeDocSelect?.enable();
            if (selects.tipoDocSelect?.getValue() == "Legislação") {
              $("#numero_lei").attr("disabled", false);
              $("#fonte_site").attr("disabled", false);
            } else {
              $("#numero_lei").attr("disabled", true);
              $("#fonte_site").attr("disabled", true);
            }
            return (await this.tipoDocService.obterTiposSelect()).value?.sort((a,b) => a.description > b.description ? 1 : -1)
          },
          onChange: () => {
            if (selects.tipoDocSelect?.getValue() == "Legislação") {
              selects.sindicatoLaboralSelect?.clear();
              selects.sindicatoLaboralSelect?.disable();
      
              selects.sindPatronalSelect?.clear();
              selects.sindPatronalSelect?.disable();
      
              $("#numero_lei").attr("disabled", false);
              $("#fonte_site").attr("disabled", false);
            }else { 
              selects.sindicatoLaboralSelect?.enable();
              selects.sindPatronalSelect?.enable();
      
              $("#numero_lei").val("");
              $("#fonte_site").val("");
              $("#numero_lei").attr("disabled", true);
              $("#fonte_site").attr("disabled", true);
            }
            selects.nomeDocSelect?.enable();
            selects.nomeDocSelect?.reload();
          } 
        });

        selects.nomeDocSelect = new SelectWrapper('#nome_doc', { onOpened: async () => (await this._obterNomesDoc())});

        selects.assuntoSelect = new SelectWrapper('#assunto', { onOpened: async () => (await this.estruturaClausulaService.obterSelect()).value });
      
        selects.nomeDocSelect?.disable();

        $('#anuencia').on('change', function () {
            if ($('#anuencia').val() === 'sim') return $("#anuenciaModalBtn").removeClass("disabled")
        
            $("#anuenciaModalBtn").addClass("disabled")
        })
    }

    async _obterNomesDoc() {
      const { tipoDocSelect } = this.pageContext.formulario.selects;

      if(tipoDocSelect.getValue()) {
        return (await this.tipoDocService.obterSelectPorTipos({tipos: [], tiposDocumentosIds: [tipoDocSelect.getValue()], processado: false}));
      } 
      else {
        return (await this.tipoDocService.obterSelect({ processado: false, filtrarSelectType: true })).value;
      }
    }

    async _submitHandler() {
      const addDocumentoResult = await addDocumento(this.pageContext);

      if (addDocumentoResult.isFailure()) return;

      this.limparFormulario();
    }

    _configurarDatepicker() {
        const { datePickers } = this.pageContext.formulario;
        datePickers.vigenciaInicialDate = new DatepickerWrapper('#vigencia_inicial');
        datePickers.vigenciaFinalDate = new DatepickerWrapper('#vigencia_final');
    }

    limparFormulario() {
        const { nomeDocSelect, sindicatoLaboralSelect, sindPatronalSelect, assuntoSelect, tipoDocSelect } = this.pageContext.formulario.selects;
        const { vigenciaInicialDate, vigenciaFinalDate } = this.pageContext.formulario.datePickers;
        const { empresaTb, empresaFiltroUsuarioTb, abrangenciasTb, cnaesTb, usuariosTb } = this.pageContext.formulario.datatables;

        $("#file").val("");
        $("#desc_doc").val("");
        $("#anuencia").val("");
        $("#numero_lei").val("");
        $("#fonte_site").val("");
        $("#restrito").val("");
        $("#comentarios").val("");
        $("#uf_input").val('0');
      
        sindPatronalSelect.clear();
        sindicatoLaboralSelect.clear();
        nomeDocSelect.clear();
        tipoDocSelect.clear();
        assuntoSelect.clear();

        vigenciaFinalDate.clear();
        vigenciaInicialDate.clear();

        usuariosTb.checkboxsSelecionados = "";
        empresaTb.checkboxsSelecionados = "";
        empresaFiltroUsuarioTb.checkboxsSelecionados = "";
        cnaesTb.checkboxsSelecionados = "";
        abrangenciasTb.checkboxsSelecionados = "";
      
        $("#seleciona_todas_empresas").prop("checked", false);
        $("#seleciona_todas_regioes").prop("checked", false);
      
        empresaTb?.content?.reload();
        empresaFiltroUsuarioTb?.content?.reload();
        abrangenciasTb?.content?.reload();
        usuariosTb?.content?.reload();
        cnaesTb?.content?.reload();
    }

    isPreenchimentoValido() {
      var isValid = true;

      $("#form_add input[required]").each(function () {
        if (!$(this).val()) {
          $(this).addClass("is-invalid");
          isValid = false;
        } else {
          $(this).removeClass("is-invalid");
        }
      });

      $("#form_add select[required]").each(function () {
        if (!$(this).val() || $(this).val() == "") {
          $(this).addClass("is-invalid");
          isValid = false;
        } else {
          $(this).removeClass("is-invalid");
        }
      });

      if (!isValid) {
        NotificationService.error({
          title:
            "Erro no preenchimento. Por favor revise os campos: Nome do Documento e Selecionar Documento.",
        });
        return false;
      }

      return true;
    }
}