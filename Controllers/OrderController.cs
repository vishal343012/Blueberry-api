using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using NextHolidaysIn.Models;
using Razorpay.Api;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;

namespace NextHolidaysIn.Controllers
{
    public class OrderController : Controller
    {
        public OrderDataEnt _orders { get; set; }
        public IActionResult Index()
        {
            return View();
        }
		[EnableCors("AllowSpecificOrigin")]
		[HttpPost]
        public string InititateOrder([FromBody] CartEnt data)
        {
            System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            string key = "rzp_test_VWf52cV1HZnBkL";
            string secret = "RQVQlGideCMEdFDSJ3z82EJ4";

            Random _random = new Random();
            string TransactionId = _random.Next(0, 10000).ToString();

            Dictionary<string, object> input = new Dictionary<string, object>();
            input.Add("amount", data.amount*100); // this amount should be same as transaction amount
            input.Add("currency", data.currency);
            input.Add("receipt", data.rctno);


            RazorpayClient client = new RazorpayClient(key, secret);

            Razorpay.Api.Order order = client.Order.Create(input);
            string orderId = order["id"].ToString();
              
            return orderId;  
        }
		[EnableCors("AllowSpecificOrigin")]
		[HttpPost]
        public async Task<IActionResult> CreatePaymentLink( [FromBody]CartEnt data )
        {
            string keyId = "rzp_test_VWf52cV1HZnBkL";
            string keySecret = "RQVQlGideCMEdFDSJ3z82EJ4";
            RazorpayClient client = new RazorpayClient(keyId, keySecret);
            var expireBy = DateTime.Now.AddMinutes(20);
            var Data = GetUnixTimestamp(expireBy);
            var val = GetUnixTimestamp(DateTime.Now.AddMinutes(20));
            Dictionary<string, object> paymentLinkRequest = new Dictionary<string, object>
            {
                { "amount", data.amount*100 },
                { "currency", data.currency },
                { "accept_partial", false },
                { "first_min_partial_amount", 0 },
                { "expire_by", val },
                { "reference_id",data.orderid  },
                { "description", "Payment for policy no" +data.orderid },
                { "customer", new Dictionary<string, object>
                    {
                        { "contact", data.mobile},
                        { "name", data.name },
                        { "email", data.email }
                    }
                },
                { "notify", new Dictionary<string, object>
                    {
                        { "sms", true },
                        { "email", true }
                    }
                },
                { "reminder_enable", true },
                { "notes", new Dictionary<string, object>
                    {
                        { "policy_name", "Jeevan Bima" }
                    }
                },
                { "callback_url", "https://mainweb.nextholidays.co.in/Home/SuccessPayment" },
                { "callback_method", "get" }
            };

            try
            {
                PaymentLink paymentLink = client.PaymentLink.Create(paymentLinkRequest);
                var responseJson = paymentLink.Attributes.ToString();
                dynamic paymentLinkResponse = JObject.Parse(responseJson);

                var paymentId = paymentLinkResponse.id; // Extracted payment ID

				if (paymentLinkResponse.status == "Created")
				{
					BookingEnt b = new BookingEnt()
					{
						UserId = Convert.ToString(data.UserId),
						TempUserId = Convert.ToString(data.TempUserId),
						CartIds = Convert.ToString(data.CartIds),
						IPAddress = "0:0",
						FirstName = Convert.ToString(data.name),
						LastName = Convert.ToString(data.name),
						Gender = Convert.ToString(data.gender),
						CountryCode = Convert.ToString(data.countrycode),
						MobileNumber = Convert.ToString(data.mobile),
						Email = Convert.ToString(data.email),
						Country = Convert.ToString(data.country),
						PaymentMethod = "RazorPay",
						PaymentID = paymentId,
						CouponId = Convert.ToInt32(0),
						HearAboutUs = Convert.ToString(""),
						SessionId = Convert.ToString(""),

					};
		           MessageStatus s = Models.Order.ProcessBooking(b);
                    return Ok(new { PaymentId = paymentId, PaymentLink = paymentLinkResponse,Order= s.Message });
				}

				return Ok(new { PaymentId = paymentId, PaymentLink = paymentLinkResponse});
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
        private static long GetUnixTimestamp(DateTime dateTime)
        {
            DateTimeOffset dateTimeOffset = new DateTimeOffset(dateTime);
            return dateTimeOffset.ToUnixTimeSeconds();
        }
    }
}
