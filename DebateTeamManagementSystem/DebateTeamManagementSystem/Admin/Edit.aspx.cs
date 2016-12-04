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
        private DateTime prevStartDate;
        private DateTime prevEndDate;


        protected void Page_Load(object sender, EventArgs e)
        {

            DebateContext db = new DebateContext();
            //dont display the options for the schedule unless a schedule exists.
            if (!(db.TimeSlots.Count() == 0))
            {
                FinalizeButton.Visible = true;
                AddAnotherWeek.Visible = true;
                TimeSlotsDropDown.Visible = true;
                DeleteSchedule.Visible = true;
            }
            else {
                FinalizeButton.Visible = false;
                AddAnotherWeek.Visible = false;
                DeleteSchedule.Visible = false;
                TimeSlotsDropDown.Visible = false;
            }
            //if (!IsPostBack && Session["IsAlreadyLoad"] == null)
            //{
            //    StartDate.SelectedDate = DateTime.Today.AddDays(((int)DayOfWeek.Saturday - (int)DateTime.Today.DayOfWeek + 7) % 7);
            //    prevStartDate = StartDate.SelectedDate;


            //    EndDate.SelectedDate = StartDate.SelectedDate.AddDays(7);
            //    prevEndDate = EndDate.SelectedDate;
            //    Session["IsAlreadyLoad"] = true;

            //}

            if (StartDate.SelectedDate < DateTime.Now) {
                StartDate.SelectedDate = DateTime.Today.AddDays(((int)DayOfWeek.Saturday - (int)DateTime.Today.DayOfWeek + 7) % 7);
                prevStartDate = StartDate.SelectedDate;

                
               
            }
            if (EndDate.SelectedDate < DateTime.Now) {
                EndDate.SelectedDate = StartDate.SelectedDate.AddDays(7);
                prevEndDate = EndDate.SelectedDate;
            }
            if (!validNumberOfTeams())
            {
                TeamErrorText.Text = "You must have less than 10 but more than 1 team to schedule a season.";
                TeamError.Visible = true;
            }
            else
            {
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
                bool originalActivity = item.isActive;
                //need to add a check to make sure the updated team name is actually in the team list.
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
                        if (!item.isActive)
                        {
                            deleteTeamFromSchedule(TeamID);
                            db.SaveChanges();
                            Response.Redirect("~/Admin/Edit");
                        }
                        if (!originalActivity && item.isActive) {
                            reactivateTeam(TeamID);
                            db.SaveChanges();
                            Response.Redirect("~/Admin/Edit");
                        }

                        if (itemTeamName != item.TeamName){
                            updateTeamInSchedule(itemTeamName, item.TeamName);
                            db.SaveChanges();
                            Response.Redirect("~/Admin/Edit");
                        }

                        db.SaveChanges();
                        
                        TeamError.Visible = false;
                    }
                }
                else
                {

                    item.TeamName = itemTeamName;
                    TeamErrorText.Text = "That team name is not unique. Please choose a new one";
                    TeamError.Visible = true;
                    db.SaveChanges();

                }

            }
        }

        public void teamsGrid_DeleteItem(int TeamID)
        {
            using (DebateContext db = new DebateContext())
            {

                var item = db.Teams.Find(TeamID);

                if (db.TimeSlots == null || !db.TimeSlots.Any()){
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
                else {
                try
                {
                    deleteTeamFromSchedule(TeamID);
                    item.isActive = false;
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
        }

        protected void TextBox1_TextChanged(object sender, EventArgs e)
        {

        }

        protected void Button1_Click(object sender, EventArgs e)
        {
            using (DebateContext db = new DebateContext())
            {
                var teamName = TextBox1.Text;
                TeamError.Visible = false;
                var teamList = db.Teams.ToList();

                Boolean isTeamUnique = isTeamNameUnique(teamName, teamList);

                if (teamList.Count() < 10)
                {

                    if (isTeamUnique)
                    {
                        var item = new Team { TeamName = teamName, isActive = true };

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
                        Response.Redirect("~/Admin/Edit");
                    }
                    else
                    {
                        TeamErrorText.Text = "That team name is not unique. Please choose a new one";
                        TeamError.Visible = true;
                    }
                }
                else {

                    TeamErrorText.Text = "You can only have up to 10 teams";
                    TeamError.Visible = true;
                }
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
                ScheduleError.Visible = false;
                item = db.TimeSlots.Find(TimeSlotID);
                string originalTeam1 = item.Team1Name;
                string originalTeam2 = item.Team2Name;
                int originalTeam1Score = item.Team1Score;
                int originalTeam2Score = item.Team2Score;

                if (item == null)
                {
                    ModelState.AddModelError("",
                      String.Format("Item with id {0} was not found", TimeSlotID));
                    return;
                }
                    
                TryUpdateModel(item);
                if (item.Team1Name == "---" && item.Team2Name == "---") {
                    item.Team1Score = 0;
                    item.Team2Score = 0;
                    db.SaveChanges();
                }
                if (item.Team1Score < 0 || item.Team2Score < 0)
                {
                    item.Team1Score = originalTeam1Score;
                    item.Team2Score = originalTeam2Score;
                    ScheduleErrorText.Text = "Scores must be between 0 and 100 inclusive";
                    ScheduleError.Visible = true;
                    db.SaveChanges();
                }
                else if (item.Team1Score > 100 || item.Team2Score > 100) {
                    item.Team1Score = originalTeam1Score;
                    item.Team2Score = originalTeam2Score;
                    ScheduleErrorText.Text = "Scores must be between 0 and 100 inclusive";
                    ScheduleError.Visible = true;
                    db.SaveChanges();
                }
                if (item.Team1Name != originalTeam1)
                {
                    Team possibleTeam = findTeamByName(item.Team1Name);

                    if (possibleTeam == null)
                    {
                        //the team name is not in the team list so we need to alert the user.
                        ScheduleErrorText.Text = "Team 1's name is not from the teams list.";
                        ScheduleError.Visible = true;

                        item.Team1Name = originalTeam1;
                        db.SaveChanges();
                    } else if (possibleTeam != null && item.Team2Name == "FREE") {
                        //the team name is not in the team list so we need to alert the user.
                        ScheduleErrorText.Text = "A team cannot be in a time slot by themselves";
                        ScheduleError.Visible = true;

                        item.Team1Name = originalTeam1;
                        db.SaveChanges();
                    }
                    else {
                        if (ModelState.IsValid)
                        {
                            db.SaveChanges();
                        }
                        if (item.RoundStatus == null || item.RoundStatus == "")
                        {
                            if (!(item.Team1Score == 0 && item.Team2Score == 0)) {
                                item.RoundStatus = "Completed";

                                item.isLocked = true;
                            }
                            
                            db.SaveChanges();
                        }
                        CalculateTeamScore(item.Team1Name);
                        CalculateTeamScore(item.Team2Name);
                    }
                }
                else if (item.Team2Name != originalTeam2)
                {
                    Team possibleTeam = findTeamByName(item.Team2Name);

                    if (possibleTeam == null)
                    {
                        item.Team2Name = originalTeam2;

                        ScheduleErrorText.Text = "Team 2's name is not from the teams list.";
                        ScheduleError.Visible = true;

                        db.SaveChanges();
                        //throw an error
                    }else if (possibleTeam != null && item.Team1Name == "---")
                    {
                        //the team name is not in the team list so we need to alert the user.
                        ScheduleErrorText.Text = "A team cannot be in a time slot by themselves";
                        ScheduleError.Visible = true;

                        item.Team2Name = originalTeam2;
                        db.SaveChanges();
                    }
                    else {
                        if (ModelState.IsValid)
                        {
                            db.SaveChanges();
                        }
                        if (item.RoundStatus == null || item.RoundStatus == "")
                        {

                            if (!(item.Team1Score == 0 && item.Team2Score == 0))
                            {
                                item.RoundStatus = "Completed";

                                item.isLocked = true;
                            }
                            db.SaveChanges();
                        }
                        CalculateTeamScore(item.Team1Name);
                        CalculateTeamScore(item.Team2Name);
                        
                    }
                }
                else {

                    if (ModelState.IsValid){
                        db.SaveChanges();
                    }
                    if (item.RoundStatus == null || item.RoundStatus == ""){

                        if (!(item.Team1Score == 0 && item.Team2Score == 0))
                        {
                            item.RoundStatus = "Completed";

                            item.isLocked = true;
                        }
                        
                        db.SaveChanges();
                        
                    }
                    CalculateTeamScore(item.Team1Name);
                    CalculateTeamScore(item.Team2Name);
                    
                }

            }
        }

        public void scheduleGrid_DeleteItem(int TimeSlotID)
        {
            using (DebateContext db = new DebateContext())
            {
                var item = db.TimeSlots.Find(TimeSlotID);

                if (!item.isLocked)
                {
                    ScheduleError.Visible = false;
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
                else
                {
                    ScheduleErrorText.Text = "Cannot delete a row that is locked. Please unlock the row before deleting";
                    ScheduleError.Visible = true;
                }

            }
        }

        Boolean isTeamNameUnique(String teamName, List<Team> teamList)
        {
            Boolean isTeamNameUnique = true;

            if (teamName.ToLower() == "---") {
                isTeamNameUnique = false;
                return isTeamNameUnique;
            }
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
            if (teamName.ToLower() == "---")
            {
                isTeamNameUnique = false;
                return isTeamNameUnique;
            }
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

        protected void GenerateSchedule(object sender, EventArgs e)
        {
            DebateContext teamsDB = new DebateContext();
            Util.TimeSlot[] TimeSlotArray = Util.timeSlots;
            DebateContext scheduleDB = new DebateContext();

            if ((EndDate.SelectedDate - StartDate.SelectedDate).Days / 7 + 1 < Util.minWeeks[teamsDB.Teams.ToArray().Length])
            {
                //tell an error and return
                ScheduleErrorText.Text = "Please select more weeks in order to generate a valid schdeule";
                ScheduleError.Visible = true;
                return;
            }
            GenerateNewScheduleCheck.Visible = true;
            GenerateNewScheduleText.Text = "By generating a new schedule, you will delete all of the timeslots that are not locked.";
            GenerateNewSchedule.Visible = true;

            if (GenerateNewScheduleCheck.Checked && GenerateNewScheduleCheck.Visible == true)
            {

                var teams = listOfActiveTeams().ToArray();

                string[] teamNames = new string[teams.Length];

                for (int i = 0; i < teams.Length; i++)
                {
                    teamNames[i] = teams[i].TeamName;
                }

                List<Team> activeTeams = listOfActiveTeams();

                if (!isDateGood())
                {
                    InvalidDateText.Text = "Selected Dates are not valid.";
                    DateErrorMessage.Visible = true;
                    return;
                }

                if (!validNumberOfTeams())
                {
                    //send out an error to the screen
                    return;
                }

                Util.SetTeams(teamNames);

                Util.CreateSchedule(StartDate.SelectedDate, EndDate.SelectedDate);


                foreach (TimeSlot item in scheduleDB.TimeSlots.ToList())
                {

                    if (!item.isLocked)
                    {
                        scheduleDB.TimeSlots.Remove(item);
                        scheduleDB.SaveChanges();
                    }
                }

                TimeSlot TimeSlotToEnter = new TimeSlot();
                DbSet dbset = scheduleDB.Set(TimeSlotToEnter.GetType());

                foreach (Util.TimeSlot item in Util.timeSlots)
                {

                    TimeSlotToEnter = new TimeSlot();


                    if (item.team1Name == null)
                    {
                        TimeSlotToEnter.Team1Name = "---";
                    }
                    else
                    {
                        TimeSlotToEnter.Team1Name = item.team1Name;
                    }

                    if (item.team2Name == null)
                    {

                        TimeSlotToEnter.Team2Name = "---";
                    }
                    else
                    {

                        TimeSlotToEnter.Team2Name = item.team2Name;
                    }

                    TimeSlotToEnter.Team1Score = item.team1Score;
                    TimeSlotToEnter.Team2Score = item.team2Score;
                    TimeSlotToEnter.date = item.date;
                    TimeSlotToEnter.time = item.morning ? "Morning" : "Afternoon";



                    if (isTimeSlotUnique(TimeSlotToEnter))
                    {
                        dbset.Add(TimeSlotToEnter);

                    }


                    scheduleDB.SaveChanges();


                }

                foreach (Team item in teamsDB.Teams.ToList())
                {

                    CalculateTeamScore(item.TeamName);

                }
                Response.Redirect("~/Admin/Edit");
            }

        }

        protected void StartDate_SelectionChanged(object sender, EventArgs e)
        {
            if (StartDate.SelectedDate.DayOfWeek != DayOfWeek.Saturday || StartDate.SelectedDate < DateTime.Today || EndDate.SelectedDate < StartDate.SelectedDate)
            {
                
                //StartDate.SelectedDate = prevStartDate;
                InvalidDateText.Text = "The start date must be on a saturday and at least today or in the future. The End date cannot be before the start date";
                DateErrorMessage.Visible = true;
                StartDate.SelectedDate = DateTime.Today.AddDays(((int)DayOfWeek.Saturday - (int)DateTime.Today.DayOfWeek + 7) % 7);

            }
            else
            {
                prevStartDate = StartDate.SelectedDate;

                DateErrorMessage.Visible = false;
            }
        }

        protected void EndDate_SelectionChanged(object sender, EventArgs e)
        {
            if (EndDate.SelectedDate.DayOfWeek != DayOfWeek.Saturday || EndDate.SelectedDate < DateTime.Today || EndDate.SelectedDate < StartDate.SelectedDate)
            {
                //EndDate.SelectedDate = prevEndDate;
                InvalidDateText.Text = "The end date must be on a saturday and at least today or in the future. The end date cannot be before the start date";
                DateErrorMessage.Visible = true;
                EndDate.SelectedDate = StartDate.SelectedDate.AddDays(7);

            }
            else
            {
                prevEndDate = EndDate.SelectedDate;
                DateErrorMessage.Visible = false;



            }
        }

        protected bool isDateGood()
        {

            bool isDateGood = true;

            if (StartDate.SelectedDate.CompareTo(EndDate.SelectedDate) > 0 || !StartDate.SelectedDate.DayOfWeek.Equals(DayOfWeek.Saturday) || !EndDate.SelectedDate.DayOfWeek.Equals(DayOfWeek.Saturday))
            {
                isDateGood = false;
                InvalidDateText.Text = "Date is not good!";
                DateErrorMessage.Visible = true;
            }

            return isDateGood;
        }

        protected bool validNumberOfTeams()
        {
            bool numberOfTeamsValid = true;
            DebateContext teamsDB = new DebateContext();

            var teams = teamsDB.Teams.ToArray();

            if (teams.Length < 2 || teams.Length > 10)
            {
                numberOfTeamsValid = false;
                TeamErrorText.Text = "You must have less than 10 but more than 1 team to schedule a season.";
                TeamError.Visible = true;
                return numberOfTeamsValid;
            }


            return numberOfTeamsValid;
        }

        protected bool isTimeSlotUnique(TimeSlot possibleTimeslot)
        {
            bool timeSlotIsUnique = true;
            DebateContext timeslotDB = new DebateContext();
            foreach (TimeSlot item in timeslotDB.TimeSlots.ToList())
            {

                if (item.isLocked && item.Team1Name.Equals(possibleTimeslot.Team1Name) && item.Team2Name.Equals(possibleTimeslot.Team2Name) && item.date.Equals(possibleTimeslot.date))
                {
                    timeSlotIsUnique = false;
                    break;
                }
            }
            return timeSlotIsUnique;
        }

        protected void deleteTeamFromSchedule(int teamID)
        {
            using (DebateContext db = new DebateContext())
            {

                var item = db.Teams.Find(teamID);
                int numberOfRowsChanged = 0;
                if (item == null)
                {
                    TeamErrorText.Text = "Team could not be found in the schedule";
                    TeamError.Visible = true;
                }
                else
                {
                    TeamError.Visible = false;

                    foreach (TimeSlot scheduleItem in db.TimeSlots.ToList())
                    {
                        //need to make a back up of the schedule before deleting this.
                        if (scheduleItem.Team1Name.Equals(item.TeamName) || scheduleItem.Team2Name.Equals(item.TeamName))
                        {

                            if (scheduleItem.RoundStatus != null && scheduleItem.RoundStatus != "" && scheduleItem.RoundStatus != "Completed")
                            {

                                scheduleItem.RoundStatus += " | " + item.TeamName + " dropout";
                                numberOfRowsChanged++;
                            }
                            else
                            {
                                scheduleItem.RoundStatus = item.TeamName + " dropout";
                                numberOfRowsChanged++;
                            }
                        }
                    }
                    db.SaveChanges();
                }
                foreach (Team team in db.Teams.ToList())
                {

                    CalculateTeamScore(team.TeamName);

                }
            }

        }

        protected void DeleteSchedule_Click(object sender, EventArgs e)
        {
            DebateContext scheduleDB = new DebateContext();
            //need to first output the schedule to save.
            ConfirmDeletion.Visible = true;
            DeletionWarning.Visible = true;
            DeletionWarningText.Text = "WARNING: By clicking the confirm checkbox, you will delete all scheduled timeslots that are not locked. WARNING: If all rows are locked, the schedule will be COMPLETELY deleted.";

            
            if (ConfirmDeletion.Checked && ConfirmDeletion.Visible == true)
            {
                if (areAllRowsLocked()){
                    foreach (TimeSlot item in scheduleDB.TimeSlots.ToList())
                    {

                        scheduleDB.TimeSlots.Remove(item);
                        scheduleDB.SaveChanges();
                        
                    }

                    foreach (Team team in scheduleDB.Teams.ToList())
                    {

                        CalculateTeamScore(team.TeamName);

                    }
                }
                else{
                    foreach (TimeSlot item in scheduleDB.TimeSlots.ToList())
                    {

                        if (!item.isLocked)
                        {
                            scheduleDB.TimeSlots.Remove(item);
                            scheduleDB.SaveChanges();
                        }
                    }

                    foreach (Team team in scheduleDB.Teams.ToList())
                    {

                        CalculateTeamScore(team.TeamName);

                    }
                }

                
                Response.Redirect("~/Admin/Edit");
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

        protected void FinalizeButton_Click(object sender, EventArgs e)
        {
            FinalizeCheckbox.Visible = true;
            FinalizeScheduleWarningText.Text = "By Clicking this button, you will lock all rows, and finalize all scores and debates. The final winner of the season will then be declared";
            FinalizeWarning.Visible = true;

            if (FinalizeCheckbox.Checked && FinalizeWarning.Visible == true)
            {
                DebateContext db = new DebateContext();

                foreach (TimeSlot item in db.TimeSlots.ToList())
                {
                    item.isLocked = true;
                    db.SaveChanges();
                }

                Response.Redirect("~/Admin/Edit");
            }

        }

        protected bool areAllRowsLocked()
        {
            bool result = true;

            DebateContext db = new DebateContext();
            if (db.TimeSlots.ToList().Count == 0)
            {
                return false;
            }
            foreach (TimeSlot item in db.TimeSlots.ToList())
            {

                if (!item.isLocked)
                {
                    result = false;
                    break;
                }
            }
            return result;
        }

        protected List<Team> listOfActiveTeams() {
            DebateContext teamsDB = new DebateContext();

            List<Team> listOfActiveTeams = new List<Team>();

            foreach (Team item in teamsDB.Teams.ToList()) {
                if (item.isActive) {
                    listOfActiveTeams.Add(item);
                }
            }

            return listOfActiveTeams;
        }

        protected void reactivateTeam(int teamID) {
            using (DebateContext db = new DebateContext())
            {

                var item = db.Teams.Find(teamID);
                int numberOfRowsChanged = 0;
                if (item == null)
                {
                    TeamErrorText.Text = "Team could not be found in the schedule";
                    TeamError.Visible = true;
                }
                else
                {
                    TeamError.Visible = false;

                    foreach (TimeSlot scheduleItem in db.TimeSlots.ToList())
                    {
                        //need to make a back up of the schedule before deleting this.
                        if (scheduleItem.Team1Name.Equals(item.TeamName) || scheduleItem.Team2Name.Equals(item.TeamName))
                        {

                            if (scheduleItem.RoundStatus != null && scheduleItem.RoundStatus != "" && scheduleItem.RoundStatus != "Completed")
                            {

                                scheduleItem.RoundStatus += " | " + item.TeamName + " Reactivated";
                                numberOfRowsChanged++;
                            }
                            else
                            {
                                scheduleItem.RoundStatus = item.TeamName + " Reactivated";
                                numberOfRowsChanged++;
                            }
                        }
                    }
                    db.SaveChanges();
                }
                foreach (Team team in db.Teams.ToList())
                {

                    CalculateTeamScore(team.TeamName);

                }
            }

        }

        protected Team findTeamByName(String name) {
            DebateContext db = new DebateContext();
            Team team = null;

            foreach (Team item in db.Teams.ToList()){
                if (item.TeamName == name) {
                    team = item;
                    break;
                }
            }

            return team;            
        }

        protected void updateTeamInSchedule(string oldName, string newName) {
            DebateContext db = new DebateContext();

            foreach (TimeSlot item in db.TimeSlots.ToList()) {

                if (item.Team1Name == oldName)
                {
                    item.Team1Name = newName;
                    item.RoundStatus += "| " + "Team " + oldName + " Changed their name to: " + newName; 
                }
                else if (item.Team2Name == oldName) {
                    item.Team2Name = newName;
                    item.RoundStatus += "| " + "Team " + oldName + " Changed their name to: " + newName;
                }

            }
            db.SaveChanges();

        }

        protected void AddAnotherWeek_Click(object sender, EventArgs e)
        {
            DebateContext db = new DebateContext();

            TimeSlot timeslotToEnter = new TimeSlot();

            timeslotToEnter.Team1Name = "---";
            timeslotToEnter.Team2Name = "---";
            timeslotToEnter.Team1Score = 0;
            timeslotToEnter.Team2Score = 0;
            timeslotToEnter.RoundStatus = "Added Extra Week";
            TimeSlot[] timeSlotArray = db.TimeSlots.ToArray();
            string date = timeSlotArray[timeSlotArray.Length - 1].date;
            DateTime dateToEnter = Util.DateTimeConverter(date);
            dateToEnter = dateToEnter.AddDays(7);

            timeslotToEnter.date = Util.DateTimeConverter(dateToEnter);
           
            for (int i = 0; i < Int32.Parse(TimeSlotsDropDown.SelectedValue); i++) {
                timeslotToEnter.time = "Morning";
                db.TimeSlots.Add(timeslotToEnter);
                db.SaveChanges();
            }

            for (int i = 0; i < Int32.Parse(TimeSlotsDropDown.SelectedValue); i++)
            {
                timeslotToEnter.time = "Afternoon";
                db.TimeSlots.Add(timeslotToEnter);
                db.SaveChanges();
            }
            Response.Redirect("~/Admin/Edit");
        }

    }
}