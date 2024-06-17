using Backend.Models;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace NextHolidaysIn.Models
{
    public class Common
    {
        private static IHttpContextAccessor _httpContextAccessor;

        public static void Configure(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public static string GetTempUserId()
        {
            var context = _httpContextAccessor.HttpContext;
            string tempUserId = context.Session.GetString("TempUserId");
            string userId = context.Session.GetString("UserId");

            if (string.IsNullOrEmpty(tempUserId) && (string.IsNullOrEmpty(userId) || Convert.ToInt32(userId) == 0))
            {
                tempUserId = Guid.NewGuid().ToString();
                context.Session.SetString("TempUserId", tempUserId);
            }

            return tempUserId;
        }
        public static MyAccountEnt GetUserData()
        {
            try
            {
                var userId = Convert.ToInt32(_httpContextAccessor.HttpContext.Session.GetString("UserId"));
                SqlParameter[] parameters = new SqlParameter[] {
                new SqlParameter("@UserId", userId)
            };
                DBConnection db = new DBConnection();
                DataTable dataTable = db.GetDataTable("GetUserData", parameters);

                return HTMLExtensions.ToList<MyAccountEnt>(dataTable).FirstOrDefault();
            }
            catch (Exception ex)
            {
                // Log the exception
                return null;
            }
        }
        public static string send_email(
                        string from,
                        string Name,
                        string to,
                        string cc,
                        string subject,
                        string body, string bcc = "")
        {


            System.Net.Mail.MailMessage email = new System.Net.Mail.MailMessage();
            System.Net.Mail.MailMessage mailmsg = new System.Net.Mail.MailMessage();
            System.Net.Mail.SmtpClient mailer = new System.Net.Mail.SmtpClient();

            System.Net.Mail.MailAddress mailaddr = new System.Net.Mail.MailAddress(from, Name);
            System.Net.NetworkCredential creds = new System.Net.NetworkCredential("emailappsmtp.71e9e70fcee36bd3", "FExgNyMwQpLK");

            try
            {
                System.Net.ServicePointManager.SecurityProtocol = System.Net.SecurityProtocolType.Tls12 | System.Net.SecurityProtocolType.Tls11 | System.Net.SecurityProtocolType.Tls;


                mailmsg.IsBodyHtml = true;
                mailmsg.Subject = subject;
                mailmsg.To.Add(to);


                if (cc != "")
                {
                    mailmsg.CC.Add(cc);
                }



                if (bcc != "")
                {
                    mailmsg.Bcc.Add(bcc);
                }

                mailmsg.Body = body;
                mailmsg.From = mailaddr;
                mailer.EnableSsl = true;
                mailer.Host = "smtp.zeptomail.in"; //smtp_server;
                mailer.Port = Convert.ToInt32(587);
                mailer.UseDefaultCredentials = false;
                mailer.Credentials = creds;

                mailer.Send(mailmsg);

                return "";
            }
            catch (Exception e)
            {
                return (e.GetBaseException().Message);
            }

        }
        public static string GetUserId()
        {
            var context = _httpContextAccessor.HttpContext;
            string userId = context.Session.GetString("UserId");

            if (string.IsNullOrEmpty(userId) || Convert.ToInt32(userId) == 0)
            {
                userId = "0";
            }

            return userId;
        }

        public static string GetIsB2B()
        {
            var context = _httpContextAccessor.HttpContext;
            string isB2B = context.Session.GetString("IsB2B");

            if (string.IsNullOrEmpty(isB2B) || Convert.ToInt32(isB2B) == 0)
            {
                isB2B = "0";
            }

            return isB2B;
        }
    }
}
