$(document).ready(function () {

    //$(".Select2_facturaLista").select2({
    //    placeholder: "Seleccione Factura",
    //    allowClear: true
    //});
    $('.select2').select2({
        placeholder: "Seleccione",
        allowClear: true,
        width: '100%',
        minimumResultsForSearch: 5, // oculta buscador si hay pocas opciones
        language: {
            noResults: () => "No se encontraron facturas"
        }
    });

    $('.Select2_facturaLista').select2({
        placeholder: "Seleccione Factura",
        allowClear: true,
        width: '100%',
        minimumResultsForSearch: 5, // oculta buscador si hay pocas opciones
        language: {
            noResults: () => "No se encontraron facturas"
        }
    });


    // Deben existir ANTES de inicializar validate()
    $.validator.addMethod("rutNumerico", function (value, element) {
        if (this.optional(element)) return true;
        return /^\d{1,8}$/.test(String(value).trim());  // solo números, hasta 8
    }, "El RUT debe contener solo números (sin puntos ni guion).");

    $.validator.addMethod("facturaNumerica", function (value, element) {
        if (this.optional(element)) return true;
        return /^\d+$/.test(String(value).trim());      // solo números
    }, "La factura debe contener solo números.");

    $('#form-upload-documentos').validate({

        ignore: [], // IMPORTANTE para Select2

        rules: {
            _rutProveedorUpload: {
                required: true,
                maxlength: 8,
                rutNumerico: true
            },
            _facturaLista: {
                required: true,
                facturaNumerica: true
            },
            archivoPDF: {
                required: true,
                extension: "pdf"
            }
        },
        messages: {
            _rutProveedorUpload: {
                required: "Debe indicar el RUT del proveedor",
                maxlength: "El RUT no puede superar 8 dígitos"
            },
            _facturaLista: {
                required: "Debe seleccionar una factura"
            },
            archivoPDF: {
                required: "Debe seleccionar un archivo",
                extension: "Solo se permiten archivos PDF"
            }
        },
        errorClass: "input-error",
        errorPlacement: function (error, element) {
            error.insertAfter(element);
        },
        submitHandler: function (form) {
            //enviarDocumento(); 
        }
    });
});

function limpiarTablaDocumentosAdjuntos() {
    if ($.fn.DataTable.isDataTable('#tablaDocumentosAdjuntos-1')) {
        tableDocumentos.clear().draw(); // limpia todas las filas y actualiza la tabla
    }
}

