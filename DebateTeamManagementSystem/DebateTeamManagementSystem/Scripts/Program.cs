using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DebateTeamManagementSystem.Scripts
{
    class Program
    {
        static void Main(string[] args)
        {
            // Create a Scheduler
            Scheduler scheduler = new Scheduler();
            string teamList = "";
            string singleTeam = "";

            // Enter Team Names
            Console.WriteLine("Please input a list of teams.  Press 'Enter' between names.");
            Console.WriteLine("Press the 'Escape' key when the list is complete");
            ConsoleKeyInfo keyInfo;
            do
            {
                keyInfo = Console.ReadKey();
                if (keyInfo.Key == ConsoleKey.Enter)
                {
                    Console.WriteLine(singleTeam);
                    teamList += singleTeam + "|";
                    singleTeam = "";
                }
                singleTeam += keyInfo.KeyChar;
            } while (keyInfo.Key != ConsoleKey.Escape);

            teamList = teamList.Substring(0, teamList.Length - 2);

            // Push teams to the Scheduler
            string[] teams = teamList.Split('|');
            scheduler.TeamList = teams;

            /* Console feedback
            Console.WriteLine("The team list is " + teamList.Split('|').Length + " long:");
            for (int i = 0; i < teams.Length; i++)
            {
                Console.WriteLine(teams[i]);
            }
            */

            // Start Date
            Console.WriteLine("Please enter a start date [YYYY/MM/DD]");
            string startDateString = Console.ReadLine();
            DateTime startDate = new DateTime(Int32.Parse(startDateString.Split('/')[0]), Int32.Parse(startDateString.Split('/')[1]), Int32.Parse(startDateString.Split('/')[2]));
            scheduler.StartDate = startDate;

            // End Date
            Console.WriteLine("Please enter an end date [YYYY/MM/DD]");
            string endDateString = Console.ReadLine();
            DateTime endDate = new DateTime(Int32.Parse(endDateString.Split('/')[0]), Int32.Parse(endDateString.Split('/')[1]), Int32.Parse(endDateString.Split('/')[2]));
            scheduler.EndDate = endDate;

            // Define the hour slots
            string singleHour = "";
            string hoursString = "";
            Console.WriteLine("Please enter the valid debate hours.  Press enter between hours");
            Console.WriteLine("Press the 'Escape' key when the list is complete");
            do
            {
                keyInfo = Console.ReadKey();
                if (keyInfo.Key == ConsoleKey.Enter)
                {
                    Console.WriteLine(singleHour);
                    hoursString += singleHour + "|";
                    singleHour = "";
                }
                singleHour += keyInfo.KeyChar;
            } while (keyInfo.Key != ConsoleKey.Escape);

            // Configure and push the hour slots
            hoursString = hoursString.Substring(0, hoursString.Length - 1);
            Console.WriteLine("There are " + hoursString.Split('|').Length + "hour slots");
            float[] hourSlots = new float[hoursString.Split('|').Length];
            float hour = 0;
            for (int i = 0; i < hoursString.Split('|').Length; i++)
            {
                if (float.TryParse(hoursString.Split('|')[i], out hour))
                {
                    hourSlots[i] = float.Parse(hoursString.Split('|')[i]);
                }
            }
            scheduler.HourSlots = hourSlots;

            // Define number of free slots
            Console.WriteLine("Please enter the number of free slots to leave open per day");
            scheduler.FreeSlots = Int32.Parse(Console.ReadLine());
            
            // Create Schedule and get feedback
            Console.WriteLine(scheduler.CreateSchedule());
            
            /* Console feedback of schedule
            if (scheduler.scheduleMade)
            {
                Util.TimeSlot[] timeSlots = scheduler.TimeSlots;

                for (int i = 0; i < timeSlots.Length; i++)
                {
                    Console.WriteLine();
                    Console.WriteLine(timeSlots[i].timeSlot);
                    Console.WriteLine(timeSlots[i].team1Index + 1);
                    Console.WriteLine(timeSlots[i].team2Index + 1);
                }
            }
            */

            Console.WriteLine("Press any key to continue");
            Console.ReadKey();
        }
    }
}
