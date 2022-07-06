using Champions.Context;
using Champions.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace Champions.Controllers
{
    public class AccountController : Controller
    {
        MyContext db = new MyContext();

        public ActionResult Login()
        {
            return View();
        }

        public ActionResult Register(string msg)
        {
            List<Players> PlayersList = db.PlayersDB.OrderBy(x => x.ID).ToList();
            List<Teams> TeamsList = db.TeamsDB.OrderBy(x => x.ID).ToList();
            var tuple = new Tuple<List<Teams>, List<Players>>(TeamsList, PlayersList);
            ViewBag.Message = msg;
            return View(tuple);
        }

        public ActionResult ForgotPassword()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(CustomerModel objUser)
        {
            if (ModelState.IsValid)
            {
                using (MyContext db = new MyContext())
                {
                    var obj = db.Customers.Where(a => a.UserName.Equals(objUser.UserName) && a.Password.Equals(objUser.Password)).FirstOrDefault();
                    if (obj != null)
                    {
                        Session["UserID"] = obj.ID.ToString();
                        Session["FirstName"] = obj.UserName.ToString();
                        if (obj.AdminCode == 17342)
                            Session["AdimCode"] = "Admin";
                        else
                            Session["AdimCode"] = "User";
                        return RedirectToAction("Index", "Home");
                    }
                }
            }
            return View(objUser);
        }

        public ActionResult Logout()
        {
            int id = Convert.ToInt32(Session["UserID"]);
            MyContext db = new MyContext();
            var obj = db.Customers.Where(a => a.ID.Equals(id)).FirstOrDefault();


            db.SaveChanges();
            FormsAuthentication.SignOut();
            Session.Abandon(); // it will clear the session at the end of request
            return RedirectToAction("Index", "Home");
        }


        [HttpPost]
        public ActionResult ForgotPassword(string EmailID)
        {
            string resetCode = Guid.NewGuid().ToString();
            var verifyUrl = "/Account/ResetPassword/" + resetCode;
            var link = Request.Url.AbsoluteUri.Replace(Request.Url.PathAndQuery, verifyUrl);
            using (var context = new MyContext())
            {
                var getUser = (from s in context.Customers where s.Email == EmailID select s).FirstOrDefault();
                if (getUser != null)
                {
                    getUser.ResetPasswordCode = resetCode;

                    //This line I have added here to avoid confirm password not match issue , as we had added a confirm password property 

                    context.Configuration.ValidateOnSaveEnabled = false;
                    context.SaveChanges();

                    var subject = "בקשה לאיפוס סיסמא";
                    var body = "Hi " + getUser.UserName + ", <br/> You recently requested to reset your password for your account. Click the link below to reset it. " +

                         " <br/><br/><a href='" + link + "'>" + link + "</a> <br/><br/>" +
                         "If you did not request a password reset, please ignore this email or reply to let us know.<br/><br/> Thank you";



                    SendEmail(getUser.Email, body, subject);

                    ViewBag.Message = "לינק לאיפוס הסיסמא נשלח לכתובת המייל שלך";
                }
                else
                {
                    ViewBag.Message = "משתמש לא קיים במערכת";
                    return View();
                }
            }

            return View();
        }

        private void SendEmail(string emailAddress, string body, string subject)
        {
            using (MailMessage mm = new MailMessage("shacharassen3667@gmail.com", emailAddress))
            {
                mm.Subject = subject;
                mm.Body = body;

                mm.IsBodyHtml = true;
                SmtpClient smtp = new SmtpClient
                {
                    Host = "smtp.gmail.com",
                    EnableSsl = true
                };
                NetworkCredential NetworkCred = new NetworkCredential("shacharassen3667@gmail.com", "$lichi2017");
                smtp.UseDefaultCredentials = true;
                smtp.Credentials = NetworkCred;
                smtp.Port = 587;
                smtp.Send(mm);

            }
        }

        public ActionResult ResetPassword(string id)
        {
            //Verify the reset password link
            //Find account associated with this link
            //redirect to reset password page
            if (string.IsNullOrWhiteSpace(id))
            {
                return HttpNotFound();
            }

            using (var context = new MyContext())
            {
                var user = context.Customers.Where(a => a.ResetPasswordCode == id).FirstOrDefault();
                if (user != null)
                {
                    ResetPasswordModel model = new ResetPasswordModel
                    {
                        ResetCode = id
                    };
                    return View(model);
                }
                else
                {
                    return HttpNotFound();
                }
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ResetPassword(ResetPasswordModel model)
        {
            var message = "";

            if (ModelState.IsValid)
            {
                using (var context = new MyContext())
                {
                    var user = context.Customers.Where(a => a.ResetPasswordCode == model.ResetCode).FirstOrDefault();
                    if (user != null)
                    {
                        //you can encrypt password here, we are not doing it
                        user.Password = model.NewPassword;
                        //make resetpasswordcode empty string now
                        user.ResetPasswordCode = "";
                        //to avoid validation issues, disable it
                        context.Configuration.ValidateOnSaveEnabled = false;
                        context.SaveChanges();
                        message = "סיסמא חדשה עודכנה בהצלחה.";
                        Session["UserID"] = user.ID.ToString();
                        Session["FirstName"] = user.UserName.ToString();
                    }
                }
            }
            else
            {
                message = "משהו השתבש.";
            }
            ViewBag.Message = message;
            //return View(model);
            return RedirectToAction("index", "Home");
        }
    }
}