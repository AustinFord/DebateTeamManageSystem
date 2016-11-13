using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DebateTeamManagementSystem.Scripts
{
    /// <summary>
    /// Utility class to be used for various functions and custom structs
    /// </summary>
    public static class Util
    {

        public static string[] teamList;
        public static DateTime? startDate;
        public static DateTime? endDate;
        public static DateTime? backupEndDate;
        public static TimeSlot[] timeSlots;

        private static Vec2[] fightPairings;
        private static int numWeeks = 0;
        private static int totalDays;
        private static List<DateTime> validDays;
        private static Vec2[] sideBoard;

        private static string[] months = { "Jan", "Feb", "Mar", "Apr", "May", "Jun", "Jul", "Aug", "Sep", "Oct", "Nov", "Dec" };
                             // TeamCount 0  1  2  3  4  5  6  7  8  9  10
        private static int[] minWeeks = { 0, 0, 1, 2, 2, 3, 3, 4, 4, 5, 5 };

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

        /// <summary>
        /// Takes a DateTime variable and converts it to a sentence.
        /// </summary>
        /// <param name="dateTime">The DateTime to convert from</param>
        /// <returns></returns>
        public static string DateTimeConverter(DateTime dateTime)
        {
            string temp = "";
            temp += dateTime.ToString("MMM") + " " + dateTime.Day + " " + dateTime.Year + "|" + dateTime.Hour + ":00 " + dateTime.ToString("tt");
            return temp;
        }

        /// <summary>
        /// Takes a string date and converts it to a sentence.
        /// </summary>
        /// <param name="date">Date in terms of MM/DD/YYYY</param>
        /// <returns></returns>
        public static string DateTimeConverter(string date)
        {
            return "" + months[Int32.Parse(date.Split('/')[0]) - 1] + " " + date.Split('/')[1] + ", " + date.Split('/')[2];
        }

        public static string SetTeams(string[] teams)
        {
            if (teams.Length < 2)
                return "Please submit at least two (2) teams";

            if (teams.Length > 10)
                return "Only a maximum of ten (10) teams are allowed";

            teamList = teams;
            return "";
        }

        public static string SetStartDate(DateTime date)
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

        public static string SetEndDate(DateTime date)
        {
            if (date.DayOfWeek != DayOfWeek.Saturday)
                return "Please select a Saturday";

            if (endDate != null && endDate < startDate)
                return "The End Date cannot be before the Start Date";

            endDate = date;

            return "";
        }

        public static string[] GenerateSchedule()
        {
            string[] fatal = new string[0];

            // Check Team List
            if (teamList == null)
            {
                fatal = ExtendArray(fatal, 1);
                fatal[fatal.Length - 1] = "Please submit a valid list of teams";
            }

            // Check start date
            if (startDate == null)
            {
                fatal = ExtendArray(fatal, 1);
                fatal[fatal.Length - 1] = "Please choose a valid Start Date";
            }

            // Check end date
            if (endDate == null)
            {
                fatal = ExtendArray(fatal, 1);
                fatal[fatal.Length - 1] = "Please choose a valid End Date";
            }

            validDays = new List<DateTime>();
            DateTime tempDate = (DateTime)startDate;
            while (tempDate <= endDate)
            {
                validDays.Add(tempDate);
                tempDate = tempDate.AddDays(7);
            }

            totalDays = validDays.Count;

            int minSlots = 0;

            for (int i = teamList.Length - 1; i > 0; i--)
                minSlots += i;

            if (fatal.Length > 0)
                return fatal;

            CreateSchedule((DateTime)startDate, (DateTime)endDate);

            return fatal;
        }

        public static bool CreateSchedule(DateTime start, DateTime end)
        {
            // Time Slaughter
            timeSlots = new TimeSlot[0];

            numWeeks = (end - start).Days / 7 + 1;
            float adjustedTeamSize = teamList.Length + (teamList.Length % 2);
            float tsD = ((adjustedTeamSize / 2) * (adjustedTeamSize - 1)) / (numWeeks);
            tsD = tsD + (tsD % 2);
            float carryOver = tsD % 1;
            int slots = (int)tsD;
            Console.WriteLine(tsD + " | " + carryOver + " | " + slots);

            float leapSlot = (carryOver > 0 ? 1 - carryOver : 0);
            bool leapAM = false;
            bool carryAM = true;
            bool useCarryAm = ((float)slots / 2f % 1 == 0 ? false : true);
            int jVal = (int)Math.Ceiling((float)slots / 2);

            for (int i = 0; i < numWeeks; i++)
            {
                Console.WriteLine("Week " + (i + 1) + " | Carry: " + (leapSlot + carryOver));
                for (int j = 0; j < jVal; j++)
                {
                    if (!(useCarryAm && j == jVal - 1))
                    {
                        Console.WriteLine("D | D");
                        SetSlot(i, start, end, true, false);
                        SetSlot(i, start, end, false, false);
                    }
                    else
                    {
                        if (carryAM)
                        {
                            Console.WriteLine("D | F");
                            SetSlot(i, start, end, true, false);
                            SetSlot(i, start, end, false, true);
                        }
                        else
                        {
                            Console.WriteLine("F | D");
                            SetSlot(i, start, end, true, true);
                            SetSlot(i, start, end, false, false);
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
                        Console.WriteLine("D | F");
                        SetSlot(i, start, end, true, false);
                        SetSlot(i, start, end, false, true);
                    }
                    else
                    {
                        Console.WriteLine("F | D");
                        SetSlot(i, start, end, true, true);
                        SetSlot(i, start, end, false, false);
                    }
                    leapAM = !leapAM;
                }

                if (i != 0)
                {
                    Console.WriteLine("F | F");
                    SetSlot(i, start, end, true, true);
                    SetSlot(i, start, end, false, true);
                }
            }

            PairTeams(teamList);

            return FillSchedule(teamList);
        }

        private static void SetSlot(int i, DateTime start, DateTime end, bool morning, bool freeSlot)
        {
            timeSlots = ExtendArray(timeSlots, 1);
            timeSlots[timeSlots.Length - 1].date = DateTimeConverter("" + start.AddDays(i * 7).Date.Month + "/" + start.AddDays(i * 7).Date.Day + "/" + start.AddDays(i * 7).Date.Year);
            timeSlots[timeSlots.Length - 1].morning = morning;
            timeSlots[timeSlots.Length - 1].freeSlot = freeSlot;
        }

        private static void PairTeams(string[] teamList)
        {
            // Get Ready To Rock
            bool odd = (teamList.Length % 2 == 0 ? false : true);
            fightPairings = new Util.Vec2[0];
            int pairIndex = 0;

            if (odd)
            {
                teamList = Util.ExtendArray(teamList, 1);
                teamList[teamList.Length - 1] = "BYE";
            }

            int halfSize = teamList.Length / 2;

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
                    Console.WriteLine(teamList[(int)fightPairings[pairIndex].x] + " against " + teamList[(int)fightPairings[pairIndex].y]);
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

        private static bool FillSchedule(string[] teamList)
        {
            sideBoard = new Vec2[0];

            string date = timeSlots[0].date;
            int timeSlotIndex = 0;

            for (int i = 0; i < numWeeks; i++)
            {
                int[] currentDay = new int[0];
                while (timeSlotIndex < timeSlots.Length && date == timeSlots[timeSlotIndex].date)
                {
                    if (timeSlots[timeSlotIndex].freeSlot)
                    {
                        timeSlotIndex++;
                        continue;
                    }
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
                        morningSlots[morningSlots.Length - 1] = currentDay[j];
                    }
                    else
                    {
                        afternoonSlots = ExtendArray(afternoonSlots, 1);
                        afternoonSlots[afternoonSlots.Length - 1] = currentDay[j];
                    }
                }

                int[] badTeams = new int[0];

                // Morning slots
                FillSlots(morningSlots);

                // Afternoon slots
                FillSlots(afternoonSlots);
            }

            return sideBoard.Length == 0;
        }

        private static void FillSlots(int[] slots)
        {
            int[] badTeams = new int[0];

            for (int j = 0; j < slots.Length; j++)
            {
                if (fightPairings.Length == 0 && sideBoard.Length == 0)
                    break;
                bool noSideBoard = true;
                for (int k = 0; k < sideBoard.Length; k++)
                {
                    bool goodTeam = true;
                    foreach (int b in badTeams)
                    {
                        if (sideBoard[k].x == b || sideBoard[k].y == b)
                        {
                            goodTeam = false;
                            break;
                        }
                    }
                    if (!goodTeam)
                        continue;

                    noSideBoard = false;
                    timeSlots[slots[j]].team1Name = teamList[(int)sideBoard[k].x];
                    timeSlots[slots[j]].team2Name = teamList[(int)sideBoard[k].y];
                    Console.WriteLine(timeSlots[slots[j]].team1Name + " - " + timeSlots[slots[j]].team2Name);

                    badTeams = ExtendArray(badTeams, 2);
                    badTeams[badTeams.Length - 2] = (int)sideBoard[k].x;
                    badTeams[badTeams.Length - 1] = (int)sideBoard[k].y;

                    sideBoard = RemoveAt(sideBoard, k);
                    k--;
                    break;
                }

                if (noSideBoard && fightPairings.Length > 0)
                {
                    // After sideBoard is checked and taken care of, do this if there's not a team added to that slot
                    bool goodTeam = true;
                    do
                    {
                        if (fightPairings.Length == 0)
                            break;
                        goodTeam = true;
                        for (int k = 0; k < badTeams.Length; k++)
                        {
                            if (badTeams[k] == fightPairings[0].x || badTeams[k] == fightPairings[0].y)
                            {
                                goodTeam = false;
                                sideBoard = ExtendArray(sideBoard, 1);
                                sideBoard[sideBoard.Length - 1] = fightPairings[0];
                                fightPairings = RemoveAt(fightPairings, 0);
                                if (fightPairings.Length == 0)
                                    break;
                            }
                        }

                        if (goodTeam)
                        {
                            timeSlots[slots[j]].team1Name = teamList[(int)fightPairings[0].x];
                            timeSlots[slots[j]].team2Name = teamList[(int)fightPairings[0].y];
                            Console.WriteLine(timeSlots[slots[j]].team1Name + " - " + timeSlots[slots[j]].team2Name);

                            badTeams = ExtendArray(badTeams, 2);
                            badTeams[badTeams.Length - 2] = (int)fightPairings[0].x;
                            badTeams[badTeams.Length - 1] = (int)fightPairings[0].y;
                            fightPairings = RemoveAt(fightPairings, 0);
                            if (fightPairings.Length == 0)
                                break;
                        }

                    } while (!goodTeam);
                }
            }
        }
    }
}