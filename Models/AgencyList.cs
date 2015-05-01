using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Linq;
using System.Web;

namespace UDS.Models
{
    public class AgencyList
    {
        public int Id { get; set; }
        public int GrantorId { get; set; }
        public string GrantorName { get; set; }
        public int AgentId { get; set; }
        public string AgentName { get; set; }
        [Required(ErrorMessage = "*")]
        public string BeginTime { get; set; }
        [Required(ErrorMessage = "*")]
        public string EndTime { get; set; }
        public AgencyList() { }

        public AgencyList DBDataToAgency(DataRow row, DataColumnCollection columns)
        {
            foreach (DataColumn colname in columns)
            {
                switch (colname.ColumnName)
                {
                    case "id":
                        this.Id = Convert.ToInt32(row["id"] ?? 0);
                        break;
                    case "grantorid":
                        this.GrantorId = Convert.ToInt32(row["grantorid"] ?? 0);
                        break;
                    case "grantorname":
                        this.GrantorName = row["grantorname"].ToString();
                        break;
                    case "agentid":
                        this.AgentId = Convert.ToInt32(row["agentid"] ?? 0);
                        break;
                    case "agentname":
                        this.AgentName = row["agentname"].ToString();
                        break;
                    case "begintime":
                        this.BeginTime = Convert.ToDateTime(row["begintime"] ?? "").ToString("yyyy-MM-dd HH:mm");
                        break;
                    case "endtime":
                        this.EndTime = Convert.ToDateTime(row["endtime"] ?? "").ToString("yyyy-MM-dd HH:mm");
                        break;
                    default:
                        break;
                }
            }
            return this;
        }

        public static int AddInfo(AgencyList agency)
        {
            string sql = "insert into T_agency(grantorid, agentid, begintime, endtime)values(@grantorid, @agentid, @begintime, @endtime)";
            return SQLHelper.ExecuteNonQuery(sql, agency.GrantorId, agency.AgentId, agency.BeginTime, agency.EndTime);
        }
    }
}