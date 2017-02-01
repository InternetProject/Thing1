using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using PayPal.Api;

namespace Thing1.Models
{
    public static class Configuration
    {
        public static string ClientId;
        public static string ClientSecret;
        private static user_managementEntities db = new user_managementEntities();

        static Configuration()
        {
            //var config = GetConfig();
            //ClientId = config["clientId"];
            //ClientSecret = config["clientSecret"];
        }

        /*
        public static Dictionary<string, string> GetConfig()
        {
            return PayPal.Api.ConfigManager.Instance.GetProperties();
        }
        */

        public static Dictionary<string, string> GetConfig(int? clubId)
        {
            // default setup (if this club does not have a PayPal account) 
            Dictionary<string, string> dictionary = PayPal.Api.ConfigManager.Instance.GetProperties();
            ClientId = dictionary["clientId"];
            ClientSecret = dictionary["clientSecret"];

            // assign club's specific paypal account here
            List<PayPalAccount> list = db.PayPalAccounts.Where(c => c.ClubId == clubId).ToList();
           

            foreach (PayPalAccount item in list)
            {
                ClientId = item.PayPalClientId;
                dictionary["clientId"] = item.PayPalClientId;
                ClientSecret = item.PayPalClientSecret;
                dictionary["clientSecret"] = item.PayPalClientSecret;
                return dictionary;
            }

            // if error occurs, just return the default setup
            return PayPal.Api.ConfigManager.Instance.GetProperties();

        }
    
        // Create accessToken
        private static string GetAccessToken(int? clubId)
        {
            // ###AccessToken
            // Retrieve the access token from
            // OAuthTokenCredential by passing in
            // ClientID and ClientSecret
            // It is not mandatory to generate Access Token on a per call basis.
            // Typically the access token can be generated once and
            // reused within the expiry window          

            var config = GetConfig(clubId);
            ClientId = config["clientId"];
            ClientSecret = config["clientSecret"];

            //string accessToken = new OAuthTokenCredential(ClientId, ClientSecret, GetConfig()).GetAccessToken();
            string accessToken = new OAuthTokenCredential(ClientId, ClientSecret, config).GetAccessToken();

            return accessToken;
        }

        // Returns APIContext object
        public static APIContext GetAPIContext(int? clubId)
        {
            // ### Api Context
            // Pass in a `APIContext` object to authenticate 
            // the call and to send a unique request id 
            // (that ensures idempotency). The SDK generates
            // a request id if you do not pass one explicitly. 
            APIContext apiContext = new APIContext(GetAccessToken(clubId));
            apiContext.Config = GetConfig(clubId);

            // Use this variant if you want to pass in a request id  
            // that is meaningful in your application, ideally 
            // a order id.
            // String requestId = Long.toString(System.nanoTime();
            // APIContext apiContext = new APIContext(GetAccessToken(), requestId ));

            return apiContext;
        }
    }
}