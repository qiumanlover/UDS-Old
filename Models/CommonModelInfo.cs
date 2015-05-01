using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Linq;
using System.Web;

namespace UDS.Models
{
    public class CommonModelInfo
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "*")]
        public string AttachContent { get; set; }

        public CommonModelInfo() { }

        internal static int AddInfo(CommonModelInfo commoninfo)
        {
            object obj = SQLHelper.ExecuteScalar("insert into T_bgwj(details) output inserted.id values(@details)", commoninfo.AttachContent);
            return Convert.ToInt32(obj);
        }

        internal static int UpdateInfo(CommonModelInfo commoninfo, int id)
        {
            object obj = SQLHelper.ExecuteNonQuery("update T_bgwj set details=@details where id=@id", commoninfo.AttachContent, id);
            return Convert.ToInt32(obj);
        }


        internal static CommonModelInfo GetInfoById(int innerid)
        {
            CommonModelInfo info = new CommonModelInfo();
            DataTable dt = SQLHelper.ExecuteDataTable("select * from T_bgwj where id = @id", innerid);
            info.Id = innerid;
            info.AttachContent = dt.Rows[0]["details"].ToString();
            return info;
        }
    }
}