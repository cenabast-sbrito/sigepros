const segundos = 60;
const unidadesDeTiempo = 5;        // segun lo definido en variable JWT_EXPIRE_MINUTES  de Web.config
const tiempoEspera = 10;  //Tiempo de espera una vez que se ha detectado que la sesion está por vencer a causa de inactividad para que usuario presione el botón 'Mantener Sesión Activa'
var counter = tiempoEspera;
var tiempoSesionActiva = unidadesDeTiempo * segundos * 1000; // tiempo que durará la sesión activa sin actividad , transformacion unidadesDeTiempo a minutos
var idleTime = 0;
var countdown;

$(document).ready(function () {

    $(document).on('click', '#logout', function () {
        window.location.hash = '';
        logout();
    });

    /*window.onhashchange = function (evt) {
        this.currentHash();
    }*/

    $('.menu-principal a[data-toggle="tab"]').click(function () {
        window.location.hash = '/' + $(this).attr('href');
        currentHash();
    });
    currentHash();
    GetVersion();


    /*****  Control de expiración de sesión .INI *****/
    //Detecta actividad
    $(window).click(function () {
 //       console.log("click realizado");
        idleTime = 0;
    })
    $(window).keyup(function () {
//        console.log("tecla presionada")
        idleTime = 0;
    })
    $(window).mousemove(function () {
 //       console.log("movimiento mouse")
        idleTime = 0;
    })
    //Incrementa contador idletime cada minuti.

    var idleInterval = setInterval(timerIncrement, tiempoSesionActiva);

    $('#btnMantenerSesion').click(function () {
        idleTime = 0;
        $('#SesionExpiroNotif').modal('hide');
        counter = tiempoEspera;
        clearInterval(countdown);
    });
    /*****  Control de expiración de sesión .FIN *****/
   
});

function currentHash() {

    var currentHash = location.hash.split('/');
    $('.menu-principal a[data-toggle="tab"]').each(function (i, element) {
        if ($(this).attr("href") == currentHash[1]) {
            $(this).tab('show')
        }
    });

}

function logout() {
    try {
        $.ajax({
            type: "GET",
            url: "../Home/LogOut",
            success: function (res) {
                window.location.href = '..';
//                window.location.href = '../';       //login
            },
            error: function (jqXHR, textStatus, errorThrown) {
                console.error("Ha ocurrido un error : " + jqXHR.responseText);
            }
        });
    } catch (ex) {
        //$("#area").html("Error : " + ex);
        console.error("Ha ocurrido un error : " + ex.name + " " + ex.message);
    }
}

function formatDate(fecha) {

    const re = /-?\d+/;
    const m = re.exec(fecha);
    var date = parseInt(m[0], 10);
    date = new Date(date);
    return ("00" + date.getDate()).slice(-2) + "-" +
        ("00" + (date.getMonth() + 1)).slice(-2) + "-" +
        date.getFullYear();

}

function formatLoadDate(fecha) {

    const re = /-?\d+/;
    const m = re.exec(fecha);
    var date = parseInt(m[0], 10);
    date = new Date(date);
    return date.getFullYear() + "-" + ("00" + (date.getMonth() + 1)).slice(-2) + "-" + ("00" + date.getDate()).slice(-2);

}

function fromatDateTime(fecha) {
    const re = /-?\d+/;
    const m = re.exec(fecha);
    var date = parseInt(m[0], 10);
    date = new Date(date);
    return ("00" + date.getDate()).slice(-2) + "-" +
        ("00" + (date.getMonth() + 1)).slice(-2) + "-" +
        date.getFullYear() + " " +
        ("00" + date.getHours()).slice(-2) + ":" +
        ("00" + date.getMinutes()).slice(-2) + ":" +
        ("00" + date.getSeconds()).slice(-2);
}

function getYearNext() {
    const fecha = new Date();
    const year = fecha.getFullYear() + 1;

    return year;
}

function GetVersion() {
    try {
        $.ajax({
            type: "GET",
            url: "../Config/getVersion",
            success: function (res) {
                $('#version').text(res);
            },
            error: function (jqXHR, textStatus, errorThrown) {
                //        console.error("Ha ocurrido un error : " + ex.name + " " + ex.message);
                console.error("Ha ocurrido un error 3: ");
            }
        });
    } catch (ex) {
        //        console.error("Ha ocurrido un error : " + ex.name + " " + ex.message);
        console.error("Ha ocurrido un error 3: ");
    }
}


function darFormato(valor, decimales) {
    resultado = number_format(valor, decimales, ",", ".");
    return resultado; //.toLocaleString('fullwide');
}

function number_format(number, decimals, dec_point, thousands_sep) {
    // Strip all characters but numerical ones.
    number = (number + '').replace(/[^0-9+\-Ee.]/g, '');
    var n = !isFinite(+number) ? 0 : +number,
        prec = !isFinite(+decimals) ? 0 : Math.abs(decimals),
        sep = (typeof thousands_sep === 'undefined') ? ',' : thousands_sep,
        dec = (typeof dec_point === 'undefined') ? '.' : dec_point,
        s = '',
        toFixedFix = function (n, prec) {
            var k = Math.pow(10, prec);
            return '' + Math.round(n * k) / k;
        };
    // Fix for IE parseFloat(0.55).toFixed(0) = 0;
    s = (prec ? toFixedFix(n, prec) : '' + Math.round(n)).split('.');
    if (s[0].length > 3) {
        s[0] = s[0].replace(/\B(?=(?:\d{3})+(?!\d))/g, sep);
    }
    if ((s[1] || '').length < prec) {
        s[1] = s[1] || '';
        s[1] += new Array(prec - s[1].length + 1).join('0');
    }
    return s.join(dec);
}

function formatTime(segundos) {
    return [
        parseInt(segundos / 60 % 60),
        parseInt(segundos % 60)
    ]
        .join(":")
        .replace(/\b(\d)\b/g, "0$1")
};


/*****  Control de Timers expiración de sesión .INI *****/
function timerIncrement() {
    const tiempoNotificacion = 1;  //minutos unidadesDeTiempo
    const tiempoRedireccion = 2;   //minutos 

    idleTime = idleTime + 1;
    if (idleTime > tiempoNotificacion) {
        $('#SesionExpiroNotif').modal('show');
        startTimer();
    }
    if (idleTime > tiempoRedireccion) {

        $('#SesionExpiroNotif').modal('hide');
        logout();
        alert("Su sesión ha expirado por inactividad.");
    }
};


function startTimer() {
    countdown = setInterval(countDownClock, 1000);
};


function countDownClock() {
    counter = counter - 1
    if (counter < 10) {
        $('#tiemporestante').text("0" + formatTime(counter));
    }
    else {
        $('#tiemporestante').text(formatTime(counter));
    }
    if (counter == 0) {
        counter = tiempoEspera;
        clearInterval(countdown);
    }
};

/*****  Control de expiración de sesión Timers .FIN *****/