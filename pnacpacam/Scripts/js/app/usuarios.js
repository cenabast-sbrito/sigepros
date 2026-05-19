$(document).ready(function () {
   
    $(document).on('click', '#guardarNuevoUsuario', function () {
        setUsuario();
        getTableroUsuario(); 
    });
    $(document).on('click', '#modificarUsuario', function () {
        putUsuario();
    });
    // Desactivar proveedor
    $(document).on('click', '.accionDesactivarUsuario', function () {
        var rut = $(this).parents('tr').find('td:eq(0)').html();
        toggleUsuario(rut,0);
        getTableroProveedores();
    });
    // Activar proveedor
    $(document).on('click', '.accionActivarUsuario', function () {
        var rut = $(this).parents('tr').find('td:eq(0)').html();
        toggleUsuario(rut, 1);
        getTableroProveedores();
    });

    $("#rutInsumo").Rut({
        format_on: 'keyup'
    })

    $(document).on('click', '.btnModificarUsuario', function () {

        $('#guardarNuevoUsuario').hide();
        $('#modificarUsuario').show();

        $('#rut').val('');
        $('#rut').prop('disabled', true);

        var rut = $(this).data('rut');
        GetUsuario(rut);

        //$('#guardarNuevoProveedor').hide();
        //$('#modalModificarProveedor').show();
        //$('#nombreProveedorMod').val('');
        //$('#rutProveedorMod').val('');
        //var rut = $(this).parents('tr').find('td:eq(0)').html();
        //GetProveedor(rut);

        $('#organismo').prop('disabled', true);

        ///document.getElementById("organismo").disabled = true;


        $('#modalNuevoUsuario').modal('show');
    });
    
    $(document).on('click', '#asignarRolUsuario', function () {

        rolUsuario = $('#listaRoles').val(); 
        rut = $('#rut').val();

        setRolUsuario(rut, rolUsuario)
        

    });

    $(document).on('click', '#btnNuevoUsuario', function () {

        $('#guardarNuevoUsuario').show();
        $('#modificarUsuario').hide(); 

        $('#formUsuario')[0].reset();
        $('#rut').val(''); $('#rut').prop('disabled', false);

        $('#organismo').prop('disabled', false);

        //_getListRoles("usuario_rol");

    });
    $(document).on('click', '.nombre-seleccionado', function () {
        var rut = $(this).attr('id');
        var nombre = $(this).attr('data');
        $('#nombre').val(nombre);
        $('#rut').val(rut); 
        $("#sugerencia-nombre").html("");
    });
});
function GetName(val) {
    try {
        $.ajax({
            type: "GET",
            url: '../Usuario/GetName',
            data: 'name=' + val,
            success: function (data) {
                var opt = "";
                for (var user of data) {
                    opt = opt + "<div><a class='nombre-seleccionado' id='" + user.rut + "' data='" + user.nombre + "'>" + user.nombre + "</a></div>";
                }
                $("#sugerencia-nombre").html(opt);
            },
            error: function (xhr) {
                if (xhr.status === 404) { // 404: No se encuentra el recurso
                    $(".loader_Tablas").fadeOut("slow");
                    alert('003 Tu sesión ha expirado.');
                    window.location.href = apiUrl("");
                } else {
                    avisoFinProceso("Ocurrio algún error");
                }
            }
        });
    } catch (ex) {
        showToast(ex.description, "error", 4000);
        //console.log(ex.description);
    }
}
function setUsuario() {

    var data = {
        "Rut": $('#rut').val(),
        "Estado": "true",
        "Rol": $('#usuario_rol').val(),
        "Nombre": $('#nombre').val(),
        "Apellido": $('#apellido').val(),
        "Organismo": $('#organismo').val(),
        "Clave": $('#usuario_clave').val()
    };
    try {
        $.ajax({
            type: "post",
            url: '../Usuario/SetUsuario',
            contentType: 'application/json; charset=utf-8',
            data: JSON.stringify(data),
            success: function (res) {
                $('#tableroUsuario').DataTable().ajax.reload(null, false);
                $('#modalNuevoUsuario').modal('hide');
                $('#modalMensajeSistema').modal('show');
                $('#mensajeSistema').text(res.message);
            },
            error: function (xhr) {
                if (xhr.status === 404) { // 404: No se encuentra el recurso
                    $(".loader_Tablas").fadeOut("slow");
                    alert('004. Tu sesión ha expirado.');
                    window.location.href = apiUrl("");
                } else {
                    avisoFinProceso("Ocurrio algún error");
                }
            }
        });
    } catch (ex) {
        showToast(ex.description, "error", 4000);
        //alert(ex.description);
    }
        
}

