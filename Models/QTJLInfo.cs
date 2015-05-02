using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace UDS.Models
{
    public class QTJLInfo
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "*")]
        public int VisitorId { get; set; }

        [Required(ErrorMessage = "*")]
        public string BeVisitor { get; set; }

        [Required(ErrorMessage = "*")]
        public string Property { get; set; }

        [Required(ErrorMessage = "*")]
        [DataType(DataType.Time, ErrorMessage = "*")]
        public DateTime BeginTime { get; set; }

        [Required(ErrorMessage = "*")]
        [DataType(DataType.Time, ErrorMessage = "*")]
        public DateTime EndTime { get; set; }

        [Required(ErrorMessage = "*")]
        public string AttachContent { get; set; }

        public QTJLInfo() { }

        internal static int AddInfo(QTJLInfo qtjlinfo)
        {
            object obj = SQLHelper.ExecuteScalar("insert into T_qtjl(visitorid, bevisitor, property, begintime, endtime, attachcontent) output inserted.id values(@visitorid, @bevisitor, @property, @begintime, @endtime, @attachcontent)", qtjlinfo.VisitorId, qtjlinfo.BeVisitor, qtjlinfo.Property, qtjlinfo.BeginTime, qtjlinfo.EndTime, qtjlinfo.AttachContent);
            return Convert.ToInt32(obj);
        }

        internal static int UpdateInfo(QTJLInfo qtjlinfo, int id)
        {
            object obj = SQLHelper.ExecuteNonQuery("update T_qtjl set visitorid=@visitorid, bevisitor=@bevisitor, property=@bevisitor, begintime=@begintime, endtime=@endtime, attachcontent=@attachcontent where id=@id", qtjlinfo.VisitorId, qtjlinfo.BeVisitor, qtjlinfo.Property, qtjlinfo.BeginTime, qtjlinfo.EndTime, qtjlinfo.AttachContent, id);
            return Convert.ToInt32(obj);
        }

        internal static List<SelectListItem> GetTypeList()
        {
            DataTable typedata = SQLHelper.ProcDataTable("usp_EmployeeSelectorOn");
            //DataTable typedata = SQLHelper.ExecuteDataTable("select id, name from T_employee where isonjob=1", null);
            List<SelectListItem> typeList = new List<SelectListItem>();
            foreach (DataRow row in typedata.Rows)
            {
                typeList.Add(new SelectListItem { Value = row["id"].ToString(), Text = row["name"].ToString() });
            }
            return typeList;
        }

        internal static QTJLInfo GetInfoById(int innerid)
        {
            QTJLInfo info = new QTJLInfo();
            DataTable dt = SQLHelper.ExecuteDataTable("select * from T_qtjl where id = @id", innerid);
            info.Id = innerid;
            info.VisitorId = Convert.ToInt32(dt.Rows[0]["visitorid"]);
            info.BeVisitor = dt.Rows[0]["bevisitor"].ToString();
            info.Property = dt.Rows[0]["property"].ToString();
            info.BeginTime = Convert.ToDateTime(dt.Rows[0]["begintime"].ToString());
            info.EndTime = Convert.ToDateTime(dt.Rows[0]["endtime"].ToString());
            info.AttachContent = dt.Rows[0]["attachcontent"].ToString();
            return info;
        }
    }
}