function fn_getDatos_Documentos(td, full, modal, target, tabla, doc, rut, nombre, permiteVer) {

    try {

        // =========================
        // 1. LIMPIEZA DE UI
        // =========================

        // Limpiar iframe
        const iframe = document.getElementById(target);
        if (iframe) iframe.src = 'about:blank';

        // Limpiar tabla (estado UX limpio)
        inicializarOTablaVacia(tabla);

        // Mostrar feedback visual
        showSpinnerDocumentos('spinnerDocumentos');

        // Loading específico que ya usas
        if (target === "_guiaPreview") {
            showLoading('.box-loading');
        }

        // =========================
        // 2. ARMAR REQUEST
        // =========================

        let ruta, json;

        if (td === 'DA') {
            ruta = "Documentos/getDocumentosAdjuntos";
            if (full === "") full = doc;
            json = { rut: rut, factura: full };
        } else {
            ruta = "Proveedor/ObtenerDocumentos";
            json = { rut: rut, doc: doc };
        }
        console.log('3. LLAMADA AJAX');
        // =========================
        // 3. LLAMADA AJAX
        // =========================

        $.ajax({
            type: "GET",
            data: json,
            url: apiUrl(ruta),

            success: function (res) {

                if (modal !== "" && res?.state) {
                    document.getElementById('rutModal').value = rut;
                    document.getElementById('nombreModal').value = nombre;
                    document.getElementById('facturaModal').value = doc;
                    $('#' + modal).modal('show');
                //} else {
                //    hideSpinnerDocumentos('spinnerDocumentos');
                //    avisoFinProceso("Ocurrió algún error: " + res.message);
                //    return;
                }

                // Validación UX-friendly
                //if (
                //    !res ||
                //    !res.state ||
                //    !res.archivos ||
                //    !Array.isArray(res.archivos.Nombres) ||
                //    res.archivos.Nombres.length === 0
                //) {
                //    avisoFinProceso('Sin Documentos Adjuntos');
                //    console.info("No se encontraron documentos.");
                //    return;
                //}

                // Cargar datos
                res.archivos.Nombres.forEach(function (item, i) {

                    let url = td === 'DA'
                        ? `${res.archivos.UrlDA}${item.Nombre}`
                        : `${res.archivos.Url}${item.Nombre}`;

                    let fila = permiteVer
                        ? [
                            i + 1,
                            item.Nombre,
                            `<a target='${target}' href='${url}'>
                                <i class='fas fa-eye' style='font-size:28px'></i>
                             </a>`
                        ]
                        : [i + 1, item.Nombre];

                    tableDocumentos.row.add(fila);
                });

                tableDocumentos.draw();
            },

            complete: function () {
                hideSpinnerDocumentos('spinnerDocumentos');

                if (target === "_guiaPreview") {
                    hideLoading('.box-loading');
                }
            },

            error: function (xhr) {

                console.log(xhr);
                hideSpinnerDocumentos('spinnerDocumentos');

                if (xhr.status === 404) {
                    alert('Tu sesión ha expirado.');
                    window.location.href = apiUrl("");
                } else {
                    avisoFinProceso("Ocurrió algún error", xhr);
                }
            }
        });

    } catch (ex) {
        console.log(ex);
        hideSpinnerDocumentos('spinnerDocumentos');
        avisoFinProceso("Ocurrió algún error");
        console.error("Error:", ex);
    }
}
function fn_getDatos_DocumentosOLD(td, full, modal, target, tabla, doc, rut, nombre, permiteVer) {

    //if (existeDocumentoAjunto(rut, doc) === false) { avisoFinProceso("fn_getDatos_Documentos: Sin Documentos"); return; }
    // Limpiar iframe
    const iframe = document.getElementById(target);
    iframe.src = 'about:blank';



    let ruta, json;
    if (td == 'DA') {
        ruta = "Documentos/getDocumentosAdjuntos";
        if (full == "") full = doc;
        json = { rut: rut, factura: full };
    } else {
        ruta = "Proveedor/ObtenerDocumentos";
        json = { rut: rut, doc: doc };
    }

    try {
        if (target == "_guiaPreview") showLoading('.box-loading');

        $.ajax({
            type: "GET",
            data: json,
            url: apiUrl(ruta),
            success: function (res) {

                if (modal != "" && res.state ) {

                    document.getElementById('rutModal').value = rut;
                    document.getElementById('nombreModal').value = nombre;
                    document.getElementById('facturaModal').value = doc;
                    $('#' + modal).modal('show');
                }
                // Siempre asegurar estado limpio
                inicializarOTablaVacia(tabla);

                if (
                    !res ||
                    !res.state ||
                    !res.archivos ||
                    !Array.isArray(res.archivos.Nombres) ||
                    res.archivos.Nombres.length === 0
                ) {
                    avisoFinProceso('Sin Documentos Adjuntos')
                    // UX clara: tabla vacía + mensaje
                    console.info("No se encontraron documentos para los criterios ingresados.");
                    return;
                }

                // Si hay datos, agregarlos
                res.archivos.Nombres.forEach(function (item, i) {

                    let url = td === 'DA'
                        ? `${res.archivos.UrlDA}${item.Nombre}`
                        : `${res.archivos.Url}${item.Nombre}`;

                    let fila = permiteVer
                        ? [
                            i + 1,
                            item.Nombre,
                            `<a target='${target}' href='${url}'>
                    <i class='fas fa-eye' style='font-size:28px'></i>
                  </a>`
                        ]
                        : [i + 1, item.Nombre];

                    tableDocumentos.row.add(fila);
                });

                tableDocumentos.draw();
            },

            complete: function () {
                if (target == "_guiaPreview") hideLoading('.box-loading');
            },
            error: function (xhr) {
                if (xhr.status === 404) {
                    $(".loader_Tablas").fadeOut("slow");
                    alert('Tu sesión ha expirado.');
                    window.location.href = apiUrl("");
                } else {
                    avisoFinProceso("Ocurrio algún error");
                }
            }
        });

    } catch (ex) {
        avisoFinProceso("Ocurrio algún error");
        console.error("Error: " + ex.name + " " + ex.message);
    }
}
// ✅ Nombre corregido y retorno normalizado a boolean
async function existeDocumentoAdjunto002(rut, ndoc) {
    try {
        const res = await $.ajax({
            type: "GET",
            url: apiUrl("Proveedor/existeDocumentoAdjunto"),
            data: { rutProveedor: rut, factura: ndoc },
            dataType: "json",     // si tu back devuelve JSON boolean; ajusta si fuera texto
            cache: false
        });

        // Normaliza: acepta true/false, "true"/"false", 1/0
        if (typeof res === "boolean") return res;
        if (typeof res === "number") return res === 1;
        if (typeof res === "string") return res.trim().toLowerCase() === "true" || res.trim() === "1";

        // Si viniera un objeto { existe: true }:
        if (res && typeof res === "object" && "existe" in res) {
            const v = res.existe;
            return v === true || v === 1 || (typeof v === "string" && (v.trim().toLowerCase() === "true" || v.trim() === "1"));
        }

        // por defecto
        return !!res;
    } catch (err) {
        console.error("existeDocumentoAdjunto:error", err);
        return false; // ante error de red/servidor, comportarse como que no existe
    }
}
async function existeDocumentoAdjunto001(rut, ndoc) {
    try {
        const res = await $.ajax({
            type: "GET",
            url: apiUrl("Proveedor/existeDocumentoAdjunto"),
            data: { rutProveedor: rut, factura: ndoc },
            dataType: "json",   // el back devuelve true/false JSON
            cache: false
        });
        // Si el back retorna boolean JSON, res ya es true/false
        return !!res;
    } catch (err) {
        console.error("existeDocumentoAdjunto:error", err);
        return false; // ante error, que se comporte como "no existe"
    }
}

