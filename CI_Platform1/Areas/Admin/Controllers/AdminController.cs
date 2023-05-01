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
        private readonly IAdmin _Admin;


        public AdminController(CiPlatformContext CiPlatformContext,IAdmin AdminInterface, IUser UserInterface, ILanding landing)
        {
            _CiPlatformContext = CiPlatformContext;
            _Interface = UserInterface;
            _Landing = landing;
            _Admin = AdminInterface;
        }

        public IActionResult Index()
        {
            var data = _Admin.GetUserData();

            return View(data);
        }

        //Users
        public IActionResult _UserAdmin()
        {
            var data = _Admin.GetUserData();

            return PartialView("_UserAdmin", data);
        }
        [HttpPost]
        public IActionResult UserAddEdit(string img, long USERID, string first,string last, string mail, string password, string employeeid, string department, long country, long city, string profile,string status)
        {
            var data = _Admin.PostUserData(img, USERID, first, last, mail, password, employeeid, department, country, city, profile, status);
            return Json("_UserAdmin");
        }

        public IActionResult GetUserData(long USERID)
        {
            var data = _Admin.GetUserEditData(USERID);
            //var Data = _CiPlatformContext.Users.FirstOrDefault(x => x.UserId == USERID);
            return Json(data);
        }

        public IActionResult UserDelete(long USERID)
        {
            _Admin.UserDelete(USERID);

            return View("Index");
        }


        //CMS
        public IActionResult _CMSAdmin()
        {
            var data= _Admin.GetCMSData();



            return PartialView("_CMSAdmin", data);
        }

        public IActionResult CMSAdd()
        {
            return View();
        }

        [HttpPost]
        public IActionResult CMSAdd(long CMSId, String Title, String Desc, String Slug, String Status)
        {
            var data = _Admin.PostCMSData(CMSId, Title, Desc, Slug, Status);

            return Json("_CMSAdmin");
        }

        /*CMS Get Data*/
        public IActionResult GetCMSData(long CMSId)
        {
            var Data = _Admin.GetCMSEditData(CMSId);
            return Json(Data);
        }

        public IActionResult CMSDelete(long CMSId)
        {
            _Admin.CMSDelete(CMSId);

            return RedirectToAction("Index","Admin");
        }

        //Missions
        public IActionResult _MissionAdmin()
        {
            var data = _Admin.GetMissionData();

            return PartialView("_MissionAdmin", data);
        }

        public IActionResult MissionAddEdit(string title, string shortdesc, string description,string orgname, int country, int city,string orgdetail,
            string misstype,int themeid, DateTime start , DateTime end, string avail,  int MISSIONID,int skill)
        {
            var data = _Admin.PostMissionData(title, shortdesc, description, orgname, country, city, orgdetail, misstype, themeid, start, end, avail, MISSIONID, skill);

            if (MISSIONID == 0)
            {
                var skills = _Admin.PostSkillData(skill);
            }
            else
            {
                var skills = _Admin.PostSkillEditData(skill);
            }
            return Json("_MissionAdmin");
        }

        public IActionResult MissionDelete(long MISSIONID)
        {
            _Admin.MissionDelete(MISSIONID);

            return RedirectToAction("Index", "Admin");
        }

        public IActionResult GetMissionData(long MISSIONID)
        {
            var data= _Admin.GetMissionEditData(MISSIONID);
            return Json(data);
        }

        //Story
        public IActionResult _StoryAdmin()
        {
            var data = _Admin.GetStoryData();

            return PartialView("_StoryAdmin", data);
        }

        public bool ApproveStory(long storyid)
        {
            _Admin.ApproveStory(storyid);

            return true;
        }

        public bool RejectStory(long storyid)
        {
            _Admin.RejectStory(storyid);

            return true;
        }

        public IActionResult StoryDelete(long id)
        {
            _Admin.StoryDelete(id);

            return View("Index");
        }

        //MissionApplication
        public IActionResult _Applicationadmin()
        {
            var data = _Admin.GetApplicationData();
            return PartialView("_Applicationadmin", data);
        }

        public bool ApproveApplication(long Missionapplicationid)
        {
            _Admin.ApproveApplication(Missionapplicationid);
            return true;
        }

        public bool RejectApplication(long Missionapplicationid)
        {
            _Admin.RejectApplication(Missionapplicationid);
            return true;
        }

        //Theme
        public IActionResult _ThemeAdmin()
        {
            var data= _Admin.GetThemeData();
            return PartialView("_ThemeAdmin", data);
        }

        public IActionResult ThemeAddEdit(long Themeid, string title,byte status)
        {
            var data = _Admin.ThemeAddEdit(Themeid, title, status);

            return Json("_CMSAdmin");
        }

        public IActionResult GetThemeData(long Themeid)
        {
            var theme = _Admin.GetThemeEditData(Themeid);
            return Json(theme);
        }

        public IActionResult ThemeDelete(long id)
        {
            _Admin.ThemeDelete(id);
            return RedirectToAction("Index", "Admin");
        }

        //Skill
        public IActionResult _SkillAdmin()
        {
            var data = _Admin.GetSkillData();
            return PartialView("_SkillAdmin", data);
        }

        public IActionResult SkillAddEdit(string status, string title, long SkillId)
        {
            var data = _Admin.SkillAddEdit(status, title, SkillId);
            return Json("_SkillAdmin");
        }

        public IActionResult GetSkillData(long SkillId)
        {
            var skill = _Admin.GetSkillEditData(SkillId);
            return Json(skill);
        }

        public IActionResult SkillDelete(long SkillId)
        {
            _Admin.SkillDelete(SkillId);
            return Json("_SkillAdmin");
        }

        //Banner Management
        public IActionResult _BannerAdmin()
        {
            var data = _Admin.GetAdminData();
            return PartialView("_BannerAdmin", data);
        }

        [HttpPost]
        public IActionResult _BannerAdmin(AdminVM adminVM)
        {
            var data = _Admin.AddEditAdmin(adminVM);
            //There may be issuue in passing data
            return View("Index");
        }

        public IActionResult BannerEdit(long id)
        {
            var data = _Admin.BannerEdit(id);
            return View("_BannerAdmin", data);
        }

        public IActionResult BannerDelete(long id)
        {
            _Admin.BannerDelete(id);
            return View("Index");
        }
    }
}
