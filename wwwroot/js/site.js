toastr.options = {
    "closeButton": true,
    "debug": false,
    "newestOnTop": true,
    "progressBar": true,
    "positionClass": "toast-top-right",
    "preventDuplicates": false,
    "onclick": null,
    "showDuration": "300",
    "hideDuration": "1000",
    "timeOut": "3000",
    "extendedTimeOut": "1000",
    "showEasing": "swing",
    "hideEasing": "linear",
    "showMethod": "fadeIn",
    "hideMethod": "fadeOut"
};


$(function () {
    if ($.validator) {
        $.validator.methods.number = function (value, element) {
            return this.optional(element) || /^-?(?:\d+|\d{1,3}(?:\.\d{3})*)?(?:,\d+)?$/.test(value);
        };

        $.validator.methods.range = function (value, element, param) {
            var globalizedValue = value.replace(",", ".");
            return this.optional(element) || (globalizedValue >= param[0] && globalizedValue <= param[1]);
        };
    }

    if ($('#EhParcelado').length && $('#campoNumeroParcelas').length) {
        function toggleParcelas() {
            if ($('#EhParcelado').is(':checked')) {
                $('#campoNumeroParcelas').show();
                $('#ajudaValor').text('Insira o valor DA PARCELA.');
            } else {
                $('#campoNumeroParcelas').hide();
                $('#NumeroDeParcelas').val(1);
                $('#ajudaValor').text('Insira o valor total. Se for parcelado, insira o valor da parcela.');
            }
        }
        toggleParcelas();
        $('#EhParcelado').on('change', toggleParcelas);
    }

    if ($('#ehParceladoSimulacao').length) {
        function toggleParcelasSimulacao() {
            if ($('#ehParceladoSimulacao').is(':checked')) {
                $('#campoNumeroParcelasSimulacao').show();
                $('#labelDataSimulacao').text('Data da 1ª Parcela');
            } else {
                $('#campoNumeroParcelasSimulacao').hide();
                $('#numeroParcelas').val(1);
                $('#labelDataSimulacao').text('Data');
            }
        }
        toggleParcelasSimulacao();
        $('#ehParceladoSimulacao').on('change', toggleParcelasSimulacao);
    }
});


document.addEventListener('DOMContentLoaded', function () {
    const themeToggleBtn = document.getElementById('theme-toggle');
    const themeToggleDarkIcon = document.getElementById('theme-toggle-dark-icon');
    const themeToggleLightIcon = document.getElementById('theme-toggle-light-icon');

    if (themeToggleBtn && themeToggleDarkIcon && themeToggleLightIcon) {

        function setIconVisibility() {
            if (localStorage.getItem('color-theme') === 'dark' || (!('color-theme' in localStorage) && window.matchMedia('(prefers-color-scheme: dark)').matches)) {
                themeToggleLightIcon.classList.remove('hidden');
                themeToggleDarkIcon.classList.add('hidden');
            } else {
                themeToggleDarkIcon.classList.remove('hidden');
                themeToggleLightIcon.classList.add('hidden');
            }
        }

        setIconVisibility();

        themeToggleBtn.addEventListener('click', function () {
            document.documentElement.classList.toggle('dark');

            if (localStorage.getItem('color-theme')) {
                if (localStorage.getItem('color-theme') === 'light') {
                    localStorage.setItem('color-theme', 'dark');
                } else {
                    localStorage.setItem('color-theme', 'light');
                }
            } else {
                if (document.documentElement.classList.contains('dark')) {
                    localStorage.setItem('color-theme', 'dark');
                } else {
                    localStorage.setItem('color-theme', 'light');
                }
            }

            setIconVisibility();
        });
    }
});