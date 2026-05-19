async function verGuias(rut, factura) {
    anchorOFF('getGuias_' + rut + factura);
    await compruebaTablaSAP_ASYNC(rut, factura, 'verGuias');
}

async function compruebaTablaSAP_ASYNC(rut, factura, accion) {

    try {
        const res = await $.ajax({
            type: "GET",
            url: apiUrl("Proveedor/getCountVBPA")
        });

        if (!res.state) {
            avisoFinProceso(res.message);
            throw new Error(res.message);
        }

        if (accion === 'verGuias') {
            getGuiasByFactura(rut, factura);
            anchorON('getGuias_' + rut + factura);
            $('#_Factura_Guias').modal('show');
        }
        else if (accion === 'getExpediente') {
            await crearExpedienteCrea(rut, factura);
        }

        return res; // importante

    } catch (error) {

        // Error de sesión expirada
        if (error.status === 404) {
            $(".loader_Tablas").fadeOut("slow");
            alert('Tu sesión ha expirado.');
            window.location.href = apiUrl("");
        } else {
            showToast("Ocurrió algún error", "error", 4000);
        }

        console.error("Error:", error);
        throw error; // importante para el finally del botón
    }
}



function getGuiasByFactura(rut, factura) {

    let incluirDatosAdicionales = document.getElementById('incluirDatosAdicionales');
    
    $(".loader_Tablas").fadeIn("slow");

    var json = {
        rutProveedor: rut,
        factura: factura,
        incluye: incluirDatosAdicionales.checked
    }
    
    try {
        if ($.fn.DataTable.isDataTable('#tablaGuias')) {
            $('#tablaGuias').DataTable().destroy();  // Destruir previa instancia si existe
        }

        window.tablaguia = $('#tablaGuias').DataTable({
            "ajax": {
                url: apiUrl("Proveedor/getDespachosByFactura"),
                data: json,
                dataType: "json",
                "dataSrc": function (json) {

                    if (json.state === false) {
                        return [];
                    }

                    // Si no hubo error y hay datos
                    if (json.resultado && json.resultado.length > 0) {
                        return json.resultado;
                    }

                    return [];
                   
                },
                error: function (xhr) {
                    if (xhr.status === 401 || xhr.status === 440 || xhr.status === 419 || xhr.status === 404) {
                        avisoFinProceso("El recurso solicitado no está disponible.");
                    } else {
                        avisoFinProceso("Ocurrio algún error");
                    }
                }
            },
            "columns": [
                { "data": "NFactura" },
                { "data": "DocumentoVenta" },
                { "data": "PedidoCompra" },
                { "data": "GrupoArticulos" },
                { "data": "Canal" },
                { "data": "DescripcionCanal" },
                { "data": "GuiaDespacho" },
                { "data": "CodigoMaterial" },
                { "data": "Denominacion" },
                { "data": "Lote" },
                { "data": "CantidadDocumento" },
                { "data": "CantidadInformada" },
                { "data": "ValorNeto" },
                { "data": "ZZIDMER_E" },
                { "data": "RutEstablecimiento" },
                { "data": "Destinatario" },
                { "data": "ServicioSalud" }
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
            layout: {
                topStart: {
                    buttons: {
                        name: 'primary',
                        buttons: ['copy', 'csv', 'excel']
                    }
                }
            },
            destroy: true,
            scrollX: true,
            "autoWidth": false,
            paging: true,
            "initComplete": function () {
                $(".loader_Tablas").fadeOut("slow");

                $('#tablaGuias tbody').on('click', 'tr', function () {

                    const table = $('#tablaGuias').DataTable();
                    const data = table.row(this).data();

                    if (!data) return;

                    // Estilo de selección
                    $('#tablaGuias tbody tr').removeClass('row_selected');
                    $(this).addClass('row_selected');
                    
                    //if (existeDocumentoAjunto(data.LIFNR, data.NFactura))
                        // Acción principal
                        fn_getDatos_Documentos(
                            '',
                            '',
                            '',
                            '_guiaPreview',
                            'tablaDocumentos',
                            data.DocumentoVenta,
                            data.LIFNR,
                            '',
                            true
                        );
                   // else avisoFinProceso("Sin DOcumentos Adjuntos");
                });
                // Evento para mostrar/ocultar columnas
                document.querySelectorAll('a.toggle-vis').forEach((el) => {
                    el.addEventListener('click', function (e) {
                        e.preventDefault();
                        let columnIdx = parseInt(this.getAttribute('data-column'));
                        let column = window.tablaguia.column(columnIdx);
                        column.visible(!column.visible());
                    });
                });
            },
            error: function (jqXHR, textStatus, errorThrown) {
                console.error("Ha ocurrido un error : " + jqXHR.responseText);
            }
        });

    } catch (ex) {
        console.log("Error: " + ex.message);
    }
}



function verDocumentos(doc, rut) {

    var tbody = $("#tablaDocumentos tbody");
    tbody.empty();

    try {
        $.ajax({
            type: "GET",
            data: {
                rut: rut,
                doc: doc
            },
            url: apiUrl("Proveedor/ObtenerDocumentos"),
            success: function (res) {

                // Limpiar el contenido del iframe _guiaPreview
                const iframe = document.getElementById('_guiaPreview');
                iframe.src = 'about:blank';  

                if (res.state) {

                    if (res.archivos.Nombres == null) return null;

                    var tbody = $("#tablaDocumentos tbody");
                    tbody.empty();

                    if (res.archivos.Nombres.length > 0) {
                        $.each(res.archivos.Nombres, function (i, item) {
                            var fila = `
                                <tr>
                                    <td>${i + 1}</td>
                                    <td>${item.Nombre}</td>
                                    <td><a target='_guiaPreview' href='${res.archivos.Url}${item.Nombre}'><i class='fas fa-eye' style='font-size:28px'></i></a></td>
                                </tr>`;
                            tbody.append(fila);
                        });
                    } else {
                        tbody.append('<tr><td colspan="4" style="text-align:center;">No se encontraron documentos</td></tr>');
                    }


                } else {
                    console.error("Ha ocurrido un error : " + res.message);
                }

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
    }
    catch (ex) {
        avisoFinProceso("Ocurrio algún error");
        console.error("Ha ocurrido un error : " + ex.name + " " + ex.message);
    }
}













