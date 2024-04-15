using Microsoft.Extensions.Configuration;
using System.Data;
using System.Data.Common;
using System.Xml;
using Npgsql;
using OzdocsMobileWebAPI.Models;
using System.Windows.Input;
using Microsoft.AspNetCore.Mvc;
using System.Text;
using System.Reflection.Metadata;

namespace OzdocsMobileWebAPI.DataAccessLayer
{
    public class DataAccess
    {

        private readonly IConfiguration _configuration;
        private readonly string pgdbconnection;
        private readonly string sSchemas = "ozserver";

        public DataAccess(IConfiguration configuration)
        {
            _configuration = configuration;
            pgdbconnection = _configuration.GetConnectionString("DefaultConnection");
        }

        internal DataTable GetUserLoginDtls(string UserName, string Password)
        {
            return RunQuery("SELECT tu.\"Id\", tOO.\"Id\" as \"OrgId\",  tOO.\"OfficeId\", tOO.\"CompanyName\", tOO.\"Email_Address\", tu.\"UserId\", tu.\"Password\" FROM ozserver.\"tblUser\" as tu inner join ozserver.\"tblOrganisation\" as tOO on tOO.\"OfficeId\" = tU.\"OrgId\"  where \"UserId\" = '" + UserName + "' and \"Password\" = '" + Password + "'");
        }

        internal DataTable GetOrganisationData()
        {
            return RunQuery("SELECT * FROM " + sSchemas + "." + "\"tblOrganisation\"");
        }

        internal DataTable GetOrganisationData(string officeId)
        {
            return RunQuery("SELECT * FROM " + sSchemas + "." + "\"tblOrganisation\" where \"OfficeId\" = '" + officeId + "'");
        }

        internal int SaveOrganisationData(Organisation oOrganisation)
        {
            var timestamp = DateTime.UtcNow;

            using (NpgsqlConnection conn = new NpgsqlConnection(pgdbconnection))
            {
                conn.Open();

                string sQuery = "INSERT INTO " + sSchemas + "." + "\"tblOrganisation\" (\"OfficeId\", \"CompanyName\", \"Email_Address\") VALUES (:OfficeId, :CompanyName, :Email_Address) RETURNING \"Id\"";

                using (NpgsqlCommand cmd = new NpgsqlCommand(sQuery, conn))
                {
                    DateTime now = DateTime.Now;

                    cmd.Parameters.AddWithValue("OfficeId", oOrganisation.OfficeId);
                    cmd.Parameters.AddWithValue("CompanyName", oOrganisation.CompanyName);
                    cmd.Parameters.AddWithValue("Email_Address", oOrganisation.Email_Address);  
                   
                    {
                        Object _result = cmd.ExecuteScalar();

                        if (_result != null && _result != DBNull.Value)
                        {
                            return (int)_result;
                        }
                        return -1;
                    }                    
                }
            }
        }

        internal DataTable GetUserData()
        {
            return RunQuery("SELECT * FROM " + sSchemas + "." + "\"tblUser\"");
        }

        internal DataTable GetUserData(string OrgId)
        {
            return RunQuery("SELECT * FROM " + sSchemas + "." + "\"tblUser\" where \"OrgId\" = '" + OrgId + "'");
        }

        internal string SaveUserData(User oUser)
        {
            var timestamp = DateTime.UtcNow;

            using (NpgsqlConnection conn = new NpgsqlConnection(pgdbconnection))
            {
                conn.Open();

                string sQuery = "INSERT INTO " + sSchemas + "." + "\"tblUser\" (\"OrgId\", \"UserId\", \"Password\") VALUES (:OrgId, :UserId, :Password) RETURNING \"OrgId\"";

                using (NpgsqlCommand cmd = new NpgsqlCommand(sQuery, conn))
                {
                    DateTime now = DateTime.Now;

                    cmd.Parameters.AddWithValue("OrgId", oUser.OrgId);
                    cmd.Parameters.AddWithValue("UserId", oUser.UserId);
                    cmd.Parameters.AddWithValue("Password", oUser.Password);

                    {
                        Object _result = cmd.ExecuteScalar();

                        if (_result != null && _result != DBNull.Value)
                        {
                            return (string)_result;
                        }
                        return null;
                    }
                }
            }
        }

        internal DataTable GetMasterDocumentData()
        {
            return RunQuery("SELECT * FROM " + sSchemas + "." + "\"tblMasterDocument\"");
        }

        internal DataTable GetMasterDocumentData(string officeId)
        {
            return RunQuery("SELECT * FROM " + sSchemas + "." + "\"tblMasterDocument\" where \"OfficeId\" = '" + officeId + "'");
        }

        internal DataTable GetMasterDocumentData(int Id)
        {
            return RunQuery("SELECT * FROM " + sSchemas + "." + "\"tblMasterDocument\" where \"Id\" = " + Id);
        }

        internal DataTable IsMasterDocumentExist(string MasterId, int Version, string OfficeId)
        {
            return RunQuery("SELECT * FROM " + sSchemas + "." + "\"tblMasterDocument\" where \"DocumentId\" = '" + MasterId + "' and \"Version\"='" + Version + "' and \"OfficeId\"='" + OfficeId + "'");
        }

       
        internal DataTable GetMasterDocumentByFilter(DateTime? toDate, DateTime? fromDate, string? DocumentId, string? InvoiceNo, DateTime? ToInvoice, DateTime? FromInvoice, string? Exporter, string? Edn)
        {
            StringBuilder queryBuilder = new StringBuilder("SELECT * FROM ");
            queryBuilder.Append(sSchemas).Append(".").Append("\"tblMasterDocument\" WHERE 1=1");


            if (toDate == null && fromDate != null)
            {
                toDate = fromDate;
            }

            else if (fromDate == null && toDate != null)
            {
                fromDate = toDate;
            }


            if (toDate != null && fromDate != null)
            {
                DateTime toDateValue = toDate.Value.Date;
                DateTime fromDateValue = fromDate.Value.Date;
                queryBuilder.Append(" AND CAST(\"RequestDate\" AS DATE) BETWEEN '")
                            .Append(fromDateValue.ToString("yyyy-MM-dd"))
                            .Append("' AND '")
                            .Append(toDateValue.ToString("yyyy-MM-dd"))
                            .Append("'");
            }


            if (!string.IsNullOrEmpty(DocumentId))
            {
                string trimmedDocumentId = DocumentId.Trim();
                queryBuilder.Append(" AND \"DocumentId\" = '").Append(trimmedDocumentId).Append("'");
            }

            if (!string.IsNullOrEmpty(InvoiceNo))
            {
                InvoiceNo = InvoiceNo.Trim();
                queryBuilder.Append(" AND \"InvoiceNo\" = '").Append(InvoiceNo).Append("'");
            }

            if (ToInvoice == null && FromInvoice != null)
            {
                ToInvoice = FromInvoice;
            }

            else if (FromInvoice == null && ToInvoice != null)
            {
                FromInvoice = ToInvoice;
            }


            if (ToInvoice != null && FromInvoice != null)
            {
                DateTime toDateValue = ToInvoice.Value.Date;
                DateTime fromDateValue = FromInvoice.Value.Date;
                queryBuilder.Append(" AND CAST(\"RequestDate\" AS DATE) BETWEEN '")
                            .Append(fromDateValue.ToString("yyyy-MM-dd"))
                            .Append("' AND '")
                            .Append(toDateValue.ToString("yyyy-MM-dd"))
                            .Append("'");
            }
            if (!string.IsNullOrEmpty(Exporter))
            {
                Exporter = Exporter.Trim();
                queryBuilder.Append(" AND \"Exporter\" = '").Append(Exporter).Append("'");
            }
            if (!string.IsNullOrEmpty(Edn))
            {
                Edn = Edn.Trim();
                queryBuilder.Append(" AND \"EDN\" = '").Append(Edn).Append("'");
            }
            string query = queryBuilder.ToString();
            return RunQuery(query);
        }


        internal DataTable GetMasterDocumentRecordCount(DateTime? toDate, DateTime? fromDate, string? TypeOfQuery, string? Id)
        {
            StringBuilder queryBuilder = new StringBuilder();

            if(TypeOfQuery == "DATE")
            {
                DateTime toDateValue = toDate.Value.Date;
                queryBuilder.Append("SELECT COUNT(*) FROM " + sSchemas + "." + "\"tblMasterDocument\" WHERE CAST(\"RequestDate\" AS DATE) = '")
                   .Append(toDateValue.ToString("yyyy-MM-dd"))
                   .Append("'");
               
            }
            else if(TypeOfQuery == "DATERANGE")
            {
                if (toDate == null && fromDate != null)
                {
                    toDate = fromDate;
                }

                else if (fromDate == null && toDate != null)
                {
                    fromDate = toDate;
                }


                if (toDate != null && fromDate != null)
                {
                    DateTime toDateValue = toDate.Value.Date;
                    DateTime fromDateValue = fromDate.Value.Date;
                    queryBuilder.Append("SELECT COUNT(*) FROM " + sSchemas + "." + "\"tblMasterDocument\" WHERE CAST(\"RequestDate\" AS DATE) BETWEEN '")
                                .Append(fromDateValue.ToString("yyyy-MM-dd"))
                                .Append("' AND '")
                                .Append(toDateValue.ToString("yyyy-MM-dd"))
                                .Append("'");
                }
            }
            else if(TypeOfQuery == "OFFICEID")
            {
                Id = Id.Trim();
                queryBuilder.Append("SELECT COUNT(*) FROM ")
                            .Append(sSchemas)
                            .Append(".\"tblMasterDocument\" ")
                            .Append("WHERE \"OfficeId\" = '")
                            .Append(Id) 
                            .Append("'");
                 
            }
            else
            {
                queryBuilder.Append("SELECT COUNT(*) FROM ")
                            .Append(sSchemas)
                            .Append(".\"tblMasterDocument\" ");
            }
            string query = queryBuilder.ToString();
            return RunQuery(query);
        }
        internal int SaveMasterData(Master omaster)
        {
            var timestamp = DateTime.UtcNow;

            using (NpgsqlConnection conn = new NpgsqlConnection(pgdbconnection))
            {
                conn.Open();

                string sQuery = "INSERT INTO " + sSchemas + "." + "\"tblMasterDocument\" (\"OfficeId\", \"DocumentId\", \"ReferenceId\", \"Exporter\", \"Consignee\", \"Buyer\", \"InvoiceValue\", \"InvoiceNo\", \"InvoiceDate\", \"Currency\", \"DocumentStatus\", \"EDNStatus\", \"EDN\", \"Details\", \"ExporterRef\", \"BuyerRef\", \"MUserId\", \"RequestDate\", \"RequestTime\", \"Version\", \"CreationDate\", \"RevisionDate\") VALUES (:OfficeId, :DocumentId, :ReferenceId, :Exporter, :Consignee, :Buyer, :InvoiceValue, :InvoiceNo, :InvoiceDate, :Currency, :DocumentStatus, :EDNStatus, :EDN, :Details, :ExporterRef, :BuyerRef, :MUserId, :RequestDate, :RequestTime, :Version, :CreationDate, :RevisionDate) RETURNING \"Id\"";

                using (NpgsqlCommand cmd = new NpgsqlCommand(sQuery, conn))
                {
                    DateTime now = DateTime.Now;
                    cmd.Parameters.AddWithValue("RequestTime", now);

                    cmd.Parameters.AddWithValue("RequestDate", timestamp);

                    cmd.Parameters.AddWithValue("OfficeId", omaster.OfficeId);
                    cmd.Parameters.AddWithValue("DocumentId", omaster.DocumentId);
                    cmd.Parameters.AddWithValue("ReferenceId", omaster.ReferenceId);

                    cmd.Parameters.AddWithValue("Version", omaster.Version);

                    cmd.Parameters.AddWithValue("Exporter", omaster.Exporter);
                    cmd.Parameters.AddWithValue("Consignee", omaster.Consignee);
                    cmd.Parameters.AddWithValue("Buyer", omaster.Buyer);
                    cmd.Parameters.AddWithValue("InvoiceValue", omaster.InvoiceValue);
                    cmd.Parameters.AddWithValue("Currency", omaster.Currency);
                    cmd.Parameters.AddWithValue("InvoiceNo", omaster.InvoiceNo);
                    cmd.Parameters.AddWithValue("InvoiceDate", omaster.InvoiceDate);

                    cmd.Parameters.AddWithValue("DocumentStatus", omaster.DocumentStatus);
                    cmd.Parameters.AddWithValue("EDNStatus", omaster.EDNStatus);
                    cmd.Parameters.AddWithValue("EDN", omaster.EDN);

                    cmd.Parameters.AddWithValue("Details", omaster.Details);
                    cmd.Parameters.AddWithValue("ExporterRef", omaster.ExporterRef);
                    cmd.Parameters.AddWithValue("BuyerRef", omaster.BuyerRef);
                    cmd.Parameters.AddWithValue("MUserId", omaster.MUserId);

                    cmd.Parameters.AddWithValue("CreationDate", omaster.CreationDate);
                    cmd.Parameters.AddWithValue("RevisionDate", omaster.RevisionDate);


                    //try
                    {
                        Object _result = cmd.ExecuteScalar();

                        if (_result != null && _result != DBNull.Value)
                        {
                            return (int)_result;
                        }
                        return -1;
                        //return "";
                        //oResponseMsg.ResponseDateTime = DateTime.Now;
                    }
                    //catch (Exception oEx)
                    //{
                    //ErrorMessage = oEx.Message;

                    //oCreateLog.CreateLogFile(LogFilePath, oEx.StackTrace.ToString());

                    //oResponseMsg.Status = "Error";
                    //oResponseMsg.ErrorMsg = oEx.Message;
                    //oResponseMsg.ResponseDateTime = DateTime.Now;
                    return -1; // Return -1 if the insertion was unsuccessful
                    //}
                }
            }

            //return "";
        }


