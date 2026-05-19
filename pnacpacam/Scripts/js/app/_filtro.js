$(document).ready(function () {


    $('.select2').select2({
        placeholder: "Seleccione",
        allowClear: true,
        width: '100%',
        minimumResultsForSearch: 5, // oculta buscador si hay pocas opciones
        language: {
            noResults: () => "No se encontraron facturas"
        }
    });


    $.validator.addMethod("rutNumerico", function (value, element) {
        if (this.optional(element)) return true;
        return /^\d{1,8}$/.test(String(value).trim());
    }, "El RUT debe contener solo números (sin puntos ni guion) y hasta 8 dígitos.");
    $.validator.addMethod("facturaNumerica", function (value, element) {
        if (this.optional(element)) return true;
        return /^\d{1,8}$/.test(String(value).trim());
    }, "La Factura debe contener solo números (hasta 8 dígitos).");
    $.validator.addMethod("anioValido", function (value, element) {
        if (this.optional(element)) return true;
        const v = String(value).trim();
        if (!/^\d{4}$/.test(v)) return false;
        const anio = parseInt(v, 10);
        const currentYear = new Date().getFullYear(); 
        return anio >= 1900 && anio <= 2060;
    }, "Ingrese un año válido (formato AAAA, desde 1900).");


    $('#form-filtro').validate({

        ignore: [], // IMPORTANTE para Select2

        rules: {
            _rutProveedor: {
                maxlength: 8,
                rutNumerico: true
            },
            _nfactura: {
                facturaNumerica: true
            },
            _anio: {
                anioValido : true
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