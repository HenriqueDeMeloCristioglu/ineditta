export class ModalInfoSindicato {
    constructor(renderizarModal, sindicatoService, DataTableWrapper) {
        this.renderizarModal = renderizarModal;
        this.sindicatoService = sindicatoService;
        this.datatableWrapper = DataTableWrapper;
    }

    initialize(divModalWrapper) {
        this._render(divModalWrapper);
        this._configurarModalInfoSindicato();
    }

    _render(divModalWrapper) {
        document.getElementById(divModalWrapper).innerHTML = /*HTML*/`
        <!-- MODAL INFO SINDICATOS -->
        <input type="hidden" id="sind-id-input">
        <input type="hidden" id="tipo-sind-input">
        <button style="display: none" id="openInfoSindModalBtn" data-toggle="modal" data-target="#infoSindModal"></button>
        <div class="hidden" id="infoSindModalHidden">
        <div id="infoSindModalHiddenContent">
            <div class="modal-content">
                <div class="modal-header">
                    <div style="display: flex; width: 100%; justify-content: space-between;">
                        <h4 class="modal-title" id="infoSindModalTitle">Informações Sindicais</h4>
                        <div class="dropdown" style="margin-left: 50%;">
                            <button class="btn btn-default dropdown-toggle" type="button" id="dropdownMenu2" data-toggle="dropdown"
                            aria-haspopup="true" aria-expanded="false">
                            Módulos <i class="fa fa-th"></i></span>
                            </button>
                            <ul class="dropdown-menu" aria-labelledby="dropdownMenu2">
                                <li><a href="#" id="direct-clausulas-btn">Consultar Cláusulas</a></li>
                                <li><a href="#" id="direct-comparativo-btn">Comparar Cláusulas</a></li>
                                <li><a href="#" id="direct-calendarios-btn">Calendário Sindical</a></li>
                                <li><a href="#" id="direct-documentos-btn">Consulta de documentos</a></li>
                                <li><a href="#" id="direct-sindicatos-btn">Sindicatos</a></li>
                                <li><a href="#" id="direct-gerar-excel-btn">Mapa Sindical (Excel)</a></li>
                                <li><a href="#" id="direct-formulario-aplicacao-btn">Mapa sindical (Formulário Aplicação)</a></li>
                                <li><a href="#" id="direct-comparativo-mapa-btn">Mapa sindical (Comparativo)</a></li>
                                <li><a href="#" id="direct-relatorio-negociacoes-btn">Negociação (Acompanhamento CCT Ineditta)</a></li>
                            </ul>
                        </div>

                        <button type="button" class="close" data-dismiss="modal" aria-hidden="true">&times;</button>
                    </div>
                </div>

                <div class="modal-body">
                    <form id="infoSindForm">
                    <div class="panel panel-primary">
                        <div class="panel-heading">
                            <h4>Dados cadastrais</h4>
                            <div class="options">
                                <a href="javascript:;" class="panel-collapse" id="collapseDadosCadastrais"><i
                                    class="fa fa-chevron-down"></i></a>
                            </div>
                        </div>

                        <div class="panel-body collapse in" id="collapseDadosCadastraisBody">
                            <div class="form-group">
                                <div class="col-sm-3">
                                    <label for="info-sigla">Sigla</label>
                                    <input class="col-sm-9 form-control" type="text" id="info-sigla" disabled>
                                </div>

                                <div class="col-sm-3">
                                    <label for="info-cnpj">CNPJ</label>
                                    <input class="col-sm-9 form-control" type="text" id="info-cnpj" disabled>
                                </div>

                                <div class="col-sm-6">
                                    <label for="info-razao">Razão Social</label>
                                    <input class="col-sm-8 form-control" type="text" id="info-razao" disabled>
                                </div>

                                <div class="col-sm-4">
                                    <label for="info-denominacao">Denominação</label>
                                    <input class="col-sm-9 form-control" type="text" id="info-denominacao" disabled>
                                </div>

                                <div class="col-sm-4">
                                    <label for="info-cod-sindical">Código Sindical</label>
                                    <input class="col-sm-8 form-control" type="text" id="info-cod-sindical" disabled>
                                </div>
                            </div>
                        </div>
                    </div>

                    <div class="panel panel-primary">
                        <div class="panel-heading">
                            <h4>Localização</h4>
                            <div class="options">
                                <a href="javascript:;" class="panel-collapse" id="collapseLocalizacao"><i
                                    class="fa fa-chevron-down"></i></a>
                            </div>
                        </div>

                        <div class="panel-body collapse in" id="collapseLocalizacaoBody">
                            <div class="form-group">
                                <div class="col-sm-2">
                                    <label for="info-uf">UF</label>
                                    <input class="col-sm-8 form-control" type="text" id="info-uf" disabled>
                                </div>

                                <div class="col-sm-3">
                                    <label for="info-municipio">Município</label>
                                    <input class="col-sm-9 form-control" type="text" id="info-municipio" disabled>
                                </div>

                                <div class="col-sm-4">
                                    <label for="info-logradouro">Logradouro</label>
                                    <input class="col-sm-9 form-control" type="text" id="info-logradouro" disabled>
                                </div>
                            </div>
                        </div>
                    </div>

                    <div class="panel panel-primary">
                        <div class="panel-heading">
                            <h4>Informações de Contato</h4>
                            <div class="options">
                                <a href="javascript:;" class="panel-collapse" id="collapseContato"><i
                                    class="fa fa-chevron-down"></i></a>
                            </div>
                        </div>

                        <div class="panel-body collapse in" id="collapseContatoBody">
                            <div class="form-group">
                                <div class="col-sm-3">
                                    <label for="info-telefone1">Telefone</label>
                                    <input class="col-sm-8 form-control" type="text" id="info-telefone1" disabled>
                                </div>

                                <div class="col-sm-3">
                                    <label for="info-telefone2">Telefone 2</label>
                                    <input class="col-sm-8 form-control" type="text" id="info-telefone2" disabled>
                                </div>

                                <div class="col-sm-3">
                                    <label for="info-telefone3">Telefone 3</label>
                                    <input class="col-sm-8 form-control" type="text" id="info-telefone3" disabled>
                                </div>

                                <div class="col-sm-3">
                                    <label for="info-ramal">Ramal</label>
                                    <input class="col-sm-9 form-control" type="text" id="info-ramal" disabled>
                                </div>

                                <div class="col-sm-4">
                                    <label for="info-enquadramento">Contato Enquadramento</label>
                                    <input class="col-sm-8 form-control" type="text" id="info-enquadramento" disabled>
                                </div>

                                <div class="col-sm-4">
                                    <label for="info-negociador">Contato Negociador</label>
                                    <input class="col-sm-8 form-control" type="text" id="info-negociador" disabled>
                                </div>

                                <div class="col-sm-4">
                                    <label for="info-contribuicao">Contato Contribuição</label>
                                    <input class="col-sm-8 form-control" type="text" id="info-contribuicao" disabled>
                                </div>

                                <div class="col-sm-4">
                                    <label for="info-email1">Email</label>
                                    <a id="info-email1-link" style="display: flex;" target="_blank">
                                        <input class="col-sm-9 form-control" type="text" id="info-email1" readonly>
                                    </a>
                                </div>

                                <div class="col-sm-4">
                                    <label for="info-email2">Email 2</label>
                                    <a id="info-email2-link" style="display: flex;" target="_blank">
                                        <input class="col-sm-9 form-control" type="text" id="info-email2" readonly>
                                    </a>
                                </div>

                                <div class="col-sm-4">
                                    <label for="info-email3">Email 3</label>
                                    <a id="info-email3-link" style="display: flex;" target="_blank">
                                        <input class="col-sm-9 form-control" type="text" id="info-email3" readonly>
                                    </a>
                                </div>

                                <div class="col-sm-4">
                                    <label for="info-twitter">Twitter</label>
                                    <a id="info-twitter-link" style="display: flex;" target="_blank">
                                        <input class="col-sm-9 form-control" type="text" id="info-twitter" readonly>
                                    </a>
                                </div>

                                <div class="col-sm-4">
                                    <label for="info-facebook">Facebook</label>
                                    <a id="info-facebook-link" style="display: flex;" target="_blank">
                                        <input class="col-sm-9 form-control" type="text" id="info-facebook" readonly>
                                    </a>
                                </div>

                                <div class="col-sm-4">
                                    <label for="info-instagram">Instagram</label>
                                    <a id="info-instagram-link" style="display: flex;" target="_blank">
                                        <input class="col-sm-9 form-control" type="text" id="info-instagram" readonly>
                                    </a>
                                </div>

                                <div class="col-sm-4">
                                    <label for="info-site">Site</label>
                                    <a id="info-site-link" style="display: flex;" target="_blank">
                                        <input class="col-sm-9 form-control" type="text" id="info-site" readonly>
                                    </a>
                                </div>
                            </div>
                        </div>
                    </div>

                    <div class="panel panel-primary">
                        <div class="panel-heading">
                            <h4>Associações</h4>
                            <div class="options">
                                <a href="javascript:;" class="panel-collapse" id="collapseAssociacoes"><i
                                    class="fa fa-chevron-down"></i></a>
                            </div>
                        </div>

                        <div class="panel-body collapse in" id="collapseAssociacoesBody">
                            <div class="form-group">
                                <div class="col-sm-6">
                                    <label for="info-federacao-nome">Nome Federação</label>
                                    <input class="col-sm-9 form-control" type="text" id="info-federacao-nome" disabled>
                                </div>

                                <div class="col-sm-6">
                                    <label for="info-federacao-cnpj">CNPJ Federação</label>
                                    <input class="col-sm-9 form-control" type="text" id="info-federacao-cnpj" disabled>
                                </div>

                                <div class="col-sm-6">
                                    <label for="info-confederacao-nome">Nome Confederação</label>
                                    <input class="col-sm-9 form-control" type="text" id="info-confederacao-nome" disabled>
                                </div>

                                <div class="col-sm-6">
                                    <label for="info-confederacao-cnpj">CNPJ Confederação</label>
                                    <input class="col-sm-9 form-control" type="text" id="info-confederacao-cnpj" disabled>
                                </div>

                                <div class="col-sm-6">
                                    <label for="info-central-sind-nome">Nome Central Sindical</label>
                                    <input class="col-sm-9 form-control" type="text" id="info-central-sind-nome" disabled>
                                </div>

                                <div class="col-sm-6">
                                    <label for="info-central-sind-cnpj">CNPJ Central Sindical</label>
                                    <input class="col-sm-9 form-control" type="text" id="info-central-sind-cnpj" disabled>
                                </div>
                            </div>
                        </div>
                    </div>

                    <div class="panel panel-primary">
                        <div class="panel-heading">
                            <h4>Diretoria</h4>
                            <div class="options">
                                <a href="javascript:;" class="panel-collapse" id="collapseDiretoria"><i
                                    class="fa fa-chevron-down"></i></a>
                            </div>
                        </div>

                        <div class="panel-body collapse in" id="collapseDiretoriaBody">
                            <div class="box text-shadow">
                                <table style="width: 100%" cellpadding="0" cellspacing="0" border="0"
                                class="table table-striped table-bordered demo-tbl" id="diretoriainfosindtb"
                                data-order='[[ 1, "asc" ]]'>
                                </table>
                            </div>
                        </div>
                    </div>
                    </form>
                </div>

                <div class="modal-footer">
                    <div class="row">
                        <div class="col-sm-12">
                            <div class="btn-toolbar" style="display: flex; justify-content: flex-end;">
                            <button type="button" data-toggle="modal" class="btn btn-danger btn-rounded btn-cancelar"
                                data-dismiss="modal">Voltar</a>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>

        <div class="modal-footer">
            <button data-toggle="modal" type="button" class="btn btn-secondary" data-dismiss="modal">Voltar</button>
        </div>
        </div>
        `
    }

    async _configurarModalInfoSindicato() {
        $("#infoSindForm").on('submit', (e) => e.preventDefault());
      
        const pageCtn = document.getElementById("page-content");
      
        const modalInfo = document.getElementById("infoSindModalHidden");
        const contentInfo = document.getElementById("infoSindModalHiddenContent");
      
        const buttonsInfoConfig = [];
      
        const modalsConfig = [
          {
            id: "infoSindModal",
            modal_hidden: modalInfo,
            content: contentInfo,
            btnsConfigs: buttonsInfoConfig,
            onOpen: async () => {
              const id = $("#sind-id-input").val();
              const tipoSind = $("#tipo-sind-input").val();
              if (id) {
                await this._obterInfoSindicatoPorId(id, tipoSind);
                await this._carregarDataTableInfoDiretoriaTb();
              }
            },
            onClose: () => {
              this._limparModalInfo();
            },
          },
        ];
      
        this.renderizarModal(pageCtn, modalsConfig);
    }
    
    async _obterInfoSindicatoPorId(id, tipoSind) {
        const infoResult = await this.sindicatoService.obterInfoSindical(id, tipoSind);
        
        this._preencherModalInfoSindical(infoResult.value, id, tipoSind);
    }

    _preencherModalInfoSindical(data, id, tipoSind) {
        this._limparModalInfo();
      
        $("#info-sigla").val(data.sigla);
        $("#info-cnpj").maskCNPJ().val(data.cnpj).trigger("input");
        $("#info-razao").val(data.razaoSocial);
        $("#info-denominacao").val(data.denominacao);
        $("#info-cod-sindical").val(data.codigoSindical);
        $("#info-uf").val(data.uf);
        $("#info-municipio").val(data.municipio);
        $("#info-logradouro").val(data.logradouro);
        $("#info-telefone1").maskCelPhone().val(data.telefone1).trigger("input");
        $("#info-telefone2").maskCelPhone().val(data.telefone2).trigger("input");
        $("#info-telefone3").maskCelPhone().val(data.telefone3).trigger("input");
        $("#info-ramal").val(data.ramal);
        $("#info-enquadramento").val(data.contatoEnquadramento);
        $("#info-negociador").val(data.contatoNegociador);
        $("#info-contribuicao").val(data.contatoContribuicao);
        $("#info-email1")
          .val(data.email1)
          .attr("style", data.email1 ? "cursor: pointer" : null);
        $("#info-email1-link").attr("href", `mailto:${data.email1}`);
        $("#info-email2")
          .val(data.email2)
          .attr("style", data.email2 ? "cursor: pointer" : null);
        $("#info-email2-link").attr("href", `mailto:${data.email2}`);
        $("#info-email3")
          .val(data.email3)
          .attr("style", data.email3 ? "cursor: pointer" : null);
        $("#info-email3-link").attr("href", `mailto:${data.email3}`);
        $("#info-twitter")
          .val(data.twitter)
          .attr("style", data.twitter ? "cursor: pointer" : null);
        $("#info-twitter-link").attr("href", this._formatarLinks(data.twitter, "twitter"));
        $("#info-facebook")
          .val(data.facebook)
          .attr("style", data.facebook ? "cursor: pointer" : null);
        $("#info-facebook-link").attr(
          "href",
          this._formatarLinks(data.facebook, "facebook")
        );
        $("#info-instagram")
          .val(data.instagram)
          .attr("style", data.instagram ? "cursor: pointer" : null);
        $("#info-instagram-link").attr(
          "href",
          this._formatarLinks(data.instagram, "instagram")
        );
        $("#info-site")
          .val(data.site)
          .attr("style", data.site ? "cursor: pointer" : null);
        $("#info-site-link").attr("href", this._formatarLinks(data.site, "site"));
        $("#info-data-base").val(data.dataBase);
        $("#info-ativ-econ").val(data.atividadeEconomica);
        $("#info-federacao-nome").val(data?.federacao?.nome);
        $("#info-federacao-cnpj")
          .maskCNPJ()
          .val(data?.federacao?.cnpj)
          .trigger("input");
        $("#info-confederacao-nome").val(data?.confederacao?.nome);
        $("#info-confederacao-cnpj")
          .maskCNPJ()
          .val(data?.confederacao?.cnpj)
          .trigger("input");
        $("#info-central-sind-nome").val(data?.centralSindical?.nome);
        $("#info-central-sind-cnpj")
          .maskCNPJ()
          .val(data?.centralSindical?.cnpj)
          .trigger("input");
      
        $("#direct-clausulas-btn").attr(
          "href",
          `consultaclausula.php?sindId=${id}&tipoSind=${tipoSind}&comparativo=${false}&sigla=${data.sigla
          }`
        );

        $("#direct-comparativo-btn").attr(
          "href",
          `consultaclausula.php?sindId=${id}&tipoSind=${tipoSind}&comparativo=${true}&sigla=${data.sigla
          }`
        );

        $("#direct-calendarios-btn").attr(
          "href",
          `calendario_sindical.php?sindId=${id}&tipoSind=${tipoSind}&sigla=${data.sigla}`
        );

        $("#direct-documentos-btn").attr(
          "href",
          `consulta_documentos.php?sindId=${id}&tipoSind=${tipoSind}&sigla=${data.sigla}`
        );

        $("#direct-sindicatos-btn").attr(
            "href",
            `modulo_sindicatos.php?sindId=${id}&tipoSind=${tipoSind}&sigla=${data.sigla}`
          );

        $("#direct-formulario-aplicacao-btn").attr(
          "href",
          `formulario_comunicado.php?sindId=${id}&tipoSind=${tipoSind}&sigla=${data.sigla}`
        );
      
        $("#direct-gerar-excel-btn").attr(
          "href",
          `geradorCsv.php?sindId=${id}&tipoSind=${tipoSind}&sigla=${data.sigla}`
        );
      
        $("#direct-comparativo-mapa-btn").attr(
          "href",
          `comparativo.php?sindId=${id}&tipoSind=${tipoSind}&sigla=${data.sigla}`
        );
      
        $("#direct-relatorio-negociacoes-btn").attr(
          "href",
          `relatorio_negociacoes.php?sindId=${id}&tipoSind=${tipoSind}&sigla=${data.sigla}`
        );
    }

    _limparModalInfo() {
        $("#infoSindForm").trigger("reset");
    }

    async _carregarDataTableInfoDiretoriaTb() {
        if (this.diretoriaInfoSindTb) {
          this.diretoriaInfoSindTb.reload();
          return;
        }
      
        this.diretoriaInfoSindTb = new this.datatableWrapper("#diretoriainfosindtb", {
          ajax: async (requestData) => {
            const id = $("#sind-id-input").val();
            const tipoSind = $("#tipo-sind-input").val();

            return await this.sindicatoService.obterInfoDiretoriaSindDatatable(
              requestData,
              id,
              tipoSind
            )},
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
              render: (data) => this.datatableWrapper.formatDate(data),
            },
            {
              targets: "_all",
              defaultContent: "",
            },
          ],
        });
      
        await this.diretoriaInfoSindTb.initialize();
    }

    _formatarLinks(string, tipo) {
        if (string == null) return "";
        if (string.includes(".com")) {
          if (string.includes("http")) {
            return string;
          } else {
            return `https://${string}`;
          }
        }
      
        if (tipo === "site") {
          if (string.includes("http")) {
            return string;
          } else {
            return `https://${string}`;
          }
        }
      
        if (tipo === "twitter") return `https://twitter.com/${string}`;
        if (tipo === "instagram") return `https://instagram.com/${string}`;
        if (tipo === "facebook") return `https://facebook.com/${string}`;
    }
}