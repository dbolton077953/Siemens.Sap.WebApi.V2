using System.Diagnostics;

namespace Siemens.Sap.WebAPI
{
    public class App
    {
        private const string _EvtSource = "Siemens.Sap.WebAPI";
        private AppSettings _appSettings;
        public App (IConfiguration config)
        {
            _appSettings = new AppSettings();
            config.GetSection("AppSettings").Bind(_appSettings);
            

        }

        public void WriteToTextFile(string message)
        {
            if (_appSettings.ShowDebugInfo)
            {
                FileStream myFileStream = null;
                var watch = System.Diagnostics.Stopwatch.StartNew();
                bool timeElapsed = false;
                while (timeElapsed == false && myFileStream == null)
                {
                    try
                    {
                        myFileStream = new FileStream(_appSettings.ErrorLog, FileMode.Append, FileAccess.Write, FileShare.None);
                        break;
                    }
                    catch (Exception ex)
                    {
                        WriteToEventLog(message, ex);
                        if (watch.ElapsedMilliseconds >= 5000)
                        {
                            timeElapsed = true;
                            watch.Stop();
                        }
                        Thread.Sleep(20);
                    }

                }

                if (timeElapsed)
                    return;

                using (StreamWriter w = new StreamWriter(myFileStream))
                {
                    w.WriteLine(DateTime.Now.ToString());
                    w.WriteLine(message);
                    w.Flush();
                    w.Close();
                }
            }
        }

        private void WriteToEventLog(string message, Exception ex)
        {
            try
            {
                EventLog.WriteEntry(_EvtSource
                    , string.Format("Date:{0}{1}Exception:{2}{3}Message:{4}{5}"
                                    , DateTime.Now.ToString()
                                    , Environment.NewLine
                                    , ex.ToString()
                                    , Environment.NewLine
                                    , message
                                    , Environment.NewLine));
            }
            catch (Exception)
            {

            }
        }

    }
}
