using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CI_Entities1.Models.ViewModel
{
    public class AdminVM
    {

        public long BannerId { get; set; }

        public IFormFile Image { get; set; }

        public string? Text { get; set; }

        public int? SortOrder { get; set; }
        public List<Banner> banners { get; set; }

        public List<Admin> admins { get; set; }
        public long AdminId { get; set; }
        public long cmsid { get; set; }
        public string? FirstName { get; set; }

        public string? LastName { get; set; }

        public string Email { get; set; } = null!;

        public string Password { get; set; } = null!;

        public DateTime CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }

        public DateTime? DeletedAt { get; set; }

        //_User.cshtml

        public List<User> users { get; set; }

        public string? UserFirstName { get; set; }

        public string? UserLastName { get; set; }

        public string? UserEmail { get; set; }

        public string? EmployeeId { get; set; }
        public string? Department { get; set; }

        public string Status { get; set; } = null!;

        public string UserPassword { get; set; } = null!;

        public long? CityId { get; set; }

        public long? CountryId { get; set; }

        public List<Country> countries { get; set; }

        public List<City> cities { get; set; }

        public string? ProfileText { get; set; }

        //_AdminMission.cshtml

        public List<Mission> missions { get; set; }

        public string Title { get; set; }
        public string MissionType { get; set; }
        public DateTime? StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        public string? MissionStatus { get; set; }

        //_StoryAdmin
        public List<Story> stories { get; set; }

        public long MissionId { get; set; }

        public long UserId { get; set; }

        public string? StoryTitle { get; set; }

        public List<StoryMedium> storyMedia { get; set; }

        //_ApplicationAdmin
        public List<MissionApplication> missionApplications { get; set; }

        public DateTime AppliedAt { get; set; }

        public string ApprovalStatus { get; set; } = null!;
        
        public List<MissionTheme> missionThemes { get; set; }

        public List<Skill> skills { get; set; }

        
        //CMSPAGE
        public List<CmsPage> cmsPages { get; set; }
        public long CmsPageId { get; set; }

        public string? editor1 { get; set; }

        public string Slug { get; set; } = null!;

        public string? cmsStatus { get; set; }


    }
}
