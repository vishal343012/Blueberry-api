using Microsoft.AspNetCore.Http;
using System;
using System.Web;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using NextHolidaysIn.Models;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace NextHolidaysIn.Models
{
    public class Currency
    {
        private static IHttpContextAccessor _httpContextAccessor;
        public Currency( IHttpContextAccessor httpContextAccessor)
        {
            
            _httpContextAccessor = httpContextAccessor;
        }


        public static void ChangeCurrency(string ShortName)
        {
            try
            {
                SqlParameter[] para = new SqlParameter[] {
                new SqlParameter("@CurrencyCode",ShortName)
            };
                DBConnection db = new DBConnection();
                Decimal amt = Convert.ToDecimal(db.GetExecuteScalarSP("GetCurrencyROE", para));
                if (amt > 0)
                {
                    _httpContextAccessor.HttpContext.Session.SetString("Currency", ShortName);
                    _httpContextAccessor.HttpContext.Session.Set<decimal>("ROE", amt);
                }

            }
            catch (Exception)
            {
                _httpContextAccessor.HttpContext.Session.SetString("Currency", "AED");
                _httpContextAccessor.HttpContext.Session.Set<decimal>("ROE", 1);
            }
        }

        public static Decimal CalculateAmount(decimal amt, Decimal s = 0)
        {
            try
            {
                decimal ROE = _httpContextAccessor.HttpContext.Session.Get<decimal>("ROE");
                if (s != 0)
                {
                    return amt * s;
                }
                return amt * ROE;
            }
            catch (Exception ex)
            {
                return amt;
            }
        }
        public static String CurrencyName()
        {
            try
            {
                return _httpContextAccessor.HttpContext.Session.GetString("Currency") ?? "AED";
            }
            catch (Exception ex)
            {
                _httpContextAccessor.HttpContext.Session.SetString("Currency", "AED");
                _httpContextAccessor.HttpContext.Session.Set<decimal>("ROE", 1);
                return "AED";
            }
        }
    }
}
