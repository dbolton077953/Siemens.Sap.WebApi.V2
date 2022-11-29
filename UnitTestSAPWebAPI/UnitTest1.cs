

using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json.Linq;
using NuGet.Frameworks;
using NUnit.Framework;
using Siemens.Sap.ERPConnect.Utilities;
using Siemens.Sap.WebAPI.Common.Models;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Net;
using System.Text.Json;
using System.Xml.Linq;

namespace UnitTestSAPWebAPI
{
    /*
    internal class Tests:UnitTestBase
    {

        private App _app;

        [SetUp]
        public void Setup()
        {
            InitHttpClientFactory();

            var _httpClientFactory = serviceProvider.GetService<IHttpClientFactory>();

            _app = new App(_httpClientFactory, _webAPIEndPoint);
        }

        [Test]
        public void TestOrderBOM()
        {
    
            List<SAPParameter> listX = new List<SAPParameter>();
            listX.Add(new SAPParameter { Name = "OPERATIONS", Value = "X" });
            listX.Add(new SAPParameter { Name = "COMPONENTS", Value = "X" });


            var request = new SAPCommandRequest()
            {

                SAPRFCInformation = new ExecuteInformation()
                {
                    RFCUser="RFC_CPO",
                    CommandText = "BAPI_PRODORD_GET_DETAIL",
                    ParameterInformationArray = new ParameterInformation[]
                        {
                            new ParameterInformation()
                            {
                                ContainerName="NUMBER",
                                ContainerType = ContainerType.Function,
                                ContainerOrdinalPosition=0,
                                Parameters = new List<SAPParameter>(new SAPParameter[]
                                {
                                 new SAPParameter { Name = "NUMBER", Value = "600000118201" }
                                })
                            },
                            new ParameterInformation
                            {
                                ContainerName = "ORDER_OBJECTS",
                                ContainerType = ContainerType.Structure,
                                ContainerOrdinalPosition=1,
                                Parameters = listX
                            }
                        }
                }
            };


            HttpResponseMessage response = _app.GetInfoFromSAP(request);

            string apiResponse = response.Content.ReadAsStringAsync().Result;
            var res = Newtonsoft.Json.JsonConvert.DeserializeObject<SAPCommandResponse>(apiResponse);

            Assert.IsTrue(response.StatusCode == HttpStatusCode.OK);

            if (response.StatusCode == HttpStatusCode.OK)
            {

                DataTable[] tbls = GetTables(res.Tables);

                Assert.IsTrue(tbls.Length == 2);


            }
    
        }


        [Test]
        public void NonExistantOrderBOM()
        {

            List<SAPParameter> listX = new List<SAPParameter>();
            listX.Add(new SAPParameter { Name = "OPERATIONS", Value = "X" });
            listX.Add(new SAPParameter { Name = "COMPONENTS", Value = "X" });


            var request = new SAPCommandRequest()
            {

                SAPRFCInformation = new ExecuteInformation()
                {
                    RFCUser="RFC_CPO",
                    CommandText = "BAPI_PRODORD_GET_DETAIL",
                    ParameterInformationArray = new ParameterInformation[]
                    {
                            new ParameterInformation()
                            {
                                ContainerName ="BAPI_PRODORD_GET_DETAIL",
                                ContainerType = ContainerType.Function,
                                ContainerOrdinalPosition=0,
                                Parameters = new List<SAPParameter>(new SAPParameter[]
                                {
                                 new SAPParameter { Name = "NUMBER", Value = "80000118201" }
                                })
                            },
                            new ParameterInformation
                            {
                                ContainerName = "ORDER_OBJECTS",
                                ContainerType = ContainerType.Structure,
                                ContainerOrdinalPosition=1,
                                Parameters = listX
                            }
                    },
                    OutParameterInformationArray = new ParameterInformation[]
                    {
                        
                        new ParameterInformation()
                        {
                            ContainerName = "RETURN",
                            ContainerType = ContainerType.Structure,
                            ContainerOrdinalPosition = 0,
                            Parameters = new List<SAPParameter>( new SAPParameter[]
                            {
                                new SAPParameter { Name = "MESSAGE", Value="" }
                            })
                        }
                    }
                }
            };


            HttpResponseMessage response = _app.GetInfoFromSAP(request);


            string apiResponse = response.Content.ReadAsStringAsync().Result;
            var res = Newtonsoft.Json.JsonConvert.DeserializeObject<SAPCommandResponse>(apiResponse);

            Assert.IsTrue(response.StatusCode ==  HttpStatusCode.NotFound);

            if (response.StatusCode == HttpStatusCode.NotFound)
            {


                Assert.IsTrue(response.IsSuccessStatusCode == false);


            }

        }


        [Test]
        public void GetPurchaseOrder()
        {

            var request = new SAPCommandRequest()
            {

                SAPRFCInformation = new ExecuteInformation()
                {
                    CommandText = "BAPI_PO_GETDETAIL1",
                    RequiresSession = true,
                    ParameterInformationArray = new ParameterInformation[]
                    {
                        
                            new ParameterInformation()
                            {
                                ContainerName="BAPI_PO_GETDETAIL1",
                                ContainerType = ContainerType.Function,
                                ContainerOrdinalPosition=0,
                                Parameters = new List<SAPParameter>(new SAPParameter[]
                                {
                                    new SAPParameter { Name = "PURCHASEORDER", Value = "5000565206" }
                                })
                            }

                    },
                    OutParameterInformationArray = new ParameterInformation[]
                    {

                        new ParameterInformation()
                        {
                            ContainerName = "POHEADER",
                            ContainerType = ContainerType.Structure,
                            ContainerOrdinalPosition = 0,
                            Parameters = new List<SAPParameter>( new SAPParameter[]
                            {
                                new SAPParameter { Name = "VENDOR", Value="" }
                            })
                        }
                    }
                }
            };


            HttpResponseMessage response = _app.GetInfoFromSAP(request);


            string apiResponse = response.Content.ReadAsStringAsync().Result;
            var res = Newtonsoft.Json.JsonConvert.DeserializeObject<SAPCommandResponse>(apiResponse);


            if (response.StatusCode == HttpStatusCode.OK)
            {

                DataTable[] tbls = GetTables(res.Tables);

                Assert.IsTrue(tbls.Length > 0);

                var vendor = res.OutParams[0].Value.ToString();

                Assert.IsNotEmpty(vendor);

            }
 
        }


        [Test]
        public void RFCReadTable()
        {

            var request = new SAPCommandRequest()
            {

                SAPRFCInformation = new ExecuteInformation()
                {
                    CommandText = "RFC_READ_TABLE",
                    RequiresSession = false,
                    ParameterInformationArray = new ParameterInformation[]
                    {

                        new ParameterInformation()
                        {
                            ContainerName = "RFC_READ_TABLE",
                            ContainerType = ContainerType.Function,
                            ContainerOrdinalPosition = 0,
                            Parameters = new List<SAPParameter>(new SAPParameter[]
                                {
                                    new SAPParameter { Name = "QUERY_TABLE", Value = "MARD" },
                                    new SAPParameter { Name = "DELIMITER", Value = "," },
                                    new SAPParameter { Name = "ROWCOUNT", Value = "100" }
                                })

                        },
                        new ParameterInformation()
                        {
                            ContainerName = "FIELDS",
                            ContainerType = ContainerType.Table,
                            ContainerOrdinalPosition = 0,
                            Parameters = new List<SAPParameter>(new SAPParameter[]
                                {
                                    new SAPParameter { Name = "FIELDNAME", Value = "MATNR" },
                                })
                        },
                        new ParameterInformation()
                        {
                            ContainerName = "FIELDS",
                            ContainerType = ContainerType.Table,
                            ContainerOrdinalPosition = 1,
                            Parameters = new List<SAPParameter>(new SAPParameter[]
                                {
                                    new SAPParameter { Name = "FIELDNAME", Value = "LGORT" },
                                })
                        },

                    },
                    Where = new WhereClause()
                    {
                        ContainerName = "OPTONS",
                        Items = (new WhereItem[]
                         {
                             new WhereItem { Condition = WhereConditionType.NONE, Name = "", Value= "("},
                             new WhereItem { Condition = WhereConditionType.EQUALS, Name = "LGORT", Value= "FST"},
                             new WhereItem { Condition = WhereConditionType.NONE, Name = "", Value= ")"}
                         }
                         ).ToList()
                    }
                }
            };


            HttpResponseMessage response = _app.GetInfoFromSAP(request);


            string apiResponse = response.Content.ReadAsStringAsync().Result;
            var res = Newtonsoft.Json.JsonConvert.DeserializeObject<SAPCommandResponse>(apiResponse);


            if (response.StatusCode == HttpStatusCode.OK)
            {

                DataTable[] tbls = GetTables(res.Tables);

                Assert.IsTrue(tbls.Length > 0);


            }

        }




        [Test]
        public void MRNItem()
        {
            DateTime dat = new DateTime(2022, 9, 16);
            var request = new SAPCommandRequest()
            {

                SAPRFCInformation = new ExecuteInformation()
                {
                    CommandText = "BAPI_GOODSMVT_CREATE",
                    RequiresSession = true,
                    ParameterInformationArray = new ParameterInformation[]
                    {

                            new ParameterInformation()
                            {
                                ContainerName="GOODSMVT_HEADER",
                                ContainerType = ContainerType.Structure,
                                ContainerOrdinalPosition=0,

                                Parameters = new List<SAPParameter>(new SAPParameter[]
                                {
                                    new SAPParameter { Name = "PSTNG_DATE", Value=dat.ToSAPDateTimeFromDateTime() },
                                    new SAPParameter { Name = "PR_UNAME", Value="gb7swid1"},
                                    new SAPParameter { Name = "HEADER_TXT", Value="XXX"},
                                    new SAPParameter { Name = "DOC_DATE", Value=dat.ToSAPDateTimeFromDateTime()},
                                })
                            },
                            new ParameterInformation()
                            {
                                ContainerName="GOODSMVT_CODE",
                                ContainerType = ContainerType.Structure,
                                ContainerOrdinalPosition=1,
                                Parameters = new List<SAPParameter>(new SAPParameter[]
                                {
                                    new SAPParameter { Name = "GM_CODE", Value = "03" }
                                })
                            },
                            new ParameterInformation()
                            {
                                ContainerName="GOODSMVT_ITEM",
                                ContainerType = ContainerType.Table,
                                ContainerOrdinalPosition=2,
                                Parameters = new List<SAPParameter>(new SAPParameter[]
                                {
                                    new SAPParameter { Name = "PLANT", Value="1011"},
                                    new SAPParameter { Name = "MATERIAL", Value = "1790L801A" },
                                    new SAPParameter { Name = "ENTRY_QNT", Value=1},
                                    new SAPParameter { Name = "PO_ITEM", Value = "" },
                                    new SAPParameter { Name = "PO_NUMBER", Value = "" },
                                    new SAPParameter { Name = "MOVE_TYPE", Value="551"},
                                    new SAPParameter { Name = "MOVE_STLOC", Value="FST"},
                                    new SAPParameter { Name = "STGE_LOC", Value="FST"},
                                    new SAPParameter { Name = "COSTCENTER",Value="0000100321"},
                                    new SAPParameter { Name = "GL_ACCOUNT",Value="0000696110"}
                                })
                            },
       
                    },
                    OutParameterInformationArray = new ParameterInformation[]
                     {

                        new ParameterInformation()
                        {
                            ContainerName = "MATERIALDOCUMENT",
                            ContainerType = ContainerType.Function,
                            ContainerOrdinalPosition = 0,
                            Parameters = new List<SAPParameter>( new SAPParameter[]
                            {
                                new SAPParameter { Name = "MATERIALDOCUMENT", Value="" },
                                new SAPParameter { Name = "MATDOCUMENTYEAR", Value=""}
                            })
                        },
                        new ParameterInformation()
                        {
                            ContainerName = "RETURN",
                            ContainerType = ContainerType.Table,
                            ContainerOrdinalPosition = 1,
                            Parameters = new List<SAPParameter>( new SAPParameter[]
                            {
                                new SAPParameter { Name = "NUMBER", Value="" },
                                new SAPParameter { Name = "MESSAGE", Value=""}
                            })

                         }
                    }
                }
            };


            HttpResponseMessage response = _app.GetInfoFromSAP(request);


            string apiResponse = response.Content.ReadAsStringAsync().Result;
            var res = Newtonsoft.Json.JsonConvert.DeserializeObject<SAPCommandResponse>(apiResponse);


            if (response.StatusCode == HttpStatusCode.OK)
            {

                DataTable[] tbls = GetTables(res.Tables);

                Assert.IsTrue(tbls.Length > 0);

                var docNumber= res.OutParams[0].Value.ToString();

                Assert.IsNotEmpty(docNumber);

            }

        }

        [Test]
        public void GetMaterialAvailability()
        {
            var request = new SAPCommandRequest()
            {

                SAPRFCInformation = new ExecuteInformation()
                {
                    RFCUser = "RFC_GMC",
                    CommandText = "BAPI_MATERIAL_AVAILABILITY",
                    ParameterInformationArray = new ParameterInformation[]
                    {

                            new ParameterInformation()
                            {
                                ContainerName="BAPI_MATERIAL_AVAILABILITY",
                                ContainerType = ContainerType.Function,
                                ContainerOrdinalPosition=0,

                                Parameters = new List<SAPParameter>(new SAPParameter[]
                                {
                                    new SAPParameter { Name = "PLANT", Value="1011" },
                                    new SAPParameter { Name = "MATERIAL", Value="1790L801A"},
                                    new SAPParameter { Name = "UNIT", Value="PC"}
                                   
                                })
                            }

                     },
                    OutParameterInformationArray = new ParameterInformation[]
                     {

                        new ParameterInformation()
                        {
                            ContainerName = "AV_QTY_PLT",
                            ContainerType = ContainerType.Function,
                            ContainerOrdinalPosition = 0,
                            Parameters = new List<SAPParameter>( new SAPParameter[]
                            {
                                new SAPParameter { Name = "AV_QTY_PLT", Value="" },
                            })
                        },
                        new ParameterInformation()
                        {
                            ContainerName = "RETURN",
                            ContainerType = ContainerType.Structure,
                            ContainerOrdinalPosition = 1,
                            Parameters = new List<SAPParameter>( new SAPParameter[]
                            {
                                new SAPParameter { Name = "CODE", Value="" },
                                new SAPParameter { Name = "MESSAGE", Value=""}
                            })

                         }
                     }
                }
            };

            HttpResponseMessage response = _app.GetInfoFromSAP(request);

            string apiResponse = response.Content.ReadAsStringAsync().Result;
            var res = Newtonsoft.Json.JsonConvert.DeserializeObject<SAPCommandResponse>(apiResponse);

            if(response.IsSuccessStatusCode)
            {
                Assert.IsTrue(res.OutParams[0].Value != "");
            }


        }

    

    }

    */
}