        internal DataTable GetEDNDocumentData()
        {
            return RunQuery("SELECT * FROM " + sSchemas + "." + "\"tblEDNDocument\"");
        }
        internal DataTable GetEDNDocumentDataWithoutContent()
        {
            return RunQuery("SELECT \"Id\", \"EdndocsId\", \"SenderRef\", \"Version\", \"Status\", \"EDN\", \"RequestDate\", \"RequestTime\", \"OfficeId\", \"SDateTime\", \"RDateTime\", \"ControlRefNo\", \"Acknowledge\", \"StatusDesc\", \"ReasonDesc\", \"FileInName\", \"FileOutName\", \"Error1\", \"Error2\", \"Error3\", \"Error4\", \"Error5\", \"Error6\", \"Error7\", \"Error8\", \"Error9\", \"Error10\", \"Error11\", \"Error12\", \"Error13\", \"Error14\", \"Error15\", \"Error16\", \"Error17\", \"Error18\", \"Error19\", \"Error20\" FROM " + sSchemas + "." + "\"tblEDNDocument\"");
        }
        internal DataTable GetEDNDocumentData(string officeId)
        {
            return RunQuery("SELECT * FROM " + sSchemas + "." + "\"tblEDNDocument\" where \"OfficeId\" = '" + officeId + "'");
        }
        internal DataTable GetEDNDocumentDataWithoutContent(string officeId)
        {
            return RunQuery("SELECT \"Id\", \"EdndocsId\", \"SenderRef\", \"Version\", \"Status\", \"EDN\", \"RequestDate\", \"RequestTime\", \"OfficeId\", \"SDateTime\", \"RDateTime\", \"ControlRefNo\", \"Acknowledge\", \"StatusDesc\", \"ReasonDesc\", \"FileInName\", \"FileOutName\", \"Error1\", \"Error2\", \"Error3\", \"Error4\", \"Error5\", \"Error6\", \"Error7\", \"Error8\", \"Error9\", \"Error10\", \"Error11\", \"Error12\", \"Error13\", \"Error14\", \"Error15\", \"Error16\", \"Error17\", \"Error18\", \"Error19\", \"Error20\" FROM " + sSchemas + "." + "\"tblEDNDocument\" where \"OfficeId\" = '" + officeId + "'");
        }

        internal DataTable GetEDNDocumentData(int Id)
        {
            return RunQuery("SELECT * FROM " + sSchemas + "." + "\"tblEDNDocument\" where \"Id\" = " + Id);
        }

        internal DataTable IsEDNDocumentExist(string EdndocsId, int Version, string OfficeId)
        {
            return RunQuery("SELECT * FROM " + sSchemas + "." + "\"tblEDNDocument\" where \"EdndocsId\" = '" + EdndocsId + "' and \"Version\"='" + Version + "' and \"OfficeId\"='" + OfficeId + "'");
        }
        internal DataTable GetEDNDataByFilter(DateTime? toDate, DateTime? fromDate, string? senderRef, string? edn)
        {
            StringBuilder queryBuilder = new StringBuilder("SELECT * FROM ");
            queryBuilder.Append(sSchemas).Append(".").Append("\"tblEDNDocument\" WHERE 1=1");

          
            if (toDate == null && fromDate != null)
            {
                toDate = fromDate;
            }
           
            else if (fromDate == null && toDate != null)
            {
                fromDate = toDate;
            }

         
            if (toDate != null && fromDate != null)
            {
                DateTime toDateValue = toDate.Value.Date;
                DateTime fromDateValue = fromDate.Value.Date;
                queryBuilder.Append(" AND CAST(\"RequestDate\" AS DATE) BETWEEN '")
                            .Append(fromDateValue.ToString("yyyy-MM-dd"))
                            .Append("' AND '")
                            .Append(toDateValue.ToString("yyyy-MM-dd"))
                            .Append("'");
            }


            if (!string.IsNullOrEmpty(senderRef))
            {
                
                string trimmedSenderRef = senderRef.Trim();
                queryBuilder.Append(" AND \"SenderRef\" = '").Append(trimmedSenderRef).Append("'");
            }
           
            if (!string.IsNullOrEmpty(edn))
            {
               
                string trimmedEdn = edn.Trim();
                queryBuilder.Append(" AND \"EDN\" = '").Append(trimmedEdn).Append("'");
            }

           
            string query = queryBuilder.ToString();
            return RunQuery(query);
        }

        internal DataTable GetEDNDocumentRecordCount(DateTime? toDate, DateTime? fromDate, string? TypeOfQuery, string? Id)
        {
            StringBuilder queryBuilder = new StringBuilder();

            if (TypeOfQuery == "DATE")
            {
                DateTime toDateValue = toDate.Value.Date;
                queryBuilder.Append("SELECT COUNT(*) FROM " + sSchemas + "." + "\"tblEDNDocument\" WHERE CAST(\"RequestDate\" AS DATE) = '")
                   .Append(toDateValue.ToString("yyyy-MM-dd"))
                   .Append("'");

            }
            else if (TypeOfQuery == "DATERANGE")
            {
                if (toDate == null && fromDate != null)
                {
                    toDate = fromDate;
                }

                else if (fromDate == null && toDate != null)
                {
                    fromDate = toDate;
                }


                if (toDate != null && fromDate != null)
                {
                    DateTime toDateValue = toDate.Value.Date;
                    DateTime fromDateValue = fromDate.Value.Date;
                    queryBuilder.Append("SELECT COUNT(*) FROM " + sSchemas + "." + "\"tblEDNDocument\" WHERE CAST(\"RequestDate\" AS DATE) BETWEEN '")
                                .Append(fromDateValue.ToString("yyyy-MM-dd"))
                                .Append("' AND '")
                                .Append(toDateValue.ToString("yyyy-MM-dd"))
                                .Append("'");
                }
            }
            else if (TypeOfQuery == "OFFICEID")
            {
                string trimmedId = Id.Trim();
                queryBuilder.Append("SELECT COUNT(*) FROM ")
                            .Append(sSchemas)
                            .Append(".\"tblEDNDocument\" ")
                            .Append("WHERE \"OfficeId\" = '")
                            .Append(trimmedId)
                            .Append("'");

            }
            else
            {
                queryBuilder.Append("SELECT COUNT(*) FROM ")
                            .Append(sSchemas)
                            .Append(".\"tblEDNDocument\" ");
            }
            string query = queryBuilder.ToString();
            return RunQuery(query);
        }

        internal int SaveEDNData(EDNDocument oEDN)
        {
            var timestamp = DateTime.UtcNow;

            using (NpgsqlConnection conn = new NpgsqlConnection(pgdbconnection))
            {
                conn.Open();
                
                string sQuery = "INSERT INTO " + sSchemas + "." + "\"tblEDNDocument\" (\"EdndocsId\", \"SenderRef\", \"Version\", \"Status\", \"EDN\", \"RequestDate\", \"RequestTime\", \"OfficeId\", \"SDateTime\", \"RDateTime\", \"ControlRefNo\", \"Acknowledge\", \"StatusDesc\", \"ReasonDesc\", \"FileInName\", \"FileOutName\", \"Error1\", \"Error2\", \"Error3\", \"Error4\", \"Error5\", \"Error6\", \"Error7\", \"Error8\", \"Error9\", \"Error10\", \"Error11\", \"Error12\", \"Error13\", \"Error14\", \"Error15\", \"Error16\", \"Error17\", \"Error18\", \"Error19\", \"Error20\", \"FileOutContent\", \"FileInContent\") VALUES (:EdndocsId, :SenderRef, :Version, :Status, :EDN, :RequestDate, :RequestTime, :OfficeId, :SDateTime, :RDateTime, :ControlRefNo, :Acknowledge, :StatusDesc, :ReasonDesc, :FileInName, :FileOutName, :Error1, :Error2, :Error3, :Error4, :Error5, :Error6, :Error7, :Error8, :Error9, :Error10, :Error11, :Error12, :Error13, :Error14, :Error15, :Error16, :Error17, :Error18, :Error19, :Error20, :FileOutContent, :FileInContent) RETURNING \"Id\"";

                using (NpgsqlCommand cmd = new NpgsqlCommand(sQuery, conn))
                {
                    DateTime now = DateTime.Now;

                    cmd.Parameters.AddWithValue("EdndocsId", oEDN.EdndocsId);  
                    cmd.Parameters.AddWithValue("SenderRef", oEDN.SenderRef);
                    cmd.Parameters.AddWithValue("Version", oEDN.Version);
                    cmd.Parameters.AddWithValue("Status", oEDN.Status);
                    cmd.Parameters.AddWithValue("EDN", oEDN.EDN);

                    cmd.Parameters.AddWithValue("RequestTime", now);
                    cmd.Parameters.AddWithValue("RequestDate", timestamp);

                    cmd.Parameters.AddWithValue("OfficeId", oEDN.OfficeId);

                    cmd.Parameters.AddWithValue("SDateTime", oEDN.SDateTime);
                    cmd.Parameters.AddWithValue("RDateTime", oEDN.RDateTime);
                    cmd.Parameters.AddWithValue("ControlRefNo", oEDN.ControlRef);
                    cmd.Parameters.AddWithValue("Acknowledge", oEDN.Acknowledge);
                    cmd.Parameters.AddWithValue("StatusDesc", oEDN.StatusDesc);
                    cmd.Parameters.AddWithValue("ReasonDesc", oEDN.ReasonDesc);
                    cmd.Parameters.AddWithValue("FileInName", oEDN.File_In_Name);
                    cmd.Parameters.AddWithValue("FileOutName", oEDN.File_Out_Name);
                    cmd.Parameters.AddWithValue("Error1", oEDN.Error1);
                    cmd.Parameters.AddWithValue("Error2", oEDN.Error2);
                    cmd.Parameters.AddWithValue("Error3", oEDN.Error3);
                    cmd.Parameters.AddWithValue("Error4", oEDN.Error4);
                    cmd.Parameters.AddWithValue("Error5", oEDN.Error5);
                    cmd.Parameters.AddWithValue("Error6", oEDN.Error6);
                    cmd.Parameters.AddWithValue("Error7", oEDN.Error7);
                    cmd.Parameters.AddWithValue("Error8", oEDN.Error8);
                    cmd.Parameters.AddWithValue("Error9", oEDN.Error9);
                    cmd.Parameters.AddWithValue("Error10", oEDN.Error10);
                    cmd.Parameters.AddWithValue("Error11", oEDN.Error11);
                    cmd.Parameters.AddWithValue("Error12", oEDN.Error12);
                    cmd.Parameters.AddWithValue("Error13", oEDN.Error13);
                    cmd.Parameters.AddWithValue("Error14", oEDN.Error14);
                    cmd.Parameters.AddWithValue("Error15", oEDN.Error15);
                    cmd.Parameters.AddWithValue("Error16", oEDN.Error16);
                    cmd.Parameters.AddWithValue("Error17", oEDN.Error17);
                    cmd.Parameters.AddWithValue("Error18", oEDN.Error18);
                    cmd.Parameters.AddWithValue("Error19", oEDN.Error19);
                    cmd.Parameters.AddWithValue("Error20", oEDN.Error20);
                    cmd.Parameters.AddWithValue("FileOutContent", oEDN.File_Out_Content);
                    cmd.Parameters.AddWithValue("FileInContent", oEDN.File_In_Content);
                    //try
                    {
                        Object _result = cmd.ExecuteScalar();

                        if (_result != null && _result != DBNull.Value)
                        {
                            return (int)_result;
                        }
                        return -1;
                    }
                    //catch (Exception oEx)
                    //{
                    //    //ErrorMessage = oEx.Message;

                    //    //oCreateLog.CreateLogFile(LogFilePath, oEx.StackTrace.ToString());

                    //    //oResponseMsg.Status = "Error";
                    //    //oResponseMsg.ErrorMsg = oEx.Message;
                    //    //oResponseMsg.ResponseDateTime = DateTime.Now;
                    //    return -1; // Return -1 if the insertion was unsuccessful
                    //}
                }
            }
        }

