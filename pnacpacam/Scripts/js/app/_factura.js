let table = null;
function _ocultarImagen() {
    var x = document.getElementById("_imagenPNacPacam");
    x.style.display = "none";
}
// Cuando la pestaña "Facturación" se muestre:
document.addEventListener("DOMContentLoaded", () => {
    $('#href_facturacion').on('shown.bs.tab', function () {
        try {
            _ocultarImagen();
            ///_getFacturas();
            _getListProveedores('_rutProveedor', '_nombreProveedor');
            _getEstados('_tEstados');
        } catch (err) {
            console.error("Error al inicializar facturas:", err);
        }
    });
});
// Cuando la pestaña "Facturación" se muestre:
document.addEventListener("DOMContentLoaded", () => {
    $('#href_facturacion_minsal').on('shown.bs.tab', function () {
        try {
            _ocultarImagen();
            ///_getFacturas();
            _getListProveedores('_rutProveedor', '_nombreProveedor');
            _getEstados('_tEstados');
        } catch (err) {
            console.error("Error al inicializar facturas:", err);
        }
    });
});
$(document).on('click', '#_generar-Expediente-Factura', async function () {

    toggleBtn('_generar-Expediente-Factura', 'Crear Expediente', 'Creando...');

    try {

        const rut = $('#_Expediente_rutProveedor').val();
        const factura = $('#_Expediente_nFactura').val();
        await compruebaTablaSAP_ASYNC(rut, factura, 'getExpediente');

    } catch (ex) {
        console.error('Ocurrió algún error:', ex);
    } finally {
        toggleBtn('_generar-Expediente-Factura', 'Crear Expediente', 'Creando...');
    }

});
function _getFacturas() {

    rol = sessionStorage.getItem("_pnacpacam_Rol");
    const rutProveedor = +document.getElementById('_rutProveedor').value;
    const nFactura = document.getElementById('_nfactura').value;
    const anio = document.getElementById('_anio').value;
    const nEstado = document.getElementById('_nEstado').value;

    reset('tablaFacturas');

    $(".loader_Tablas").fadeIn("slow");
    const json = {
        rutProveedor,
        nFactura,
        anio,
        nEstado
    }
    try {
        table = new DataTable('#tablaFacturas', {
            layout: {
                topStart: {
                    buttons: [
                        {
                            extend: 'copy', text: '<i class="fas fa-copy"></i> Copiar',
                            className: 'btn-copy'
                        },
                        {
                            extend: 'csv', text: '<i class="fas fa-file-csv"></i> CSV',
                            className: 'btn-csv'
                        },
                        {
                            extend: 'excel', text: '<i class="fas fa-file-excel"></i> Excel',
                            className: 'btn-excel'
                        }
                    ]
                }
            },
            features: {
                lengthChange: false,   // Oculta el selector de filas por página
                searching: false       // Oculta el campo de búsqueda
            },
            pageLength: 5,
            destroy: true,
            ajax: {
                url: apiUrl("Proveedor/getFacturas"),
                data: json,
                dataType: "json",
                dataSrc: function (json) {

                    if (!json || json.state !== true) {
                        return [];
                    }

                    if (!Array.isArray(json.resultado)) {
                        return [];
                    }

                    return json.resultado;
                },
                error: function (resp) {

                    if (resp.status === 401 || resp.status === 440 || resp.status === 419 || resp.status === 404) {
                        avisoFinProceso("El recurso solicitado no está disponible.");
                    }
                }
            },
            columns: [
                { "data": "LIFNR" },
                { "data": "NAME1" },
                { "data": "CodigoMaterial", },
                { "data": "Denominacion" },
                { "data": "NFactura" },
                {
                    "render": function (data, type, full, meta) {
                        return _administraEstados(full, meta.row);
                    }
                },
                { "data": "Expediente", "visible": false },
                {
                    render: function (data, type, full, meta) {

                        // ¿Tiene observación?
                        const tieneObs = full.TObservacion && full.TObservacion.trim() !== "";

                        // Ícono de observación (solo si tiene)
                        const iconObs = tieneObs
                            ? `<i class="fas fa-comment-dots action-icon icon-observacion"
                                   title="Ver observación"
                                   onclick="mostrarObservacion('${full.LIFNR}', '${full.NFactura}', \`${full.TObservacion.replace(/`/g, "\\`")}\`)">
                               </i>`
                            : "";

                        // Botón habilitado
                        var habilitado = `
                            <a class="button"
                               onclick="getExpediente(${parseInt(full.LIFNR)}, ${full.NFactura})"
                               id="getExpediente_${parseInt(full.LIFNR)}${full.NFactura}"
                               title="Construir expediente PDF">
                                <i class="fas fa-file-pdf action-icon icon-create"></i>
                            </a> 
                            ${iconObs}
                        `;

                        // Botón deshabilitado
                        var deshabilitado = `
                            <a class="button is-disabled"
                               aria-disabled="true"
                               tabindex="-1"
                               title="No disponible">
                                <i class="fas fa-file-pdf action-icon icon-disabled"></i>
                            </a>  
                            ${iconObs}
                        `;

                        /////////// COMPROBAR ///////////

                        //console.log({
                        //    factura: full.NFactura,
                        //    estado: full.CEstado,
                        //    existeExpediente: full.ExisteExpediente,
                        //    link: full.linkExpedientes, full
                        //});

                        /////////// FIN COMPROBAR ///////////


                        if (full.CEstado === "1") {
                            return habilitado;
                        } else {
                            return deshabilitado;
                        }
                    },
                    visible: ["ADM", "ADP"].includes(String(rol).toUpperCase())


                },
                {
                    render: function (data, type, full) {


                        const colorIcono = full.ExisteExpediente
                            ? '#1976d2'   // azul normal (activo)
                            : '#90caf9';  // azul más débil (pendiente)

                        const titulo = full.ExisteExpediente
                            ? 'Ver expediente'
                            : 'Expediente aún no disponible';

                        return `
                                <a target="_Factura_ExpedientePreviewIFrame"
                                   href="${full.linkExpedientes}"
                                   onclick="verExpediente(${parseInt(full.LIFNR)}, ${full.NFactura});"
                                   title="${titulo}">
                                    <i class="fas fa-folder-open action-icon"
                                       style="
                                           color:${colorIcono};
                                           transition: color .2s ease;
                                       "></i>
                                </a>
                            `;

                    }
                }
                ,
                {
                    "render": function (data, type, full, meta) {
                        return `<a href='#' class='button' onclick='verGuias(${parseInt(full.LIFNR)},${full.NFactura});'
                        id='getGuias_${parseInt(full.LIFNR)}${full.NFactura}'>
                        <i class="fas fa-truck action-icon icon-guides" title="Ver guías"></i>
                        </a>`;
                    }
                },
                {
                    "render": function (data, type, full, meta) {
                        enlace = '';
                        if (!full.ExisteComision) {
                            return '';
                        }
                        return `<a target='_Factura_ComisionPreviewIFrame' href='${full.linkComision}'
                        class='button' onclick='verFacturaComision(${parseInt(full.LIFNR)},${full.NFactura});'>
                        <i class="fas fa-paperclip action-icon icon-file" title="Documentos adjuntos"></i>
                        </a>
                        `+ enlace;
                    }
                },
                {
                    "render": function (data, type, full, meta) {
                        let texto = full.NAME1;
                        let url = full.Url;
                        texto = texto.replace(/["']/g, "");

                        if (!full.ExisteDocumentoAdjunto) {
                            return '';
                        }

                        return `<a onclick='fn_getDatos_Documentos(
                            "DA",
                            "",
                            "modal_DocumentosAdjuntos_2",
                            "_documentoAdjuntoPreview",
                            "_tablaDocumentosAdjuntosView-1",
                            ${full.NFactura},
                            ${parseInt(full.LIFNR)},
                            "${texto}",
                            true
                             );'>
                                <i class='fas fa-paperclip' style='font-size:28px'></i>
                                </a>`;
                    }
                }, { "data": "ano" }
            ],
            language: {
                "lengthMenu": "Mostrar _MENU_ registros por página",
                "zeroRecords": "No hay registros disponibles",
                "info": "Mostrando registro _START_ de _END_",
                "infoEmpty": "No hay registros habilitados",
                "infoFiltered": "(filtrado de _MAX_ registros totales)",
                paginate: {
                    previous: '<i class="fas fa-arrow-left"></i>',
                    next: '<i class="fas fa-arrow-right"></i>'
                },
                "search": "Buscar",
                "searchPlaceholder": "Buscar ",
                "select": false
            },
            initComplete: function (settings, json) {
                $(".loader_Tablas").fadeOut("slow");
            },
            error: function (jqXHR, textStatus, errorThrown) {
                console.error("Ha ocurrido un error : " + jqXHR.responseText);
            }

        });


    } catch (ex) {
        console.log("Error: " + ex.message);
    }
}
function _getListFacturas(campo, listaDestino) {

    var json = {
        rutproveedor: campo
    }

    try {
        $.ajax({
            type: "GET",
            url: apiUrl("Proveedor/getListFacturas"),
            data: json,
            success: function (res) {
                let option = "<option value=''>Seleccione una Factura</option>";
                for (var element of res) {
                    option = option +
                        '<option value="' + element.factura + '">' +
                        element.factura +
                        '</option>';
                }
                $('#' + campo).val('');
                $('#' + listaDestino).html(option);
            },
            error: function (xhr) {
                if (xhr.status === 404) { // 404: No se encuentra el recurso
                    $(".loader_Tablas").fadeOut("slow");
                    alert('Tu sesión ha expirado.');
                    window.location.href = apiUrl("");
                } else {
                    avisoFinProceso("Ocurrio algún error");
                }
            }
        });
    } catch (ex) {
        console.error("Ha ocurrido un error : " + ex.name + " " + ex.message);
    }
}
$(document).on('change', '#_nombreProveedor', function () {
    document.getElementById('_rutProveedor').value = this.value
});
$(document).on('change', '#_tEstados', function () {
    document.getElementById('_nEstado').value = this.value
});

