using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using DebateTeamManagementSystem.Models;
using System.Data.Entity.Infrastructure;
using System.Web.Providers.Entities;
using DebateTeamManagementSystem.Scripts;
namespace DebateTeamManagementSystem
{


    public partial class Edit : Page
    {
        public ArrayList teamList = new ArrayList();
        

        protected void Page_Load(object sender, EventArgs e)
        {
           
        }

        protected void TeamText_TextChanged(object sender, EventArgs e)
        {

        }

        public IQueryable<Team> teamsGrid_GetData()
        {
            DebateContext db = new DebateContext();
            var query = db.Teams;
            return query;
        }

        public void teamsGrid_UpdateItem(int TeamID)
        {
            using (DebateContext db = new DebateContext())
            {
                Team item = null;
                item = db.Teams.Find(TeamID);

                //found in the database
                //if (db.Teams.Any(x => x.TeamName == item.TeamName && x.TeamID != item.TeamID))
               // {

                    if (item == null)
                    {
                    ModelState.AddModelError("",
                      String.Format("Item with id {0} was not found", TeamID));
                    return;
                    }

                    TryUpdateModel(item);

                    if (ModelState.IsValid)
                    {
                        db.SaveChanges();
                    }
               // }
                else {
                }
            }
        }

        public void teamsGrid_DeleteItem(int TeamID)
        {
            using (DebateContext db = new DebateContext())
            {
                var item = new Team { TeamID = TeamID };
                db.Entry(item).State = EntityState.Deleted;
                try
                {
                    db.SaveChanges();
                }
                catch (DbUpdateConcurrencyException)
                {
                    ModelState.AddModelError("",
                      String.Format("Item with id {0} no longer exists in the database.", TeamID));
                }
            }
        }

        protected void TextBox1_TextChanged(object sender, EventArgs e)
        {

        }

        protected void Button1_Click(object sender, EventArgs e)
        {
            using (DebateContext db = new DebateContext())
           {
                var teamName = TextBox1.Text;
               
                var teamList = db.Teams.ToList();
                
                Boolean isTeamUnique = isTeamNameUnique(teamName, teamList);
                                

                if (isTeamUnique) {
                var item = new Team { TeamName = teamName };
                
                    if (TextBox1.Text != null && TextBox1.Text != "")
                    {
                        DbSet dbset = db.Set(item.GetType());
                        
                        dbset.Add(item);

                        db.Entry(item).State = EntityState.Added;
                    }

                    if (ModelState.IsValid)
                    {
                        db.SaveChanges();
                    }

                }
                

                Response.Redirect("~/Admin/Edit");
            }
        }

        protected void scheduleGrid_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        public IQueryable<TimeSlot> scheduleGrid_GetData()
        {
            DebateContext db = new DebateContext();
            var query = db.TimeSlots;
            return query;
        }

        public void scheduleGrid_UpdateItem(int TimeSlotID)
        {
            using (DebateContext db = new DebateContext())
            {
                TimeSlot item = null;
                item = db.TimeSlots.Find(TimeSlotID);
                if (item == null)
                {
                    ModelState.AddModelError("",
                      String.Format("Item with id {0} was not found", TimeSlotID));
                    return;
                }

                TryUpdateModel(item);
                if (ModelState.IsValid)
                {
                    db.SaveChanges();
                }
            }
        }

        public void scheduleGrid_DeleteItem(int TimeSlotID)
        {
            using (DebateContext db = new DebateContext())
            {
                var item = new TimeSlot { TimeSlotID = TimeSlotID };
                db.Entry(item).State = EntityState.Deleted;
                try
                {
                    db.SaveChanges();
                }
                catch (DbUpdateConcurrencyException)
                {
                    ModelState.AddModelError("",
                      String.Format("Item with id {0} no longer exists in the database.", TimeSlotID));
                }
            }
        }

        Boolean isTeamNameUnique(String teamName, System.Collections.Generic.List<Team> teamList) {
            Boolean isTeamNameUnique = true;

            foreach (Team currentItem in teamList)
            {
                
                //found a match in the database.
                if (currentItem.TeamName.ToUpper().Equals(teamName.ToUpper()))
                {
                    isTeamNameUnique = false;
                    break;
                }
            }
            return isTeamNameUnique;
        }
                
        protected void GenerateSchedule(object sender, EventArgs e) {
            Scheduler scheduler = new Scheduler();
            DebateContext teamsDB = new DebateContext();

            var teams = teamsDB.Teams.ToArray();
            string[] teamNames = new string[teams.Length];

            for (int i = 0; i < teams.Length; i++) {
                teamNames[i] = teams[i].TeamName;
            }

            scheduler.TeamList = teamNames;

            scheduler.StartDate = StartDate.SelectedDate;

            scheduler.EndDate = EndDate.SelectedDate;

            scheduler.FreeSlots = Int32.Parse(FreeSlots.SelectedValue);


            List<float> hourSlots = new List<float>();

            for (int i = 0; i < HourSlots.Items.Count; i++) {

                if (HourSlots.Items[i].Selected) {
                    hourSlots.Add(float.Parse(HourSlots.Items[i].Value));
                }
            }

            scheduler.HourSlots = hourSlots.ToArray();

            scheduler.CreateSchedule();
            
            //checks to see if the scheduler can perform a generation
            if (!scheduler.IsGood) {
                return;
            }

            DebateContext scheduleDB = new DebateContext();

            //for (int i = scheduleDB.TimeSlots.ToList().Count - 1; i >= 0; i--)
            //{
            
            //    scheduleDB.TimeSlots.ToList().RemoveAt(i);
            //    scheduleDB.SaveChanges();
            //}
            scheduleDB.Database.ExecuteSqlCommand("delete from TimeSlots");
            
            TimeSlot TimeSlotToEnter = new TimeSlot();
            DbSet dbset = scheduleDB.Set(TimeSlotToEnter.GetType());
            foreach (Util.TimeSlot item in scheduler.TimeSlots) {
                
                TimeSlotToEnter = new TimeSlot();
                TimeSlotToEnter.Team1Name = item.team1Name;
                TimeSlotToEnter.Team2Name = item.team2Name;
                TimeSlotToEnter.Team1Score = item.team1Score;
                TimeSlotToEnter.Team2Score = item.team2Score;
                TimeSlotToEnter.date = item.date;
                TimeSlotToEnter.time = item.time;


                //TimeSlotToEnter.Team1Name = "ABC";
                //TimeSlotToEnter.Team2Name = "ABC";
                //TimeSlotToEnter.Team1Score = 0;
                //TimeSlotToEnter.Team2Score = 0;
                //TimeSlotToEnter.date = "ABC";
                //TimeSlotToEnter.time = "ABC";


                dbset.Add(TimeSlotToEnter);
                // scheduleDB.Entry(TimeSlotToEnter).State = EntityState.Added;

                scheduleDB.SaveChanges();

            }

            Response.Redirect("~/Admin/Edit");
                        
        }
    }
}