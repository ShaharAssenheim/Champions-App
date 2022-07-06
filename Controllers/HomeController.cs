using Champions.Context;
using Champions.Models;
using Microsoft.Ajax.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Champions.Controllers
{
    public class HomeController : Controller
    {
        MyContext db = new MyContext();

        public ActionResult Index()
        {
            List<User_Games> UserGamesList = new List<User_Games>();
            List<GameModel> GameList = db.GameDb.OrderBy(x => x.ID).ToList();
            if (Session["FirstName"] != null)
            {
                string User = Session["FirstName"].ToString();
                UserGamesList = db.UserGamesDB.Where(x => x.UserName == User).ToList();
            }
            var tuple = new Tuple<List<User_Games>, List<GameModel>>(UserGamesList, GameList);
            return View(tuple);
        }

        public ActionResult GamesUpdate()
        {
            List<GameModel> GameList = db.GameDb.OrderBy(x => x.ID).ToList();
            return View(GameList);
        }

        public ActionResult GameLive()
        {
            DateTime date = @TimeZoneInfo.ConvertTime(DateTime.Now, TimeZoneInfo.FindSystemTimeZoneById("Israel Standard Time"));
            List<GameModel> GameList = db.GameDb.Where(x => x.Time < date).OrderByDescending(x => x.ID).ToList();
            List<User_Games> UserGamesList = db.UserGamesDB.ToList();
            List<CustomerModel> CustomerList = db.Customers.ToList();
            foreach (var item in UserGamesList)
            {
                item.Img = CustomerList.SingleOrDefault(x => x.UserName == item.UserName).Image;
            }
            var tuple = new Tuple<List<GameModel>, List<User_Games>>(GameList, UserGamesList);
            return View(tuple);
        }

        public ActionResult Tables()
        {
            var TeamList = db.TeamsDB.GroupBy(item => item.Group).Select(group => new { Group = group.Key.Trim(), Items = group.OrderByDescending(x => x.Points).ToList() }).ToList();
            List<Groups> GropsList = new List<Groups>();

            foreach (var item in TeamList)
            {
                Groups g = new Groups();
                g.Name = item.Group;
                g.TeamsList = item.Items;
                GropsList.Add(g);
            }
            List<Players> PlayerList = db.PlayersDB.OrderByDescending(x => x.Goals).ToList();
            var tuple = new Tuple<List<Groups>, List<Players>>(GropsList, PlayerList);
            return View(tuple);
        }

        [HttpPost]
        public JsonResult SaveUserGame(User_Games[] Games)
        {
            var status = false;
            int i = 1;
            foreach (var Game in Games)
            {
                string User = Session["FirstName"].ToString();
                if (db.UserGamesDB.Any(c => c.GameID == i && c.UserName.Equals(User)))
                {
                    User_Games UG = db.UserGamesDB.SingleOrDefault(c => c.GameID == i && c.UserName.Equals(User));
                    if (UG == null)
                        return new JsonResult { Data = new { status } };
                    UG.UserName = Session["FirstName"].ToString();
                    UG.GameID = i;
                    UG.Team1_Bet = Game.Team1_Bet;
                    UG.Team2_Bet = Game.Team2_Bet;
                    db.SaveChanges();
                    i++;
                }
            }
            status = true;
            return new JsonResult { Data = new { status } };
        }

        [HttpPost]
        public ActionResult SaveGameResult(GameModel[] GameRes)
        {
            int i = 1;
            foreach (var Game in GameRes)
            {
                GameModel GM = db.GameDb.SingleOrDefault(c => c.ID == i);
                if (GM == null)
                    return HttpNotFound();
                GM.Team1_Goal = Game.Team1_Goal;
                GM.Team2_Goal = Game.Team2_Goal;
                if (Game.Team1_Goal != -1 && Game.Team2_Goal != -1)
                    UpdateUserScore(i, GM.Team1_Score, GM.Team2_Score, GM.Drow_Score, GM.Team1_Goal, GM.Team2_Goal);
                db.SaveChanges();
                i++;
            }
            return RedirectToAction("Index");
        }

        private void UpdateUserScore(int _id, int _t1_score, int _t2_score, int _drow_score, int _t1_goal, int _t2_goal)
        {
            string WinReal = "";
            string WinBet = "";
            int score = 0;
            foreach (var user in db.UserGamesDB.Where(x => x.GameID == _id))
            {
                if (user.Team1_Bet == -1 || user.Team2_Bet == -1) continue;
                WinReal = _t1_goal > _t2_goal ? "Team1" : _t1_goal == _t2_goal ? "Drow" : "Team2";
                WinBet = user.Team1_Bet > user.Team2_Bet ? "Team1" : user.Team1_Bet == user.Team2_Bet ? "Drow" : "Team2";
                if (user.Team1_Bet == _t1_goal && user.Team2_Bet == _t2_goal)//if it exac match
                    score = WinReal == "Team1" ? _t1_score : WinReal == "Team2" ? _t2_score : _drow_score;
                else if (WinReal == WinBet)//if only match side
                    score = WinReal == "Team1" ? _t1_score / 2 : WinReal == "Team2" ? _t2_score / 2 : _drow_score / 2;
                else
                    score = 0;

                user.Score = score;
            }
        }

        [HttpPost]
        public ActionResult UpdateTables(Teams[] TeamsList)
        {
            foreach (var T in TeamsList)
            {
                Teams Team = db.TeamsDB.SingleOrDefault(c => c.Name.Trim() == T.Name.Trim());
                if (Team == null)
                    return HttpNotFound();
                Team.Points = T.Points;
                db.SaveChanges();
            }
            return RedirectToAction("Tables");
        }
    }
}