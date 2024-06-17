using Newtonsoft.Json;
using Org.BouncyCastle.Asn1.Crmf;
using Razorpay.Api;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Xml;
using System;
using System.Security.Cryptography;

namespace NextHolidaysIn.Models
{
    public class Order
    {
		public static MessageStatus ProcessBooking(BookingEnt data)
		{
			try
			{
				SqlParameter[] para = new SqlParameter[] {
					new SqlParameter("@UserId",data.UserId),
					new SqlParameter("@TempUserId",data.TempUserId),
					new SqlParameter("@CartIds",data.CartIds),
					new SqlParameter("@IPAddress",data.IPAddress),
					new SqlParameter("@FirstName",data.FirstName),
					new SqlParameter("@LastName",data.LastName),
					new SqlParameter("@Gender",data.Gender),
					new SqlParameter("@CountryCode",data.CountryCode),
					new SqlParameter("@MobileNumber",data.MobileNumber),
					new SqlParameter("@Email",data.Email),
					new SqlParameter("@Country",data.Country),
					new SqlParameter("@PaymentMethod",data.PaymentMethod),
					new SqlParameter("@PaymentID",data.PaymentID),
					new SqlParameter("@CouponId",data.CouponId),
					new SqlParameter("@MakePending",data.MakePending),
					new SqlParameter("@HearAboutUs",data.HearAboutUs),
					new SqlParameter("@SessionId",data.SessionId),
				};
				DBConnection db = new DBConnection();
				String RefrenceNo = Convert.ToString(db.GetExecuteScalarSP("SaveBooking", para));

				if (RefrenceNo == "")
					return new MessageStatus()
					{
						Status = 0,
						Message = "Something went wrong. ERROR:PB1"
					};
			
			
				
				String HTMLEMail = "<html xmlns=\"http://www.w3.org/1999/xhtml\" xmlns:v=\"urn:schemas-microsoft-com:vml\" xmlns:o=\"urn:schemas-microsoft-com:office:office\"><head><!--[if gte mso 9]><xml>  <o:OfficeDocumentSettings>    <o:AllowPNG/>    <o:PixelsPerInch>96</o:PixelsPerInch>  </o:OfficeDocumentSettings></xml><![endif]-->  <meta http-equiv=\"Content-Type\" content=\"text/html; charset=UTF-8\">  <meta name=\"viewport\" content=\"width=device-width, initial-scale=1.0\">  <meta name=\"x-apple-disable-message-reformatting\">  <!--[if !mso]><!--><meta http-equiv=\"X-UA-Compatible\" content=\"IE=edge\"><!--<![endif]-->  <title></title>      <style type=\"text/css\">      @media only screen and (min-width: 620px) {  .u-row {    width: 600px !important;  }  .u-row .u-col {    vertical-align: top;  }  .u-row .u-col-50 {    width: 300px !important;  }  .u-row .u-col-100 {    width: 600px !important;  }}@media (max-width: 620px) {  .u-row-container {    max-width: 100% !important;    padding-left: 0px !important;    padding-right: 0px !important;  }  .u-row .u-col {    min-width: 320px !important;    max-width: 100% !important;    display: block !important;  }  .u-row {    width: calc(100% - 40px) !important;  }  .u-col {    width: 100% !important;  }  .u-col > div {    margin: 0 auto;  }}body {  margin: 0;  padding: 0;}table,tr,td {  vertical-align: top;  border-collapse: collapse;}p {  margin: 0;}.ie-container table,.mso-container table {  table-layout: fixed;}* {  line-height: inherit;}a[x-apple-data-detectors='true'] {  color: inherit !important;  text-decoration: none !important;}table, td { color: #000000; } #u_body a { color: #161a39; text-decoration: underline; }    </style>    <!--[if !mso]><!--><link href=\"https://fonts.googleapis.com/css?family=Lato:400,700&amp;display=swap\" rel=\"stylesheet\" type=\"text/css\"><!--<![endif]--></head><body class=\"clean-body u_body\" style=\"margin: 0;padding: 0;-webkit-text-size-adjust: 100%;background-color: #f9f9f9;color: #000000\">  <!--[if IE]><div class=\"ie-container\"><![endif]-->  <!--[if mso]><div class=\"mso-container\"><![endif]-->  <table id=\"u_body\" style=\"border-collapse: collapse;table-layout: fixed;border-spacing: 0;mso-table-lspace: 0pt;mso-table-rspace: 0pt;vertical-align: top;min-width: 320px;Margin: 0 auto;background-color: #f9f9f9;width:100%\" cellpadding=\"0\" cellspacing=\"0\">  <tbody>  <tr style=\"vertical-align: top\">    <td style=\"word-break: break-word;border-collapse: collapse !important;vertical-align: top\">    <!--[if (mso)|(IE)]><table width=\"100%\" cellpadding=\"0\" cellspacing=\"0\" border=\"0\"><tr><td align=\"center\" style=\"background-color: #f9f9f9;\"><![endif]-->    <div class=\"u-row-container\" style=\"padding: 0px;background-color: transparent\">  <div class=\"u-row\" style=\"Margin: 0 auto;min-width: 320px;max-width: 600px;overflow-wrap: break-word;word-wrap: break-word;word-break: break-word;background-color: #ffffff;\">    <div style=\"border-collapse: collapse;display: table;width: 100%;height: 100%;background-color: transparent;\">      <!--[if (mso)|(IE)]><table width=\"100%\" cellpadding=\"0\" cellspacing=\"0\" border=\"0\"><tr><td style=\"padding: 0px;background-color: transparent;\" align=\"center\"><table cellpadding=\"0\" cellspacing=\"0\" border=\"0\" style=\"width:600px;\"><tr style=\"background-color: #ffffff;\"><![endif]-->      <!--[if (mso)|(IE)]><td align=\"center\" width=\"600\" style=\"width: 600px;padding: 0px;border-top: 0px solid transparent;border-left: 0px solid transparent;border-right: 0px solid transparent;border-bottom: 0px solid transparent;\" valign=\"top\"><![endif]--><div class=\"u-col u-col-100\" style=\"max-width: 320px;min-width: 600px;display: table-cell;vertical-align: top;\">  <div style=\"height: 100%;width: 100% !important;\">  <!--[if (!mso)&(!IE)]><!--><div style=\"height: 100%; padding: 0px;border-top: 0px solid transparent;border-left: 0px solid transparent;border-right: 0px solid transparent;border-bottom: 0px solid transparent;\"><!--<![endif]-->  <table style=\"font-family:'Lato',sans-serif;\" role=\"presentation\" cellpadding=\"0\" cellspacing=\"0\" width=\"100%\" border=\"0\">  <tbody>    <tr>      <td style=\"overflow-wrap:break-word;word-break:break-word;padding:25px 10px;font-family:'Lato',sans-serif;\" align=\"left\">        <table width=\"100%\" cellpadding=\"0\" cellspacing=\"0\" border=\"0\">  <tbody><tr>    <td style=\"padding-right: 0px;padding-left: 0px;\" align=\"center\">            <img align=\"center\" border=\"0\" src=\"http://d5ohkitzini5f.cloudfront.net/notifications/images/logo.png\" alt=\"Image\" title=\"Image\" style=\"outline: none;text-decoration: none;-ms-interpolation-mode: bicubic;clear: both;display: inline-block !important;border: none;height: auto;float: none;width: 29%;max-width: 168.2px;\" width=\"168.2\">          </td>  </tr></tbody></table>      </td>    </tr>  </tbody></table>  <!--[if (!mso)&(!IE)]><!--></div><!--<![endif]-->  </div></div><!--[if (mso)|(IE)]></td><![endif]-->      <!--[if (mso)|(IE)]></tr></table></td></tr></table><![endif]-->    </div>  </div></div><div class=\"u-row-container\" style=\"padding: 0px;background-color: transparent\">  <div class=\"u-row\" style=\"Margin: 0 auto;min-width: 320px;max-width: 600px;overflow-wrap: break-word;word-wrap: break-word;word-break: break-word;background-color: #d92027;\">    <div style=\"border-collapse: collapse;display: table;width: 100%;height: 100%;background-color: transparent;\">      <!--[if (mso)|(IE)]><table width=\"100%\" cellpadding=\"0\" cellspacing=\"0\" border=\"0\"><tr><td style=\"padding: 0px;background-color: transparent;\" align=\"center\"><table cellpadding=\"0\" cellspacing=\"0\" border=\"0\" style=\"width:600px;\"><tr style=\"background-color: #d92027;\"><![endif]-->      <!--[if (mso)|(IE)]><td align=\"center\" width=\"600\" style=\"width: 600px;padding: 0px;border-top: 0px solid transparent;border-left: 0px solid transparent;border-right: 0px solid transparent;border-bottom: 0px solid transparent;\" valign=\"top\"><![endif]--><div class=\"u-col u-col-100\" style=\"max-width: 320px;min-width: 600px;display: table-cell;vertical-align: top;\">  <div style=\"height: 100%;width: 100% !important;\">  <!--[if (!mso)&(!IE)]><!--><div style=\"height: 100%; padding: 0px;border-top: 0px solid transparent;border-left: 0px solid transparent;border-right: 0px solid transparent;border-bottom: 0px solid transparent;\"><!--<![endif]-->  <table style=\"font-family:'Lato',sans-serif;\" role=\"presentation\" cellpadding=\"0\" cellspacing=\"0\" width=\"100%\" border=\"0\">  <tbody>    <tr>      <td style=\"overflow-wrap:break-word;word-break:break-word;padding:30px 10px;font-family:'Lato',sans-serif;\" align=\"left\">          <div style=\"line-height: 140%; text-align: left; word-wrap: break-word;\">    <p style=\"font-size: 14px; line-height: 140%; text-align: center;\"><span style=\"font-size: 28px; line-height: 39.2px; color: #ffffff; font-family: Lato, sans-serif;\">Thanks for Booking</span></p>  </div>      </td>    </tr>  </tbody></table>  <!--[if (!mso)&(!IE)]><!--></div><!--<![endif]-->  </div></div><!--[if (mso)|(IE)]></td><![endif]-->      <!--[if (mso)|(IE)]></tr></table></td></tr></table><![endif]-->    </div>  </div></div><div class=\"u-row-container\" style=\"padding: 0px;background-color: transparent\">  <div class=\"u-row\" style=\"Margin: 0 auto;min-width: 320px;max-width: 600px;overflow-wrap: break-word;word-wrap: break-word;word-break: break-word;background-color: #ffffff;\">    <div style=\"border-collapse: collapse;display: table;width: 100%;height: 100%;background-color: transparent;\">      <!--[if (mso)|(IE)]><table width=\"100%\" cellpadding=\"0\" cellspacing=\"0\" border=\"0\"><tr><td style=\"padding: 0px;background-color: transparent;\" align=\"center\"><table cellpadding=\"0\" cellspacing=\"0\" border=\"0\" style=\"width:600px;\"><tr style=\"background-color: #ffffff;\"><![endif]-->      <!--[if (mso)|(IE)]><td align=\"center\" width=\"600\" style=\"width: 600px;padding: 0px;border-top: 0px solid transparent;border-left: 0px solid transparent;border-right: 0px solid transparent;border-bottom: 0px solid transparent;\" valign=\"top\"><![endif]--><div class=\"u-col u-col-100\" style=\"max-width: 320px;min-width: 600px;display: table-cell;vertical-align: top;\">  <div style=\"height: 100%;width: 100% !important;\">  <!--[if (!mso)&(!IE)]><!--><div style=\"height: 100%; padding: 0px;border-top: 0px solid transparent;border-left: 0px solid transparent;border-right: 0px solid transparent;border-bottom: 0px solid transparent;\"><!--<![endif]-->  <table style=\"font-family:'Lato',sans-serif;\" role=\"presentation\" cellpadding=\"0\" cellspacing=\"0\" width=\"100%\" border=\"0\">  <tbody>    <tr>      <td style=\"overflow-wrap:break-word;word-break:break-word;padding:40px 40px 30px;font-family:'Lato',sans-serif;\" align=\"left\">          <div style=\"line-height: 140%; text-align: left; word-wrap: break-word;\">    <p style=\"font-size: 14px; line-height: 140%;\"><span style=\"font-size: 18px; line-height: 25.2px; color: #666666;\">Hello #Name,</span></p><p style=\"font-size: 14px; line-height: 140%;\">&nbsp;</p><p style=\"font-size: 14px; line-height: 140%;\"><span style=\"font-size: 18px; line-height: 25.2px; color: #666666;\">Thank you for booking with us. Your Booking has been proceed. You can access your booking by clicking on below given URL</span></p><p style=\"font-size: 14px; line-height: 140%;\">&nbsp;</p><p style=\"font-size: 14px; line-height: 140%;\"><span style=\"font-size: 18px; line-height: 25.2px; color: #666666;\"><b>Refrence No. : #RefNo</b></span></p>  </div>      </td>    </tr>  </tbody></table><table style=\"font-family:'Lato',sans-serif;\" role=\"presentation\" cellpadding=\"0\" cellspacing=\"0\" width=\"100%\" border=\"0\">  <tbody>    <tr>      <td style=\"overflow-wrap:break-word;word-break:break-word;padding:0px 40px;font-family:'Lato',sans-serif;\" align=\"left\">        <div align=\"left\">  <!--[if mso]><table width=\"100%\" cellpadding=\"0\" cellspacing=\"0\" border=\"0\" style=\"border-spacing: 0; border-collapse: collapse; mso-table-lspace:0pt; mso-table-rspace:0pt;font-family:'Lato',sans-serif;\"><tr><td style=\"font-family:'Lato',sans-serif;\" align=\"left\"><v:roundrect xmlns:v=\"urn:schemas-microsoft-com:vml\" xmlns:w=\"urn:schemas-microsoft-com:office:word\" href=\"\" style=\"height:52px; v-text-anchor:middle; width:205px;\" arcsize=\"2%\" stroke=\"f\" fillcolor=\"#d92027\"><w:anchorlock/><center style=\"color:#FFFFFF;font-family:'Lato',sans-serif;\"><![endif]-->    <a href=\"#AccessURL\" target=\"_blank\" style=\"box-sizing: border-box;display: inline-block;font-family:'Lato',sans-serif;text-decoration: none;-webkit-text-size-adjust: none;text-align: center;color: #FFFFFF; background-color: #d92027; border-radius: 1px;-webkit-border-radius: 1px; -moz-border-radius: 1px; width:auto; max-width:100%; overflow-wrap: break-word; word-break: break-word; word-wrap:break-word; mso-border-alt: none;\">      <span style=\"display:block;padding:15px 40px;line-height:120%;\"><span style=\"font-size: 18px; line-height: 21.6px;\">Access Booking</span></span>    </a>  <!--[if mso]></center></v:roundrect></td></tr></table><![endif]--></div>      </td>    </tr>  </tbody></table><table style=\"font-family:'Lato',sans-serif;\" role=\"presentation\" cellpadding=\"0\" cellspacing=\"0\" width=\"100%\" border=\"0\">  <tbody>    <tr>      <td style=\"overflow-wrap:break-word;word-break:break-word;padding:40px 40px 30px;font-family:'Lato',sans-serif;\" align=\"left\">          <div style=\"line-height: 140%; text-align: left; word-wrap: break-word;\">    <p style=\"font-size: 14px; line-height: 140%;\"><span style=\"color: #888888; font-size: 14px; line-height: 19.6px;\"><em><span style=\"font-size: 16px; line-height: 22.4px;\">It’s arrived, <b>Next Holidays</b> you’ve been waiting for....</span></em></span><br><span style=\"color: #888888; font-size: 14px; line-height: 19.6px;\"><em><span style=\"font-size: 16px; line-height: 22.4px;\">&nbsp;</span></em></span></p>  </div>      </td>    </tr>  </tbody></table>  <!--[if (!mso)&(!IE)]><!--></div><!--<![endif]-->  </div></div><!--[if (mso)|(IE)]></td><![endif]-->      <!--[if (mso)|(IE)]></tr></table></td></tr></table><![endif]-->    </div>  </div></div><div class=\"u-row-container\" style=\"padding: 0px;background-color: transparent\">  <div class=\"u-row\" style=\"Margin: 0 auto;min-width: 320px;max-width: 600px;overflow-wrap: break-word;word-wrap: break-word;word-break: break-word;background-color: #d92027;\">    <div style=\"border-collapse: collapse;display: table;width: 100%;height: 100%;background-color: transparent;\">      <!--[if (mso)|(IE)]><table width=\"100%\" cellpadding=\"0\" cellspacing=\"0\" border=\"0\"><tr><td style=\"padding: 0px;background-color: transparent;\" align=\"center\"><table cellpadding=\"0\" cellspacing=\"0\" border=\"0\" style=\"width:600px;\"><tr style=\"background-color: #d92027;\"><![endif]-->      <!--[if (mso)|(IE)]><td align=\"center\" width=\"300\" style=\"width: 300px;padding: 20px 20px 0px;border-top: 0px solid transparent;border-left: 0px solid transparent;border-right: 0px solid transparent;border-bottom: 0px solid transparent;\" valign=\"top\"><![endif]--><div class=\"u-col u-col-50\" style=\"max-width: 320px;min-width: 300px;display: table-cell;vertical-align: top;\">  <div style=\"height: 100%;width: 100% !important;\">  <!--[if (!mso)&(!IE)]><!--><div style=\"height: 100%; padding: 20px 20px 0px;border-top: 0px solid transparent;border-left: 0px solid transparent;border-right: 0px solid transparent;border-bottom: 0px solid transparent;\"><!--<![endif]-->  <table style=\"font-family:'Lato',sans-serif;\" role=\"presentation\" cellpadding=\"0\" cellspacing=\"0\" width=\"100%\" border=\"0\">  <tbody>    <tr>      <td style=\"overflow-wrap:break-word;word-break:break-word;padding:10px;font-family:'Lato',sans-serif;\" align=\"left\">          <div style=\"line-height: 140%; text-align: left; word-wrap: break-word;\">    <p style=\"font-size: 14px; line-height: 140%;\"><span style=\"font-size: 16px; line-height: 22.4px; color: #ecf0f1;\">Contact</span></p><p style=\"font-size: 14px; line-height: 140%;\"><span style=\"font-size: 14px; line-height: 19.6px; color: #ecf0f1;\">1210, Regal Tower, Business Bay, Dubai</span></p><p style=\"font-size: 14px; line-height: 140%;\"><span style='font-size:14px;line-height:19.6px;color:#ecf0f1'>+971 4 770 7355<br><a href='mailto:customer.service@nextholidays.com' target='_blank' style='color: white;'>customer.service@nextholidays.com</a></span></p>  </div>      </td>    </tr>  </tbody></table>  <!--[if (!mso)&(!IE)]><!--></div><!--<![endif]-->  </div></div><!--[if (mso)|(IE)]></td><![endif]--><!--[if (mso)|(IE)]><td align=\"center\" width=\"300\" style=\"width: 300px;padding: 0px 0px 0px 20px;border-top: 0px solid transparent;border-left: 0px solid transparent;border-right: 0px solid transparent;border-bottom: 0px solid transparent;\" valign=\"top\"><![endif]--><div class=\"u-col u-col-50\" style=\"max-width: 320px;min-width: 300px;display: table-cell;vertical-align: top;\">  <div style=\"height: 100%;width: 100% !important;\">  <!--[if (!mso)&(!IE)]><!--><div style=\"height: 100%; padding: 0px 0px 0px 20px;border-top: 0px solid transparent;border-left: 0px solid transparent;border-right: 0px solid transparent;border-bottom: 0px solid transparent;\"><!--<![endif]-->  <table style=\"font-family:'Lato',sans-serif;\" role=\"presentation\" cellpadding=\"0\" cellspacing=\"0\" width=\"100%\" border=\"0\">  <tbody>    <tr>      <td style=\"overflow-wrap:break-word;word-break:break-word;padding:25px 10px 10px;font-family:'Lato',sans-serif;\" align=\"left\">        <div align=\"left\">  <div style=\"display: table; max-width:187px;\">  <!--[if (mso)|(IE)]><table width=\"187\" cellpadding=\"0\" cellspacing=\"0\" border=\"0\"><tr><td style=\"border-collapse:collapse;\" align=\"left\"><table width=\"100%\" cellpadding=\"0\" cellspacing=\"0\" border=\"0\" style=\"border-collapse:collapse; mso-table-lspace: 0pt;mso-table-rspace: 0pt; width:187px;\"><tr><![endif]-->          <!--[if (mso)|(IE)]><td width=\"32\" style=\"width:32px; padding-right: 15px;\" valign=\"top\"><![endif]-->    <table align=\"left\" border=\"0\" cellspacing=\"0\" cellpadding=\"0\" width=\"32\" height=\"32\" style=\"width: 32px !important;height: 32px !important;display: inline-block;border-collapse: collapse;table-layout: fixed;border-spacing: 0;mso-table-lspace: 0pt;mso-table-rspace: 0pt;vertical-align: top;margin-right: 15px\">      <tbody><tr style=\"vertical-align: top\"><td align=\"left\" valign=\"middle\" style=\"word-break: break-word;border-collapse: collapse !important;vertical-align: top\">       <a href=\"http://facebook.com/nextholidayscom\" title=\"Facebook\" target=\"_blank\">          <img src=\"https://d5ohkitzini5f.cloudfront.net/notifications/images/image-1.png\" alt=\"Facebook\" title=\"Facebook\" width=\"32\" style=\"outline: none;text-decoration: none;-ms-interpolation-mode: bicubic;clear: both;display: block !important;border: none;height: auto;float: none;max-width: 32px !important\">        </a>      </td></tr>    </tbody></table>    <!--[if (mso)|(IE)]></td><![endif]-->        <!--[if (mso)|(IE)]><td width=\"32\" style=\"width:32px; padding-right: 15px;\" valign=\"top\"><![endif]-->    <table align=\"left\" border=\"0\" cellspacing=\"0\" cellpadding=\"0\" width=\"32\" height=\"32\" style=\"width: 32px !important;height: 32px !important;display: inline-block;border-collapse: collapse;table-layout: fixed;border-spacing: 0;mso-table-lspace: 0pt;mso-table-rspace: 0pt;vertical-align: top;margin-right: 15px\">      <tbody><tr style=\"vertical-align: top\"><td align=\"left\" valign=\"middle\" style=\"word-break: break-word;border-collapse: collapse !important;vertical-align: top\">        <a href=\"http://twitter.com/nextholidayscom\" title=\"Twitter\" target=\"_blank\">          <img src=\"https://d5ohkitzini5f.cloudfront.net/notifications/images/image-2.png\" alt=\"Twitter\" title=\"Twitter\" width=\"32\" style=\"outline: none;text-decoration: none;-ms-interpolation-mode: bicubic;clear: both;display: block !important;border: none;height: auto;float: none;max-width: 32px !important\">        </a>      </td></tr>    </tbody></table>    <!--[if (mso)|(IE)]></td><![endif]-->        <!--[if (mso)|(IE)]><td width=\"32\" style=\"width:32px; padding-right: 15px;\" valign=\"top\"><![endif]-->    <table align=\"left\" border=\"0\" cellspacing=\"0\" cellpadding=\"0\" width=\"32\" height=\"32\" style=\"width: 32px !important;height: 32px !important;display: inline-block;border-collapse: collapse;table-layout: fixed;border-spacing: 0;mso-table-lspace: 0pt;mso-table-rspace: 0pt;vertical-align: top;margin-right: 15px\">      <tbody><tr style=\"vertical-align: top\"><td align=\"left\" valign=\"middle\" style=\"word-break: break-word;border-collapse: collapse !important;vertical-align: top\">        <a href=\"http://instagram.com/nextholidayscom\" title=\"Instagram\" target=\"_blank\">          <img src=\"https://d5ohkitzini5f.cloudfront.net/notifications/images/image-3.png\" alt=\"Instagram\" title=\"Instagram\" width=\"32\" style=\"outline: none;text-decoration: none;-ms-interpolation-mode: bicubic;clear: both;display: block !important;border: none;height: auto;float: none;max-width: 32px !important\">        </a>      </td></tr>    </tbody></table>    <!--[if (mso)|(IE)]></td><![endif]-->        <!--[if (mso)|(IE)]><td width=\"32\" style=\"width:32px; padding-right: 0px;\" valign=\"top\"><![endif]-->    <table align=\"left\" border=\"0\" cellspacing=\"0\" cellpadding=\"0\" width=\"32\" height=\"32\" style=\"width: 32px !important;height: 32px !important;display: inline-block;border-collapse: collapse;table-layout: fixed;border-spacing: 0;mso-table-lspace: 0pt;mso-table-rspace: 0pt;vertical-align: top;margin-right: 0px\">      <tbody><tr style=\"vertical-align: top\"><td align=\"left\" valign=\"middle\" style=\"word-break: break-word;border-collapse: collapse !important;vertical-align: top\">        <a href=\"http://linkedin.com/company/nextholidayscom\" title=\"LinkedIn\" target=\"_blank\">          <img src=\"https://d5ohkitzini5f.cloudfront.net/notifications/images/image-4.png\" alt=\"LinkedIn\" title=\"LinkedIn\" width=\"32\" style=\"outline: none;text-decoration: none;-ms-interpolation-mode: bicubic;clear: both;display: block !important;border: none;height: auto;float: none;max-width: 32px !important\">        </a>      </td></tr>    </tbody></table>    <!--[if (mso)|(IE)]></td><![endif]-->            <!--[if (mso)|(IE)]></tr></table></td></tr></table><![endif]-->  </div></div>      </td>    </tr>  </tbody></table><table style=\"font-family:'Lato',sans-serif;\" role=\"presentation\" cellpadding=\"0\" cellspacing=\"0\" width=\"100%\" border=\"0\">  <tbody>    <tr>      <td style=\"overflow-wrap:break-word;word-break:break-word;padding:5px 10px 10px;font-family:'Lato',sans-serif;\" align=\"left\">          <div style=\"line-height: 140%; text-align: left; word-wrap: break-word;\">    <p style=\"line-height: 140%; font-size: 14px;\"><span style=\"font-size: 14px; line-height: 19.6px;\"><span style=\"color: #ecf0f1; font-size: 14px; line-height: 19.6px;\"><span style=\"line-height: 19.6px; font-size: 14px;\">Next Holidays ©&nbsp; All Rights Reserved</span></span></span></p>  </div>      </td>    </tr>  </tbody></table>  <!--[if (!mso)&(!IE)]><!--></div><!--<![endif]-->  </div></div><!--[if (mso)|(IE)]></td><![endif]-->      <!--[if (mso)|(IE)]></tr></table></td></tr></table><![endif]-->    </div>  </div></div>    <!--[if (mso)|(IE)]></td></tr></table><![endif]-->    </td>  </tr>  </tbody>  </table>  <!--[if mso]></div><![endif]-->  <!--[if IE]></div><![endif]--></body></html>";
				HTMLEMail = HTMLEMail.Replace("#Name", data.FirstName);
				HTMLEMail = HTMLEMail.Replace("#RefNo", RefrenceNo);
				HTMLEMail = HTMLEMail.Replace("#AccessURL", ("http://www.nextholidays.com/Booking?order=" + encrypt(RefrenceNo)));
				Common.send_email("customer.service@nextholidays.com", "Next Holidays", data.Email, "", "Thanks for Booking with us.", HTMLEMail);
				return new MessageStatus()
				{
					Message = "/Booking?order=" + encrypt(RefrenceNo),
					Status = 1
				};
			}
			catch (Exception ex)
			{
				return new MessageStatus()
				{
					Status = 0,
					Message = "Something went wrong"
				};
			}
		}

