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
            adminVM.users = _CiPlatformContext.Users.Where(u => u.DeletedAt == null).ToList();
            adminVM.countries = _CiPlatformContext.Countries.ToList();
            adminVM.cities = _CiPlatformContext.Cities.ToList();
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
                user.UpdatedAt = DateTime.Now;
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
            adminVM.missions = _CiPlatformContext.Missions.Where(u => u.DeletedAt == null).ToList();
            adminVM.cities = _CiPlatformContext.Cities.ToList();
            adminVM.countries = _CiPlatformContext.Countries.ToList();
            adminVM.skills = _CiPlatformContext.Skills.ToList();
            adminVM.missionThemes = _CiPlatformContext.MissionThemes.ToList();

            return PartialView("_MissionAdmin", adminVM);
        }

        public IActionResult MissionAddEdit(string title, string shortdesc, string description,string orgname, int country, int city,string orgdetail,
            string misstype,int themeid, DateTime start , DateTime end, string avail,  int MISSIONID,int skill)
        {

            if(MISSIONID==0)
            {
                Mission mission = new Mission()
                {
                    Title = title,
                    ShortDescription = shortdesc,
                    Description= description,
                    OrganizationName = orgname,
                    OrganizationDetail= orgdetail,
                    CountryId=country,
                    CityId= city,
                    MissionType=misstype,
                    ThemeId= themeid,
                    StartDate=start,
                    EndDate=end,
                    Availability= avail,
                };
                _CiPlatformContext.Missions.Add(mission);
                _CiPlatformContext.SaveChanges();

                MissionSkill missionSkill = new MissionSkill()
                {
                    MissionSkillId = skill,
                };
                _CiPlatformContext.MissionSkills.Add(missionSkill);
                _CiPlatformContext.SaveChanges();
            }

            else
            {
                var mission = _CiPlatformContext.Missions.FirstOrDefault(miss => miss.MissionId == MISSIONID);
                //var skills= _CiPlatformContext.MissionSkills.FirstOrDefault(skill=> skill.SkillId==)

                mission.Title= title;
                mission.ShortDescription= shortdesc;    
                mission.Description= description;
                mission.OrganizationName= orgname;
                mission.OrganizationDetail= orgdetail;
                mission.CountryId= country;
                mission.CityId= city;
                mission.MissionType= misstype;
                mission.ThemeId= themeid;
                mission.StartDate= start;
                mission.EndDate= end;
                mission.Availability= avail;
                mission.UpdatedAt=DateTime.Now;

                _CiPlatformContext.Missions.Update(mission);
                _CiPlatformContext.SaveChanges();
            }

            return Json("_MissionAdmin");


        }

        public IActionResult MissionDelete(long MISSIONID)
        {
            var mission = _CiPlatformContext.Missions.FirstOrDefault(miss => miss.MissionId == MISSIONID);
            mission.DeletedAt = DateTime.Now;


            var goal = _CiPlatformContext.GoalMissions.Where(miss => miss.MissionId == MISSIONID).ToList();
            foreach(var i in goal)
            {
                i.DeletedAt = DateTime.Now;
            }

            var favrouite = _CiPlatformContext.FavoriteMissions.Where(miss => miss.MissionId == MISSIONID);
            foreach(var i in favrouite)
            {
                i.DeletedAt= DateTime.Now;
            }

            _CiPlatformContext.Missions.Update(mission);
            _CiPlatformContext.SaveChanges();

            return RedirectToAction("Index", "Admin");

        }

        public IActionResult GetMissionData(long MISSIONID)
        {
            var mission= _CiPlatformContext.Missions.FirstOrDefault(miss=> miss.MissionId == MISSIONID);
            return Json(mission);

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

        public IActionResult ThemeAddEdit(long Themeid, string title,byte status)
        {
            if(Themeid == 0)
            {
                MissionTheme missionTheme = new MissionTheme()
                {
                    Title = title,
                    Status=status,
                };

                _CiPlatformContext.MissionThemes.Add(missionTheme);
                _CiPlatformContext.SaveChanges();
            }

            else
            {
                MissionTheme missionTheme = _CiPlatformContext.MissionThemes.FirstOrDefault(mt=> mt.MissionThemeId == Themeid); 
                missionTheme.Title = title;
                missionTheme.Status = status;
                missionTheme.UpdatedAt = DateTime.Now;

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

        public IActionResult SkillAddEdit(string status, string title, long SkillId)
        {
            if (SkillId == 0)
            {
                Skill skills = new Skill()
                {
                    Status = status,
                    SkillName = title,
                };
                _CiPlatformContext.Skills.Add(skills);
            }

            else
            {
                var skill= _CiPlatformContext.Skills.FirstOrDefault(u=> u.SkillId == SkillId);

                skill.Status = status;
                skill.SkillName= title;
                skill.UpdatedAt = DateTime.Now;

                _CiPlatformContext.Skills.Update(skill);

            }
            _CiPlatformContext.SaveChanges();

            return Json("_SkillAdmin");


        }

        public IActionResult GetSkillData(long SkillId)
        {
            var skill = _CiPlatformContext.Skills.FirstOrDefault(u => u.SkillId == SkillId);
            return Json(skill);
        }

        public IActionResult SkillDelete(long SkillId)
        {
            var skill = _CiPlatformContext.Skills.FirstOrDefault(u => u.SkillId == SkillId);

            _CiPlatformContext.Skills.Remove(skill);
            _CiPlatformContext.SaveChanges();

            return Json("_SkillAdmin");
        }

        //Banner Management

        public IActionResult _BannerAdmin()
        {
            AdminVM adminVM = new AdminVM();
            adminVM.banners = _CiPlatformContext.Banners.ToList();

            return PartialView("_BannerAdmin", adminVM);
        }

        [HttpPost]
        public async Task<IActionResult> _BannerAdminAsync(AdminVM adminVM)
        {
            Banner banner = new Banner();

            banner.Text = adminVM.Text;
            banner.SortOrder = adminVM.SortOrder;

            var FileName = "";
            using (var ms = new MemoryStream())
            {
                await adminVM.Image.CopyToAsync(ms);
                var imageBytes = ms.ToArray();
                var base64String = Convert.ToBase64String(imageBytes);
                FileName = "data:image/png;base64," + base64String;
            }

            banner.Image= FileName;

            _CiPlatformContext.Banners.Add(banner);
            _CiPlatformContext.SaveChanges();

            return View("Index");
        }

        //public IActionResult BannerAddEdit(long BANNERID, string bannertext, int order, IFormFile fileInput)
        //{
        //    if(BANNERID ==0)
        //    {
        //        Banner banner = new Banner()
        //        { 
        //            //Image= fileInput,
        //            Text =bannertext,
        //            SortOrder = order,
        //        };

        //        _CiPlatformContext.Banners.Add(banner);
        //        _CiPlatformContext.SaveChanges();
        //    }

        //    return Json("_BannerAdmin");
        //}

    }
}
