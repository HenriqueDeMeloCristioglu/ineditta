import $ from 'jquery';

// Demo for FullCalendar with Drag/Drop internal

$(document).ready(function() {

	var date = new Date();
	var d = date.getDate();
	var m = date.getMonth();
	var y = date.getFullYear();
	
	var calendar = $('#calendar-drag').fullCalendar({
		header: {
			left: 'prev,next today',
			center: 'title',
			right: 'month,agendaWeek,agendaDay'
		},
		selectable: true,
		selectHelper: true,
		select: function(start, end, allDay) {
			var title = prompt('Compromisso:');
			if (title) {
				calendar.fullCalendar('renderEvent',
					{
						title: title,
						start: start,
						end: end,
						allDay: allDay
					},
					true // make the event "stick"
				);
			}
			calendar.fullCalendar('unselect');
		},
		editable: true,		
		buttonText: {
			prev: '<i class="fa fa-angle-left"></i>',
			next: '<i class="fa fa-angle-right"></i>',
			prevYear: '<i class="fa fa-angle-double-left"></i>',  // <<
			nextYear: '<i class="fa fa-angle-double-right"></i>',  // >>
			today:    'Hoje',
			month:    'Mês',
			week:     'Semana',
			day:      'Dia'
		}
	});
	
});


