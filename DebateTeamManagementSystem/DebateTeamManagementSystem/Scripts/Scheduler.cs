using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Globalization;

namespace DebateTeamManagementSystem.Scripts
{
    /// <summary>
    /// Module used to sort out a schedule for the debates.
    /// </summary>
    public class Scheduler
    {
        // Only need Month, Day, Year.  Time will be sorted using the timeSlot variable.
        // Time is set to midnight with the 'set' for these.
        DateTime startDate, endDate;

        // Ex: float[8.f, 20.5f] would be two time slots per day, 8:30am and 8:30pm
        float[] hourSlots;

        // Number of slots to leave free each day for reschedules
        int freeSlots;

        // Needs to contain teamNames or int IDs in case a team drops out.
        // We will use a special case value like -1 to indicate a team that
        // is no longer in the running.  This will maintain data integrity as
        // the team IDs will all stay the same throughout the season.
        string[] teamList;

        // Special struct that holds the DateTime of the debate, both team indices, and both team scores.
        Util.TimeSlot[] timeSlots;

        // list of team indices where even numbers are team1 and odd numbers are team2 of the fight.
        // Should only be used internally in this module.
        // External references of the pairings should use the timeSlots array.
        Util.Vec2[] fightPairings;

        /// <summary>
        /// Constructor, not currently used.
        /// </summary>
        public Scheduler() { }

        /// <summary>
        /// Boolean to declare the schedule is properly created
        /// </summary>
        public bool scheduleMade = false;

        /// <summary>
        /// Uses a DateTime startDate, DateTime endDate, float[] timeSlots, and string[] teamList set as updated from the editable display to
        /// sort out if a proper shcedule with a minimum amount of extra slots is available.  Will return a string describing whether more
        /// time slots or more weeks should be added to the season to accomodate the debates and a suitable amount for unforseen schedule changes.
        /// </summary>
        /// <returns>string message</returns>
        public string CreateSchedule()
        {
            int totalDays = (int)((endDate - startDate).TotalDays) / 7;
            int totalSlots = totalDays * hourSlots.Length;

            if (freeSlots > hourSlots.Length / 2)
            {
                return "Too many free slots or not enough hour slots";
            }

            if (totalSlots < teamList.Length - 1 * (teamList.Length / 2) + freeSlots * (totalDays - 1))
            {
                return "Not enough time slots and/or days exist for the number of teams or there are too many free slots.";
            }

            if (freeSlots > hourSlots.Length / 3)
            {
                return "Please add more free slots in the event of reschedules or tie-breakers";
            }

            // This assumes that the SetStartDate method was used to assign the startDate (startDate is a Saturday)
            int possibleDays = (int)((endDate - startDate).TotalDays / 7);

            // Create TimeSlot[] with slots for each hourSlot of each day.
            timeSlots = new Util.TimeSlot[possibleDays * hourSlots.Length];
            int hourIndex = 0;
            DateTime tempDateTime;
            for (int i = 0; i < timeSlots.Length; i++)
            {
                tempDateTime = new DateTime();
                tempDateTime = startDate.AddDays((int)(i / hourSlots.Length));
                tempDateTime = tempDateTime.AddHours((int)hourSlots[hourIndex]);
                tempDateTime = tempDateTime.AddMinutes((int)(60 * (hourSlots[hourIndex] % 1)));
                timeSlots[i].date = Util.DateTimeConverter(tempDateTime).Split('|')[0];
                timeSlots[i].time = Util.DateTimeConverter(tempDateTime).Split('|')[1];
                //Console.WriteLine(timeSlots[i].timeSlot);
                timeSlots[i].team1Name = "";
                timeSlots[i].team2Name = "";
                hourIndex++;
                if (hourIndex >= hourSlots.Length)
                {
                    hourIndex = 0;
                }
            }

            // Round Robin requires an even number of teams so if odd, add a stand-in BYE team.
            bool odd = false;
            if (teamList.Length % 2 != 0)
            {
                teamList = Util.ExtendArray(teamList, 1);
                teamList[teamList.Length - 1] = "BYE";
                odd = true;
            }

            // Get Ready To Rock
            int halfSize = teamList.Length / 2;
            fightPairings = new Util.Vec2[0];
            int pairIndex = 0;
            for (int i = 0; i < teamList.Length - 1; i++)
            {
                // Setup the rounds for one set
                int[] round = new int[teamList.Length];
                // Always start with the first team
                round[0] = 0;
                // Setup the rest of the circle for the round robin tournament algorithm
                for (int j = 1; j < teamList.Length; j++)
                {
                    round[j] = j + i - (j + i > teamList.Length - 1 ? teamList.Length - 1 : 0);
                }

                // Pair off the teams using the wheel (Just look up graphics of the Round Robin Tournament algorithm, this is what reads the wheel
                for (int j = 0; j < halfSize; j++)
                {
                    fightPairings = Util.ExtendArray(fightPairings, 1);
                    fightPairings[pairIndex] = new Util.Vec2(round[j], round[round.Length - 1 - j]);
                    pairIndex++;
                }
            }

            // If there was an odd number of teams, remove the BYE team pairings.  Keep in mind, we should only have (int)(teamList.Length/2) plays per round
            if (odd)
            {
                for (int i = fightPairings.Length - 1; i >= 0; i--)
                {
                    if (fightPairings[i].x == teamList.Length - 1 || fightPairings[i].y == teamList.Length - 1)
                        fightPairings = Util.RemoveAt(fightPairings, i);
                }
                // Get rid of the place-holder team.
                teamList = Util.ExtendArray(teamList, -1);
            }

            // Set the schedule here
            // Currently has no fail-safes
            // NEEDS TESTING AND CHECKS
            pairIndex = 0;
            for (int i = 0; i < totalDays; i++)
            {
                if (i == 0)
                {
                    for (int j = 0; j < hourSlots.Length; j++)
                    {
                        timeSlots[i * hourSlots.Length + j].team1Name = fightPairings[pairIndex].x.ToString();
                        timeSlots[i * hourSlots.Length + j].team2Name = fightPairings[pairIndex].y.ToString();
                        pairIndex++;
                        if (pairIndex > fightPairings.Length - 1)
                            break;
                    }
                }
                else
                {
                    for (int j = 0; j < hourSlots.Length - freeSlots; j++)
                    {
                        timeSlots[i * hourSlots.Length + j].team1Name = fightPairings[pairIndex].x.ToString();
                        timeSlots[i * hourSlots.Length + j].team2Name = fightPairings[pairIndex].y.ToString();
                        pairIndex++;
                        if (pairIndex > fightPairings.Length - 1)
                            break;
                    }
                }
                if (pairIndex > fightPairings.Length - 1)
                    break;
            }
            scheduleMade = true;

            return "This schedule is acceptable";
        }

