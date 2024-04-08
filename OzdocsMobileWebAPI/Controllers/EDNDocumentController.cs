using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using OzdocsMobileWebAPI.DataAccessLayer;
using OzdocsMobileWebAPI.BusinessLayer;
using OzdocsMobileWebAPI.Models;
using System.Data;
using System.Configuration;
using OzdocsMobileWebAPI.CreateLogFiles;

namespace OzdocsMobileWebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EDNDocumentController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        string _connectionString = string.Empty;
        string LogFilePath = string.Empty;
        string RevisionDate = "20/02/2024";
        CreateLog oCreateLog = new CreateLog();
        
        public EDNDocumentController(IConfiguration configuration)
        {
            _configuration = configuration;
            _connectionString = _configuration.GetConnectionString("DefaultConnection");
            LogFilePath = _configuration.GetValue<string>("LogfileFolder");
        }

        [HttpPost("EDNDocumentCreate")]
        public ActionResult EDNDocumentCreate(EDNDocument oEDN)
        {
            Response response = new Response();
            int retData = 0;
            oCreateLog.CreateLogFile(LogFilePath, "");
            oCreateLog.CreateLogFile(LogFilePath, "======================================================================================");
            oCreateLog.CreateLogFile(LogFilePath, "Webservice OzdocsMobileWebAPI Execution Started | Revised On " + RevisionDate);
            oCreateLog.CreateLogFile(LogFilePath, "======================================================================================");
            try
            {
                DataAccess dataAccess = new DataAccess(_configuration);
                BusinessTier businessTier = new BusinessTier(_configuration);               

                DataTable _retData = dataAccess.IsEDNDocumentExist(oEDN.EdndocsId, oEDN.Version, oEDN.OfficeId);

                if (_retData.Rows.Count == 0) {
                    oCreateLog.CreateLogFile(LogFilePath, "Saving for EdndocsId "+oEDN.EdndocsId+" Version "+ oEDN.Version+" OfficeId "+ oEDN.OfficeId);
                    retData = dataAccess.SaveEDNData(oEDN);
                }
                else
                {
                    oCreateLog.CreateLogFile(LogFilePath, "Updating for EdndocsId " + oEDN.EdndocsId + " Version " + oEDN.Version + " OfficeId " + oEDN.OfficeId);
                    retData = dataAccess.UpdateEDNData(oEDN);
                }                

                if (retData != -1)
                {
                    oEDN.Id = retData;
                    oEDN.IsNew = 1;

                    //if (_retData.Rows.Count > 0) { oEDN.Id = Convert.ToInt32(_retData.Rows[0]["Id"].ToString()); oEDN.IsNew = 0; }
                    //oCreateLog.CreateLogFile(LogFilePath, "Saving EDI File for EdndocsId " + oEDN.EdndocsId + " Version " + oEDN.Version + " OfficeId " + oEDN.OfficeId);
                    //BusinessTier.FileData fileData = new BusinessTier.FileData()
                    //{
                    //    OfficeId = oEDN.OfficeId,
                    //    RefId = oEDN.EdndocsId,
                    //    Version = oEDN.Version,
                    //    Type = (BusinessTier.FileType) oEDN.IsNew,
                    //    RecordId = oEDN.Id,
                    //    FileContent = (oEDN.IsNew == 1) ? oEDN.File_Out_Content : oEDN.File_In_Content
                    //};

                    //businessTier.SaveEDIFile(fileData);

                    return Created("", oEDN);
                }
                else
                    return BadRequest();
            }
            catch (Exception ex)
            {
                response.Success = "0";
                response.Message = ex.Message;
                oCreateLog.CreateLogFile(LogFilePath, "Error occured while saving or updating Error: "+response.Message);
            }
            return new JsonResult(response);
        }


        [HttpGet("GetEDNData")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(EDNDocument))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(Response))]
        public IActionResult GetEDNData()
        {
            Response response = new Response();
            DataTable retData;
            string json;
            try
            {
                DataAccess dataAccess = new DataAccess(_configuration);
                retData = dataAccess.GetEDNDocumentDataWithoutContent();
                oCreateLog.CreateLogFile(LogFilePath, "Getting all the EDN data ");
                if (retData.Rows.Count > 0)
                {
                    oCreateLog.CreateLogFile(LogFilePath, "Successfull at getting data");
                    json = JsonConvert.SerializeObject(retData, Formatting.Indented);
                    return Ok(json);
                }
                else
                {
                    oCreateLog.CreateLogFile(LogFilePath, "No data found");
                    return NotFound();
                }
            }
            catch (Exception ex)
            {
                response.Success = "0";
                response.Message = ex.Message;
                oCreateLog.CreateLogFile(LogFilePath, "Error occured while getting data Error: " + response.Message);
                if (ex.StackTrace is not null)
                    response.Data = ex.StackTrace.ToString();
                return NotFound(response);
            }
        }

        [HttpGet("GetEDNDataByOfficeId")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(EDNDocument))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(Response))]
        public IActionResult GetEDNDataByOfficeId(string officeId)
        {
            Response response = new Response();
            DataTable retData;
            string json;
            try
            {
                DataAccess dataAccess = new DataAccess(_configuration);
                retData = dataAccess.GetEDNDocumentDataWithoutContent(officeId);
                oCreateLog.CreateLogFile(LogFilePath, "Getting EDN data by OfficeId");

                if (retData.Rows.Count > 0)
                {
                    oCreateLog.CreateLogFile(LogFilePath, "Successfull at getting data by OfficeId");
                    json = JsonConvert.SerializeObject(retData, Formatting.Indented);
                    return Ok(json);
                }
                else
                {
                    oCreateLog.CreateLogFile(LogFilePath, "No data found");
                    return NotFound();
                }
            }
            catch (Exception ex)
            {
                response.Success = "0";
                response.Message = ex.Message;
                oCreateLog.CreateLogFile(LogFilePath, "Error occured while getting data Error: " + response.Message);
                if (ex.StackTrace is not null)
                    response.Data = ex.StackTrace.ToString();
                return NotFound(response);
            }
        }

        [HttpGet("GetEDNDataById")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(EDNDocument))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(Response))]
        public IActionResult GetEDNDataById(int Id)
        {
            Response response = new Response();
            DataTable retData;
            string json;
            try
            {
                DataAccess dataAccess = new DataAccess(_configuration);
                retData = dataAccess.GetEDNDocumentData(Id);
                oCreateLog.CreateLogFile(LogFilePath, "Getting EDN data by ID");

                if (retData.Rows.Count > 0)
                {
                    oCreateLog.CreateLogFile(LogFilePath, "Successfull at getting data by ID");
                    json = JsonConvert.SerializeObject(retData, Formatting.Indented);
                    return Ok(json);
                }
                else
                {
                    oCreateLog.CreateLogFile(LogFilePath, "No data found");
                    return NotFound();
                }
            }
            catch (Exception ex)
            {
                response.Success = "0";
                response.Message = ex.Message;
                oCreateLog.CreateLogFile(LogFilePath, "Error occured while getting data Error: " + response.Message);
                if (ex.StackTrace is not null)
                    response.Data = ex.StackTrace.ToString();
                return NotFound(response);
            }
        }

        [HttpGet("GetEDNDataByFilter")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(EDNDocument))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(Response))]
        public IActionResult GetEDNDataByFilter(DateTime? toDate, DateTime? fromDate, string? senderRef, string? edn)
        {
            Response response = new Response();
            DataTable retData;
            string json;
            try
            {

                DataAccess dataAccess = new DataAccess(_configuration);
                retData = dataAccess.GetEDNDataByFilter(toDate, fromDate, senderRef, edn);
                oCreateLog.CreateLogFile(LogFilePath, "Getting all the EDN data ");
                if (retData.Rows.Count > 0)
                {
                    oCreateLog.CreateLogFile(LogFilePath, "Successfully retrieved data");
                    json = JsonConvert.SerializeObject(retData, Formatting.Indented);
                    return Ok(json);
                }
                else
                {
                    oCreateLog.CreateLogFile(LogFilePath, "No data found");
                    return NotFound();
                }
            }
            catch (Exception ex)
            {
                response.Success = "0";
                response.Message = ex.Message;
                oCreateLog.CreateLogFile(LogFilePath, "Error occurred while getting data Error: " + response.Message);
                if (ex.StackTrace is not null)
                    response.Data = ex.StackTrace.ToString();
                return NotFound(response);
            }
        }

    }
}