$(document).on('click', '#_acepta-cambio-estado', function () {
    rut = $('#modal_rut').val();
    factura = $('#modal_factura').val();
    estadoActual = $('#modal_estadoActual').val();
    estadoNuevo = $('#modal_estadoNuevo').val();
    fila = $('#modal_fila').val();
    obj = $('#modal_obj').val();
    obs = $('#_CampoObservacion').val();
    putEstadoConfirmado(obj, rut, factura, estadoActual, estadoNuevo, fila, obs);
    document.activeElement?.blur();
    $('#_Acepta_Observacion').modal('hide');

});
$('#_rutProveedor').on('change', function () {

    let rutProveedor = +document.getElementById('_rutProveedor').value;
    if (!isNaN(rutProveedor)) {
        try {

            destino = '_nombreProveedor';
            indiceSeleccionado = $("#_nombreProveedor option[value='" + this.value + "']").index();

            if (indiceSeleccionado > 0) {

                document.getElementById("_nombreProveedor").options.item(indiceSeleccionado).selected = 'selected';
                document.getElementById('_dspNombreProveedor').value = document.getElementById("_nombreProveedor").options.item(indiceSeleccionado).text
                $("#" + destino).change();

            } else {

                avisoFinProceso('Proveedor NO Hallado');
                document.getElementById("_nombreProveedor").options.item(0).selected = 'selected';
                document.getElementById('_dspNombreProveedor').value = "";
                $("#" + destino).change();

            }

        } catch (error) {
            console.error(error);
        }
    } else {
        $('#modalMensajeSistema').modal('show');
        $('#mensajeSistema').text('Rut de Proveedor no es válido');

    }

});

