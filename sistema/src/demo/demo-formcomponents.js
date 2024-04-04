// -------------------------------
// Demos: Form Components
// -------------------------------

$(function() {
    
    //SELECT2

    //For detailed documentation, see: http://ivaynberg.github.io/select2/index.html

    //Populate all select boxes with from select#source
    var opts=$("#source").html(), opts2="<option></option>"+opts;
    $("select.populate").each(function() { var e=$(this); e.html(e.hasClass("placeholder")?opts2:opts); });

    
    $("#e3").select2({
            minimumInputLength: 2,
            width: 'resolve'
        });

});







