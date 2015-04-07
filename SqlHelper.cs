using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Text.RegularExpressions;

namespace UDS
{
    public class SQLHelper
    {
        //连接字符串
        //private static string connStr = ConfigurationManager.ConnectionStrings[2].ConnectionString;
        private static string connStr = ConfigurationManager.ConnectionStrings["ConnStr"].ConnectionString;

        /// <summary>
        /// 非查询类数据库操作，返回受影响的行数或数据库异常
        /// </summary>
        /// <param name="sql">sql语句</param>
        /// <param name="parameters">sql语句中的参数可变数组</param>
        /// <returns>受影响的行数</returns>
        public static int ExecuteNonQuery(string sql, params SqlParameter[] parameters)
        {
            try
            {
                using (SqlConnection sqlConn = new SqlConnection(connStr))
                {
                    if (sqlConn.State != ConnectionState.Open)
                    {
                        sqlConn.Open();
                    }
                    using (SqlCommand cmd = new SqlCommand(sql, sqlConn))
                    {
                        if (parameters != null && parameters.Length != 0)
                        {
                            cmd.Parameters.AddRange(parameters);
                        }
                        return cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (SqlException SqlExp)
            {
                throw SqlExp;
            }
            catch (InvalidOperationException IvOExp)
            {
                throw IvOExp;
            }
            catch (ArgumentException ArguExp)
            {
                throw ArguExp;
            }
        }

        /// <summary>
        /// 非查询类数据库操作
        /// </summary>
        /// <param name="sql">sql语句</param>
        /// <param name="objs">Object对象数组表示的参数数组</param>
        /// <returns>受影响的行数</returns>
        public static int ExecuteNonQuery(string sql, params object[] objs)
        {
            List<SqlParameter> list = InsertParameters(sql, objs);
            return ExecuteNonQuery(sql, list.ToArray());
        }

        /// <summary>
        /// 单值结果的查询操作
        /// </summary>
        /// <param name="sql">sql语句</param>
        /// <param name="parameters">参数数组，可变</param>
        /// <returns>第一行第一列的查询结果</returns>
        public static object ExecuteScalar(string sql, params SqlParameter[] parameters)
        {
            try
            {
                using (SqlConnection sqlConn = new SqlConnection(connStr))
                {
                    if (sqlConn.State != ConnectionState.Open)
                    {
                        sqlConn.Open();
                    }
                    using (SqlCommand cmd = new SqlCommand(sql, sqlConn))
                    {
                        if (parameters != null && parameters.Length != 0)
                        {
                            cmd.Parameters.AddRange(parameters);
                        }
                        return cmd.ExecuteScalar();
                    }
                }
            }
            catch (SqlException SqlExp)
            {
                throw SqlExp;
            }
            catch (InvalidOperationException IvOExp)
            {
                throw IvOExp;
            }
            catch (ArgumentException ArguExp)
            {
                throw ArguExp;
            }
        }

        /// <summary>
        /// 单值查询操作
        /// </summary>
        /// <param name="sql">sql语句</param>
        /// <param name="objs">参数的Object数组</param>
        /// <returns>查询到的第一行第一列的结果</returns>
        public static object ExecuteScalar(string sql, params object[] objs)
        {
            List<SqlParameter> list = InsertParameters(sql, objs);
            return ExecuteScalar(sql, list.ToArray());
        }

        /// <summary>
        /// 在线查询数据库操作
        /// </summary>
        /// <param name="sqlConn">out参数，返回数据库的连接</param>
        /// <param name="sql">sql语句</param>
        /// <param name="parameters">参数数组</param>
        /// <returns>在线查询指针SqlDataReader对象</returns>
        public static SqlDataReader ExecuteReader(out SqlConnection sqlConn, string sql, params SqlParameter[] parameters)
        {
            try
            {
                sqlConn = new SqlConnection(connStr);
                if (sqlConn.State != ConnectionState.Open)
                {
                    sqlConn.Open();
                }
                using (SqlCommand cmd = new SqlCommand(sql, sqlConn))
                {
                    if (parameters != null && parameters.Length != 0)
                    {
                        cmd.Parameters.AddRange(parameters);
                    }
                    return cmd.ExecuteReader();
                }
            }
            catch (SqlException SqlExp)
            {
                throw SqlExp;
            }
            catch (InvalidOperationException IvOExp)
            {
                throw IvOExp;
            }
            catch (ArgumentException ArguExp)
            {
                throw ArguExp;
            }
        }

        /// <summary>
        /// 在线查询数据库操作
        /// </summary>
        /// <param name="sqlConn">out参数，返回数据库的连接</param>
        /// <param name="sql">sql语句</param>
        /// <param name="objs">参数的Object数组</param>
        /// <returns>在线查询的指针SqlDataReader对象</returns>
        public static SqlDataReader ExecuteReader(out SqlConnection sqlConn, string sql, params object[] objs)
        {
            List<SqlParameter> list = InsertParameters(sql, objs);
            return ExecuteReader(out sqlConn, sql, list.ToArray());
        }

        public static SqlDataReader ExecuteReader(string sql, params SqlParameter[] parameters)
        {
            try
            {
                SqlConnection sqlConn = new SqlConnection(connStr);
                if (sqlConn.State != ConnectionState.Open)
                {
                    sqlConn.Open();
                }
                using (SqlCommand cmd = new SqlCommand(sql, sqlConn))
                {
                    if (parameters != null && parameters.Length != 0)
                    {
                        cmd.Parameters.AddRange(parameters);
                    }
                    return cmd.ExecuteReader(CommandBehavior.CloseConnection);
                }
            }
            catch (SqlException SqlExp)
            {
                throw SqlExp;
            }
            catch (InvalidOperationException IvOExp)
            {
                throw IvOExp;
            }
            catch (ArgumentException ArguExp)
            {
                throw ArguExp;
            }
        }

        public static SqlDataReader ExecuteReader(string sql, params object[] objs)
        {
            List<SqlParameter> list = InsertParameters(sql, objs);
            return ExecuteReader(sql, list.ToArray());
        }

        /// <summary>
        /// 离线数据库查询操作
        /// </summary>
        /// <param name="sql">sql语句</param>
        /// <param name="parameters">参数数组</param>
        /// <returns>查询到的数据表</returns>
        public static DataTable ExecuteDataTable(string sql, params SqlParameter[] parameters)
        {
            try
            {
                return ExecuteDataTables(sql, parameters)[0];
            }
            catch (SqlException SqlExp)
            {
                throw SqlExp;
            }
            catch (InvalidOperationException IvOExp)
            {
                throw IvOExp;
            }
            catch (ArgumentException ArguExp)
            {
                throw ArguExp;
            }
        }

        /// <summary>
        /// 离线查询数据库操作
        /// </summary>
        /// <param name="sql">sql语句</param>
        /// <param name="objs">参数的Object数组</param>
        /// <returns>查询到的数据表</returns>
        public static DataTable ExecuteDataTable(string sql, params object[] objs)
        {
            List<SqlParameter> list = InsertParameters(sql, objs);
            return ExecuteDataTables(sql, list.ToArray())[0];
        }

        /// <summary>
        /// 离线查询数据库操作
        /// </summary>
        /// <param name="sql">sql语句</param>
        /// <param name="parameters">参数数组</param>
        /// <returns>查询到的多张数据表</returns>
        public static DataTableCollection ExecuteDataTables(string sql, params SqlParameter[] parameters)
        {
            try
            {
                using (SqlConnection sqlConn = new SqlConnection(connStr))
                {
                    if (sqlConn.State != ConnectionState.Open)
                    {
                        sqlConn.Open();
                    }
                    using (SqlCommand cmd = new SqlCommand(sql, sqlConn))
                    {
                        if (parameters != null && parameters.Length != 0)
                        {
                            cmd.Parameters.AddRange(parameters);
                        }
                        using (SqlDataAdapter adapter = new SqlDataAdapter(cmd))
                        {
                            using (DataSet dataset = new DataSet())
                            {
                                adapter.Fill(dataset);
                                return dataset.Tables;
                            }
                        }
                    }
                }
            }
            catch (SqlException SqlExp)
            {
                throw SqlExp;
            }
            catch (InvalidOperationException IvOExp)
            {
                throw IvOExp;
            }
            catch (ArgumentException ArguExp)
            {
                throw ArguExp;
            }
        }

        /// <summary>
        /// 离线查询数据库操作
        /// </summary>
        /// <param name="sql">sql语句</param>
        /// <param name="objs">参数的Object数组</param>
        /// <returns>查询到的多张表</returns>
        public static DataTableCollection ExecuteDataTables(string sql, params object[] objs)
        {
            List<SqlParameter> list = InsertParameters(sql, objs);
            return ExecuteDataTables(sql, list.ToArray());
        }

        /// <summary>
        /// 参数转换
        /// </summary>
        /// <param name="sql">sql语句</param>
        /// <param name="objs">Object参数数组</param>
        /// <returns>sql语句中的参数对象数组</returns>
        private static List<SqlParameter> InsertParameters(string sql, params object[] objs)
        {
            string regex = @"@\w+";
            MatchCollection ms = Regex.Matches(sql, regex);
            if (ms.Count > 0 && objs == null)
            {
                throw new Exception("Sql语句中存在参数，但未输入任何参数");
            }
            if (ms.Count != objs.Length)
            {
                throw new Exception("Sql语句中的变量与传入的参数的个数不一致");
            }
            List<SqlParameter> list = new List<SqlParameter>();
            for (int i = 0; i < ms.Count; i++)
            {
                list.Add(new SqlParameter(ms[i].Value, objs[i]));
            }
            return list;
        }

        /// <summary>
        /// 离线查询操作
        /// </summary>
        /// <param name="sql">sql语句</param>
        /// <param name="objs">Object参数数组</param>
        /// <returns>离线数据集DataSet对象</returns>
        public static DataSet ExecuteDataSet(string sql, params object[] objs)
        {
            List<SqlParameter> list = InsertParameters(sql, objs);
            return ExecuteDataSet(sql, list.ToArray());
        }

        /// <summary>
        /// 离线查询数据库操作
        /// </summary>
        /// <param name="sql">sql语句</param>
        /// <param name="parameters">可变参数数组</param>
        /// <returns>离线数据集DataSet对象</returns>
        public static DataSet ExecuteDataSet(string sql, params SqlParameter[] parameters)
        {
            try
            {
                DataSet dataset;
                using (SqlConnection sqlConn = new SqlConnection(connStr))
                {
                    if (sqlConn.State != ConnectionState.Open)
                    {
                        sqlConn.Open();
                    }
                    using (SqlCommand cmd = new SqlCommand(sql, sqlConn))
                    {
                        if (parameters != null && parameters.Length != 0)
                        {
                            cmd.Parameters.AddRange(parameters);
                        }
                        using (SqlDataAdapter adapter = new SqlDataAdapter(cmd))
                        {
                            dataset = new DataSet();
                            adapter.Fill(dataset);
                        }
                    }
                }
                return dataset;
            }
            catch (SqlException SqlExp)
            {
                throw SqlExp;
            }
            catch (InvalidOperationException IvOExp)
            {
                throw IvOExp;
            }
            catch (ArgumentException ArguExp)
            {
                throw ArguExp;
            }
        }


        public static SqlDataAdapter ExecuteSqlDataAdapter(out SqlConnection sqlConn, string sql, params SqlParameter[] parameters)
        {
            try
            {
                SqlDataAdapter adapter;
                sqlConn = new SqlConnection(connStr);
                if (sqlConn.State != ConnectionState.Open)
                {
                    sqlConn.Open();
                }
                using (SqlCommand cmd = new SqlCommand(sql, sqlConn))
                {
                    if (parameters != null && parameters.Length != 0)
                    {
                        cmd.Parameters.AddRange(parameters);
                    }
                    adapter = new SqlDataAdapter(cmd);
                }
                return adapter;
            }
            catch (SqlException SqlExp)
            {
                throw SqlExp;
            }
            catch (InvalidOperationException IvOExp)
            {
                throw IvOExp;
            }
            catch (ArgumentException ArguExp)
            {
                throw ArguExp;
            }
        }

        public static SqlDataAdapter ExecuteSqlDataAdapter(out SqlConnection sqlConn, string sql, params object[] objs)
        {
            List<SqlParameter> list = InsertParameters(sql, objs);
            return ExecuteSqlDataAdapter(out sqlConn, sql, list.ToArray());
        }

        /// <summary>
        /// 根据id删除数据的存储过程
        /// </summary>
        /// <param name="procedurename">存储过程名</param>
        /// <param name="id">要删除的数据的id</param>
        public static void ProcedureDeleteById(string procedurename, int id)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connStr))
                {
                    if (conn.State != ConnectionState.Open)
                    {
                        conn.Open();
                    }
                    using (SqlCommand cmd = new SqlCommand(procedurename, conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        //由于存储过程名中没有参数信息，因此无法做成通用存储过程调用_2014-08-01
                        cmd.Parameters.Add("@id", SqlDbType.Int).Value = id;
                        using (SqlDataReader dr = cmd.ExecuteReader())
                        {
                            //while (dr.Read())
                            //{
                            //    Console.WriteLine(dr.GetString(dr.GetOrdinal("text")));
                            //}
                        }
                    }
                }
            }
            catch (SqlException SqlExp)
            {
                throw SqlExp;
            }
            catch (InvalidOperationException IvOExp)
            {
                throw IvOExp;
            }
            catch (ArgumentException ArguExp)
            {
                throw ArguExp;
            }
            catch (InvalidCastException InCExp)
            {
                throw InCExp;
            }
            catch (IndexOutOfRangeException IndexExp)
            {
                throw IndexExp;
            }
        }

