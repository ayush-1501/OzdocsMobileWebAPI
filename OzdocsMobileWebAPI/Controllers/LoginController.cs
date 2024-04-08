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
    public class LoginController : ControllerBase
    {
        private readonly IConfiguration _configuration;

        public LoginController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        //[HttpPost("Login")]
        //public IActionResult Login([FromBody] Login myLogin)
        //{
        //    DataAccess dataAccess = new DataAccess(_configuration);
        //    DataTable retData = dataAccess.GetUserLoginDtls(myLogin.UserName, myLogin.Password);
        //    string json = JsonConvert.SerializeObject(retData, Formatting.Indented);
        //    return Ok(json);           
        //}

        [HttpGet("Login")]
        public IActionResult Login(string UserName, string Password)
        {
            DataAccess dataAccess = new DataAccess(_configuration);
            DataTable retData = dataAccess.GetUserLoginDtls(UserName, Password);
            string json = JsonConvert.SerializeObject(retData, Formatting.Indented);
            return Ok(json);
        }

    }
}
