using System.Diagnostics;

namespace Siemens.Sap.ERPConnect.Utilities
{
    [Serializable]
    public class SAPConnection : IDisposable
    {
        private string _destinationName;

        private bool _disposed = false;
        private string _traceName;



        public bool IsOpen { get; set; }

        private SAPConnection()
        {

        }

        //public SAPConnection(string destName) :
        //    this(DestinationConfiguration.Get(destName))
        //{
        //    IsOpen = false;
        //    this._destinationName = destName;
        //}

        public SAPConnection(IDestinationConfiguration config) : this()
        {
            if (ApplicationParameters.IsTracingEnabled)
            {
                _traceName = string.Format("{0}_{1}", Guid.NewGuid().ToString().Replace("-", string.Empty), Thread.CurrentThread.ManagedThreadId.ToString());

                Trace.Listeners.Add(new TextWriterTraceListener(string.Format(ApplicationParameters.TraceFile, _traceName), _traceName));

            }

            Trace.WriteLineIf(ApplicationParameters.IsTracingEnabled, string.Format("SAPConnection constructor, Configuration name: {0}", _destinationName));

            _configuration = config;

            Trace.WriteLineIf(ApplicationParameters.IsTracingEnabled, "SAPConnection constructor, Registering confguration with RfcDestinationManager");

            try
            {
                RfcDestinationManager.RegisterDestinationConfiguration(_configuration);
            }
            catch { }
        }

        public void Open()
        {
            Trace.WriteLineIf(ApplicationParameters.IsTracingEnabled, string.Format("SAPConnection Open, RfcDestinationManager.GetDestination({0})", _destinationName));
            Destination = RfcDestinationManager.GetDestination(_destinationName);
            Trace.WriteLineIf(ApplicationParameters.IsTracingEnabled, "SAPConnection Open, Connection open == true");
            IsOpen = true;
        }

        public void Close()
        {
            Trace.WriteLineIf(ApplicationParameters.IsTracingEnabled, string.Format("SAPConnection Close, (_configuration != null) == ", _configuration != null));
            if (_configuration != null)
            {
                Trace.WriteLineIf(ApplicationParameters.IsTracingEnabled, "SAPConnection Close, RfcDestinationManager.UnRegisterDestinationConfiguration(_configuration)");
                //RfcDestinationManager.UnregisterDestinationConfiguration(_configuration);
                Destination = null;
                _configuration = null;
            }
            IsOpen = false;
        }

        #region IDisposable Members

        public void Dispose()
        {
            Trace.WriteLineIf(ApplicationParameters.IsTracingEnabled, "SAPConnection Dispose");
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    if (_configuration != null)
                    {
                        try
                        {
                            Close();
                        }
                        catch (Exception ex)
                        {
                            Trace.WriteLineIf(ApplicationParameters.IsTracingEnabled, string.Format("SAPConnection Dispose: Error occured while disposing SapConnection: {0}", ex));
                        }
                    }
                }
                if (ApplicationParameters.IsTracingEnabled)
                {
                    var listener = System.Diagnostics.Trace.Listeners.OfType<TextWriterTraceListener>().Where(t => t.Name.ToLower() == _traceName.ToLower()).FirstOrDefault();

                    listener.Flush();
                    listener.Close();
                    listener.Dispose();

                    System.Diagnostics.Trace.Listeners.Remove(_traceName);
                }
                _disposed = true;
            }
        }

        ~SAPConnection()
        {
            Trace.WriteLineIf(ApplicationParameters.IsTracingEnabled, "SAPConnection destructor");
            Dispose(false);
        }

        #endregion
    }

}
