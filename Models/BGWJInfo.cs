using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Linq;
using System.Web;

namespace UDS.Models
{
    public class BGWJInfo
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "*")]
        public string Details { get; set; }

        public BGWJInfo() { }

        internal static int AddInfo(BGWJInfo bgwjinfo)
        {
            object obj = SQLHelper.ExecuteScalar("insert into T_bgwj(details) output inserted.id values(@details)", bgwjinfo.Details);
            return Convert.ToInt32(obj);
        }

        internal static int UpdateInfo(BGWJInfo bgwjinfo, int id)
        {
            object obj = SQLHelper.ExecuteNonQuery("update T_bgwj set details=@details, where id=@id", bgwjinfo.Details, id);
            return Convert.ToInt32(obj);
        }


        internal static BGWJInfo GetInfoById(int innerid)
        {
            BGWJInfo info = new BGWJInfo();
            DataTable dt = SQLHelper.ExecuteDataTable("select * from T_bgwj where id = @id", innerid);
            info.Id = innerid;
            info.Details = dt.Rows[0]["details"].ToString();
            return info;
        }
    }
}