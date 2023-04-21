using CI_Entities1.Data;
using CI_Entities1.Models;
using CI_Entities1.Models.ViewModel;
using CI_Platform1.Models;
using CI_Project.Repository.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Data;
using System.Diagnostics;
using System.Diagnostics.Metrics;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Web;

namespace CI_Platform1.Areas.Admin.Controllers
{
    [Area("Admin")]

    public class AdminController : Controller
    {
        private readonly CiPlatformContext _CiPlatformContext;
        private readonly IUser _Interface;
        private readonly ILanding _Landing;

        public AdminController(CiPlatformContext CiPlatformContext, IUser UserInterface, ILanding landing)
        {
            _CiPlatformContext = CiPlatformContext;
            _Interface = UserInterface;
            _Landing = landing;
        }

        public IActionResult Index()
        {
            AdminVM adminVM = new AdminVM();
            adminVM.users = _CiPlatformContext.Users.ToList();
            return View(adminVM);
        }

        //Users
        public IActionResult _UserAdmin()
        {
            //.Where(u => u.DeletedAt == null)
            AdminVM adminVM = new AdminVM();
            adminVM.users = _CiPlatformContext.Users.Where(u => u.DeletedAt == null).ToList();
            adminVM.cities= _CiPlatformContext.Cities.ToList();
            adminVM.countries = _CiPlatformContext.Countries.ToList();

            return PartialView("_UserAdmin", adminVM);
        }
        [HttpPost]
        public IActionResult UserAddEdit(string img, long USERID, string first,string last, string mail, string password, string employeeid, string department, long country, long city, string profile,string status)
        {
            if(USERID ==0)
            {
                User user = new User()
                {
                    FirstName = first,
                    LastName = last,
                    Email = mail,
                    Password = password,
                    EmployeeId = employeeid,
                    Department = department,
                    ProfileText = profile,
                    Status = status,
                    CountryId = country,
                    CityId = city,
                    Avatar=img,

                };
                _CiPlatformContext.Users.Add(user);
                _CiPlatformContext.SaveChanges();
            }

            else
            {
                User user= _CiPlatformContext.Users.FirstOrDefault(x=> x.UserId == USERID);
                user.FirstName = first;
                user.LastName = last;
                user.Email = mail;
                user.Password = password;
                user.EmployeeId = employeeid;
                user.Department = department;
                user.ProfileText = profile;
                user.Status = status;   
                user.CountryId= country;
                user.CityId= city;
                if (img != null) 
                {
                    user.Avatar = img;
                }

                _CiPlatformContext.Update(user);
                _CiPlatformContext.SaveChanges();
            }

            return Json("_UserAdmin");
        }

        public IActionResult GetUserData(long USERID)
        {
            var Data = _CiPlatformContext.Users.FirstOrDefault(x => x.UserId == USERID);
            return Json(Data);
        }

        public IActionResult UserDelete(long USERID)
        {
            var user = _CiPlatformContext.Users.FirstOrDefault(u => u.UserId == USERID);
            user.DeletedAt = DateTime.Now;

            _CiPlatformContext.Users.Update(user);
            _CiPlatformContext.SaveChanges();

            return View("Index");
        }


        //CMS
        public IActionResult _CMSAdmin()
        {
            AdminVM adminVM = new AdminVM();
            adminVM.cmsPages= _CiPlatformContext.CmsPages.ToList();

            return PartialView("_CMSAdmin", adminVM);
        }

        public IActionResult CMSAdd()
        {
            return View();
        }

        [HttpPost]
        public IActionResult CMSAdd(long CMSId, String Title, String Desc, String Slug, String Status)
        {
            if(CMSId == 0)
            {
                CmsPage cms = new CmsPage()
                {
                    Title = Title,
                    Description = Desc,
                    Slug = Slug,
                    Status = Status,
                    CreatedAt = DateTime.Now,
                };

                _CiPlatformContext.Add(cms);
                _CiPlatformContext.SaveChanges();
            }
            else
            {
                CmsPage cms = _CiPlatformContext.CmsPages.FirstOrDefault(x => x.CmsPageId == CMSId);
                cms.Status = Status;
                cms.Title = Title;
                cms.Description = Desc;
                cms.Slug = Slug;
                cms.UpdatedAt = DateTime.Now;

                _CiPlatformContext.Update(cms);
                _CiPlatformContext.SaveChanges();
            }

            return Json("_CMSAdmin");
        }

        /*CMS Get Data*/
        public IActionResult GetCMSData(long CMSId)
        {
            var Data = _CiPlatformContext.CmsPages.FirstOrDefault(x => x.CmsPageId == CMSId);
            return Json(Data);
        }

        public IActionResult CMSDelete(long CMSId)
        {
            var cms = _CiPlatformContext.CmsPages.FirstOrDefault(u => u.CmsPageId == CMSId);

            _CiPlatformContext.CmsPages.Remove(cms);
            _CiPlatformContext.SaveChanges();

            return RedirectToAction("Index","Admin");
        }

        //Missions
        public IActionResult _MissionAdmin()
        {
            AdminVM adminVM = new AdminVM();
            adminVM.missions = _CiPlatformContext.Missions.ToList();

            return PartialView("_MissionAdmin", adminVM);
        }

        //Story
        public IActionResult _StoryAdmin()
        {
            AdminVM adminVM = new AdminVM();
            adminVM.stories = _CiPlatformContext.Stories.Where(u=> u.Status =="Pending").ToList();
            adminVM.missions = _CiPlatformContext.Missions.ToList();
            adminVM.users = _CiPlatformContext.Users.ToList();

            return PartialView("_StoryAdmin", adminVM);
        }

