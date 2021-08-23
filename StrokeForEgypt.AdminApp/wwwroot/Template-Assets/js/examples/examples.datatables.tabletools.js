/*
Name: 			Tables / Advanced - Examples
Written by: 	Okler Themes - (http://www.okler.net)
Theme Version: 	2.2.0
*/

(function ($) {

    'use strict';

    var datatableInit = function (table_Id) {
        var $table = $(table_Id);

        var table = $table.dataTable({
            sDom: '<"text-right mb-md"T><"row"<"col-lg-6"l><"col-lg-6"f>><"table-responsive"t>p',
            buttons: ['csv', 'excel', 'pdf', 'print'],
            "order": [[0, "desc"]]
        });

        $('<div />').addClass('dt-buttons mb-2 pb-1 text-right').prependTo(table_Id + '_wrapper');

        $table.DataTable().buttons().container().prependTo(table_Id + '_wrapper .dt-buttons');

        $(table_Id + '_wrapper').find('.btn-secondary').removeClass('btn-secondary').addClass('btn-default');
    };

    $(function () {
        datatableInit('#datatable-tabletools');
        datatableInit('#datatable-tabletools-');
        datatableInit('#datatable-tabletools-0');
        datatableInit('#datatable-tabletools-1');
        datatableInit('#datatable-tabletools-2');
        datatableInit('#datatable-tabletools-3');
        datatableInit('#datatable-tabletools-4');
    });

}).apply(this, [jQuery]);