function actualizarFilaFactura(RutProveedor, Factura, estadoNuevo, botonera) {


    var habilitado = `
                                <a class="button"
                                   onclick="getExpediente(${parseInt(RutProveedor)}, ${Factura})"
                                   id="getExpediente_${parseInt(RutProveedor)}${Factura}"
                                   title="Construir expediente PDF">
                                    <i class="fas fa-file-pdf action-icon icon-create"></i>
                                </a>`;
    // En estado "no disponible": sin onclick, sin navegación, aspecto deshabilitado
    var deshabilitado = `
                                <a class="button is-disabled"
                                   aria-disabled="true"
                                   tabindex="-1"
                                   title="No disponible">
                                    <i class="fas fa-file-pdf action-icon icon-disabled"></i>
                                </a>`;

    //console.log('actualizarFilaFactura-> botonera ',botonera);
    tabla = $('#tablaFacturas').DataTable();
    tabla.rows().every(function () {
        var data = this.data();

        if (String(data[4]) === String(Factura)) {
            data[5] = botonera.botonera;
            // console.log(String(botonera.botonera));
            if (estadoNuevo === "1") { data[6] = habilitado } else { data[6] = deshabilitado }

            this.data(data);
        }

    });
    tabla.draw(false);

}

function _administraEstados(obj, fila) {
    ///console.log(obj);
    var PuedeEjecutarNOOK = "";
    var boton_NOOK = "";

    var estilo = "color: burlywood;font-size: small;";

    var nivel = 0;
    var botonera = "";
    var prefijoBoton = " <button type='button' class='btn btn-sm' onclick='putEstado(this,&apos;" + obj.TEstadoOK + "&apos;,&apos;" + obj.TEstadoNOOK + "&apos;," + parseInt(obj.LIFNR) + "," + obj.NFactura + "," + obj.CEstado + "";

    var prefijoBoton_Observaciones = " <button type='button' class='btn btn-sm' onclick='putEstado(this,&apos;" + obj.TEstadoOK_Observaciones + "&apos;,&apos;" + obj.TEstadoNOOK + "&apos;," + parseInt(obj.LIFNR) + "," + obj.NFactura + "," + obj.CEstadoOK_Observaciones + "";

    var codigo = "" + parseInt(obj.LIFNR) + "-" + obj.NFactura + "";
    var finBoton = "</button>";

    var textoEstadoActual = " <div style='" + estilo + "'> " + obj.TEstadoTiempoPresente + "</div>";  // Estado Actual, por lo que no requiere uso de botón

    var iconoOK = " <i class='fa fa-user' style='color:cadetblue'></i> " + obj.TEstadoOK;
    var iconoOK_Observaciones = " <i class='fa fa-user' style='color:cadetblue'></i> " + obj.TEstadoOK_Observaciones;
    var iconoNOOK = " <i class='fa fa-times-circle' style='color:goldenrod'></i> " + obj.TEstadoNOOK;

    rol = sessionStorage.getItem("_pnacpacam_Rol");

    puedeEjecutarOK = (obj.PuedeEjecutarOK == null) ? "False" : obj.PuedeEjecutarOK

    //    console.log(puedeEjecutarOK, puedeEjecutarOK == "True");
    if (Number(puedeEjecutarOK) === 1) {
        boton_OK = prefijoBoton + "," + obj.Estado_OK + "," + fila + ",1);' >" + iconoOK + finBoton;
    } else {
        boton_OK = prefijoBoton + "," + obj.Estado_OK + "," + fila + ",1);' disabled >" + iconoOK + finBoton;
    }

    PuedeEjecutarNOOK = obj.PuedeEjecutarNOOK;
    if (Number(PuedeEjecutarNOOK) === 1) {
        boton_NOOK = prefijoBoton + "," + obj.Estado_NOOK + "," + fila + ",0);'  >" + iconoNOOK + finBoton;
    } else {
        boton_NOOK = prefijoBoton + "," + obj.Estado_NOOK + "," + fila + ",0);' disabled >" + iconoNOOK + finBoton;
    }

    boton_OK_Observaciones = prefijoBoton_Observaciones + "," + obj.CEstadoOK_Observaciones + "," + fila + ",1);' >" + iconoOK_Observaciones + finBoton;

    if (obj.Estado_OK == 0) boton_OK = "";
    if (obj.Estado_NOOK == 0) boton_NOOK = "";

    botonera = textoEstadoActual;


    ////    console.log('CESTADO ', obj.CEstado);
    ////    console.log('ROL ', rol);
    if ((obj.CEstado == 5) && (rol == "MIN" || rol == "MIP")) botonera = botonera + boton_OK + boton_OK_Observaciones + boton_NOOK; // estado actual en 5 es porque esta en revision ejecutivo MINSAL, y el ejecutivo minsal puede aceptar sin observaciones o aceptar con observaciones
    else botonera = botonera + boton_OK + boton_NOOK;
    ///    console.log('botonera ', botonera );
    if (obj.CEstado == 10) botonera = textoEstadoActual;

    return "<div id='" + codigo + "'>" + botonera + "</div>";// + "<p>(" + obj.CEstado +")</p>";

}

