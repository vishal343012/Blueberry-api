using Backend.Models;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Twilio;
using Twilio.Rest.Api.V2010.Account;
using Twilio.Types;

namespace NextHolidaysIn.Models
{
    public class User
    {
        private static readonly IHttpContextAccessor _httpContextAccessor;
        
        public static MessageStatus RegisterUser(UsersEnt data)
        {
            try
            {
                DBConnection db = new DBConnection();
                string SQL = "SELECT COUNT(1) from AccountMaster with(nolock) Where EmailId='" + data.EmailId + "' and ISNULL(IsDeleted,0) = 0";
                int eMailCount = Convert.ToInt32(db.GetExecuteScalarQry(SQL, new SqlParameter[] { }));
                if (eMailCount == 0)
                {
                    SqlParameter[] para = new SqlParameter[] {
                            new SqlParameter("@UserId",data.UserId),
                            new SqlParameter("@FirstName",data.FirstName),
                            new SqlParameter("@LastName",data.LastName),
                            new SqlParameter("@Gender",data.Gender),
                            new SqlParameter("@EmailId",data.EmailId),
                            new SqlParameter("@CountryCode",data.CountryCode),
                            new SqlParameter("@MobileNumber",data.MobileNumber),
                            new SqlParameter("@CountryId",data.CountryId),
                            new SqlParameter("@CityId",data.CityId),
                            new SqlParameter("@IsActive",data.IsActive),
                            new SqlParameter("@Password",encryptP(data.Password)),
                            new SqlParameter("@DOB",data.DOB),
                            new SqlParameter("@HearAboutUs",data.HearAboutUs)
                        };
                    int UserId = Convert.ToInt32(db.GetExecuteScalarSP("SPF_CreateUpdateAccount", para));
                    if (UserId > 0)
                    {
                       
                        if (data.IsActive == 0)
                        {
                            return new MessageStatus()
                            {
                                Status = 2,
                                Message = encryptP(UserId.ToString()),
                                UserId = Convert.ToInt32(UserId)
                            };
                        }
                        else
                        {
                            return new MessageStatus()
                            {
                                Status = 1,
                                Message = "Account Created Successfully.",
                                UserId = Convert.ToInt32(UserId)
                            };
                        }

                    }
                    else
                    {
                        return new MessageStatus()
                        {
                            Status = 0,
                            Message = "Unable to register user."
                        };
                    }

                }
                else
                {
                    return new MessageStatus()
                    {
                        Status = 0,
                        Message = "Email already registered."
                    };
                }
            }
            catch (Exception ex)
            {
                return new MessageStatus()
                {
                    Status = 0,
                    Message = "Unable to register user."
                };
            }
        }
        public static int GetCountryId(string Name)
        {
            try
            {
                DBConnection db = new DBConnection();
                SqlParameter[] parameters = new SqlParameter[]
               {
                    new SqlParameter("@Name",Name),
               };
                return Convert.ToInt32(db.GetExecuteScalarSP("SPF_CountryID", parameters));
            }


            catch (Exception ex)
            {
                // Log the exception if needed
                return 0;
            }
        }
        public static int GetCityId(string Name)
        {
            try
            {
                DBConnection db = new DBConnection();
                SqlParameter[] parameters = new SqlParameter[]
               {
                    new SqlParameter("@Name",Name),
               };
                return Convert.ToInt32(db.GetExecuteScalarSP("SPF_CityID", parameters));
            }


            catch (Exception ex)
            {
                // Log the exception if needed
                return 0;
            }
        }
        public static MessageStatus LoginUser(string EmailId, string Password)
        {
            try
            {
                DBConnection db = new DBConnection();

                SqlParameter[] para = new SqlParameter[] {
                new SqlParameter("@Email", EmailId),
                new SqlParameter("@Password", encryptP(Password))
            };

                DataTable dt = db.GetDataTable("Login_User", para);
                if (dt.Rows.Count > 0)
                {
                    if (Convert.ToInt32(dt.Rows[0]["Status"]) == 0)
                    {
                        return new MessageStatus()
                        {
                            Status = 0,
                            Message = Convert.ToString(dt.Rows[0]["Message"])
                        };
                    }
                    else
                    {
                        int isActive = Convert.ToInt32(dt.Rows[0]["IsActive"]);
                        int userId = Convert.ToInt32(dt.Rows[0]["UserId"]);
                        int isB2B = Convert.ToInt32(dt.Rows[0]["IsB2B"]);
                        string firstName = Convert.ToString(dt.Rows[0]["FirstName"]);

                        if (isActive == 0)
                        {
                            return new MessageStatus()
                            {
                                Status = 2,
                                Message = encryptP(userId.ToString())
                            };
                        }
                        else
                        {

                            //var session = _httpContextAccessor.HttpContext.Session;
                            //session.SetInt32("UserId", userId);
                            //session.SetString("UserName", firstName);
                            //session.SetInt32("IsB2B", isB2B);
                           //MoveTempCartData();

                            //Testing for multiple users

                            return new MessageStatus()
                            {
                                Status = 1,
                                Message = "Login Successfully.",
                                UserId = userId
                            };
                        }
                    }
                }
                else
                {
                    return new MessageStatus()
                    {
                        Status = 0,
                        Message = "Unable to login user."
                    };
                }
            }
            catch (Exception ex)
            {
                return new MessageStatus()
                {
                    Status = 0,
                    Message = ex+" Unable to login user."
                };
            }
        }
        public static MessageStatusAccount LoginUserApp(UsersEntApp data)
        {
            try
            {
                DBConnection db = new DBConnection();
                SqlParameter[] para = new SqlParameter[] {
                        new SqlParameter("@Email",data.EmailId),
                        new SqlParameter("@Password",encryptP(data.Password))
                    };
                DataTable dt = (db.GetDataTable("Login_User", para));
                if (dt.Rows.Count > 0)
                {
                    if (Convert.ToInt32(dt.Rows[0]["Status"]) == 0)
                    {
                        return new MessageStatusAccount()
                        {
                            Status = 0,
                            Message = Convert.ToString(dt.Rows[0]["Message"])
                        };
                    }
                    else
                    {
                        int IsActive = Convert.ToInt32(dt.Rows[0]["IsActive"]);
                        int UserId = Convert.ToInt32(dt.Rows[0]["UserId"]);
                        string FirstName = Convert.ToString(dt.Rows[0]["FirstName"]);

                        if (IsActive == 0)
                        {
                            return new MessageStatusAccount()
                            {
                                Status = 2,
                                Message = encryptP(UserId.ToString()),
                                UserId = UserId

                            };
                        }
                        else
                        {

                            return new MessageStatusAccount()
                            {
                                Status = 1,
                                Message = "Login Successfully.",
                                UserId = UserId
                            };
                        }
                    }

                }
                else
                {
                    return new MessageStatusAccount()
                    {
                        Status = 0,
                        Message = "Unable to login user."
                    };
                }


            }
            catch (Exception ex)
            {
                return new MessageStatusAccount()
                {
                    Status = 0,
                    Message = "Unable to login user."
                };
            }
        }
        public static List<Dictionary<string, object>> GetCountryList()
        {
            try
            {
                SqlParameter[] para = new SqlParameter[] {
                    };
                DBConnection db = new DBConnection();
                return HTMLExtensions.GetTableRows(db.GetDataTable("SPF_CountryList", para));
            }
            catch (Exception ex)
            {
                return new List<Dictionary<string, object>>();
            }
        }

