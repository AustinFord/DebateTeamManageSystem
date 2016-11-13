using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using DebateTeamManagementSystem.Models;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;

namespace DebateTeamManagementSystem
{
    public partial class _Default : Page
    {
        protected void Page_Load(object sender, EventArgs e)
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
            }
        }

        public IQueryable<TimeSlot> scheduleGrid_GetData()
        {
            DebateContext db = new DebateContext();
            List<TimeSlot> displayList = new List<TimeSlot>();
            
            foreach (TimeSlot item in db.TimeSlots.ToList()) {

                if (!item.Team1Name.Equals("FREE") && !item.Team2Name.Equals("FREE")) {
                    displayList.Add(item);
                }
            }
            IQueryable<TimeSlot> query = displayList.AsQueryable();
            return query;
            
        }
    }
}