        internal int UpdateEDNData(EDNDocument oEDN)
        {
            var timestamp = DateTime.UtcNow;

            using (NpgsqlConnection conn = new NpgsqlConnection(pgdbconnection))
            {
                conn.Open();

                string sQuery = "UPDATE " + sSchemas + ".\"tblEDNDocument\" SET \"SenderRef\"=@SenderRef, \"Status\"=@Status, \"EDN\"=@EDN, \"RequestDate\"=@RequestDate, \"RequestTime\"=@RequestTime, \"SDateTime\"=@SDateTime, \"RDateTime\"=@RDateTime, \"ControlRefNo\"=@ControlRefNo, \"Acknowledge\"=@Acknowledge, \"StatusDesc\"=@StatusDesc, \"ReasonDesc\"=@ReasonDesc, \"FileInName\"=@FileInName, \"FileOutName\"=@FileOutName, \"Error1\"=@Error1, \"Error2\"=@Error2, \"Error3\"=@Error3, \"Error4\"=@Error4, \"Error5\"=@Error5, \"Error6\"=@Error6, \"Error7\"=@Error7, \"Error8\"=@Error8, \"Error9\"=@Error9, \"Error10\"=@Error10, \"Error11\"=@Error11, \"Error12\"=@Error12, \"Error13\"=@Error13, \"Error14\"=@Error14, \"Error15\"=@Error15, \"Error16\"=@Error16, \"Error17\"=@Error17, \"Error18\"=@Error18, \"Error19\"=@Error19, \"Error20\"=@Error20, \"FileOutContent\"=@FileOutContent, \"FileInContent\"=@FileInContent WHERE \"EdndocsId\" = '" + oEDN.EdndocsId + "' and \"Version\"='" + oEDN.Version + "' and \"OfficeId\"='" + oEDN.OfficeId + "'";

                using (NpgsqlCommand cmd = new NpgsqlCommand(sQuery, conn))
                {
                    DateTime now = DateTime.Now;

                    //cmd.Parameters.AddWithValue("EdndocsId", oEDN.EdndocsId);
                    cmd.Parameters.AddWithValue("SenderRef", oEDN.SenderRef);
                    //cmd.Parameters.AddWithValue("Version", oEDN.Version);
                    cmd.Parameters.AddWithValue("Status", oEDN.Status);
                    cmd.Parameters.AddWithValue("EDN", oEDN.EDN);

                    cmd.Parameters.AddWithValue("RequestTime", now);
                    cmd.Parameters.AddWithValue("RequestDate", timestamp);

                    //cmd.Parameters.AddWithValue("OfficeId", oEDN.OfficeId);

                    cmd.Parameters.AddWithValue("SDateTime", oEDN.SDateTime);
                    cmd.Parameters.AddWithValue("RDateTime", oEDN.RDateTime);
                    cmd.Parameters.AddWithValue("ControlRefNo", oEDN.ControlRef);
                    cmd.Parameters.AddWithValue("Acknowledge", oEDN.Acknowledge);
                    cmd.Parameters.AddWithValue("StatusDesc", oEDN.StatusDesc);
                    cmd.Parameters.AddWithValue("ReasonDesc", oEDN.ReasonDesc);
                    cmd.Parameters.AddWithValue("FileInName", oEDN.File_In_Name);
                    cmd.Parameters.AddWithValue("FileOutName", oEDN.File_Out_Name);
                    cmd.Parameters.AddWithValue("Error1", oEDN.Error1);
                    cmd.Parameters.AddWithValue("Error2", oEDN.Error2);
                    cmd.Parameters.AddWithValue("Error3", oEDN.Error3);
                    cmd.Parameters.AddWithValue("Error4", oEDN.Error4);
                    cmd.Parameters.AddWithValue("Error5", oEDN.Error5);
                    cmd.Parameters.AddWithValue("Error6", oEDN.Error6);
                    cmd.Parameters.AddWithValue("Error7", oEDN.Error7);
                    cmd.Parameters.AddWithValue("Error8", oEDN.Error8);
                    cmd.Parameters.AddWithValue("Error9", oEDN.Error9);
                    cmd.Parameters.AddWithValue("Error10", oEDN.Error10);
                    cmd.Parameters.AddWithValue("Error11", oEDN.Error11);
                    cmd.Parameters.AddWithValue("Error12", oEDN.Error12);
                    cmd.Parameters.AddWithValue("Error13", oEDN.Error13);
                    cmd.Parameters.AddWithValue("Error14", oEDN.Error14);
                    cmd.Parameters.AddWithValue("Error15", oEDN.Error15);
                    cmd.Parameters.AddWithValue("Error16", oEDN.Error16);
                    cmd.Parameters.AddWithValue("Error17", oEDN.Error17);
                    cmd.Parameters.AddWithValue("Error18", oEDN.Error18);
                    cmd.Parameters.AddWithValue("Error19", oEDN.Error19);
                    cmd.Parameters.AddWithValue("Error20", oEDN.Error20);
                    cmd.Parameters.AddWithValue("FileOutContent", oEDN.File_Out_Content);
                    cmd.Parameters.AddWithValue("FileInContent", oEDN.File_In_Content);
                    //try
                    {
                        Object _result = cmd.ExecuteNonQuery();

                        if (_result != null && _result != DBNull.Value)
                        {
                            return (int)_result;
                        }
                        return -1;
                    }                    
                }
            }
        }



        internal DataTable GetPRADocumentData()
        {
            return RunQuery("SELECT * FROM " + sSchemas + "." + "\"tblPRADocument\"");
        }
        internal DataTable GetPRADocumentDataWithoutContent()
        {
            return RunQuery("SELECT \"Id\", \"PradocsId\", \"ShippersRef\", \"Version\", \"Status\", \"RequestDate\", \"RequestTime\", \"OfficeId\", \"SDateTime\", \"RDateTime\", \"StatusDesc\", \"FileInName\", \"FileOutName\", \"LastVersion\", \"ContainerNo\", \"OneStopRef\", \"SentType\", \"Error1\", \"Error2\", \"Error3\", \"Error4\", \"Error5\", \"Error6\", \"Error7\", \"Error8\", \"Error9\", \"Error10\", \"Error11\", \"Error12\", \"Error13\", \"Error14\", \"Error15\", \"Error16\", \"Error17\", \"Error18\", \"Error19\", \"Error20\", \"Error21\", \"Error22\", \"Error23\", \"Error24\", \"Error25\", \"Error26\", \"Error27\", \"Error28\", \"Error29\", \"Error30\" FROM " + sSchemas + "." + "\"tblPRADocument\"");
        }

        internal DataTable GetPRADocumentData(string officeId)
        {
            return RunQuery("SELECT * FROM " + sSchemas + "." + "\"tblPRADocument\" where \"OfficeId\" = '" + officeId + "'");
        }
        internal DataTable GetPRADocumentDataWithoutContent(string officeId)
        {
            return RunQuery("SELECT \"Id\", \"PradocsId\", \"ShippersRef\", \"Version\", \"Status\", \"RequestDate\", \"RequestTime\", \"OfficeId\", \"SDateTime\", \"RDateTime\", \"StatusDesc\", \"FileInName\", \"FileOutName\", \"LastVersion\", \"ContainerNo\", \"OneStopRef\", \"SentType\", \"Error1\", \"Error2\", \"Error3\", \"Error4\", \"Error5\", \"Error6\", \"Error7\", \"Error8\", \"Error9\", \"Error10\", \"Error11\", \"Error12\", \"Error13\", \"Error14\", \"Error15\", \"Error16\", \"Error17\", \"Error18\", \"Error19\", \"Error20\", \"Error21\", \"Error22\", \"Error23\", \"Error24\", \"Error25\", \"Error26\", \"Error27\", \"Error28\", \"Error29\", \"Error30\" FROM " + sSchemas + "." + "\"tblPRADocument\" where \"OfficeId\" = '" + officeId + "'");
        }

        internal DataTable GetPRADocumentData(int Id)
        {
            return RunQuery("SELECT * FROM " + sSchemas + "." + "\"tblPRADocument\" where \"Id\" = " + Id);
        }

        internal DataTable IsPRADocumentExist(string PradocsId, int Version, string OfficeId)
        {
            return RunQuery("SELECT * FROM " + sSchemas + "." + "\"tblPRADocument\" where \"PradocsId\" = '" + PradocsId + "' and \"Version\"='" + Version + "' and \"OfficeId\"='" + OfficeId + "'");
        }
        internal DataTable GetPRADataByFilter(DateTime? toDate, DateTime? fromDate, string? OneStopRef, string? ShipperRef)
        {
            StringBuilder queryBuilder = new StringBuilder("SELECT * FROM ");
            queryBuilder.Append(sSchemas).Append(".").Append("\"tblPRADocument\" WHERE 1=1");


            if (toDate == null && fromDate != null)
            {
                toDate = fromDate;
            }

            else if (fromDate == null && toDate != null)
            {
                fromDate = toDate;
            }


            if (toDate != null && fromDate != null)
            {
                DateTime toDateValue = toDate.Value.Date;
                DateTime fromDateValue = fromDate.Value.Date;
                queryBuilder.Append(" AND CAST(\"RequestDate\" AS DATE) BETWEEN '")
                            .Append(fromDateValue.ToString("yyyy-MM-dd"))
                            .Append("' AND '")
                            .Append(toDateValue.ToString("yyyy-MM-dd"))
                            .Append("'");
            }

            
            if (!string.IsNullOrEmpty(OneStopRef))
            {   
                OneStopRef = OneStopRef.Trim();
                queryBuilder.Append(" AND \"OneStopRef\" = '").Append(OneStopRef).Append("'");
            }

            if (!string.IsNullOrEmpty(ShipperRef))
            {   
                ShipperRef = ShipperRef.Trim();
                queryBuilder.Append(" AND \"ShippersRef\" = '").Append(ShipperRef).Append("'");
            }
           
            string query = queryBuilder.ToString();
            return RunQuery(query);
        }

        internal DataTable GetPRADocumentRecordCount(DateTime? toDate, DateTime? fromDate, string? TypeOfQuery, string? Id)
        {
            StringBuilder queryBuilder = new StringBuilder();

            if (TypeOfQuery == "DATE")
            {
                DateTime toDateValue = toDate.Value.Date;
                queryBuilder.Append("SELECT COUNT(*) FROM " + sSchemas + "." + "\"tblPRADocument\" WHERE CAST(\"RequestDate\" AS DATE) = '")
                   .Append(toDateValue.ToString("yyyy-MM-dd"))
                   .Append("'");

            }
            else if (TypeOfQuery == "DATERANGE")
            {
                if (toDate == null && fromDate != null)
                {
                    toDate = fromDate;
                }

                else if (fromDate == null && toDate != null)
                {
                    fromDate = toDate;
                }


                if (toDate != null && fromDate != null)
                {
                    DateTime toDateValue = toDate.Value.Date;
                    DateTime fromDateValue = fromDate.Value.Date;
                    queryBuilder.Append("SELECT COUNT(*) FROM " + sSchemas + "." + "\"tblPRADocument\" WHERE CAST(\"RequestDate\" AS DATE) BETWEEN '")
                                .Append(fromDateValue.ToString("yyyy-MM-dd"))
                                .Append("' AND '")
                                .Append(toDateValue.ToString("yyyy-MM-dd"))
                                .Append("'");
                }
            }
            else if (TypeOfQuery == "OFFICEID")
            {
                Id = Id.Trim();
                queryBuilder.Append("SELECT COUNT(*) FROM ")
                            .Append(sSchemas)
                            .Append(".\"tblPRADocument\" ")
                            .Append("WHERE \"OfficeId\" = '")
                            .Append(Id)
                            .Append("'");

            }
            else
            {
                queryBuilder.Append("SELECT COUNT(*) FROM ")
                            .Append(sSchemas)
                            .Append(".\"tblPRADocument\" ");
            }
            string query = queryBuilder.ToString();
            return RunQuery(query);
        }
        internal int SavePRAData(PRADocument oPRA)
        {
            var timestamp = DateTime.UtcNow;

            using (NpgsqlConnection conn = new NpgsqlConnection(pgdbconnection))
            {
                conn.Open();

                string sQuery = "INSERT INTO " + sSchemas + "." + "\"tblPRADocument\" (\"PradocsId\", \"ShippersRef\", \"Version\", \"Status\", \"RequestDate\", \"RequestTime\", \"OfficeId\", \"SDateTime\", \"RDateTime\", \"StatusDesc\", \"FileInName\", \"FileOutName\", \"LastVersion\", \"ContainerNo\", \"OneStopRef\", \"SentType\", \"Error1\", \"Error2\", \"Error3\", \"Error4\", \"Error5\", \"Error6\", \"Error7\", \"Error8\", \"Error9\", \"Error10\", \"Error11\", \"Error12\", \"Error13\", \"Error14\", \"Error15\", \"Error16\", \"Error17\", \"Error18\", \"Error19\", \"Error20\", \"Error21\", \"Error22\", \"Error23\", \"Error24\", \"Error25\", \"Error26\", \"Error27\", \"Error28\", \"Error29\", \"Error30\", \"FileInContent\", \"FileOutContent\") VALUES (:PradocsId, :ShippersRef, :Version, :Status, :RequestDate, :RequestTime, :OfficeId, :SDateTime, :RDateTime, :StatusDesc, :FileInName, :FileOutName, :LastVersion, :ContainerNo, :OneStopRef, :SentType, :Error1, :Error2, :Error3, :Error4, :Error5, :Error6, :Error7, :Error8, :Error9, :Error10, :Error11, :Error12, :Error13, :Error14, :Error15, :Error16, :Error17, :Error18, :Error19, :Error20, :Error21, :Error22, :Error23, :Error24, :Error25, :Error26, :Error27, :Error28, :Error29, :Error30, :FileInContent, :FileOutContent) RETURNING \"Id\"";
                using (NpgsqlCommand cmd = new NpgsqlCommand(sQuery, conn))
                {
                    DateTime now = DateTime.Now;

                    cmd.Parameters.AddWithValue("PradocsId", oPRA.PradocsId);
                    cmd.Parameters.AddWithValue("ShippersRef", oPRA.ShippersRef);
                    cmd.Parameters.AddWithValue("Version", oPRA.Version);
                    cmd.Parameters.AddWithValue("Status", oPRA.Status);

                    cmd.Parameters.AddWithValue("RequestTime", now);
                    cmd.Parameters.AddWithValue("RequestDate", timestamp);

                    cmd.Parameters.AddWithValue("ContainerNo", oPRA.ContainerNo);
                    cmd.Parameters.AddWithValue("OneStopRef", oPRA.OneStopRef);
                    cmd.Parameters.AddWithValue("OfficeId", oPRA.OfficeId);
                    cmd.Parameters.AddWithValue("SDateTime", oPRA.SDateTime);
                    cmd.Parameters.AddWithValue("RDateTime", oPRA.RDateTime);
                    cmd.Parameters.AddWithValue("StatusDesc", oPRA.StatusDesc);
                    cmd.Parameters.AddWithValue("FileInName", oPRA.File_In_Name);
                    cmd.Parameters.AddWithValue("FileOutName", oPRA.File_Out_Name);
                    cmd.Parameters.AddWithValue("LastVersion", oPRA.LastVersion);
                    cmd.Parameters.AddWithValue("SentType", oPRA.SentType);
                    cmd.Parameters.AddWithValue("Error1", oPRA.Error1);
                    cmd.Parameters.AddWithValue("Error2", oPRA.Error2);
                    cmd.Parameters.AddWithValue("Error3", oPRA.Error3);
                    cmd.Parameters.AddWithValue("Error4", oPRA.Error4);
                    cmd.Parameters.AddWithValue("Error5", oPRA.Error5);
                    cmd.Parameters.AddWithValue("Error6", oPRA.Error6);
                    cmd.Parameters.AddWithValue("Error7", oPRA.Error7);
                    cmd.Parameters.AddWithValue("Error8", oPRA.Error8);
                    cmd.Parameters.AddWithValue("Error9", oPRA.Error9);
                    cmd.Parameters.AddWithValue("Error10", oPRA.Error10);
                    cmd.Parameters.AddWithValue("Error11", oPRA.Error11);
                    cmd.Parameters.AddWithValue("Error12", oPRA.Error12);
                    cmd.Parameters.AddWithValue("Error13", oPRA.Error13);
                    cmd.Parameters.AddWithValue("Error14", oPRA.Error14);
                    cmd.Parameters.AddWithValue("Error15", oPRA.Error15);
                    cmd.Parameters.AddWithValue("Error16", oPRA.Error16);
                    cmd.Parameters.AddWithValue("Error17", oPRA.Error17);
                    cmd.Parameters.AddWithValue("Error18", oPRA.Error18);
                    cmd.Parameters.AddWithValue("Error19", oPRA.Error19);
                    cmd.Parameters.AddWithValue("Error20", oPRA.Error20);
                    cmd.Parameters.AddWithValue("Error21", oPRA.Error21);
                    cmd.Parameters.AddWithValue("Error22", oPRA.Error22);
                    cmd.Parameters.AddWithValue("Error23", oPRA.Error23);
                    cmd.Parameters.AddWithValue("Error24", oPRA.Error24);
                    cmd.Parameters.AddWithValue("Error25", oPRA.Error25);
                    cmd.Parameters.AddWithValue("Error26", oPRA.Error26);
                    cmd.Parameters.AddWithValue("Error27", oPRA.Error27);
                    cmd.Parameters.AddWithValue("Error28", oPRA.Error28);
                    cmd.Parameters.AddWithValue("Error29", oPRA.Error29);
                    cmd.Parameters.AddWithValue("Error30", oPRA.Error30);
                    cmd.Parameters.AddWithValue("FileInContent", oPRA.File_In_Content);
                    cmd.Parameters.AddWithValue("FileOutContent", oPRA.File_Out_Content);

                    //try
                    {
                        Object _result = cmd.ExecuteScalar();

                        if (_result != null && _result != DBNull.Value)
                        {
                            return (int)_result;
                        }
                        return -1;
                    }
                    //catch (Exception oEx)
                    //{
                    //    return -1; // Return -1 if the insertion was unsuccessful
                    //}
                }
            }
        }