        public static List<Dictionary<string, object>> GetCityList(int CountryId)
        {
            try
            {
                SqlParameter[] para = new SqlParameter[] {
                    new SqlParameter("@CountryId",CountryId)
                    };
                DBConnection db = new DBConnection();
                return HTMLExtensions.GetTableRows(db.GetDataTable("SPF_CityList", para));
            }
            catch (Exception ex)
            {
                return new List<Dictionary<string, object>>();
            }
        }

		public static async Task<List<ReturnCartEnt>> GetShortCartList(string t, int u)
		{
			try
			{
                t = "";//added by gautam
				DBConnection db = new DBConnection();
				CancellationToken cts = new CancellationToken();
				SqlParameter[] para = new SqlParameter[] {
					new SqlParameter("@TempUserId",t),
					new SqlParameter("@UserId",u)
				};
				DataSet ds = await Task.Run(() =>
				{
					return db.GetDataSetAsync("SPF_GetShortCart", para, cts, 10000);
				}, cts);
				List<ReturnCartEnt> cartdata = HTMLExtensions.ToList<ReturnCartEnt>(ds.Tables[0]);
				foreach (ReturnCartEnt item in cartdata)
				{
					if (item.Type == "Tour")
						item.CartDetails = HTMLExtensions.GetTableRows(ds.Tables[1].Select("CartId = " + item.CartId).CopyToDataTable());
					if (item.Type == "Visa")
						item.CartDetails = HTMLExtensions.GetTableRows(ds.Tables[2].Select("CartId = " + item.CartId).CopyToDataTable());
					if (item.Type == "Staycation")
						item.CartDetails = HTMLExtensions.GetTableRows(ds.Tables[3].Select("CartId = " + item.CartId).CopyToDataTable());
					if (item.Type == "Insurance")
						item.CartDetails = HTMLExtensions.GetTableRows(ds.Tables[4].Select("CartId = " + item.CartId).CopyToDataTable());
					if (item.Type == "Package")
						item.CartDetails = HTMLExtensions.GetTableRows(ds.Tables[5].Select("CartId = " + item.CartId).CopyToDataTable());
					if (item.Type == "Transfer")
						item.CartDetails = HTMLExtensions.GetTableRows(ds.Tables[6].Select("CartId = " + item.CartId).CopyToDataTable());
				}
				return cartdata;
			}
			catch (Exception ex)
			{
				return null;
			}
		}

