/*
$(document).on('click', '#href_facturacion', function () {

    _getFacturas();

});
*/

$(document).on('click', '#href_usuarios', function () {

    getTableroUsuario();

}); 


$(document).on('click', '#href_proveedores', function () {

    getTableroProveedores();

}); 


function reset(tabla) {

    var table = $('#' + table).DataTable({
        paging: true,
        destroy: true,
        responsive: true
    });
    table.clear().destroy();

} 



function descarga(rut, ndoc) {
    try {
        $.ajax({
            type: "GET",
            data: {
                rutProveedor: rut,
                factura: ndoc
            },
            url: apiUrl("Proveedor/descarga"),
            success: function (res) {

                $('#mensajeSistema').text('Expediente Construido');
                $('#modalMensajeSistema').modal('show');

            },
            error: function (xhr) {
                if (xhr.status === 404) { // 404: No se encuentra el recurso
                    $(".loader_Tablas").fadeOut("slow");
                    alert('Tu sesión ha expirado.');
                    window.location.href = apiUrl(""); // obtenerRuta("");
                } else {
                    avisoFinProceso("Ocurrio algún error");
                }
            }
        });
    } catch (ex) {
        console.error("Ha ocurrido un error : " + ex.name + " " + ex.message);
    }
}




/* NOTIFICACIONES */

function getNotificaciones() {

    var table = $('#tablaNotificaciones').DataTable();
    rut = sessionStorage.getItem("_pnacpacam_Rut");

    reset('tablaNotificaciones');
/*    var groupColumn = 1;
    var nGroupColumn = 6;
    */
    var nrows = 5;
    var json = {
        rutProveedor: rut
    }
    try {
        $('#tablaNotificaciones').DataTable({
            ajax: {
                url: apiUrl("Proveedor/getNotificaciones"),
                data: json,
                dataType: "json",
                "dataSrc": function (json) {
                    return json.state ? json.resultado : [];
                },
                error: function (xhr) {//return json.state ? json.resultado : [];
                    if (xhr.status === 404) { // 404: No se encuentra el recurso
                        $(".loader_Tablas").fadeOut("slow");
                        alert('Tu sesión ha expirado.');
                        window.location.href = apiUrl("");// obtenerRuta("");
                    } else {
                        avisoFinProceso("Ocurrio algún error");
                    }
                }
            },
            columns: [
                {
                    "render": function (data, type, full, meta) {

                        if (full.BLeido)
                            return "<i class='far fa-square' style='font-size:22px'></i>";
                        else return "<i class='fas fa-check-square' style='font-size:22px'></i>";

                    }, width: "1"
                }, 
                { "data": "RutUsuario", width: "5", visible: false },
                { "data": "RutProveedor", width: "5"  },
                { "data": "NFactura", width: "5" },
                { "data": "FNotificacion", className: "notificacionFecha" },
                { "data": "TMensaje", className: "notificacionMensaje"  }
            ],
            displayLength: nrows, /* numero de filas que despliega inicialmente la tabla */
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
            scrollX: true,
            success: function (res) {
                console.log(res);
            },
            error: function (jqXHR, textStatus, errorThrown) {
                console.error("Ha ocurrido un error : " + jqXHR.responseText);
            }

        });
        /*
        $('#tablaFacturas tbody').on('click', 'tr', function () {
            alert('Row index: ' + table.row(this).index());
        });
        */
    } catch (ex) {
        console.log("Error: " + ex.message);
    }
}

/* FIN NOTIFICACIONES */






/* ADMINISTRA NOTIFICACIONES */

function initNotificaciones() {
    rut = sessionStorage.getItem("_pnacpacam_Rut");
    getNotificaciones(rut);
}




/* FIN ADMINISTRA NOTIFICACIONES */


/* MENSAJES AL USUARIO */

function mensajeSistema(aviso) {

    $('#mensajeSistema').text(aviso);
    $('#modalMensajeSistema').modal('show');

}

/* MENSAJES AL USUARIO */
