using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace UDS
{
    public class User
    {
        public int Id { get; set; }

        public int Eid { get; set; }

        public string Loginname { get; set; }

        public string Password { get; set; }
        public string Ename { get; set; }
        public int Userlevel { get; set; }

        private bool isDelete { get; set; }
        public User() { }

        public User(int id, int eid, string loginname, string password, string ename, int userlevel, bool isdelete)
        {
            this.Id = id;
            this.Eid = eid;
            this.Loginname = loginname;
            this.Password = password;
            this.Ename = ename;
            this.Userlevel = userlevel;
            this.isDelete = isdelete;
        }

        public int CheckLogin(string username, string inputpass)
        {
            if (GetDBUser(username) == null)
            {
                return -1;
            }
            else if (this.isDelete)
            {
                return -2;
            }
            else if (!this.Password.Equals(inputpass))
            {
                return -3;
            }
            return 1;
        }

        private User GetDBUser(string inputname)
        {
            SqlParameter[] param = { new SqlParameter("@username", SqlDbType.NVarChar) };
            param[0].Value = inputname;
            DataTable dt = SQLHelper.ProcDataTable("usp_Login", param);
            if (dt == null || dt.Rows.Count <= 0) return null;
            return DataTableToLogin(dt);
        }

        private User DataTableToLogin(DataTable dt)
        {
            this.Id = Convert.ToInt32(dt.Rows[0]["uid"]);
            this.Loginname = dt.Rows[0]["username"].ToString();
            this.Password = dt.Rows[0]["password"].ToString();
            this.Userlevel = Convert.ToInt32(dt.Rows[0]["userlevel"]);
            this.Eid = Convert.ToInt32(dt.Rows[0]["eid"]);
            this.Ename = dt.Rows[0]["ename"].ToString();
            this.isDelete = Convert.ToBoolean(dt.Rows[0]["isdelete"]);
            return this;
        }

        internal int UpdatePass(string pass)
        {
            return SQLHelper.ExecuteNonQuery("update T_user set password=@pass where id=@id", pass, this.Id);
        }
    }
}