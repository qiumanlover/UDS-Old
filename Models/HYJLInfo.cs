using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Linq;
using System.Web;

namespace UDS.Models
{
    public class HYJLInfo
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "*")]
        [DataType(DataType.Time, ErrorMessage = "日期错误")]
        public DateTime MeetingDate { get; set; }

        [Required(ErrorMessage = "*")]
        [StringLength(50, ErrorMessage = "最多50个字")]
        public string Topic { get; set; }

        [Required(ErrorMessage = "*")]
        public string AttachContent { get; set; }

        public HYJLInfo() { }

        internal static int AddInfo(HYJLInfo hyjlinfo)
        {
            object obj = SQLHelper.ExecuteScalar("insert into T_hyjl(meetingdate, topic, attachcontent) output inserted.id values(@meetingdate, @topic, @attachcontent)", hyjlinfo.MeetingDate, hyjlinfo.Topic, hyjlinfo.AttachContent);
            return Convert.ToInt32(obj);
        }

        internal static int UpdateInfo(HYJLInfo hyjlinfo, int id)
        {
            object obj = SQLHelper.ExecuteNonQuery("update T_hyjl set meetingdate=@meetingdate, topic=@topic, attachcontent=@attachcontent where id=@id", hyjlinfo.MeetingDate, hyjlinfo.Topic, hyjlinfo.AttachContent, id);
            return Convert.ToInt32(obj);
        }


        internal static HYJLInfo GetInfoById(int innerid)
        {
            HYJLInfo info = new HYJLInfo();
            DataTable dt = SQLHelper.ExecuteDataTable("select * from T_hyjl where id = @id", innerid);
            info.Id = innerid;
            info.MeetingDate = Convert.ToDateTime(dt.Rows[0]["meetingdate"].ToString());
            info.Topic = dt.Rows[0]["topic"].ToString();
            info.AttachContent = dt.Rows[0]["attachcontent"].ToString();
            return info;
        }

    }
}