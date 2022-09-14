$(document).ready(function () {

    //on click function for date input
    async function dateOnClick() {
        let enableDays = await fetch('../../api/sitting/dates')
            .then(response => response.json())
            .then(data => data.map(d => d.slice(0, 10)))
            .then($('#datepicker').datepicker(
                {
                    dateFormat: 'yy-mm-dd',
                    beforeShowDay: d => {
                        var date = $.datepicker.formatDate('yy-mm-dd', d);
                        if ($.inArray(date, enableDays) != -1) return [true];
                        return [false];
                    }
                }));
    }
    async function timeOnClick() {
        await fetch(`../../api/sitting/times?date=${$('#datepicker').val()}`)
            .then(response => response.json())
            .then(data => {
                $("#timepicker").timepicker('option', 'minTime', data.minTime);
                $("#timepicker").timepicker('option', 'maxTime', data.maxTime);
                $("#timepicker").timepicker('option', 'disableTimeRanges', data.disableTimeRanges);
                $("#timepicker").timepicker('show');
            });
    }
    //adds click event listener and calls onClick to initalise datepicker
    $('#datepicker').on('click', dateOnClick);
    $('#timepicker').on('click', timeOnClick);
    $("#timepicker").timepicker({
        showOn: null,
        step: 15,
        listWidth: 1,
        disableTextInput: true
    });
    dateOnClick();
})