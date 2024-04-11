using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using OzdocsMobileWebAPI.CreateLogFiles;
using OzdocsMobileWebAPI.DataAccessLayer;
using OzdocsMobileWebAPI.Models;
using System.Data;

namespace OzdocsMobileWebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AQISDocumentController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        string _connectionString = string.Empty;
        public AQISDocumentController(IConfiguration configuration)
        {
            _configuration = configuration;
            _connectionString = _configuration.GetConnectionString("DefaultConnection");
        }

        [HttpPost("AQISDocumentCreate")]
        public ActionResult AQISDocumentCreate(AQISDocument oAQIS)
        {
            Response response = new Response();
            int retData = 0;

            try
            {
                DataAccess dataAccess = new DataAccess(_configuration);
                DataTable _retData = dataAccess.IsAQISDocumentExist(oAQIS.AQISId, oAQIS.Version, oAQIS.OfficeId);

                if (_retData.Rows.Count == 0)
                {
                    retData = dataAccess.SaveAQISData(oAQIS);
                }
                else
                {
                    retData = dataAccess.UpdateAQISData(oAQIS);
                }

                if (retData != -1)
                {
                    oAQIS.Id = retData;
                    oAQIS.IsNew = 1;

                    if (_retData.Rows.Count > 0) { oAQIS.Id = Convert.ToInt32(_retData.Rows[0]["Id"].ToString()); oAQIS.IsNew = 0; }

                    return Created("", oAQIS);
                }
                else
                    return BadRequest();
            }
            catch (Exception ex)
            {
                response.Success = "0";
                response.Message = ex.Message;
            }
            return new JsonResult(response);
        }


        [HttpGet("GetAQISData")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(AQISDocument))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(Response))]
        public IActionResult GetAQISData()
        {
            Response response = new Response();
            DataTable retData;
            string json;
            try
            {
                DataAccess dataAccess = new DataAccess(_configuration);
                retData = dataAccess.GetAQISDocumentDataWithoutContent();

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

        [HttpGet("GetAQISDataByOfficeId")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(AQISDocument))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(Response))]
        public IActionResult GetAQISDataByOfficeId(string officeId)
        {
            Response response = new Response();
            DataTable retData;
            string json;
            try
            {
                DataAccess dataAccess = new DataAccess(_configuration);
                retData = dataAccess.GetAQISDocumentDataWithoutContent(officeId);

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

        [HttpGet("GetAQISDataById")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(AQISDocument))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(Response))]
        public IActionResult GetAQISDataById(int Id)
        {
            Response response = new Response();
            DataTable retData;
            string json;
            try
            {
                DataAccess dataAccess = new DataAccess(_configuration);
                retData = dataAccess.GetAQISDocumentData(Id);

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

        [HttpGet("GetAQISDataByFilter")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(AQISDocument))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(Response))]
        public IActionResult GetAQISDataByFilter(DateTime? toDate, DateTime? fromDate, string? AQISId, string? RFPNo)
        {
            Response response = new Response();
            DataTable retData;
            string json;
            try
            {
               
                DataAccess dataAccess = new DataAccess(_configuration);
                retData = dataAccess.GetAQISDataByFilter(toDate, fromDate, AQISId, RFPNo);
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

        [HttpGet("GetAQISDataRecordCount")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(AQISDocument))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(Response))]
        public IActionResult GetAQISDataRecordCount(DateTime? toDate, DateTime? fromDate, string? TypeOfQuery, string? Id)
        {
            Response response = new Response();
            DataTable retData;
            string json;
            try
            {
                DataAccess dataAccess = new DataAccess(_configuration);
                retData = dataAccess.GetAQISDocumentRecordCount(toDate, fromDate, TypeOfQuery, Id);

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
