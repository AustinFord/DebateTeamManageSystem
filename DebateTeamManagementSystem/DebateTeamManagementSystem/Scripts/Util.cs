using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IntroSWEConsoleApp
{
    /// <summary>
    /// Utility class to be used for various functions and custom structs
    /// </summary>
    public static class Util
    {
<<<<<<< HEAD

        public static string[] teamList;
        public static DateTime? startDate;
        public static DateTime? endDate;
        public static DateTime? backupEndDate;
        public static int hourSlots;
        public static int freeSlots = 1;

        private static TimeSlot[] timeSlots;
        private static Vec2[] fightPairings;
        private static Vec2[] sidePairings;
        private static int numWeeks = 0;
        private static int totalDays;
        private static List<DateTime> validDays;
        
=======
        public static TimeSlot[] timeSlots;
        private static Vec2[] fightPairings;
        private static int numWeeks = 0;

>>>>>>> d79fbd1b8e9a48e3ed12a7d401f609f7cb7bffc5
        // Struct to hold all debate schedules, the teams involved, and the scores of each team
        // Might eventually hold the referee assigned to the debate
        public struct TimeSlot
        {
            public string team1Name, team2Name, date;
            public bool morning, freeSlot;
            public int team2Score, team1Score;
        }

        // Struct to hold pairs of floats
        public struct Vec2
        {
            public float x, y;

            public Vec2(float inX = 0, float inY = 0)
            {
                x = inX;
                y = inY;
            }
        }

        /// <summary>
        /// Takes an array of any type and the number of elements to add or subtract from the total, then does.
        /// If "extensions" is negative, it will cut off that many elements from the end.
        /// </summary>
        /// <typeparam name="T">Array Type</typeparam>
        /// <param name="array">The array to change the size of</param>
        /// <param name="extensions">number of elements to add or subtract.</param>
        public static T[] ExtendArray<T>(T[] array, int extensions)
        {
            // If extensions would subtract more slots than the array has, just return a zero length array.
            if (-extensions > array.Length)
                return new T[0];

            // Create a temp array with a length of the inbound array adjusted by the extentions argument.
            T[] tempArray = new T[array.Length + extensions];

            // Start copying values up to the length of the NEW array.  This prevents leaving the bounds of a shortened array.
            for (int i = 0; i < tempArray.Length; i++)
            {
                // If we run out of values, stop trying to copy them over
                if (i >= array.Length)
                    break;

                tempArray[i] = array[i];
            }
            return tempArray;
        }

        /// <summary>
        /// Removes slot of array at 'index'
        /// </summary>
        /// <typeparam name="T">Type of Array</typeparam>
        /// <param name="array">Array to remove from</param>
        /// <param name="index">Index to remove at</param>
        /// <returns>Modified array</returns>
        public static T[] RemoveAt<T>(T[] array, int index)
        {
            // Worker boolean to identify the removed index and modify the copy algorithm
            bool removed = false;
            // New Array
            T[] tempArray = new T[array.Length - 1];
            // Copy over the values from the old array, skipping the value at index
            for (int i = 0; i < tempArray.Length; i++)
            {
                // Once index is found, set Removed to true and skip that value
                if (i == index)
                    removed = true;
                // Shorthand if statement that will shift the indicies by 1 once the removed index is found.
                tempArray[i] = array[i + (removed ? 1 : 0)];
            }
            return tempArray;
        }

        public static string DateTimeConverter(DateTime dateTime)
        {
            string temp = "";
            temp += dateTime.ToString("MMM") + " " + dateTime.Day + " " + dateTime.Year + "|" + dateTime.Hour + ":00 " + dateTime.ToString("tt");
            return temp;
        }
<<<<<<< HEAD
        
        public static string SetTeams (string[] teams)
        {
            if (teams.Length < 2)
                return "Please submit at least two (2) teams";

            if (teams.Length > 10)
                return "Only a maximum of ten (10) teams are allowed";

            teamList = teams;
            return "";
        }

        public static string SetStartDate (DateTime date)
        {
            if (date.DayOfWeek != DayOfWeek.Saturday)
                return "Please select a Saturday";

            startDate = date;

            if (endDate != null && endDate < startDate)
            {
                backupEndDate = endDate;
                endDate = null;
                return "The End Date cannot be before the Start Date";
            }

            if (backupEndDate != null)
            {
                endDate = backupEndDate;
                backupEndDate = null;
            }

            return "";
        }

        public static string SetEndDate (DateTime date)
        {
            if (date.DayOfWeek != DayOfWeek.Saturday)
                return "Please select a Saturday";
            
            if (endDate != null && endDate < startDate)
                return "The End Date cannot be before the Start Date";

            endDate = date;

            return "";
        }

        public static string SetHourSlots (int hours)
        {
            hourSlots = hours;
            return "";
        }

        public static string SetFreeSlots (int free)
        {
            freeSlots = free;
            return "";
        }

        public static string[] GenerateSchedule ()
        {
            string[] fatal = new string[0];
            string[] warnings = new string[0];

            // Check Team List
            if (teamList == null)
            {
                fatal = ExtendArray(fatal, 1);
                fatal[fatal.Length - 1] = "!Please submit a valid list of teams";
            }

            // Check start date
            if (startDate == null)
            {
                fatal = ExtendArray(fatal, 1);
                fatal[fatal.Length - 1] = "!Please choose a valid Start Date";
            }

            // Check end date
            if (endDate == null)
            {
                fatal = ExtendArray(fatal, 1);
                fatal[fatal.Length - 1] = "!Please choose a valid End Date";
            }

            validDays = new List<DateTime>();
            DateTime tempDate = (DateTime)startDate;
            while (tempDate <= endDate)
            {
                validDays.Add(tempDate);
                tempDate = tempDate.AddDays(7);
            }

            totalDays = validDays.Count;
            int totalSlots = totalDays * hourSlots;

            int minSlots = 0;

            for (int i = teamList.Length-1; i > 0; i--)
                minSlots += i;

            minSlots += (totalDays - 1) * freeSlots;

            if (minSlots < totalSlots)
            {
                fatal = ExtendArray(fatal, 1);
                fatal[fatal.Length - 1] = "!Not enough time slots exist to accomodate all of the teams and the number of free slots.";
            }

            if (freeSlots > (float)hourSlots / 2f)
            {
                warnings = ExtendArray(warnings, 1);
                warnings[warnings.Length - 1] = "@There might too many free slots allocated for a reasonible schedule.";
            }

            if (freeSlots < (float)hourSlots / 3f)
            {
                warnings = ExtendArray(warnings, 1);
                warnings[warnings.Length - 1] = "@There might not be enough free slots allocated for reschedules";
            }

            // If there are any fatal errors, return the fatal errors and warnings, else generate a schedule and return the warnings.
            string[] outString = new string[fatal.Length + warnings.Length];

            for (int i = 0; i < fatal.Length; i++)
            {
                outString[i] = fatal[i];
            }

            for (int i = 0; i < warnings.Length - 1; i++)
            {
                outString[fatal.Length + i] = warnings[i];
            }

            if (fatal.Length > 0)
                return outString;

            CreateSchedule((DateTime)startDate, (DateTime)endDate);

            return outString;
        }

        public static void CreateSchedule(DateTime start, DateTime end)
=======

        public static void CreateSchedule (string[] teams, DateTime start, DateTime end)
>>>>>>> d79fbd1b8e9a48e3ed12a7d401f609f7cb7bffc5
        {
            // Time Slaughter
            timeSlots = new TimeSlot[0];

            numWeeks = (end - start).Days / 7;
<<<<<<< HEAD
            float tsD = (((float)teamList.Length / 2) * (teamList.Length - 1)) / (numWeeks);
=======
            float tsD = (((float)teams.Length / 2) * (teams.Length - 1) )/(numWeeks);
>>>>>>> d79fbd1b8e9a48e3ed12a7d401f609f7cb7bffc5
            float carryOver = tsD % 1;
            int slots = (int)tsD;
            Console.WriteLine(tsD + " | " + carryOver + " | " + slots);

<<<<<<< HEAD
            float leapSlot = (carryOver > 0 ? 1 - carryOver : 0);
            bool leapAM = true;
            bool carryAM = true;
            bool useCarryAm = ((float)slots / 2f % 1 == 0 ? false : true);
=======
            float leapSlot = (carryOver > 0 ? 1-carryOver : 0);
            bool leapAM = true;
            bool carryAM = true;
            bool useCarryAm = ((float)slots/2f % 1 == 0 ? false : true);
>>>>>>> d79fbd1b8e9a48e3ed12a7d401f609f7cb7bffc5
            int jVal = (int)Math.Ceiling((float)slots / 2);

            for (int i = 0; i < numWeeks; i++)
            {
                //Console.WriteLine();
                //Console.WriteLine("AM | PM - Day " + (i+1) + " | CarryOver: " + (leapSlot + carryOver));
                for (int j = 0; j < jVal; j++)
                {
                    if (!(useCarryAm && j == jVal - 1))
                    {
                        //Console.WriteLine(" X | X ");

                        timeSlots = ExtendArray(timeSlots, 1);
<<<<<<< HEAD
                        timeSlots[timeSlots.Length - 1].date = "" + start.AddDays(i * 7).Date.Day + "/" + start.AddDays(i * 7).Date.Month + "/" + start.AddDays(i * 7).Date.Year;
                        timeSlots[timeSlots.Length - 1].morning = true;
                        timeSlots[timeSlots.Length - 1].freeSlot = false;

                        timeSlots = ExtendArray(timeSlots, 1);
                        timeSlots[timeSlots.Length - 1].date = "" + start.AddDays(i * 7).Date.Day + "/" + start.AddDays(i * 7).Date.Month + "/" + start.AddDays(i * 7).Date.Year;
                        timeSlots[timeSlots.Length - 1].morning = false;
                        timeSlots[timeSlots.Length - 1].freeSlot = false;
=======
                        timeSlots[timeSlots.Length -1].date = "" + start.AddDays(i * 7).Date.Day + "/" + start.AddDays(i * 7).Date.Month + "/" + start.AddDays(i * 7).Date.Year;
                        timeSlots[timeSlots.Length -1].morning = true;
                        timeSlots[timeSlots.Length -1].freeSlot = false;

                        timeSlots = ExtendArray(timeSlots, 1);
                        timeSlots[timeSlots.Length - 1].date = "" + start.AddDays(i * 7).Date.Day + "/" + start.AddDays(i * 7).Date.Month + "/" + start.AddDays(i * 7).Date.Year;
                        timeSlots[timeSlots.Length -1].morning = false;
                        timeSlots[timeSlots.Length -1].freeSlot = false;
>>>>>>> d79fbd1b8e9a48e3ed12a7d401f609f7cb7bffc5
                    }
                    else
                    {
                        if (carryAM)
                        {
                            //Console.WriteLine(" X | F ");

                            timeSlots = ExtendArray(timeSlots, 1);
                            timeSlots[timeSlots.Length - 1].date = "" + start.AddDays(i * 7).Date.Day + "/" + start.AddDays(i * 7).Date.Month + "/" + start.AddDays(i * 7).Date.Year;
<<<<<<< HEAD
                            timeSlots[timeSlots.Length - 1].morning = true;
                            timeSlots[timeSlots.Length - 1].freeSlot = false;

                            timeSlots = ExtendArray(timeSlots, 1);
                            timeSlots[timeSlots.Length - 1].date = "" + start.AddDays(i * 7).Date.Day + "/" + start.AddDays(i * 7).Date.Month + "/" + start.AddDays(i * 7).Date.Year;
                            timeSlots[timeSlots.Length - 1].morning = false;
                            timeSlots[timeSlots.Length - 1].freeSlot = true;
=======
                            timeSlots[timeSlots.Length -1].morning = true;
                            timeSlots[timeSlots.Length -1].freeSlot = false;

                            timeSlots = ExtendArray(timeSlots, 1);
                            timeSlots[timeSlots.Length - 1].date = "" + start.AddDays(i * 7).Date.Day + "/" + start.AddDays(i * 7).Date.Month + "/" + start.AddDays(i * 7).Date.Year;
                            timeSlots[timeSlots.Length -1].morning = false;
                            timeSlots[timeSlots.Length -1].freeSlot = true;
>>>>>>> d79fbd1b8e9a48e3ed12a7d401f609f7cb7bffc5
                        }
                        else
                        {
                            //Console.WriteLine(" F | X ");

                            timeSlots = ExtendArray(timeSlots, 1);
                            timeSlots[timeSlots.Length - 1].date = "" + start.AddDays(i * 7).Date.Day + "/" + start.AddDays(i * 7).Date.Month + "/" + start.AddDays(i * 7).Date.Year;
<<<<<<< HEAD
                            timeSlots[timeSlots.Length - 1].morning = true;
                            timeSlots[timeSlots.Length - 1].freeSlot = true;

                            timeSlots = ExtendArray(timeSlots, 1);
                            timeSlots[timeSlots.Length - 1].date = "" + start.AddDays(i * 7).Date.Day + "/" + start.AddDays(i * 7).Date.Month + "/" + start.AddDays(i * 7).Date.Year;
                            timeSlots[timeSlots.Length - 1].morning = false;
                            timeSlots[timeSlots.Length - 1].freeSlot = false;
=======
                            timeSlots[timeSlots.Length -1].morning = true;
                            timeSlots[timeSlots.Length -1].freeSlot = true;

                            timeSlots = ExtendArray(timeSlots, 1);
                            timeSlots[timeSlots.Length - 1].date = "" + start.AddDays(i * 7).Date.Day + "/" + start.AddDays(i * 7).Date.Month + "/" + start.AddDays(i * 7).Date.Year;
                            timeSlots[timeSlots.Length -1].morning = false;
                            timeSlots[timeSlots.Length -1].freeSlot = false;
>>>>>>> d79fbd1b8e9a48e3ed12a7d401f609f7cb7bffc5
                        }
                        carryAM = !carryAM;
                    }
                }

                leapSlot += carryOver;
                if (leapSlot >= 1)
                {
                    leapSlot -= 1f;
                    if (leapAM)
                    {
                        //Console.WriteLine(" X | F ");

                        timeSlots = ExtendArray(timeSlots, 1);
                        timeSlots[timeSlots.Length - 1].date = "" + start.AddDays(i * 7).Date.Day + "/" + start.AddDays(i * 7).Date.Month + "/" + start.AddDays(i * 7).Date.Year;
<<<<<<< HEAD
                        timeSlots[timeSlots.Length - 1].morning = true;
                        timeSlots[timeSlots.Length - 1].freeSlot = false;

                        timeSlots = ExtendArray(timeSlots, 1);
                        timeSlots[timeSlots.Length - 1].date = "" + start.AddDays(i * 7).Date.Day + "/" + start.AddDays(i * 7).Date.Month + "/" + start.AddDays(i * 7).Date.Year;
                        timeSlots[timeSlots.Length - 1].morning = false;
                        timeSlots[timeSlots.Length - 1].freeSlot = true;
=======
                        timeSlots[timeSlots.Length -1].morning = true;
                        timeSlots[timeSlots.Length -1].freeSlot = false;

                        timeSlots = ExtendArray(timeSlots, 1);
                        timeSlots[timeSlots.Length - 1].date = "" + start.AddDays(i * 7).Date.Day + "/" + start.AddDays(i * 7).Date.Month + "/" + start.AddDays(i * 7).Date.Year;
                        timeSlots[timeSlots.Length -1].morning = false;
                        timeSlots[timeSlots.Length -1].freeSlot = true;
>>>>>>> d79fbd1b8e9a48e3ed12a7d401f609f7cb7bffc5
                    }
                    else
                    {
                        //Console.WriteLine(" F | X ");

                        timeSlots = ExtendArray(timeSlots, 1);
                        timeSlots[timeSlots.Length - 1].date = "" + start.AddDays(i * 7).Date.Day + "/" + start.AddDays(i * 7).Date.Month + "/" + start.AddDays(i * 7).Date.Year;
<<<<<<< HEAD
                        timeSlots[timeSlots.Length - 1].morning = true;
                        timeSlots[timeSlots.Length - 1].freeSlot = true;

                        timeSlots = ExtendArray(timeSlots, 1);
                        timeSlots[timeSlots.Length - 1].date = "" + start.AddDays(i * 7).Date.Day + "/" + start.AddDays(i * 7).Date.Month + "/" + start.AddDays(i * 7).Date.Year;
                        timeSlots[timeSlots.Length - 1].morning = false;
                        timeSlots[timeSlots.Length - 1].freeSlot = false;
=======
                        timeSlots[timeSlots.Length -1].morning = true;
                        timeSlots[timeSlots.Length -1].freeSlot = true;

                        timeSlots = ExtendArray(timeSlots, 1);
                        timeSlots[timeSlots.Length - 1].date = "" + start.AddDays(i * 7).Date.Day + "/" + start.AddDays(i * 7).Date.Month + "/" + start.AddDays(i * 7).Date.Year;
                        timeSlots[timeSlots.Length -1].morning = false;
                        timeSlots[timeSlots.Length -1].freeSlot = false;
>>>>>>> d79fbd1b8e9a48e3ed12a7d401f609f7cb7bffc5
                    }
                    leapAM = !leapAM;
                }

                if (i != 0)
                {
                    //Console.WriteLine(" F | F ");

                    timeSlots = ExtendArray(timeSlots, 1);
                    timeSlots[timeSlots.Length - 1].date = "" + start.AddDays(i * 7).Date.Day + "/" + start.AddDays(i * 7).Date.Month + "/" + start.AddDays(i * 7).Date.Year;
<<<<<<< HEAD
                    timeSlots[timeSlots.Length - 1].morning = true;
                    timeSlots[timeSlots.Length - 1].freeSlot = true;

                    timeSlots = ExtendArray(timeSlots, 1);
                    timeSlots[timeSlots.Length - 1].date = "" + start.AddDays(i * 7).Date.Day + "/" + start.AddDays(i * 7).Date.Month + "/" + start.AddDays(i * 7).Date.Year;
                    timeSlots[timeSlots.Length - 1].morning = false;
                    timeSlots[timeSlots.Length - 1].freeSlot = true;
                }
            }

            PairTeams(teamList);

            FillSchedule(teamList);
=======
                    timeSlots[timeSlots.Length -1].morning = true;
                    timeSlots[timeSlots.Length -1].freeSlot = true;

                    timeSlots = ExtendArray(timeSlots, 1);
                    timeSlots[timeSlots.Length - 1].date = "" + start.AddDays(i * 7).Date.Day + "/" + start.AddDays(i * 7).Date.Month + "/" + start.AddDays(i * 7).Date.Year;
                    timeSlots[timeSlots.Length -1].morning = false;
                    timeSlots[timeSlots.Length -1].freeSlot = true;
                }
            }

            PairTeams(teams);

            FillSchedule(teams);
>>>>>>> d79fbd1b8e9a48e3ed12a7d401f609f7cb7bffc5

            Console.WriteLine();

            for (int i = 0; i < timeSlots.Length; i += 2)
            {
<<<<<<< HEAD
                Console.WriteLine(timeSlots[i].team1Name + "-" + timeSlots[i].team2Name + " | " + timeSlots[i + 1].team1Name + "-" + timeSlots[i + 1].team2Name);
            }
        }

        private static void PairTeams(string[] teamList)