function _getEstados(listaDestino) {
    try {
        $.ajax({
            type: "GET",
            url: apiUrl("Proveedor/getEstados"),
            success: function (res) {
                let option = "<option value=''>Seleccione un Estado</option>";
                for (var element of res) {
                    option = option +
                        '<option value="' + element.CEstado + '">' +
                        element.TEstadoTiempoPresente +
                        '</option>';
                }
                $('#' + listaDestino).html(option);
            },
            error: function (xhr) {
                if (xhr.status === 404) { // 404: No se encuentra el recurso
                    $(".loader_Tablas").fadeOut("slow");
                    alert('Tu sesión ha expirado.');
                    window.location.href = apiUrl("");
                } else {
                    avisoFinProceso("Ocurrio algún error");
                }
            }
        });
    } catch (ex) {
        console.error("Ha ocurrido un error : " + ex.name + " " + ex.message);
    }
}
function putEstado(obj, TEstadoOK, TEstadoNOOK, rut, factura, estadoActual, estadoNuevo, fila, bEstadoSeleccionado) {

    $('#_CampoObservacion').val('');
    if (estadoNuevo == '8') $('#_CampoObservacion').show(); else $('#_CampoObservacion').hide();

    if (bEstadoSeleccionado) {
        $('#modal_TEstadoOK').val(TEstadoOK); $('#modal_TEstadoOK').show();
        $('#modal_TEstadoNOOK').val(''); $("#modal_TEstadoNOOK").hide();
    } else {
        $('#modal_TEstadoOK').val(''); $('#modal_TEstadoOK').hide();
        $('#modal_TEstadoNOOK').val(TEstadoNOOK); $("#modal_TEstadoNOOK").show();
    }

    $('#modal_rut').val(rut);
    $('#modal_factura').val(factura);
    $('#modal_estadoActual').val(estadoActual);
    $('#modal_estadoNuevo').val(estadoNuevo);
    $('#modal_fila').val(fila);
    $('#modal_obj').val(obj);

    $('#_Acepta_Observacion').modal('show');
}
function putEstadoConfirmado(obj, rut, factura, estadoActual, estadoNuevo, fila, obs) {

    rutUsuario = sessionStorage.getItem("_pnacpacam_Rut");

    var json = {
        rutUsuario: rutUsuario,
        rutProveedor: rut,
        NFactura: factura,
        estadoActual: estadoActual,
        estadoNuevo: estadoNuevo,
        obs: obs
    };

    try {
        $.ajax({
            type: "GET",
            url: apiUrl("Proveedor/putEstado"),
            data: json,
            success: function (data) {

                console.log(data);

                if (!data.success) {
                    avisoFinProceso(data.message);
                    return;
                }

                _getFacturas();
            },
            //success: function (data) {
            //    console.log(data);
            //    if (!data.success) avisoFinProceso(data.message);
            //    if (data.resultado === null) {
            //        _getFacturas();
            //    } else {
            //        botonera = _administraEstados(data.resultado, fila);
            //        actualizarFilaFactura(rut, factura, estadoNuevo, {
            //            botonera: botonera
            //        });
            //    }
            //},
            error: function (xhr, status, error) {

                let mensaje = "No se pudo actualizar el estado de la factura.";

                // ✅ 1. Mensaje controlado desde el backend (recomendado)
                if (xhr.responseJSON && xhr.responseJSON.mensaje) {
                    mensaje = xhr.responseJSON.mensaje;
                }
                // ✅ 2. Mensaje plano (por ejemplo ArgumentException)
                else if (xhr.responseText) {
                    mensaje = xhr.responseText;
                }

                // Logs para diagnóstico
                if (xhr.status === 0) {
                    console.log("No hay conexión con el servidor.");
                } else if (xhr.status === 404) {
                    console.log("Recurso no encontrado (404).");
                    window.location.href = apiUrl("");
                } else if (xhr.status === 500) {
                    console.log("Error interno del servidor (500).");
                } else {
                    console.log("Error inesperado:", error);
                }

                // ✅ Mostrar mensaje real al usuario
                avisoFinProceso(mensaje);
            }
        });

    } catch (ex) {
        console.log(ex);
    }

    obj.disabled = true;
}
function putEstadoConfirmadoOLD(obj, rut, factura, estadoActual, estadoNuevo, fila, obs) {
    //console.log('estadoActual ->', estadoActual);
    rutUsuario = sessionStorage.getItem("_pnacpacam_Rut");
    var json = {
        rutUsuario: rutUsuario,
        rutProveedor: rut,
        NFactura: factura,
        estadoActual: estadoActual,
        estadoNuevo: estadoNuevo,
        obs: obs
    }
    try {
        $.ajax({
            type: "GET",
            url: apiUrl("Proveedor/putEstado"),
            data: json,
            success: function (data) {
                if (data.resultado === null) { _getFacturas(); } else {
                    botonera = _administraEstados(data.resultado, fila);
                    actualizarFilaFactura(rut, factura, estadoNuevo, {
                        botonera: botonera
                    });
                }
            },
            error: function (xhr, status, error) {
                if (xhr.status === 0) {
                    console.log("No hay conexión con el servidor. Verifica tu red.");
                } else if (xhr.status === 404) {
                    console.log("El recurso solicitado no fue encontrado (404).");
                    window.location.href = apiUrl(""); // si quieres redirigir
                } else if (xhr.status === 500) {
                    console.log("Error interno del servidor (500).");
                } else {
                    console.log("Ocurrió un error inesperado: " + error);
                }
                avisoFinProceso("No se pudo actualizar el estado de la factura.");
            }
        });
    } catch (ex) {
        console.log(ex.description);
    }
    obj.disabled = true;
}
function verExpediente(rut, factura) {
    existeExpediente(rut, factura);
}
function verFacturaComision(rut, factura) {
    existeFacturaComision(rut, factura);
}
function getExpediente(rut, factura) {
    $('#_Expediente_rutProveedor').val(rut);
    $('#_Expediente_nFactura').val(factura);
    $('#_Factura_Confirmar_Expediente').modal('show');
}
async function crearExpedienteCrea(rut, factura) {

    try {
        const res = await $.ajax({
            type: "GET",
            url: apiUrl("Proveedor/crearExpediente"),
            data: {
                rutProveedor: rut,
                factura: factura
            }
        });

        $('#_Factura_Confirmar_Expediente').modal('hide');

        if (!res.state) {
            avisoFinProceso(
                'Ocurrió un error, el expediente NO pudo ser construido. ' + res.message
            );
            throw new Error(res.message);
        }

        avisoFinProceso('Expediente Construido');
        return res;

    } catch (error) {

        if (error.status === 404) {
            $(".loader_Tablas").fadeOut("slow");
            alert('Tu sesión ha expirado.');
            window.location.href = apiUrl("");
        } else {
            showToast("Ocurrió algún error", "error", 4000);
        }

        console.error("Error en crearExpedienteCrea:", error);
        throw error;
    }
}

