using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

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

        // Special struct that holds the DateTime of the debate, both team indices, and both team scores.
        Util.TimeSlot[] timeSlots;
        
        // Needs to contain teamNames or int IDs in case a team drops out.
        // We will use a special case value like -1 to indicate a team that
        // is no longer in the running.  This will maintain data integrity as
        // the team IDs will all stay the same throughout the season.
        string[] teamList;

        // list of team indices where even numbers are team1 and odd numbers are team2 of the fight.
        // Should only be used internally in this module.
        // External references of the pairings should use the timeSlots array.
        Util.Vec2[] fightPairings;

        /// <summary>
        /// Constructor, not currently used.
        /// </summary>
        public Scheduler () { }

        /// <summary>
        /// Uses a DateTime startDate, DateTime endDate, float[] timeSlots, and string[] teamList set as updated from the editable display to
        /// sort out if a proper shcedule with a minimum amount of extra slots is available.  Will return a string describing whether more
        /// time slots or more weeks should be added to the season to accomodate the debates and a suitable amount for unforseen schedule changes.
        /// </summary>
        /// <returns>string message</returns>
        public string CreateSchedule ()
        {
            // This assumes that the SetStartDate method was used to assign the startDate (startDate is a Saturday)
            int possibleDays = (int)((endDate - startDate).TotalDays / 7);

            // Create TimeSlot[] with slots for each hourSlot of each day.
            timeSlots = new Util.TimeSlot[possibleDays * hourSlots.Length];
            for (int i = 0; i < timeSlots.Length; i++)
            {
                timeSlots[i].timeSlot = startDate.AddDays((int)(i / hourSlots.Length));
            }

            // Make a tempArray of all team pairings
            Util.Vec2[] tempPairings = new Util.Vec2[0];
            // Assign pairings
            for (int i = 0; i < teamList.Length-1; i++)
            {
                for (int j = i+1; j < teamList.Length; j++)
                {
                    // Make a new slot for this pair
                    tempPairings = Util.ExtendArray<Util.Vec2>(tempPairings, 1);
                    // Assign the pair
                    tempPairings[tempPairings.Length - 1] = new Util.Vec2(i, j);
                }
            }
            
            // Sort the tempArray into the fightPairings array using our algorithm.
            fightPairings = new Util.Vec2[tempPairings.Length];
            int fightIndex = 0;
            for (int i = 0; i < teamList.Length-1; i++)
            {
                int index = 0;
                for (int j = teamList.Length-1; index < tempPairings.Length; j--)
                {
                    fightPairings[fightIndex] = tempPairings[index];
                    fightIndex++;
                    index += j;
                }
                tempPairings = Util.RemoveAt<Util.Vec2>(tempPairings, 0);
            }

            return "This schedule is acceptable";
        }

        // All sets can be done as they are changed in the Editable Display since it's only once at the time of change.
        #region Gets and Sets
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
        
        public string[] TeamList
        {
            get { return teamList; }
            set { teamList = value; }
        }

        public Util.Vec2[] Pairings
        {
            get { return fightPairings; }
            // Setting fightPairings might become obsolete.  Depends on if we want to allow manual overrides and how they are handled.
            set { fightPairings = value; }
        }
        #endregion
    }
}