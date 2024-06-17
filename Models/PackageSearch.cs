using Backend.Models;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;

namespace NextHolidaysIn.Models
{
    public class PackageSearch
    {
        private static IConfiguration _configuration;


        public PackageSearch(IConfiguration configuration)
        {
            _configuration = configuration;
        }

		public static async Task<PackageListFinal> GetPackageList(PackageSearchEnt data)
		{
			try
			{

				if (data.pageIndex == 0)
					data.pageIndex = 1;
				DBConnection db = new DBConnection();
				CancellationToken cts = new CancellationToken();
				SqlParameter[] para = new SqlParameter[] {
					new SqlParameter("@CountryId",data.CountryId),
					new SqlParameter("@CityId",data.CityId),
					new SqlParameter("@PackageId",data.PackageId),
					new SqlParameter("@PackageType",data.PackageType),
					new SqlParameter("@EntryType",data.EntryType)
				};

				Task<DataSet> dttourlist = null;
				int isreloadagain = 1;
				PackageListFinal s = new PackageListFinal();

				return await Task.Run(() =>
				{
					if (isreloadagain == 1)
					{
						List<string> SupplierHit = new List<string>();

						dttourlist = db.GetDataSetAsync("SPF_PackageList", para, cts, 10000);
						dttourlist.Wait();
						s.PackageList = HTMLExtensions.ToList<PackageListEnt>(dttourlist.Result.Tables[0]);
						s.PackageType = HTMLExtensions.ToList<PackageTypeEnt>(dttourlist.Result.Tables[1]);

					}
					decimal min = 0;
					decimal max = 0;
					foreach (PackageListEnt d in s.PackageList)
					{
						if (data.PackageId.Contains(d.PackageId.ToString()))
							d.SortBy = -1;

						if (min > d.FinalPrice)
							min = d.FinalPrice;
						if (max < d.FinalPrice)
							max = d.FinalPrice;
					}
					s.MinPrice = min;
					s.MaxPrice = max;
					//if (!String.IsNullOrEmpty(data.CityIds))
					//    s.PackageList = s.PackageList.Where(item => ("," + data.CityIds + ",").Contains("," + Convert.ToString(item.CityId) + ",")).ToList();
					if (!String.IsNullOrEmpty(data.PackageTypeIds))
						s.PackageList = s.PackageList.Where(item => ("," + data.PackageTypeIds + ",").Contains("," + Convert.ToString(item.PackageTypeId) + ",")).ToList();

					if (!String.IsNullOrEmpty(data.CancellationIds))
					{
						//s.TourList = s.TourList.Where(item => item.CancellationPolicyName).ToList();
						s.PackageList = s.PackageList.Where(item => ((item.CancellationPolicyName != "Non Refundable" && data.CancellationIds.Contains("Free Cancellation")) || (item.CancellationPolicyName == "Non Refundable" && data.CancellationIds.Contains("Non Refundable")))).ToList();
					}

					if ((data.MinPrice) > 0)
						s.PackageList = s.PackageList.Where(item => item.FinalPrice >= data.MinPrice).ToList();
					if ((data.MaxPrice) > 0)
						s.PackageList = s.PackageList.Where(item => item.FinalPrice <= data.MaxPrice + 1).ToList();

					if (!String.IsNullOrEmpty(data.SearchText))
						s.PackageList = s.PackageList.Where(item => item.PackageName.ToLower().Contains(data.SearchText.ToLower())).ToList();

					if (data.SortBy == "PriceASC")
						s.PackageList = s.PackageList.OrderBy(x => x.FinalPrice).ToList();
					if (data.SortBy == "PriceDESC")
						s.PackageList = s.PackageList.OrderBy(x => x.FinalPrice).Reverse().ToList();
					if (data.SortBy == "NameASC")
						s.PackageList = s.PackageList.OrderBy(x => x.PackageName).ToList();
					if (data.SortBy == "NameDESC")
						s.PackageList = s.PackageList.OrderBy(x => x.PackageName).Reverse().ToList();
					if (data.PackageId != "")
						s.PackageList = s.PackageList.OrderBy(x => x.SortBy).ToList();

					s.Count = s.PackageList.Count;

					if (data.FirstTimeLoading == 1 && data.PackageId == "0")
					{
						var numberGroups = s.PackageList.GroupBy(i => i.PackageTypeName).Select(group => new
						{
							Metric = group.Key,
							acount = group.Count()
						})
						.OrderBy(x => x.acount).Reverse().ToList();
						if (numberGroups.Count > 3)
						{

							s.PackageTypeList = new List<PackageTypeListPageEnt>();
							for (int z = 0; z < 2; z++)
							{
								PackageTypeListPageEnt lst = new PackageTypeListPageEnt();
								lst.PackageTypeName = numberGroups[z].Metric;
								lst.List = s.PackageList.Where(item => item.PackageTypeName == lst.PackageTypeName).ToList();
								s.PackageTypeList.Add(lst);
							}
						}
					}

					List<PackageListEnt> sdata = new List<PackageListEnt>();
					int pagesize = 100;
					int from = (pagesize * data.pageIndex) - pagesize;
					int to = (pagesize * data.pageIndex) - 1;
					for (int i = from; from <= to; from++)
					{
						if (s.PackageList.Count > from)
							sdata.Add(s.PackageList[from]);
					}
					s.PackageList = sdata;
					//           var userId = ((Task<Int64>)HttpContext.Current.Session["UserId"]);



					if (data.pageIndex > 1)
					{
						s.PackageType = null;
					}
					return s;
				}, cts);


			}
			catch (Exception ex)
			{
				return null;
			}
		}


