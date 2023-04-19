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

        public IActionResult _UserAdmin()
        {
            AdminVM adminVM = new AdminVM();
            adminVM.users = _CiPlatformContext.Users.ToList();

            return PartialView("_UserAdmin", adminVM);
        }

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
        public IActionResult CMSAdd(AdminVM adminVM)
        {
            CmsPage cms = new CmsPage();

            cms.Title= adminVM.Title;
            cms.Description= adminVM.editor1;
            cms.Slug= adminVM.Slug;
            cms.Status= adminVM.cmsStatus;

            _CiPlatformContext.Add(cms);
            _CiPlatformContext.SaveChanges();
            return RedirectToAction("Index", "Admin");
        }

        public IActionResult CMSEdit(long id)
        {
            AdminVM adminVM = new AdminVM();
            var cms = _CiPlatformContext.CmsPages.FirstOrDefault(u => u.CmsPageId == id);

            adminVM.Title = cms.Title;
            adminVM.editor1 = cms.Description;
            adminVM.Slug= cms.Slug;
            adminVM.cmsStatus=cms.Status;
            adminVM.cmsid=id;
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

        public IActionResult CMSDelete(long id)
        {
            var cms = _CiPlatformContext.CmsPages.FirstOrDefault(u => u.CmsPageId == id);

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
