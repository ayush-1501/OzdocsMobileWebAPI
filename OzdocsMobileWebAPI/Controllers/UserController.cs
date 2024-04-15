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
    public class UserController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        string _connectionString = string.Empty;
        public UserController(IConfiguration configuration)
        {
            _configuration = configuration;
            _connectionString = _configuration.GetConnectionString("DefaultConnection");
        }

        [HttpPost("UserCreate")]
        public ActionResult UserCreate(User oUser)
        {
            Response response = new Response();
            try
            {
                DataAccess dataAccess = new DataAccess(_configuration);
                string retData = dataAccess.SaveUserData(oUser);

                if (retData != null)
                {
                    oUser.OrgId = retData;
                    return Created("", oUser);
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


        [HttpGet("GetUserData")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(User))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(Response))]
        public IActionResult GetUserData()
        {
            Response response = new Response();
            DataTable retData;
            string json;
            try
            {
                DataAccess dataAccess = new DataAccess(_configuration);
                retData = dataAccess.GetUserData();

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

        [HttpGet("GetUserDataByOrgId")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(User))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(Response))]
        public IActionResult GetUserByOrgId(string OrgId)
        {
            Response response = new Response();
            DataTable retData;
            string json;
            try
            {
                DataAccess dataAccess = new DataAccess(_configuration);
                retData = dataAccess.GetUserData(OrgId);

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
