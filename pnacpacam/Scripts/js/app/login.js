$(document).ready(function () {

    $("#username").Rut({
        format_on: 'keyup',
        validaRut: false    
    })

    $.validator.addMethod("rut", function (value, element) {
        return this.optional(element) || $.Rut.validar(value);
    }, "Este campo debe ser un rut válido.");

    const formLogin = $('#formLogin').validate({
        rules: {
            username: {
                required: true,
                rut: true
            },
            password: {
                required: true
            }
        },
        errorPlacement: function (error, element) {
            error.insertAfter(element);
        },
        submitHandler: function (e) {
            if (formLogin.valid()) {
                //var v = grecaptcha.getResponse(captchaLogin);
                //if (v.length == 0) {
                //    showMessageError("Debe completar el Captcha");
                //} else {
                //    isUser();
                //    grecaptcha.reset(captchaLogin);
                //}
                authenticate();
            }
        }
    });

});
//        static async Task < Product > GetProductAsync(string path)
// url: "/Index/Login",

function authenticate() {
    console.log("location.href    ",location.href    );
    console.log("location         ",location         );
    console.log("location.pathnam ",location.pathname);
    console.log("location.hostname",location.hostname);
    var formData = $('#formLogin').serialize();
    try {
        $.ajax({
            type: "POST",
            url: "Index/login",
            data: formData,
            contentType: 'application/x-www-form-urlencoded; charset=utf-8',
            success: function (response) {
                if (response.state) {
                    sessionStorage.setItem("Inventario_perfil", response.perfil);
                    if (response.perfil !== "3") {              // 3 - Perfil EUG
                        console.log(response);
                        //redireccionar("Home/#/#lista-inventarios");
                        redireccionar("Home/#/#admin-bienvenida");
                    }/*
                    else {
                        console.log(response)
                        redireccionar("/Home/#/#ingreso-resultados");
                    }*/
                } else {
                    showMessageError(response);
                }
            },
            error: function (ex) {
                showMessageError("Ha ocurrido un error." + ex.description);
            }
        });
    } catch (ex) {
        showMessageError(ex.description);
    }
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
    window.location = redirec;
}






