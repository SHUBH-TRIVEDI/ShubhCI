using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CI_Entities1.Models.ViewModel
{
    public class StoryShareVM
    {
        public List<StoryMedium> StoryMediums { get; set;}
        [Required(AllowEmptyStrings = false, ErrorMessage = "Please Provide Images for the Story")]
        public List<IFormFile> attachment { get; set; }
        public long StoryId { get; set; }
        public int? StoryViews { get; set; }

        public User Singleuser { get; set; }
        public Timesheet Singlesheet { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Please Provide the mission for your story ")]
        public long MissionId { get; set; }

        public long UserId { get; set; }
        [Required(AllowEmptyStrings = false, ErrorMessage = "Please Provide Title")]

        public string? Title { get; set; }
        [Required(AllowEmptyStrings = false, ErrorMessage = "Please Provide Description for the story")]

        public string? editor1 { get; set; }

        public string? Status { get; set; }
        [Required(AllowEmptyStrings = false, ErrorMessage = "Please Provide Date of Publication")]

        public DateTime? PublishedAt { get; set; }

        public DateTime? date { get; set; }


        public DateTime CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }

        public DateTime? DeletedAt { get; set; }

        public long MissionApplicationId { get; set; }

        public DateTime AppliedAt { get; set; }

        public string ApprovalStatus { get; set; } = null!;

        public long StoryMediaId { get; set; }

        public string StoryType { get; set; } = null!;

        public string StoryPath { get; set; } = null!;

        public List<Mission> missions { get; set; }

        public List<MissionApplication> missionApplications { get; set; }

        public Story singleStory { get; set; }

        //StoryList
        public List<Story> Stories { get; set; }

        public List<City> cities { get; set; }

        public List<Country> countries { get; set; }

        public List<MissionTheme> missionThemes { get; set; }

        public List<StoryMedium> storymedia { get; set; }

        public string? FirstName { get; set; }


        //StoryDetails
        public List<User> users { get; set; }

        public int user_id { get; set; }
        public int mission_id { get; set; }

        public string username { get; set; }
        public string? ShortDescription { get; set; }

        //Story Media

        public virtual Story Story { get; set; } = null!;

        //Timesheet
        public List<Timesheet> timesheets { get; set; }

        public long TimesheetId { get; set; }

        public long hiddenid { get; set; }


        public string TimesheetTime { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Please Provide necessary Details")]
        public int? Action { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Please Provide date Volunteered")]
        public DateTime DateVolunteered { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Please Provide any specific Message")]
        public string? Notes { get; set; }
        [Required(AllowEmptyStrings = false, ErrorMessage = "Please Provide working hours")]
        public int hour { get; set; }
        [Required(AllowEmptyStrings = false, ErrorMessage = "Please Provide working minutes")]
        public int minute { get; set; }

        public virtual Mission? Mission { get; set; }

     

    }
}