		public static MessageStatus GenerateOTP(VerificationEnt data)
        {
            try
            {
                DBConnection db = new DBConnection();
                string SQL = "SELECT UserId from AccountMaster with(nolock) Where EmailId='" + data.EmailId + "'";
                int eMailCount = Convert.ToInt32(db.GetExecuteScalarQry(SQL, new SqlParameter[] { }));
                //int UserId = Convert.ToInt32(User.DecryptP(data.UserId));
                DataTable dt = db.GetDataTable("GenerateVerificationCode", new System.Data.SqlClient.SqlParameter[] {
                new System.Data.SqlClient.SqlParameter("@UserId",eMailCount)
            });

                if (dt.Rows.Count == 0)
                {
                    return new MessageStatus()
                    {
                        Status = 0,
                        Message = "Something went wrong."
                    };
                }

                string Message = Convert.ToString(dt.Rows[0]["Message"]);

                if (Convert.ToInt32(dt.Rows[0]["Status"]) == 1)
                {
                    string HTMLEMail = "<html xmlns=\"http://www.w3.org/1999/xhtml\" xmlns:v=\"urn:schemas-microsoft-com:vml\" xmlns:o=\"urn:schemas-microsoft-com:office:office\"><head>\n<!--[if gte mso 9]>\n<xml>\n  <o:OfficeDocumentSettings>\n    <o:AllowPNG/>\n    <o:PixelsPerInch>96</o:PixelsPerInch>\n  </o:OfficeDocumentSettings>\n</xml>\n<![endif]-->\n  <meta http-equiv=\"Content-Type\" content=\"text/html; charset=UTF-8\">\n  <meta name=\"viewport\" content=\"width=device-width, initial-scale=1.0\">\n  <meta name=\"x-apple-disable-message-reformatting\">\n  <!--[if !mso]><!--><meta http-equiv=\"X-UA-Compatible\" content=\"IE=edge\"><!--<![endif]-->\n  <title></title>\n  \n    <style type=\"text/css\">\n      @media only screen and (min-width: 620px) {\n  .u-row {\n    width: 600px !important;\n  }\n  .u-row .u-col {\n    vertical-align: top;\n  }\n\n  .u-row .u-col-50 {\n    width: 300px !important;\n  }\n\n  .u-row .u-col-100 {\n    width: 600px !important;\n  }\n\n}\n\n@media (max-width: 620px) {\n  .u-row-container {\n    max-width: 100% !important;\n    padding-left: 0px !important;\n    padding-right: 0px !important;\n  }\n  .u-row .u-col {\n    min-width: 320px !important;\n    max-width: 100% !important;\n    display: block !important;\n  }\n  .u-row {\n    width: calc(100% - 40px) !important;\n  }\n  .u-col {\n    width: 100% !important;\n  }\n  .u-col > div {\n    margin: 0 auto;\n  }\n}\nbody {\n  margin: 0;\n  padding: 0;\n}\n\ntable,\ntr,\ntd {\n  vertical-align: top;\n  border-collapse: collapse;\n}\n\np {\n  margin: 0;\n}\n\n.ie-container table,\n.mso-container table {\n  table-layout: fixed;\n}\n\n* {\n  line-height: inherit;\n}\n\na[x-apple-data-detectors=\'true\'] {\n  color: inherit !important;\n  text-decoration: none !important;\n}\n\ntable, td { color: #000000; } #u_body a { color: #161a39; text-decoration: underline; }\n    </style>\n  \n  \n\n<!--[if !mso]><!--><link href=\"https://fonts.googleapis.com/css?family=Lato:400,700&amp;display=swap\" rel=\"stylesheet\" type=\"text/css\"><!--<![endif]-->\n\n</head>\n\n<body class=\"clean-body u_body\" style=\"margin: 0;padding: 0;-webkit-text-size-adjust: 100%;background-color: #f9f9f9;color: #000000\">\n  <!--[if IE]><div class=\"ie-container\"><![endif]-->\n  <!--[if mso]><div class=\"mso-container\"><![endif]-->\n  <table id=\"u_body\" style=\"border-collapse: collapse;table-layout: fixed;border-spacing: 0;mso-table-lspace: 0pt;mso-table-rspace: 0pt;vertical-align: top;min-width: 320px;Margin: 0 auto;background-color: #f9f9f9;width:100%\" cellpadding=\"0\" cellspacing=\"0\">\n  <tbody>\n  <tr style=\"vertical-align: top\">\n    <td style=\"word-break: break-word;border-collapse: collapse !important;vertical-align: top\">\n    <!--[if (mso)|(IE)]><table width=\"100%\" cellpadding=\"0\" cellspacing=\"0\" border=\"0\"><tr><td align=\"center\" style=\"background-color: #f9f9f9;\"><![endif]-->\n    \n\n<div class=\"u-row-container\" style=\"padding: 0px;background-color: transparent\">\n  <div class=\"u-row\" style=\"Margin: 0 auto;min-width: 320px;max-width: 600px;overflow-wrap: break-word;word-wrap: break-word;word-break: break-word;background-color: #ffffff;\">\n    <div style=\"border-collapse: collapse;display: table;width: 100%;height: 100%;background-color: transparent;\">\n      <!--[if (mso)|(IE)]><table width=\"100%\" cellpadding=\"0\" cellspacing=\"0\" border=\"0\"><tr><td style=\"padding: 0px;background-color: transparent;\" align=\"center\"><table cellpadding=\"0\" cellspacing=\"0\" border=\"0\" style=\"width:600px;\"><tr style=\"background-color: #ffffff;\"><![endif]-->\n      \n<!--[if (mso)|(IE)]><td align=\"center\" width=\"600\" style=\"width: 600px;padding: 0px;border-top: 0px solid transparent;border-left: 0px solid transparent;border-right: 0px solid transparent;border-bottom: 0px solid transparent;\" valign=\"top\"><![endif]-->\n<div class=\"u-col u-col-100\" style=\"max-width: 320px;min-width: 600px;display: table-cell;vertical-align: top;\">\n  <div style=\"height: 100%;width: 100% !important;\">\n  <!--[if (!mso)&(!IE)]><!--><div style=\"height: 100%; padding: 0px;border-top: 0px solid transparent;border-left: 0px solid transparent;border-right: 0px solid transparent;border-bottom: 0px solid transparent;\"><!--<![endif]-->\n  \n<table style=\"font-family:\'Lato\',sans-serif;\" role=\"presentation\" cellpadding=\"0\" cellspacing=\"0\" width=\"100%\" border=\"0\">\n  <tbody>\n    <tr>\n      <td style=\"overflow-wrap:break-word;word-break:break-word;padding:25px 10px;font-family:\'Lato\',sans-serif;\" align=\"left\">\n        \n<table width=\"100%\" cellpadding=\"0\" cellspacing=\"0\" border=\"0\">\n  <tbody><tr>\n    <td style=\"padding-right: 0px;padding-left: 0px;\" align=\"center\">\n      \n      <img align=\"center\" border=\"0\" src=\"http://cdnholidays.blueberrygroup.org/notifications/images/logo.png\" alt=\"Image\" title=\"Image\" style=\"outline: none;text-decoration: none;-ms-interpolation-mode: bicubic;clear: both;display: inline-block !important;border: none;height: auto;float: none;width: 29%;max-width: 168.2px;\" width=\"168.2\">\n      \n    </td>\n  </tr>\n</tbody></table>\n\n      </td>\n    </tr>\n  </tbody>\n</table>\n\n  <!--[if (!mso)&(!IE)]><!--></div><!--<![endif]-->\n  </div>\n</div>\n<!--[if (mso)|(IE)]></td><![endif]-->\n      <!--[if (mso)|(IE)]></tr></table></td></tr></table><![endif]-->\n    </div>\n  </div>\n</div>\n\n\n\n<div class=\"u-row-container\" style=\"padding: 0px;background-color: transparent\">\n  <div class=\"u-row\" style=\"Margin: 0 auto;min-width: 320px;max-width: 600px;overflow-wrap: break-word;word-wrap: break-word;word-break: break-word;background-color: #d92027;\">\n    <div style=\"border-collapse: collapse;display: table;width: 100%;height: 100%;background-color: transparent;\">\n      <!--[if (mso)|(IE)]><table width=\"100%\" cellpadding=\"0\" cellspacing=\"0\" border=\"0\"><tr><td style=\"padding: 0px;background-color: transparent;\" align=\"center\"><table cellpadding=\"0\" cellspacing=\"0\" border=\"0\" style=\"width:600px;\"><tr style=\"background-color: #d92027;\"><![endif]-->\n      \n<!--[if (mso)|(IE)]><td align=\"center\" width=\"600\" style=\"width: 600px;padding: 0px;border-top: 0px solid transparent;border-left: 0px solid transparent;border-right: 0px solid transparent;border-bottom: 0px solid transparent;\" valign=\"top\"><![endif]-->\n<div class=\"u-col u-col-100\" style=\"max-width: 320px;min-width: 600px;display: table-cell;vertical-align: top;\">\n  <div style=\"height: 100%;width: 100% !important;\">\n  <!--[if (!mso)&(!IE)]><!--><div style=\"height: 100%; padding: 0px;border-top: 0px solid transparent;border-left: 0px solid transparent;border-right: 0px solid transparent;border-bottom: 0px solid transparent;\"><!--<![endif]-->\n  \n<table style=\"font-family:\'Lato\',sans-serif;\" role=\"presentation\" cellpadding=\"0\" cellspacing=\"0\" width=\"100%\" border=\"0\">\n  <tbody>\n    <tr>\n      <td style=\"overflow-wrap:break-word;word-break:break-word;padding:30px 10px;font-family:\'Lato\',sans-serif;\" align=\"left\">\n        \n  <div style=\"line-height: 140%; text-align: left; word-wrap: break-word;\">\n    <p style=\"font-size: 14px; line-height: 140%; text-align: center;\"><span style=\"font-size: 28px; line-height: 39.2px; color: #ffffff; font-family: Lato, sans-serif;\">Verification Pending</span></p>\n  </div>\n\n      </td>\n    </tr>\n  </tbody>\n</table>\n\n  <!--[if (!mso)&(!IE)]><!--></div><!--<![endif]-->\n  </div>\n</div>\n<!--[if (mso)|(IE)]></td><![endif]-->\n      <!--[if (mso)|(IE)]></tr></table></td></tr></table><![endif]-->\n    </div>\n  </div>\n</div>\n\n\n\n<div class=\"u-row-container\" style=\"padding: 0px;background-color: transparent\">\n  <div class=\"u-row\" style=\"Margin: 0 auto;min-width: 320px;max-width: 600px;overflow-wrap: break-word;word-wrap: break-word;word-break: break-word;background-color: #ffffff;\">\n    <div style=\"border-collapse: collapse;display: table;width: 100%;height: 100%;background-color: transparent;\">\n      <!--[if (mso)|(IE)]><table width=\"100%\" cellpadding=\"0\" cellspacing=\"0\" border=\"0\"><tr><td style=\"padding: 0px;background-color: transparent;\" align=\"center\"><table cellpadding=\"0\" cellspacing=\"0\" border=\"0\" style=\"width:600px;\"><tr style=\"background-color: #ffffff;\"><![endif]-->\n      \n<!--[if (mso)|(IE)]><td align=\"center\" width=\"600\" style=\"width: 600px;padding: 0px;border-top: 0px solid transparent;border-left: 0px solid transparent;border-right: 0px solid transparent;border-bottom: 0px solid transparent;\" valign=\"top\"><![endif]-->\n<div class=\"u-col u-col-100\" style=\"max-width: 320px;min-width: 600px;display: table-cell;vertical-align: top;\">\n  <div style=\"height: 100%;width: 100% !important;\">\n  <!--[if (!mso)&(!IE)]><!--><div style=\"height: 100%; padding: 0px;border-top: 0px solid transparent;border-left: 0px solid transparent;border-right: 0px solid transparent;border-bottom: 0px solid transparent;\"><!--<![endif]-->\n  \n<table style=\"font-family:\'Lato\',sans-serif;\" role=\"presentation\" cellpadding=\"0\" cellspacing=\"0\" width=\"100%\" border=\"0\">\n  <tbody>\n    <tr>\n      <td style=\"overflow-wrap:break-word;word-break:break-word;padding:40px 40px 30px;font-family:\'Lato\',sans-serif;\" align=\"left\">\n        \n  <div style=\"line-height: 140%; text-align: left; word-wrap: break-word;\">\n    <p style=\"font-size: 14px; line-height: 140%;\"><span style=\"font-size: 18px; line-height: 25.2px; color: #666666;\">Hello,</span></p>\n<p style=\"font-size: 14px; line-height: 140%;\">&nbsp;</p>\n<p style=\"font-size: 14px; line-height: 140%;\"><span style=\"font-size: 18px; line-height: 25.2px; color: #666666;\">We have sent you this email in response to your account activation on Next Holidays.</span></p>\n<p style=\"font-size: 14px; line-height: 140%;\">&nbsp;</p>\n<p style=\"font-size: 14px; line-height: 140%;\"><span style=\"font-size: 18px; line-height: 25.2px; color: #666666;\">To Verify your account, please use below given otp on verification page: </span></p>\n  </div>\n\n      </td>\n    </tr>\n  </tbody>\n</table>\n\n<table style=\"font-family:\'Lato\',sans-serif;\" role=\"presentation\" cellpadding=\"0\" cellspacing=\"0\" width=\"100%\" border=\"0\">\n  <tbody>\n    <tr>\n      <td style=\"overflow-wrap:break-word;word-break:break-word;padding:0px 40px;font-family:\'Lato\',sans-serif;\" align=\"left\">\n        \n<div align=\"left\">\n  <!--[if mso]><table width=\"100%\" cellpadding=\"0\" cellspacing=\"0\" border=\"0\" style=\"border-spacing: 0; border-collapse: collapse; mso-table-lspace:0pt; mso-table-rspace:0pt;font-family:\'Lato\',sans-serif;\"><tr><td style=\"font-family:\'Lato\',sans-serif;\" align=\"left\"><v:roundrect xmlns:v=\"urn:schemas-microsoft-com:vml\" xmlns:w=\"urn:schemas-microsoft-com:office:word\" href=\"\" style=\"height:52px; v-text-anchor:middle; width:205px;\" arcsize=\"2%\" stroke=\"f\" fillcolor=\"#d92027\"><w:anchorlock/><center style=\"color:#FFFFFF;font-family:\'Lato\',sans-serif;\"><![endif]-->\n    <a href=\"javascript:void(0)\" style=\"box-sizing: border-box;display: inline-block;font-family:\'Lato\',sans-serif;text-decoration: none;-webkit-text-size-adjust: none;text-align: center;color: #FFFFFF; background-color: #d92027; border-radius: 1px;-webkit-border-radius: 1px; -moz-border-radius: 1px; width:auto; max-width:100%; overflow-wrap: break-word; word-break: break-word; word-wrap:break-word; mso-border-alt: none;\">\n      <span style=\"display:block;padding:15px 40px;line-height:120%;\"><span style=\"font-size: 18px; line-height: 21.6px;\">#OTP</span></span>\n    </a>\n  <!--[if mso]></center></v:roundrect></td></tr></table><![endif]-->\n</div>\n\n      </td>\n    </tr>\n  </tbody>\n</table>\n\n<table style=\"font-family:\'Lato\',sans-serif;\" role=\"presentation\" cellpadding=\"0\" cellspacing=\"0\" width=\"100%\" border=\"0\">\n  <tbody>\n    <tr>\n      <td style=\"overflow-wrap:break-word;word-break:break-word;padding:40px 40px 30px;font-family:\'Lato\',sans-serif;\" align=\"left\">\n        \n  <div style=\"line-height: 140%; text-align: left; word-wrap: break-word;\">\n    <p style=\"font-size: 14px; line-height: 140%;\"><span style=\"color: #888888; font-size: 14px; line-height: 19.6px;\"><em><span style=\"font-size: 16px; line-height: 22.4px;\">It’s arrived, <b>Next Holidays</b> you’ve been waiting for....</span></em></span><br><span style=\"color: #888888; font-size: 14px; line-height: 19.6px;\"><em><span style=\"font-size: 16px; line-height: 22.4px;\">&nbsp;</span></em></span></p>\n  </div>\n\n      </td>\n    </tr>\n  </tbody>\n</table>\n\n  <!--[if (!mso)&(!IE)]><!--></div><!--<![endif]-->\n  </div>\n</div>\n<!--[if (mso)|(IE)]></td><![endif]-->\n      <!--[if (mso)|(IE)]></tr></table></td></tr></table><![endif]-->\n    </div>\n  </div>\n</div>\n\n\n\n<div class=\"u-row-container\" style=\"padding: 0px;background-color: transparent\">\n  <div class=\"u-row\" style=\"Margin: 0 auto;min-width: 320px;max-width: 600px;overflow-wrap: break-word;word-wrap: break-word;word-break: break-word;background-color: #d92027;\">\n    <div style=\"border-collapse: collapse;display: table;width: 100%;height: 100%;background-color: transparent;\">\n      <!--[if (mso)|(IE)]><table width=\"100%\" cellpadding=\"0\" cellspacing=\"0\" border=\"0\"><tr><td style=\"padding: 0px;background-color: transparent;\" align=\"center\"><table cellpadding=\"0\" cellspacing=\"0\" border=\"0\" style=\"width:600px;\"><tr style=\"background-color: #d92027;\"><![endif]-->\n      \n<!--[if (mso)|(IE)]><td align=\"center\" width=\"300\" style=\"width: 300px;padding: 20px 20px 0px;border-top: 0px solid transparent;border-left: 0px solid transparent;border-right: 0px solid transparent;border-bottom: 0px solid transparent;\" valign=\"top\"><![endif]-->\n<div class=\"u-col u-col-50\" style=\"max-width: 320px;min-width: 300px;display: table-cell;vertical-align: top;\">\n  <div style=\"height: 100%;width: 100% !important;\">\n  <!--[if (!mso)&(!IE)]><!--><div style=\"height: 100%; padding: 20px 20px 0px;border-top: 0px solid transparent;border-left: 0px solid transparent;border-right: 0px solid transparent;border-bottom: 0px solid transparent;\"><!--<![endif]-->\n  \n<table style=\"font-family:\'Lato\',sans-serif;\" role=\"presentation\" cellpadding=\"0\" cellspacing=\"0\" width=\"100%\" border=\"0\">\n  <tbody>\n    <tr>\n      <td style=\"overflow-wrap:break-word;word-break:break-word;padding:10px;font-family:\'Lato\',sans-serif;\" align=\"left\">\n        \n  <div style=\"line-height: 140%; text-align: left; word-wrap: break-word;\">\n    <p style=\"font-size: 14px; line-height: 140%;\"><span style=\"font-size: 16px; line-height: 22.4px; color: #ecf0f1;\">Contact</span></p>\n<p style=\"font-size: 14px; line-height: 140%;\"><span style=\"font-size: 14px; line-height: 19.6px; color: #ecf0f1;\">1210-1211, Regal Tower, Business Bay, Dubai</span></p>\n<p style=\"font-size: 14px; line-height: 140%;\"><span style='font-size:14px;line-height:19.6px;color:#ecf0f1'>+971 4 770 7355<br><a href='mailto:customer.service@nextholidays.com' target='_blank' style='color: white;'>customer.service@nextholidays.com</a></span></p>\n  </div>\n\n      </td>\n    </tr>\n  </tbody>\n</table>\n\n  <!--[if (!mso)&(!IE)]><!--></div><!--<![endif]-->\n  </div>\n</div>\n<!--[if (mso)|(IE)]></td><![endif]-->\n<!--[if (mso)|(IE)]><td align=\"center\" width=\"300\" style=\"width: 300px;padding: 0px 0px 0px 20px;border-top: 0px solid transparent;border-left: 0px solid transparent;border-right: 0px solid transparent;border-bottom: 0px solid transparent;\" valign=\"top\"><![endif]-->\n<div class=\"u-col u-col-50\" style=\"max-width: 320px;min-width: 300px;display: table-cell;vertical-align: top;\">\n  <div style=\"height: 100%;width: 100% !important;\">\n  <!--[if (!mso)&(!IE)]><!--><div style=\"height: 100%; padding: 0px 0px 0px 20px;border-top: 0px solid transparent;border-left: 0px solid transparent;border-right: 0px solid transparent;border-bottom: 0px solid transparent;\"><!--<![endif]-->\n  \n<table style=\"font-family:\'Lato\',sans-serif;\" role=\"presentation\" cellpadding=\"0\" cellspacing=\"0\" width=\"100%\" border=\"0\">\n  <tbody>\n    <tr>\n      <td style=\"overflow-wrap:break-word;word-break:break-word;padding:25px 10px 10px;font-family:\'Lato\',sans-serif;\" align=\"left\">\n        \n<div align=\"left\">\n  <div style=\"display: table; max-width:187px;\">\n  <!--[if (mso)|(IE)]><table width=\"187\" cellpadding=\"0\" cellspacing=\"0\" border=\"0\"><tr><td style=\"border-collapse:collapse;\" align=\"left\"><table width=\"100%\" cellpadding=\"0\" cellspacing=\"0\" border=\"0\" style=\"border-collapse:collapse; mso-table-lspace: 0pt;mso-table-rspace: 0pt; width:187px;\"><tr><![endif]-->\n  \n    \n    <!--[if (mso)|(IE)]><td width=\"32\" style=\"width:32px; padding-right: 15px;\" valign=\"top\"><![endif]-->\n    <table align=\"left\" border=\"0\" cellspacing=\"0\" cellpadding=\"0\" width=\"32\" height=\"32\" style=\"width: 32px !important;height: 32px !important;display: inline-block;border-collapse: collapse;table-layout: fixed;border-spacing: 0;mso-table-lspace: 0pt;mso-table-rspace: 0pt;vertical-align: top;margin-right: 15px\">\n      <tbody><tr style=\"vertical-align: top\"><td align=\"left\" valign=\"middle\" style=\"word-break: break-word;border-collapse: collapse !important;vertical-align: top\">\n        <a href=\"http://facebook.com/nextholidayscom\" title=\"Facebook\" target=\"_blank\">\n          <img src=\"http://cdnholidays.blueberrygroup.org/notifications/images/image-1.png\" alt=\"Facebook\" title=\"Facebook\" width=\"32\" style=\"outline: none;text-decoration: none;-ms-interpolation-mode: bicubic;clear: both;display: block !important;border: none;height: auto;float: none;max-width: 32px !important\">\n        </a>\n      </td></tr>\n    </tbody></table>\n    <!--[if (mso)|(IE)]></td><![endif]-->\n    \n    <!--[if (mso)|(IE)]><td width=\"32\" style=\"width:32px; padding-right: 15px;\" valign=\"top\"><![endif]-->\n    <table align=\"left\" border=\"0\" cellspacing=\"0\" cellpadding=\"0\" width=\"32\" height=\"32\" style=\"width: 32px !important;height: 32px !important;display: inline-block;border-collapse: collapse;table-layout: fixed;border-spacing: 0;mso-table-lspace: 0pt;mso-table-rspace: 0pt;vertical-align: top;margin-right: 15px\">\n      <tbody><tr style=\"vertical-align: top\"><td align=\"left\" valign=\"middle\" style=\"word-break: break-word;border-collapse: collapse !important;vertical-align: top\">\n        <a href=\"http://twitter.com/nextholidayscom\" title=\"Twitter\" target=\"_blank\">\n          <img src=\"http://cdnholidays.blueberrygroup.org/notifications/images/image-2.png\" alt=\"Twitter\" title=\"Twitter\" width=\"32\" style=\"outline: none;text-decoration: none;-ms-interpolation-mode: bicubic;clear: both;display: block !important;border: none;height: auto;float: none;max-width: 32px !important\">\n        </a>\n      </td></tr>\n    </tbody></table>\n    <!--[if (mso)|(IE)]></td><![endif]-->\n    \n    <!--[if (mso)|(IE)]><td width=\"32\" style=\"width:32px; padding-right: 15px;\" valign=\"top\"><![endif]-->\n    <table align=\"left\" border=\"0\" cellspacing=\"0\" cellpadding=\"0\" width=\"32\" height=\"32\" style=\"width: 32px !important;height: 32px !important;display: inline-block;border-collapse: collapse;table-layout: fixed;border-spacing: 0;mso-table-lspace: 0pt;mso-table-rspace: 0pt;vertical-align: top;margin-right: 15px\">\n      <tbody><tr style=\"vertical-align: top\"><td align=\"left\" valign=\"middle\" style=\"word-break: break-word;border-collapse: collapse !important;vertical-align: top\">\n        <a href=\"http://instragram.com/nextholidayscom\" title=\"Instagram\" target=\"_blank\">\n          <img src=\"http://cdnholidays.blueberrygroup.org/notifications/images/image-3.png\" alt=\"Instagram\" title=\"Instagram\" width=\"32\" style=\"outline: none;text-decoration: none;-ms-interpolation-mode: bicubic;clear: both;display: block !important;border: none;height: auto;float: none;max-width: 32px !important\">\n        </a>\n      </td></tr>\n    </tbody></table>\n    <!--[if (mso)|(IE)]></td><![endif]-->\n    \n    <!--[if (mso)|(IE)]><td width=\"32\" style=\"width:32px; padding-right: 0px;\" valign=\"top\"><![endif]-->\n    <table align=\"left\" border=\"0\" cellspacing=\"0\" cellpadding=\"0\" width=\"32\" height=\"32\" style=\"width: 32px !important;height: 32px !important;display: inline-block;border-collapse: collapse;table-layout: fixed;border-spacing: 0;mso-table-lspace: 0pt;mso-table-rspace: 0pt;vertical-align: top;margin-right: 0px\">\n      <tbody><tr style=\"vertical-align: top\"><td align=\"left\" valign=\"middle\" style=\"word-break: break-word;border-collapse: collapse !important;vertical-align: top\">\n        <a href=\"http://linkedin.com/company/nextholidayscom\" title=\"LinkedIn\" target=\"_blank\">\n          <img src=\"http://cdnholidays.blueberrygroup.org/notifications/images/image-4.png\" alt=\"LinkedIn\" title=\"LinkedIn\" width=\"32\" style=\"outline: none;text-decoration: none;-ms-interpolation-mode: bicubic;clear: both;display: block !important;border: none;height: auto;float: none;max-width: 32px !important\">\n        </a>\n      </td></tr>\n    </tbody></table>\n    <!--[if (mso)|(IE)]></td><![endif]-->\n    \n    \n    <!--[if (mso)|(IE)]></tr></table></td></tr></table><![endif]-->\n  </div>\n</div>\n\n      </td>\n    </tr>\n  </tbody>\n</table>\n\n<table style=\"font-family:\'Lato\',sans-serif;\" role=\"presentation\" cellpadding=\"0\" cellspacing=\"0\" width=\"100%\" border=\"0\">\n  <tbody>\n    <tr>\n      <td style=\"overflow-wrap:break-word;word-break:break-word;padding:5px 10px 10px;font-family:\'Lato\',sans-serif;\" align=\"left\">\n        \n  <div style=\"line-height: 140%; text-align: left; word-wrap: break-word;\">\n    <p style=\"line-height: 140%; font-size: 14px;\"><span style=\"font-size: 14px; line-height: 19.6px;\"><span style=\"color: #ecf0f1; font-size: 14px; line-height: 19.6px;\"><span style=\"line-height: 19.6px; font-size: 14px;\">Next Holidays ©&nbsp; All Rights Reserved</span></span></span></p>\n  </div>\n\n      </td>\n    </tr>\n  </tbody>\n</table>\n\n  <!--[if (!mso)&(!IE)]><!--></div><!--<![endif]-->\n  </div>\n</div>\n<!--[if (mso)|(IE)]></td><![endif]-->\n      <!--[if (mso)|(IE)]></tr></table></td></tr></table><![endif]-->\n    </div>\n  </div>\n</div>\n\n\n    <!--[if (mso)|(IE)]></td></tr></table><![endif]-->\n    </td>\n  </tr>\n  </tbody>\n  </table>\n  <!--[if mso]></div><![endif]-->\n  <!--[if IE]></div><![endif]-->\n\n\n\n</body></html>";

                    HTMLEMail = HTMLEMail.Replace("#OTP", Convert.ToString(dt.Rows[0]["OTP"]));
                    if (Convert.ToInt32(dt.Rows[0]["IsMobileSent"]) == 1)
                    {
                        try
                        {
                            System.Net.ServicePointManager.SecurityProtocol |= System.Net.SecurityProtocolType.Tls12 | System.Net.SecurityProtocolType.Tls11 | System.Net.SecurityProtocolType.Tls;

                            var accountSid = "ACc87bd43cb11cc2bf4b4eaffee1c87b6d";
                            var authToken = "ca12acde9803765c0446a2c00d8d4c53";
                            TwilioClient.Init(accountSid, authToken);

                            var messageOptions = new CreateMessageOptions(
                                new PhoneNumber("+" + dt.Rows[0]["CountryCode"] + dt.Rows[0]["MobileNumber"]));
                            messageOptions.MessagingServiceSid = "MGef3c8e55929462623340f9f26c93a7b4";
                            messageOptions.Body = "Your Next Holidays Verification Code is " + Convert.ToString(dt.Rows[0]["OTP"]);

                            var message = MessageResource.Create(messageOptions);
                            Message = "OTP is sent to mobile and email.";
                        }
                        catch (Exception ex) { }
                    }
                    Common.send_email("customer.service@nextholidays.com", "Next Holidays", Convert.ToString(dt.Rows[0]["Email"]), "", "OTP for Activation.", HTMLEMail);
                    return new MessageStatus()
                    {
                        Status = 1,
                        Message = Message
                    };
                }


                return new MessageStatus()
                {
                    Status = 0,
                    Message = Message
                };
            }
            catch (Exception ex)
            {
                return new MessageStatus()
                {
                    Status = 0,
                    Message = "Unable to Generate OTP"
                };
            }
        }
        public static MessageStatus ChangePassword_User(PasswordChangeEnt data)

