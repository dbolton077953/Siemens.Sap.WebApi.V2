using Siemens.Sap.ERPConnect.Utilities;
using System.Data;
using Microsoft.AspNetCore.Mvc;
using Siemens.Sap.WebAPI.Common.Models;
using System.Reflection;
using System.Linq.Expressions;
using Microsoft.AspNetCore.Authorization;

namespace Siemens.Sap.WebAPI.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class RfcController : ControllerBase
    {
        private AppSettings _appSettings;
        private SAPConnectionSettings _erpSettings;
        private static string _file;
        private App _app;

        public RfcController(IConfiguration configuration)
        {
            _appSettings = new AppSettings();
            configuration.GetSection("AppSettings").Bind(_appSettings);
   

            // Get appropriate NCO section
            _erpSettings = new SAPConnectionSettings();
            configuration.GetSection("ERPSettings").Bind(_erpSettings);

            if (_erpSettings.EnableTracing)
            {
                _erpSettings.TraceFile = SetTraceOutputFile(_erpSettings.TraceFile);
            }

            _app = new App(configuration);
        }


        [HttpPost]
        [Authorize(Policy = Policies.SapApiAdminRole)]
        [Route("[Action]", Name = "CallRFC")]
        public ActionResult<SAPCommandResponse> CallRFC([FromBody] SAPCommandRequest request)
        {
            _app.WriteToTextFile("Init SAPCommandResponse");

            SAPCommandResponse response = new SAPCommandResponse();

            _app.WriteToTextFile("Init Service");
            try
            {

                using (Service myRFCService = new Service(_erpSettings))
                {
                    _app.WriteToTextFile("Open SAP Connection");
                    _app.WriteToTextFile(string.Format("Conn= {0}",myRFCService.Connection.ConnectionString));


                    if (request.SAPRFCInformation.RFCUser != string.Empty)
                    {
                        myRFCService.Connection.UserName = request.SAPRFCInformation.RFCUser;
                    }

                    _app.WriteToTextFile("Open SAP Connection");
                
                    if (!myRFCService.Connection.IsOpen)
                    {
                        myRFCService.Connection.Open(true);
                    }
                    _app.WriteToTextFile("Get SAP COMMAND");
                    using (SAPCommand sc = myRFCService.GetSAPCommand(request.SAPRFCInformation))
                    {
                        _app.WriteToTextFile("Execute Dataset");

                        DataSet ds = sc.ExecuteDataSet();

                        _app.WriteToTextFile("Get Tables as Json string");
                        string tables = myRFCService.GetTablesAsJsonDelimitedList(ds);

                        _app.WriteToTextFile("Return response");

                        if (string.IsNullOrEmpty(tables))
                        {
                            if (sc.Out.Count > 0)
                            {
                                response.OutParams = sc.Out.ToArray();
                                return Ok(response);
                            }
                            else
                            {
                                return NotFound(response);
                            }
                        }
                        else
                        {
                            response.Tables = tables;
                            response.OutParams = sc.Out.ToArray();
                            return Ok(response);
                        }
                    }
                }
                
            }
            catch (Exception ex)
            {
                _app.WriteToTextFile(ex.ToString());
                return BadRequest(response);
            }
        }

        [HttpGet]
        [Route("[Action]", Name = "Test")]
        public ActionResult<string> Test()
        {
            return "Connected";
        }



        [HttpPost]
        [Authorize(Policy = Policies.SapApiAdminRole)]
        [Route("[Action]", Name = "ReadFromSAPTable")]
        public ActionResult<SAPReadTableResponse> ReadFromSAPTable([FromBody] SAPReadTableRequest request)
        {
            SAPReadTableResponse response = new SAPReadTableResponse();
            try
            {
                _app.WriteToTextFile("Create service");
                using (Service myRFCService = new Service(_erpSettings))
                {
                    _app.WriteToTextFile("Open SAP Connection");
                    myRFCService.Connection.Open(true);
                    _app.WriteToTextFile("Read SAP Table");

                    response.Results = myRFCService.ReadSAPTable(request.SAPTableName
                        , request.SAPTableFieldNames
                        , request.WhereClause
                        , request.MaxRowsToReturn);

                    if (response.Results.Length > 0)
                    {
                        return Ok(response);
                    }
                    else
                    {
                        return NotFound(response);
                    }

                }
            }
            catch (Exception ex)
            {
                _app.WriteToTextFile(ex.ToString());
                return BadRequest(response);
            }
        }

            private string SetTraceOutputFile(string traceFile)
        {
            if (string.IsNullOrWhiteSpace(_file))
            {
                if (string.IsNullOrEmpty(traceFile))
                {
                    string[] items = Assembly.GetExecutingAssembly().CodeBase.Replace("file:///", string.Empty).Split(new char[] { '/' }, StringSplitOptions.RemoveEmptyEntries);
                    for (int i = 0; i < items.Length - 1; i++)
                    {
                        _file += items[i] + "/";
                    }
                    _file += "ERP_{0}.trc";
                }
                else
                {
                    _file = traceFile;
                }
            }

            return _file;
        }
    }
}
