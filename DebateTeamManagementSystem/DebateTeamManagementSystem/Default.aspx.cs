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
            if (areAllRowsLocked()) {
            declareWinner();

            }
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

        public void declareWinner() {

            List<int> winners = new List<int>();
            int highestScore = findLargestScore();
            DebateContext db = new DebateContext();
            Team[] teamsArray = db.Teams.ToArray();

            for (int i = 0; i < teamsArray.Length; i++) {

                if (teamsArray[i].Score == teamsArray[highestScore].Score) {
                    winners.Add(i);
                }
                
            }

            if (winners.Count == 1)
            {
                WinnerText.Text = "The winner is: " + teamsArray[winners[0]].TeamName;
                Winner.Visible = true;
            }
            else {
                int[] wins = new int[winners.Count];
                for (int i = 0; i < winners.Count; i++)
                {
                    wins[i] = getTeamWin(teamsArray[winners[i]]);
                }

                int maxWin = FindMaxValue(wins);

                List<int> hyperTie = new List<int>();
                for (int i = 0; i < winners.Count; i++)
                {
                    if (wins[i] == maxWin)
                    {
                        hyperTie.Add(winners[i]);
                    }
                }

                if (hyperTie.Count == 1)
                {
                    WinnerText.Text = "The winner is: " + teamsArray[hyperTie[0]].TeamName;
                    Winner.Visible = true;
                } else
                {
                    // Send email to super ref
                    WinnerText.Text = "Teams ";
                    for (int i = 0; i < hyperTie.Count; i++)
                    {
                        WinnerText.Text += teamsArray[hyperTie[i]].TeamName + ", ";
                    }
                    WinnerText.Text += "will have to play off.";
                    Winner.Visible = true;
                }
            }
            
        }

        public int findLargestScore() {
            DebateContext db = new DebateContext();

            Team[] teamArray = db.Teams.ToArray();
            int highScore = 0;
            for (int i = 1; i < teamArray.Length; i++) {
                if (teamArray[i].Score > teamArray[highScore].Score) {
                    highScore = i;
                }
            }
            return highScore; 
        }

        public int getTeamWin(Team currentTeam ) {
            DebateContext db = new DebateContext();
            int wins = 0;
            foreach (TimeSlot item in db.TimeSlots.ToList()) {

                if (item.Team1Name == currentTeam.TeamName && item.Team1Score >= item.Team2Score && item.Team1Score != 0)
                {
                    wins++;
                }
                else if (item.Team2Name == currentTeam.TeamName && item.Team1Score <= item.Team2Score && item.Team2Score != 0) {
                    wins++;
                }
            }
            return wins;
        }

        protected bool areAllRowsLocked()
        {
            bool result = true;

            DebateContext db = new DebateContext();
            if (db.TimeSlots.ToList().Count == 0) {
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

        public int FindMaxValue (int[] array)
        {
            if (array.Length == 0)
                return 0;
            int max = array[0];
            for (int i = 1; i < array.Length; i++)
            {
                if (array[i] > max)
                    max = array[i];
            }
            return max;
        }
    }
}