        internal int UpdatePRAData(PRADocument oPRA)
        {
            var timestamp = DateTime.UtcNow;

            using (NpgsqlConnection conn = new NpgsqlConnection(pgdbconnection))
            {
                conn.Open();
                string sQuery = "UPDATE " + sSchemas + ".\"tblPRADocument\" SET \"ShippersRef\" = @ShippersRef, \"Status\"= @Status, \"RequestDate\"= @RequestDate, \"RequestTime\"= @RequestTime,\"SDateTime\"= @SDateTime, \"RDateTime\"= @RDateTime, \"StatusDesc\"= @StatusDesc, \"FileInName\"= @FileInName, \"FileOutName\"= @FileOutName, \"LastVersion\"= @LastVersion, \"ContainerNo\"= @ContainerNo, \"OneStopRef\"= @OneStopRef, \"SentType\"= @SentType, \"Error1\"= @Error1, \"Error2\"= @Error2, \"Error3\"= @Error3, \"Error4\"= @Error4, \"Error5\"= @Error5, \"Error6\"= @Error6, \"Error7\"= @Error7, \"Error8\"= @Error8, \"Error9\"= @Error9, \"Error10\"= @Error10, \"Error11\"= @Error11, \"Error12\"= @Error12, \"Error13\"= @Error13, \"Error14\"= @Error14, \"Error15\"= @Error15, \"Error16\"= @Error16, \"Error17\"= @Error17, \"Error18\"= @Error18, \"Error19\"= @Error19, \"Error20\"= @Error20, \"Error21\"= @Error21, \"Error22\"= @Error22, \"Error23\"= @Error23, \"Error24\"= @Error24, \"Error25\"= @Error25, \"Error26\"= @Error26, \"Error27\"= @Error27, \"Error28\"= @Error28, \"Error29\"= @Error29, \"Error30\"= @Error30, \"FileInContent\"=@FileInContent, \"FileOutContent\"=@FileOutContent WHERE \"PradocsId\" = '" + oPRA.PradocsId + "' and \"Version\"='" + oPRA.Version + "' and \"OfficeId\"= '" + oPRA.OfficeId + "'";
                using (NpgsqlCommand cmd = new NpgsqlCommand(sQuery, conn))
                {
                    DateTime now = DateTime.Now;

                    //cmd.Parameters.AddWithValue("PradocsId", oPRA.PradocsId);
                    cmd.Parameters.AddWithValue("ShippersRef", oPRA.ShippersRef);
                    //cmd.Parameters.AddWithValue("Version", oPRA.Version);
                    cmd.Parameters.AddWithValue("Status", oPRA.Status);

                    cmd.Parameters.AddWithValue("RequestTime", now);
                    cmd.Parameters.AddWithValue("RequestDate", timestamp);

                    cmd.Parameters.AddWithValue("ContainerNo", oPRA.ContainerNo);
                    cmd.Parameters.AddWithValue("OneStopRef", oPRA.OneStopRef);
                    //cmd.Parameters.AddWithValue("OfficeId", oPRA.OfficeId);
                    cmd.Parameters.AddWithValue("SDateTime", oPRA.SDateTime);
                    cmd.Parameters.AddWithValue("RDateTime", oPRA.RDateTime);
                    cmd.Parameters.AddWithValue("StatusDesc", oPRA.StatusDesc);
                    cmd.Parameters.AddWithValue("FileInName", oPRA.File_In_Name);
                    cmd.Parameters.AddWithValue("FileOutName", oPRA.File_Out_Name);
                    cmd.Parameters.AddWithValue("LastVersion", oPRA.LastVersion);
                    cmd.Parameters.AddWithValue("SentType", oPRA.SentType);
                    cmd.Parameters.AddWithValue("Error1", oPRA.Error1);
                    cmd.Parameters.AddWithValue("Error2", oPRA.Error2);
                    cmd.Parameters.AddWithValue("Error3", oPRA.Error3);
                    cmd.Parameters.AddWithValue("Error4", oPRA.Error4);
                    cmd.Parameters.AddWithValue("Error5", oPRA.Error5);
                    cmd.Parameters.AddWithValue("Error6", oPRA.Error6);
                    cmd.Parameters.AddWithValue("Error7", oPRA.Error7);
                    cmd.Parameters.AddWithValue("Error8", oPRA.Error8);
                    cmd.Parameters.AddWithValue("Error9", oPRA.Error9);
                    cmd.Parameters.AddWithValue("Error10", oPRA.Error10);
                    cmd.Parameters.AddWithValue("Error11", oPRA.Error11);
                    cmd.Parameters.AddWithValue("Error12", oPRA.Error12);
                    cmd.Parameters.AddWithValue("Error13", oPRA.Error13);
                    cmd.Parameters.AddWithValue("Error14", oPRA.Error14);
                    cmd.Parameters.AddWithValue("Error15", oPRA.Error15);
                    cmd.Parameters.AddWithValue("Error16", oPRA.Error16);
                    cmd.Parameters.AddWithValue("Error17", oPRA.Error17);
                    cmd.Parameters.AddWithValue("Error18", oPRA.Error18);
                    cmd.Parameters.AddWithValue("Error19", oPRA.Error19);
                    cmd.Parameters.AddWithValue("Error20", oPRA.Error20);
                    cmd.Parameters.AddWithValue("Error21", oPRA.Error21);
                    cmd.Parameters.AddWithValue("Error22", oPRA.Error22);
                    cmd.Parameters.AddWithValue("Error23", oPRA.Error23);
                    cmd.Parameters.AddWithValue("Error24", oPRA.Error24);
                    cmd.Parameters.AddWithValue("Error25", oPRA.Error25);
                    cmd.Parameters.AddWithValue("Error26", oPRA.Error26);
                    cmd.Parameters.AddWithValue("Error27", oPRA.Error27);
                    cmd.Parameters.AddWithValue("Error28", oPRA.Error28);
                    cmd.Parameters.AddWithValue("Error29", oPRA.Error29);
                    cmd.Parameters.AddWithValue("Error30", oPRA.Error30);
                    cmd.Parameters.AddWithValue("FileInContent", oPRA.File_In_Content);
                    cmd.Parameters.AddWithValue("FileOutContent", oPRA.File_Out_Content);

                    //try
                    {
                        Object _result = cmd.ExecuteNonQuery();

                        if (_result != null && _result != DBNull.Value)
                        {
                            return (int)_result;
                        }
                        return -1;
                    }
                }
            }
        }


        internal DataTable GetFIDocumentData()
        {
            return RunQuery("SELECT * FROM " + sSchemas + "." + "\"tblFIDocument\"");
        }

        internal DataTable GetFIDocumentDataWithoutContent()
        {
            return RunQuery("SELECT \"Id\", \"FidocsId\", \"Version\", \"Status\", \"RequestDate\", \"RequestTime\", \"OfficeId\", \"SDateTime\", \"RDateTime\", \"ControlAck\", \"Flag\", \"Temp\", \"FileInName\", \"FileOutName\", \"StatusDesc\", \"ControlRef\", \"FiRef\", \"BillRef\", \"IsLastLine\", \"Info\", \"Error1\", \"Error2\", \"Error3\", \"Error4\", \"Error5\", \"Error6\", \"Error7\", \"Error8\", \"Error9\", \"Error10\", \"Error11\", \"Error12\", \"Error13\", \"Error14\", \"Error15\", \"Error16\", \"Error17\", \"Error18\", \"Error19\", \"Error20\" FROM " + sSchemas + "." + "\"tblFIDocument\"");
        }

        internal DataTable GetFIDocumentData(string officeId)
        {
            return RunQuery("SELECT * FROM " + sSchemas + "." + "\"tblFIDocument\" where \"OfficeId\" = '" + officeId + "'");
        }

        internal DataTable GetFIDocumentDataWithoutContent(string officeId)
        {
            return RunQuery("SELECT \"Id\", \"FidocsId\", \"Version\", \"Status\", \"RequestDate\", \"RequestTime\", \"OfficeId\", \"SDateTime\", \"RDateTime\", \"ControlAck\", \"Flag\", \"Temp\", \"FileInName\", \"FileOutName\", \"StatusDesc\", \"ControlRef\", \"FiRef\", \"BillRef\", \"IsLastLine\", \"Info\", \"Error1\", \"Error2\", \"Error3\", \"Error4\", \"Error5\", \"Error6\", \"Error7\", \"Error8\", \"Error9\", \"Error10\", \"Error11\", \"Error12\", \"Error13\", \"Error14\", \"Error15\", \"Error16\", \"Error17\", \"Error18\", \"Error19\", \"Error20\" FROM " + sSchemas + "." + "\"tblFIDocument\" where \"OfficeId\" = '" + officeId + "'");
        }

        internal DataTable GetFIDocumentData(int Id)
        {
            return RunQuery("SELECT * FROM " + sSchemas + "." + "\"tblFIDocument\" where \"Id\" = '" + Id + "'");
        }

        internal DataTable IsFIDocumentExist(string FIdocsId, int Version, string OfficeId)
        {
            return RunQuery("SELECT * FROM " + sSchemas + "." + "\"tblFIDocument\" where \"FidocsId\" = '" + FIdocsId + "' and \"Version\"='" + Version + "' and \"OfficeId\"='" + OfficeId + "'");
        }

