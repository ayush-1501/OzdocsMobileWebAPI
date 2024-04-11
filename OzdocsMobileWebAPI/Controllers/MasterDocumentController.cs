using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using OzdocsMobileWebAPI.Models;
using System.Data;
//using System.Xml;
using OzdocsMobileWebAPI.DataAccessLayer;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using OzdocsMobileWebAPI.CreateLogFiles;

namespace OzdocsMobileWebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MasterDocumentController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        string _connectionString = string.Empty;
        string LogFilePath = string.Empty;
        string RevisionDate = "06/04/2024";
        CreateLog oCreateLog = new CreateLog();

        public MasterDocumentController(IConfiguration configuration)
        {
            _configuration = configuration;
            _connectionString = _configuration.GetConnectionString("DefaultConnection");
            LogFilePath = _configuration.GetValue<string>("LogfileFolder");
        }

        [HttpPost("MasterDocumentCreate")]
        public ActionResult MasterDocumentCreate(Master master)
        {
            Response response = new Response();

            oCreateLog.CreateLogFile(LogFilePath, "");
            oCreateLog.CreateLogFile(LogFilePath, "======================================================================================");
            oCreateLog.CreateLogFile(LogFilePath, "Webservice OzdocsMobileWebAPI Execution Started | Revised On " + RevisionDate);
            oCreateLog.CreateLogFile(LogFilePath, "======================================================================================");
            oCreateLog.CreateLogFile(LogFilePath, "Master Document Create");

            try
            {
                DataAccess dataAccess = new DataAccess(_configuration);
                int retData = dataAccess.SaveMasterData(master);

                if (retData != -1)
                {
                    master.Id = retData;
                    return Created("", master);
                }
                else
                    return BadRequest();

            }
            catch (Exception ex)
            {
                response.Success = "0";
                response.Message = ex.Message;
                if (ex.StackTrace is not null)
                    response.Data = ex.StackTrace.ToString();

                oCreateLog.CreateLogFile(LogFilePath, "Error occured while saving or updating Master Document, Error: " + response.Message + "\n" + ex.StackTrace);
            }
            return new JsonResult(response);
        }


        [HttpGet("GetMasterData")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Master))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(Response))]
        public IActionResult GetMasterData()
        {
            Response response = new Response();
            DataTable retData;
            string json;
            try
            {
                DataAccess dataAccess = new DataAccess(_configuration);
                retData = dataAccess.GetMasterDocumentData();

                if (retData.Rows.Count > 0)
                {
                    json = JsonConvert.SerializeObject(retData, Formatting.Indented);
                    return Ok(json);
                }
                else
                {
                    return NotFound();
                }
            }
            catch (Exception ex)
            {
                response.Success = "0";
                response.Message = ex.Message;
                if (ex.StackTrace is not null)
                    response.Data = ex.StackTrace.ToString();
                return NotFound(response);
            }
        }

        [HttpGet("GetMasterDataByOfficeId")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Master))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(Response))]
        public IActionResult GetMasterDataByOfficeId(string officeId)
        {
            Response response = new Response();
            DataTable retData;
            string json;
            try
            {
                DataAccess dataAccess = new DataAccess(_configuration);
                retData = dataAccess.GetMasterDocumentData(officeId);

                if (retData.Rows.Count > 0)
                {
                    json = JsonConvert.SerializeObject(retData, Formatting.Indented);
                    return Ok(json);
                }
                else
                {
                    return NotFound();
                }
            }
            catch (Exception ex)
            {
                response.Success = "0";
                response.Message = ex.Message;
                if (ex.StackTrace is not null)
                    response.Data = ex.StackTrace.ToString();
                return NotFound(response);
            }
        }

        [HttpGet("GetMasterDataById")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Master))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(Response))]
        public IActionResult GetMasterDataById(int Id)
        {
            Response response = new Response();
            DataTable retData;
            string json;
            try
            {
                DataAccess dataAccess = new DataAccess(_configuration);
                retData = dataAccess.GetMasterDocumentData(Id);

                if (retData.Rows.Count > 0)
                {
                    json = JsonConvert.SerializeObject(retData, Formatting.Indented);
                    return Ok(json);
                }
                else
                {
                    return NotFound();
                }
            }
            catch (Exception ex)
            {
                response.Success = "0";
                response.Message = ex.Message;
                if (ex.StackTrace is not null)
                    response.Data = ex.StackTrace.ToString();
                return NotFound(response);
            }
        }

        [HttpGet("GetMasterDataByFilter")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Master))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(Response))]
        public IActionResult GetMasterDataByFilter(DateTime? toDate, DateTime? fromDate, string? DocumentId, string? InvoiceNo, DateTime? ToInvoice, DateTime? FromInvoice, string? Exporter, string? Edn)
        {
            Response response = new Response();
            DataTable retData;
            string json;
            try
            {
                DataAccess dataAccess = new DataAccess(_configuration);
                retData = dataAccess.GetMasterDocumentByFilter(toDate, fromDate, DocumentId, InvoiceNo, ToInvoice, FromInvoice, Exporter, Edn);

                if (retData.Rows.Count > 0)
                {
                    json = JsonConvert.SerializeObject(retData, Formatting.Indented);
                    return Ok(json);
                }
                else
                {
                    return NotFound();
                }
            }
            catch (Exception ex)
            {
                response.Success = "0";
                response.Message = ex.Message;
                if (ex.StackTrace is not null)
                    response.Data = ex.StackTrace.ToString();
                return NotFound(response);
            }
        }

        [HttpGet("GetMasterDataRecordCount")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Master))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(Response))]
        public IActionResult GetMasterDataRecordCount(DateTime? toDate, DateTime? fromDate, string? TypeOfQuery,string? Id)
        {
            Response response = new Response();
            DataTable retData;
            string json;
            try
            {
                DataAccess dataAccess = new DataAccess(_configuration);
                retData = dataAccess.GetMasterDocumentRecordCount(toDate, fromDate, TypeOfQuery, Id);

                if (retData.Rows.Count > 0)
                {
                    json = JsonConvert.SerializeObject(retData, Formatting.Indented);
                    return Ok(json);
                }
                else
                {
                    return NotFound();
                }
            }
            catch (Exception ex)
            {
                response.Success = "0";
                response.Message = ex.Message;
                if (ex.StackTrace is not null)
                    response.Data = ex.StackTrace.ToString();
                return NotFound(response);
            }
        }

    }
}