		public static MessageStatus RemoveFromCart(RemoveCart data)
		{
			try
			{

				String tempuserid = data.TempUserId;
				int userid = data.UserId;

				SqlParameter[] para = new SqlParameter[] {
					new SqlParameter("@CartId",data.CartId),
					new SqlParameter("@UserId",userid),
					new SqlParameter("@TempUserId",tempuserid)
				};
				DBConnection db = new DBConnection();
				db.ExecuteNonQuery("SPF_RemoveCart", para);
				return new MessageStatus()
				{
					Status = 1,
					Message = "Service Removed."
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
		public static async Task<DataSet> GetPackageOptions(PackageOptionsEnt data)
		{
			try
			{
				DBConnection db = new DBConnection();

				CancellationToken cts = new CancellationToken();
				SqlParameter[] para = new SqlParameter[] {
							new SqlParameter("@PackageIds",data.PackageId),
							new SqlParameter("@Ages",HTMLExtensions.ConvertToDataTable<PackageAgeEnt>(data.Ages)),
						};
				DataSet ds = await Task.Run(() =>
				{
					return db.GetDataSetAsync("SPF_PackageOptions", para, cts, 10000);
				}, cts);
				return ds;


			}
			catch (Exception ex)
			{
				return null;
			}
		}


		public static PackageDetailsEnt GetPackageDescription(PackageDes data)
		{
			try
			{
				DBConnection db = new DBConnection();
				SqlParameter[] para = new SqlParameter[] {
				new SqlParameter("@PackageId",Convert.ToInt32(data.PackageId)),
				new SqlParameter("@Slug",data.Slug),
				  };
				PackageDetailsEnt d = new PackageDetailsEnt();
				DataSet dt = db.GetDataSet("SPF_PackageDetail", para);
				List<string> images = new List<string>();
				string bannerimage = "";
				for (int i = 0; i < dt.Tables[2].Rows.Count; i++)
				{
					if (Convert.ToInt32(dt.Tables[2].Rows[i]["IsFrontImage"]) == 0)
						images.Add(Convert.ToString(dt.Tables[2].Rows[i]["ImagePath"]));
					if (Convert.ToInt32(dt.Tables[2].Rows[i]["IsFrontImage"]) == 1)
						bannerimage = Convert.ToString(dt.Tables[2].Rows[i]["ImagePath"]);
				}
				d.PackageId = Convert.ToInt32(dt.Tables[0].Rows[0]["PackageId"]);
				d.PackageName = Convert.ToString(dt.Tables[0].Rows[0]["PackageName"]);
				d.Duration = Convert.ToInt32(dt.Tables[0].Rows[0]["Nights"]) + " Nights & " + (Convert.ToInt32(dt.Tables[0].Rows[0]["Nights"]) + 1) + " Days";
				d.PackageType = Convert.ToString(dt.Tables[0].Rows[0]["PackageTypeName"]);
				d.CitiesName = Convert.ToString(dt.Tables[0].Rows[0]["CitiesName"]);
				d.Cities = Convert.ToString(dt.Tables[0].Rows[0]["Cities"]);
				d.CancellationPolicyName = Convert.ToString(dt.Tables[0].Rows[0]["CancellationPolicyName"]);
				d.CancellationPolicyDescription = Convert.ToString(dt.Tables[0].Rows[0]["CancellationPolicyDescription"]);
				d.Images = images;
				string pattern = @"<[^/>][^>]*><\/[^>]+>";
				Regex regex = new Regex(pattern);

				foreach (DataRow item in dt.Tables[1].Rows)
				{
					item["Description"] = regex.Replace(Convert.ToString(item["Description"]), "");


				}
				foreach (DataRow item in dt.Tables[4].Rows)
				{
					item["Description"] = regex.Replace(Convert.ToString(item["Description"]), "");


				}
				d.Descriptions = HTMLExtensions.GetTableRows(dt.Tables[1]);
				d.CityData = HTMLExtensions.GetTableRows(dt.Tables[3]);
				d.Inclusion = HTMLExtensions.GetTableRows(dt.Tables[4]);

				d.MinAdult = Convert.ToInt32(dt.Tables[0].Rows[0]["MinAdult"]);
				d.Nights = Convert.ToInt32(dt.Tables[0].Rows[0]["Nights"]);
				d.MaxAdult = Convert.ToInt32(dt.Tables[0].Rows[0]["MaxAdult"]);
				d.MinChild = Convert.ToInt32(dt.Tables[0].Rows[0]["MinChild"]);
				d.MaxChild = Convert.ToInt32(dt.Tables[0].Rows[0]["MaxChild"]);
				d.MaxInfantAge = Convert.ToInt32(dt.Tables[0].Rows[0]["MaxInfantAge"]);
				d.MaxChildAge = Convert.ToInt32(dt.Tables[0].Rows[0]["MaxChildAge"]);
				d.SelectedDate = Convert.ToString(dt.Tables[0].Rows[0]["SelectedDate"]);
				d.IsFlightIncluded = Convert.ToInt32(dt.Tables[0].Rows[0]["IsFlightIncluded"]);
				d.IsHotelIncluded = Convert.ToInt32(dt.Tables[0].Rows[0]["IsHotelIncluded"]);
				d.IsTransferIncluded = Convert.ToInt32(dt.Tables[0].Rows[0]["IsTransferIncluded"]);
				d.IsDynamicPackage = Convert.ToInt32(dt.Tables[0].Rows[0]["IsDynamicPackage"]);
				return d;

			}
			catch (Exception ex)
			{
				return null;
			}
		}

		public static MessageStatus AddToCart(PackageCartEnt data)
		{
			try
			{
				DBConnection db = new DBConnection();

				foreach (PackageCartDetailsEnt tour in data.data)
				{
					SqlParameter[] para = new SqlParameter[] {
								new SqlParameter("@PackageIds",data.ServiceId),
								new SqlParameter("@PriceId",tour.OptionId),
								new SqlParameter("@TravelDate",tour.TravelDate),
							   new SqlParameter("@Ages",HTMLExtensions.ConvertToDataTable<PackageAgeEnt>(data.Ages))
					};
					DataSet ds = db.GetDataSet("SPF_PackageOptions", para);
					if (ds.Tables[0].Rows.Count > 0)
					{
						tour.NoOfAdult = Convert.ToInt32(ds.Tables[0].Rows[0]["NoOfAdult"]);
						tour.NoOfChild = Convert.ToInt32(ds.Tables[0].Rows[0]["NoOfChild"]);
						tour.NoOfInfant = Convert.ToInt32(ds.Tables[0].Rows[0]["NoOfInfant"]);

						tour.OptionName = Convert.ToString(ds.Tables[0].Rows[0]["OptionName"]);
						tour.OptionAdultPrice = Convert.ToDecimal(ds.Tables[0].Rows[0]["AdultPrice"]) / tour.NoOfAdult;
						if (tour.NoOfChild == 0)
							tour.OptionChildPrice = 0;
						else
							tour.OptionChildPrice = Convert.ToDecimal(ds.Tables[0].Rows[0]["ChildPrice"]) / tour.NoOfChild;
						if (tour.NoOfInfant == 0)
							tour.OptionInfantPrice = 0;
						else
							tour.OptionInfantPrice = Convert.ToDecimal(ds.Tables[0].Rows[0]["InfantPrice"]) / tour.NoOfInfant;
						tour.FinalOptionAdultPrice = Convert.ToDecimal(ds.Tables[0].Rows[0]["AdultPrice"]);
						tour.FinalOptionChildPrice = Convert.ToDecimal(ds.Tables[0].Rows[0]["ChildPrice"]);
						tour.FinalOptionInfantPrice = Convert.ToDecimal(ds.Tables[0].Rows[0]["InfantPrice"]);
						tour.FinalPrice = Convert.ToDecimal(ds.Tables[0].Rows[0]["Price"]);

					}

					else
					{
						return new MessageStatus()
						{
							Status = 0,
							Message = "Something went wrong."
						};
					}



				}
				//int UserId = Convert.ToInt32(Common.GetUserId());
				int UserId = data.UserId;
				string TempUserId = Common.GetTempUserId();
				if (UserId!=0)
				{
                    TempUserId = "";
                }
				SqlParameter[] paracart = new SqlParameter[] {
					new SqlParameter("@TempUserId",TempUserId),
					new SqlParameter("@UserId",UserId),
					new SqlParameter("@Type",data.Type),
					new SqlParameter("@ServiceName",data.ServiceName),
					new SqlParameter("@ServiceId",data.ServiceId),
					new SqlParameter("@data",HTMLExtensions.ConvertToDataTable<PackageCartDetailsEnt>(data.data))
				};
				db.ExecuteNonQuery("SPF_AddPackageToCart", paracart);
				return new MessageStatus()
				{
					Status = 1,
					Message = data.Type + " added to Cart.",
					TempId = TempUserId,
					UserId = UserId
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

		public static PackageOptionsResEnt GetPackageOptionsNew(PackageOptionsEnt data)
		{
			try
			{
				PackageOptionsResEnt d = new PackageOptionsResEnt();
				d.SelectedDate = data.TravelDate;
				SqlParameter[] para;
				DBConnection db = new DBConnection();

				if (data.IsFirstTime == 1)
				{
					para = new SqlParameter[] {
						new SqlParameter("@PackageId",data.PackageId)
					};
					DataRow dr = db.GetDataTable("SPF_GetPackageStaticDataForAPI", para).Rows[0];
					d.IsDynamicPackage = Convert.ToInt32(dr["IsDynamicPackage"]);
					if (d.SelectedDate == "")
						d.SelectedDate = Convert.ToString(dr["SelectedDate"]);
					para = new SqlParameter[] {
							new SqlParameter("@PackageId",data.PackageId),
					};
					DataTable date = db.GetDataTable("GetSlabData_Package", para);
					d.Dates = HTMLExtensions.GetTableRows(date);

				}
				para = new SqlParameter[] {
							new SqlParameter("@PackageIds",data.PackageId),
							new SqlParameter("@TravelDate",d.SelectedDate),
							new SqlParameter("@Ages",HTMLExtensions.ConvertToDataTable<PackageAgeEnt>(data.Ages)),
					};
				DataTable options = db.GetDataTable("SPF_PackageOptions", para);
				d.Options = HTMLExtensions.GetTableRows(options);
				return d;

			}
			catch (Exception ex)
			{
				return null;
			}
		}


		public static List<Dictionary<string, object>> GetPackageDates(PackageOptionsEnt data)
		{
			try
			{
				DBConnection db = new DBConnection();


				CancellationToken cts = new CancellationToken();
				SqlParameter[] para = new SqlParameter[] {
							new SqlParameter("@PackageId",data.PackageId),
						};
				//DataTable ds = await Task.Run(() =>
				//{
				//	return db.GetDataTableAsync("GetSlabData_Package", para, cts, 10000);
				//}, cts);
				//return ds;
				return HTMLExtensions.GetTableRows(db.GetDataTable("GetSlabData_Package", para));

			}
			catch (Exception ex)
			{
				return null;
			}
		}
	}


	public class PackageDetailsEnt
	{
		public int PackageId { get; set; }
		public string PackageName { get; set; }
		public string Duration { get; set; }
		public string PackageType { get; set; }
		public string CitiesName { get; set; }
		public string Cities { get; set; }
		public string CancellationPolicyName { get; set; }
		public string CancellationPolicyDescription { get; set; }
		public List<string> Images { get; set; }
		public List<Dictionary<string, object>> Descriptions { get; set; }
		public List<Dictionary<string, object>> CityData { get; set; }
		public List<Dictionary<string, object>> Inclusion { get; set; }
		public int MinAdult { get; set; }
		public int Nights { get; set; }
		public int MaxAdult { get; set; }
		public int MinChild { get; set; }
		public int MaxChild { get; set; }
		public int MaxInfantAge { get; set; }
		public int MaxChildAge { get; set; }
		public string SelectedDate { get; set; }
		public int IsFlightIncluded { get; set; }
		public int IsHotelIncluded { get; set; }
		public int IsTransferIncluded { get; set; }
		public int IsDynamicPackage { get; set; }
	}
	public class RemoveCart
	{
		public int CartId { get; set; }
		public int UserId { get; set; }
		public string TempUserId { get; set; }
	}

	public class PackageDes
	{
		public int PackageId { get; set; }
		public string Slug { get; set; }
	}
	public class PackageSearchEnt
    {
        public int CountryId { get; set; }
        public int CityId { get; set; }
        public string PackageId { get; set; }
        public int PackageType { get; set; }
        public string text { get; set; }
        public string date { get; set; }
        public int pageIndex { get; set; }
        public string CityIds { get; set; }
        public string PackageTypeIds { get; set; }
        public string CancellationIds { get; set; }
        public decimal MinPrice { get; set; }
        public decimal MaxPrice { get; set; }
        public string SortBy { get; set; }
        public string SearchText { get; set; }
        public Decimal ROE { get; set; }
        public string Currency { get; set; }
        public int FirstTimeLoading { get; set; }
        public int IsSoldOut { get; set; }
        public int EntryType { get; set; }
    }

	public class PackageOptionsResEnt
	{
		public List<Dictionary<string, object>> Dates { get; set; }
		public List<Dictionary<string, object>> Options { get; set; }
		public string SelectedDate { get; set; }
		public int IsDynamicPackage { get; set; }
	}
	public class PackageListFinal
    {
        public List<PackageListEnt> PackageList { get; set; }
        public List<PackageTypeEnt> PackageType { get; set; }
        public int Count { get; set; }
        public decimal MinPrice { get; set; }
        public decimal MaxPrice { get; set; }
        public List<PackageTypeListPageEnt> PackageTypeList { get; set; }
    }
    public class PackageListEnt
    {
        public int PackageId { get; set; }
        public string PackageName { get; set; }
        public string PackageTypeName { get; set; }
        public string ShortDescription { get; set; }
        public string Image { get; set; }
        public decimal FinalPrice { get; set; }
        public int PackageTypeId { get; set; }
        public string Slug { get; set; }
        public string CancellationPolicyName { get; set; }
        public int Nights { get; set; }
        public string Cities { get; set; }
        public string CitiesName { get; set; }
        public int MinAdult { get; set; }
        public int MaxAdult { get; set; }
        public int MinChild { get; set; }
        public int MaxChild { get; set; }
        public int MaxChildAge { get; set; }
        public int MaxInfantAge { get; set; }
        public string SelectedDate { get; set; }
        public int IsDynamicPackage { get; set; }
        public int IsSoldOut { get; set; }
        public int SortBy { get; set; }

    }
    public class PackageTypeEnt
    {
        public int PackageTypeId { get; set; }
        public string PackageTypeName { get; set; }
        public bool IsChecked { get; set; } = false;
    }

    public class PackageTypeListPageEnt
    {
        public string PackageTypeName { get; set; }
        public List<PackageListEnt> List { get; set; }
    }
    public class PackageOptionsEnt
    {
        public int PackageId { get; set; }
		public int IsFirstTime { get; set; }
		public int PriceId { get; set; }
        public string TravelDate { get; set; }
        public List<PackageAgeEnt> Ages { get; set; }
    }
    public class PackageAgeEnt
    {
        public int Age { get; set; }
        public string Type { get; set; }
    }

    public class PackageReviewEnt
    {
        public string FullName { get; set; }
        public string Email { get; set; }
        public decimal Rating { get; set; }
        public int PackageId { get; set; }
        public string Review { get; set; }
    }

    public class PackageCartEnt
    {
		public int UserId { get; set; }
        public string Type { get; set; }
        public string ServiceName { get; set; }
        public int ServiceId { get; set; }
        public List<PackageCartDetailsEnt> data { get; set; }
        public List<PackageAgeEnt> Ages { get; set; }

    }
    public class PackageCartDetailsEnt
    {
        public int OptionId { get; set; }
        public string OptionName { get; set; }
        public decimal OptionAdultPrice { get; set; }
        public decimal OptionChildPrice { get; set; }
        public decimal OptionInfantPrice { get; set; }
        public decimal FinalOptionAdultPrice { get; set; }
        public decimal FinalOptionChildPrice { get; set; }
        public decimal FinalOptionInfantPrice { get; set; }
        public decimal FinalPrice { get; set; }
        public int NoOfAdult { get; set; }
        public int NoOfChild { get; set; }
        public string TravelDate { get; set; }
        public int NoOfInfant { get; set; }
        public int Nights { get; set; }
    }
}
    