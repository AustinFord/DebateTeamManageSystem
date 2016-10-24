using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;

namespace DebateTeamManagementSystem.Models
{
    public class DebateContext : DbContext
    {
        public DbSet<Team> Teams { get; set; }
        public DbSet<TimeSlot> TimeSlots { get; set; }
    }

    public class Team
    {
        [Key, Display(Name = "ID")]
        [ScaffoldColumn(false)]
        public int TeamID { get; set; }

        [Required, StringLength(40), Display(Name = "Team Name")]
        public string TeamName { get; set; }
    }
    public class TimeSlot
    {
        [Key]
        public int TimeSlotID { get; set; }
        public string Team1Name { get; set; }
        public string Team2Name { get; set; }
    }
}