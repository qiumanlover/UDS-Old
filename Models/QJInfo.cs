using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace UDS.Models
{
    public class QJInfo
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "*")]
        [DataType(DataType.Time, ErrorMessage = "时间格式错误")]
        public DateTime BeginTime { get; set; }

        [Required(ErrorMessage = "*")]
        [DataType(DataType.Time, ErrorMessage = "时间格式错误")]
        public DateTime EndTime { get; set; }

        [Range(0.0, double.MaxValue, ErrorMessage = "开始时间必须早于结束时间")]
        public double TotalTime { get; set; }

        [Required(ErrorMessage = "*")]
        [StringLength(50, ErrorMessage = "最多可输入50个字")]
        public string Reason { get; set; }

        public int TypeId { get; set; }

        public QJInfo() { }

        internal static int AddInfo(QJInfo qjinfo)
        {
            object obj = SQLHelper.ExecuteScalar("insert into T_qj(begintime, endtime, totaltime, reason, typeid) output inserted.id values(@begintime, @endtime, @totaltime, @reason, @typeid)", qjinfo.BeginTime, qjinfo.EndTime, qjinfo.TotalTime, qjinfo.Reason, qjinfo.TypeId);
            return Convert.ToInt32(obj);
        }

        internal static int UpdateInfo(QJInfo qjinfo, int id)
        {
            object obj = SQLHelper.ExecuteNonQuery("update T_qj set begintime=@begintime, endtime=@endtime, totaltime=@totaltime, reason=@reason, typeid=@typeid where id=@id", qjinfo.BeginTime, qjinfo.EndTime, qjinfo.TotalTime, qjinfo.Reason, qjinfo.TypeId, id);
            return Convert.ToInt32(obj);
        }

        internal static List<SelectListItem> GetTypeList()
        {
            DataTable typedata = SQLHelper.ExecuteDataTable("select id, subname from T_fieldtype where typename = @typename", "请假类别");
            List<SelectListItem> typeList = new List<SelectListItem>();
            foreach (DataRow row in typedata.Rows)
            {
                typeList.Add(new SelectListItem { Value = row["id"].ToString(), Text = row["subname"].ToString() });
            }
            return typeList;
        }

        internal static QJInfo GetInfoById(int innerid)
        {
            QJInfo info = new QJInfo();
            DataTable dt = SQLHelper.ExecuteDataTable("select * from T_qj where id = @id", innerid);
            info.Id = innerid;
            info.BeginTime = Convert.ToDateTime(dt.Rows[0]["begintime"].ToString());
            info.EndTime = Convert.ToDateTime(dt.Rows[0]["endtime"].ToString());
            info.TotalTime = Convert.ToDouble(dt.Rows[0]["totaltime"]);
            info.Reason = dt.Rows[0]["reason"].ToString();
            info.TypeId = Convert.ToInt32(dt.Rows[0]["typeid"]);
            return info;
        }
    }
}