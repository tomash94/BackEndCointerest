using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
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
        //---------------------------------------------------------------------------------
        // Create the SqlCommand
        //---------------------------------------------------------------------------------
        private SqlCommand CreateCommand(String CommandSTR, SqlConnection con)
        {

            SqlCommand cmd = new SqlCommand(); // create the command object

            cmd.Connection = con;              // assign the connection to the command object

            cmd.CommandText = CommandSTR;      // can be Select, Insert, Update, Delete 

            cmd.CommandTimeout = 10;           // Time to wait for the execution' The default is 30 seconds

            cmd.CommandType = System.Data.CommandType.Text; // the type of the command, can also be stored procedure

            return cmd;
        }

        //---------------------------------------------------------------------------------
        // Inserts
        //---------------------------------------------------------------------------------

        public int Insert(object obj)
        {
            SqlConnection con;
            SqlCommand cmd;

            try
            {
                con = connect("DBConnectionString"); // create the connection
            }
            catch (Exception ex)
            {
                // write to log
                throw (ex);
            }

            String cStr = BuildInsertCommand(obj);      // helper method to build the insert string
            cmd = CreateCommand(cStr, con);             // create the command

            try
            {
                int numEffected = cmd.ExecuteNonQuery(); // execute the command
                return numEffected;
            }
            catch (SqlException ex)
            {
                if (ex.Number == 2627)
                {
                    return 0;
                }
                else throw;
            }

            finally
            {
                if (con != null)
                {
                    // close the db connection
                    con.Close();
                }
            }

        }

        private String BuildInsertCommand(Object obj)
        {
            String command = "";
            StringBuilder sb = new StringBuilder();

            if (obj is User)
            {
                User user = (User)obj;
                // use a string builder to create the dynamic string
                sb.AppendFormat("Values('{0}', '{1}', '{2}', '{3}', '{4}')", user.Email, user.Username, user.Password, user.Image, user.Bio);
                String prefix = "INSERT INTO Users_2022 " + "([Email],[username], [userPassword], [userImage],[bio])";
                command = prefix + sb.ToString();
            }

            if (obj is Coin_update)
            {
                Coin_update c_update = (Coin_update)obj;
                // use a string builder to create the dynamic string
                sb.AppendFormat("Values('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}',", c_update.Coin_name, c_update.Coin_value, c_update.Symbol, c_update.Percent_change_1h, c_update.Percent_change_24h, c_update.Percent_change_7d, c_update.Percent_change_30d);
                String prefix = "INSERT INTO Coin_Updates_2022 " + "([coin_name],[coin_value],[symbol],[p_change_1h],[p_change_24h],[p_change_7d],[p_change_30d],[update_time])";
                command = prefix + sb.ToString() + "convert(datetime, '" + c_update.Update_date.ToString("yyyy-MM-dd HH:mm:ss.fff") + "'))";
            }

            //  ---     trying to append a full list in one request    --- 
            if (obj is List<Coin_update>)
            {
                List<Coin_update> update_list = (List<Coin_update>)obj;
                command = "INSERT INTO Coin_Updates_2022 " + "([coin_name],[coin_value],[symbol],[p_change_1h],[p_change_24h],[p_change_7d],[p_change_30d],[volume_24h],[update_time]) Values";
                foreach (Coin_update item in update_list)
                {
                    sb.AppendFormat("('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}',", item.Coin_name, item.Coin_value, item.Symbol, item.Percent_change_1h, item.Percent_change_24h, item.Percent_change_7d, item.Percent_change_30d, item.Volume_24h);
                    command += sb.ToString() + "convert(datetime, '" + item.Update_date.ToString("yyyy-MM-dd HH:mm:ss.fff") + "'))";
                    sb = new StringBuilder();
                    if (update_list.IndexOf(item) != update_list.Count - 1) // if its not last add ","
                    {
                        command += ", ";
                    }
                }
            }

            if (obj is Transaction)
            {
                Transaction trans = (Transaction)obj;
                sb.AppendFormat("Values('{0}', '{1}', '{2}', '{3}', '{4}',", trans.Email, trans.Coin_name, trans.Coin_amount, trans.Dollar_amount, trans.Comment);
                String prefix = "INSERT INTO Transactions_2022 " + "([email], [coin_name], [coin_amount], [dollar_amount], [comment], [t_date])";
                command = prefix + sb.ToString() + "convert(datetime, '" + trans.T_date.ToString("yyyy-MM-dd HH:mm:ss.fff") + "'))";
            }

            if (obj is Asset) 
            {
                // will check if an asset already exists, if so it will update its values based on the transaction
                // else
                // create a new asset 


                Asset ast = (Asset)obj;

                String cStr = "IF EXISTS (SELECT 1 FROM Assets_2022 WHERE email = '" + ast.Email + "' AND coin_name='" + ast.Coin_name + "')";
                cStr += "BEGIN ";
                cStr += "  UPDATE Assets_2022 ";
                cStr += "  SET amount = amount +" + ast.Amount;
                cStr += "  WHERE email = '" + ast.Email + "' AND coin_name='" + ast.Coin_name + "' ";
                cStr += "END ";
                cStr += "ELSE ";
                cStr += "BEGIN ";
                
                String prefix = "INSERT INTO Assets_2022  ([email], [coin_name], [amount]) "+ sb.AppendFormat("Values('{0}', '{1}', '{2}')", ast.Email, ast.Coin_name, ast.Amount).ToString(); 
                command = cStr +prefix + " END";
            }

            if (obj is Prediction)
            {
                Prediction p = (Prediction)obj;
                String prefix;
                if (p.X_current_price == -1)
                {

                    prefix = "UPDATE Predictions_2022 set y_true_price =" + p.Y_true_price + "where coin_name='" + p.Coin_name + "' And y_time IN (SELECT max(y_time) FROM Predictions_2022 )";
                    command = prefix;
                }
                else
                {

                    sb.AppendFormat("Values('{0}', '{1}', '{2}',", p.Coin_name, p.Predicted_price, p.X_current_price);
                    prefix = "INSERT INTO Predictions_2022 " + "([coin_name], [predicted_price], [x_current_price], [x_time], [y_time] )";
                    command = prefix + sb.ToString() + "convert(datetime, '" + p.X_time.ToString("yyyy-MM-dd HH:mm:ss.fff") + "')" + ", convert(datetime, '" + p.Y_time.ToString("yyyy-MM-dd HH:mm:ss.fff") + "')) ";
                }
            }

            if (obj is Tweet)
            {
                Tweet t = (Tweet)obj;
                sb.AppendFormat("Values('{0}', '{1}', '{2}', '{3}', '{4}',", t.Tweet_id, t.Author, t.Comp_score, t.Engagement, t.Tweet_text);
                String prefix = "INSERT INTO Cointerest_Tweets_2022 " + "([tweet_id], [author], [comp_score], [engagement], [tweet_text], [tweet_time])";
                command = prefix + sb.ToString() + " convert(datetime, '" + t.Tweet_time.ToString("yyyy-MM-dd HH:mm:ss.fff") + "'))";

            }

            if (obj is Following)
            {
                Following f = (Following)obj;
                sb.AppendFormat("Values('{0}', '{1}')", f.Email, f.Username);
                String prefix = "INSERT INTO Following_2022 " + "([email], [username])";
                command = prefix + sb.ToString();
            }

            if (obj is List<Wallet_worth>)
            {
                List<Wallet_worth> w_list = (List<Wallet_worth>)obj;
                command = "INSERT INTO Cointerest_wallet_value_2022 " + "([email],[dollar_worth],[time_stamp]) Values";
                foreach (Wallet_worth item in w_list)
                {
                    sb.AppendFormat("('{0}', '{1}', ", item.Email, item.Worth);
                    command += sb.ToString() + "convert(datetime, '" + item.Time.ToString("yyyy-MM-dd HH:mm:ss.fff") + "'))";
                    sb = new StringBuilder();
                    if (w_list.IndexOf(item) != w_list.Count - 1) // if its not last add ","
                    {
                        command += ", ";
                    }
                }
            }

            if(obj is Wallet_worth) //means its a "sign up" new user - needs to be added to wallet value table in the db.
            {
                Wallet_worth w_item = (Wallet_worth)obj;
                command = "INSERT INTO Cointerest_wallet_value_2022  ([email],[dollar_worth],[time_stamp]) Values";
                sb.AppendFormat("('{0}', '{1}', ", w_item.Email, w_item.Worth);
                command += sb.ToString() + " convert(datetime, '" + w_item.Time.ToString("yyyy-MM-dd HH:mm:ss.fff") + "'))";
            }

            if(obj is Login)
            {
                Login l = (Login)obj;
                sb.AppendFormat("Values('{0}', ", l.Email);
                String prefix = "INSERT INTO Logins_2022 " + "([email],[time])";
                command = prefix + sb.ToString() + "convert(datetime, '" + l.Time.ToString("yyyy-MM-dd HH:mm:ss.fff") + "'))";

            }

            return command;
        }


       
        


        //---------------------------------------------------------------------------------
        // Delete
        //---------------------------------------------------------------------------------


        public int Delete(object obj)
        {
            SqlConnection con;
            SqlCommand cmd;

            try
            {
                con = connect("DBConnectionString"); // create the connection
            }
            catch (Exception ex)
            {
                // write to log
                throw (ex);
            }

            String cStr = BuildDeleteCommand(obj);      // helper method to build the delete string
            cmd = CreateCommand(cStr, con);             // create the command

            try
            {
                int numEffected = cmd.ExecuteNonQuery(); // execute the command
                return numEffected;
            }
            catch (SqlException ex)
            {
                if (ex.Number == 2627)
                {
                    return 0;
                }
                else throw;
            }

            finally
            {
                if (con != null)
                {
                    // close the db connection
                    con.Close();
                }
            }

        }
        private String BuildDeleteCommand(Object obj)
        {
            String command = "";
            StringBuilder sb = new StringBuilder();

            if (obj is Following) // email is UNFOLLOWING username 
            {
                Following f = (Following)obj;
                sb.AppendFormat("email = '{0}' AND username = '{1}'", f.Email, f.Username);
                String prefix = "DELETE FROM Following_2022 WHERE ";
                command = prefix + sb.ToString();
            }

            return command;

        }

        //---------------------------------------------------------------------------------
        // Update
        //---------------------------------------------------------------------------------

        public int update_bio(string email, string bio)
        {

            SqlConnection con = null;

            try
            {

                con = connect("DBConnectionString");

                String command = "";
                StringBuilder sb = new StringBuilder();
                sb.AppendFormat("'{0}' ", bio);
                String prefix = "Update Users_2022 SET bio = ";
                command = prefix + sb.ToString() + " where email = '" + email + "'";

                SqlCommand cmd = new SqlCommand(command, con);


                int numEffected = cmd.ExecuteNonQuery(); // execute the command
                return numEffected;

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

        public int Change_image(string email, string pic_url)
        {

            SqlConnection con;
            SqlCommand cmd;

            try
            {
                con = connect("DBConnectionString"); // create the connection
            }
            catch (Exception ex)
            {
                // write to log
                throw (ex);
            }

            String command = "";
            StringBuilder sb1 = new StringBuilder();
            StringBuilder sb2 = new StringBuilder();

            sb1.AppendFormat("'{0}'", pic_url);
            sb2.AppendFormat("'{0}'", email);
            String prefix = "UPDATE Users_2022 SET userImage = ";
            command = prefix + sb1.ToString() + " where email=" + sb2.ToString();


            // helper method to build the insert string
            cmd = CreateCommand(command, con);             // create the command

            try
            {
                int numEffected = cmd.ExecuteNonQuery(); // execute the command
                return numEffected;
            }
            catch (SqlException ex)
            {
                if (ex.Number == 2627)
                {
                    return 0;
                }
                else throw;
            }

            finally
            {
                if (con != null)
                {
                    // close the db connection
                    con.Close();
                }
            }

        }

        //---------------------------------------------------------------------------------
        // Get - mix
        //---------------------------------------------------------------------------------

        public List<TopUser> top_5_earners(DateTime since)
        {
            //get a list of the top 5 earners by % size of total wallet worth.

            SqlConnection con = null;
            List<TopUser> u_list = new List<TopUser>();

            try
            {
                con = connect("DBConnectionString");

                String selectSTR = " ";
                selectSTR += "select top(5) U.userImage,u.username, u.bio, new.email, new.dollar_worth as balance, (((new.dollar_worth-old.dollar_worth)/old.dollar_worth)*100) as percent_change   ";
                selectSTR += " from (select N.email, N.time_stamp, N.dollar_worth";
                selectSTR += " from Cointerest_wallet_value_2022 N";
                selectSTR += " inner join (";
                selectSTR += " select email, max(time_stamp) as MaxDate";
                selectSTR += " from Cointerest_wallet_value_2022";
                selectSTR += " group by email";
                selectSTR += " ) tm on N.email = tm.email and N.time_stamp = tm.MaxDate";
                selectSTR += " ) as NEW inner join ";
                selectSTR += " (select N.email, N.time_stamp, N.dollar_worth";
                selectSTR += " from Cointerest_wallet_value_2022 N";
                selectSTR += " inner join (";
                selectSTR += " select email, min(time_stamp) as MaxDate";
                selectSTR += " from Cointerest_wallet_value_2022";
                selectSTR += " where time_stamp >= (getdate()-7)";
                selectSTR += " group by email";
                selectSTR += " ) tm on N.email = tm.email and N.time_stamp = tm.MaxDate)";
                selectSTR += " as OLD on NEW.email = OLD.email";
                selectSTR += " inner join Users_2022 as U on U.email = new.email";
                selectSTR += " order by percent_change desc";

                SqlCommand cmd = new SqlCommand(selectSTR, con);

                // get a reader
                SqlDataReader dr = cmd.ExecuteReader(CommandBehavior.CloseConnection);


                while (dr.Read())
                {
                    TopUser item = new TopUser();
                    item.Email = dr["email"] as string;
                    item.Bio = dr["bio"] as string;
                    item.Username = dr["username"] as string;
                    item.Image = dr["userImage"] as string;
                    item.Balance.Net_worth = (float)Math.Round((Decimal)((float)(double)dr["balance"]), 2, MidpointRounding.AwayFromZero);
                    item.Balance.Weekly_change_percent = (float)Math.Round((Decimal)((float)(double)dr["percent_change"]), 2, MidpointRounding.AwayFromZero);

                    u_list.Add(item);
                }
                return u_list;
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

        public User Get_user(string email)
        {
            SqlConnection con = null;

            try
            {
                con = connect("DBConnectionString");

                String selectSTR = "SELECT * FROM Users_2022 WHERE email = '" + email + "'";

                SqlCommand cmd = new SqlCommand(selectSTR, con);

                // get a reader
                SqlDataReader dr = cmd.ExecuteReader(CommandBehavior.CloseConnection);

                User usr = new User();
                while (dr.Read())
                {
                    usr.Email = dr["email"] as string;
                    usr.Username = dr["username"] as string;
                    usr.Image = dr["userImage"] as string;
                    usr.Password = dr["userPassword"] as string;
                    usr.Bio = dr["bio"] as string;
                }
                return usr;
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




        public List<User> Search_users(string username_query_search, int n)
        {
            SqlConnection con = null;

            try
            {
                con = connect("DBConnectionString");
                String selectSTR = "";

                if (n == 0) // search by username
                {
                    selectSTR = "select * from Users_2022 where username like '%" + username_query_search + "%'";
                }
                if(n == 1) // search by owning a certain coin
                {
                    selectSTR = " select U.bio, U.userImage, U.username, u.email, A.amount from Users_2022 as U inner join Assets_2022 as A on A.email = U.email ";
                    selectSTR += " where A.coin_name = '"+username_query_search+"'";
                    selectSTR += " order by amount desc";
                }

                SqlCommand cmd = new SqlCommand(selectSTR, con);

                // get a reader
                SqlDataReader dr = cmd.ExecuteReader(CommandBehavior.CloseConnection);

                List<User> user_list = new List<User>();
                
                while (dr.Read())
                {
                    User usr = new User();
                    usr.Email = dr["email"] as string;
                    usr.Username = dr["username"] as string;
                    usr.Image = dr["userImage"] as string;
                    
                    usr.Bio = dr["bio"] as string;
                    user_list.Add(usr);
                }
                return user_list;
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


        public int check_follow(string email, string discover_user)
        {

            SqlConnection con = null;

            try
            {
                con = connect("DBConnectionString");

                String selectSTR = "SELECT * FROM Following_2022 WHERE email = '" + email + "' AND username='"+discover_user+"'";

                SqlCommand cmd = new SqlCommand(selectSTR, con);

                // get a reader
                SqlDataReader dr = cmd.ExecuteReader(CommandBehavior.CloseConnection);
                int i = 0;
                User usr = new User();
                while (dr.Read())
                {
                    i++;
                }
                return i;
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




        //  --    Coins    -- //

        public Coin get_coin_info(string coin_name)
        {
            SqlConnection con = null;
            Coin coin = new Coin();

            try
            {
                con = connect("DBConnectionString");

                String selectSTR = "SELECT * FROM Coins_2022 WHERE coin_name ='" + coin_name + "'";

                SqlCommand cmd = new SqlCommand(selectSTR, con);

                // get a reader
                SqlDataReader dr = cmd.ExecuteReader(CommandBehavior.CloseConnection);


                while (dr.Read())
                {
                    coin.Coin_name = dr["coin_name"] as string;
                    coin.Coin_info = dr["coin_info"] as string;
                    coin.Coin_url = dr["coin_url"] as string;
                    coin.Coin_picture = dr["coin_picture"] as string;
                    if (coin.Coin_name != null)
                    {
                        coin.Price_history = Get_latest_price_per_coin_name(coin.Coin_name);
                    }
                  
                }
                return coin;
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



        //this will return the last date & price of a single coin
        public List<Coin_update> Get_latest_price_per_coin_name(string coin_name)
        {
            SqlConnection con = null;
            List<Coin_update> latest_prices = new List<Coin_update>();

            try
            {
                con = connect("DBConnectionString");
                
                String selectSTR = "SELECT * FROM Coin_Updates_2022 WHERE coin_name ='"+coin_name+"' AND update_time = (Select max(update_time) FROM Coin_Updates_2022 where coin_name = '"+coin_name+"')";

                SqlCommand cmd = new SqlCommand(selectSTR, con);

                // get a reader
                SqlDataReader dr = cmd.ExecuteReader(CommandBehavior.CloseConnection);


                while (dr.Read())
                {
                    Coin_update c_update = new Coin_update();
                    if (dr["coin_name"] != System.DBNull.Value)
                    {
                        c_update.Coin_name = dr["coin_name"] as string;
                    }

                    c_update.Coin_value = (float)(double)dr["coin_value"];
                    if (dr["update_time"] != System.DBNull.Value)
                    {
                        c_update.Update_date = (DateTime)dr["update_time"];
                    }
                    if (dr["p_change_24h"] != System.DBNull.Value)
                    {
                        c_update.Percent_change_24h = (float)Math.Round((Decimal)((float)(double)dr["p_change_24h"]), 5, MidpointRounding.AwayFromZero);
                    }
                    if (dr["volume_24h"] != System.DBNull.Value)
                    {
                        c_update.Volume_24h = (float)Math.Round((Decimal)((float)(double)dr["volume_24h"]), 5, MidpointRounding.AwayFromZero);
                    }
                    //c_update.Percent_change_1h = (float)(double)dr["p_change_1h"];
                    //c_update.Percent_change_30d = (float)(double)dr["p_change_30d"];
                    //c_update.Percent_change_7d = (float)(double)dr["p_change_7d"];
                    latest_prices.Add(c_update);

                }
                return latest_prices;
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

        //this will return all dates & prices for each coin 
        public List<Coin_update> Get_price_history_per_coin_name(string name, DateTime start, DateTime finish)
        {
            SqlConnection con = null;
            List<Coin_update> latest_prices = new List<Coin_update>();

            try
            {
                con = connect("DBConnectionString");

                String selectSTR = "SELECT coin_name, coin_value, update_time, p_change_24h, volume_24h FROM Coin_Updates_2022 WHERE coin_name = '" + name + "' AND update_time between '"+ start.ToString("yyyy-MM-dd HH:mm:ss.fff") + "' and '"+finish.ToString("yyyy-MM-dd HH:mm:ss.fff") + "' order by update_time desc ";

                SqlCommand cmd = new SqlCommand(selectSTR, con);

                // get a reader
                SqlDataReader dr = cmd.ExecuteReader(CommandBehavior.CloseConnection);


                while (dr.Read())
                {
                    Coin_update c_update = new Coin_update();
                    c_update.Coin_name = (string)dr["coin_name"];
                    c_update.Coin_value = (float)(double)dr["coin_value"];
                    c_update.Update_date = (DateTime)dr["update_time"];
                    if (dr["p_change_24h"] != System.DBNull.Value)
                    {
                        c_update.Percent_change_24h = (float)(double)dr["p_change_24h"];
                    }
                    if (dr["volume_24h"] != System.DBNull.Value)
                    {
                        c_update.Volume_24h = (float)Math.Round((Decimal)((float)(double)dr["volume_24h"]), 5, MidpointRounding.AwayFromZero); 
                    }
                    //c_update.Percent_change_1h = (float)(double)dr["p_change_1h"];
                    //c_update.Percent_change_30d = (float)(double)dr["p_change_30d"];
                    //c_update.Percent_change_7d = (float)(double)dr["p_change_7d"];
                    latest_prices.Add(c_update);

                }
                return latest_prices;
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


        public float Get_latest_price_of_coin(string coin_name)
        {
            SqlConnection con = null;
            List<Coin_update> latest_prices = new List<Coin_update>();

            try
            {
                con = connect("DBConnectionString");

                String selectSTR = "SELECT * FROM Coin_Updates_2022 WHERE coin_name = '"+coin_name+"' AND update_time = (Select max(update_time) FROM Coin_Updates_2022 where coin_name = '"+coin_name+"')";

                SqlCommand cmd = new SqlCommand(selectSTR, con);

                // get a reader
                SqlDataReader dr = cmd.ExecuteReader(CommandBehavior.CloseConnection);
                float latest_price = 0;

                while (dr.Read())
                {
                    latest_price = (float)(double)dr["coin_value"];
                }
                if(coin_name == "Bitcoin")
                {
                    latest_price = (float)Math.Round((Decimal)((float)(double)latest_price), 0, MidpointRounding.AwayFromZero);
                }
                return latest_price;
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

        // this will return each coin with the latest price only - less data
        public List<Coin> Get_Coins_with_latest_price_only()
        {
            SqlConnection con = null;
            List<Coin> coins_list = new List<Coin>();

            try
            {
                con = connect("DBConnectionString");

                String selectSTR = "SELECT * FROM Coins_2022";

                SqlCommand cmd = new SqlCommand(selectSTR, con);

                // get a reader
                SqlDataReader dr = cmd.ExecuteReader(CommandBehavior.CloseConnection);


                while (dr.Read())
                {
                    Coin coin = new Coin();
                    coin.Coin_name = dr["coin_name"] as string;
                    coin.Coin_info = dr["coin_info"] as string;
                    coin.Coin_url = dr["coin_url"] as string;
                    coin.Coin_picture = dr["coin_picture"] as string;
                    if (coin.Coin_name != null)
                    {
                        coin.Price_history = Get_latest_price_per_coin_name(coin.Coin_name);
                    }
                    coins_list.Add(coin);

                }
                return coins_list;
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

        

        public List<Prediction> get_predictions(string coin_name)
        {
            SqlConnection con = null;
            List<Prediction> p_list = new List<Prediction>();

            try
            {
                con = connect("DBConnectionString");
                String selectSTR;
                if (coin_name == null || coin_name == " ")
                {
                    selectSTR = "SELECT * FROM Predictions_2022";
                }
                else selectSTR = "SELECT * FROM Predictions_2022 WHERE coin_name = '" + coin_name + "' order by y_time desc";

                SqlCommand cmd = new SqlCommand(selectSTR, con);

                // get a reader
                SqlDataReader dr = cmd.ExecuteReader(CommandBehavior.CloseConnection);


                while (dr.Read())
                {
                    Prediction p = new Prediction();
                    p.Coin_name = dr["coin_name"] as string;
                    p.Predicted_price = (float)(double)dr["predicted_price"];
                    p.X_time = (DateTime)dr["x_time"];
                    p.Y_time = (DateTime)dr["y_time"];
                    p.X_current_price = (float)(double)dr["x_current_price"];

                    if(dr["y_true_price"] !=  System.DBNull.Value)
                    {
                        p.Y_true_price = (float)(double)dr["y_true_price"];
                    }
                    p_list.Add(p);

                }
                return p_list;
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

        public List<Tweet> get_tweets()
        {
            SqlConnection con = null;
            List<Tweet> t_list = new List<Tweet>();

            try
            {
                con = connect("DBConnectionString");
                String selectSTR;
                
                selectSTR = "SELECT * FROM Cointerest_Tweets_2022 order by tweet_time desc";

                SqlCommand cmd = new SqlCommand(selectSTR, con);

                // get a reader
                SqlDataReader dr = cmd.ExecuteReader(CommandBehavior.CloseConnection);


                while (dr.Read())
                {
                    Tweet t = new Tweet();
                    t.Tweet_id = dr["tweet_id"] as string;
                    t.Author = dr["author"] as string;
                    t.Comp_score = (float)(double)dr["comp_score"];
                    t.Engagement = (int)dr["engagement"];
                    t.Tweet_text = dr["tweet_text"] as string;
                    t.Tweet_time = (DateTime)dr["tweet_time"];
                    
                    t_list.Add(t);

                }
                return t_list;
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

        public List<Value_interval> get_hourly_graph_data(string coin_name, string interval_type, DateTime start, DateTime finish)
        {
            SqlConnection con = null;
            List<Value_interval> v_list = new List<Value_interval>();

            try
            {
                con = connect("DBConnectionString");
                String selectSTR;

                selectSTR = "SELECT ";
                if (interval_type == "H")
                {
                    selectSTR += "dateadd(hour, (datediff(hour, 0, cu.update_time)), 0) Interval ";
                    selectSTR += " ,AVG(cu.coin_value) AvgResult";
                    selectSTR += " FROM Coin_Updates_2022 cu";
                    selectSTR += " where cu.coin_name = '" + coin_name + "' AND cu.update_time > '" + start.ToString("yyyy-MM-dd HH:mm:ss.fff") + "' and cu.update_time < '" + finish.ToString("yyyy-MM-dd HH:mm:ss.fff") + "'";
                    selectSTR += " GROUP BY dateadd(hour, (datediff(hour,0,cu.update_time)), 0)";
                    selectSTR += " order by Interval asc";
                }
                if(interval_type == "D")
                {
                    selectSTR += "dateadd(day, (datediff(day, 0, cu.update_time)), 0) Interval ";
                    selectSTR += " ,AVG(cu.coin_value) AvgResult";
                    selectSTR += " FROM Coin_Updates_2022 cu";
                    selectSTR += " where cu.coin_name = '" + coin_name + "' AND cu.update_time > '" + start.ToString("yyyy-MM-dd HH:mm:ss.fff") + "' and cu.update_time < '" + finish.ToString("yyyy-MM-dd HH:mm:ss.fff") + "'";
                    selectSTR += " GROUP BY dateadd(day, (datediff(day,0,cu.update_time)), 0)";
                    selectSTR += " order by Interval asc";
                }
                
                SqlCommand cmd = new SqlCommand(selectSTR, con);

                // get a reader
                SqlDataReader dr = cmd.ExecuteReader(CommandBehavior.CloseConnection);


                while (dr.Read())
                {
                    Value_interval v = new Value_interval();
                    v.Value = (double)dr["AvgResult"];
                    if(interval_type == "H") v.Name = ((DateTime)dr["Interval"]).ToString("HH:mm");
                    if(interval_type == "D") v.Name = ((DateTime)dr["Interval"]).ToString("dd/MM");


                    v_list.Add(v);

                }
                return v_list;
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

        public List<Graph_value_interval> get_compound(string coin_name, string interval_type, DateTime start, DateTime finish)
        {
            SqlConnection con = null;
            List<Graph_value_interval> v_list = new List<Graph_value_interval>();

            try
            {
                con = connect("DBConnectionString");
                String selectSTR;

                selectSTR = "SELECT ";
                selectSTR += "t1.Interval, t1.AvgResult, t2.avg_comp";
                selectSTR += " from";
                selectSTR += " (SELECT dateadd(day, (datediff(day, 0, cu.update_time)), 0) Interval, AVG(cu.coin_value) AvgResult ";
                selectSTR += "FROM Coin_Updates_2022 cu";
                selectSTR += " where cu.coin_name = '"+coin_name+ "' AND cu.update_time > '" + start.ToString("yyyy-MM-dd HH:mm:ss.fff") + "' and cu.update_time < '" + finish.ToString("yyyy-MM-dd HH:mm:ss.fff") + "'";
                selectSTR += " GROUP BY dateadd(day, (datediff(day, 0, cu.update_time)), 0)) t1";
                selectSTR += " left join";
                selectSTR += " (SELECT dateadd(day, (datediff(day, 0, ct.tweet_time)), 0) Interval, AVG(ct.comp_score) avg_comp";
                selectSTR += " FROM Cointerest_Tweets_2022 ct";
                selectSTR += " where  ct.tweet_time > '" + start.ToString("yyyy-MM-dd HH:mm:ss.fff") + "' and ct.tweet_time < '" + finish.ToString("yyyy-MM-dd HH:mm:ss.fff") + "'";
                selectSTR += " GROUP BY dateadd(day, (datediff(day, 0, ct.tweet_time)), 0)) t2";
                selectSTR += " on(t1.Interval = t2.Interval)";
                SqlCommand cmd = new SqlCommand(selectSTR, con);
                
                
                
                // get a reader
                SqlDataReader dr = cmd.ExecuteReader(CommandBehavior.CloseConnection);


                while (dr.Read())
                {
                    Graph_value_interval v = new Graph_value_interval();
                    v.Value = (double)dr["AvgResult"];
                    
                    v.Name = ((DateTime)dr["Interval"]).ToString("dd/MM");
                    if(dr["avg_comp"] == System.DBNull.Value){
                        v.Comp = 0;
                    }
                    else v.Comp = (double)dr["avg_comp"];

                    v_list.Add(v);

                }
                return v_list;
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



        public List<Login> get_logins(string email)
        {
            SqlConnection con = null;
            List<Login> login_list = new List<Login>();

            try
            {
                con = connect("DBConnectionString");
                String selectSTR;
                if (email == null || email == " ")
                {
                    selectSTR = "SELECT * FROM Logins_2022";
                }
                else selectSTR = "SELECT * FROM Logins_2022 WHERE email = '" + email+"'";

                SqlCommand cmd = new SqlCommand(selectSTR, con);

                // get a reader
                SqlDataReader dr = cmd.ExecuteReader(CommandBehavior.CloseConnection);


                while (dr.Read())
                {
                    Login li = new Login();
                    li.Email = dr["email"] as string;
                    li.Time = (DateTime)dr["time"];
                    login_list.Add(li);

                }
                return login_list;
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

        //  --    Assets    -- //

        //this will return all assets of a certain user
        public List<Asset> get_assets_of_certain_user(string email)
        {

            SqlConnection con = null;
            List<Asset> assets_list = new List<Asset>();

            try
            {
                con = connect("DBConnectionString");

                String selectSTR = "SELECT * FROM Assets_2022 Where email='"+email+"'";

                SqlCommand cmd = new SqlCommand(selectSTR, con);

                // get a reader
                SqlDataReader dr = cmd.ExecuteReader(CommandBehavior.CloseConnection);

                while (dr.Read())
                {
                    Asset ast = new Asset();
                    ast.Coin_name = dr["coin_name"] as string;
                    ast.Email = dr["email"] as string;
                    ast.Amount = (float)(double)dr["amount"];
                    if (ast.Coin_name != "USD")
                    {
                        ast.Asset_worth_in_USD = Get_latest_price_of_coin(ast.Coin_name) * ast.Amount;
                    }
                    else
                    {
                        ast.Asset_worth_in_USD = ast.Amount;
                    }
                    ast.Coin_info = get_coin_info(ast.Coin_name);
                    assets_list.Add(ast);
                }
                return assets_list;
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

        //  --    Transactions    -- //
        public List<Transaction> Get_transactions_of_friends(string email)
        {
            SqlConnection con = null;
            List<Transaction> transac_list = new List<Transaction>();

            try
            {
                con = connect("DBConnectionString");

                String selectSTR = "SELECT T.coin_amount, T.coin_name, T.comment, T.dollar_amount, T.email, T.t_date, u.username, u.userImage, C.coin_picture FROM Transactions_2022 as T inner join Users_2022 as U on T.email = U.email inner join Coins_2022 as C on C.coin_name = T.coin_name ";
                selectSTR +=        " where T.email in (select Users_2022.email from Users_2022 where username in (select Following_2022.username from Following_2022 where email = '" + email + "'))";
                selectSTR +=        " order by T.t_date desc";

                SqlCommand cmd = new SqlCommand(selectSTR, con);

                // get a reader
                SqlDataReader dr = cmd.ExecuteReader(CommandBehavior.CloseConnection);
               

                while (dr.Read())
                {
                    Transaction trnsc = new Transaction();
                    trnsc.Email = dr["email"] as string;
                    trnsc.Coin_name = dr["coin_name"] as string;
                    trnsc.T_date = (DateTime)dr["t_date"];
                    trnsc.Coin_amount = (float)(double)dr["coin_amount"];
                    trnsc.Dollar_amount = (float)(double)dr["dollar_amount"];
                    trnsc.Comment = dr["comment"] as string;
                    
                    
                    trnsc.Coin_pic = dr["coin_picture"] as string;
                    
                    trnsc.Username = dr["username"] as string;
                    trnsc.User_pic = dr["userImage"] as string;
                    trnsc.create_timeAgo_Str();
                    transac_list.Add(trnsc);
                }
                return transac_list;
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

        public List<Transaction> Get_transactions_of_coin(string coin_name) // without specifying who the user is following
        {
            SqlConnection con = null;
            List<Transaction> transac_list = new List<Transaction>();

            try
            {
                con = connect("DBConnectionString");

                String selectSTR = "SELECT T.coin_amount, T.coin_name, T.comment, T.dollar_amount, T.dollar_amount, T.email, T.t_date,U.userImage, U.username, c.coin_picture FROM Transactions_2022 as T inner join Users_2022 as U on T.email = U.email inner join Coins_2022 as C on C.coin_name = t.coin_name";
                selectSTR +=       " where T.coin_name ='"+coin_name+"'";
                selectSTR +=       " order by T.t_date desc";

                SqlCommand cmd = new SqlCommand(selectSTR, con);

                // get a reader
                SqlDataReader dr = cmd.ExecuteReader(CommandBehavior.CloseConnection);


                while (dr.Read())
                {
                    Transaction trnsc = new Transaction();
                    trnsc.Email = dr["email"] as string;
                    trnsc.Coin_name = dr["coin_name"] as string;
                    trnsc.T_date = (DateTime)dr["t_date"];
                    trnsc.Coin_amount = (float)(double)dr["coin_amount"];
                    trnsc.Dollar_amount = (float)(double)dr["dollar_amount"];
                    trnsc.Comment = dr["comment"] as string;
                    

                    trnsc.Coin_pic = dr["coin_picture"] as string; 
                    
                    trnsc.Username = dr["username"] as string; 
                    trnsc.User_pic = dr["userImage"] as string;
                    trnsc.create_timeAgo_Str();
                    transac_list.Add(trnsc);
                }
                return transac_list;
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
        
        public List<Transaction> Get_transactions_of_coin(string email,string coin)
        {

            SqlConnection con = null;
            List<Transaction> transac_list = new List<Transaction>();

            try
            {
                con = connect("DBConnectionString");

                String selectSTR = "select t.email, t.coin_name, t.t_date, t.coin_amount, t.dollar_amount, t.comment, U.userImage, U.username, C.coin_picture from Transactions_2022 as t inner join Users_2022 as U on U.email = t.email inner join Coins_2022 as C on C.coin_name = t.coin_name ";
                selectSTR += "where t.email in (select Users_2022.email from Users_2022 ";
                selectSTR += "where username in ";
                selectSTR += "(select Following_2022.username from Following_2022 ";
                selectSTR += "where email = '"+email+"')) ";
                selectSTR += "and t.coin_name = '"+coin+"' ";
                selectSTR += "order by t.t_date desc";

                SqlCommand cmd = new SqlCommand(selectSTR, con);

                // get a reader
                SqlDataReader dr = cmd.ExecuteReader(CommandBehavior.CloseConnection);

                while (dr.Read())
                {
                    Transaction trnsc = new Transaction();
                    trnsc.Email = dr["email"] as string;
                    trnsc.Coin_name = dr["coin_name"] as string;
                    trnsc.T_date = (DateTime)dr["t_date"];
                    trnsc.Coin_amount = (float)(double)dr["coin_amount"];
                    trnsc.Dollar_amount = (float)(double)dr["dollar_amount"];
                    trnsc.Comment = dr["comment"] as string;
                    trnsc.Coin_pic = dr["coin_picture"] as string;
                    trnsc.Username = dr["username"] as string;
                    trnsc.User_pic = dr["userImage"] as string;
                    trnsc.create_timeAgo_Str();
                    transac_list.Add(trnsc);
                }
                return transac_list;
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


        public List<Transaction> Get_transactions_of_certain_user(string email)
        {
            SqlConnection con = null;
            List<Transaction> transac_list = new List<Transaction>();

            try
            {
                con = connect("DBConnectionString");

                String selectSTR = "SELECT * FROM Transactions_2022";
                selectSTR += " where Transactions_2022.email = '"+email+"'";
                selectSTR += " order by Transactions_2022.t_date desc";

                SqlCommand cmd = new SqlCommand(selectSTR, con);

                // get a reader
                SqlDataReader dr = cmd.ExecuteReader(CommandBehavior.CloseConnection);


                while (dr.Read())
                {
                    Transaction trnsc = new Transaction();
                    trnsc.Email = dr["email"] as string;
                    trnsc.Coin_name = dr["coin_name"] as string;
                    trnsc.T_date = (DateTime)dr["t_date"];
                    trnsc.Coin_amount = (float)(double)dr["coin_amount"];
                    trnsc.Dollar_amount = (float)(double)dr["dollar_amount"];
                    trnsc.Comment = dr["comment"] as string;
                    Coin coin = get_coin_info(trnsc.Coin_name);
                    trnsc.Coin_pic = coin.Coin_picture;
                    User usr = Get_user(trnsc.Email);
                    trnsc.Username = usr.Username;
                    trnsc.User_pic = usr.Image;
                    trnsc.create_timeAgo_Str();
                    transac_list.Add(trnsc);
                }
                return transac_list;
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

        public List<Transaction> get_all_transactions()
        {
            SqlConnection con = null;
            List<Transaction> transac_list = new List<Transaction>();

            try
            {
                con = connect("DBConnectionString");

                String selectSTR = "SELECT * FROM Transactions_2022";
                

                SqlCommand cmd = new SqlCommand(selectSTR, con);

                // get a reader
                SqlDataReader dr = cmd.ExecuteReader(CommandBehavior.CloseConnection);


                while (dr.Read())
                {
                    Transaction trnsc = new Transaction();
                    trnsc.Email = dr["email"] as string;
                    trnsc.Coin_name = dr["coin_name"] as string;
                    trnsc.T_date = (DateTime)dr["t_date"];
                    trnsc.Coin_amount = (float)(double)dr["coin_amount"];
                    trnsc.Dollar_amount = (float)(double)dr["dollar_amount"];
                    trnsc.Comment = dr["comment"] as string;
                    Coin coin = get_coin_info(trnsc.Coin_name);

                    trnsc.Coin_pic = coin.Coin_picture;
                    User usr = Get_user(trnsc.Email);
                    trnsc.Username = usr.Username;
                    trnsc.User_pic = usr.Image;
                    trnsc.create_timeAgo_Str();
                    transac_list.Add(trnsc);
                }
                return transac_list;
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

        public List<Wallet_worth> get_current_wallet_worth()
        {
            SqlConnection con = null;
            List<Wallet_worth> w_list = new List<Wallet_worth>();

            try
            {
                con = connect("DBConnectionString");

                String selectSTR = "select D.email, sum(D.total_price) as total_wallet_worth ";
                selectSTR += "from (";
                selectSTR += "(select A.coin_name, A.email, A.amount, C.coin_value,  (A.amount*c.coin_value) as total_price from Assets_2022 as A inner join (Select top 8 coin_value, coin_name FROM Coin_Updates_2022 ";
                selectSTR += "order by update_time desc) as C on C.coin_name = A.coin_name ) ";
                selectSTR += "union ";
                selectSTR += "(select A1.coin_name, A1.email, A1.amount, C.coin_value,  A1.amount as total_price from Assets_2022 as A1 left join (Select top 8 coin_value, coin_name FROM Coin_Updates_2022 ";
                selectSTR += "order by update_time desc) as C on C.coin_name = A1.coin_name ";
                selectSTR += "where A1.coin_name = 'USD') ";
                selectSTR += " 		) as D ";
                selectSTR += " group by D.email ";


                SqlCommand cmd = new SqlCommand(selectSTR, con);

                // get a reader
                SqlDataReader dr = cmd.ExecuteReader(CommandBehavior.CloseConnection);


                while (dr.Read())
                {
                    Wallet_worth item = new Wallet_worth();
                    item.Email = dr["email"] as string;
                    item.Worth = (float)Math.Round((Decimal)((float)(double)dr["total_wallet_worth"]), 5, MidpointRounding.AwayFromZero);
                    item.Time = DateTime.Now;
                    w_list.Add(item);
                }
                return w_list;
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

        public List<User> get_all_users()
        {
            SqlConnection con = null;
            List<User> U_list = new List<User>();

            try
            {
                con = connect("DBConnectionString");

                String selectSTR = "Select * from Users_2022 ";
                



                SqlCommand cmd = new SqlCommand(selectSTR, con);

                // get a reader
                SqlDataReader dr = cmd.ExecuteReader(CommandBehavior.CloseConnection);


                while (dr.Read())
                {
                    User usr = new User();
                    
                    usr.Email = dr["email"] as string;
                    usr.Username = dr["username"] as string;
                    usr.Image = dr["userImage"] as string;

                    usr.Bio = dr["bio"] as string;
                    U_list.Add(usr);
                }
                return U_list;
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

        public Balance get_portfolio_worth(string email)
        {
            SqlConnection con = null;
            Balance b = new Balance();

            try
            {
                con = connect("DBConnectionString");

                String selectSTR = "select O.email, N.new, O.old ";
                selectSTR +=       "from (select top 1 email, dollar_worth as new from Cointerest_wallet_value_2022 ";
                selectSTR +=       " where email = '"+email+"' ";
                selectSTR +=       " order by time_stamp desc) as N ";
                selectSTR +=       " inner join  (select top 1 email,dollar_worth as old from Cointerest_wallet_value_2022 ";
                selectSTR +=       " where email = '" + email + "' ";
                selectSTR +=       " and time_stamp >= (getdate() - 1) ";
                selectSTR +=       " order by time_stamp asc) as O ";
                selectSTR +=       " 	on N.email = O.email";
                


                SqlCommand cmd = new SqlCommand(selectSTR, con);

                // get a reader
                SqlDataReader dr = cmd.ExecuteReader(CommandBehavior.CloseConnection);


                while (dr.Read())
                {
                    b.Net_worth = (float)(double)dr["new"];
                    float initial = (float)(double)dr["old"];                   
                    b.Weekly_change_percent = ((b.Net_worth - initial) / initial) * 100;
                    b.Weekly_change_percent = (float)Math.Round((Decimal)b.Weekly_change_percent, 2, MidpointRounding.AwayFromZero);
                    
                }
                return b;
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