using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using OzdocsMobileWebAPI.DataAccessLayer;
using OzdocsMobileWebAPI.Models;
using System.Data;

namespace OzdocsMobileWebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DPIDocumentController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        string _connectionString = string.Empty;
        public DPIDocumentController(IConfiguration configuration)
        {
            _configuration = configuration;
            _connectionString = _configuration.GetConnectionString("DefaultConnection");
        }

        [HttpPost("DPIDocumentCreate")]
        public ActionResult DPIDocumentCreate(DPIDocument oDPI)
        {
            Response response = new Response();
            int retData = 0;

            try
            {
                DataAccess dataAccess = new DataAccess(_configuration);
                DataTable _retData = dataAccess.IsDPIDocumentExist(oDPI.DPIId, oDPI.Version, oDPI.OfficeId);

                if (_retData.Rows.Count == 0)
                {
                    retData = dataAccess.SaveDPIData(oDPI);
                }
                else
                {
                    retData = dataAccess.UpdateDPIData(oDPI);
                }

                if (retData != -1)
                {
                    oDPI.Id = retData;
                    oDPI.IsNew = 1;

                    if (_retData.Rows.Count > 0) { oDPI.Id = Convert.ToInt32(_retData.Rows[0]["Id"].ToString()); oDPI.IsNew = 0; }

                    return Created("", oDPI);
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


        [HttpGet("GetDPIData")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(DPIDocument))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(Response))]
        public IActionResult GetDPIData()
        {
            Response response = new Response();
            DataTable retData;
            string json;
            try
            {
                DataAccess dataAccess = new DataAccess(_configuration);
                retData = dataAccess.GetDPIDocumentDataWithoutContent();

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

        [HttpGet("GetDPIDataByOfficeId")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(DPIDocument))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(Response))]
        public IActionResult GetDPIDataByOfficeId(string officeId)
        {
            Response response = new Response();
            DataTable retData;
            string json;
            try
            {
                DataAccess dataAccess = new DataAccess(_configuration);
                retData = dataAccess.GetDPIDocumentDataWithoutContent(officeId);

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

        [HttpGet("GetDPIDataById")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(DPIDocument))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(Response))]
        public IActionResult GetDPIDataById(int Id)
        {
            Response response = new Response();
            DataTable retData;
            string json;
            try
            {
                DataAccess dataAccess = new DataAccess(_configuration);
                retData = dataAccess.GetDPIDocumentData(Id);

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
