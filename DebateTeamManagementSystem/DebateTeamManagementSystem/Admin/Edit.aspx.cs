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
using IntroSWEConsoleApp;
namespace DebateTeamManagementSystem
{


    public partial class Edit : Page
    {
        public ArrayList teamList = new ArrayList();
        private DateTime prevStartDate;
        private DateTime prevEndDate;
        
       
        protected void Page_Load(object sender, EventArgs e)
        {

            if (!IsPostBack && Session["IsAlreadyLoad"] == null)
            {
                StartDate.SelectedDate = DateTime.Today.AddDays(((int)DayOfWeek.Saturday - (int)DateTime.Today.DayOfWeek + 7) % 7);
                prevStartDate = StartDate.SelectedDate;


                EndDate.SelectedDate = StartDate.SelectedDate.AddDays(7);
                prevEndDate = EndDate.SelectedDate;
                Session["IsAlreadyLoad"] = true;
                
            }
            if (!validNumberOfTeams())
            {
                TeamErrorText.Text = "You must have less than 10 but more than 1 team to schedule a season.";
                TeamError.Visible = true;
            }
            else {
                TeamError.Visible = false;
            }
            
                    
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
                String itemTeamName;
                int itemID;
                item = db.Teams.Find(TeamID);
                itemTeamName = item.TeamName;
                itemID = item.TeamID;
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

                if (isTeamNameUnique(item.TeamName, db.Teams.ToList(), itemID))
                {
                    if (ModelState.IsValid)
                    {
                        db.SaveChanges();
                        TeamError.Visible = false;
                    }
                } else {

                    item.TeamName = itemTeamName;
                    TeamErrorText.Text = "That team name is not unique. Please choose a new one";
                    TeamError.Visible = true;
                    db.SaveChanges();
                    //then we should display an error saying the team name isnt unique.
                }

            }
        }

