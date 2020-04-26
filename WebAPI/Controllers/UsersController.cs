using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace WebApplication1.Controllers
{
    [Route("api/")]
    public class UsersController : ControllerBase
    {

        [HttpGet]
        [Route("get/users")]
        public JsonResult GetUsers()
        {
            return GetUsersData();
        }

        [HttpPost]
        [Route("add/user")]
        public JsonResult AddUser(/*[FromBody]*/ UserBody userBody)
        {
            try
            {
                SqlCommand command = new SqlCommand
                {
                    CommandText = "SP_ADD_USER",
                    CommandType = CommandType.StoredProcedure
                };
                command.Parameters.Add(new SqlParameter("@user_name", userBody.user_name));
                command.Parameters.Add(new SqlParameter("@user_age", userBody.user_age));
                command.Parameters.Add(new SqlParameter("@user_position", userBody.user_position));

                ExecuteSPNonQuery(command);

                return GetUsersData();

            }
            catch (Exception ex)
            {
                return ReturnCatch(ex);
            }
        }

        [HttpPost]
        [Route("edit/user")]
        public JsonResult EditUser(/*[FromBody]*/ UserBody userBody)
        {
            try
            {
                SqlCommand command = new SqlCommand
                {
                    CommandText = "SP_EDIT_USER",
                    CommandType = CommandType.StoredProcedure
                };
                command.Parameters.Add(new SqlParameter("@user_id", userBody.user_id));
                command.Parameters.Add(new SqlParameter("@user_name", userBody.user_name));
                command.Parameters.Add(new SqlParameter("@user_age", userBody.user_age));
                command.Parameters.Add(new SqlParameter("@user_position", userBody.user_position));

                ExecuteSPNonQuery(command);

                return GetUsersData();

            }
            catch (Exception ex)
            {
                return ReturnCatch(ex);
            }
        }

        [HttpPost]
        [Route("delete/user/{user_id}")]
        public JsonResult DeleteUser(int user_id)
        {
            try
            {
                StringBuilder sb = new StringBuilder(); 

                sb.Remove(0, sb.Length);
                sb.Append("DELETE FROM ");  
                sb.Append("[Users] ");
                sb.Append("WHERE ");
                sb.Append("[user_id] = '");
                sb.Append(user_id);
                sb.Append("'");
                 
                ExecuteSPNonQuery(new SqlCommand(sb.ToString()));

                return GetUsersData();

            }
            catch (Exception ex)
            {
                return ReturnCatch(ex);
            }
        }
         

        private JsonResult GetUsersData()
        {
            try
            {
                StringBuilder sb = new StringBuilder();
                DataTable dt = new DataTable();

                sb.Remove(0, sb.Length);
                sb.Append("SELECT ");
                sb.Append("* ");
                sb.Append("FROM ");
                sb.Append("[Users] ");
                sb.Append("ORDER BY ");
                sb.Append("[user_id] ASC");

                dt = ExcuteDataTable(sb.ToString());
                 
                return ReturnData(dt);
            }
            catch (Exception ex)
            {
                return ReturnCatch(ex);
            }
        }

        public static string GetConnection()
        {
            SqlConnectionStringBuilder conn = new SqlConnectionStringBuilder();
            conn.DataSource = @"DESKTOP-4BCP7V8\MSSQLSERVER2014";
            conn.InitialCatalog = "WebAPI";
            conn.UserID = "sa";
            conn.Password = "12345";
            conn.ConnectTimeout = 10;

            return conn.ToString();
        }

        public DataTable ExcuteDataTable(string query)
        {
            try
            {
                SqlConnection sqlConnection = new SqlConnection();
                DataTable dataTable = new DataTable();

                sqlConnection.ConnectionString = GetConnection();
                sqlConnection.Open();

                SqlDataAdapter dataAdapter = new SqlDataAdapter(query, GetConnection());
                dataAdapter.Fill(dataTable);

                sqlConnection.Close();

                if ((dataTable == null))
                {
                    return null;
                }
                else
                {
                    return dataTable;
                }
            }
            catch (Exception)
            {
                return null;
            }

        }
         
        public void ExecuteSPNonQuery(SqlCommand command)
        {
            try
            {
                SqlConnection sqlConnection = new SqlConnection(GetConnection());
                sqlConnection.Open(); 
                command.Connection = sqlConnection; 
                int success = command.ExecuteNonQuery();
                sqlConnection.Close();
                sqlConnection = null;  
            }
            catch (Exception)
            { 
            } 
        }
        
        private JsonResult ReturnCatch(Exception ex)
        {
            return new JsonResult(new
            {
                message = ex
            });
        }

        private JsonResult ReturnData(DataTable dt)
        {
            return new JsonResult(new
            {
                message = dt
            });
        }

    }
}