=======
                Console.WriteLine(timeSlots[i].team1Name + "-" + timeSlots[i].team2Name + " | " + timeSlots[i+1].team1Name + "-" + timeSlots[i+1].team2Name);
            }
        }

        private static void PairTeams (string[] teamList)
>>>>>>> d79fbd1b8e9a48e3ed12a7d401f609f7cb7bffc5
        {
            // Get Ready To Rock
            bool odd = (teamList.Length % 2 == 0 ? false : true);
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
        }

        private static void FillSchedule(string[] teamList)
        {
            Vec2[] sideBoard = new Vec2[0];

            string date = timeSlots[0].date;
            int timeSlotIndex = 0;

            for (int i = 0; i < numWeeks; i++)
            {
                int[] currentDay = new int[0];
                while (timeSlotIndex < timeSlots.Length && date == timeSlots[timeSlotIndex].date)
                {
                    currentDay = ExtendArray(currentDay, 1);
                    currentDay[currentDay.Length - 1] = timeSlotIndex;
                    timeSlotIndex++;
                }
                if (timeSlotIndex < timeSlots.Length)
                    date = timeSlots[timeSlotIndex].date;

                int[] morningSlots = new int[0];
                int[] afternoonSlots = new int[0];

                for (int j = 0; j < currentDay.Length; j++)
                {
                    if (timeSlots[currentDay[j]].freeSlot)
                        continue;

                    if (timeSlots[currentDay[j]].morning)
                    {
                        morningSlots = ExtendArray(morningSlots, 1);
<<<<<<< HEAD
                        morningSlots[morningSlots.Length - 1] = currentDay[j];
=======
                        morningSlots[morningSlots.Length-1] = currentDay[j];
>>>>>>> d79fbd1b8e9a48e3ed12a7d401f609f7cb7bffc5
                    }
                    else
                    {
                        afternoonSlots = ExtendArray(afternoonSlots, 1);
<<<<<<< HEAD
                        afternoonSlots[afternoonSlots.Length - 1] = currentDay[j];
=======
                        afternoonSlots[afternoonSlots.Length-1] = currentDay[j];
>>>>>>> d79fbd1b8e9a48e3ed12a7d401f609f7cb7bffc5
                    }
                }

                int[] badTeams = new int[0];

                // Morning slots
                for (int j = 0; j < morningSlots.Length; j++)
                {
                    bool noSideBoard = true;
                    for (int k = 0; k < sideBoard.Length; k++)
                    {
                        bool goodTeam = true;
                        for (int l = 0; l < badTeams.Length; l++)
                        {
                            if (sideBoard[k].x == badTeams[l] || sideBoard[k].y == badTeams[l])
                            {
                                goodTeam = false;
                                break;
                            }
                        }
                        if (!goodTeam)
                            continue;

                        noSideBoard = false;
                        timeSlots[morningSlots[j]].team1Name = teamList[(int)sideBoard[k].x];
                        timeSlots[morningSlots[j]].team2Name = teamList[(int)sideBoard[k].y];

                        badTeams = ExtendArray(badTeams, 2);
                        badTeams[badTeams.Length - 2] = (int)sideBoard[k].x;
                        badTeams[badTeams.Length - 1] = (int)sideBoard[k].y;

                        sideBoard = RemoveAt(sideBoard, k);
                        break;
                    }

                    if (noSideBoard)
                    {
                        // After sideBoard is checked and taken care of, do this if there's not a team added to that slot
                        bool goodTeam = true;
                        do
                        {
                            for (int k = 0; k < badTeams.Length; k++)
                            {
                                if (badTeams[k] == fightPairings[0].x || badTeams[k] == fightPairings[0].y)
                                {
                                    goodTeam = false;
                                    sideBoard = ExtendArray(sideBoard, 1);
                                    sideBoard[sideBoard.Length - 1] = fightPairings[0];
                                }
                            }

                            if (goodTeam)
                            {
                                timeSlots[morningSlots[j]].team1Name = teamList[(int)fightPairings[0].x];
                                timeSlots[morningSlots[j]].team2Name = teamList[(int)fightPairings[0].y];

                                badTeams = ExtendArray(badTeams, 2);
                                badTeams[badTeams.Length - 2] = (int)fightPairings[0].x;
                                badTeams[badTeams.Length - 1] = (int)fightPairings[0].y;
                            }
                            fightPairings = RemoveAt(fightPairings, 0);

                        } while (!goodTeam);
                    }
                }
<<<<<<< HEAD

=======
                
>>>>>>> d79fbd1b8e9a48e3ed12a7d401f609f7cb7bffc5
                // Afternoon slots
                badTeams = new int[0];
                for (int j = 0; j < afternoonSlots.Length; j++)
                {
                    bool noSideBoard = true;
                    for (int k = 0; k < sideBoard.Length; k++)
                    {
                        bool goodTeam = true;
                        for (int l = 0; l < badTeams.Length; l++)
                        {
                            if (sideBoard[k].x == badTeams[l] || sideBoard[k].y == badTeams[l])
                            {
                                goodTeam = false;
                                break;
                            }
                        }
                        if (!goodTeam)
                            continue;

                        noSideBoard = false;
                        timeSlots[afternoonSlots[j]].team1Name = teamList[(int)sideBoard[k].x];
                        timeSlots[afternoonSlots[j]].team2Name = teamList[(int)sideBoard[k].y];

                        badTeams = ExtendArray(badTeams, 2);
                        badTeams[badTeams.Length - 2] = (int)sideBoard[k].x;
                        badTeams[badTeams.Length - 1] = (int)sideBoard[k].y;

                        sideBoard = RemoveAt(sideBoard, k);
                        break;
                    }

                    if (noSideBoard)
                    {
                        // After sideBoard is checked and taken care of, do this if there's not a team added to that slot
                        bool goodTeam = true;
                        do
                        {
                            for (int k = 0; k < badTeams.Length; k++)
                            {
                                if (badTeams[k] == fightPairings[0].x || badTeams[k] == fightPairings[0].y)
                                {
                                    goodTeam = false;
                                    sideBoard = ExtendArray(sideBoard, 1);
                                    sideBoard[sideBoard.Length - 1] = fightPairings[0];
                                }
                            }

                            if (goodTeam)
                            {
                                timeSlots[afternoonSlots[j]].team1Name = teamList[(int)fightPairings[0].x];
                                timeSlots[afternoonSlots[j]].team2Name = teamList[(int)fightPairings[0].y];

                                badTeams = ExtendArray(badTeams, 2);
                                badTeams[badTeams.Length - 2] = (int)fightPairings[0].x;
                                badTeams[badTeams.Length - 1] = (int)fightPairings[0].y;
                            }
                            fightPairings = RemoveAt(fightPairings, 0);

                        } while (!goodTeam);
                    }
                }
            }
        }
    }
}