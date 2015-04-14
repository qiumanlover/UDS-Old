using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Linq;
using System.Web;

namespace UDS.Models
{
    public class GDZCInfo
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "*")]
        [StringLength(50, ErrorMessage = "最多50字")]
        public string DeviceName { get; set; }

        [Required(ErrorMessage = "*")]
        [Range(1, int.MaxValue, ErrorMessage = "范围在1-2147483647")]
        public int Count { get; set; }

        [Required(ErrorMessage = "*")]
        [StringLength(200, ErrorMessage = "最多200字")]
        public string Description { get; set; }

        [Required(ErrorMessage = "*")]
        [RegularExpression(@"^[0-9]+\.{0,1}(\d{1,2})?$", ErrorMessage = "正数且至多2位小数")]
        [Display(Name = "单价")]
        public decimal SinglePrice { get; set; }

        [Required(ErrorMessage = "*")]
        [Range(0.0, 1e15, ErrorMessage = "超出范围")]
        public decimal TotalPrice { get; set; }

        [Required(ErrorMessage = "*")]
        [DataType(DataType.Time, ErrorMessage = "*")]
        public DateTime NeedDate { get; set; }

        public GDZCInfo() {  }

        internal static int AddInfo(GDZCInfo gdzcinfo)
        {
            object obj = SQLHelper.ExecuteScalar("insert into T_gdzc(devicename, count, description, singleprice, totalprice, needdate) output inserted.id values(@devicename, @count, @description, @singleprice, @totalprice, @needdate)", gdzcinfo.DeviceName, gdzcinfo.Count, gdzcinfo.Description, gdzcinfo.SinglePrice, gdzcinfo.TotalPrice, gdzcinfo.NeedDate);
            return Convert.ToInt32(obj);
        }

        internal static int UpdateInfo(GDZCInfo gdzcinfo, int id)
        {
            object obj = SQLHelper.ExecuteNonQuery("update T_gdzc set devicename=@devicename, count=@count, description=@description, singleprice=@singleprice, totalprice=@totalprice, needdate=@needdate where id=@id", gdzcinfo.DeviceName, gdzcinfo.Count, gdzcinfo.Description, gdzcinfo.SinglePrice, gdzcinfo.TotalPrice, gdzcinfo.NeedDate, id);
            return Convert.ToInt32(obj);
        }

        internal static GDZCInfo GetInfoById(int innerid)
        {
            GDZCInfo info = new GDZCInfo();
            DataTable dt = SQLHelper.ExecuteDataTable("select * from T_gdzc where id = @id", innerid);
            info.Id = innerid;
            info.DeviceName = dt.Rows[0]["devicename"].ToString();
            info.Count = Convert.ToInt32(dt.Rows[0]["count"].ToString());
            info.Description = dt.Rows[0]["description"].ToString();
            info.SinglePrice = Convert.ToDecimal(dt.Rows[0]["singleprice"].ToString());
            info.TotalPrice = Convert.ToDecimal(dt.Rows[0]["totalprice"].ToString());
            info.NeedDate = Convert.ToDateTime(dt.Rows[0]["needdate"]);
            return info;
        }
    }
}