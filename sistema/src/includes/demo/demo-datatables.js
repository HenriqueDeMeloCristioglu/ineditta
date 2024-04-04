import $ from 'jquery';
import 'jquery-ui-dist/jquery-ui';
import 'jquery.nicescroll';
import 'jquery-toggles';
import 'jquery-sparkline';
import 'bootstrap';
import 'datatables';
import 'datatables-bootstrap';
import 'jquery-jeditable';
import 'datatables-tabletools';

// -------------------------------
// Initialize Data Tables
// -------------------------------

$(document).ready(function () {
	$('.datatables').dataTable({
		sDom: "<'row'<'col-xs-6'l><'col-xs-6'f>r>t<'row'<'col-xs-6'i><'col-xs-6'p>>",
		sPaginationType: 'bootstrap',
		oLanguage: {
			sLengthMenu: '_MENU_ registros por página',
			zeroRecords: 'Nada encontrado',
			info: 'Mostrando página _PAGE_ de _PAGES_',
			infoEmpty: 'Sem registros disponíveis',
			infoFiltered: '(filtrado de  um total de _MAX_ registros)',
			sSearch: '',
		},
	});
	$('.dataTables_filter input')
		.addClass('form-control')
		.attr('placeholder', 'Pesquisar...');
	$('.dataTables_length select').addClass('form-control');
});
