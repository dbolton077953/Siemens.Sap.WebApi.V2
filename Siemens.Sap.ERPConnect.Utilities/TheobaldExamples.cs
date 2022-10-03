using ERPConnect;
using StrongInterop.ADODB;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace Siemens.Sap.ERPConnect.Utilities
{
    public class TheobaldExamples
    {

   
        public TheobaldExamples()
        {

        }


        public bool MRNTest(R3Connection  Con, string materialNumber, string Plant, decimal Quantity)
        {

            Trace.WriteLineIf(true, "Theobald MRNTest");

            string rMessage;
            // Fill Export Structures for the FunctionModule
            Trace.WriteLineIf(true, "Create BAPI_GOODSMVT_CREATE");
            RFCFunction func = Con.CreateFunction("BAPI_GOODSMVT_CREATE");
            Trace.WriteLineIf(true, "PSTNG_DATE\nPR_UNAME\nHEADER_TXT\nDOC_DATE");
            func.Exports["GOODSMVT_HEADER"].ToStructure()["PSTNG_DATE"] = "20220916"; //Posting Date in the Document
            func.Exports["GOODSMVT_HEADER"].ToStructure()["PR_UNAME"] = "gb7swid1";       //UserName
            func.Exports["GOODSMVT_HEADER"].ToStructure()["HEADER_TXT"] = "XXX";      //HeaderText
            func.Exports["GOODSMVT_HEADER"].ToStructure()["DOC_DATE"] = "20220916";   //Document Date in Document
            Trace.WriteLineIf(true, "GM_CODE 03");
            func.Exports["GOODSMVT_CODE"].ToStructure()["GM_CODE"] = "03";

            Trace.WriteLineIf(true, "PLANT\nMATERIAL\nENTRY_QTY\nMOVE_TYPE\nMOVE_STLOC\nCOSTCENTER\nGL_ACCOUNT");
            // Fill Table Items 
            RFCStructure itemrow = func.Tables["GOODSMVT_ITEM"].Rows.Add();
            itemrow["PLANT"] = Plant;                    //Plant
            itemrow["MATERIAL"] = materialNumber;        //Purchase Order Number
            itemrow["PO_ITEM"] = "";        //Purchase Order Number
            itemrow["PO_NUMBER"] = "";                  //Purchase Order Number
            itemrow["ENTRY_QNT"] = Quantity;         //Quantity in Unit of Entry
            itemrow["MOVE_TYPE"] = "551";            //Movement Type
            itemrow["MOVE_STLOC"] = "FST";            //Movement Type
            //itemrow["MVT_IND"] = "B";                //Movement Indicator
            itemrow["STGE_LOC"] = "FST";            //Storage Location
            itemrow["COSTCENTER"] = "0000100321";   // coST CNETER
            itemrow["GL_ACCOUNT"] = "0000696110";   // GL ACCOUNT

            Trace.WriteLineIf(true, "Exec func");
            // Execute Function Module
            func.Execute();
            Trace.WriteLineIf(true, "BAPI_TRANSACTION_COMMIT");
            RFCFunction funcCommit = Con.CreateFunction("BAPI_TRANSACTION_COMMIT");
            funcCommit.Exports["WAIT"].ParamValue = "X";

            Trace.WriteLineIf(true, "REtreieve Doc details");
            string MaterialDocument = func.Imports["MATERIALDOCUMENT"].ParamValue.ToString();
            string MatDocumentYear = func.Imports["MATDOCUMENTYEAR"].ParamValue.ToString();

            Trace.WriteLineIf(true, MaterialDocument);

            // Check Return-Table
            if (func.Tables["RETURN"].RowCount > 0)
            {
                rMessage = func.Tables["RETURN"].Rows[0, "MESSAGE"].ToString();
                funcCommit.Execute();
                return !func.Tables["RETURN"].Rows[0, "TYPE"].ToString().Equals("E");
            }
            else
            {
                funcCommit.Execute();
                rMessage = "";
                return true;
            }
        }

    }
}
