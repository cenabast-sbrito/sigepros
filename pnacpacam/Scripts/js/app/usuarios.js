$(document).ready(function () {
    getTableroUsuario();
    
    var formUsuario = $('#formUsuario').validate({
        errorPlacement: function (error, element) {
            error.insertAfter(element);
        },
        submitHandler: function (form) {
            if (formUsuario.valid) {
                if ($('#idUsuario').val() != "") {
                    putUsuario();
                } else {
                    setUsuario();
                }
            }
        }
    });
    $("#rutInsumo").Rut({
        format_on: 'keyup'
    })
    $(document).on('click', '#btnNuevoUsuario', function () {
        $('#formUsuario')[0].reset();
        $('#idUsuario').val('');
        $('#rut').val('');
    });
    $(document).on('click', '#btnModificarUsuario', function () {
        $('#idUsuario').val('');
        $('#rut').val('');
        var rut = $(this).parents('tr').find('td:eq(1)').html();
        GetUsuario(rut);
    });
    $(document).on("keyup", "#nombre", function (e) {
        var val = $(this).val();
        if (val != "") {
            GetName(val);
        } else {
            $("#sugerencia-nombre").html("");
        }
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
            }
        });
    } catch (ex) {
        //alert(ex.description);
    }
}
function setUsuario() {
    //var form = $('#formUsuario');
    var json = {
        Rut: $('#rut').val(),
        Estado: "true",
        IdPerfil: $('#perfil').val()
    };
    try {
        $.ajax({
            url: '../Usuario/SetUsuario',
            contentType: 'application/json; charset=utf-8',
            data: json,
            dataType: "json",
            "dataSrc": function (json) {
                if (json.length > 0) {
                    $('#tableroUsuario').DataTable().ajax.reload(null, false);
                    $('#nuevoUsuario').modal('hide');
                    $('#modalMensajeSistema').modal('show');
                    $('#mensajeSistema').text(res.message);
                } else {
                    return [];
                }
            },
            error: function (error) {
                console.log(error.description);
            }
        });
    } catch (ex) {
        alert(ex.description);
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
                $('#idUsuario').val(res.Id);
                $('#nombre').val(res.Nombre);
                $('#rut').val(res.Rut);
                $('#perfil').val(res.IdPerfil.toString());
                $('#estado').val(res.Estado === true ? 1 : 0);
            },
            error: function (error) {
                console.log(error.description);
            }
        });
    } catch (ex) {
        alert(ex.description);
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
//                $('#id_FuncionarioInsumo').val(res.Id_funcionario);
//                $('#rutInsumo').val(res.Rut);
                $('#CodigoDpto').val(res.IdUnidadGestora, GetResponsableUnidadGestora(res.IdUnidadGestora))

            },
            error: function (error) {
                console.log(error.description);
            }
        });
    } catch (ex) {
        alert(ex.description);
    }
}


function putUsuario() {
    var json = {
        "IdUsuario": $('#idUsuario').val(),
        "Rut": $('#rut').val(),
        "IdPerfil": $('#perfil').val(),
        "Estado": $('#estado').val() == "1" ? true : false
    };

    try {
        $.ajax({
            type: 'put',
            url: '../Usuario/PutUsuario',
            contentType: 'application/json;  charset=utf-8',
            data: json,
            success: function (res) {
                $('#nuevoUsuario').modal('hide');
                $('#modalMensajeSistema').modal('show');
                $('#mensajeSistema').text(res.message);
                $('#tableroUsuario').DataTable().ajax.reload(null, false);
            },
            error: function (error) {
                console.log(error.description);
            }
        });
    } catch (ex) {
        alert(ex.description);
    }

}
function putFuncionario() {
    //console.log($('FUNCIONARIO --->','#id_FuncionarioInsumo').val());
    var data = {
//        "IdUsuario": $('#idUsuario').val(),
//        "Rut": $('#rut').val(),
        "IdPerfil": $('#perfil').val(),
//        "Estado": $('#estado').val() == "1" ? true : false,
        "Estado": false, //inicialmente se crea al funcionario en insumos sin nungun tipo de rol y desactivado
//        "Id_Funcionario": $('#id_FuncionarioInsumo').val(),
        "Rut": $('#rutInsumo').val().split('-')[0],
//        "Password": $('#passwordInsumo').val(),
        "Nombre": $('#nombreInsumo').val(),
        "Apellido": $('#apellidoInsumo').val(),
//        "Anexo": $('#anexoInsumo').val(),
        "Email": $('#emailInsumo').val()
//        "NombreUsuario": $('#nombreUsuarioInsumo').val(),
//        "CodigoDpto": $('#CodigoDpto').val(),
//        "CodigoSubDpto": $('#CodigoSubDpto').val(),
//        "CodigoUnidad": $('#CodigoUnidad').val()
    };

    try {
        $.ajax({
            type: 'put',
            url: '../Usuario/PutFuncionario',
            contentType: 'application/json;  charset=utf-8',
            data: JSON.stringify(data),
            success: function (res) {
                /*SBRITO CORREGIR*/
                $('#nuevoUsuario').modal('hide');
                $('#modalMensajeSistema').modal('show');
                $('#mensajeSistema').text(res.message);
                $('#tableroUsuario').DataTable().ajax.reload(null, false);
                /*SBRITO CORREGIR*/

            },
            error: function (error) {
                console.log(error.description);
            }
        });
    } catch (ex) {
        alert(ex.description);
    }

}

function getTableroUsuario() {
    //RECORDAR PRIMERO AGREGAR LA CABECEERA DE LA COLUMNA Y LUEGO Y LUEGO LA COLUMNA
    try {
        $('#tableroUsuario').DataTable({
            "ajax": {
                url: '../Usuario/Index',
                dataSrc: ''
            },
            "columns": [
                { "data": "Id" },
                { "data": "Rut" },
                { "data": "Nombre" },
                { "data": "Email" },
                { "data": "NombrePerfil" },
                {
                    "data": null,
                    "render": function (data, type, row) {
                        var label = data["Estado"] == true ? "<span class='label label-success' style='font-size:1em;'>Activo</span>" : "<span class='label label-danger' style='font-size:1.1em;'>Inactivo</span>"
                        return label;
                    }
                },
                {
                    "defaultContent": "<button class='btn btn-info btn-sm' data-toggle='modal' data-target='#nuevoUsuario' id='btnModificarUsuario'>Modificar</button>"
                }
            ],
            "language": {
                "lengthMenu": "Mostrar _MENU_ registros por página",
                "zeroRecords": "No se encontraron registros",
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
            "dom": "<'row '<'toolbar'><'col-sm-5'l><'col-sm-4'f>>" +
                "<'row'<'col-sm-12'tr>>" +
                "<'row'<'col-sm-6'i><'col-sm-6'p>>"

        });
        $('div.toolbar').html("<div class='col-sm-3'><button class='btn btn-principal btn-sm' style='margin-right:3px;' data-toggle='modal' data-target='#nuevoUsuario' id='btnNuevoUsuario'>Nuevo Usuario</button><button class='btn btn-principal btn-sm' data-toggle='modal' data-target='#nuevoFuncionario' id='btnNuevoFuncionario'>Nuevo Funcionario</button></div>");



    } catch (ex) {
        alert(ex.description);
    }
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




