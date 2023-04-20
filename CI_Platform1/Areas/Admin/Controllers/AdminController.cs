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
            //.Where(u => u.DeletedAt == "NULL")
            AdminVM adminVM = new AdminVM();
            adminVM.users = _CiPlatformContext.Users.ToList();
            adminVM.cities= _CiPlatformContext.Cities.ToList();
            adminVM.countries = _CiPlatformContext.Countries.ToList();

            return PartialView("_UserAdmin", adminVM);
            //return RedirectToAction("Index", "Admin");
        }

        [HttpPost]
        public IActionResult _UserAdmin(AdminVM adminVM)
        {
            if (adminVM.UserId == 0)
            {
                User use = new User();

                use.FirstName = adminVM.UserFirstName;
                use.LastName = adminVM.UserLastName;
                use.Email = adminVM.UserEmail;
                use.Password = adminVM.UserPassword;
                use.EmployeeId = adminVM.EmployeeId;
                use.Department = adminVM.Department;
                use.CityId = adminVM.CityId;
                use.CountryId = adminVM.CountryId;
                use.Status = adminVM.Status;
                use.ProfileText = adminVM.ProfileText;

                _CiPlatformContext.Add(use);
                _CiPlatformContext.SaveChanges();

            }

            else
            {
                var use= _CiPlatformContext.Users.FirstOrDefault(u => u.UserId== adminVM.UserId);


                use.FirstName = adminVM.UserFirstName;
                use.LastName = adminVM.UserLastName;
                use.Email = adminVM.UserEmail;
                use.Password = adminVM.UserPassword;
                use.EmployeeId = adminVM.EmployeeId;
                use.Department = adminVM.Department;
                use.CityId = adminVM.CityId;
                use.CountryId = adminVM.CountryId;
                use.Status = adminVM.Status;
                use.ProfileText = adminVM.ProfileText;

                _CiPlatformContext.Update(use);
                _CiPlatformContext.SaveChanges();
            }
            //return PartialView("_UserAdmin", adminVM);
            return RedirectToAction("Index", "Admin");

        }

        public IActionResult UserEdit(long id)
        {
            var user= _CiPlatformContext.Users.FirstOrDefault(u=> u.UserId==id);
            AdminVM adminVM=new AdminVM();

            adminVM.cities = _CiPlatformContext.Cities.ToList();
            adminVM.countries = _CiPlatformContext.Countries.ToList();
            adminVM.UserFirstName= user.FirstName;
            adminVM.UserLastName= user.LastName;
            adminVM.UserEmail= user.Email;
            adminVM.UserPassword= user.Password;
            adminVM.EmployeeId= user.EmployeeId;
            adminVM.Department= user.Department;
            adminVM.CityId= user.CityId;
            adminVM.CountryId= user.CountryId;
            adminVM.Status= user.Status;
            adminVM.ProfileText = user.ProfileText;

            return View("Index",adminVM);
            //return RedirectToAction("Index", "Admin");

        }

        public IActionResult UserDelete(long id)
        {
            var user = _CiPlatformContext.Users.FirstOrDefault(u => u.UserId == id);
            user.DeletedAt= DateTime.Now;

            _CiPlatformContext.Users.Update(user);
            _CiPlatformContext.SaveChanges();

            return View("Index");
        }

        //Missions
        public IActionResult _MissionAdmin()
        {
            AdminVM adminVM = new AdminVM();
            adminVM.missions = _CiPlatformContext.Missions.ToList();

            return PartialView("_MissionAdmin", adminVM);
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


        public IActionResult CMSEdit(long id)
        {
            AdminVM adminVM = new AdminVM();
            var cms = _CiPlatformContext.CmsPages.FirstOrDefault(u => u.CmsPageId == id);

            adminVM.Title = cms.Title;
            adminVM.editor1 = cms.Description;
            adminVM.Slug= cms.Slug;
            adminVM.cmsStatus=cms.Status;
            adminVM.cmsid= id;
            return View(adminVM);
        }



        [HttpPost]
        public IActionResult CmsPageEdit(AdminVM adminVM, long id)
        {
            var cms = _CiPlatformContext.CmsPages.FirstOrDefault(u => u.CmsPageId == adminVM.cmsid);

            cms.Title = adminVM.Title;
            cms.Description = adminVM.editor1;
            cms.Slug = adminVM.Slug;
            cms.Status = adminVM.cmsStatus;

            _CiPlatformContext.CmsPages.Update(cms);
            _CiPlatformContext.SaveChanges();
            return RedirectToAction("Index", "Admin");
        }

        public IActionResult CMSDelete(long CMSId)
        {
            var cms = _CiPlatformContext.CmsPages.FirstOrDefault(u => u.CmsPageId == CMSId);

            _CiPlatformContext.CmsPages.Remove(cms);
            _CiPlatformContext.SaveChanges();

            return RedirectToAction("Index","Admin");
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

        //Skill
        public IActionResult _SkillAdmin()
        {
            AdminVM adminVM = new AdminVM();
            adminVM.skills = _CiPlatformContext.Skills.ToList();

            return PartialView("_SkillAdmin", adminVM);
        }
    }
}
