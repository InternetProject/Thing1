using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PayPal.Api;
using Thing1.Models;

namespace Thing1.Controllers
{
    public class PayPalController : Controller
    {

        private user_managementEntities db = new user_managementEntities();

        Int32 TransactionId = (Int32)(System.DateTime.Now.Ticks + (new Random()).Next(100000));
        // GET: PayPal
        public ActionResult Index()
        {
            return View();
        }




        //public ActionResult PaymentWithPaypal(int? clubId, string name, string currency, string price, string quantity)
        public ActionResult PaymentWithPaypal([Bind(Include = "name,currency,price,quantity")] Item item)
        {
            //Item item = new Item();
            //item.name = name;
            //item.currency = currency;
            //item.price = price;
            //item.quantity = quantity;
            
            var clubMembership = Session["ClubMembership Object"] as ClubMembership;
            var membershipOption = Session["MembershipOption Object"] as MembershipOption;

            //getting the apiContext as earlier
            APIContext apiContext = Configuration.GetAPIContext(clubMembership.ClubId);
            //APIContext apiContext = Configuration.GetAPIContext();

            try
            {
                string payerId = Request.Params["PayerID"];

                if (string.IsNullOrEmpty(payerId))
                {
                    //this section will be executed first because PayerID doesn't exist

                    //it is returned by the create function call of the payment class

                    // Creating a payment

                    // baseURL is the url on which paypal sendsback the data.

                    // So we have provided URL of this controller only

                    string baseURI = Request.Url.Scheme + "://" + Request.Url.Authority + "/PayPal/PaymentWithPayPal?";

                    //guid we are generating for storing the paymentID received in session

                    //after calling the create function and it is used in the payment execution

                    var guid = Convert.ToString((new Random()).Next(100000));

                    //CreatePayment function gives us the payment approval url

                    //on which payer is redirected for paypal acccount payment

                    var createdPayment = this.CreatePayment(apiContext, baseURI + "guid=" + guid, item);

                    //get links returned from paypal in response to Create function call

                    var links = createdPayment.links.GetEnumerator();

                    string paypalRedirectUrl = null;

                    while (links.MoveNext())
                    {
                        Links lnk = links.Current;

                        if (lnk.rel.ToLower().Trim().Equals("approval_url"))
                        {
                            //saving the payapalredirect URL to which user will be redirected for payment
                            paypalRedirectUrl = lnk.href;
                        }
                    }

                    // saving the paymentID in the key guid
                    Session.Add(guid, createdPayment.id);

                    return Redirect(paypalRedirectUrl);
                }
                else
                {
                    // This section is executed when we have received all the payments parameters

                    // from the previous call to the function Create

                    // Executing a payment

                    var guid = Request.Params["guid"];

                    var executedPayment = ExecutePayment(apiContext, payerId, Session[guid] as string);

                    if (executedPayment.state.ToLower() != "approved")
                    {
                        return View("Failure");
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Log("Error" + ex.Message);
                return View("Failure");
            }


            var payment = new payment
            {
                TransactionId = TransactionId,
                AspNetUser = clubMembership.AspNetUser,
                clubID = clubMembership.ClubId,
                userID = clubMembership.UserId,
                amount = membershipOption.Price,//Convert.ToInt32(item.price),
                payment_type = "membership",
                payment_time = System.DateTime.Now
            };

            ClubMembership CM = db.ClubMemberships.Add(clubMembership);
            db.payments.Add(payment);
            db.SaveChanges();
            
            return RedirectToAction("Success", new { clubId = CM.ClubId, clubMembershipId = CM.Id });
            //return View("Success");
        }
        
        public ActionResult Success(int? clubId, int? clubMembershipId)
        {
            // retrieve the database result and show the congraturation message

            Club club = db.Clubs.Find(clubId);
            ClubMembership clubMembership = db.ClubMemberships.Find(clubMembershipId);

            ViewBag.ClubId = club.Id;
            ViewBag.ClubName = club.name;
            ViewBag.ClubNickName = club.nickname;
            ViewBag.WebSite = club.website;
            ViewBag.JoinDate = clubMembership.JoinDate.ToString("d");
            ViewBag.TermDate = clubMembership.TermDate.ToString("d");
            
            return View();            
        }

        private PayPal.Api.Payment payment;

        private Payment ExecutePayment(APIContext apiContext, string payerId, string paymentId)
        {
            var paymentExecution = new PaymentExecution() { payer_id = payerId };
            this.payment = new Payment() { id = paymentId };
            return this.payment.Execute(apiContext, paymentExecution);
        }

        private Payment CreatePayment(APIContext apiContext, string redirectUrl, Item item)
        {

            //similar to credit card create itemlist and add item objects to it
            var itemList = new ItemList() { items = new List<Item>() };

            itemList.items.Add(item);

            var payer = new Payer() { payment_method = "paypal" };

            // Configure Redirect Urls here with RedirectUrls object
            var redirUrls = new RedirectUrls()
            {
                cancel_url = redirectUrl,
                return_url = redirectUrl
            };

            // similar as we did for credit card, do here and create details object
            var details = new Details()
            {
                tax = "0",
                shipping = "0",
                subtotal = item.price
            };

            // similar as we did for credit card, do here and create amount object
            var amount = new Amount()
            {
                currency = "USD",
                total = item.price, // Total must be equal to sum of shipping, tax and subtotal.
                details = details
            };

            var transactionList = new List<Transaction>();

            transactionList.Add(new Transaction()
            {
                description = "Transaction description.",
                invoice_number = TransactionId.ToString(),
                amount = amount,
                item_list = itemList
            });

            this.payment = new Payment()
            {
                intent = "sale",
                payer = payer,
                transactions = transactionList,
                redirect_urls = redirUrls
            };

            // Create a payment using a APIContext
            return this.payment.Create(apiContext);

        }
    }
}