        internal int SaveFIData(FIDocument oFI)
        {
            var timestamp = DateTime.UtcNow;

            using (NpgsqlConnection conn = new NpgsqlConnection(pgdbconnection))
            {
                conn.Open();

                //string sQuery = "INSERT INTO " + sSchemas + "." + "\"tblFIDocument\" (\"FidocsId\", \"Version\", \"Status\", \"RequestDate\", \"RequestTime\", \"OfficeId\", \"SDateTime\", \"RDateTime\", \"ControlAck\", \"Flag\", \"Temp\", \"FileInName\", \"FileOutName\", \"StatusDesc\", \"ControlRef\", \"FiRef\", \"BillRef\", \"IsLastLine\", \"Info\", \"Error1\", \"Error2\", \"Error3\", \"Error4\", \"Error5\", \"Error6\", \"Error7\", \"Error8\", \"Error9\", \"Error10\", \"Error11\", \"Error12\", \"Error13\", \"Error14\", \"Error15\", \"Error16\", \"Error17\", \"Error18\", \"Error19\", \"Error20\") VALUES (:FidocsId, :Version, :Status, :RequestDate, :RequestTime, :OfficeId, :SDateTime, :RDateTime, :ControlAck, :Flag, :Temp, :FileInName, :FileOutName, :StatusDesc, :ControlRef, :FiRef, :BillRef, :IsLastLine, :Info, :Error1, :Error2, :Error3, :Error4, :Error5, :Error6, :Error7, :Error8, :Error9, :Error10, :Error11, :Error12, :Error13, :Error14, :Error15, :Error16, :Error17, :Error18, :Error19, :Error20) RETURNING \"Id\"";
                string sQuery = "INSERT INTO " + sSchemas + "." + "\"tblFIDocument\" (\"FidocsId\", \"Version\", \"Status\", \"RequestDate\", \"RequestTime\", \"OfficeId\", \"SDateTime\", \"RDateTime\", \"ControlAck\", \"Flag\", \"Temp\", \"FileInName\", \"FileOutName\", \"StatusDesc\", \"ControlRef\", \"FiRef\", \"BillRef\", \"IsLastLine\", \"Info\", \"Error1\", \"Error2\", \"Error3\", \"Error4\", \"Error5\", \"Error6\", \"Error7\", \"Error8\", \"Error9\", \"Error10\", \"Error11\", \"Error12\", \"Error13\", \"Error14\", \"Error15\", \"Error16\", \"Error17\", \"Error18\", \"Error19\", \"Error20\", \"FileInContent\", \"FileOutContent\") VALUES (:FidocsId, :Version, :Status, :RequestDate, :RequestTime, :OfficeId, :SDateTime, :RDateTime, :ControlAck, :Flag, :Temp, :FileInName, :FileOutName, :StatusDesc, :ControlRef, :FiRef, :BillRef, :IsLastLine, :Info, :Error1, :Error2, :Error3, :Error4, :Error5, :Error6, :Error7, :Error8, :Error9, :Error10, :Error11, :Error12, :Error13, :Error14, :Error15, :Error16, :Error17, :Error18, :Error19, :Error20, :FileInContent, :FileOutContent) RETURNING \"Id\"";
                using (NpgsqlCommand cmd = new NpgsqlCommand(sQuery, conn))
                {
                    DateTime now = DateTime.Now;

                    cmd.Parameters.AddWithValue("FidocsId", oFI.FidocsId);
                    cmd.Parameters.AddWithValue("Version", oFI.Version);
                    cmd.Parameters.AddWithValue("Status", oFI.Status);

                    cmd.Parameters.AddWithValue("RequestTime", now);
                    cmd.Parameters.AddWithValue("RequestDate", timestamp);

                    cmd.Parameters.AddWithValue("OfficeId", oFI.OfficeId);
                    cmd.Parameters.AddWithValue("SDateTime", oFI.SDateTime);
                    cmd.Parameters.AddWithValue("RDateTime", oFI.RDateTime);
                    cmd.Parameters.AddWithValue("ControlAck", oFI.ControlAck);
                    cmd.Parameters.AddWithValue("Flag", oFI.Flag);
                    cmd.Parameters.AddWithValue("Temp", oFI.Temp);
                    cmd.Parameters.AddWithValue("StatusDesc", oFI.StatusDesc);
                    cmd.Parameters.AddWithValue("ControlRef", oFI.ControlRef);
                    cmd.Parameters.AddWithValue("FiRef", oFI.FiRef);
                    cmd.Parameters.AddWithValue("BillRef", oFI.BillRef);
                    cmd.Parameters.AddWithValue("IsLastLine", oFI.IsLastLine);
                    cmd.Parameters.AddWithValue("Info", oFI.Info);
                    cmd.Parameters.AddWithValue("Error1", oFI.Error1);
                    cmd.Parameters.AddWithValue("Error2", oFI.Error2);
                    cmd.Parameters.AddWithValue("Error3", oFI.Error3);
                    cmd.Parameters.AddWithValue("Error4", oFI.Error4);
                    cmd.Parameters.AddWithValue("Error5", oFI.Error5);
                    cmd.Parameters.AddWithValue("Error6", oFI.Error6);
                    cmd.Parameters.AddWithValue("Error7", oFI.Error7);
                    cmd.Parameters.AddWithValue("Error8", oFI.Error8);
                    cmd.Parameters.AddWithValue("Error9", oFI.Error9);
                    cmd.Parameters.AddWithValue("Error10", oFI.Error10);
                    cmd.Parameters.AddWithValue("Error11", oFI.Error11);
                    cmd.Parameters.AddWithValue("Error12", oFI.Error12);
                    cmd.Parameters.AddWithValue("Error13", oFI.Error13);
                    cmd.Parameters.AddWithValue("Error14", oFI.Error14);
                    cmd.Parameters.AddWithValue("Error15", oFI.Error15);
                    cmd.Parameters.AddWithValue("Error16", oFI.Error16);
                    cmd.Parameters.AddWithValue("Error17", oFI.Error17);
                    cmd.Parameters.AddWithValue("Error18", oFI.Error18);
                    cmd.Parameters.AddWithValue("Error19", oFI.Error19);
                    cmd.Parameters.AddWithValue("Error20", oFI.Error20);
                    cmd.Parameters.AddWithValue("FileInName", oFI.File_In_Name);
                    cmd.Parameters.AddWithValue("FileInContent", oFI.File_In_Content);
                    cmd.Parameters.AddWithValue("FileOutName", oFI.File_Out_Name);
                    cmd.Parameters.AddWithValue("FileOutContent", oFI.File_Out_Content);

                    //try
                    {
                        Object _result = cmd.ExecuteScalar();

                        if (_result != null && _result != DBNull.Value)
                        {
                            return (int)_result;
                        }
                        return -1;
                    }
                    //catch (Exception oEx)
                    //{
                    //    return -1; // Return -1 if the insertion was unsuccessful
                    //}
                }
            }
        }

        internal int UpdateFIData(FIDocument oFI)
        {
            var timestamp = DateTime.UtcNow;

            using (NpgsqlConnection conn = new NpgsqlConnection(pgdbconnection))
            {
                conn.Open();

               string sQuery = "UPDATE " + sSchemas + "." + "\"tblFIDocument\" SET \"Status\"=@Status, \"RequestDate\"=@RequestDate, \"RequestTime\"=@RequestTime, \"SDateTime\"=@SDateTime, \"RDateTime\"=@RDateTime, \"ControlAck\"=@ControlAck, \"Flag\"=@Flag, \"Temp\"=@Temp, \"FileInName\"=@FileInName, \"FileOutName\"=@FileOutName, \"StatusDesc\"=@StatusDesc, \"ControlRef\"=@ControlRef, \"FiRef\"=@FiRef, \"BillRef\"=@BillRef, \"IsLastLine\"=@IsLastLine, \"Info\"=@Info, \"Error1\"=@Error1, \"Error2\"=@Error2, \"Error3\"=@Error3, \"Error4\"=@Error4, \"Error5\"=@Error5, \"Error6\"=@Error6, \"Error7\"=@Error7, \"Error8\"=@Error8, \"Error9\"=@Error9, \"Error10\"=@Error10, \"Error11\"=@Error11, \"Error12\"=@Error12, \"Error13\"=@Error13, \"Error14\"=@Error14, \"Error15\"=@Error15, \"Error16\"=@Error16, \"Error17\"=@Error17, \"Error18\"=@Error18, \"Error19\"=@Error19, \"Error20\"=@Error20, \"FileInContent\"=@FileInContent, \"FileOutContent\"=@FileOutContent WHERE \"FidocsId\" ='" + oFI.FidocsId+"' AND \"Version\"= '"+oFI.Version+"' AND \"OfficeId\"= '"+oFI.OfficeId+"'";
                using (NpgsqlCommand cmd = new NpgsqlCommand(sQuery, conn))
                {
                    DateTime now = DateTime.Now;

                    //cmd.Parameters.AddWithValue("FidocsId", oFI.FidocsId);
                    //cmd.Parameters.AddWithValue("Version", oFI.Version);
                    cmd.Parameters.AddWithValue("Status", oFI.Status);

                    cmd.Parameters.AddWithValue("RequestTime", now);
                    cmd.Parameters.AddWithValue("RequestDate", timestamp);

                    //cmd.Parameters.AddWithValue("OfficeId", oFI.OfficeId);
                    cmd.Parameters.AddWithValue("SDateTime", oFI.SDateTime);
                    cmd.Parameters.AddWithValue("RDateTime", oFI.RDateTime);
                    cmd.Parameters.AddWithValue("ControlAck", oFI.ControlAck);
                    cmd.Parameters.AddWithValue("Flag", oFI.Flag);
                    cmd.Parameters.AddWithValue("Temp", oFI.Temp);
                    cmd.Parameters.AddWithValue("StatusDesc", oFI.StatusDesc);
                    cmd.Parameters.AddWithValue("ControlRef", oFI.ControlRef);
                    cmd.Parameters.AddWithValue("FiRef", oFI.FiRef);
                    cmd.Parameters.AddWithValue("BillRef", oFI.BillRef);
                    cmd.Parameters.AddWithValue("IsLastLine", oFI.IsLastLine);
                    cmd.Parameters.AddWithValue("Info", oFI.Info);
                    cmd.Parameters.AddWithValue("Error1", oFI.Error1);
                    cmd.Parameters.AddWithValue("Error2", oFI.Error2);
                    cmd.Parameters.AddWithValue("Error3", oFI.Error3);
                    cmd.Parameters.AddWithValue("Error4", oFI.Error4);
                    cmd.Parameters.AddWithValue("Error5", oFI.Error5);
                    cmd.Parameters.AddWithValue("Error6", oFI.Error6);
                    cmd.Parameters.AddWithValue("Error7", oFI.Error7);
                    cmd.Parameters.AddWithValue("Error8", oFI.Error8);
                    cmd.Parameters.AddWithValue("Error9", oFI.Error9);
                    cmd.Parameters.AddWithValue("Error10", oFI.Error10);
                    cmd.Parameters.AddWithValue("Error11", oFI.Error11);
                    cmd.Parameters.AddWithValue("Error12", oFI.Error12);
                    cmd.Parameters.AddWithValue("Error13", oFI.Error13);
                    cmd.Parameters.AddWithValue("Error14", oFI.Error14);
                    cmd.Parameters.AddWithValue("Error15", oFI.Error15);
                    cmd.Parameters.AddWithValue("Error16", oFI.Error16);
                    cmd.Parameters.AddWithValue("Error17", oFI.Error17);
                    cmd.Parameters.AddWithValue("Error18", oFI.Error18);
                    cmd.Parameters.AddWithValue("Error19", oFI.Error19);
                    cmd.Parameters.AddWithValue("Error20", oFI.Error20);
                    cmd.Parameters.AddWithValue("FileInName", oFI.File_In_Name);
                    cmd.Parameters.AddWithValue("FileInContent", oFI.File_In_Content);
                    cmd.Parameters.AddWithValue("FileOutName", oFI.File_Out_Name);
                    cmd.Parameters.AddWithValue("FileOutContent", oFI.File_Out_Content);
                    //try
                    {
                        Object _result = cmd.ExecuteNonQuery();

                        if (_result != null && _result != DBNull.Value)
                        {
                            return (int)_result;
                        }
                        return -1;
                    }
                }
            }
        }

        internal DataTable GetAQISDocumentData()
        {
            return RunQuery("SELECT * FROM " + sSchemas + "." + "\"tblAQISDocument\"");
        }
        internal DataTable GetAQISDocumentDataWithoutContent()
        {
            return RunQuery("SELECT \"Id\", \"AQISId\", \"Version\", \"Status\", \"RequestDate\", \"RequestTime\", \"OfficeId\", \"SDateTime\", \"RDateTime\", \"StatusDesc\", \"Noin\", \"Permit\", \"FileInName\", \"FileOutName\", \"Error1\", \"Error2\", \"Error3\", \"Error4\", \"Error5\", \"Error6\", \"Error7\", \"Error8\", \"Error9\", \"Error10\", \"Error11\", \"Error12\", \"Error13\", \"Error14\", \"Error15\", \"Error16\", \"Error17\", \"Error18\", \"Error19\", \"Error20\", \"Error21\", \"Error22\", \"Error23\", \"Error24\", \"Error25\", \"Error26\", \"Error27\", \"Error28\", \"Error29\", \"Error30\", \"Error31\", \"Error32\", \"Error33\", \"Error34\", \"Error35\", \"Error36\", \"Error37\", \"Error38\", \"Error39\", \"Error40\", \"Test\", \"ContPage\", \"ECN\", \"Spare\" FROM " + sSchemas + "." + "\"tblAQISDocument\"");
        }
        internal DataTable GetAQISDocumentData(string officeId)
        {
            return RunQuery("SELECT * FROM " + sSchemas + "." + "\"tblAQISDocument\" where \"OfficeId\" = '" + officeId + "'");
        }
        internal DataTable GetAQISDocumentDataWithoutContent(string officeId)
        {
            return RunQuery("SELECT \"Id\", \"AQISId\", \"Version\", \"Status\", \"RequestDate\", \"RequestTime\", \"OfficeId\", \"SDateTime\", \"RDateTime\", \"StatusDesc\", \"Noin\", \"Permit\", \"FileInName\", \"FileOutName\", \"Error1\", \"Error2\", \"Error3\", \"Error4\", \"Error5\", \"Error6\", \"Error7\", \"Error8\", \"Error9\", \"Error10\", \"Error11\", \"Error12\", \"Error13\", \"Error14\", \"Error15\", \"Error16\", \"Error17\", \"Error18\", \"Error19\", \"Error20\", \"Error21\", \"Error22\", \"Error23\", \"Error24\", \"Error25\", \"Error26\", \"Error27\", \"Error28\", \"Error29\", \"Error30\", \"Error31\", \"Error32\", \"Error33\", \"Error34\", \"Error35\", \"Error36\", \"Error37\", \"Error38\", \"Error39\", \"Error40\", \"Test\", \"ContPage\", \"ECN\", \"Spare\" FROM " + sSchemas + "." + "\"tblAQISDocument\" where \"OfficeId\" = '" + officeId + "'");
        }