        public void teamsGrid_DeleteItem(int TeamID)
        {
            using (DebateContext db = new DebateContext())
            {
                var item = new Team { TeamID = TeamID };
                db.Entry(item).State = EntityState.Deleted;
                int currentPageCount = teamsGrid.PageCount;

                try
                {
                    db.SaveChanges();
                }
                catch (DbUpdateConcurrencyException)
                {
                    ModelState.AddModelError("",
                      String.Format("Item with id {0} no longer exists in the database.", TeamID));
                }
                
                    Response.Redirect("~/Admin/Edit");
                                
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


                if (isTeamUnique)
                {
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
                else {
                    TeamErrorText.Text = "That team name is not unique. Please choose a new one";
                    TeamError.Visible = true;
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

        Boolean isTeamNameUnique(String teamName, List<Team> teamList) {
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

        Boolean isTeamNameUnique(String teamName, List<Team> teamList, int teamID)
        {
            Boolean isTeamNameUnique = true;

            foreach (Team currentItem in teamList)
            {

                //found a match in the database.
                if ((currentItem.TeamName.ToUpper().Equals(teamName.ToUpper())) && teamID != currentItem.TeamID)
                {
                    isTeamNameUnique = false;
              
                    break;
                }
            }
            return isTeamNameUnique;
        }

        protected void GenerateSchedule(object sender, EventArgs e) {
     
            DebateContext teamsDB = new DebateContext();

            var teams = teamsDB.Teams.ToArray();

            string[] teamNames = new string[teams.Length];

            for (int i = 0; i < teams.Length; i++) {
                teamNames[i] = teams[i].TeamName;
            }
                  

            if (!isDateGood()) {
                InvalidDateText.Text = "Selected Dates are not valid.";
                DateErrorMessage.Visible = true;
                return; 
            }

            if (!validNumberOfTeams()) {
                //send out an error to the screen
                return;
            }

            
            
            Util.CreateSchedule(teamNames, StartDate.SelectedDate, EndDate.SelectedDate);

            Util.TimeSlot[] TimeSlotArray = Util.timeSlots;
           

            DebateContext scheduleDB = new DebateContext();

           
                
            foreach(TimeSlot item in scheduleDB.TimeSlots.ToList()) {

                if (!item.isLocked) {
                    scheduleDB.TimeSlots.Remove(item);
                    scheduleDB.SaveChanges();
                }
            }
            //scheduleDB.Database.ExecuteSqlCommand("delete from TimeSlots");

            TimeSlot TimeSlotToEnter = new TimeSlot();
            DbSet dbset = scheduleDB.Set(TimeSlotToEnter.GetType());

            foreach (Util.TimeSlot item in Util.timeSlots) {

                TimeSlotToEnter = new TimeSlot();

                if (item.team1Name == null){
                    TimeSlotToEnter.Team1Name = "FREE";
                }
                else {
                    TimeSlotToEnter.Team1Name = item.team1Name;
                }

                if (item.team2Name ==null) {

                    TimeSlotToEnter.Team2Name = "FREE";
                }
                else {

                    TimeSlotToEnter.Team2Name = item.team2Name;
                }

                TimeSlotToEnter.Team1Score = item.team1Score;
                TimeSlotToEnter.Team2Score = item.team2Score;
                TimeSlotToEnter.date = item.date;
                TimeSlotToEnter.isMorning = item.morning;
               
        
                dbset.Add(TimeSlotToEnter);
                

                scheduleDB.SaveChanges();

            }

            Response.Redirect("~/Admin/Edit");

        }

        protected void StartDate_SelectionChanged(object sender, EventArgs e)
        {
            if (StartDate.SelectedDate.DayOfWeek != DayOfWeek.Saturday || StartDate.SelectedDate < DateTime.Today)
            {
                StartDate.SelectedDate = prevStartDate;
                InvalidDateText.Text = "The start date must be on a saturday and at least today or in the future.";
                DateErrorMessage.Visible = true;
               

            }
            else {
                prevStartDate = StartDate.SelectedDate;
             
                DateErrorMessage.Visible = false;
            }
        }

        protected void EndDate_SelectionChanged(object sender, EventArgs e)
        {
            if (EndDate.SelectedDate.DayOfWeek != DayOfWeek.Saturday || EndDate.SelectedDate < DateTime.Today)
            {
                EndDate.SelectedDate = prevEndDate;
                InvalidDateText.Text = "The end date must be on a saturday and at least today or in the future.";
                DateErrorMessage.Visible = true;
               
            }
            else
            {
                prevEndDate = EndDate.SelectedDate;
                DateErrorMessage.Visible = false;
               
            }
        }

        protected bool isDateGood() {

            bool isDateGood = true;

            if (StartDate.SelectedDate.CompareTo(EndDate.SelectedDate) > 0 || !StartDate.SelectedDate.DayOfWeek.Equals(DayOfWeek.Saturday) || !EndDate.SelectedDate.DayOfWeek.Equals(DayOfWeek.Saturday) ) {
                isDateGood = false;
                InvalidDateText.Text = "Date is not good!";
                DateErrorMessage.Visible = true;
            }

            return isDateGood;
        }

        protected bool validNumberOfTeams() {
            bool numberOfTeamsValid = true;
            DebateContext teamsDB = new DebateContext();

            var teams = teamsDB.Teams.ToArray();

            if (teams.Length < 2 || teams.Length > 10) {
                numberOfTeamsValid = false;
                TeamErrorText.Text = "You must have less than 10 but more than 1 team to schedule a season.";
                TeamError.Visible = true;
                return numberOfTeamsValid;
            }

            
            return numberOfTeamsValid;
        }

        protected bool isTimeSlotUnique(TimeSlot possibleTimeslot) {
            bool timeSlotIsUnique = true;
            DebateContext timeslotDB = new DebateContext();
            foreach (TimeSlot item in timeslotDB.TimeSlots.ToList()) {

                if (item.isLocked && item.Team1Name.Equals(possibleTimeslot.Team1Name) && item.Team2Name.Equals(possibleTimeslot.Team2Name) && item.date.Equals(possibleTimeslot.date)) {
                    timeSlotIsUnique = false;
                    break;
                }
            }
            return timeSlotIsUnique;
        }
    }
}