function GetUsuario(rut) {
    try {
        $.ajax({
            type: "post",
            url: '../Usuario/GetUsuario',
            contentType: 'application/json; charset=utf-8',
            data: '{ rut : '+rut+'}',
            success: function (res) {   
                $('#nombre').val(res.Nombre);
                $('#apellido').val(res.Apellido);
                $('#organismo').val(res.Organismo.trim());
                $('#rut').val(res.Rut); 
                $('#usuario_rol').val(res.Rol);
                $('#estado').val(res.Estado === true ? 1 : 0);
            },
            error: function (xhr) {
                if (xhr.status === 404) { // 404: No se encuentra el recurso
                    $(".loader_Tablas").fadeOut("slow");
                    alert('005. Tu sesión ha expirado.');
                    window.location.href = apiUrl("");
                } else {
                    avisoFinProceso("Ocurrio algún error");
                }
            }
        });
    } catch (ex) {
        showToast(ex.description, "error", 4000);
        //alert(ex.description);
    }
}
function GetFuncionario(rut) {

    try {
        $.ajax({
            type: "post",
            url: '../Usuario/GetFuncionario',
            contentType: 'application/json; charset=utf-8',
            data: '{ rut : ' + rut + '}',
            success: function (res) {
                $('#CodigoDpto').val(res.IdUnidadGestora, GetResponsableUnidadGestora(res.IdUnidadGestora))
            },
            error: function (xhr) {
                if (xhr.status === 404) { // 404: No se encuentra el recurso
                    $(".loader_Tablas").fadeOut("slow");
                    alert('006. Tu sesión ha expirado.');
                    window.location.href = apiUrl("");
                } else {
                    avisoFinProceso("Ocurrio algún error");
                }
            }
        });
    } catch (ex) {
        showToast(ex.description, "error", 4000);
        //alert(ex.description);
    }
}

function putUsuario() {
    var data = {
        "Rut": $('#rut').val(),
        "Rol": $('#usuario_rol').val(),
        "Nombre": $('#nombre').val(),
        "Apellido": $('#apellido').val(),
        "Organismo": $('#organismo').val(),
        "Clave": $('#usuario_clave').val(),
        "Estado": $('#usuario_estado').val() == "1" ? true : false
    };
    try {
        $.ajax({
            type: "post",
            url: '../Usuario/PutUsuario',
            contentType: 'application/json; charset=utf-8',
            data: JSON.stringify(data),
            success: function (res) {
                $('#tableroUsuario').DataTable().ajax.reload(null, false);
                $('#modalNuevoUsuario').modal('hide');
                $('#modalMensajeSistema').modal('show');
                $('#mensajeSistema').text(res.message);
                ///console.log(res);
                actualizarFilaUsuario(res.rut, {
                    Nombre: res.nombre,
                    Apellido: res.Apellido,
                    Rol: res.Rol,
                    Estado: true
                });
            },
            error: function (xhr) {//return json.state ? json.resultado : [];
                if (xhr.status === 404) { // 404: No se encuentra el recurso
                    $(".loader_Tablas").fadeOut("slow");
                    alert('007. Tu sesión ha expirado.');
                    window.location.href = apiUrl("");
                } else {
                    avisoFinProceso("Ocurrio algún error");
                }
            }
        });
    } catch (ex) {
        showToast(ex.description, "error", 4000);
        //alert(ex.description);
    }
}



function putFuncionario() {
    var data = {
        "Estado": false, //inicialmente se crea al funcionario en insumos sin nungun tipo de rol y desactivado
        "Rut": $('#rutInsumo').val().split('-')[0],
        "Nombre": $('#nombreInsumo').val(),
        "Apellido": $('#apellidoInsumo').val(),
        "Email": $('#emailInsumo').val()
    };

    try {
        $.ajax({
            type: 'put',
            url: '../Usuario/PutFuncionario',
            contentType: 'application/json;  charset=utf-8',
            data: JSON.stringify(data),
            success: function (res) {
                /*SBRITO CORREGIR*/
                $('#modalNuevoUsuario').modal('hide');
                $('#modalMensajeSistema').modal('show');
                $('#mensajeSistema').text(res.message);
                $('#tableroUsuario').DataTable().ajax.reload(null, false);
                /*SBRITO CORREGIR*/

            },
            error: function (xhr) {//return json.state ? json.resultado : [];
                if (xhr.status === 404) { // 404: No se encuentra el recurso
                    $(".loader_Tablas").fadeOut("slow");
                    alert('008. Tu sesión ha expirado.');
                    window.location.href = apiUrl("");
                } else {
                    avisoFinProceso("Ocurrio algún error");
                }
            }
        });
    } catch (ex) {
        showToast(ex.description, "error", 4000);
        //alert(ex.description);
    }

}

