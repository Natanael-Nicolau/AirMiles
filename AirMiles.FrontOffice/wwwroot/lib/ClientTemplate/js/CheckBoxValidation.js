//function checkInput(event) {
//    console.log("estou aqui");

//    var x = document.querySelectorAll("input[type=radio]");

//    for(var input in x)
//    {
//        input.checked = false;
//    }

//    event.target.checked = true;
//}

(function ($) {
    $(document).ready(function () {

        $("input[type=radio]").change(function () {
            $("input[type=radio]").prop("checked", false);
            $(this).prop("checked", true);
        })
    })
})(jQuery); 