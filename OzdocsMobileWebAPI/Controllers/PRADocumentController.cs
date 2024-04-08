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
    public class PRADocumentController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        string _connectionString = string.Empty;
        public PRADocumentController(IConfiguration configuration)
        {
            _configuration = configuration;
            _connectionString = _configuration.GetConnectionString("DefaultConnection");
        }


        [HttpPost("PRADocumentCreate")]
        public ActionResult PRADocumentCreate(PRADocument oPRA)
        {
            Response response = new Response();
            int retData = 0;
            try
            {
                DataAccess dataAccess = new DataAccess(_configuration);
                DataTable _retData = dataAccess.IsPRADocumentExist(oPRA.PradocsId, oPRA.Version, oPRA.OfficeId);

                if (_retData.Rows.Count == 0)
                {
                    retData = dataAccess.SavePRAData(oPRA);
                }
                else
                {
                    int id = Convert.ToInt32(_retData.Rows[0]["Id"].ToString());
                    retData = dataAccess.UpdatePRAData(oPRA);
                }

                if (retData != -1)
                {
                    oPRA.Id = retData;
                    oPRA.IsNew = 1;

                    if (_retData.Rows.Count > 0) { oPRA.Id = Convert.ToInt32(_retData.Rows[0]["Id"].ToString()); oPRA.IsNew = 0; }

                    return Created("", oPRA);
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


        [HttpGet("GetPRAData")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(PRADocument))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(Response))]
        public IActionResult GetPRAData()
        {
            Response response = new Response();
            DataTable retData;
            string json;
            try
            {
                DataAccess dataAccess = new DataAccess(_configuration);
                retData = dataAccess.GetPRADocumentDataWithoutContent();

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

        [HttpGet("GetPRADataByOfficeId")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(PRADocument))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(Response))]
        public IActionResult GetPRADataByOfficeId(string officeId)
        {
            Response response = new Response();
            DataTable retData;
            string json;
            try
            {
                DataAccess dataAccess = new DataAccess(_configuration);
                retData = dataAccess.GetPRADocumentDataWithoutContent(officeId);

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

        [HttpGet("GetPRADataById")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(PRADocument))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(Response))]
        public IActionResult GetPRADataById(int Id)
        {
            Response response = new Response();
            DataTable retData;
            string json;
            try
            {
                DataAccess dataAccess = new DataAccess(_configuration);
                retData = dataAccess.GetPRADocumentData(Id);

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
