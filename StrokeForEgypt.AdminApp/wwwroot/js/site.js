function getEventPackages(Fk_Event) {
    var serviceUrl = '/DataFilter/GetEventPackages/?Fk_Event=' + Fk_Event;

    $.ajax({
        type: 'GET',
        url: serviceUrl,
        success: function (result) {
            $("#Fk_EventPackage").empty();

            var options = ' ';
            if (result.length > 0) {
                for (var i = 0; i < result.length; i++) {
                    options += '<option value="' + result[i].id + '">' + result[i].name + '</option>'
                }
            }
            $("#Fk_EventPackage").append(options);
        }
    });
}
