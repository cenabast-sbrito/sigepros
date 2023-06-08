using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SAP.Middleware.Connector;

namespace WebApplicationTestConnector
{
    class SAPConfig : IDestinationConfiguration
    {
        public SAPConfig(){

        }

        // The following two are not used in this example:
        public bool ChangeEventsSupported()
        {
            return true;
        }


        RfcDestinationManager.ConfigurationChangeHandler changeHandler;
        public event RfcDestinationManager.ConfigurationChangeHandler ConfigurationChanged
        {
            add { changeHandler += value; }
            remove{ changeHandler -= value; }
        }

        public RfcConfigParameters GetParameters(String destinationName)
        {
            if ("QAS".Equals(destinationName))
            {
                RfcConfigParameters parms = new RfcConfigParameters();
                parms.Add(RfcConfigParameters.AppServerHost, "10.8.0.76");
                parms.Add(RfcConfigParameters.SystemNumber, "00");
                parms.Add(RfcConfigParameters.User, "aaguilar");
                parms.Add(RfcConfigParameters.Password, "saprfc");
                parms.Add(RfcConfigParameters.Client, "400");
                parms.Add(RfcConfigParameters.Language, "ES");
                parms.Add(RfcConfigParameters.PoolSize, "5");
                //parms.Add(RfcConfigParameters.MaxPoolSize, "10");
                //parms.Add(RfcConfigParameters.IdleTimeout, "600");
                return parms;
            }
            else if ("PRD".Equals(destinationName))
            {
                RfcConfigParameters parms = new RfcConfigParameters();
                parms.Add(RfcConfigParameters.AppServerHost, "10.8.0.77");
                parms.Add(RfcConfigParameters.SystemNumber, "00");
                parms.Add(RfcConfigParameters.User, "userrfc");
                parms.Add(RfcConfigParameters.Password, "userrfc1");
                parms.Add(RfcConfigParameters.Client, "400");
                parms.Add(RfcConfigParameters.Language, "ES");
                parms.Add(RfcConfigParameters.PoolSize, "5");
                //parms.Add(RfcConfigParameters.MaxPoolSize, "10");
                //parms.Add(RfcConfigParameters.IdleTimeout, "600");
                return parms;
            }
            else return null;
        }
                
    }
}
