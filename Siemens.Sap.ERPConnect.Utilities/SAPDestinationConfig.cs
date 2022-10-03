using SAP.Middleware.Connector;


namespace Siemens.Sap.ERPConnect.Utilities
{
    public class SAPDestinationConfig : IDestinationConfiguration
    {
        public SAPDestinationConfig( )
        {

        }
        public event RfcDestinationManager.ConfigurationChangeHandler ConfigurationChanged;

        public bool ChangeEventsSupported()
        {
            return false;
        }


        public RfcConfigParameters GetParameters(string destinationName)
        {
            RfcConfigParameters parms = new RfcConfigParameters();

            parms.Add(RfcConfigParameters.Name, destinationName);


            return null;
        }
    }
}
