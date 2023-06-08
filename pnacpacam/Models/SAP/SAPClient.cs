using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SAP.Middleware.Connector;

namespace WebApplicationTestConnector
{
    class SAPClient
    {

        //const string CONFIG_NAME = "connectiontest";
        RfcDestination destino;
        private SAPConfig config;

        public SAPClient()
        {
            //destino = ConectarSap("QAS");
        }

        public void ConectarSap(string destinationName)
        {
            /*try
            {
                destino = null;
                destino = RfcDestinationManager.GetDestination(destinationName);
            }
            catch
            {
                Console.WriteLine("Environment Not Registered", string.Format("SAP environment not registered for conn : '{0}'", destinationName));
            }*/

            if (destino == null)
            {
                try
                {
                    config = new SAPConfig();
                    RfcDestinationManager.RegisterDestinationConfiguration(config);
                    destino = RfcDestinationManager.GetDestination(destinationName);
                    Console.WriteLine("SAP Conectado.");
                    //Console.WriteLine("SAP Environment Registered", string.Format("SAP environment registered for conn : '{0}'", destinationName));
                }
                catch (RfcCommunicationException e)
                {
                    // network problem...
                    Console.WriteLine("Error de Comunicacion : " + e.Message );
                }
                catch (RfcLogonException e)
                {
                    // user could not logon...
                    Console.WriteLine("Error Logon : " + e.Message);
                }
                catch (RfcAbapRuntimeException e)
                {
                    // serious problem on ABAP system side...
                    Console.WriteLine("Error de Run Time : " + e.Message);
                }
                catch (RfcAbapBaseException e)
                {
                    // The function module returned an ABAP exception, an ABAP message
                    // or an ABAP class-based exception...
                    Console.WriteLine("Error Abap Base  : " + e.Message);
                }
            }

           // return destino;
        }

        public void DesconectarSap()
        {
            /*try
            {
                destino = null;
                destino = RfcDestinationManager.GetDestination(destinationName);
            }
            catch
            {
                Console.WriteLine("Environment Not Registered", string.Format("SAP environment not registered for conn : '{0}'", destinationName));
            }*/

            if (destino != null)
            {
                try
                {
                    RfcSessionManager.EndContext(destino);
                    Console.WriteLine("SAP Desconectado.");
                }
                catch (RfcCommunicationException e)
                {
                    // network problem...
                    Console.WriteLine("Error de Comunicacion : " + e.Message);
                }
                catch (RfcLogonException e)
                {
                    // user could not logon...
                    Console.WriteLine("Error Logon : " + e.Message);
                }
                catch (RfcAbapRuntimeException e)
                {
                    // serious problem on ABAP system side...
                    Console.WriteLine("Error de Run Time : " + e.Message);
                }
                catch (RfcAbapBaseException e)
                {
                    // The function module returned an ABAP exception, an ABAP message
                    // or an ABAP class-based exception...
                    Console.WriteLine("Error Abap Base  : " + e.Message);
                }
            }

            //return destino;
        }

        public static Type GetDataType(RfcDataType rfcDataType)
        {
            switch (rfcDataType)
            {
                case RfcDataType.DATE:
                    return typeof(string);
                case RfcDataType.CHAR:
                    return typeof(string);
                case RfcDataType.STRING:
                    return typeof(string);
                case RfcDataType.BCD:
                    return typeof(decimal);
                case RfcDataType.INT2:
                    return typeof(int);
                case RfcDataType.INT4:
                    return typeof(int);
                case RfcDataType.FLOAT:
                    return typeof(double);
                default:
                    return typeof(string);
            }
        }

        public string validarDocCenabast(string docCenabast)
        {
            if (docCenabast.Length == 9) {
                docCenabast = "0" + docCenabast;
            }

            return docCenabast;
        }

