using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Linq;
using System.Web;

namespace UDS.Models
{
    public class ZZInfo
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "*")]
        [RegularExpression(@"^[0-9]+\.{0,1}(\d{1,2})?$", ErrorMessage = "正数且至多2位小数")]
        [Range(0.0, 1e15, ErrorMessage = "超出范围")]
        [Display(Name = "暂支金额")]
        public decimal Money { get; set; }

        [Required(ErrorMessage = "*")]
        [StringLength(100, ErrorMessage = "最多100字")]
        public string Reason { get; set; }

        [Required(ErrorMessage = "*")]
        public DateTime PreDate { get; set; }

        public ZZInfo() { }

        internal static int AddInfo(ZZInfo zzinfo)
        {
            object obj = SQLHelper.ExecuteScalar("insert into T_zz(money, reason, predate) output inserted.id values(@money, @reason, @predate)", zzinfo.Money, zzinfo.Reason, zzinfo.PreDate);
            return Convert.ToInt32(obj);
        }

        internal static int UpdateInfo(ZZInfo zzinfo, int id)
        {
            object obj = SQLHelper.ExecuteNonQuery("update T_zz set money=@money, reason=@reason, predate=@predate, singleprice=@singleprice, totalprice=@totalprice, needdate=@needdate where id=@id", zzinfo.Money, zzinfo.Reason, zzinfo.PreDate, id);
            return Convert.ToInt32(obj);
        }

        internal static ZZInfo GetInfoById(int innerid)
        {
            ZZInfo info = new ZZInfo();
            DataTable dt = SQLHelper.ExecuteDataTable("select * from T_zz where id = @id", innerid);
            info.Id = innerid;
            info.Money = Convert.ToDecimal(dt.Rows[0]["money"].ToString());
            info.Reason = dt.Rows[0]["reason"].ToString();
            info.PreDate = Convert.ToDateTime(dt.Rows[0]["predate"]);
            return info;
        }
    }
}