        /// <summary>
        /// 执行查询存储过程，返回查询到的数据表
        /// </summary>
        /// <param name="procname">存储过程名</param>
        /// <param name="parameters">存储过程的参数</param>
        /// <returns></returns>
        public static DataTable ProcDataTable(string procname, params SqlParameter[] parameters)
        {
            try
            {
                DataTable dt;
                using (SqlConnection sqlConn = new SqlConnection(connStr))
                {
                    using (SqlCommand cmd = new SqlCommand(procname, sqlConn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        if (parameters != null && parameters.Length != 0)
                        {
                            cmd.Parameters.AddRange(parameters);
                        }
                        dt = new DataTable();
                        SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                        adapter.Fill(dt);
                    }
                }
                return dt;
            }
            catch (SqlException SqlExp)
            {
                throw SqlExp;
            }
            catch (InvalidOperationException IvOExp)
            {
                throw IvOExp;
            }
            catch (ArgumentException ArguExp)
            {
                throw ArguExp;
            }
        }

        /// <summary>
        /// 非查询类存储过程，返回受影响的行数
        /// </summary>
        /// <param name="procnamne">存储过程名</param>
        /// <param name="parameters">存储过程的参数</param>
        /// <returns></returns>
        public static int ProcNoQuery(string procnamne, params SqlParameter[] parameters)
        {
            try
            {
                using(SqlConnection sqlConn = new SqlConnection(connStr))
	            {
                    using(SqlCommand cmd = new SqlCommand(procnamne, sqlConn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        if (parameters != null && parameters.Length != 0)
                        {
                            cmd.Parameters.AddRange(parameters);
                        }
                        sqlConn.Open();
                        return cmd.ExecuteNonQuery();
                    }
	            }
            }
            catch (SqlException SqlExp)
            {
                throw SqlExp;
            }
            catch (InvalidOperationException IvOExp)
            {
                throw IvOExp;
            }
            catch (ArgumentException ArguExp)
            {
                throw ArguExp;
            }
        }
    }
}


