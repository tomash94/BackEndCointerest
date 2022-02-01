using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Configuration;

namespace BackEndCointerest.Models.DAL
{
    public class DBServices
    {
        public SqlDataAdapter da;
        public DataTable dt;
        public DBServices()
        {

        }
        public SqlConnection connect(String conString)
        {

            // read the connection string from the configuration file
            string cStr = WebConfigurationManager.ConnectionStrings[conString].ConnectionString;
            SqlConnection con = new SqlConnection(cStr);
            con.Open();
            return con;
        }

        public List<User> Get_users(string username1)
        {
            SqlConnection con = null;
            List<User> users = new List<User>();

            try
            {
                con = connect("DBConnectionString");

                String selectSTR = "SELECT * FROM Users_2022 WHERE username = '" + username1 +"'";

                SqlCommand cmd = new SqlCommand(selectSTR, con);

                // get a reader
                SqlDataReader dr = cmd.ExecuteReader(CommandBehavior.CloseConnection);


                while (dr.Read())
                {
                    User usr = new User();
                    usr.Username = (string)dr["username"];
                    usr.Birthdate = (DateTime)dr["birthdate"];
                    usr.Image = (string)dr["userImage"];
                    usr.Password = (string)dr["userPassword"];
                    users.Add(usr);

                }
                return users;
            }
            catch (Exception ex)
            {
                throw (ex);
            }

            finally
            {
                if (con != null)
                {
                    con.Close();
                }

            }

        }




    }
}