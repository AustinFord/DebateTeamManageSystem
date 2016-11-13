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


    public partial class RefereeEdit : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
           
            ScheduleError.Visible = false;
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

                if (!item.isLocked)
                {

                    TryUpdateModel(item);
                    if (ModelState.IsValid)
                    {
                        db.SaveChanges();
                    }
                    if (item.RoundStatus == null || item.RoundStatus == "")
                    {

                        item.RoundStatus = "Completed";
                        item.isLocked = true;
                        db.SaveChanges();
                    }
                    CalculateTeamScore(item.Team1Name);
                    CalculateTeamScore(item.Team2Name);
                }
                else
                {
                    ScheduleErrorText.Text = "You cannot edit a locked row";
                    ScheduleError.Visible = true;
                }

            }
        }

        //public void scheduleGrid_DeleteItem(int TimeSlotID)
        //{
        //    using (DebateContext db = new DebateContext())
        //    {
        //        var item = db.TimeSlots.Find(TimeSlotID);

        //        if (!item.isLocked)
        //        {
        //            ScheduleError.Visible = false;
        //            db.Entry(item).State = EntityState.Deleted;
        //            try
        //            {
        //                db.SaveChanges();
        //            }
        //            catch (DbUpdateConcurrencyException)
        //            {
        //                ModelState.AddModelError("",
        //                  String.Format("Item with id {0} no longer exists in the database.", TimeSlotID));
        //            }
        //        }
        //        else
        //        {
        //            ScheduleErrorText.Text = "Cannot delete a row that is locked. Please unlock the row before deleting";
        //            ScheduleError.Visible = true;
        //        }

        //    }


        //}

        protected void CalculateTeamScore(string teamName)
        {

            DebateContext db = new DebateContext();
            int tempScore = 0;
            foreach (TimeSlot item in db.TimeSlots.ToList())
            {
                if (item.RoundStatus == null || item.RoundStatus.Equals("Completed") || item.RoundStatus.Equals(""))
                {
                    if (item.Team1Name.Equals(teamName))
                    {
                        tempScore += item.Team1Score;
                    }
                    else if (item.Team2Name.Equals(teamName))
                    {
                        tempScore += item.Team2Score;
                    }
                }
            }

            int teamID = 0;

            foreach (Team team in db.Teams.ToList())
            {
                if (team.TeamName.Equals(teamName))
                {
                    teamID = team.TeamID;
                    break;
                }
            }

            Team itemToChange = db.Teams.Find(teamID);
            if (itemToChange != null)
            {

                itemToChange.Score = tempScore;

            }
            db.SaveChanges();
        }

        protected void scheduleGrid_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
        
    }
}