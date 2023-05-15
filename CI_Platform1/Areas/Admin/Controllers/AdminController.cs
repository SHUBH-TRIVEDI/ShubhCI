using CI_Entities1.Data;
using CI_Entities1.Models;
using CI_Entities1.Models.ViewModel;
using CI_Project.Repository.Interface;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using System.Text.Json.Serialization;

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
        public IActionResult UserAddEdit( string avatar, string img, long USERID, string first,string last, string mail, string password, string employeeid, string department, long country, long city, string profile,string status)
        {
            var data = _Admin.PostUserData(avatar, img, USERID, first, last, mail, password, employeeid, department, country, city, profile, status);
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

            return RedirectToAction("Index");
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

        public IActionResult MissionAddEdit(string[] images,int seats,DateTime deadline, string title, string shortdesc, string description,string orgname, int country, int city,string orgdetail,
            string misstype,int themeid, DateTime start , DateTime end, string avail,  int MISSIONID,int skill)
        {
            if (MISSIONID == 0)
            {
                Mission mission = new Mission()
                {
                    Title = title,
                    ShortDescription = shortdesc,
                    Description = description,
                    OrganizationName = orgname,
                    OrganizationDetail = orgdetail,
                    CountryId = country,
                    CityId = city,
                    MissionType = misstype,
                    ThemeId = themeid,
                    StartDate = start,
                    EndDate = end,
                    Availability = avail,
                    Deadline=deadline,
                    Seatleft = seats,
                };
                _CiPlatformContext.Missions.Add(mission);
                _CiPlatformContext.SaveChanges();

                MissionSkill missionSkill = new MissionSkill()
                {
                    SkillId = skill,
                    MissionId = mission.MissionId,
                };
                _CiPlatformContext.MissionSkills.Add(missionSkill);
                _CiPlatformContext.SaveChanges();

                if (images.Length != 0)
                {

                    foreach (var i in images)
                    {
                        MissionMedium missionMedium = new MissionMedium();

                        missionMedium.MediaPath = i;
                        missionMedium.MissionId = mission.MissionId;

                        _CiPlatformContext.MissionMedia.Add(missionMedium);
                        _CiPlatformContext.SaveChanges();
                    }
                }
            }


            else
            {
                var mission = _CiPlatformContext.Missions.FirstOrDefault(miss => miss.MissionId == MISSIONID);

                mission.Title = title;
                mission.ShortDescription = shortdesc;
                mission.Description = description;
                mission.OrganizationName = orgname;
                mission.OrganizationDetail = orgdetail;
                mission.CountryId = country;
                mission.CityId = city;
                mission.MissionType = misstype;
                mission.ThemeId = themeid;
                mission.StartDate = start;
                mission.EndDate = end;
                mission.Availability = avail;
                mission.UpdatedAt = DateTime.Now;
                mission.Seatleft = seats;
                mission.Deadline = deadline;

                _CiPlatformContext.Missions.Update(mission);
                _CiPlatformContext.SaveChanges();


                var missskill = _CiPlatformContext.MissionSkills.Where(u => u.MissionId == MISSIONID).ToList();
                _CiPlatformContext.RemoveRange(missskill);
                _CiPlatformContext.SaveChanges();

                MissionSkill missionSkill = new MissionSkill()
                {
                    SkillId = skill,
                    MissionId = mission.MissionId,
                };
                _CiPlatformContext.MissionSkills.Add(missionSkill);
                _CiPlatformContext.SaveChanges();

                if (images.Length != 0)
                {
                    var missionMedium = _CiPlatformContext.MissionMedia.Where(u => u.MissionId == MISSIONID).ToList();
                    _CiPlatformContext.RemoveRange(missionMedium);
                    _CiPlatformContext.SaveChanges();

                    foreach (var i in images)
                    {
                        MissionMedium missionMedium1 = new MissionMedium();

                        missionMedium1.MediaPath = i;
                        missionMedium1.MissionId = mission.MissionId;

                        _CiPlatformContext.MissionMedia.Add(missionMedium1);
                        _CiPlatformContext.SaveChanges();
                    }
                }
            }

            return Json("_MissionAdmin");
        }

        public IActionResult MissionDelete(long MISSIONID)
        {
            _Admin.MissionDelete(MISSIONID);

            return RedirectToAction("Index", "Admin");
        }

        //Cascading for city and country
        public JsonResult filterCity(long missionCountry)
        {
            IList<City> cities = _CiPlatformContext.Cities.Where(m => m.CountryId == missionCountry).ToList();
            return Json(cities);
        }

        public IActionResult GetMissionData(long MISSIONID)
        {
            var data= _Admin.GetMissionEditData(MISSIONID);
            var media = _Admin.GetMissionmediumData(MISSIONID);
            var missionData = new
            {
                data1 = data,
                media1= media,
            };
            var options = new JsonSerializerOptions
            {
                ReferenceHandler = ReferenceHandler.Preserve,
                MaxDepth = 1024
            };
            var json = System.Text.Json.JsonSerializer.Serialize(data, options);
            return Json(json);
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
            return RedirectToAction("Index", "Admin");
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
            return RedirectToAction("Index", "Admin");
        }

        public IActionResult BannerEdit(long id)
        {
            var data = _Admin.BannerEdit(id);
            return PartialView("_BannerAdmin", data);
        }

        public IActionResult BannerDelete(long id)
        {
            _Admin.BannerDelete(id);
           
            return RedirectToAction("Index", "Admin");

        }
    }
}
