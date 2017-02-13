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

    $('#calendar').fullCalendar({
        timezone: 'local',
        events: '/Events/CalendarData',
        eventRender: function (event, element, view) {
            for (var i in event.clubIds) {
                if ($("#show-club-" + event.clubIds[i]).is(":checked"))
                    return true;
            }
            return false;
        },
        error: function () { console.log("Error parsing events") }
    });

    $(".show-club-checkbox").on('change', function () {
        $('#calendar').fullCalendar("rerenderEvents");
    });

    $("#select-all-clubs-calendar").click(function () {
        $(".show-club-checkbox").prop("checked", true);
        $('#calendar').fullCalendar("rerenderEvents");
        return false;
    });
});