function existeDocumentoAjunto(rut, ndoc) {
    try {
        $.ajax({
            type: "GET",
            data: {
                rutProveedor: rut,
                factura: ndoc
            },
            url: apiUrl("Proveedor/existeDocumentoAdjunto"),
            success: function (res) {

                if (res) return true;
                else return false;

            },
            error: function (xhr) {//return json.state ? json.resultado : [];
                return false;
            }
        });
    }
    catch (ex) {
        return false;
    }
}
function inicializarOTablaVacia(tablaId) {

    if ($.fn.DataTable.isDataTable('#' + tablaId)) {
        tableDocumentos.clear().draw();
    } else {
        tableDocumentos = $('#' + tablaId).DataTable({
            paging: true,
            searching: false,
            info: false,
            autoWidth: false,
            language: {
                emptyTable: "No se encontraron documentos"
            }
        });
    }
}

$(document).on('change', '#_nombreProveedorUpload', function () {
    document.getElementById('_rutProveedorUpload').value = this.value
    _getListFacturas(document.getElementById('_rutProveedorUpload').value,'_facturaLista');
});


$(document).on('change', '#_rutProveedorUpload', function () {

    const rut = String(this.value).trim();

    if (!validarProveedorPorRut(rut)) {
        return;
    }

    // Si llega aquí → proveedor válido
    _getListFacturas(rut, '_facturaLista');
});

function limpiarFacturas() {
    const $facturas = $('#_facturaLista');
    if ($facturas.length) {
        $facturas
            .empty()
            .append(new Option("Seleccione Factura", "", true, true))
            .val(null)
            .trigger('change.select2');
    }
}
function limpiarProveedorYFacturas(mensajeUsuario) {

    $('#_rutProveedorUpload').val('');

    $('#_nombreProveedorUpload')
        .val(null)
        .trigger('change.select2');

    $('#_dspNombreProveedorUpload').val('');

    limpiarFacturas();

    limpiarTablaDocumentosAdjuntos();

    if (mensajeUsuario) {
        avisoFinProceso(mensajeUsuario);
    }
}

