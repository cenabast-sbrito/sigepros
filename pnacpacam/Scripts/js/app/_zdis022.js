$(document).on('ready', function () {

    //_getListProveedores('_rutProveedorCarga', '_nombreProveedorCarga');
    let json = {
        organismo: $('#organismo').val()
    }

    $('#form-consolidar').on('submit', function (event) {
        event.preventDefault();

        const $btn = $('#btnConsolidaCarga');

        // Evita enviar si ya está procesando
        if ($btn.data("processing") === true) return;

        try {
            setProcessingButton($btn, "Consolidando…");
//            showToast("Consolidando la información, por favor espere…", "info", 10000);

            $.ajax({
                url: apiUrl("Upload/consolidarUploadFile"),
                type: 'POST',
                contentType: 'application/json; charset=utf-8',
                // Si no necesitas enviar body, omite 'data'. Si necesitas, agrega JSON.stringify(...)
                success: function (response) {
                    if (response && response.status === true) {
                        showToast("La información ha sido consolidada exitosamente.", "success", 3000);
                    } else {
                        const msg = (response && response.message) ? response.message : "No fue posible consolidar los datos.";
                        showToast(msg, "error", 6000);
                    }
                },
                error: function (xhr) {
                    if (xhr && xhr.status === 404) {
                        $(".loader_Tablas").fadeOut("slow");
                        showToast("Tu sesión ha expirado. Redirigiendo…", "error", 5000);
                        setTimeout(() => window.location.href = apiUrl(""), 1500);
                    } else {
                        showToast("Ocurrió un problema al consolidar los datos.", "error", 6000);
                    }
                },
                complete: function () {
                    unsetProcessingButton($btn);
                }
            });

        } catch (error) {
            unsetProcessingButton($btn);
            showToast("Se produjo un error inesperado durante la consolidación.", "error", 6000);
            // console.error(error);
        }
    });

});
function accionMensaje(idBtnAccion, disable, idDivMensaje, mensaje) {
    const btn = document.getElementById(idBtnAccion); btn.disabled = disable;
    if (disable) $("." + idDivMensaje).fadeOut("slow");
    else $("." + idDivMensaje).fadeIn("slow");
    $("." + idDivMensaje).text(mensaje);

}
$(document).on('change', '#_nombreProveedorCarga', function () {
    document.getElementById('_rutProveedorCarga').value = this.value

    indiceSeleccionado = $("#_nombreProveedorCarga option[value='" + this.value + "']").index();
    document.getElementById('_dspNombreProveedor').value = document.getElementById("_nombreProveedorCarga").options.item(indiceSeleccionado).text

});
$('#_rutProveedorCarga').on('change', function () {

    try {
        destino = '_nombreProveedorCarga';
        indiceSeleccionado = $("#_nombreProveedorCarga option[value='" + this.value + "']").index();

        if (indiceSeleccionado > 0) {

            document.getElementById("_nombreProveedorCarga").options.item(indiceSeleccionado).selected = 'selected';
            document.getElementById('_dspNombreProveedor').value = document.getElementById("_nombreProveedorCarga").options.item(indiceSeleccionado).text
            $("#" + destino).change();

        } else {
            avisoFinProceso('Proveedor NO Hallado');
            document.getElementById("_nombreProveedorCarga").options.item(0).selected = 'selected';
            document.getElementById('_dspNombreProveedor').value = "";
            $("#" + destino).change();

        }
    } catch (error) {
        console.error(error);
    }

});




//sin uso
function getTableZDis022() {//incluir el js 

    //    var groupColumn = 0;
    try {
        var json = {
            topx: "TOP 500"
        }
        $('#tablaZDis022').DataTable({
            ajax: {
                url: apiUrl("Upload/getZDis022"),
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
                { data: "Documento_de_venta" },
                { data: "Pedido_de_Compra" },
                { data: "Grupo_de_articulos" },
                { data: "Canal" },
                { data: "Código_Material" },
                { data: "Fecha_de_entrega_al" },
                { data: "Fecha_entrega_al_OL" },
                { data: "Cantidad_documento_d" },
                { data: "Cantidad_informada" },
                { data: "Lote" },
                { data: "Fecha_entrega_al_ol_" },
                { data: "OS" },
                { data: "NFactura" },
                { data: "Guía_de_despacho" },
                { data: "Observaciones" },
                { data: "Suspensión_de_entreg" },
                { data: "Autoriza_suspensión" },
                { data: "Fecha_de_carga_de_in" },
                { data: "Salida_mercancía" },
                { data: "Documento_de_entrega" },
                { data: "Status_carga_cedible" },
                { data: "Fecha_Sal_mercancía" },
                { data: "Fact_comisiones_pro" },
                { data: "Fecha_fact_comision" },
                { data: "Fecha_recepción_conf" },
                { data: "Fecha_entrega_cedibl" },
                { data: "Centro" },
                { data: "Valor_neto" },
                { data: "Bultos" },
                { data: "Volumen" },
                { data: "Peso" },
                { data: "Fecha_vencimiento" },
                { data: "GTIN" },
                { data: "Agrupador" },
                { data: "Motivo_Rechazo" },
                { data: "OC" },
                { data: "DocVenta" }
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
            },
            destroy: true,
            scrollX: true,
            "autoWidth": false,
            paging: true,
        });


    } catch (ex) {
        console.log("Error: " + ex.message);
    }
}