function resetFormExpedienteVer() {
    //    $('#formVerExpediente')[0].reset();
    //    $('#formVerExpediente').validate().resetForm();
}
function existeExpediente(rut, ndoc) {
    try {
        $.ajax({
            type: "GET",
            data: {
                rutProveedor: rut,
                factura: ndoc
            },
            url: apiUrl("Proveedor/existeExpediente"),
            success: function (res) {
                //console.log(res);

                if (res == 'True') $('#_Factura_modal-ver-Expediente').modal('show');
                else avisoFinProceso("El expediente solicitado aún no ha sido creado");

            },
            error: function (xhr) {//return json.state ? json.resultado : [];
                if (xhr.status === 404) { // 404: No se encuentra el recurso
                    $(".loader_Tablas").fadeOut("slow");
                    alert('Tu sesión ha expirado.');
                    window.location.href = apiUrl("");
                } else {
                    avisoFinProceso("Ocurrio algún error");
                }
            }
        });
    }
    catch (ex) {
        avisoFinProceso("Ocurrio algún error");
        console.error("Ha ocurrido un error : " + ex.name + " " + ex.message);
    }
}
function existeFacturaComision(rut, ndoc) {
    try {
        $.ajax({
            type: "GET",
            data: {
                rutProveedor: rut,
                factura: ndoc
            },
            url: apiUrl("Proveedor/existeFacturaComision"),
            success: function (res) {

                if (res == 'True') $('#_Factura_modal-ver-FacturaComision').modal('show');
                else avisoFinProceso("La factura de comisión solicitada aún no ha sido cargada");

            },
            error: function (xhr) {//return json.state ? json.resultado : [];
                if (xhr.status === 404) { // 404: No se encuentra el recurso
                    $(".loader_Tablas").fadeOut("slow");
                    alert('Tu sesión ha expirado.');
                    window.location.href = apiUrl("");
                } else {
                    avisoFinProceso("Ocurrio algún error");
                }
            }
        });
    }
    catch (ex) {
        avisoFinProceso("Ocurrio algún error");
        //console.error("Ha ocurrido un error : " + ex.name + " " + ex.message);
    }
}

function existeDocumentosAdjuntos(rut, ndoc) {
    try {
        $.ajax({
            type: "GET",
            data: {
                rutProveedor: rut,
                factura: ndoc
            },
            url: apiUrl("Proveedor/existeDocumentoAdjunto"),
            success: function (res) {

                if (res == 'True') $('#modal_Documentos_Adjuntos').modal('show');
                else avisoFinProceso("Documentos adjuntos solicitada aún no ha sido cargados");

            },
            error: function (xhr) {//return json.state ? json.resultado : [];
                if (xhr.status === 404) { // 404: No se encuentra el recurso
                    $(".loader_Tablas").fadeOut("slow");
                    alert('Tu sesión ha expirado.');
                    window.location.href = apiUrl("");
                } else {
                    avisoFinProceso("Ocurrio algún error");
                }
            }
        });
    }
    catch (ex) {
        avisoFinProceso("Ocurrio algún error");
        console.error("Ha ocurrido un error : " + ex.name + " " + ex.message);
    }
}



