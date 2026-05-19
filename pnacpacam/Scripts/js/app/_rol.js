function _getListRoles(listaDestino, organismo, rut) {

    try {
        $.ajax({
            type: "GET",
            url: apiUrl("Rol/getRoles"),
            contentType: 'application/json;  charset=utf-8',
            data: {organismo: organismo, rut: rut},	
            success: function (res) {
                let option = "<option value=''>Seleccione un Rol</option>";
                for (var element of res) {
                    option = option +
                        '<option value="' + element.CRol + '">' +
                        element.TRol +
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


function getTableroRolesUsuario(organismo, rut) {
    _getListRoles('listaRoles',organismo, rut); 
    let json = {
        rut: $('#rut').val()
    };
    try {
        $('#tableroRolesDelUsuario').DataTable({
            "ajax": {
                url: apiUrl("Rol/GetRolesUsuario"),
                data: json,
                dataType: "json",
                dataSrc: function (json) {
                    return json.status ? json.res : [];
                },
            },
            "columns": [
                { "data": "CRol" },
                { "data": "TRol" }
            ],
            destroy: true,
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
            }
        });

    } catch (ex) {
        console.error(ex);
        alert('Ocurrió un error al inicializar la tabla.');
    }
}
function delTableroRolesUsuario(organismo) {
    let json = {
        rut: $('#rut').val(),
        organismo: organismo
    }
    try {
        $.ajax({
            type: "GET",
            url: '../Rol/DelRolesUsuario',
            contentType: 'application/json;  charset=utf-8',
            data: json,
            success: function (data) {

                if (data.res == "OK") {
                    avisoFinProceso("Limpieza exitosa, Usuario ha quedado sin rol");
                }
                else
                    avisoFinProceso(data.res);

                GetUsuario(rut);
                getTableroUsuario();


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
        console.log(ex.description);
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


function setRolUsuario(rut, rolUsuario) {
    let json = {
        rut: rut,
        rolUsuario: rolUsuario
    }
    try {
        $.ajax({
            type: "GET",
            url: '../Rol/setRolUsuario',
            contentType: 'application/json;  charset=utf-8',
            data: json,
            success: function (data) {

                if (data == "")
                    avisoFinProceso("Rol Asignado");
                else 
                    avisoFinProceso(data);

                GetUsuario(rut);
                getTableroUsuario();
                getTableroRolesUsuario($('#organismo').val());

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
        console.log(ex.description);
    }
}