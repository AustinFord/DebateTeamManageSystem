using System;
using System.Collections;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using DebateTeamManagementSystem.Models;
using System.Data.Entity.Infrastructure;
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
                if (currentItem.TeamName.Equals(teamName))
                {
                    isTeamNameUnique = false;
                    break;
                }
            }
            return isTeamNameUnique;
        }
    }
}