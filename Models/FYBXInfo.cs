using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Linq;
using System.Web;

namespace UDS.Models
{
    public class FYBXInfo
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "*")]
        public string Usage { get; set; }

        [DataType(DataType.DateTime)]
        public DateTime HappenDate { get; set; }

        [Required(ErrorMessage = "*")]
        public decimal Money { get; set; }

        [Required(ErrorMessage = "*")]
        public string BMoney { get; set; }

        public decimal PrePay { get; set; }

        public decimal ReturnPay { get; set; }

        public string AttachContent { get; set; }

        public FYBXInfo() { }

        internal static int AddInfo(FYBXInfo fybxinfo)
        {
            object obj = SQLHelper.ExecuteScalar("insert into T_fybx(usage, happendate, money, bmoney, prepay, returnpay, attachpath) output inserted.id values(@usage, @happendate, @money, @bmoney, @prepay, @returnpay, @attachpath)", fybxinfo.Usage, fybxinfo.HappenDate, fybxinfo.Money, fybxinfo.BMoney, fybxinfo.PrePay, fybxinfo.ReturnPay, fybxinfo.AttachContent);
            return Convert.ToInt32(obj);
        }

        internal static int UpdateInfo(FYBXInfo fybxinfo, int id)
        {
            object obj = SQLHelper.ExecuteNonQuery("update T_fybx set usage=@usage, happendate=@happendate, money=@money, bmoney=@bmoney, prepay=@prepay, returnpay=@returnpay, attachpath=@attachpath where id=@id", fybxinfo.Usage, fybxinfo.HappenDate, fybxinfo.Money, fybxinfo.BMoney, fybxinfo.PrePay, fybxinfo.ReturnPay, fybxinfo.AttachContent, id);
            return Convert.ToInt32(obj);
        }

        internal static FYBXInfo GetInfoById(int innerid)
        {
            FYBXInfo info = new FYBXInfo();
            DataTable dt = SQLHelper.ExecuteDataTable("select * from T_fybx where id = @id", innerid);
            info.Id = innerid;
            info.Usage = dt.Rows[0]["usage"].ToString();
            info.HappenDate = Convert.ToDateTime(dt.Rows[0]["happendate"]);
            info.Money = Convert.ToDecimal(dt.Rows[0]["money"]);
            info.BMoney = dt.Rows[0]["bmoney"].ToString();
            info.PrePay = Convert.ToDecimal(dt.Rows[0]["prepay"]);
            info.ReturnPay = Convert.ToDecimal(dt.Rows[0]["returnpay"]);
            info.AttachContent = dt.Rows[0]["attachpath"].ToString();

            return info;
        }
    }
}