		public static string encrypt(string encryptString)
		{
			string EncryptionKey = "0123456789ABCDEFGHIJKWMNOPQRSTUVWXYZ";
			byte[] clearBytes = Encoding.Unicode.GetBytes(encryptString);
			using (Aes encryptor = Aes.Create())
			{
				Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(EncryptionKey, new byte[] {
			0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76
		});
				encryptor.Key = pdb.GetBytes(32);
				encryptor.IV = pdb.GetBytes(16);
				using (MemoryStream ms = new MemoryStream())
				{
					using (CryptoStream cs = new CryptoStream(ms, encryptor.CreateEncryptor(), CryptoStreamMode.Write))
					{
						cs.Write(clearBytes, 0, clearBytes.Length);
						cs.Close();
					}
					encryptString = Convert.ToBase64String(ms.ToArray());
				}
			}
			return encryptString;
		}

	}

    public class OrderDataEnt
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Mobile { get; set; }
        public double TotalAmount { get; set; }
    }
	public class BookingEnt
	{
		public string UserId { get; set; }
		public string TempUserId { get; set; }
		public string CartIds { get; set; }
		public string IPAddress { get; set; }
		public string FirstName { get; set; }
		public string LastName { get; set; }
		public string Gender { get; set; }
		public string CountryCode { get; set; }
		public string MobileNumber { get; set; }
		public string Email { get; set; }
		public string Country { get; set; }
		public string PaymentMethod { get; set; }
		public string PaymentID { get; set; }
		public int CouponId { get; set; }
		public int MakePending { get; set; }
		public string DepDate { get; set; }
		public string ArrCity { get; set; }
		public string DepCity { get; set; }
		public string DepAirLine { get; set; }
		public string IdNumber { get; set; }
		public string IdType { get; set; }
		public string DOB { get; set; }
		public string HearAboutUs { get; set; }
		public string SessionId { get; set; }

	}
	public class CartEnt
	{
		public string rctno { get; set; }
		public int UserId { get; set; }
		public string TempUserId { get; set; }
		public string CartIds { get; set; }
		public double amount { get; set; }
		public string currency {  get; set; }
		public string orderid { get; set; }
		public string mobile { get; set; }
		public string name { get; set; }
		public string email { get; set; }
		public string gender { get; set; }
		public int countrycode { get; set; }
		public string country { get; set; }
	}
}
