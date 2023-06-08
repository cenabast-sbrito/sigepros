/* ADMINISTRACION DEL MENU */

//$(document).ready(function () {
//    createPdf();
//}
//);

$(document).on('click', '#href_documentos', function () {

    getProveedores();

});

$(document).on('change', '#nombreProveedor', function () {

    if ($(this).val() != "") {
        document.getElementById('rutProveedor').value = $(this).val();
        resetTablaDespachos();
        getDespachos($(this).val());
    }

});

function resetTablaDespachos() {

    var table = $('#tablaDespachos').DataTable({
        paging: true,
        destroy: true,
        responsive: true
    });
    table.clear().destroy();

}

function getProveedores() {
    try {
        $.ajax({
            type: "GET",
            url: "../Proveedor/getProveedores",
            success: function (res) {
                let option = "<option value=''>Seleccione un Proveedor</option>";
                for (var element of res) {
                    option = option +
                        '<option value="' + element.rut + '">' +
                        element.nombre +
                        '</option>';
                }
                $('#nombreProveedor').html(option);
            },
            error: function (jqXHR, textStatus, errorThrown) {
                console.error("Ha ocurrido un error : " + jqXHR.responseText);
            }
        });
    } catch (ex) {
        console.error("Ha ocurrido un error : " + ex.name + " " + ex.message);
    }
}

function getDespachos(rut) {
    var json = {
        rutProveedor : rut
    }
    try {
        $('#tablaDespachos').DataTable({
            ajax: {
                url: "../Proveedor/getDespachos",
                data: json,
                dataType: "json",
                "dataSrc": function (json) {
                    if (json.length > 0) {
                        return json;
                    } else {
                        return [];
                    }
                }
            },
            columns: [
                { "data": "LIFNR" },
                { "data": "NFactura" },
                { "data": "DocumentoVenta" },
                { "data": "PedidoCompra" },
                { "data": "GrupoArticulos" },
                { "data": "Canal" },
                { "data": "GuiaDespacho" },
                { "data": "CodigoMaterial" },
                { "data": "Lote" },
                { "data": "CantidadDocumento" },
                { "data": "CantidadInformada" },
                { "data": "ValorNeto" },
                { "data": "Observaciones", visible: false },
                { "data": "DocumentoEntrega", visible: false },
                { "data": "FechaVencimiento" },
                { "data": "MotivoRechazo" },
                {
                    "render": function (data, type, full, meta) {

                        var url = "https://testaplicacionesweb.cenabast.cl:7001/archivoscedibles/" + parseInt(full.LIFNR) + "/procesados/" + full.DocumentoVenta + ".pdf"
                        //var ob = "<object id='pdfHolder' data='"+url+"' type='application/pdf' width='300' height='200'></object>"

                        var cuadroPreview = "<a target='_documentoPreview' href='" + url + "'><i class='fas fa-eye' style='font-size:28px'></i></a>"

                        strBotones = cuadroPreview;

                        return strBotones;

                    }
                }
            ],
            language: {
                "lengthMenu": "Mostrar _MENU_ registros por página",
                "zeroRecords": "No hay registros disponibles",
                "info": "Mostrando registro _START_ de _END_",
                "infoEmpty": "No hay registros habilitados",
                "infoFiltered": "(filtrado de _MAX_ registros totales)",
                "paginate": {
                    "next": "Siguiente",
                    "previous": "Anterior"
                },
                "search": "Buscar",
                "searchPlaceholder": "Buscar ",
                "select": true,
            },
            responsive: true,
            destroy: true,
            scrollX: true
        });
    } catch (ex) {
        console.log("Error: " + ex.message);
    }
}

































