using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace pnacpacam.Models
{
    public static class SqlExceptionHandler
    {
        // Diccionario de errores controlados por SP / BD
        private static readonly Dictionary<int, string> _sqlErrors = new Dictionary<int, string>
        {
            // Duplicados
            { 2601,  "El usuario ya posee ese rol (duplicado)." },
            { 2627,  "El usuario ya posee ese rol (duplicado)." },

            // Errores de negocio definidos en SQL (RAISERROR 50000+)
            { 50100, "El Rut es obligatorio." },
            { 50101, "El código de rol es obligatorio." },
            { 50102, "El rol indicado no existe." },
            { 50103, "El usuario ya posee ese rol." },
            { 50104, "No se permite mezclar organismos." },
            { 50105, "No puede asignar Admin CENABAST si tiene Gestor CENABAST." },
            { 50106, "No puede asignar Gestor CENABAST si tiene Admin CENABAST." }
        };

        /// <summary>
        /// Traduce una SqlException a un mensaje de negocio entendible.
        /// </summary>
        public static string GetMessage(SqlException ex)
        {
            if (ex == null)
                return "Error SQL desconocido.";

            if (_sqlErrors.TryGetValue(ex.Number, out string mensaje))
                return mensaje;

            // Error SQL no controlado explícitamente
            return $"Error SQL ({ex.Number}).";
        }

        /// <summary>
        /// Maneja cualquier excepción genérica.
        /// </summary>
        public static string GetMessage(Exception ex)
        {
            return "Error inesperado.";
        }
    }
}