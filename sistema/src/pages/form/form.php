<!DOCTYPE html>
<!--[if IE 8]> <html lang="en" class="ie8"> <![endif]-->
<!--[if !IE]><!-->
<html lang="pt">
<!--<![endif]-->
<head>
	<meta charset="utf-8" />
	<title>Formulário</title>
	<meta content="width=device-width, initial-scale=1.0, maximum-scale=1.0, user-scalable=no" name="viewport" />
	<meta content="" name="description" />
	<meta content="" name="author" />

    <!-- Font Awesome -->
    <link rel="stylesheet" href="https://use.fontawesome.com/releases/v5.11.2/css/all.css">

	<link rel="stylesheet" href="https://fonts.googleapis.com/icon?family=Material+Icons">
	
	<!-- ================== BEGIN BASE CSS STYLE ================== -->
	<link href='includes/plugins/form-select2/select2.css' rel='stylesheet' /> 
	<link href="includes/plugins/jquery-ui/jquery-ui.min.css" rel="stylesheet" />
	<link href="includes/plugins/bootstrap/bootstrap4/css/bootstrap.min.css" rel="stylesheet" />
	<link href="includes/plugins/icon/themify-icons/themify-icons.css" rel="stylesheet" />
	<link href="includes/css/animate.min.css" rel="stylesheet" />
	<link href="includes/css/style.min.css" rel="stylesheet" />
	<link href="includes/css/theme/default.css" rel="stylesheet" id="theme" />
	<!-- ================== END BASE CSS STYLE ================== -->
	
	<script src="includes/js/jquery-3.4.1.min.js"></script>
	<script src="includes/js/popper.min.js"></script>

	
