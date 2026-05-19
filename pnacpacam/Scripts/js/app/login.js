//$(document).ready(function () {

//    $("#rutusername").Rut({
//        format_on: 'keyup',
//        validaRut: false
//    })

//    $.validator.addMethod("rut", function (value, element) {
//        return this.optional(element) || $.Rut.validar(value);
//    }, "Este campo debe ser un rut válido.");

//    const formLogin = $('#formLogin').validate({
//        rules: {
//            rutusername: {
//                required: true,
//                rut: true
//            },
//            password: {
//                required: true
//            }
//        },
//        errorPlacement: function (error, element) {
//            error.insertAfter(element);
//        },
//        submitHandler: function (e) {
//            if (formLogin.valid()) {
//                login(formLogin);
//                //authenticate();
//            }
//        }
//    });

//});







$(document).ready(function () {

    // 1) Formateo de RUT en vivo (si tu plugin lo soporta)
    $("#rutusername").Rut({
        format_on: 'keyup',
        validaRut: false // dejamos validación a jQuery Validate
    });

    // 2) Método custom de validación RUT
    // Verifica que $.Rut.validar exista; si no, ajusta a tu plugin real.
    $.validator.addMethod("rut", function (value, element) {
        const v = (value || "").trim();
        if (this.optional(element)) return true;
        if (v === "") return false;

        // Normaliza para validar (muchos validadores requieren formato sin puntos)
        // Si tu plugin acepta con puntos/guión, puedes omitir esta línea.
        const compact = v.replace(/\./g, "").replace(/-/g, "");

        // IMPORTANTE: asegúrate de que este método existe en tu plugin
        if (typeof $.Rut === "object" && typeof $.Rut.validar === "function") {
            return $.Rut.validar(compact);
        }

        // Si tu plugin usa otra API, cámbialo aquí:
        // return $.rut.validar(compact); // ejemplo alternativo

        // Si no hay validador disponible, por seguridad marca como no válido:
        return false;
    }, "Este campo debe ser un RUT válido.");

    // 3) Inicializa el validador del formulario
    const validator = $('#formLogin').validate({
        // Importante: las reglas van por "name"
        rules: {
            rutusername: {
                required: true,
                rut: true
            },
            password: {
                required: true
            }
        },
        messages: {
            rutusername: {
                required: "Ingresa tu RUT."
            },
            password: {
                required: "Ingresa tu contraseña."
            }
        },
        // Evita que ignore campos ocultos si usas select2 u otros
        ignore: [],
        // Opcional: normaliza espacios
        normalizer: function (value) {
            return $.trim(value);
        },
        errorPlacement: function (error, element) {
            error.insertAfter(element);
        },
        // Esto se ejecuta SOLO si el form ya está válido
        submitHandler: function (form, event) {
            event.preventDefault(); // evita submit nativo

            // Deshabilita botón para evitar doble submit
            $("#btnLogin").prop("disabled", true);

            // Construye formData como lo espera tu login()
            const formData = $(form).serialize(); // o un objeto:
            // const formData = {
            //   rutusername: $(form).find("[name='rutusername']").val(),
            //   password: $(form).find("[name='password']").val()
            // };

            // Llama a tu lógica AJAX
            login(formData);

            // Rehabilita el botón cuando termine dentro de login() en complete()
            // o bien aquí si usas Promesas.
        }
    });

});







