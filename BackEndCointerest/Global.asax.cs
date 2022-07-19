using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.Security;
using System.Web.SessionState;
using System.Web.Http;
using System.Threading.Tasks;
using System.Threading;
using BackEndCointerest.Models;

namespace BackEndCointerest
{
    public class Global : HttpApplication
    {
        
        static Thread get_prices_thread = new Thread(KeepAlive);

        void Application_Start(object sender, EventArgs e)
        {
            // Code that runs on application startup
            AreaRegistration.RegisterAllAreas();
            GlobalConfiguration.Configure(WebApiConfig.Register);
            RouteConfig.RegisterRoutes(RouteTable.Routes);

           

            //initiallize a task to get the latest coin prices every 10 minutes through a task
            get_prices_thread.Start();
        }

        static async void MonitoringTimer()
        {
            //coin prices
            CoinMarketCapLoop callback_coinsLoop = new CoinMarketCapLoop();
            
            Coin_update c_update = new Coin_update();
            List<Coin_update> updates = await callback_coinsLoop.api_func();
            c_update.Insert(updates);


            //wallet worth
            await Update_Wallets_Worth_Table();
        }

        static async Task <int> Update_Wallets_Worth_Table()
        {
            Wallet_worth w = new Wallet_worth();
            return w.insert();
        }

        static void KeepAlive()
        {
            
            while (true)
            {
                try
                {

                    MonitoringTimer();
                    Thread.Sleep(1000*600); // wait 10 minutes


                }
                catch (ThreadAbortException)
                {
                    break;
                }
            }
        }
    }
}