        internal DataTable GetAQISDocumentData(int Id)
        {
            return RunQuery("SELECT * FROM " + sSchemas + "." + "\"tblAQISDocument\" where \"Id\" = " + Id);
        }

        internal DataTable IsAQISDocumentExist(string AQISId, int Version, string OfficeId)
        {
            return RunQuery("SELECT * FROM " + sSchemas + "." + "\"tblAQISDocument\" where \"AQISId\" = '" + AQISId + "' and \"Version\"='" + Version + "' and \"OfficeId\"='" + OfficeId + "'");
        }
        internal DataTable GetAQISDataByFilter(DateTime? toDate, DateTime? fromDate, string? AQISId, string? RFPNo)
        {
            StringBuilder queryBuilder = new StringBuilder("SELECT * FROM ");
            queryBuilder.Append(sSchemas).Append(".").Append("\"tblAQISDocument\" WHERE 1=1");


            if (toDate == null && fromDate != null)
            {
                toDate = fromDate;
            }

            else if (fromDate == null && toDate != null)
            {
                fromDate = toDate;
            }


            if (toDate != null && fromDate != null)
            {
                DateTime toDateValue = toDate.Value.Date;
                DateTime fromDateValue = fromDate.Value.Date;
                queryBuilder.Append(" AND CAST(\"RequestDate\" AS DATE) BETWEEN '")
                            .Append(fromDateValue.ToString("yyyy-MM-dd"))
                            .Append("' AND '")
                            .Append(toDateValue.ToString("yyyy-MM-dd"))
                            .Append("'");
            }

           
            if (!string.IsNullOrEmpty(AQISId))
            {
                AQISId=AQISId.Trim();
                queryBuilder.Append(" AND \"AQISId\" = '").Append(AQISId).Append("'");
            }
            

            if (!string.IsNullOrEmpty(RFPNo))
            {
                RFPNo=RFPNo.Trim(); 
                queryBuilder.Append(" AND \"Noin\" = '").Append(RFPNo).Append("'");
            }
          
            string query = queryBuilder.ToString();
            return RunQuery(query);
        }

        internal DataTable GetAQISDocumentRecordCount(DateTime? toDate, DateTime? fromDate, string? TypeOfQuery, string? Id)
        {
            StringBuilder queryBuilder = new StringBuilder();

            if (TypeOfQuery == "DATE")
            {
                DateTime toDateValue = toDate.Value.Date;
                queryBuilder.Append("SELECT COUNT(*) FROM " + sSchemas + "." + "\"tblAQISDocument\" WHERE CAST(\"RequestDate\" AS DATE) = '")
                   .Append(toDateValue.ToString("yyyy-MM-dd"))
                   .Append("'");

            }
            else if (TypeOfQuery == "DATERANGE")
            {
                if (toDate == null && fromDate != null)
                {
                    toDate = fromDate;
                }

                else if (fromDate == null && toDate != null)
                {
                    fromDate = toDate;
                }


                if (toDate != null && fromDate != null)
                {
                    DateTime toDateValue = toDate.Value.Date;
                    DateTime fromDateValue = fromDate.Value.Date;
                    queryBuilder.Append("SELECT COUNT(*) FROM " + sSchemas + "." + "\"tblAQISDocument\" WHERE CAST(\"RequestDate\" AS DATE) BETWEEN '")
                                .Append(fromDateValue.ToString("yyyy-MM-dd"))
                                .Append("' AND '")
                                .Append(toDateValue.ToString("yyyy-MM-dd"))
                                .Append("'");
                }
            }
            else if (TypeOfQuery == "OFFICEID")
            {
                Id = Id.Trim();
                queryBuilder.Append("SELECT COUNT(*) FROM ")
                            .Append(sSchemas)
                            .Append(".\"tblAQISDocument\" ")
                            .Append("WHERE \"OfficeId\" = '")
                            .Append(Id)
                            .Append("'");

            }
            else
            {
                queryBuilder.Append("SELECT COUNT(*) FROM ")
                            .Append(sSchemas)
                            .Append(".\"tblAQISDocument\" ");
            }
            string query = queryBuilder.ToString();
            return RunQuery(query);
        }
        internal int SaveAQISData(AQISDocument oAQIS)
        {
            var timestamp = DateTime.UtcNow;

            using (NpgsqlConnection conn = new NpgsqlConnection(pgdbconnection))
            {
                conn.Open();

                string sQuery = "INSERT INTO " + sSchemas + "." + "\"tblAQISDocument\" (\"AQISId\", \"Version\", \"Status\", \"RequestDate\", \"RequestTime\", \"OfficeId\", \"SDateTime\", \"RDateTime\", \"StatusDesc\", \"Noin\", \"Permit\", \"FileInName\", \"FileOutName\", \"Error1\", \"Error2\", \"Error3\", \"Error4\", \"Error5\", \"Error6\", \"Error7\", \"Error8\", \"Error9\", \"Error10\", \"Error11\", \"Error12\", \"Error13\", \"Error14\", \"Error15\", \"Error16\", \"Error17\", \"Error18\", \"Error19\", \"Error20\", \"Error21\", \"Error22\", \"Error23\", \"Error24\", \"Error25\", \"Error26\", \"Error27\", \"Error28\", \"Error29\", \"Error30\", \"Error31\", \"Error32\", \"Error33\", \"Error34\", \"Error35\", \"Error36\", \"Error37\", \"Error38\", \"Error39\", \"Error40\", \"Test\", \"ContPage\", \"ECN\", \"Spare\", \"FileInContent\", \"FileOutContent\") VALUES (:AQISId, :Version, :Status, :RequestDate, :RequestTime, :OfficeId, :SDateTime, :RDateTime, :StatusDesc, :Noin, :Permit, :FileInName, :FileOutName, :Error1, :Error2, :Error3, :Error4, :Error5, :Error6, :Error7, :Error8, :Error9, :Error10, :Error11, :Error12, :Error13, :Error14, :Error15, :Error16, :Error17, :Error18, :Error19, :Error20, :Error21, :Error22, :Error23, :Error24, :Error25, :Error26, :Error27, :Error28, :Error29, :Error30, :Error31, :Error32, :Error33, :Error34, :Error35, :Error36, :Error37, :Error38, :Error39, :Error40, :Test, :ContPage, :ECN, :Spare, :FileInContent, :FileOutContent) RETURNING \"Id\"";

                //string sQuery = "INSERT INTO " + sSchemas + "." + "\"tblAQISDocument\" (\"AQISId\", \"Version\", \"Status\", \"RequestDate\", \"RequestTime\", \"OfficeId\", \"SDateTime\", \"RDateTime\", \"StatusDesc\", \"Noin\", \"Permit\", \"FileInName\", \"FileOutName\", \"Error1\", \"Error2\", \"Error3\", \"Error4\", \"Error5\", \"Error6\", \"Error7\", \"Error8\", \"Error9\", \"Error10\") VALUES (:AQISId, :Version, :Status, :RequestDate, :RequestTime, :OfficeId, :SDateTime, :RDateTime, :StatusDesc, :Noin, :Permit, :FileInName, :FileOutName, :Error1, :Error2, :Error3, :Error4, :Error5, :Error6, :Error7, :Error8, :Error9, :Error10) RETURNING \"Id\"";


                using (NpgsqlCommand cmd = new NpgsqlCommand(sQuery, conn))
                {
                    DateTime now = DateTime.Now;

                    cmd.Parameters.AddWithValue("AQISId", oAQIS.AQISId);
   
                    cmd.Parameters.AddWithValue("Version", oAQIS.Version);
                    cmd.Parameters.AddWithValue("Status", oAQIS.Status);  
                    cmd.Parameters.AddWithValue("RequestTime", now);
                    cmd.Parameters.AddWithValue("RequestDate", timestamp);
                    cmd.Parameters.AddWithValue("OfficeId", oAQIS.OfficeId);
                    cmd.Parameters.AddWithValue("SDateTime", oAQIS.SDateTime);
                    cmd.Parameters.AddWithValue("RDateTime", oAQIS.RDateTime);
                    cmd.Parameters.AddWithValue("StatusDesc", oAQIS.StatusDesc);
                    cmd.Parameters.AddWithValue("Noin", oAQIS.RFPNo);
                    cmd.Parameters.AddWithValue("Permit", oAQIS.PermitNo);

                    cmd.Parameters.AddWithValue("FileInName", oAQIS.File_In_Name);
                    cmd.Parameters.AddWithValue("FileOutName", oAQIS.File_Out_Name);

                    cmd.Parameters.AddWithValue("Error1", oAQIS.Error1);
                    cmd.Parameters.AddWithValue("Error2", oAQIS.Error2);
                    cmd.Parameters.AddWithValue("Error3", oAQIS.Error3);
                    cmd.Parameters.AddWithValue("Error4", oAQIS.Error4);
                    cmd.Parameters.AddWithValue("Error5", oAQIS.Error5);
                    cmd.Parameters.AddWithValue("Error6", oAQIS.Error6);
                    cmd.Parameters.AddWithValue("Error7", oAQIS.Error7);
                    cmd.Parameters.AddWithValue("Error8", oAQIS.Error8);
                    cmd.Parameters.AddWithValue("Error9", oAQIS.Error9);
                    cmd.Parameters.AddWithValue("Error10", oAQIS.Error10);
                    cmd.Parameters.AddWithValue("Error11", oAQIS.Error11);
                    cmd.Parameters.AddWithValue("Error12", oAQIS.Error12);
                    cmd.Parameters.AddWithValue("Error13", oAQIS.Error13);
                    cmd.Parameters.AddWithValue("Error14", oAQIS.Error14);
                    cmd.Parameters.AddWithValue("Error15", oAQIS.Error15);
                    cmd.Parameters.AddWithValue("Error16", oAQIS.Error16);
                    cmd.Parameters.AddWithValue("Error17", oAQIS.Error17);
                    cmd.Parameters.AddWithValue("Error18", oAQIS.Error18);
                    cmd.Parameters.AddWithValue("Error19", oAQIS.Error19);
                    cmd.Parameters.AddWithValue("Error20", oAQIS.Error20);

                    cmd.Parameters.AddWithValue("Error21", oAQIS.Error21);
                    cmd.Parameters.AddWithValue("Error22", oAQIS.Error22);
                    cmd.Parameters.AddWithValue("Error23", oAQIS.Error23);
                    cmd.Parameters.AddWithValue("Error24", oAQIS.Error24);
                    cmd.Parameters.AddWithValue("Error25", oAQIS.Error25);
                    cmd.Parameters.AddWithValue("Error26", oAQIS.Error26);
                    cmd.Parameters.AddWithValue("Error27", oAQIS.Error27);
                    cmd.Parameters.AddWithValue("Error28", oAQIS.Error28);
                    cmd.Parameters.AddWithValue("Error29", oAQIS.Error29);
                    cmd.Parameters.AddWithValue("Error30", oAQIS.Error30);

                    cmd.Parameters.AddWithValue("Error31", oAQIS.Error31);
                    cmd.Parameters.AddWithValue("Error32", oAQIS.Error32);
                    cmd.Parameters.AddWithValue("Error33", oAQIS.Error33);
                    cmd.Parameters.AddWithValue("Error34", oAQIS.Error34);
                    cmd.Parameters.AddWithValue("Error35", oAQIS.Error35);
                    cmd.Parameters.AddWithValue("Error36", oAQIS.Error36);
                    cmd.Parameters.AddWithValue("Error37", oAQIS.Error37);
                    cmd.Parameters.AddWithValue("Error38", oAQIS.Error38);
                    cmd.Parameters.AddWithValue("Error39", oAQIS.Error39);
                    cmd.Parameters.AddWithValue("Error40", oAQIS.Error40);

                    cmd.Parameters.AddWithValue("Test", oAQIS.Test);
                    cmd.Parameters.AddWithValue("ContPage", oAQIS.ContPage);
                    cmd.Parameters.AddWithValue("ECN", oAQIS.ECN);
                    cmd.Parameters.AddWithValue("Spare", oAQIS.Spare);
                    cmd.Parameters.AddWithValue("FileInContent", oAQIS.File_In_Content);
                    cmd.Parameters.AddWithValue("FileOutContent", oAQIS.File_Out_Content);

                    ////try
                    {
                        Object _result = cmd.ExecuteScalar();

                        if (_result != null && _result != DBNull.Value)
                        {
                            return (int)_result;
                        }
                        return -1;
                    }
                    //catch (Exception oEx)
                    //{
                    //    return -1; // Return -1 if the insertion was unsuccessful
                    //}
                }
            }
        }