function reinicializarDetalleInventario() {

    var table = $('#tablaInventarioDetalleById').DataTable({
        paging: true,
        destroy: true,
        responsive: true
    });
    table.clear().destroy();
}

function limpiarResumen() {

    $("#resumeCountFacturas").text("");

    $("#resume_count_filasSinFactura").text("");
    $("#resume_count_filasOtroProveedor").text("");
    $("#resume_count_filasFueraDelPeriodo").text("");
    $("#resume_count_filasOtroCanal").text("");

    $("#resumeCountRechazos").text("");

    $("#resumeCountFacturasAceptadas").text("");

}


$(document).on('click', '#btnPeriodoCarga', function () {

    var anho = $('#btnPeriodoCarga').val().substring(0, 4);
    var mes = $('#btnPeriodoCarga').val().substring(5, 7);

});

/*aqui es donde se ejecuta la carga de los datos */
$('#form-uploadPlanilla').on('submit', function (event) {
    try {

        event.preventDefault();

        btnPressed = 'btnFileUploadPlanilla';
        //console.log('form-ipload');

        btnOFF(btnPressed, 'Cargando Planilla... ');

        //$("#resumeCountFacturas").text("");
        //$("#resumeCountFacturasNoCero").text("");
        //$("#resumeCountFacturasProveedor").text("");
        //$("#resumeCountFacturasPeriodo").text("");
        //$("#resumeCountRechazos").text("");
        //$("#resumeCountFacturasAceptadas").text("");

        let fileInput = $('#file-upload')[0];
        let progressBar = $('#progress-bar');
        let antiForgeryToken = $('input[name=__RequestVerificationToken]').val();
        progressBar.css('width', '0%');
        let formData = new FormData();
        formData.append('archivoExcel', fileInput.files[0]);
        formData.append('__RequestVerificationToken', antiForgeryToken);
        formData.append('fechaCarga', $('#btnPeriodoCarga').val());
        formData.append('rutProveedorCarga', $('#_rutProveedorCarga').val());
        formData.append('programaDefinido', $('#programaDefinido').val());
        formData.append('chkFiltroPeriodo', document.getElementById('chkFiltroPeriodo').checked);

        $("#nombreFile").text('Documento :' + fileInput.files[0].name);
        $(".file-list-item").removeClass("d-none");

        $.ajax({
            url: apiUrl("Upload/UploadFile"),
            type: 'POST',
            data: formData,
            processData: false,
            contentType: false,
            xhr: function () {
                let xhr = new XMLHttpRequest();
                xhr.upload.addEventListener('progress', function (event) {
                    if (event.lengthComputable) {
                        //accionMensaje(btnPressed, false, "accion_cargar_mensaje", "Cargando planilla en tabla temporal, por favor espere.");
                        showToast("leyendo planilla ...", "info", 1000);
                        let percentComplete = (event.loaded / event.total) * 100;
                        progressBar.css('width', percentComplete + '%');
                        progressBar.attr('aria-valuenow', percentComplete);
                        progressBar.parent().show();
                    }
                }, false);
                return xhr;
            },
            xhrFields: {
                withCredentials: true
            },

            success: function (response) {
                btnON(btnPressed, 'Cargar Planilla');

                // Oculta todo hasta saber si fue éxito o error
                $("#resumenCarga").addClass("d-none");

                if (response.status) {

                    // Mensaje elegante
                    //accionMensaje(btnPressed, false, "accion_cargar_mensaje", "Carga completada correctamente.");
                    showToast("Documento ha sido procesado.", "success", 3000);
                    
                    //progressBar
                    //    .addClass('bg-gob-exito')
                    //    .removeClass('bg-gob-info bg-gob-error');

                    // Mostrar resumen
                    $("#resumeCountFacturas").text(response.count);

                    $("#resume_count_filasSinFactura").text(response.count_filasSinFactura);
                    $("#resume_count_filasOtroProveedor").text(response.count_filasOtroProveedor);
                    $("#resume_count_filasFueraDelPeriodo").text(response.count_filasFueraDelPeriodo);
                    $("#resume_count_filasOtroCanal").text(response.count_filasOtroCanal);
                    
                    //$("#resumeCountFacturasPeriodo").text(response.countPeriodo);

                    $("#resumeCountRechazos").text(response.count_Rechazos);
                    $("#resumeCountFacturasAceptadas").text(response.count_FacturasAceptadas);

                    ///$("#resumenCarga").removeClass("d-none");

                    // Refresca tabla
                    getTableZDis022();

                } else {

                    showToast("La carga no pudo completarse. Revise el archivo e intente nuevamente.", "error", 4000);
                    //accionMensaje(
                    //    btnPressed,
                    //    false,
                    //    "accion_cargar_mensaje",
                    //    "La carga no pudo completarse. Revise el archivo e intente nuevamente."
                    //);

                    //progressBar
                    //    .addClass('bg-gob-error')
                    //    .removeClass('bg-gob-info bg-gob-exito');

                    limpiarResumen();
                }
            },

            //success: function (response) {
            //    console.log(response);
            //    if (response.status) {
            //        accionMensaje(btnPressed, false, "accion_cargar_mensaje", "");
            //        //$("#estadoUpload").text("Carga Completa Realizada");
            //        progressBar.addClass('bg-gob-exito').removeClass('bg-gob-info').removeClass('bg-gob-error');
            //        $("#resumeCountFacturas").text("Se han leído " + response.count + " Filas");
            //        $("#resumeCountFacturasNoCero").text("Se han aceptado " + response.countNoCero + " Filas (Factura informada)");
            //        $("#resumeCountFacturasProveedor").text("Se han aceptado " + response.countProv + " Filas (pedido de compra pertenece al proveedor de carga)");
            //        $("#resumeCountFacturasPeriodo").text("Se han aceptado " + response.countPeriodo + " Filas (pertenece al periodo de carga)");
            //        $("#resumeCountOtroCanal").text("Filas Rechazadas por no ser parte del programa " + $('#programaDefinido').val() + " " + response.countOtroCanal + " ");
            //        $("#resumeCountOtraFecha").text("Filas Rechazadas por fecha de entrega OL fuera del rango de carga " + response.countOtraFecha + " ");
            //        $("#resumeCountRechazos").text("Total Filas Rechazadas: " + response.countRechazos + " ");
            //        getTableZDis022();
            //    } else {
            //        accionMensaje(btnPressed, false, "accion_cargar_mensaje", "Carga de información fallida, ocurrió algún problema");
            //        //$("#estadoUpload").text("Error al subir el archivo");

            //        //$("#estadoUpload").text("Ocurrio algún problema");
            //        progressBar.addClass('bg-gob-error').removeClass('bg-gob-info').removeClass('bg-gob-exito');
            //        $("#resumeCountFacturas").text("0");
            //        $("#resumeCountFacturasNoCero").text("0");
            //        $("#resumeCountFacturasProveedor").text("0");
            //        $("#resumeCountFacturasPeriodo").text("0");
            //        $("#resumeCountOtroCanal").text("0");
            //        $("#resumeCountOtraFecha").text("0");
            //        $("#resumeCountRechazos").text("0");
            //    }
            //    btnON(btnPressed, 'Cargar Planilla');
            //},
            error: function (xhr, status, error) {
                btnON(btnPressed, 'Cargar Planilla');
                //accionMensaje(btnPressed, false, "accion_cargar_mensaje", "No se ha podido cargar el archivo");
                showToast("No fue posible procesar la planilla. Verifique el archivo e intente nuevamente.", "error", 4000);
                //accionMensaje(
                //    btnPressed,
                //    false,
                //    "accion_cargar_mensaje",
                //    "No fue posible procesar la planilla. Verifique el archivo e intente nuevamente."
                //);

                console.error(xhr.responseText);
                //$("#estadoUpload").text("Error al subir el archivo");
                progressBar.addClass('bg-gob-error').removeClass('bg-gob-info').removeClass('bg-gob-exito');
            }
        });

    } catch (error) {
        btnON(btnPressed, 'Cargar Planilla');
        $("#estadoUpload").text("Ha ocurrido algún error");
    }

});