        {
            try
            {
                DBConnection db = new DBConnection();
                db.ExecuteNonQuery("ChangePassword_User", new SqlParameter[] {
                    new SqlParameter("@UserId",data.UserId),
                    new SqlParameter("@OldPassword",encryptP(data.OldPassword)),
                    new SqlParameter("@NewPassword",encryptP(data.NewPassword))
                });
                return new MessageStatus()
                {
                    Status = 1,
                    Message = "Password Changed"
                };

            }
            catch (Exception ex)
            {
                return new MessageStatus()
                {
                    Status = 0,
                    Message = "Something went wrong."
                };
            }
        }
        public static MessageStatus VerifyOTP(UsersEnt data)
        {
            try
            {
                DBConnection db = new DBConnection();
                int userId = Convert.ToInt32(DecryptP(data.EmailId));
                SqlParameter[] parameters = new SqlParameter[]
                {
                new SqlParameter("@UserId", userId),
                new SqlParameter("@OTP", data.Password),
                };

                int count = Convert.ToInt32(db.GetExecuteScalarSP("VerifyOTP", parameters));
                if (count == 1)
                {
                    //_httpContextAccessor.HttpContext.Session.SetInt32("UserId", userId);
                    MoveTempCartData();
                    return new MessageStatus()
                    {
                        Status = 1,
                        Message = "Account Verified."
                    };
                }
                else
                {
                    return new MessageStatus()
                    {
                        Status = 0,
                        Message = "Wrong OTP."
                    };
                }
            }
            catch
            {
                return new MessageStatus()
                {
                    Status = 0,
                    Message = "Unable to verify OTP"
                };
            }
        }
        public static void MoveTempCartData()
        {
            try
            {
                DBConnection db = new DBConnection();
                db.ExecuteNonQuery("MoveTempUserIdData", new SqlParameter[] {
                    new SqlParameter("@TempUserId",Common.GetTempUserId()),
                    new SqlParameter("@UserId",Common.GetUserId())
                });
                //_httpContextAccessor.HttpContext.Session.Remove("TempUserId");
            }
            catch (Exception ex) { }
        }


