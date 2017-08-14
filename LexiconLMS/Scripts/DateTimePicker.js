$(document).ready(function () {
    $('.date-picker').datetimepicker({
        //dateFormat: "dd/MM/yy",
        dateFormat: 'yy-mm-dd',
        changeMonth: true,
        changeYear: true,
        minDate: new Date(),
        yearRange: "+0:+1",
        controlType: 'select',
        timeFormat: 'HH:mm'
    });
});