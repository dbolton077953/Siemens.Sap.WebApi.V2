namespace Siemens.Sap.ERPConnect.Utilities
{
    public class SAPConnectionSettings
    {
        public string Name { get; set; }
        public string Client { get; set; }
        public string Passwd { get; set; }  
        public string SID { get; set; }   
        public string User { get; set; }
        public string Lang { get; set; }    
        public string SNCLib { get; set; }  
        public string MessageServer { get; set; }
        public string SysNr { get; set; }   
        public string Group { get; set; }   
        public string MaxPoolSize { get; set; }
        public string IdleTimeoutMs { get; set; }
        public string SNCPartnerName { get; set; }  
        public string SNCMyName { get; set; }
        public string SNCQOP { get; set; }
        public string SNCMode { get; set; }
        public string Host { get; set; }
        public bool Logging { get; set; }
        public string LogDir { get; set; }
        public bool EnableTracing { get; set; }
        public string TraceFile { get; set; }
 
    }
}
