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
                ScheduleError.Visible = false;
                int originalTeam1Score = item.Team1Score;
                int originalTeam2Score = item.Team2Score;
                if (item == null)
                {
                    ModelState.AddModelError("",
                      String.Format("Item with id {0} was not found", TimeSlotID));
                    return;
                }

                if (!item.isLocked)
                {

                    TryUpdateModel(item);

                    if (item.Team1Score < 0 || item.Team2Score < 0)
                    {
                        item.Team1Score = originalTeam1Score;
                        item.Team2Score = originalTeam2Score;
                        ScheduleErrorText.Text = "Scores must be between 0 and 100 inclusive";
                        ScheduleError.Visible = true;
                        db.SaveChanges();
                    }
                    else if (item.Team1Score > 100 || item.Team2Score > 100)
                    {
                        item.Team1Score = originalTeam1Score;
                        item.Team2Score = originalTeam2Score;
                        ScheduleErrorText.Text = "Scores must be between 0 and 100 inclusive";
                        ScheduleError.Visible = true;
                        db.SaveChanges();
                    }
                    else {
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

        protected void CalculateTeamScore(string teamName)
        {

            DebateContext db = new DebateContext();
            int tempScore = 0;
            Team team = findTeamByName(teamName);
            foreach (TimeSlot item in db.TimeSlots.ToList())
            {
                if ((item.RoundStatus == null || item.RoundStatus.Equals("Completed") || item.RoundStatus.Equals("") || item.RoundStatus.Contains("Reactivated")))
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

            foreach (Team item in db.Teams.ToList())
            {
                if (item.TeamName.Equals(teamName))
                {
                    teamID = item.TeamID;
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

        protected Team findTeamByName(String name)
        {
            DebateContext db = new DebateContext();
            Team team = null;

            foreach (Team item in db.Teams.ToList())
            {
                if (item.TeamName == name)
                {
                    team = item;
                    break;
                }
            }

            return team;
        }

    }
}