function validarFormularioUpload() {

    limpiarErroresFormulario();

    const rut = $('#_rutProveedorUpload').val();

    if (!validarProveedorPorRut(rut)) {
        return false;
    }

    if (!$('#_facturaLista').val()) {
        avisoFinProceso('Debe seleccionar una factura válida.');
        return false;
    }

    const fileInput = $('#file-upload-documento')[0];
    if (!fileInput.files || fileInput.files.length === 0) {
        avisoFinProceso('Debe seleccionar un archivo PDF.');
        return false;
    }

    if (fileInput.files[0].type !== 'application/pdf') {
        avisoFinProceso('El archivo debe ser un PDF.');
        return false;
    }

    return true;
}
function marcarProveedorValido() {
    $('#_rutProveedorUpload')
        .removeClass('input-error')
        .addClass('input-success');
}
function marcarProveedorInvalido() {
    $('#_rutProveedorUpload')
        .removeClass('input-success')
        .addClass('input-error');
}



//$(document).on('change', '#_rutProveedorUpload', function () {

//    const $rut = $('#_rutProveedorUpload');
//    const $selectProv = $('#_nombreProveedorUpload');
//    const $dspNombre = $('#_dspNombreProveedorUpload');

//    // Si faltan elementos críticos, no loguees; muestra mensaje y sal
//    if ($rut.length === 0 || $selectProv.length === 0 || $dspNombre.length === 0) {
//        // Aquí puedes optar por NO mostrar nada al usuario si esto solo pasa en otras vistas
//        // o mostrar un mensaje genérico:
//        // avisoFinProceso("No fue posible procesar la selección del proveedor. Recargue la página.");
//        return;
//    }

//    const rutProveedor = Number($rut.val());

//    if (Number.isNaN(rutProveedor)) {
//        limpiarProveedorYFacturas("El RUT ingresado no es válido. Verifique e intente nuevamente.");
//        return;
//    }

//    try {
//        const valor = String(this.value).trim();

//        // Busca opción exacta por value
//        const indiceSeleccionado = $selectProv.find(`option[value='${valor}']`).index();

//        if (indiceSeleccionado > 0) {
//            // Proveedor encontrado
//            $selectProv.prop('selectedIndex', indiceSeleccionado).trigger('change.select2');

//            $dspNombre.val($selectProv.find('option').eq(indiceSeleccionado).text());

//            // Cargar facturas asociadas
//            _getListFacturas(valor, '_facturaLista');

//        } else {
//            // Proveedor NO encontrado → limpiar y avisar
//            limpiarProveedorYFacturas("Proveedor no encontrado. Verifique el RUT ingresado o seleccione un proveedor válido.");
//        }

//    } catch (error) {
//        console.error(error);
//        limpiarProveedorYFacturas("No fue posible validar el proveedor. Intente nuevamente.");
//    }
//});



//$('#_rutProveedorUpload').on('change', function () {
//    let rutProveedor = +document.getElementById('_rutProveedorUpload').value;
//    if (!isNaN(rutProveedor)) {
//        try {

//            destino = '_nombreProveedorUpload';
//            indiceSeleccionado = $("#_nombreProveedorUpload option[value='" + this.value + "']").index();

//            if (indiceSeleccionado > 0) {

//                document.getElementById("_nombreProveedorUpload").options.item(indiceSeleccionado).selected = 'selected';
//                document.getElementById('_dspNombreProveedorUpload').value = document.getElementById("_nombreProveedorUpload").options.item(indiceSeleccionado).text
//                $("#" + destino).change();

//            } else {

//                avisoFinProceso('Proveedor NO Hallado');
//                document.getElementById("_nombreProveedorUpload").options.item(0).selected = 'selected';
//                document.getElementById('_dspNombreProveedorUpload').value = "";
//                $("#" + destino).change();

//            }

//        } catch (error) {
//            console.error(error);
//        }
//    } else {
//        $('#modalMensajeSistema').modal('show');
//        $('#mensajeSistema').text('Rut de Proveedor no es válido');
//    }

//});

