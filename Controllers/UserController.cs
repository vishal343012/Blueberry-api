using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using NextHolidaysIn.Models;
using Razorpay.Api;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


namespace NextHolidaysIn.Controllers
{
    public class UserController : Controller
    {
        [EnableCors("AllowSpecificOrigin")]
        [HttpPost]
        public ActionResult<MessageStatus> RegisterUser([FromBody] UsersEnt data)
        {
            var result = Models.User.RegisterUser(data);
            return Ok(result); // Use Ok() to return a 200 status code
        }
		[EnableCors("AllowSpecificOrigin")]
		[HttpGet]
        public IActionResult GetCountryId(string Name)
        {
            return Ok(Models.User.GetCountryId(Name));
        }
		[EnableCors("AllowSpecificOrigin")]
		[HttpGet]
        public IActionResult GetCityId(string Name)
        {
            return Ok(Models.User.GetCityId(Name));
        }
		[EnableCors("AllowSpecificOrigin")]
		[HttpPost]
        public MessageStatusAccount LoginUserApp([FromBody] UsersEntApp data)
        {
            return Models.User.LoginUserApp(data);
        }
        [EnableCors("AllowSpecificOrigin")]
        [HttpPost]
        public MessageStatus LoginUser(string EmailId, string Password)
        {
            return Models.User.LoginUser(EmailId, Password);
        }
        [EnableCors("AllowSpecificOrigin")]
        [HttpGet]
        public MyAccountEnt GetUserData()
        {
            return Models.User.GetUserData();
        }
        [EnableCors("AllowSpecificOrigin")]
        [HttpPost]
        public MessageStatus SaveUserData([FromBody] MyAccountEnt data)
        {
            return Models.User.SaveUserData(data);
        }
		[EnableCors("AllowSpecificOrigin")]
		[HttpPost]


        public MyAccountEnt GetUserData([FromBody] MyAccountEnt data)
        {
            return Models.User.GetUserData(data);
        }
		[EnableCors("AllowSpecificOrigin")]
		[HttpPost]

        public MessageStatus DeleteAccount([FromBody] MessageStatus data)
        {
            return Models.User.DeleteAccount(data);
        }
		[EnableCors("AllowSpecificOrigin")]
		[HttpPost]
        public MessageStatus GenerateOTP([FromBody] VerificationEnt data)
        {
            return Models.User.GenerateOTP(data);
        }
		[EnableCors("AllowSpecificOrigin")]
		[HttpPost]
        public MessageStatus VerifyOTP([FromBody] UsersEnt data)
        {
            return Models.User.VerifyOTP(data);
        }
		[EnableCors("AllowSpecificOrigin")]
		[HttpPost]
        public MessageStatus ChangePassword_User(PasswordChangeEnt data)
        {
            return Models.User.ChangePassword_User(data);
        }
        [EnableCors("AllowSpecificOrigin")]
        [HttpGet]
        public IActionResult GetCountryList()
        {
            return Ok(Models.User.GetCountryList());
        }

        [EnableCors("AllowSpecificOrigin")]
        [HttpGet]
        public IActionResult GetCityList(int CountryId)
        {
            return Ok(Models.User.GetCityList(CountryId));
        }
		[EnableCors("AllowSpecificOrigin")]
		[HttpGet]
		public async Task<IActionResult> GetShortCart(string tempuserid , int userid)
		{
			return Ok(await Models.User.GetShortCartList(tempuserid, userid));
		}


	}
}
