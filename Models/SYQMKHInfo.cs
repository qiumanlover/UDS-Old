using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Linq;
using System.Web;

namespace UDS.Models
{
    public class SYQMKHInfo
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "*")]
        [DataType(DataType.Time, ErrorMessage = "日期错误")]
        public DateTime EntryDate { get; set; }

        [Required(ErrorMessage = "*")]
        [DataType(DataType.Time, ErrorMessage = "日期错误")]
        public DateTime PassDate { get; set; }

        [Required(ErrorMessage = "*")]
        [StringLength(10000, ErrorMessage = "最多10000个字")]
        public string AttachContent { get; set; }

        public SYQMKHInfo() { }

        internal static int AddInfo(SYQMKHInfo syqmkhinfo)
        {
            object obj = SQLHelper.ExecuteScalar("insert into T_syqmkh(entrydate, passdate, valuation) output inserted.id values(@entrydate, @passdate, @valuation)", syqmkhinfo.EntryDate, syqmkhinfo.PassDate, syqmkhinfo.AttachContent);
            return Convert.ToInt32(obj);
        }

        internal static int UpdateInfo(SYQMKHInfo syqmkhinfo, int id)
        {
            object obj = SQLHelper.ExecuteNonQuery("update T_syqmkh set entrydate=@entrydate, passdate=@passdate, valuation=@valuation, where id=@id", syqmkhinfo.EntryDate, syqmkhinfo.PassDate, syqmkhinfo.AttachContent, id);
            return Convert.ToInt32(obj);
        }


        internal static SYQMKHInfo GetInfoById(int innerid)
        {
            SYQMKHInfo info = new SYQMKHInfo();
            DataTable dt = SQLHelper.ExecuteDataTable("select * from T_syqmkh where id = @id", innerid);
            info.Id = innerid;
            info.EntryDate = Convert.ToDateTime(dt.Rows[0]["entrydate"].ToString());
            info.PassDate = Convert.ToDateTime(dt.Rows[0]["passdate"].ToString());
            info.AttachContent = dt.Rows[0]["valuation"].ToString();
            return info;
        }
    }
}