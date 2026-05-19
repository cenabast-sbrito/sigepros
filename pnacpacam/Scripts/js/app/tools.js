
function logout() {
    try {
        $.ajax({
            type: "GET",
            url: apiUrl("Home/LogOut"),
            success: function (res) {
                window.location.href = apiUrl("");// obtenerRuta("");
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
            url: apiUrl("Config/getVersion"),
            success: function (res) {
                $('#version').text('ver.'+res);
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

/*****  Control de expiración de sesión Timers .FIN *****/
function obtenerRuta(relativePath) {
    let pathprefix = window.location.protocol + "//" + window.location.host + "/";
    if (window.location.hostname.includes("testaplicacionesweb")) {
        pathprefix += "/ProyectosTI/sigepros/";
    } else if (window.location.hostname.includes("aplicacionesweb")) {
        pathprefix += "/proyectosti/sigepros/";
    }
    return pathprefix + relativePath;
}
function btnON(idBtn, textBtn) {

    const myButton = document.getElementById(idBtn);
    myButton.disabled = false;
    myButton.style.opacity = 1;
    myButton.textContent = textBtn;

}
function btnOFF(idBtn, textBtn) {
    const myButton = document.getElementById(idBtn);
    myButton.disabled = true;
    myButton.style.opacity = 0.7;
    myButton.textContent = textBtn;

}
function anchorOFF(idBtn) {

    const myButton = document.getElementById(idBtn);
    myButton.disabled = true;
    myButton.style.opacity = 0.1;

}
function anchorON(idBtn) {

    const myButton = document.getElementById(idBtn);
    myButton.disabled = false;
    myButton.style.opacity = 1;

}

function toggleBtn(idBtn, textOn, textOff) {
    const btn = document.getElementById(idBtn);

    // si está deshabilitado → habilitar
    if (btn.disabled) {
        btn.disabled = false;
        btn.style.opacity = 1;
        if (textOn) btn.textContent = textOn;
    }
    // si está habilitado → deshabilitar
    else {
        btn.disabled = true;
        btn.style.opacity = 0.7;
        if (textOff) btn.textContent = textOff;
    }
}

function avisoFinProceso(aviso) {

    $('#mensajeSistema').text(aviso);
    $('#modalMensajeSistema').modal('show');

}
function setSize(objeto) {

    var height = $(window).height()-140;
    var width = $(window).width()-55;
    $('#' + objeto).height(height);
    $('#' + objeto).width(width);
    
}
function setSizeTwo(objeto, div) {

    var height = $(window).height() - 140;
    var width = ($(window).width())/div - 80;
    $('#' + objeto).height(height);
    $('#' + objeto).width(width);

}



function apiUrl(relativePath) {

    const base = (window.APP_CONFIG?.apiBaseUrl || "/")
        .replace(/\/+$/, "") + "/";
    const clean = relativePath.replace(/^\/+/, "");

    return new URL(clean, window.location.origin + base).toString();
}

/* Funciones Spinner */
function showLoading(selector) {
    $(selector).find('.loading-overlay').removeClass('d-none');
}

function hideLoading(selector) {
    $(selector).find('.loading-overlay').addClass('d-none');
}
/* Fin Funciones Spinner */





function btnPrueba(){

    fetch(apiUrl("Proveedor/getFacturas"))
        .then(console.log(r.json()));

}






function showToast(message, type = "info", duration = 4000) {
    const container = document.getElementById("toast-container-app");

    const toast = document.createElement("div");
    toast.className = `toast toast-${type}`;
    toast.innerHTML = message;

    container.appendChild(toast);

    // Activar animación
    setTimeout(() => toast.classList.add("show"), 50);

    // Auto eliminar
    setTimeout(() => {
        toast.classList.remove("show");
        setTimeout(() => toast.remove(), 300);
    }, duration);
}




//Helpers para el botón (desactivar/activar + accesibilidad)

function setProcessingButton(btn, processingText = "Consolidando…") {
    const $btn = $(btn);
    if ($btn.data("processing") === true) return; // evita doble activación

    $btn.data("processing", true);
    $btn.data("original-text", $btn.html()); // guarda HTML original

    // Variante con spinner custom
    const html = `<span class="spinner" aria-hidden="true"></span>${processingText}`;
    $btn.html(html);

    $btn.prop("disabled", true)
        .attr("aria-busy", "true")
        .attr("aria-live", "polite");
}

//Helpers para el botón (desactivar/activar + accesibilidad)
function unsetProcessingButton(btn) {
    const $btn = $(btn);
    const original = $btn.data("original-text") || "Consolidar";

    $btn.html(original);
    $btn.prop("disabled", false)
        .attr("aria-busy", "false")
        .removeAttr("aria-live")
        .data("processing", false);
}