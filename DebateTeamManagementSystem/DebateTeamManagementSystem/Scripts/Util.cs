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
        // Struct to hold all debate schedules, the teams involved, and the scores of each team
        // Might eventually hold the referee assigned to the debate
        public struct TimeSlot
        {
            public string team1Name, team2Name, date, time;
            public int team1Index, team1Score;
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
        public static T[] ExtendArray<T> (T[] array, int extensions)
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
        public static T[] RemoveAt<T> (T[] array, int index)
        {
            // Worker boolean to identify the removed index and modify the copy algorithm
            bool removed = false;
            // New Array
            T[] tempArray = new T[array.Length-1];
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
    }
}