        public string ingresoInformacionDespacho(string Doc_Cenabast, string Factura, string Guia, DateTime FechaMenor, int Cantidad, string O_Trans, string Lote, string observacion, DateTime FechaRecibido, int descMovimiento)
        {
            //RfcDestinationManager.RegisterDestinationConfiguration(new SAP());
            //RfcDestination destino = RfcDestinationManager.GetDestination("QAS");
            string codigoRetorno = null;
            try
            {
                RfcRepository repo = destino.Repository;
                IRfcFunction despacho = repo.CreateFunction("ZSD_INTERFAZ_CARGA_DESP");
                
                //despacho.GetStructure("ZSD_DESP_PARAMS");
                IRfcTable tiparams = despacho.GetTable("TIPARAMS");
                tiparams.Insert();
                tiparams.CurrentIndex = tiparams.Count - 1;
                //VBELN es un variable tipo char(10) por lo que se debe validar si tiene el largo de no serlo se le agrega un 0 adelante
                Doc_Cenabast = validarDocCenabast(Doc_Cenabast);
                tiparams[0].SetValue("VBELN", Doc_Cenabast);
                //tiparams[0].SetValue("VBELN", "0304408710");
                tiparams[0].SetValue("VBELN_VF", Factura);
                //tiparams[0].SetValue("VBELN_VF", "123987");
                tiparams[0].SetValue("GUIA_DESPACHO", Guia);
                //tiparams[0].SetValue("GUIA_DESPACHO", "0");
                //las Fechas para insertarlas en sap deben ser datatime en el formato dd-mm-yyyy
                //LA FECH_ENT_OP ES LA FECHA MENOR ENTRE LA FECHA GUIA Y FACTURA 
                tiparams[0].SetValue("FECH_ENT_OP", FechaMenor);
                //tiparams[0].SetValue("FECH_ENT_OP", Convert.ToDateTime(("04012020").Substring(0, 2) + "-" + ("04012020").Substring(2, 2) + "-" + ("04012020").Substring(4, 4)));
                tiparams[0].SetValue("KWMENG_PV", Cantidad);
                //tiparams[0].SetValue("KWMENG_PV", 12);
                tiparams[0].SetValue("ORDEN_SALIDA", O_Trans);
                //tiparams[0].SetValue("ORDEN_SALIDA", "");
                tiparams[0].SetValue("LOTE_DESPACHADO", Lote);
                //tiparams[0].SetValue("LOTE_DESPACHADO", "l002");
                tiparams[0].SetValue("OBSERVACION", observacion);//AQUIE SE DEBE INGRESAR EL TAG DescMovimiento
                //tiparams[0].SetValue("OBSERVACION", "RECIBIDO");//AQUIE SE DEBE INGRESAR EL TAG DescMovimiento
                if (descMovimiento == 1 || descMovimiento == 3)
                {
                    tiparams[0].SetValue("FECHARECEP", FechaRecibido);
                }
                tiparams[0].SetValue("LFDAT", FechaRecibido);
                //tiparams[0].SetValue("LFDAT", Convert.ToDateTime(("04012020").Substring(0, 2) + "-" + ("04012020").Substring(2, 2) + "-" + ("04012020").Substring(4, 4)));
                //tiparams[0].SetValue("CODRETORNO", "0");

                despacho.Invoke(destino);
                //trans.AddFunction(despacho);
                //trans.Commit(qas);

                //IRfcStructure retorno = despacho.GetStructure("TIPARAMS");
                IRfcTable retorno = despacho.GetTable("TIPARAMS");
                codigoRetorno = retorno.GetString("CODRETORNO");
            }
            catch (RfcCommunicationException e)
            {
                // network problem...
                Console.WriteLine("Error de Comunicacion : " + e.Message + "  " + e.TargetSite);
            }
            catch (RfcLogonException e)
            {
                // user could not logon...
                Console.WriteLine("Error Logon : " + e.Message);
            }
            catch (RfcAbapRuntimeException e)
            {
                // serious problem on ABAP system side...
                Console.WriteLine("Error de Run Time : " + e.Message);
            }
            catch (RfcAbapBaseException e)
            {
                // The function module returned an ABAP exception, an ABAP message
                // or an ABAP class-based exception...
                Console.WriteLine("Error Abap Base  : " + e.Message);
            }
            //RfcSessionManager.EndContext(destino);
            return codigoRetorno;
        }

        public string getTableLRS()
        {
            //RfcDestinationManager.RegisterDestinationConfiguration(new SAP());
            //RfcDestination qas = RfcDestinationManager.GetDestination("QAS");
            string codigoRetorno = null;
            try
            {
                RfcRepository repo = destino.Repository;
                IRfcFunction rfcfunction = repo.CreateFunction("Z_SD_EXTRACT_LEYRSOTO");
                rfcfunction.SetValue("I_WRK", "3000");
                rfcfunction.SetValue("I_VTW", "53");
                rfcfunction.SetValue("I_FDE", Convert.ToDateTime(("01012018").Substring(0, 2) + "-" + ("01012018").Substring(2, 2) + "-" + ("01012018").Substring(4, 4)));
                rfcfunction.SetValue("I_FHA", Convert.ToDateTime(("15012018").Substring(0, 2) + "-" + ("15012018").Substring(2, 2) + "-" + ("31012018").Substring(4, 4)));
                rfcfunction.SetValue("I_OFE", "");
                rfcfunction.SetValue("I_POS", "000000");

                rfcfunction.Invoke(destino);

                IRfcTable retorno = rfcfunction.GetTable("OUTPUT");

                foreach (IRfcStructure row in retorno)
                {
                    for (int i = 0; i < retorno.ElementCount; i++)
                    {
                        RfcElementMetadata rfcEMD = retorno.GetElementMetadata(i);
                        Console.WriteLine(rfcEMD.Name + ":" + row.GetString(rfcEMD.Name));
                    }
                    Console.WriteLine("---------------------------------------------------------\n\n");
                }

                Console.WriteLine(retorno.RowCount);

            }
            catch (RfcCommunicationException e)
            {
                // network problem...
                Console.WriteLine("Error de Comunicacion : " + e.Message + "  " + e.TargetSite);
            }
            catch (RfcLogonException e)
            {
                // user could not logon...
                Console.WriteLine("Error Logon : " + e.Message);
            }
            catch (RfcAbapRuntimeException e)
            {
                // serious problem on ABAP system side...
                Console.WriteLine("Error de Run Time : " + e.Message);
            }
            catch (RfcAbapBaseException e)
            {
                // The function module returned an ABAP exception, an ABAP message
                // or an ABAP class-based exception...
                Console.WriteLine("Error Abap Base  : " + e.Message);
            }

            return codigoRetorno;
        }
    }
}