function login(formData) {
    // Muestra al usuario que se está intentando conectar
    showMessageInfo("Conectando con el servidor...");

    // Si el navegador reporta estar offline, avisamos de inmediato
    if (typeof navigator !== "undefined" && navigator && navigator.onLine === false) {
        showMessageError("Sin conexión a Internet. Verifica tu red e inténtalo nuevamente.");
        //alert("");
        return;
    }

    $.ajax({
        type: "POST",
        url: "Index/Login",
        data: formData,
        dataType: "json",
        timeout: 15000, // 15s: evita que el usuario quede esperando indefinidamente
        // beforeSend: opcional para deshabilitar botón de login
        beforeSend: function () {
            // disableButton("#btnLogin");
        },
        success: function (response) {
            if (!response || response.state === false) {
                const msg = (response && response.message)
                    ? response.message
                    : "No fue posible autenticar. Inténtalo nuevamente.";
                showMessageError(msg);
                ////alert("");
                return;
            }

            // SOLO para UI (opcional)
            sessionStorage.setItem("_pnacpacam_Rol", response.Rol);
            sessionStorage.setItem("_pnacpacam_Rut", response.Rut);

            // El backend decide qué puede ver el usuario logueado
            window.location.href = obtenerRutaLogin("Home/Index");
        },
        error: function (jqXHR, textStatus, errorThrown) {
            // textStatus típicos: "timeout", "error", "abort", "parsererror"
            // jqXHR.status típicos: 0, 400, 401, 403, 404, 408, 422, 429, 500, 502, 503, 504
            let userMessage = "Ha ocurrido un error al autenticar.";

            if (textStatus === "timeout") {
                userMessage = "Tiempo de espera agotado. El servidor no responde.";
            } else if (jqXHR.status === 0) {
                // status 0 suele indicar que no hubo respuesta (servidor caído, CORS, o red caída)
                if (typeof navigator !== "undefined" && navigator && navigator.onLine === false) {
                    userMessage = "Estás sin conexión. Verifica tu red e inténtalo nuevamente.";
                } else {
                    userMessage = "No se puede contactar al servidor. Es posible que esté caído o no disponible.";
                }
            } else if (jqXHR.status === 401) {
                userMessage = "No autorizado. Verifica tus credenciales.";
            } else if (jqXHR.status === 403) {
                userMessage = "Acceso denegado. No tienes permisos para ingresar.";
            } else if (jqXHR.status === 404) {
                userMessage = "No se encuentra el servicio de autenticación (404).";
            } else if (jqXHR.status === 408) {
                userMessage = "Tiempo de espera de la solicitud (408).";
            } else if (jqXHR.status === 429) {
                userMessage = "Demasiados intentos. Espera un momento e inténtalo de nuevo.";
            } else if (jqXHR.status === 500) {
                userMessage = "Error interno del servidor (500).";
            } else if (jqXHR.status === 502) {
                userMessage = "Puerta de enlace inválida (502).";
            } else if (jqXHR.status === 503) {
                userMessage = "Servicio no disponible (503). El servidor podría estar en mantenimiento o caído.";
            } else if (jqXHR.status === 504) {
                userMessage = "Tiempo de espera en la puerta de enlace (504).";
            } else if (textStatus === "parsererror") {
                userMessage = "La respuesta del servidor no es válida (parser error).";
            } else if (errorThrown) {
                userMessage = "Error: " + errorThrown;
            }

            showMessageError(userMessage);
            //alert("");
            // Opcional: logging más detallado para consola/desarrollo
            // console.error("Login AJAX error =>", { status: jqXHR.status, textStatus, errorThrown, responseText: jqXHR.responseText });
        },
        complete: function () {
            // enableButton("#btnLogin");
            //alert("");
        }
    });
}

function authenticateOLD() {
    //console.log("authenticate Index/Login");
    var formData = $('#formLogin').serialize();

    $.ajax({
        type: "POST",
        url: "Index/Login",
        data: formData,
        success: function (response) {

            if (!response.state) {
                showMessageError(response.message);
                return;
            }

            // SOLO para UI (opcional)
            sessionStorage.setItem("_pnacpacam_Rol", response.Rol);
            sessionStorage.setItem("_pnacpacam_Rut", response.Rut);

            // El backend decide qué puede ver el usuario logueado
            window.location.href = obtenerRutaLogin("Home/Index");

        },
        error: function () {
            showMessageError("Ha ocurrido un error al autenticar.");
        }
    });
}


function replaceAll(text, busca, reemplaza) {
    while (text.toString().indexOf(busca) != -1)
        text = text.toString().replace(busca, reemplaza);
    return text;
}

function showMessageError(message) {
    $("#message").html("").show();
    $("#message").html("<div class='col-md-12  role='alert'>" +
                            "<div class='alert alert-danger alert-dismissible' >" +
                                "<button type='button' class='close' data-dismiss='alert'><span aria-hidden='true'>&times;</span><span class='sr-only'>Close</span></button>" +
                                "<strong>Error!</strong> " + message + "" +
                            "</div>" +
                        "</div>");
}
function redireccionar(redirec) {
    window.location.href = redirec;
}

function obtenerRutaLogin(relativePath) {
    return relativePath;
    let pathprefix = window.location.protocol + "//" + window.location.host + "/";
    if (window.location.hostname.includes("testaplicacionesweb")) {
        pathprefix += "/proyectosti/sigepros/";
    } else if (window.location.hostname.includes("aplicacionesweb")) {
        pathprefix += "/proyectosti/sigepros/";
    }
    return pathprefix + relativePath;
}
