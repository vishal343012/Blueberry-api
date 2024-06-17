using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NextHolidaysIn.Models;
using System;
using System.Threading.Tasks;




namespace NextHolidaysIn.Controllers
{
    public class PackageController : Controller
    {
		[EnableCors("AllowSpecificOrigin")]
		[HttpPost]
        public async Task<IActionResult> GetPackageList([FromBody] PackageSearchEnt data)
        {
            decimal sessionROE = HttpContext.Session.Get<decimal>("ROE");
            data.ROE = sessionROE;
            return Ok(await Models.PackageSearch.GetPackageList(data));
        }
		[EnableCors("AllowSpecificOrigin")]
		[HttpPost]
        public async Task<IActionResult> GetPackageOptions([FromBody] PackageOptionsEnt data)
        {
			return Ok(Models.PackageSearch.GetPackageOptionsNew(data));

        }
		[EnableCors("AllowSpecificOrigin")]
		[HttpPost]
		public async Task<IActionResult> GetPackageDates([FromBody]PackageOptionsEnt data)
		{
			return Ok( Models.PackageSearch.GetPackageDates(data));
		}
		[EnableCors("AllowSpecificOrigin")]
		[HttpPost]
		public IActionResult GetPackageDescription([FromBody]PackageDes data)
		{
			var result = Models.PackageSearch.GetPackageDescription(data);
			return Ok(result);
		}
		[EnableCors("AllowSpecificOrigin")]
		[HttpPost]
		public async Task<IActionResult> AddToCart([FromBody]PackageCartEnt data)
		{
			return Ok(Models.PackageSearch.AddToCart(data));
		}
		[EnableCors("AllowSpecificOrigin")]
		[HttpPost]

		public IActionResult RemoveFromCart([FromBody]RemoveCart data)
		{
			return Ok(Models.PackageSearch.RemoveFromCart(data));
		}

	}
}
