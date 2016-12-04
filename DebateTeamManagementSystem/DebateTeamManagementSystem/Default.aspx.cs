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

            List<Team> displayList = new List<Team>();

            foreach (Team item in db.Teams.ToList())
            {

                if (item.isActive)
                {
                    displayList.Add(item);
                }
            }
            IQueryable<Team> query = displayList.AsQueryable();
            return query;
        }

        public IQueryable<TimeSlot> scheduleGrid_GetData()
        {
            DebateContext db = new DebateContext();
            List<TimeSlot> displayList = new List<TimeSlot>();
            
            foreach (TimeSlot item in db.TimeSlots.ToList()) {

                if (!item.Team1Name.Equals("---") && !item.Team2Name.Equals("---")) {
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
            Team[] teamsArray = listOfActiveTeams().ToArray();

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

            Team[] teamArray = listOfActiveTeams().ToArray();
            int highScore = 0;
            for (int i = 0; i <= teamArray.Length -1; i++) {
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
        protected List<Team> listOfActiveTeams()
        {
            DebateContext teamsDB = new DebateContext();

            List<Team> listOfActiveTeams = new List<Team>();

            foreach (Team item in teamsDB.Teams.ToList())
            {
                if (item.isActive)
                {
                    listOfActiveTeams.Add(item);
                }
            }

            return listOfActiveTeams;
        }

        

    }
}