        internal int UpdateAQISData(AQISDocument oAQIS)
        {
            var timestamp = DateTime.UtcNow;

            using (NpgsqlConnection conn = new NpgsqlConnection(pgdbconnection))
            {
                conn.Open();

                string sQuery = "UPDATE " + sSchemas + ".\"tblAQISDocument\" SET \"AQISId\"=@AQISId, \"Version\"=@Version, \"Status\"=@Status, \"RequestDate\"=@RequestDate, \"RequestTime\"=@RequestTime, \"OfficeId\"=@OfficeId, \"SDateTime\"=@SDateTime, \"RDateTime\"=@RDateTime, \"StatusDesc\"=@StatusDesc, \"Noin\"=@Noin, \"Permit\"=@Permit, \"FileInName\"=@FileInName, \"FileOutName\"=@FileOutName, \"Error1\"=@Error1, \"Error2\"=@Error2, \"Error3\"=@Error3, \"Error4\"=@Error4, \"Error5\"=@Error5, \"Error6\"=@Error6, \"Error7\"=@Error7, \"Error8\"=@Error8, \"Error9\"=@Error9, \"Error10\"=@Error10, \"Error11\"=@Error11, \"Error12\"=@Error12, \"Error13\"=@Error13, \"Error14\"=@Error14, \"Error15\"=@Error15, \"Error16\"=@Error16, \"Error17\"=@Error17, \"Error18\"=@Error18, \"Error19\"=@Error19, \"Error20\"=@Error20, \"Error21\"=@Error21, \"Error22\"=@Error22, \"Error23\"=@Error23, \"Error24\"=@Error24, \"Error25\"=@Error25, \"Error26\"=@Error26, \"Error27\"=@Error27, \"Error28\"=@Error28, \"Error29\"=@Error29, \"Error30\"=@Error30, \"Error31\"=@Error31, \"Error32\"=@Error32, \"Error33\"=@Error33, \"Error34\"=@Error34, \"Error35\"=@Error35, \"Error36\"=@Error36, \"Error37\"=@Error37, \"Error38\"=@Error38, \"Error39\"=@Error39, \"Error40\"=@Error40, \"Test\"=@Test, \"ContPage\"=@ContPage, \"ECN\"=@ECN, \"Spare\"=@Spare, \"FileInContent\"=@FileInContent, \"FileOutContent\"=@FileOutContent WHERE \"AQISId\" = '" + oAQIS.AQISId + "' and \"Version\"='" + oAQIS.Version + "' and \"OfficeId\"='" + oAQIS.OfficeId + "'";

                using (NpgsqlCommand cmd = new NpgsqlCommand(sQuery, conn))
                {
                    DateTime now = DateTime.Now;

                    cmd.Parameters.AddWithValue("AQISId", oAQIS.AQISId);

                    cmd.Parameters.AddWithValue("Version", oAQIS.Version);
                    cmd.Parameters.AddWithValue("Status", oAQIS.Status);
                    cmd.Parameters.AddWithValue("RequestTime", now);
                    cmd.Parameters.AddWithValue("RequestDate", timestamp);
                    cmd.Parameters.AddWithValue("OfficeId", oAQIS.OfficeId);
                    cmd.Parameters.AddWithValue("SDateTime", oAQIS.SDateTime);
                    cmd.Parameters.AddWithValue("RDateTime", oAQIS.RDateTime);
                    cmd.Parameters.AddWithValue("StatusDesc", oAQIS.StatusDesc);
                    cmd.Parameters.AddWithValue("Noin", oAQIS.RFPNo);
                    cmd.Parameters.AddWithValue("Permit", oAQIS.PermitNo);

                    cmd.Parameters.AddWithValue("FileInName", oAQIS.File_In_Name);
                    cmd.Parameters.AddWithValue("FileOutName", oAQIS.File_Out_Name);

                    cmd.Parameters.AddWithValue("Error1", oAQIS.Error1);
                    cmd.Parameters.AddWithValue("Error2", oAQIS.Error2);
                    cmd.Parameters.AddWithValue("Error3", oAQIS.Error3);
                    cmd.Parameters.AddWithValue("Error4", oAQIS.Error4);
                    cmd.Parameters.AddWithValue("Error5", oAQIS.Error5);
                    cmd.Parameters.AddWithValue("Error6", oAQIS.Error6);
                    cmd.Parameters.AddWithValue("Error7", oAQIS.Error7);
                    cmd.Parameters.AddWithValue("Error8", oAQIS.Error8);
                    cmd.Parameters.AddWithValue("Error9", oAQIS.Error9);
                    cmd.Parameters.AddWithValue("Error10", oAQIS.Error10);
                    cmd.Parameters.AddWithValue("Error11", oAQIS.Error11);
                    cmd.Parameters.AddWithValue("Error12", oAQIS.Error12);
                    cmd.Parameters.AddWithValue("Error13", oAQIS.Error13);
                    cmd.Parameters.AddWithValue("Error14", oAQIS.Error14);
                    cmd.Parameters.AddWithValue("Error15", oAQIS.Error15);
                    cmd.Parameters.AddWithValue("Error16", oAQIS.Error16);
                    cmd.Parameters.AddWithValue("Error17", oAQIS.Error17);
                    cmd.Parameters.AddWithValue("Error18", oAQIS.Error18);
                    cmd.Parameters.AddWithValue("Error19", oAQIS.Error19);
                    cmd.Parameters.AddWithValue("Error20", oAQIS.Error20);

                    cmd.Parameters.AddWithValue("Error21", oAQIS.Error21);
                    cmd.Parameters.AddWithValue("Error22", oAQIS.Error22);
                    cmd.Parameters.AddWithValue("Error23", oAQIS.Error23);
                    cmd.Parameters.AddWithValue("Error24", oAQIS.Error24);
                    cmd.Parameters.AddWithValue("Error25", oAQIS.Error25);
                    cmd.Parameters.AddWithValue("Error26", oAQIS.Error26);
                    cmd.Parameters.AddWithValue("Error27", oAQIS.Error27);
                    cmd.Parameters.AddWithValue("Error28", oAQIS.Error28);
                    cmd.Parameters.AddWithValue("Error29", oAQIS.Error29);
                    cmd.Parameters.AddWithValue("Error30", oAQIS.Error30);

                    cmd.Parameters.AddWithValue("Error31", oAQIS.Error31);
                    cmd.Parameters.AddWithValue("Error32", oAQIS.Error32);
                    cmd.Parameters.AddWithValue("Error33", oAQIS.Error33);
                    cmd.Parameters.AddWithValue("Error34", oAQIS.Error34);
                    cmd.Parameters.AddWithValue("Error35", oAQIS.Error35);
                    cmd.Parameters.AddWithValue("Error36", oAQIS.Error36);
                    cmd.Parameters.AddWithValue("Error37", oAQIS.Error37);
                    cmd.Parameters.AddWithValue("Error38", oAQIS.Error38);
                    cmd.Parameters.AddWithValue("Error39", oAQIS.Error39);
                    cmd.Parameters.AddWithValue("Error40", oAQIS.Error40);

                    cmd.Parameters.AddWithValue("Test", oAQIS.Test);
                    cmd.Parameters.AddWithValue("ContPage", oAQIS.ContPage);
                    cmd.Parameters.AddWithValue("ECN", oAQIS.ECN);
                    cmd.Parameters.AddWithValue("Spare", oAQIS.Spare);
                    cmd.Parameters.AddWithValue("FileInContent", oAQIS.File_In_Content);
                    cmd.Parameters.AddWithValue("FileOutContent", oAQIS.File_Out_Content);

                    //try
                    {
                        Object _result = cmd.ExecuteNonQuery();

                        if (_result != null && _result != DBNull.Value)
                        {
                            return (int)_result;
                        }
                        return -1;
                    }
                }
            }
        }


        //************DPI Documents*******************

        internal DataTable GetDPIDocumentData()
        {
            return RunQuery("SELECT * FROM " + sSchemas + "." + "\"tblDPIDocument\"");
        }
        internal DataTable GetDPIDocumentDataWithoutContent()
        {
            return RunQuery("SELECT \"Id\", \"DPIId\", \"Version\", \"Status\", \"RequestDate\", \"RequestTime\", \"OfficeId\", \"SDateTime\", \"RDateTime\", \"StatusDesc\", \"Noin\", \"Permit\", \"FileInName\", \"FileOutName\", \"Error1\", \"Error2\", \"Error3\", \"Error4\", \"Error5\", \"Error6\", \"Error7\", \"Error8\", \"Error9\", \"Error10\", \"Error11\", \"Error12\", \"Error13\", \"Error14\", \"Error15\", \"Error16\", \"Error17\", \"Error18\", \"Error19\", \"Error20\", \"Error21\", \"Error22\", \"Error23\", \"Error24\", \"Error25\", \"Error26\", \"Error27\", \"Error28\", \"Error29\", \"Error30\", \"Error31\", \"Error32\", \"Error33\", \"Error34\", \"Error35\", \"Error36\", \"Error37\", \"Error38\", \"Error39\", \"Error40\", \"Test\", \"ContPage\", \"ECN\", \"Spare\" FROM " + sSchemas + "." + "\"tblDPIDocument\"");
        }

        internal DataTable GetDPIDocumentData(string officeId)
        {
            return RunQuery("SELECT * FROM " + sSchemas + "." + "\"tblDPIDocument\" where \"OfficeId\" = '" + officeId + "'");
        }
        internal DataTable GetDPIDocumentDataWithoutContent(string officeId)
        {
            return RunQuery("SELECT \"Id\", \"DPIId\", \"Version\", \"Status\", \"RequestDate\", \"RequestTime\", \"OfficeId\", \"SDateTime\", \"RDateTime\", \"StatusDesc\", \"Noin\", \"Permit\", \"FileInName\", \"FileOutName\", \"Error1\", \"Error2\", \"Error3\", \"Error4\", \"Error5\", \"Error6\", \"Error7\", \"Error8\", \"Error9\", \"Error10\", \"Error11\", \"Error12\", \"Error13\", \"Error14\", \"Error15\", \"Error16\", \"Error17\", \"Error18\", \"Error19\", \"Error20\", \"Error21\", \"Error22\", \"Error23\", \"Error24\", \"Error25\", \"Error26\", \"Error27\", \"Error28\", \"Error29\", \"Error30\", \"Error31\", \"Error32\", \"Error33\", \"Error34\", \"Error35\", \"Error36\", \"Error37\", \"Error38\", \"Error39\", \"Error40\", \"Test\", \"ContPage\", \"ECN\", \"Spare\" FROM " + sSchemas + "." + "\"tblDPIDocument\" where \"OfficeId\" = '" + officeId + "'");
        }

        internal DataTable GetDPIDocumentData(int Id)
        {
            return RunQuery("SELECT * FROM " + sSchemas + "." + "\"tblDPIDocument\" where \"Id\" = " + Id);
        }

        internal DataTable IsDPIDocumentExist(string DPIId, int Version, string OfficeId)
        {
            return RunQuery("SELECT * FROM " + sSchemas + "." + "\"tblDPIDocument\" where \"DPIId\" = '" + DPIId + "' and \"Version\"='" + Version + "' and \"OfficeId\"='" + OfficeId + "'");
        }