        public static string encryptP(string encryptString)
        {
            string EncryptionKey = "01234561503ABCDEFGHIJKWMNOPQRSTUVWXYZ";
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

        public static string DecryptP(string cipherText)
        {
            string EncryptionKey = "01234561503ABCDEFGHIJKWMNOPQRSTUVWXYZ";
            cipherText = cipherText.Replace(" ", "+");
            byte[] cipherBytes = Convert.FromBase64String(cipherText);
            using (Aes encryptor = Aes.Create())
            {
                Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(EncryptionKey, new byte[] {
            0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76
        });
                encryptor.Key = pdb.GetBytes(32);
                encryptor.IV = pdb.GetBytes(16);
                using (MemoryStream ms = new MemoryStream())
                {
                    using (CryptoStream cs = new CryptoStream(ms, encryptor.CreateDecryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(cipherBytes, 0, cipherBytes.Length);
                        cs.Close();
                    }
                    cipherText = Encoding.Unicode.GetString(ms.ToArray());
                }
            }
            return cipherText;
        }
        public static MessageStatus DeleteAccount(MessageStatus data)
        {
            try
            {

                SqlParameter[] para = new SqlParameter[] {
                    new SqlParameter("@UserId",Common.GetUserId()),
                    new SqlParameter("@DeleteReason",data.Message)
                };
                DBConnection db = new DBConnection();
                db.ExecuteNonQuery("SPF_DeleteAccount", para);
                return new MessageStatus()
                {
                    Status = 1,
                    Message = "Account Deleted. We will miss you."
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
        public static MyAccountEnt GetUserData(MyAccountEnt data)
        {
            try
            {
                SqlParameter[] para = new SqlParameter[] {
                //new SqlParameter("@UserId",Common.GetUserId())
                
                new SqlParameter("@UserId",Convert.ToInt32(data.EmailId))
				};
				DBConnection db = new DBConnection();
                DataTable dt = db.GetDataTable("SPF_AccountDetailByUserId", para);
                List<MyAccountEnt> lst = HTMLExtensions.ToList<MyAccountEnt>(dt);
                return lst.FirstOrDefault();
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        public static MessageStatus SaveUserData(MyAccountEnt data)
        {
            try
            {
                var userId = data.UserId;
                SqlParameter[] parameters = new SqlParameter[] {
                new SqlParameter("@FirstName", data.FirstName),
                new SqlParameter("@LastName", data.LastName),
                new SqlParameter("@Gender", data.Gender),
                new SqlParameter("@CountryCode", data.CountryCode),
                new SqlParameter("@MobileNumber", data.MobileNumber),
                new SqlParameter("@CountryId", data.CountryId),
                new SqlParameter("@CityId", data.CityId),
                new SqlParameter("@DOB", data.DOB),
                new SqlParameter("@ProfileImage", data.ProfileImage),
                new SqlParameter("@UserId", userId),
            };
                DBConnection db = new DBConnection();
                db.ExecuteNonQuery("SaveUserData", parameters);

                return new MessageStatus()
                {
                    Status = 1,
                    Message = "Profile Updated."
                };
            }
            catch (Exception ex)
            {
                // Log the exception
                return new MessageStatus()
                {
                    Status = 0,
                    Message = "Something went wrong."
                };
            }
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

    }

	public class MessageStatusAccount
	{
		public int Status { get; set; }
		public string Message { get; set; }
		public int UserId { get; set; }
	}

	public class ReturnCartEnt
	{
		public int CartId { get; set; }
		public string Type { get; set; }
		public string ServiceName { get; set; }
		public int ServiceId { get; set; }
		public string Image { get; set; }
		public decimal Price { get; set; }
		public int Pax { get; set; }
		public string OptionName { get; set; }
		public List<Dictionary<string, object>> CartDetails { get; set; }
	}
	public class UsersEntApp
	{
		public string EmailId { get; set; }
		public string Password { get; set; }
	}
    public class VerificationEnt
    {
        public string UserId { get; set; }
        public string OTP { get; set; }
        public string EmailId { get; set; }
    }
    public class PasswordChangeEnt
    {
        public string UserId { get; set; }
        public string OldPassword { get; set; }
        public string NewPassword { get; set; }
    }
    public class UsersEnt
    {
        public int UserId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Gender { get; set; }
        public string DOB { get; set; }
        public string EmailId { get; set; }
        public int CountryCode { get; set; }
        public string MobileNumber { get; set; }
        public int CountryId { get; set; }
        public int CityId { get; set; }
        public int IsActive { get; set; }
        public string Password { get; set; }
        public string HearAboutUs { get; set; }
    }
    public class MessageStatus
    {
        public int Status { get; set; }
        public string Message { get; set; }
        public string TempId { get; set; }
        public int UserId { get; set; }
    }


    public class MyAccountEnt
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Gender { get; set; }
        public string EmailId { get; set; }
        public int CountryCode { get; set; }
        public string MobileNumber { get; set; }
        public int CountryId { get; set; }
        public int CityId { get; set; }
        public string DOB { get; set; }
        public string ProfileImage { get; set; }
        public int UserId { get; set; }
    }
}
