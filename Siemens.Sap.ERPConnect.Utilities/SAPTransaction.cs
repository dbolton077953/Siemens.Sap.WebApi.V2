using ERPConnect;
using ERPConnect.Utils;
using System.Diagnostics;


namespace Siemens.Sap.ERPConnect.Utilities
{
    [Serializable]
    public class SAPTransaction : IDisposable
    {
        //TODO
        private readonly R3Connection _connection;
        private Transaction _transaction;
        private bool _disposed = false;
        private RFCFunction _function;

        public SAPTransaction()
        {
        }

        public SAPTransaction(R3Connection connection)
            : this()
        {
            Trace.WriteLineIf(ApplicationParameters.IsTracingEnabled, string.Format("SAPTransaction constructor, Destination name: {0}", connection.LogonGroup != null ? connection.LogonGroup : string.Empty));
            //_transaction = new Transaction();
            _connection = connection;
        }

        public void Commit()
        {
            DoRfcCommit();
        }

        public void AddFunction(RFCFunction function)
        {
            //TODO:
            // Trace.WriteLineIf(ApplicationParameters.IsTracingEnabled, string.Format("SAPTransaction.AddFunction, function Name: {0}", function.Name));
            //_transaction.Function.Add(function);
        }

        private void DoRfcCommit()
        {
            try
            {
                Trace.WriteLineIf(ApplicationParameters.IsTracingEnabled, string.Format("SAPTransaction.Commit, Destination Name: {0}", _connection.LogonGroup));
               //toDO:  _transaction.Commit();
            }
            catch (Exception ex)
            {

                Trace.WriteLineIf(ApplicationParameters.IsTracingEnabled, string.Format("SAPTransaction.Commit, Error occured while committing: {0}", ex));
                throw ex;
            }
            finally
            {
            }
        }


        #region IDisposable Members

        public void Dispose()
        {
            Trace.WriteLineIf(ApplicationParameters.IsTracingEnabled, "SAPTransaction.Dispose");
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                   // _transaction = null;
                }
                _disposed = true;
            }
        }

        ~SAPTransaction()
        {
            Trace.WriteLineIf(ApplicationParameters.IsTracingEnabled, "SAPTransaction.Destructor");
            Dispose(false);
        }

        #endregion
    }

}
