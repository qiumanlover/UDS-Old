using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace UDS.Models
{
    public class PXInfo
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "*")]
        [DataType(DataType.Time, ErrorMessage = "*")]
        public DateTime BeginDate { get; set; }

        [Required(ErrorMessage = "*")]
        [DataType(DataType.Time, ErrorMessage = "*")]
        public DateTime EndDate { get; set; }

        [Required(ErrorMessage = "*")]
        public int TypeId { get; set; }

        [Required(ErrorMessage = "*")]
        public string Title { get; set; }

        [Required(ErrorMessage = "*")]
        public string AttachContent { get; set; }

        public PXInfo() { }

        internal static int AddInfo(PXInfo pxinfo)
        {
            object obj = SQLHelper.ExecuteScalar("insert into T_px(begindate, enddate, typeid, title, attachcontent) output inserted.id values(@begindate, @enddate, @typeid, @title, @attachcontent)", pxinfo.BeginDate, pxinfo.EndDate, pxinfo.TypeId, pxinfo.Title, pxinfo.AttachContent);
            return Convert.ToInt32(obj);
        }

        internal static int UpdateInfo(PXInfo pxinfo, int id)
        {
            object obj = SQLHelper.ExecuteNonQuery("update T_px set begindate=@begindate, enddate=@enddate, typeid=@typeid, title=@title, attachcontent=@attachcontent where id=@id", pxinfo.BeginDate, pxinfo.EndDate, pxinfo.TypeId, pxinfo.Title, pxinfo.AttachContent, id);
            return Convert.ToInt32(obj);
        }

        internal static List<SelectListItem> GetTypeList()
        {
            DataTable typedata = SQLHelper.ExecuteDataTable("select id, subname from T_fieldtype where typename = @typename", "培训类型");
            List<SelectListItem> typeList = new List<SelectListItem>();
            foreach (DataRow row in typedata.Rows)
            {
                typeList.Add(new SelectListItem { Value = row["id"].ToString(), Text = row["subname"].ToString() });
            }
            return typeList;
        }

        internal static PXInfo GetInfoById(int innerid)
        {
            PXInfo info = new PXInfo();
            DataTable dt = SQLHelper.ExecuteDataTable("select * from T_px where id = @id", innerid);
            info.Id = innerid;
            info.BeginDate = Convert.ToDateTime(dt.Rows[0]["begindate"].ToString());
            info.EndDate = Convert.ToDateTime(dt.Rows[0]["enddate"].ToString());
            info.TypeId = Convert.ToInt32(dt.Rows[0]["typeid"]);
            info.Title = dt.Rows[0]["title"].ToString();
            info.AttachContent = dt.Rows[0]["attachcontent"].ToString();
            return info;
        }
    }
}