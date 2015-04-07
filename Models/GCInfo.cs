using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace UDS.Models
{
    public class GCInfo
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "*")]
        [DataType(DataType.Time, ErrorMessage = "时间格式错误")]
        public DateTime BeginTime { get; set; }

        [Required(ErrorMessage = "*")]
        [DataType(DataType.Time, ErrorMessage = "时间格式错误")]
        public DateTime EndTime { get; set; }

        [Required(ErrorMessage = "*")]
        [StringLength(100, ErrorMessage = "最多可输入100个字")]
        public string Reason { get; set; }

        public GCInfo() { }

        internal static int AddInfo(GCInfo gcinfo)
        {
            object obj = SQLHelper.ExecuteScalar("insert into T_gc(begintime, endtime, reason) output inserted.id values(@begintime, @endtime, @reason)", gcinfo.BeginTime, gcinfo.EndTime, gcinfo.Reason);
            return Convert.ToInt32(obj);
        }

        internal static int UpdateInfo(GCInfo gcinfo, int id)
        {
            object obj = SQLHelper.ExecuteNonQuery("update T_gc set begintime=@begintime, endtime=@endtime, reason=@reason, where id=@id", gcinfo.BeginTime, gcinfo.EndTime, gcinfo.Reason, id);
            return Convert.ToInt32(obj);
        }

        internal static GCInfo GetInfoById(int innerid)
        {
            GCInfo info = new GCInfo();
            DataTable dt = SQLHelper.ExecuteDataTable("select * from T_gc where id = @id", innerid);
            info.Id = innerid;
            info.BeginTime = Convert.ToDateTime(dt.Rows[0]["begintime"].ToString());
            info.EndTime = Convert.ToDateTime(dt.Rows[0]["endtime"].ToString());
            info.Reason = dt.Rows[0]["reason"].ToString();
            return info;
        }
    }
}