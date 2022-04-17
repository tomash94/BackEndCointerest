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
        // Users sections
        //---------------------------------------------------------------------------------

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

        public List<User> Search_users(string username_query_search)
        {
            SqlConnection con = null;

            try
            {
                con = connect("DBConnectionString");

                String selectSTR = "select * from Users_2022 where username like '%"+username_query_search+"%'";

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

            if (obj is Transaction)
            {
                Transaction trans = (Transaction)obj;
                sb.AppendFormat("Values('{0}', '{1}', '{2}', '{3}', '{4}',", trans.Email, trans.Coin_name, trans.Coin_amount, trans.Dollar_amount, trans.Comment);
                String prefix = "INSERT INTO Transactions_2022 " + "([email], [coin_name], [coin_amount], [dollar_amount], [comment], [t_date])";
                command = prefix + sb.ToString() + "convert(datetime, '" + trans.T_date.ToString("yyyy-MM-dd HH:mm:ss.fff") + "'))";
            }

            if (obj is Asset)
            {
                Asset ast = (Asset)obj;
                sb.AppendFormat("Values('{0}', '{1}', '{2}')", ast.Email, ast.Coin_name, ast.Amount);
                String prefix = "INSERT INTO Assets_2022 " + "([email], [coin_name], [amount])";
                command = prefix + sb.ToString();
            }

            if (obj is Prediction)
            {
                Prediction p = (Prediction)obj;
                String prefix;
                if (p.X_current_price == -1)
                {
                    
                    prefix = "UPDATE Predictions_2022 set y_true_price =" +p.Y_true_price+ "where coin_name='"+p.Coin_name+ "' And y_time IN (SELECT max(y_time) FROM Predictions_2022 )";
                    command = prefix;
                }
                else
                {

                    sb.AppendFormat("Values('{0}', '{1}', '{2}',", p.Coin_name, p.Predicted_price, p.X_current_price);
                    prefix = "INSERT INTO Predictions_2022 " + "([coin_name], [predicted_price], [x_current_price], [x_time], [y_time] )";
                    command = prefix + sb.ToString() + "convert(datetime, '" + p.X_time.ToString("yyyy-MM-dd HH:mm:ss.fff") + "')" + ", convert(datetime, '" + p.Y_time.ToString("yyyy-MM-dd HH:mm:ss.fff") + "')) ";
                }
            }

            if(obj is Tweet)
            {
                Tweet t = (Tweet)obj;
                sb.AppendFormat("Values('{0}', '{1}', '{2}', '{3}', '{4}',",t.Tweet_id, t.Author, t.Comp_score, t.Engagement, t.Tweet_text);
                String prefix = "INSERT INTO Cointerest_Tweets_2022 " + "([tweet_id], [author], [comp_score], [engagement], [tweet_text], [tweet_time])";
                command = prefix + sb.ToString() + " convert(datetime, '" + t.Tweet_time.ToString("yyyy-MM-dd HH:mm:ss.fff") + "'))";

            }

            return command;
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
            String prefix = "UPDATE Users_2022 SET userImage = " ;
            command = prefix + sb1.ToString() +" where email="+sb2.ToString();


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

        public int login(string email, DateTime time)
        {

            SqlConnection con = null;

            try
            {

                con = connect("DBConnectionString");

                String command = "";
                StringBuilder sb = new StringBuilder();
                sb.AppendFormat("Values('{0}', ", email);
                String prefix = "INSERT INTO Logins_2022 " + "([email],[time])";
                command = prefix + sb.ToString() + "convert(datetime, '" + time.ToString("yyyy-MM-dd HH:mm:ss.fff") + "'))";

                SqlCommand cmd = new SqlCommand(command, con);

                // get a reader
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

        public int check_follow(string email, string discover_user)
        {

            SqlConnection con = null;

            try
            {
                con = connect("DBConnectionString");

                String selectSTR = "SELECT * FROM Following_2022 WHERE email = '" + email + "' AND username="+discover_user;

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
                    if (dr["coin_name"] != null)
                    {
                        c_update.Coin_name = dr["coin_name"] as string;
                    }
                       c_update.Coin_value = (float)(double)dr["coin_value"];
                    if ((DateTime)dr["update_time"] != null)
                    {
                        c_update.Update_date = (DateTime)dr["update_time"];
                    }
                    if (dr["p_change_24h"] != null)
                    {
                        c_update.Percent_change_24h = (float)(double)dr["p_change_24h"];
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
        public List<Coin_update> Get_price_history_per_coin_name(string name)
        {
            SqlConnection con = null;
            List<Coin_update> latest_prices = new List<Coin_update>();

            try
            {
                con = connect("DBConnectionString");

                String selectSTR = "SELECT coin_name, coin_value, update_time, p_change_24h FROM Coin_Updates_2022 WHERE coin_name = '" + name + "'";

                SqlCommand cmd = new SqlCommand(selectSTR, con);

                // get a reader
                SqlDataReader dr = cmd.ExecuteReader(CommandBehavior.CloseConnection);


                while (dr.Read())
                {
                    Coin_update c_update = new Coin_update();
                    c_update.Coin_name = (string)dr["coin_name"];
                    c_update.Coin_value = (float)(double)dr["coin_value"];
                    c_update.Update_date = (DateTime)dr["update_time"];
                    if (dr["p_change_24h"] != null)
                    {
                        c_update.Percent_change_24h = (float)(double)dr["p_change_24h"];
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

        // this will return each coin with its entire price history - more data
        public List<Coin> Get_Coins_with_all_prices()
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
                        coin.Price_history = Get_price_history_per_coin_name(coin.Coin_name);
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
                else selectSTR = "SELECT * FROM Predictions_2022 WHERE coin_name = '" + coin_name + "'";

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
                
                selectSTR = "SELECT * FROM Cointerest_Tweets_2022";

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
                    ast.Asset_worth_in_USD = Get_latest_price_of_coin(ast.Coin_name) * ast.Amount;
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


        //this will check if a user has a certain asset and insert to it, else will create new one for him
        public int Insert(Asset ast)
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

            String cStr = "IF EXISTS (SELECT 1 FROM Assets_2022 WHERE email = '" + ast.Email + "' AND coin_name='" + ast.Coin_name + "')";      // helper method to build the insert string
            cStr += "BEGIN ";
            cStr += "  UPDATE Assets_2022 ";
            cStr += "  SET amount = amount +" + ast.Amount;
            cStr += "  WHERE email = '" + ast.Email + "' AND coin_name='" + ast.Coin_name + "' ";
            cStr += "END ";
            cStr += "ELSE ";
            cStr += "BEGIN ";
            cStr += BuildInsertCommand(ast);
            cStr += "END";

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

        

        //  --    Transactions    -- //
        public int Insert(Transaction ts)
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

            String cStr = BuildInsertCommand(ts);      // helper method to build the insert string
           

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

        public List<Transaction> Get_transactions_of_friends(string email)
        {
            SqlConnection con = null;
            List<Transaction> transac_list = new List<Transaction>();

            try
            {
                con = connect("DBConnectionString");

                String selectSTR = "SELECT * FROM Transactions_2022";
                selectSTR += " where Transactions_2022.email in (select Users_2022.email from Users_2022 where username in (select Following_2022.username from Following_2022 where email = '" + email + "'))";
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

    }
}