using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace UDS.Models
{
    public class InfoList
    {
        public int Id { get; set; }
        public string FormName { get; set; }
        public string EmployeeName { get; set; }
        public string WriteTime { get; set; }
        public string NextSignName { get; set; }
        public string CurrentState { get; set; }

        public InfoList DBDataToInfo(DataRow row, DataColumnCollection columns)
        {
            foreach (DataColumn colname in columns)
            {
                switch (colname.ColumnName)
                {
                    case "id":
                        this.Id = Convert.ToInt32(row["id"] ?? 0);
                        break;
                    case "formname":
                        this.FormName = (row["formname"] ?? "").ToString();
                        break;
                    case "employeename":
                        this.EmployeeName = (row["employeename"] ?? "").ToString();
                        break;
                    case "writetime":
                        this.WriteTime = Convert.ToDateTime((row["writetime"] ?? "")).ToString("yyyy-MM-dd HH:mm:ss");
                        break;
                    case "nextname":
                        this.NextSignName = (row["nextname"] ?? "").ToString();
                        break;
                    case "state":
                        this.CurrentState = (row["state"] ?? "").ToString();
                        break;
                    default:
                        break;
                }
            }
            return this;
        }
    }
}