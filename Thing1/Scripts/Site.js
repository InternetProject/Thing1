$(document).ready(function(){

    $(".expandable-menu dt a").click(function(){
        $(this).parent().parent().children("dd").toggleClass("hidden");
        $(this).toggleClass("is-collapsed");
        if ($(this).hasClass("is-collapsed"))
            $(this).text("[+]");
        else
            $(this).text("[-]");
        return false;
    });

    $(".expandable-menu dt a").click();
});