$('#form-upload-documentos').on('submit', function (event) {

    event.preventDefault();

    btnPressed = 'btnFileUploadDocumento';

    if (!validarFormularioUpload()) {
        btnON(btnPressed, 'Cargar');
        return;
    }

    btnOFF(btnPressed, 'Cargando Documento...');
    //mostrarMensaje('Cargando documento, por favor espere.');

    let factura = $('#_facturaLista').val();
    let rut = $('#_rutProveedorUpload').val();
    let tdoc = $('#_tipoDocumentoUpload').val();
    let fileInput = $('#file-upload-documento')[0];
    let antiForgeryToken = $('input[name=__RequestVerificationToken]').val();

    let formData = new FormData();
    formData.append('documentoUpload', fileInput.files[0]);
    formData.append('__RequestVerificationToken', antiForgeryToken);
    formData.append('_facturaLista', factura);
    formData.append('_rutProveedorUpload', rut);
    formData.append('_tipoDocumento', tdoc);

    $("#nombreFile").text('Documento: ' + fileInput.files[0].name);
//    console.log('$(#form-upload-documentos).on(submit, function (event)');
    $.ajax({
        url: apiUrl("Documentos/subirDocumento"),
        type: 'POST',
        data: formData,
        processData: false,
        contentType: false,

        success: function (response) {
            if (response.status) {
                showToast("Documento cargado.", "success", 3000);
                llamarDocumentosAdjuntos();
                //mostrarMensaje('Documento adjuntado de forma exitosa');
            } else {
                showToast("Ocurrió algún error. " + response.message, "error", 4000);
                //mostrarMensaje(response.message || 'Error al cargar el documento');
            }
            btnON(btnPressed, 'Cargar');
        },

        error: function () {
            console.log(error);
            showToast("Ocurrió algún error.", "error", 4000); //mostrarMensaje('No se ha podido cargar el archivo');
            btnON(btnPressed, 'Cargar');
        }
    });

});

function enviarDocumento() {
//    console.log('enviarDocumento');
    const btnPressed = 'btnFileUploadDocumento';

    btnOFF(btnPressed, 'Cargando Documento...');
    //mostrarMensaje('Cargando documento, por favor espere.');

   // let formData = new FormData($('#form-upload-documentos')[0]);

    let factura = $('#_facturaLista').val();
    let rut = $('#_rutProveedorUpload').val();
    let tdoc = $('#_tipoDocumentoUpload').val();
    let fileInput = $('#file-upload-documento')[0];
    let antiForgeryToken = $('input[name=__RequestVerificationToken]').val();

    let formData = new FormData();
    formData.append('documentoUpload', fileInput.files[0]);
    formData.append('__RequestVerificationToken', antiForgeryToken);
    formData.append('_facturaLista', factura);
    formData.append('_rutProveedorUpload', rut);
    formData.append('_tipoDocumento', tdoc);

    $.ajax({
        url: apiUrl("Documentos/subirDocumento"),
        type: 'POST',
        data: formData,
        processData: false,
        contentType: false,

        success: function (response) {
            if (response.status) {
                showToast("Documento cargado.", "success", 3000);
               // mostrarMensaje('Documento adjuntado de forma exitosa');
            } else {
                showToast("Ocurrió algún error.", "error", 4000);
                //mostrarMensaje(response.message || 'Error al cargar el documento');
            }
            btnON(btnPressed, 'Cargar');
        },

        error: function () {
            showToast("Ocurrió algún error.", "error", 4000);
            //mostrarMensaje('No se ha podido cargar el archivo');
            btnON(btnPressed, 'Cargar');
        }
    });
}
function validarFormularioUpload() {

    limpiarErroresFormulario();

    const rut = $('#_rutProveedorUpload');
    const factura = $('#_facturaLista');
    const fileInput = $('#file-upload-documento')[0];

    if (!rut.val()) {
        mostrarError(rut, 'Debe indicar el RUT del proveedor.');
        return false;
    }

    if (!factura.val()) {
        mostrarError(factura, 'Debe seleccionar una factura.');
        return false;
    }

    if (!fileInput.files || fileInput.files.length === 0) {
        mostrarError($('#file-upload-documento'), 'Debe seleccionar un archivo PDF.');
        return false;
    }

    // Validar tipo de archivo
    if (fileInput.files[0].type !== 'application/pdf') {
        mostrarError($('#file-upload-documento'), 'El archivo debe ser un PDF.');
        return false;
    }

    return true;
}
function limpiarErroresFormulario() {
    $('.input-error').removeClass('input-error');
    $('.accion_cargar_mensaje').attr('hidden', true).text('');
}

