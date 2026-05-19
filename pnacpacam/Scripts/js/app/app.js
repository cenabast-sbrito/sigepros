$(document).on('click', '#logout', function () {
    showToast("cerrando sesión", "info", 6000);
    window.location.hash = '';
    logout();
});
// SessionGuard v1.0 – Control robusto de inactividad + limpieza de timers
// Requiere: jQuery y (opcional) Bootstrap modal para #SesionExpiroNotif

(function (w, $) {
  if (!$) {
    console.error("SessionGuard: jQuery es requerido.");
    return;
  }

  const MS = 1000;
  const MIN =  60 * 1000; //const MIN = 60 * MS;    //solo para pruebas setear MIN a 1
    
  const SessionGuard = {
    _cfg: null,
    _intervalId: null,
    _countdownId: null,
    _countdownRem: 0,
    _activityBound: false,
    _lastActivity: Date.now(),
    _wrappedLogout: false,
    init(userCfg = {}) {
      // Defaults
      const defaults = {
        // Tiempo total de sesión (minutos). Ajusta según tu Web.config (JWT_EXPIRE_MINUTES).
        jwtExpireMinutes: w.APP_CONFIG?.JWT_EXPIRE_MINUTES || 90,

        // Minutos antes del vencimiento para mostrar advertencia
        warningMinutes: 2,

        // Segundos de cuenta regresiva en el modal de advertencia
        keepAliveSeconds: 30,

        // Selectores del UI
        selectors: {
          modal: '#SesionExpiroNotif',     // modal Bootstrap
          countdownText: '#tiemporestante',// span/elemento donde mostrar 00:SS o MM:SS
          keepAliveBtn: '#btnMantenerSesion',
          logoutBtn: '#logout'
        },

        // Callbacks (opcionales)
        onKeepAlive: null,   // p.ej. ping al backend
        onExpire: null,      // p.ej. redirigir a login
        onLogout: null       // llamado justo antes del logout real
      };

      this._cfg = $.extend(true, {}, defaults, userCfg);
      this._lastActivity = Date.now();

      // Limpieza previa si ya estaba corriendo
      this.stop();

      // Enlazar actividad
      this._bindActivity();

      // Enlazar botón "Mantener Sesión"
      this._bindKeepAlive();

      // Envolver logout() global si existe para asegurar limpieza
      this._wrapLogout();

      // Limpieza al salir
      w.addEventListener('beforeunload', this.stop.bind(this));

      // Iniciar loop de verificación (cada 1 segundo)
      this._intervalId = setInterval(this._tick.bind(this), 1000);

      // También limpiamos si se hace click en el botón de logout por selector
      const { logoutBtn } = this._cfg.selectors;
      if (logoutBtn) {
        $(document).off('click.SessionGuard', logoutBtn).on('click.SessionGuard', logoutBtn, () => {
          // Si tu app ya maneja logout por AJAX + redirect, igual limpiamos por seguridad
          this.stop();
        });
      }

      // Exponer publicamente por si necesitas tocar desde fuera
      w.SessionGuard = this;
      // console.info("SessionGuard iniciado", this._cfg);
    },

    stop() {
      if (this._intervalId) { clearInterval(this._intervalId); this._intervalId = null; }
      if (this._countdownId) { clearInterval(this._countdownId); this._countdownId = null; }
      this._unbindActivity();
      this._hideWarningModal();
    },

    touch() { // Permite resetear inactividad manualmente
      this._lastActivity = Date.now();
    },

    // ---- Internos ----
    _bindActivity() {
      if (this._activityBound) return;
      const reset = () => { this._lastActivity = Date.now(); };
      $(w).on('click.SessionGuard keyup.SessionGuard mousemove.SessionGuard', reset);
      this._activityBound = true;
    },

    _unbindActivity() {
      if (!this._activityBound) return;
      $(w).off('click.SessionGuard keyup.SessionGuard mousemove.SessionGuard');
      this._activityBound = false;
    },

    _bindKeepAlive() {
      const { keepAliveBtn } = this._cfg.selectors;
      if (!keepAliveBtn) return;

      $(document).off('click.SessionGuard', keepAliveBtn).on('click.SessionGuard', keepAliveBtn, () => {
        this._lastActivity = Date.now();
        this._hideWarningModal();
        this._stopCountdown();

        // Callback opcional para refrescar sesión en backend
        if (typeof this._cfg.onKeepAlive === 'function') {
          try { this._cfg.onKeepAlive(); } catch (e) { console.warn("onKeepAlive error:", e); }
        }
      });
    },

    _wrapLogout() {
      if (this._wrappedLogout) return;

      if (typeof w.logout === 'function') {
        const original = w.logout.bind(w);
        w.logout = (...args) => {
          // Limpieza ANTES de logout real
          this.stop();
          if (typeof this._cfg.onLogout === 'function') {
            try { this._cfg.onLogout(); } catch (e) { console.warn("onLogout error:", e); }
          }
          return original(...args);
        };
      }
      this._wrappedLogout = true;
    },

    _tick() {
      const now = Date.now();
      const inactiveMs = now - this._lastActivity;

      const sessionMaxMs = this._cfg.jwtExpireMinutes * MIN;
      const warningMs    = this._cfg.warningMinutes * MIN;

      // Mostrar advertencia cuando se ingresa al tramo final
      if (inactiveMs >= (sessionMaxMs - warningMs) && inactiveMs < sessionMaxMs) {
        this._showWarningModal();
        this._startCountdown();
      } else {
        // Si aún no entra a tramo de advertencia, aseguramos el modal oculto
        this._hideWarningModal();
        this._stopCountdown();
      }

      // Expirar sesión
      if (inactiveMs >= sessionMaxMs) {
        this._hideWarningModal();
        this.stop(); // Detener todo antes de salir

        if (typeof this._cfg.onExpire === 'function') {
          try { this._cfg.onExpire(); } catch (e) { console.warn("onExpire error:", e); }
        }

        // Si existe logout() global en tu app, llamarlo
        if (typeof w.logout === 'function') {
          w.logout();
        } else {
          // Fallback: recarga a raíz de la app (ajusta si necesitas)
          w.location.reload();
        }

        // (opcional) feedback visual
        try { alert("Su sesión ha expirado por inactividad."); } catch (e) {}
      }
    },

    _showWarningModal() {
      const { modal } = this._cfg.selectors;
      if (!modal) return;
      try { $(modal).modal('show'); } catch (e) { /* no Bootstrap */ }
    },

    _hideWarningModal() {
      const { modal } = this._cfg.selectors;
      if (!modal) return;
      try { $(modal).modal('hide'); } catch (e) { /* no Bootstrap */ }
    },

    _startCountdown() {
      if (this._countdownId) return; // ya corriendo

      this._countdownRem = this._cfg.keepAliveSeconds;
      this._renderCountdown();

      this._countdownId = setInterval(() => {
        this._countdownRem -= 1;
        this._renderCountdown();

        if (this._countdownRem <= 0) {
          this._stopCountdown();
          // Aquí podrías decidir forzar expiración inmediata si lo deseas.
          // this._lastActivity = Date.now() - (this._cfg.jwtExpireMinutes * MIN + 1);
        }
      }, 1000);
    },

    _stopCountdown() {
      if (!this._countdownId) return;
      clearInterval(this._countdownId);
      this._countdownId = null;
    },

    _renderCountdown() {
      const { countdownText } = this._cfg.selectors;
      if (!countdownText) return;
      const s = this._countdownRem;
      const mm = String(Math.floor(s / 60)).padStart(2, '0');
      const ss = String(s % 60).padStart(2, '0');
      $(countdownText).text(`${mm}:${ss}`);
    }
  };

  // Exponer en window
  w.SessionGuard = SessionGuard;

})(window, window.jQuery);

// ======== Inicialización plug-and-play ========
// Ajusta jwtExpireMinutes y warningMinutes a tu realidad.
// Si tienes un endpoint para refrescar sesión, inclúyelo en onKeepAlive.
$(function () {
  SessionGuard.init({
    jwtExpireMinutes: window.APP_CONFIG?.JWT_EXPIRE_MINUTES || 90,
    warningMinutes: 2,
    keepAliveSeconds: 30,
    selectors: {
      modal: '#SesionExpiroNotif',
      countdownText: '#tiemporestante',
      keepAliveBtn: '#btnMantenerSesion',
      logoutBtn: '#logout'
    },
    onKeepAlive: function () {
      // (Opcional) ping para mantener sesión del lado servidor:
      // $.get(apiUrl("Auth/KeepAlive"));
    },
    onLogout: function () {
      // (Opcional) logging/metricas
      // console.log("Limpiando recursos antes de logout…");
    },
    onExpire: function () {
      // (Opcional) acciones al expirar (tracking, etc.)
      // console.log("Sesión expirada por inactividad");
    }
  });
});

