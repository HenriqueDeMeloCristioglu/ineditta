$(function() {
    //////////////////////////////////
    //          ZOOM SCRIPT
    //////////////////////////////////
    const img = document.querySelectorAll("#zoom-active");

    img.forEach((element, index) => {
        $('.zoom-active-' + index).okzoom({
            width: 300,
            height: 300,
            border:"1px solid black",
            shadow:"0 0 5px #000"
        });
    });
    
    //////////////////////////////////
    //      ACCORDION SCRIPT
    //////////////////////////////////

    const accordion = document.querySelectorAll(".accordion-item");
    const accordionHeader = document.querySelectorAll(".accordion-item .accordion-header");

    for(var x = 0; x < accordionHeader.length; x++) {
        (function(index) {
            accordionHeader[index].addEventListener("click", () => {
                switch (index) {
                    case 0:
                        accordion[0].classList.toggle("open");
                        accordion[1].classList.remove("open");
                        accordion[2].classList.remove("open");
                    break;

                    case 1:
                        accordion[0].classList.remove("open");
                        accordion[1].classList.toggle("open");
                        accordion[2].classList.remove("open");
                    break;

                    case 2:
                        accordion[0].classList.remove("open");
                        accordion[1].classList.remove("open");
                        accordion[2].classList.toggle("open");
                    break;
                
                    default:
                        accordion[0].classList.remove("open");
                        accordion[1].classList.remove("open");
                        accordion[2].classList.remove("open");
                    break;
                }
            });
        })(x);
    };
});