function mostrarError($el, mensaje) {
    $el.addClass('input-error');
    avisoFinProceso(mensaje);
}

function mostrarMensaje(mensaje) {
    $('.accion_cargar_mensaje')
        .removeAttr('hidden')
        .text(mensaje);
}


function llamarDocumentosAdjuntos() {
    let rut = $('#_rutProveedorUpload').val();
    let factura = $('#_facturaLista').val();
    if (rut == null) return avisoFinProceso("Ingrese el rut de proveedor");
   // if (existeDocumentoAjunto(rut, factura)) 
        fn_getDatos_Documentos(
            "DA",
            "*",
            "",
            "_documentoAdjuntoPreview",
            "tablaDocumentosAdjuntos-1",
            "*",
            rut,
            "",
            false)
   // else avisoFinProceso("Sin Documentos Adjuntos")
    _getFacturaComision(rut, factura)
}

function existeDocumentoAdunto(rut, ndoc) {
    try {
        $.ajax({
            type: "GET",
            data: {
                rutProveedor: rut,
                factura: ndoc
            },
            url: apiUrl("Documentos/existeDocumentoAdjunto"),
            success: function (res) {

                if (res == 'True') $('#_Factura_modal-ver-FacturaComision').modal('show');
                else avisoFinProceso("Ningún documento ha sido cargado");

            },
            error: function (xhr) {//return json.state ? json.resultado : [];
                if (xhr.status === 404) { // 404: No se encuentra el recurso
                    $(".loader_Tablas").fadeOut("slow");
                    alert('10. Tu sesión ha expirado.');
                    window.location.href = apiUrl("");
                } else {
                    showToast("Ocurrió algún error.", "error", 4000);
                   // avisoFinProceso("Ocurrio algún error");
                }
            }
        });
    }
    catch (ex) {
        showToast("Ocurrió algún error.", "error", 4000);
        //avisoFinProceso("Ocurrio algún error");
        console.error("Ha ocurrido un error : " + ex.name + " " + ex.message);
    }
}






//function limpiarProveedorYFacturas(mensajeUsuario) {
//    // Limpia proveedor (select)
//    const $selectProv = $('#_nombreProveedorUpload');
//    if ($selectProv.length) {
//        $selectProv.prop('selectedIndex', 0).trigger('change'); // si es select normal
//        // si es Select2, también:
//        $selectProv.val(null).trigger('change.select2');
//    }

//    // Limpia nombre visible
//    const $dspNombre = $('#_dspNombreProveedorUpload');
//    if ($dspNombre.length) $dspNombre.val('');

//    // Limpia factura lista (Select2)
//    const $facturas = $('#_facturaLista');
//    if ($facturas.length) {
//        $facturas.empty().append(new Option("Seleccione Factura", "", true, true));
//        $facturas.val(null).trigger('change.select2');
//    }

//    // Limpia input RUT (opcional)
//    // $('#_rutProveedorUpload').val('');

//    // Mensaje formal al usuario
//    if (mensajeUsuario) {
//        // Usa tu mensajería existente
//        avisoFinProceso(mensajeUsuario);
//        // o si ya usas el sistema elegante:
//        // showToast(mensajeUsuario, "error", 6000);
//    }
//}

















function validarProveedorPorRut(rut) {
    const $selectProv = $('#_nombreProveedorUpload');
    const $dspNombre = $('#_dspNombreProveedorUpload');

    if (!rut || !/^\d{1,8}$/.test(rut)) {
        limpiarProveedorYFacturas("El RUT ingresado no es válido.");
        return false;
    }

    const $opcion = $selectProv.find(`option[value='${rut}']`);

    if ($opcion.length === 0) {
        limpiarProveedorYFacturas("El proveedor no existe en la lista precargada.");
        return false;
    }

    // Proveedor válido
    $selectProv.val(rut).trigger('change.select2');
    $dspNombre.val($opcion.text());

    return true;
}


// getFacturaComision(string rut, string factura)
function _getFacturaComision(rut, factura) {

    var json = {
        rut: rut,
        factura: factura
    }

    try {
        $.ajax({
            type: "GET",
            url: apiUrl("Documentos/getFacturaComision"),
            data: json,
            success: function (res) {
                $('#upload_facturaComision').html(res.archivo);
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