        public bool ApproveStory(long storyid)
        {
            var story = _CiPlatformContext.Stories.FirstOrDefault(u => u.StoryId == storyid);
            story.Status = "Approved";
            _CiPlatformContext.Stories.Update(story);
            _CiPlatformContext.SaveChanges();

            return true;
        }

        public bool RejectStory(long storyid)
        {
            var story = _CiPlatformContext.Stories.FirstOrDefault(u => u.StoryId == storyid);

            story.Status = "Rejected";
            _CiPlatformContext.Stories.Update(story);
            _CiPlatformContext.SaveChanges();

            return true;
        }

        public IActionResult StoryDelete(long id)
        {
            var story = _CiPlatformContext.Stories.FirstOrDefault(u => u.StoryId == id);
            var media = _CiPlatformContext.StoryMedia.FirstOrDefault(u => u.StoryId == id);

            _CiPlatformContext.StoryMedia.Remove(media);
            _CiPlatformContext.Stories.Remove(story);
            _CiPlatformContext.SaveChanges();

            return View("Index");
        }

        //MissionApplication
        public IActionResult _Applicationadmin()
        {
            AdminVM adminVM = new AdminVM();
            adminVM.missionApplications = _CiPlatformContext.MissionApplications.Where(u => u.ApprovalStatus == "Pending").ToList();
            adminVM.missions = _CiPlatformContext.Missions.ToList();
            adminVM.users = _CiPlatformContext.Users.ToList();

            return PartialView("_Applicationadmin", adminVM);
        }

        public bool ApproveApplication(long Missionapplicationid)
        {
            var missapp = _CiPlatformContext.MissionApplications.FirstOrDefault(u => u.MissionApplicationId == Missionapplicationid);
            missapp.ApprovalStatus = "Approved";

            _CiPlatformContext.MissionApplications.Update(missapp);
            _CiPlatformContext.SaveChanges();

            return true;
        }

        public bool RejectApplication(long Missionapplicationid)
        {
            var missapp = _CiPlatformContext.MissionApplications.FirstOrDefault(u => u.MissionApplicationId == Missionapplicationid);
            missapp.ApprovalStatus = "Rejected";

            _CiPlatformContext.MissionApplications.Update(missapp);
            _CiPlatformContext.SaveChanges();

            return true;
        }

        //Theme
        public IActionResult _ThemeAdmin()
        {
            AdminVM adminVM = new AdminVM();
            adminVM.missionThemes= _CiPlatformContext.MissionThemes.ToList();

            return PartialView("_ThemeAdmin", adminVM);
        }

        public IActionResult ThemeAddEdit(long Themeid, string title)
        {
            if(Themeid == 0)
            {
                MissionTheme missionTheme = new MissionTheme()
                {
                    Title = title,
                };

                _CiPlatformContext.MissionThemes.Add(missionTheme);
                _CiPlatformContext.SaveChanges();
            }

            else
            {
                MissionTheme missionTheme = _CiPlatformContext.MissionThemes.FirstOrDefault(mt=> mt.MissionThemeId == Themeid); 
                missionTheme.Title = title;

                _CiPlatformContext.MissionThemes.Update(missionTheme);
                _CiPlatformContext.SaveChanges();
            }

            return Json("_CMSAdmin");

        }

        public IActionResult GetThemeData(long Themeid)
        {
            var theme = _CiPlatformContext.MissionThemes.FirstOrDefault(t => t.MissionThemeId == Themeid);
            return Json(theme);
        }

        public IActionResult ThemeDelete(long id)
        {
            var theme = _CiPlatformContext.MissionThemes.FirstOrDefault(t => t.MissionThemeId == id);

            _CiPlatformContext.MissionThemes.Remove(theme);
            _CiPlatformContext.SaveChanges();

            return RedirectToAction("Index", "Admin");
        }

        //Skill
        public IActionResult _SkillAdmin()
        {
            AdminVM adminVM = new AdminVM();
            adminVM.skills = _CiPlatformContext.Skills.ToList();

            return PartialView("_SkillAdmin", adminVM);
        }




        //[HttpPost]
        //public IActionResult _UserAdmin(AdminVM adminVM)
        //{
        //    if (adminVM.UserId == 0)
        //    {
        //        User use = new User();

        //        use.FirstName = adminVM.UserFirstName;
        //        use.LastName = adminVM.UserLastName;
        //        use.Email = adminVM.UserEmail;
        //        use.Password = adminVM.UserPassword;
        //        use.EmployeeId = adminVM.EmployeeId;
        //        use.Department = adminVM.Department;
        //        use.CityId = adminVM.CityId;
        //        use.CountryId = adminVM.CountryId;
        //        use.Status = adminVM.Status;
        //        use.ProfileText = adminVM.ProfileText;

        //        _CiPlatformContext.Add(use);
        //        _CiPlatformContext.SaveChanges();

        //    }

        //    else
        //    {
        //        var use = _CiPlatformContext.Users.FirstOrDefault(u => u.UserId == adminVM.UserId);


        //        use.FirstName = adminVM.UserFirstName;
        //        use.LastName = adminVM.UserLastName;
        //        use.Email = adminVM.UserEmail;
        //        use.Password = adminVM.UserPassword;
        //        use.EmployeeId = adminVM.EmployeeId;
        //        use.Department = adminVM.Department;
        //        use.CityId = adminVM.CityId;
        //        use.CountryId = adminVM.CountryId;
        //        use.Status = adminVM.Status;
        //        use.ProfileText = adminVM.ProfileText;

        //        _CiPlatformContext.Update(use);
        //        _CiPlatformContext.SaveChanges();
        //    }
        //    //return PartialView("_UserAdmin", adminVM);
        //    return RedirectToAction("Index", "Admin");

        //}

    }
}