        internal int SaveDPIData(DPIDocument oDPI)
        {
            var timestamp = DateTime.UtcNow;

            using (NpgsqlConnection conn = new NpgsqlConnection(pgdbconnection))
            {
                conn.Open();

                string sQuery = "INSERT INTO " + sSchemas + "." + "\"tblDPIDocument\" (\"DPIId\", \"Version\", \"Status\", \"RequestDate\", \"RequestTime\", \"OfficeId\", \"SDateTime\", \"RDateTime\", \"StatusDesc\", \"Noin\", \"Permit\", \"FileInName\", \"FileOutName\", \"Error1\", \"Error2\", \"Error3\", \"Error4\", \"Error5\", \"Error6\", \"Error7\", \"Error8\", \"Error9\", \"Error10\", \"Error11\", \"Error12\", \"Error13\", \"Error14\", \"Error15\", \"Error16\", \"Error17\", \"Error18\", \"Error19\", \"Error20\", \"Error21\", \"Error22\", \"Error23\", \"Error24\", \"Error25\", \"Error26\", \"Error27\", \"Error28\", \"Error29\", \"Error30\", \"Error31\", \"Error32\", \"Error33\", \"Error34\", \"Error35\", \"Error36\", \"Error37\", \"Error38\", \"Error39\", \"Error40\", \"Test\", \"ContPage\", \"ECN\", \"Spare\", \"FileInContent\", \"FileOutContent\") VALUES (:DPIId, :Version, :Status, :RequestDate, :RequestTime, :OfficeId, :SDateTime, :RDateTime, :StatusDesc, :Noin, :Permit, :FileInName, :FileOutName, :Error1, :Error2, :Error3, :Error4, :Error5, :Error6, :Error7, :Error8, :Error9, :Error10, :Error11, :Error12, :Error13, :Error14, :Error15, :Error16, :Error17, :Error18, :Error19, :Error20, :Error21, :Error22, :Error23, :Error24, :Error25, :Error26, :Error27, :Error28, :Error29, :Error30, :Error31, :Error32, :Error33, :Error34, :Error35, :Error36, :Error37, :Error38, :Error39, :Error40, :Test, :ContPage, :ECN, :Spare, :FileInContent, :FileOutContent) RETURNING \"Id\"";

                //string sQuery = "INSERT INTO " + sSchemas + "." + "\"tblDPIDocument\" (\"DPIId\", \"Version\", \"Status\", \"RequestDate\", \"RequestTime\", \"OfficeId\", \"SDateTime\", \"RDateTime\", \"StatusDesc\", \"Noin\", \"Permit\", \"FileInName\", \"FileOutName\", \"Error1\", \"Error2\", \"Error3\", \"Error4\", \"Error5\", \"Error6\", \"Error7\", \"Error8\", \"Error9\", \"Error10\") VALUES (:DPIId, :Version, :Status, :RequestDate, :RequestTime, :OfficeId, :SDateTime, :RDateTime, :StatusDesc, :Noin, :Permit, :FileInName, :FileOutName, :Error1, :Error2, :Error3, :Error4, :Error5, :Error6, :Error7, :Error8, :Error9, :Error10) RETURNING \"Id\"";


                using (NpgsqlCommand cmd = new NpgsqlCommand(sQuery, conn))
                {
                    DateTime now = DateTime.Now;

                    cmd.Parameters.AddWithValue("DPIId", oDPI.DPIId);

                    cmd.Parameters.AddWithValue("Version", oDPI.Version);
                    cmd.Parameters.AddWithValue("Status", oDPI.Status);
                    cmd.Parameters.AddWithValue("RequestTime", now);
                    cmd.Parameters.AddWithValue("RequestDate", timestamp);
                    cmd.Parameters.AddWithValue("OfficeId", oDPI.OfficeId);
                    cmd.Parameters.AddWithValue("SDateTime", oDPI.SDateTime);
                    cmd.Parameters.AddWithValue("RDateTime", oDPI.RDateTime);
                    cmd.Parameters.AddWithValue("StatusDesc", oDPI.StatusDesc);
                    cmd.Parameters.AddWithValue("Noin", oDPI.RFPNo);
                    cmd.Parameters.AddWithValue("Permit", oDPI.PermitNo);

                    cmd.Parameters.AddWithValue("FileInName", oDPI.File_In_Name);
                    cmd.Parameters.AddWithValue("FileOutName", oDPI.File_Out_Name);

                    cmd.Parameters.AddWithValue("Error1", oDPI.Error1);
                    cmd.Parameters.AddWithValue("Error2", oDPI.Error2);
                    cmd.Parameters.AddWithValue("Error3", oDPI.Error3);
                    cmd.Parameters.AddWithValue("Error4", oDPI.Error4);
                    cmd.Parameters.AddWithValue("Error5", oDPI.Error5);
                    cmd.Parameters.AddWithValue("Error6", oDPI.Error6);
                    cmd.Parameters.AddWithValue("Error7", oDPI.Error7);
                    cmd.Parameters.AddWithValue("Error8", oDPI.Error8);
                    cmd.Parameters.AddWithValue("Error9", oDPI.Error9);
                    cmd.Parameters.AddWithValue("Error10", oDPI.Error10);
                    cmd.Parameters.AddWithValue("Error11", oDPI.Error11);
                    cmd.Parameters.AddWithValue("Error12", oDPI.Error12);
                    cmd.Parameters.AddWithValue("Error13", oDPI.Error13);
                    cmd.Parameters.AddWithValue("Error14", oDPI.Error14);
                    cmd.Parameters.AddWithValue("Error15", oDPI.Error15);
                    cmd.Parameters.AddWithValue("Error16", oDPI.Error16);
                    cmd.Parameters.AddWithValue("Error17", oDPI.Error17);
                    cmd.Parameters.AddWithValue("Error18", oDPI.Error18);
                    cmd.Parameters.AddWithValue("Error19", oDPI.Error19);
                    cmd.Parameters.AddWithValue("Error20", oDPI.Error20);

                    cmd.Parameters.AddWithValue("Error21", oDPI.Error21);
                    cmd.Parameters.AddWithValue("Error22", oDPI.Error22);
                    cmd.Parameters.AddWithValue("Error23", oDPI.Error23);
                    cmd.Parameters.AddWithValue("Error24", oDPI.Error24);
                    cmd.Parameters.AddWithValue("Error25", oDPI.Error25);
                    cmd.Parameters.AddWithValue("Error26", oDPI.Error26);
                    cmd.Parameters.AddWithValue("Error27", oDPI.Error27);
                    cmd.Parameters.AddWithValue("Error28", oDPI.Error28);
                    cmd.Parameters.AddWithValue("Error29", oDPI.Error29);
                    cmd.Parameters.AddWithValue("Error30", oDPI.Error30);

                    cmd.Parameters.AddWithValue("Error31", oDPI.Error31);
                    cmd.Parameters.AddWithValue("Error32", oDPI.Error32);
                    cmd.Parameters.AddWithValue("Error33", oDPI.Error33);
                    cmd.Parameters.AddWithValue("Error34", oDPI.Error34);
                    cmd.Parameters.AddWithValue("Error35", oDPI.Error35);
                    cmd.Parameters.AddWithValue("Error36", oDPI.Error36);
                    cmd.Parameters.AddWithValue("Error37", oDPI.Error37);
                    cmd.Parameters.AddWithValue("Error38", oDPI.Error38);
                    cmd.Parameters.AddWithValue("Error39", oDPI.Error39);
                    cmd.Parameters.AddWithValue("Error40", oDPI.Error40);

                    cmd.Parameters.AddWithValue("Test", oDPI.Test);
                    cmd.Parameters.AddWithValue("ContPage", oDPI.ContPage);
                    cmd.Parameters.AddWithValue("ECN", oDPI.ECN);
                    cmd.Parameters.AddWithValue("Spare", oDPI.Spare);
                    cmd.Parameters.AddWithValue("FileInContent", oDPI.File_In_Content);
                    cmd.Parameters.AddWithValue("FileOutContent", oDPI.File_Out_Content);

                    ////try
                    {
                        Object _result = cmd.ExecuteScalar();

                        if (_result != null && _result != DBNull.Value)
                        {
                            return (int)_result;
                        }
                        return -1;
                    }
                    //catch (Exception oEx)
                    //{
                    //    return -1; // Return -1 if the insertion was unsuccessful
                    //}
                }
            }
        }

        internal int UpdateDPIData(DPIDocument oDPI)
        {
            var timestamp = DateTime.UtcNow;

            using (NpgsqlConnection conn = new NpgsqlConnection(pgdbconnection))
            {
                conn.Open();

                string sQuery = "UPDATE " + sSchemas + ".\"tblDPIDocument\" SET \"DPIId\"=@DPIId, \"Version\"=@Version, \"Status\"=@Status, \"RequestDate\"=@RequestDate, \"RequestTime\"=@RequestTime, \"OfficeId\"=@OfficeId, \"SDateTime\"=@SDateTime, \"RDateTime\"=@RDateTime, \"StatusDesc\"=@StatusDesc, \"Noin\"=@Noin, \"Permit\"=@Permit, \"FileInName\"=@FileInName, \"FileOutName\"=@FileOutName, \"Error1\"=@Error1, \"Error2\"=@Error2, \"Error3\"=@Error3, \"Error4\"=@Error4, \"Error5\"=@Error5, \"Error6\"=@Error6, \"Error7\"=@Error7, \"Error8\"=@Error8, \"Error9\"=@Error9, \"Error10\"=@Error10, \"Error11\"=@Error11, \"Error12\"=@Error12, \"Error13\"=@Error13, \"Error14\"=@Error14, \"Error15\"=@Error15, \"Error16\"=@Error16, \"Error17\"=@Error17, \"Error18\"=@Error18, \"Error19\"=@Error19, \"Error20\"=@Error20, \"Error21\"=@Error21, \"Error22\"=@Error22, \"Error23\"=@Error23, \"Error24\"=@Error24, \"Error25\"=@Error25, \"Error26\"=@Error26, \"Error27\"=@Error27, \"Error28\"=@Error28, \"Error29\"=@Error29, \"Error30\"=@Error30, \"Error31\"=@Error31, \"Error32\"=@Error32, \"Error33\"=@Error33, \"Error34\"=@Error34, \"Error35\"=@Error35, \"Error36\"=@Error36, \"Error37\"=@Error37, \"Error38\"=@Error38, \"Error39\"=@Error39, \"Error40\"=@Error40, \"Test\"=@Test, \"ContPage\"=@ContPage, \"ECN\"=@ECN, \"Spare\"=@Spare, \"FileInContent\"=@FileInContent, \"FileOutContent\"=@FileOutContent WHERE \"DPIId\" = '" + oDPI.DPIId + "' and \"Version\"='" + oDPI.Version + "' and \"OfficeId\"='" + oDPI.OfficeId + "'";

                using (NpgsqlCommand cmd = new NpgsqlCommand(sQuery, conn))
                {
                    DateTime now = DateTime.Now;

                    cmd.Parameters.AddWithValue("DPIId", oDPI.DPIId);

                    cmd.Parameters.AddWithValue("Version", oDPI.Version);
                    cmd.Parameters.AddWithValue("Status", oDPI.Status);
                    cmd.Parameters.AddWithValue("RequestTime", now);
                    cmd.Parameters.AddWithValue("RequestDate", timestamp);
                    cmd.Parameters.AddWithValue("OfficeId", oDPI.OfficeId);
                    cmd.Parameters.AddWithValue("SDateTime", oDPI.SDateTime);
                    cmd.Parameters.AddWithValue("RDateTime", oDPI.RDateTime);
                    cmd.Parameters.AddWithValue("StatusDesc", oDPI.StatusDesc);
                    cmd.Parameters.AddWithValue("Noin", oDPI.RFPNo);
                    cmd.Parameters.AddWithValue("Permit", oDPI.PermitNo);

                    cmd.Parameters.AddWithValue("FileInName", oDPI.File_In_Name);
                    cmd.Parameters.AddWithValue("FileOutName", oDPI.File_Out_Name);

                    cmd.Parameters.AddWithValue("Error1", oDPI.Error1);
                    cmd.Parameters.AddWithValue("Error2", oDPI.Error2);
                    cmd.Parameters.AddWithValue("Error3", oDPI.Error3);
                    cmd.Parameters.AddWithValue("Error4", oDPI.Error4);
                    cmd.Parameters.AddWithValue("Error5", oDPI.Error5);
                    cmd.Parameters.AddWithValue("Error6", oDPI.Error6);
                    cmd.Parameters.AddWithValue("Error7", oDPI.Error7);
                    cmd.Parameters.AddWithValue("Error8", oDPI.Error8);
                    cmd.Parameters.AddWithValue("Error9", oDPI.Error9);
                    cmd.Parameters.AddWithValue("Error10", oDPI.Error10);
                    cmd.Parameters.AddWithValue("Error11", oDPI.Error11);
                    cmd.Parameters.AddWithValue("Error12", oDPI.Error12);
                    cmd.Parameters.AddWithValue("Error13", oDPI.Error13);
                    cmd.Parameters.AddWithValue("Error14", oDPI.Error14);
                    cmd.Parameters.AddWithValue("Error15", oDPI.Error15);
                    cmd.Parameters.AddWithValue("Error16", oDPI.Error16);
                    cmd.Parameters.AddWithValue("Error17", oDPI.Error17);
                    cmd.Parameters.AddWithValue("Error18", oDPI.Error18);
                    cmd.Parameters.AddWithValue("Error19", oDPI.Error19);
                    cmd.Parameters.AddWithValue("Error20", oDPI.Error20);

                    cmd.Parameters.AddWithValue("Error21", oDPI.Error21);
                    cmd.Parameters.AddWithValue("Error22", oDPI.Error22);
                    cmd.Parameters.AddWithValue("Error23", oDPI.Error23);
                    cmd.Parameters.AddWithValue("Error24", oDPI.Error24);
                    cmd.Parameters.AddWithValue("Error25", oDPI.Error25);
                    cmd.Parameters.AddWithValue("Error26", oDPI.Error26);
                    cmd.Parameters.AddWithValue("Error27", oDPI.Error27);
                    cmd.Parameters.AddWithValue("Error28", oDPI.Error28);
                    cmd.Parameters.AddWithValue("Error29", oDPI.Error29);
                    cmd.Parameters.AddWithValue("Error30", oDPI.Error30);

                    cmd.Parameters.AddWithValue("Error31", oDPI.Error31);
                    cmd.Parameters.AddWithValue("Error32", oDPI.Error32);
                    cmd.Parameters.AddWithValue("Error33", oDPI.Error33);
                    cmd.Parameters.AddWithValue("Error34", oDPI.Error34);
                    cmd.Parameters.AddWithValue("Error35", oDPI.Error35);
                    cmd.Parameters.AddWithValue("Error36", oDPI.Error36);
                    cmd.Parameters.AddWithValue("Error37", oDPI.Error37);
                    cmd.Parameters.AddWithValue("Error38", oDPI.Error38);
                    cmd.Parameters.AddWithValue("Error39", oDPI.Error39);
                    cmd.Parameters.AddWithValue("Error40", oDPI.Error40);

                    cmd.Parameters.AddWithValue("Test", oDPI.Test);
                    cmd.Parameters.AddWithValue("ContPage", oDPI.ContPage);
                    cmd.Parameters.AddWithValue("ECN", oDPI.ECN);
                    cmd.Parameters.AddWithValue("Spare", oDPI.Spare);
                    cmd.Parameters.AddWithValue("FileInContent", oDPI.File_In_Content);
                    cmd.Parameters.AddWithValue("FileOutContent", oDPI.File_Out_Content);

                    //try
                    {
                        Object _result = cmd.ExecuteNonQuery();

                        if (_result != null && _result != DBNull.Value)
                        {
                            return (int)_result;
                        }
                        return -1;
                    }
                }
            }
        }


        internal DataTable RunQuery(string strQuery)
        {
            using (NpgsqlConnection _connection = new NpgsqlConnection(pgdbconnection))
            {
                using (NpgsqlCommand _command = new NpgsqlCommand())
                {
                    using (NpgsqlDataAdapter da = new NpgsqlDataAdapter(_command))
                    {
                        _connection.Open();
                        _command.Connection = _connection;
                        _command.CommandType = CommandType.Text;
                        _command.CommandText = strQuery;
                        DataTable dt = new DataTable();
                        da.Fill(dt);
                        return dt;
                    }
                }
            }
        }
    }
}
