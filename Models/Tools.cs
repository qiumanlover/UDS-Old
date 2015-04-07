using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace UDS.Models
{
    public class Tools
    {
        internal static int AddForm(int innerid, int formid, int eid, DateTime datetime, string signlist)
        {
            SqlParameter[] parameters = { new SqlParameter("@innerid", SqlDbType.Int), new SqlParameter("@formid", SqlDbType.Int), new SqlParameter("@eid", SqlDbType.Int), new SqlParameter("@writetime", SqlDbType.DateTime), new SqlParameter("@signlist", SqlDbType.VarChar), new SqlParameter("@id", SqlDbType.Int) };
            parameters[0].Value = innerid;
            parameters[1].Value = formid;
            parameters[2].Value = eid;
            parameters[3].Value = datetime;
            parameters[4].Value = signlist;
            parameters[5].Direction = ParameterDirection.Output;
            SQLHelper.ProcNoQuery("usp_WriteSave", parameters);
            return Convert.ToInt32(parameters[5].Value);
        }

        internal static List<string> CalcSignList(int flowid, int eid)
        {
            List<string> signlist = new List<string>();
            DataTable dt = SQLHelper.ProcDataTable("usp_WriteDetail", new SqlParameter("@id", flowid));
            string flow = dt.Rows[0]["flow"].ToString();
            string[] flowlist = flow.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries);
            int temp;
            dt = SQLHelper.ProcDataTable("usp_PosId", new SqlParameter("@id", eid));
            string posid = dt.Rows[0]["positionid"].ToString();
            for (int i = 0; i < flowlist.Length - 1; i++)
            {
                if (int.TryParse(flowlist[i], out temp))
                {
                    if (!signlist.Contains(flowlist[i]))
                    {
                        signlist.Add(flowlist[i]);
                    }
                    continue;
                }
                if (flowlist[i].Equals("U"))
                {
                    dt = SQLHelper.ProcDataTable("usp_SuperiorPosId", new SqlParameter("@id", eid));
                    signlist.Add(dt.Rows[0]["superiorposid"].ToString());
                    continue;
                }

                if (flowlist[i].Equals("T"))
                {
                    signlist.Add(posid);
                    continue;
                }
                if (flowlist[i].Equals("D"))
                {
                    dt = SQLHelper.ProcDataTable("usp_DepPosId", new SqlParameter("@id", eid));
                    string depposid = dt.Rows[0]["directorposid"].ToString();
                    if (posid.Equals("1") || posid.Equals("2") || posid.Equals(depposid))
                    {
                        continue;
                    }
                    else
                    {
                        List<string> tempSuper = new List<string>();
                        int supereid = eid;
                        string superposid = string.Empty;
                        do
                        {
                            dt = SQLHelper.ProcDataTable("usp_SuperiorPosId", new SqlParameter("@id", supereid));
                            superposid = dt.Rows[0]["superiorposid"].ToString();
                            if (!superposid.Equals(depposid))
                            {
                                tempSuper.Add(superposid);
                            }
                            dt = SQLHelper.ProcDataTable("usp_SuperEid", new SqlParameter("@id", supereid));
                            supereid = Convert.ToInt32(dt.Rows[0]["employeeid"]);
                        }
                        while (!superposid.Equals(depposid));
                        foreach (string item in tempSuper)
                        {
                            if (!signlist.Contains(item))
                            {
                                signlist.Add(item);
                            }
                        }
                        if (!signlist.Contains(depposid))
                        {
                            signlist.Add(depposid);
                        }
                        continue;
                    }
                }
            }
            signlist.Add(flowlist[flowlist.Length - 1]);
            return signlist;
        }

    }
}