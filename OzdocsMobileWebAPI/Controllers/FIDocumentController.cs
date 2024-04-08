using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using OzdocsMobileWebAPI.DataAccessLayer;
using OzdocsMobileWebAPI.Models;
using System.Data;

namespace OzdocsMobileWebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FIDocumentController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        string _connectionString = string.Empty;
        public FIDocumentController(IConfiguration configuration)
        {
            _configuration = configuration;
            _connectionString = _configuration.GetConnectionString("DefaultConnection");
        }


        [HttpPost("FIDocumentCreate")]
        public ActionResult FIDocumentCreate(FIDocument oFI)
        {
            Response response = new Response();
            int retData = 0;
            try
            {
                DataAccess dataAccess = new DataAccess(_configuration);
                DataTable _retData = dataAccess.IsFIDocumentExist(oFI.FidocsId, oFI.Version, oFI.OfficeId);

                if (_retData.Rows.Count == 0)
                {
                    retData = dataAccess.SaveFIData(oFI);
                }
                else
                {
                    
                    retData = dataAccess.UpdateFIData(oFI);
                }

                if (retData != -1)
                {
                    oFI.Id = retData;
                    oFI.IsNew = 1;

                    if (_retData.Rows.Count > 0) { oFI.Id = Convert.ToInt32(_retData.Rows[0]["Id"].ToString()); oFI.IsNew = 0; }

                    return Created("", oFI);
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


        [HttpGet("GetFIData")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(FIDocument))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(Response))]
        public IActionResult GetFIData()
        {
            Response response = new Response();
            DataTable retData;
            string json;
            try
            {
                DataAccess dataAccess = new DataAccess(_configuration);
                retData = dataAccess.GetFIDocumentDataWithoutContent();

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

        [HttpGet("GetFIDataByOfficeId")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(FIDocument))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(Response))]
        public IActionResult GetFIDataByOfficeId(string officeId)
        {
            Response response = new Response();
            DataTable retData;
            string json;
            try
            {
                DataAccess dataAccess = new DataAccess(_configuration);
                retData = dataAccess.GetFIDocumentDataWithoutContent(officeId);

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

        [HttpGet("GetFIDataById")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(FIDocument))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(Response))]
        public IActionResult GetFIDataById(int Id)
        {
            Response response = new Response();
            DataTable retData;
            string json;
            try
            {
                DataAccess dataAccess = new DataAccess(_configuration);
                retData = dataAccess.GetFIDocumentData(Id);

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