</head>
<body>
	<!-- BEGIN #page-container -->
		<div id="content" >
			 
		<div class="wizard-footer">
						<a href="#" class="btn btn-primary btn-rounded" onclick="addTicket();">Processar</a>
		</div>
	 
		</div>
			<!-- END wizard -->
 
	</div>
	
	

		
	<script src="includes/js/form.js"></script>
		
	<!-- ================== BEGIN BASE JS ================== -->
	<script type="text/javascript" src="includes/js/addons-MDB/jquery-3.4.1.min.js"></script>
	<script type="text/javascript" src="includes/js/addons-MDB/popper.min.js"></script>
	<script type="text/javascript" src="includes/js/addons-MDB/bootstrap.min.js"></script>
	<!-- ================== END BASE JS ================== -->

	<!-- MDB core JavaScript -->
	<script type="text/javascript" src="includes/js/addons-MDB/mdb.min.js"></script>
	<script type="text/javascript" src="includes/js/addons-MDB/datatables.min.js"></script>
	<script type="text/javascript" src="includes/js/addons-MDB/mdb-editor.js"></script>


	<!-- ================== BEGIN BASE JS ================== -->
	<script src="includes/plugins/scrollbar/slimscroll/jquery.slimscroll.min.js"></script>	
	<script type="text/javascript" src='includes/plugins/form-select2/select2.min.js'></script>
	<script src='includes/plugins/demo-formcomponents.js'></script> 
	<!-- ================== END BASE JS ================== -->
	
	<!-- ================== BEGIN PAGE LEVEL JS ================== -->
	<script type="text/javascript" src="includes/plugins/form/bootstrap-wizard/js/bootstrap-wizard.min.js"></script>
	<script type="text/javascript" src="includes/js/page/form-wizards.demo.min.js"></script>
	<!-- ================== END PAGE LEVEL JS ================== -->

	<script type='text/javascript' src='includes/plugins/form-inputmask/jquery.inputmask.bundle.min.js'></script> 
    <script type='text/javascript' src='includes/plugins/demo-mask.js'></script> 
	<script type='text/javascript' src='includes/js/jquery.mask.min.js'></script>
	
	<!-- The jQuery UI widget factory, can be omitted if jQuery UI is already included -->
    <script src="includes/plugins/file-upload-master/js/vendor/jquery.ui.widget.js"></script>
    <!-- The Templates plugin is included to render the upload/download listings -->
    <script src="https://blueimp.github.io/JavaScript-Templates/js/tmpl.min.js"></script>
    <!-- The Load Image plugin is included for the preview images and image resizing functionality -->
    <script src="https://blueimp.github.io/JavaScript-Load-Image/js/load-image.all.min.js"></script>
	
    <!-- The Iframe Transport is required for browsers without support for XHR file uploads -->
    <script src="includes/plugins/file-upload-master/js/jquery.iframe-transport.js"></script>
    <!-- The basic File Upload plugin -->
    <script src="includes/plugins/file-upload-master/js/jquery.fileupload.js"></script>
    <!-- The File Upload processing plugin -->
    <script src="includes/plugins/file-upload-master/js/jquery.fileupload-process.js"></script>
    <!-- The File Upload image preview & resize plugin -->
    <script src="includes/plugins/file-upload-master/js/jquery.fileupload-image.js"></script>
    <!-- The File Upload audio preview plugin -->
    <script src="includes/plugins/file-upload-master/js/jquery.fileupload-audio.js"></script>
    <!-- The File Upload video preview plugin -->
    <script src="includes/plugins/file-upload-master/js/jquery.fileupload-video.js"></script>
    <!-- The File Upload validation plugin -->
    <script src="includes/plugins/file-upload-master/js/jquery.fileupload-validate.js"></script>
    <!-- The File Upload user interface plugin -->
    <script src="includes/plugins/file-upload-master/js/jquery.fileupload-ui.js"></script>
    <!-- The main application script -->
	
	<script>
		$('.start').hide();
	</script>
	
	<!-- O modelo para exibir os arquivos disponíveis para upload -->
    <script id="template-upload" type="text/x-tmpl">
		$('.start').hide();
      {% for (var i=0, file; file=o.files[i]; i++) { %}
          <tr class="template-upload fade{%=o.options.loadImageFileTypes.test(file.type)?' image':''%}">
              <td>
                  <span class="preview"></span>
              </td>
              <td>
                  <p class="name">{%=file.name%}</p>
                  <strong class="error text-danger"></strong>
              </td>
              <td>
                  <p class="size">Processing...</p>
                  <div class="progress progress-striped active" role="progressbar" aria-valuemin="0" aria-valuemax="100" aria-valuenow="0"><div class="progress-bar progress-bar-success" style="width:0%;"></div></div>
              </td>
              <td>
                  {% if (!o.options.autoUpload && o.options.edit && o.options.loadImageFileTypes.test(file.type)) { %}
                    <button class="btn btn-success edit" data-index="{%=i%}" disabled>
                        <i class="glyphicon glyphicon-edit"></i>
                        <span>Edit</span>
                    </button>
                  {% } %}
                  {% if (!i && !o.options.autoUpload) { %}
                      <button class="btn btn-primary start" style="visibility: hidden;">
                          <i class="fas fa-upload"></i>
                          <span>Start</span>
                      </button>
                  {% } %}
                  {% if (!i) { %}
                      <button class="btn btn-warning cancel">
                          <i class="glyphicon glyphicon-ban-circle"></i>
                          <span>Cancel</span>
                      </button>
                  {% } %}
              </td>
          </tr>
      {% } %}
    </script>
	
	<!-- O modelo para exibir os arquivos disponíveis para download -->
    <script id="template-download" type="text/x-tmpl">
      {% for (var i=0, file; file=o.files[i]; i++) { %}
          <tr class="template-download fade{%=file.thumbnailUrl?' image':''%}">
              <td>
                  <span class="preview">
                      {% if (file.thumbnailUrl) { %}
                          <a href="{%=file.url%}" title="{%=file.name%}" download="{%=file.name%}" data-gallery><img src="{%=file.thumbnailUrl%}"></a>
                      {% } %}
                  </span>
              </td>
              <td>
                  <p class="name">
                      {% if (file.url) { %}
                          <a href="{%=file.url%}" title="{%=file.name%}" download="{%=file.name%}" {%=file.thumbnailUrl?'data-gallery':''%}>{%=file.name%}</a>
                      {% } else { %}
                          <span>{%=file.name%}</span>
                      {% } %}
                  </p>
                  {% if ( typeof file.error !== "undefined" ) { %}
                      <div><span class="label label-danger">Error</span> {%=file.error%}</div>
                  {% } %}
              </td>
              <td>
                  <span class="size">{%=o.formatFileSize(file.size)%}</span>
              </td>
              <td>
                  {% if (file.deleteUrl) { %}
                      <button class="btn btn-danger delete" data-type="{%=file.deleteType%}" data-url="{%=file.deleteUrl%}"{% if (file.deleteWithCredentials) { %} data-xhr-fields='{"withCredentials":true}'{% } %}>
                          <i class="glyphicon glyphicon-trash"></i>
                          <span>Delete</span>
                      </button>
                      <input type="checkbox" name="delete" value="1" class="toggle">
                  {% } %}
				  <a href="#" class="text-success"><i style="color: ;" class="glyphicon glyphicon glyphicon-ok"></i></a>	
              </td>
          </tr>
      {% } %}
    </script>
	
	<script>
		$(document).ready(function() {
			FormWizards.init();
			$('#Qtde').mask('000.000', {reverse: true});
		});
	</script>
	
	<script>
		$('#tabelaProdutos').dataTable({
			paging: false,
			searching: false,
			info: false,
			"language": {
				"lengthMenu": "Display _MENU_ records per page",
				"zeroRecords": "Nenhum produto solicitado",
				"info": "Quantidade de produtos inseridos _PAGE_ de _PAGES_",
				"infoEmpty": "Nenhum produto inserido na solicitação",
				"infoFiltered": "(filtered from _MAX_ total records)"
			}
	    });

		$('#tabelaProdutos').mdbEditor({
			modalEditor: true,
			headerLength: 8,
		});


		$('.dataTables_length').addClass('bs-select');
		
	  </script>	
</body>
</html>
