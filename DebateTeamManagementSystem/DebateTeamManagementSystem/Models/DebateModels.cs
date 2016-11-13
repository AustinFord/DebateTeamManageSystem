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

        [Required, Display(Name ="Active Status")]
        public bool isActive { get; set; }

        [Display(Name = "Score")]
        public int Score { get; set; }

    }
    public class TimeSlot
    {
        [Key, Display(Name = "ID")]
        [ScaffoldColumn(false)]
        public int TimeSlotID { get; set; }

        [Required, StringLength(40), Display(Name = "Team 1 Name")]
        public string Team1Name { get; set; }

        [Required, StringLength(40), Display(Name = "Team 2 Name")]
        public string Team2Name { get; set; }

        [Display(Name = "Team 1 Score")]
        public int Team1Score { get; set; }

        [Display(Name = "Team 2 Score")]
        public int Team2Score { get; set; }

        [Required, Display(Name = "Date of Debate")]
        public string date { get; set; }

        [Required, Display(Name = "Time")]
        public string time { get; set; }

        [Display(Name = "Locked")]
        public bool isLocked { get; set; }

        [Display(Name = "Round Status")]
        public string RoundStatus { get; set; }
    }
}