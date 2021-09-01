/*
Name: 			Tables / Advanced - Examples
Written by: 	Okler Themes - (http://www.okler.net)
Theme Version: 	2.2.0
*/

(function ($) {

    'use strict';

    var datatableInit = function () {
        $.extend($.fn.dataTable.defaults, {
            // Design Assets
            dom: 'Bfrtip',
            buttons: ['pageLength', 'excel', 'print'],
            lengthMenu: [
                [10, 25, 50, 100, 1000],
                ['10', '25', '50', '100', '1000']
            ],
            stateSave: true,
            autoWidth: true,
            // ServerSide Setups
            processing: true,
            serverSide: true,
            // Paging Setups
            paging: true,
            "scrollX": true,
            // Searching Setups
            searching: { regex: true },
            // Column Definitions
            columnDefs: [
                { targets: "no-sort", orderable: true },
                { targets: "no-search", searchable: false },
                {
                    targets: "trim",
                    render: function (data, type, full, meta) {
                        if (type === "display") {
                            data = strtrunc(data, 10);
                        }
                        return data;
                    }
                },
                { targets: "date-type", type: "date-euro" }
            ]
        });
    };

    function strtrunc(str, num) {
        if (str.length > num) {
            return str.slice(0, num) + "...";
        }
        else {
            return str;
        }
    }

    $(function () {
        datatableInit();
    });

}).apply(this, [jQuery]);