function getTableroUsuario() {
    try {
        $('#tableroUsuario').DataTable({
            "ajax": {
                url: '../Usuario/GetUSuarios',
                dataSrc: ''
            },
            "columns": [
                { "data": "Rut" },
                { "data": "Nombre" },
                { "data": "Apellido" },
                { "data": "Rol" },
                {
                    "data": null,
                    "render": function (data, type, row) {
                        //console.log(row);
                        const estado = row.Estado;     // null o 1 = activo
                        const rut = row.RutProveedor;

                        if (estado == null || estado === true) {
                            // Activo → mostrar botón para DESACTIVAR
                            return `
                            <i class="fas fa-toggle-on action-icon icon-toggle-on accionDesactivarUsuario"
                               title="Desactivar Usuario"
                               aria-label="Desactivar Usuario"
                               role="button"
                               data-rut="${rut}">
                            </i>`;
                        } else {
                            // Inactivo → mostrar botón para ACTIVAR
                            return `
                            <i class="fas fa-toggle-off action-icon icon-toggle-off accionActivarUsuario"
                               title="Activar Usuario"
                               aria-label="Activar Usuario"
                               role="button"
                               data-rut="${rut}">
                            </i>`;

                        }
                    }
                },
                {
                    "data": "Rut",
                    "render": function (data, type, row) {
                        //console.log(row);
                        const Rut = row.Rut;
                        const estado = row.Estado;
                        const Nombre = row.Nombre || '';
                        const Apellido = row.Apellido;
                        const Rol = row.Rol;
                        const Organismo = row.Organismo;

                        if (estado == null || estado === true) {
                            // Ícono lápiz habilitado
                            return `
                                <i class="fas fa-edit action-icon icon-edit btnModificarUsuario"
                                   title="Modificar Usuario"
                                   aria-label="Modificar Usuario"
                                   role="button"
                                   data-rut="${Rut}">
                                </i>`;
                        } else {
                            // Inactivo → deshabilitado
                            return `
                                <i class="fas fa-edit action-icon disabled"
                                   title="Usuario desactivado"
                                   aria-label="Usuario desactivado">
                                </i>`;
                        }
                       // return "<button class='btn btn-primary btnModificarUsuario' data-toggle='modal' data-target='#modalNuevoUsuario' data-rut='" + data + "'>Modificar</button>";
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
            scrollX: true,
            "dom": "<'row '<'toolbar'><'col-sm-5'l><'col-sm-4'f>>" +
                "<'row'<'col-sm-12'tr>>" +
                "<'row'<'col-sm-6'i><'col-sm-6'p>>"
        });
        $('div.toolbar').html("<div class='col-sm-3'><button class='btn btn-primary' style='margin-right:3px;' data-toggle='modal' data-target='#modalNuevoUsuario' id='btnNuevoUsuario'>Nuevo Usuario</button></div>");

    } catch (ex) {
        showToast(ex.description, "error", 4000);
        //alert(ex.description);
    }
}

function actualizarFilaUsuario(rutModificado, nuevoUsuario) {

    var tabla = $('#tableroUsuario').DataTable();

    tabla.rows().every(function () {
        var data = this.data();
        if (data.Rut === rutModificado) {
            // Actualiza los campos necesarios
            data.Nombre = nuevoUsuario.Nombre;
            data.Apellido = nuevoUsuario.Apellido;
            data.Rol = nuevoUsuario.Rol;
            data.Estado = nuevoUsuario.Estado;

            // Aplica los cambios
            this.data(data).draw(false); // false evita que se reinicie la paginación
        }
    });

}

function validaRut(rut) {

    // Despejar Puntos
    var valor = rut.value.split('.').join("");;
    // Despejar Guión
    valor = valor.replace('-', '');
    // Aislar Cuerpo y Dígito Verificador
    cuerpo = valor.slice(0, -1);
    dv = valor.slice(-1).toUpperCase();
    // Formatear RUN
    rut.value = cuerpo + '-' + dv
    //
    // Si no cumple con el mínimo ej. (n.nnn.nnn)
    // if (cuerpo.length < 7) { rut.setCustomValidity("RUT Incompleto"); return false; }
    // Calcular Dígito Verificador
    suma = 0;
    multiplo = 2;
    // Para cada dígito del Cuerpo
    for (i = 1; i <= cuerpo.length; i++) {

        // Obtener su Producto con el Múltiplo Correspondiente
        index = multiplo * valor.charAt(cuerpo.length - i);

        // Sumar al Contador General
        suma = suma + index;

        // Consolidar Múltiplo dentro del rango [2,7]
        if (multiplo < 7) { multiplo = multiplo + 1; } else { multiplo = 2; }

    }

    // Calcular Dígito Verificador en base al Módulo 11
    dvEsperado = 11 - (suma % 11);

    // Casos Especiales (0 y K)
    dv = (dv == 'K') ? 10 : dv;
    dv = (dv == 0) ? 11 : dv;

//    console.log('VALIDANDO....');

    // Validar que el Cuerpo coincide con su Dígito Verificador
    if (dvEsperado != dv) {
        return 'ERROR';
    } else {
        return '';
    }


    // Si todo sale bien, eliminar errores (decretar que es válido)
}

function toggleUsuario(rut, estado) {
    // Normaliza y valida insumos
    const payload = {
        Rut: String(rut || "").trim(),
        Estado: estado ? 1 : 0
    };

    if (!payload.Rut) {
        if (typeof showToast === "function") showToast("El RUT es obligatorio.", "error");
        return;
    }
    
    // Si lo llamas desde un ícono/botón, puedes pasar el elemento y bloquearlo acá.
    // Ejemplo: setIconProcessing($icon, true);

    $.ajax({
        type: "POST",
        url: "../Usuario/toggleUsuario",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        data: JSON.stringify(payload),

        //beforeSend: function () {
        //    // Mensaje elegante en progreso (opcional)
        //    if (typeof showToast === "function") {
        //        window.__toastToggle = showToast("Actualizando estado del usuario…", "info", 10000);
        //    }
        //},

        success: function (res) {
           //  console.log(res.ok, res.message);
            const ok = (res && (res.ok === true || res.status === true));
            const msg = (res && res.message) ? res.message : (ok
                ? "Estado actualizado correctamente."
                : "No fue posible actualizar el estado del usuario.");

            if (ok) {///if (response && response.status === true) {
                showToast("La información ha sido consolidada exitosamente.", "success", 3000);
                getTableroUsuario(); 
            } else {
                showToast(msg, "error", 4000);
            }

        },

        error: function (xhr) {
            if (typeof closeToast === "function" && window.__toastToggle) {
                closeToast(window.__toastToggle);
                window.__toastToggle = null;
            }

            // Intenta leer mensaje desde el server
            let serverMsg = "";
            if (xhr && xhr.responseJSON && xhr.responseJSON.message) serverMsg = xhr.responseJSON.message;
            else if (xhr && xhr.responseText) {
                try {
                    const tmp = JSON.parse(xhr.responseText);
                    if (tmp && tmp.message) serverMsg = tmp.message;
                } catch (_) { /* ignore */ }
            }

            // Clasificación por status
            if (xhr && xhr.status === 404) {
                $(".loader_Tablas").fadeOut("slow");
                if (typeof showToast === "function") showToast("Tu sesión ha expirado. Redirigiendo…", "error", 5000);
                setTimeout(() => window.location.href = apiUrl(""), 1500);
                return;
            }

            if (xhr && (xhr.status === 400 || xhr.status === 422)) {
                if (typeof showToast === "function") showToast(serverMsg || "Solicitud inválida.", "error", 6000);
                return;
            }

            if (xhr && xhr.status === 409) {
                if (typeof showToast === "function") showToast(serverMsg || "No es posible cambiar el estado por una condición de negocio.", "error", 6000);
                return;
            }

            // Genérico
            if (typeof showToast === "function") showToast(serverMsg || "Ocurrió un error al actualizar el usuario.", "error", 6000);
            // console.error("toggleUsuario error", xhr);
        },

        complete: function () {
            // Rehabilitar UI si la bloqueaste (ícono/botón)
            // setIconProcessing($icon, false);
        }
    });
}




