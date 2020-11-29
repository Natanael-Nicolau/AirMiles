(function ($) {
    $(document).ready(function () {

        $("input[type=radio]").change(function () {
            $("input[type=radio]").prop("checked", false);
            $(this).prop("checked", true);
        })
    })
})(jQuery); 