        // All sets can be done as they are changed in the Editable Display since it's only once at the time of change.
        #region Gets and Sets
        /// <summary>
        /// First day of the season
        /// </summary>
        public DateTime StartDate
        {
            get { return startDate; }
            set
            {
                // Get the next Saturday from the incoming date, if the date is already Saturday, make no change.
                int firstSaturday = ((int)DayOfWeek.Saturday - (int)value.DayOfWeek + 7) % 7;
                startDate = value.AddDays(firstSaturday).Date;
            }
        }

        /// <summary>
        /// Last day of the season
        /// </summary>
        public DateTime EndDate
        {
            get { return endDate; }
            set
            {
                // Get the next Saturday from the incoming date, if the date is already Saturday, make no change.
                int firstSaturday = ((int)DayOfWeek.Saturday - (int)value.DayOfWeek + 7) % 7;
                endDate = value.AddDays(firstSaturday).Date;
            }
        }

        /// <summary>
        /// Takes in an array of floats describing the hourSlots that the debates will take place.
        /// </summary>
        /// <param name="slots">0 to 23.9~ describing each hour and percent of the hour (0.5f = 30 minutes)</param>
        public float[] HourSlots
        {
            get { return hourSlots; }
            set { hourSlots = value; }
        }

        /// <summary>
        /// Number of free slots to hold per day for rescheduling purposes
        /// </summary>
        public int FreeSlots
        {
            get { return freeSlots; }
            set { freeSlots = value; }
        }

        /// <summary>
        /// List of team names
        /// </summary>
        public string[] TeamList
        {
            get { return teamList; }
            set { teamList = value; }
        }

        /// <summary>
        /// Final Team pairings
        /// </summary>
        public Util.Vec2[] Pairings
        {
            get { return fightPairings; }
            // Setting fightPairings might become obsolete.  Depends on if we want to allow manual overrides and how they are handled.
            set { fightPairings = value; }
        }

        /// <summary>
        /// Get the schedule
        /// </summary>
        public Util.TimeSlot[] TimeSlots
        {
            get { return timeSlots; }
        }

        /// <summary>
        /// Get the variable that says the schedule is ready to be read
        /// </summary>
        public bool IsGood
        {
            get { return scheduleMade; }
        }
        #endregion
    }
}