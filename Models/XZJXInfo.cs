using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Linq;
using System.Web;

namespace UDS.Models
{
    public class XZJXInfo
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "*")]
        [DataType(DataType.Time, ErrorMessage = "*")]
        public DateTime ApplyMonth { get; set; }

        [Required(ErrorMessage = "*")]
        public string AttachContent { get; set; }

        public XZJXInfo() { }

        internal static int AddInfo(XZJXInfo xzjxinfo)
        {
            object obj = SQLHelper.ExecuteScalar("insert into T_xzjx(applymonth, attachcontent) output inserted.id values(@applymonth, @attachcontent)", xzjxinfo.ApplyMonth, xzjxinfo.AttachContent);
            return Convert.ToInt32(obj);
        }

        internal static int UpdateInfo(XZJXInfo xzjxinfo, int id)
        {
            object obj = SQLHelper.ExecuteNonQuery("update T_xzjx set applymonth=@applymonth, attachcontent=@attachcontent where id=@id", xzjxinfo.ApplyMonth, xzjxinfo.AttachContent, id);
            return Convert.ToInt32(obj);
        }


        internal static XZJXInfo GetInfoById(int innerid)
        {
            XZJXInfo info = new XZJXInfo();
            DataTable dt = SQLHelper.ExecuteDataTable("select * from T_xzjx where id = @id", innerid);
            info.Id = innerid;
            info.ApplyMonth = Convert.ToDateTime(dt.Rows[0]["applymonth"].ToString());
            info.AttachContent = dt.Rows[0]["attachcontent"].ToString();
            return info;
        }
    }
}