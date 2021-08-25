
function getEventPackagePrice(Fk_EventPackage) {
    var serviceUrl = '/DataFilter/GetEventPackagePrice/?Fk_EventPackage=' + Fk_EventPackage;

    $.ajax({
        type: 'GET',
        url: serviceUrl,
        success: function (result) {

            $("#TotalPrice").val(' ');
            $("#TotalPrice").val(result);
            $("#TotalPrice").append(result);
        }
    });

}


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
            if (result.length > 0) {
                getEventPackagePrice(result[0].id);
            }
        }
    });
}

