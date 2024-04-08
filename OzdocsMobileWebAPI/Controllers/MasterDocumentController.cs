using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using OzdocsMobileWebAPI.Models;
using System.Data;
//using System.Xml;
using OzdocsMobileWebAPI.DataAccessLayer;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace OzdocsMobileWebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MasterDocumentController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        string _connectionString = string.Empty;
        public MasterDocumentController(IConfiguration configuration)
        {
            _configuration = configuration;
            _connectionString = _configuration.GetConnectionString("DefaultConnection");
        }

        [HttpPost("MasterDocumentCreate")]
        public ActionResult MasterDocumentCreate(Master master)
        {
            Response response = new Response();
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

                //int count = 0;
                //foreach (var item in retData.Rows)
                //{
                //    string profilePicpath = retData.Rows[count]["FilePath"].ToString();
                //    if (!string.IsNullOrEmpty(profilePicpath) || System.IO.File.Exists(profilePicpath))
                //    {
                //        var bytes = System.IO.File.ReadAllBytes(profilePicpath);
                //        retData.Rows[count]["FilePath"] = Convert.ToBase64String(bytes);
                //    }
                //    count++;
                //}
                //string json = JsonConvert.SerializeObject(retData, Formatting.None);
                //response.Success = "1";
                //response.Message = $"Medicle Records.";
                //response.Data = json;
            }
            catch (Exception ex)
            {
                response.Success = "0";
                response.Message = ex.Message;
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

    }
}
