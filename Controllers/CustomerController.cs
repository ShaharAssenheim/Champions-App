using Champions.Context;
using Champions.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using System.IO;
using System.Net;
using System.Web.Security;

namespace Champions.Controllers
{
    public class CustomerController : Controller
    {
        MyContext db = new MyContext();

        // GET: Customer
        public ActionResult CustomersList()
        {
            List<CustomerModel> list = db.Customers.ToList();
            foreach (var item in list)
            {
                item.WinPlayerGoals = db.PlayersDB.SingleOrDefault(c => c.Name == item.WinPlayer).Goals;
            }
            var tuple = new Tuple<List<CustomerModel>, List<User_Games>>(list, db.UserGamesDB.ToList());
            return View(tuple);
        }

        public ActionResult CustomersPage(int? id)
        {
            int SessionId = Convert.ToInt32(Session["UserID"]);
            id = SessionId == id || id == null ? SessionId : id;
            CustomerModel customer = db.Customers.SingleOrDefault(c => c.ID == id);
            if (customer == null)
                return HttpNotFound();
            List<GameModel> GamesList = new List<GameModel>();
            int goals = db.PlayersDB.SingleOrDefault(c => c.Name == customer.WinPlayer).Goals * 2;
            customer.WinPlayerGoals = goals;
            var tuple = new Tuple<CustomerModel, List<GameModel>>(customer, GamesList);
            return View(tuple);
        }


        public ActionResult Delete(int id)
        {
            CustomerModel customer = db.Customers.SingleOrDefault(c => c.ID == id);
            if (customer == null)
                return HttpNotFound();
            if (!(customer.Image == "http://ssl.gstatic.com/accounts/ui/avatar_2x.png"))
            {
                var myAccount = new Account { ApiKey = "555682285552641", ApiSecret = "Id-vLH2JZBKc7x0wK3ZEZYCsGkA", Cloud = "dmrx96yqx" };
                Cloudinary _cloudinary = new Cloudinary(myAccount);
                int pos = customer.Image.LastIndexOf("Champions/");
                string delImg = customer.Image.Substring(pos, customer.Image.Length - pos - 4);
                _cloudinary.DeleteResources(delImg);
            }

            db.Customers.Remove(customer);
            db.UserGamesDB.RemoveRange(db.UserGamesDB.Where(x => x.UserName == customer.UserName));
            db.SaveChanges();
            return RedirectToAction("CustomersList", "Customer");

        }

        [HttpPost]
        public ActionResult Create(CustomerModel customer)
        {
            if (db.Customers.Any(x => x.UserName == customer.UserName))
            {
                string Message = "שם משתמש קיים במערכת";
                return RedirectToAction("Register", "Account", new { msg = Message });
            }
            customer.Image = customer.Image ?? "http://ssl.gstatic.com/accounts/ui/avatar_2x.png";
            customer.AdminCode = 0;
            db.Customers.Add(customer);
            db.SaveChanges();
            int n = db.GameDb.Count();
            for (int i = 1; i <= n; i++)
            {
                User_Games u = new User_Games();
                u.UserName = customer.UserName;
                u.GameID = i;
                u.Team1_Bet = -1;
                u.Team2_Bet = -1;
                u.Score = 0;
                db.UserGamesDB.Add(u);
            }
            db.SaveChanges();
            Session["UserID"] = customer.ID.ToString();
            Session["FirstName"] = customer.UserName.ToString();
            return RedirectToAction("CustomersPage");
        }

        [HttpPost]
        public ActionResult Edit(CustomerModel customer)
        {
            if (ModelState.IsValid)
            {
                CustomerModel user = db.Customers.FirstOrDefault(u => u.ID.Equals(customer.ID));
                // Update fields
                user.UserName = customer.UserName ?? user.UserName;
                user.Password = customer.Password ?? user.Password;
                user.Email = customer.Email ?? user.Email;
                user.Image = customer.Image ?? user.Image;
                db.SaveChanges();
                return RedirectToAction("CustomersPage", customer);
            }
            return View(customer);
        }


        [HttpPost]
        [Obsolete]
        public ActionResult UploadImage()
        {
            //save file in App_Data
            var res = "";
            var file = Request.Files[0];
            var fileName = Path.GetFileName(file.FileName);
            var path = Path.Combine(Server.MapPath("~/Content/imgs/"), fileName);
            file.SaveAs(path);

            //connect to cloudinary account
            var myAccount = new Account { ApiKey = "555682285552641", ApiSecret = "Id-vLH2JZBKc7x0wK3ZEZYCsGkA", Cloud = "dmrx96yqx" };
            Cloudinary _cloudinary = new Cloudinary(myAccount);

            int id = Convert.ToInt32(Session["UserID"]);
            if (id != 0)//if the user is connected, update image
            {
                CustomerModel user = db.Customers.FirstOrDefault(u => u.ID.Equals(id));
                int pos = user.Image.LastIndexOf('/') + 1;
                string delImg = user.Image.Substring(pos, user.Image.Length - pos - 4);
                var uploadParams = new ImageUploadParams()
                {
                    File = new FileDescription(path),
                    Folder = "Champions",
                    PublicId = delImg,

                };
                var uploadResult = _cloudinary.Upload(uploadParams);
                res = uploadResult.SecureUri.ToString();
                user.Image = res;
                db.SaveChanges();
            }
            else//if the user is register, upload new image
            {
                var uploadParams = new ImageUploadParams()
                {
                    File = new FileDescription(path),
                    Folder = "Champions",
                };
                var uploadResult = _cloudinary.Upload(uploadParams);
                res = uploadResult.SecureUri.ToString();
            }
            //delete the image from App_Data
            FileInfo del = new FileInfo(path);
            del.Delete();
            //send back url.
            Response.StatusCode = (int)HttpStatusCode.OK;
            return Json(new { success = true, responseText = res }, JsonRequestBehavior.AllowGet);
        }
    }
}