using CI_Entities1.Data;
using CI_Entities1.Models;
using CI_Entities1.Models.ViewModel;
using CI_Project.Repository.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CI_Project.Repository.Repository
{
    public class AdminRepository : IAdmin
    {

        private readonly CiPlatformContext _CiPlatformContext;
        public AdminRepository(CiPlatformContext db)
        {
            _CiPlatformContext = db;
        }

        //USER
        public AdminVM GetUserData()
        {
            AdminVM adminVM = new AdminVM();
            adminVM.users = _CiPlatformContext.Users.Where(u => u.DeletedAt == null).ToList();
            adminVM.countries = _CiPlatformContext.Countries.ToList();
            adminVM.cities = _CiPlatformContext.Cities.ToList();

            return adminVM;
        }

        public User PostUserData(string avatar, string img, long USERID, string first, string last, string mail, string password, string employeeid, string department, long country, long city, string profile, string status)
        {
            if (USERID == 0)
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
                    Avatar = avatar,

                };
                _CiPlatformContext.Users.Add(user);
                _CiPlatformContext.SaveChanges();

                return user;
            }

            else
            {
                User user = _CiPlatformContext.Users.FirstOrDefault(x => x.UserId == USERID);
                user.FirstName = first;
                user.LastName = last;
                user.Email = mail;
                user.Password = password;
                user.EmployeeId = employeeid;
                user.Department = department;
                user.ProfileText = profile;
                user.Status = status;
                user.CountryId = country;
                user.CityId = city;
                user.Avatar = avatar;
                user.UpdatedAt = DateTime.Now;
                if (img != null)
                {
                    user.Avatar = img;
                }

                _CiPlatformContext.Users.Update(user);
                _CiPlatformContext.SaveChanges();

                return user;
            }
        }

        public User GetUserEditData(long USERID)
        {
            return (_CiPlatformContext.Users.FirstOrDefault(x => x.UserId == USERID));
        }

        public void UserDelete(long USERID)
        {
            var user = _CiPlatformContext.Users.FirstOrDefault(u => u.UserId == USERID);
            user.DeletedAt = DateTime.Now;

            var applied = _CiPlatformContext.MissionApplications.Where(app => app.UserId == USERID);
            foreach (var i in applied)
            {
                i.DeletedAt = DateTime.Now;
            }

            var comment = _CiPlatformContext.Comments.Where(x => x.UserId == USERID);
            foreach (var item in comment)
            {
                item.DeletedAt = DateTime.Now;
            }


            _CiPlatformContext.Users.Update(user);
            _CiPlatformContext.SaveChanges();
        }

        //CMS

        public AdminVM GetCMSData()
        {
            AdminVM adminVM = new AdminVM();
            adminVM.cmsPages = _CiPlatformContext.CmsPages.ToList();

            return adminVM;
        }

        public CmsPage PostCMSData(long CMSId, String Title, String Desc, String Slug, String Status)
        {
            if (CMSId == 0)
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

                return cms;
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

                return cms;
            }
        }

        public CmsPage GetCMSEditData(long CMSId)
        {
            return _CiPlatformContext.CmsPages.FirstOrDefault(x => x.CmsPageId == CMSId);
        }

        public void CMSDelete(long CMSId)
        {
            var cms = _CiPlatformContext.CmsPages.FirstOrDefault(u => u.CmsPageId == CMSId);

            _CiPlatformContext.CmsPages.Remove(cms);
            _CiPlatformContext.SaveChanges();
        }

        //Misssions
        public AdminVM GetMissionData()
        {
            AdminVM adminVM = new AdminVM();
            adminVM.missions = _CiPlatformContext.Missions.Where(u => u.DeletedAt == null).ToList();
            adminVM.cities = _CiPlatformContext.Cities.ToList();
            adminVM.countries = _CiPlatformContext.Countries.ToList();
            adminVM.skills = _CiPlatformContext.Skills.ToList();
            adminVM.missionThemes = _CiPlatformContext.MissionThemes.ToList();

            return adminVM;
        }

        public MissionSkill PostSkillData(int skill)
        {
            MissionSkill missionSkill = new MissionSkill()
            {
                MissionSkillId = skill,
            };
            _CiPlatformContext.MissionSkills.Add(missionSkill);
            _CiPlatformContext.SaveChanges();

            return missionSkill;
        }

        public MissionSkill PostSkillEditData(int skill)
        {
            MissionSkill missionSkill = _CiPlatformContext.MissionSkills.FirstOrDefault(u => u.SkillId == skill);
            missionSkill.SkillId = skill;

            _CiPlatformContext.MissionSkills.Update(missionSkill);
            _CiPlatformContext.SaveChanges();

            return missionSkill;
        }

        public Mission PostMissionData(string title, string shortdesc, string description, string orgname, int country, int city, string orgdetail, string misstype, int themeid, DateTime start, DateTime end, string avail, int MISSIONID, int skill)
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
                };
                _CiPlatformContext.Missions.Add(mission);
                _CiPlatformContext.SaveChanges();

                return mission;
            }

            else
            {
                var mission = _CiPlatformContext.Missions.FirstOrDefault(miss => miss.MissionId == MISSIONID);
                //var skills= _CiPlatformContext.MissionSkills.FirstOrDefault(skill=> skill.SkillId==)

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

                _CiPlatformContext.Missions.Update(mission);
                _CiPlatformContext.SaveChanges();
                return mission;
            }
        }

        public void MissionDelete(long MISSIONID)
        {
            var mission = _CiPlatformContext.Missions.FirstOrDefault(miss => miss.MissionId == MISSIONID);
            mission.DeletedAt = DateTime.Now;


            var goal = _CiPlatformContext.GoalMissions.Where(miss => miss.MissionId == MISSIONID).ToList();
            foreach (var i in goal)
            {
                i.DeletedAt = DateTime.Now;

                _CiPlatformContext.GoalMissions.Update(i);
                _CiPlatformContext.SaveChanges();
            }

            var favrouite = _CiPlatformContext.FavoriteMissions.Where(miss => miss.MissionId == MISSIONID);
            foreach (var i in favrouite)
            {
                i.DeletedAt = DateTime.Now;
                _CiPlatformContext.FavoriteMissions.Update(i);
                _CiPlatformContext.SaveChanges();
            }

            var media = _CiPlatformContext.MissionMedia.Where(miss => miss.MissionId == MISSIONID);
            foreach (var i in media)
            {
                i.DeletedAt = DateTime.Now;
                _CiPlatformContext.MissionMedia.Update(i);
                _CiPlatformContext.SaveChanges();
            }

            var comment = _CiPlatformContext.Comments.Where(miss => miss.MissionId == MISSIONID);
            foreach (var i in comment)
            {
                i.DeletedAt = DateTime.Now;
            _CiPlatformContext.Comments.Update(i);
            _CiPlatformContext.SaveChanges();
            }


            var rating = _CiPlatformContext.MissionRatings.Where(miss => miss.MissionId == MISSIONID);
            foreach (var i in rating)
            {
                i.DeletedAt = DateTime.Now;
                _CiPlatformContext.MissionRatings.Update(i);
                _CiPlatformContext.SaveChanges();
            }
        }

        public Mission GetMissionEditData(long MISSIONID)
        {
            return _CiPlatformContext.Missions.FirstOrDefault(miss => miss.MissionId == MISSIONID);
        }

        public IEnumerable<MissionMedium> GetMissionmediumData(long MISSIONID)
        {
            return _CiPlatformContext.MissionMedia.Where(u => u.MissionId == MISSIONID).ToList();
        }

        //story

        public AdminVM GetStoryData()
        {
            AdminVM adminVM = new AdminVM();
            adminVM.stories = _CiPlatformContext.Stories.Where(u => u.Status == "Pending" && u.DeletedAt == null).ToList();
            adminVM.missions = _CiPlatformContext.Missions.ToList();
            adminVM.users = _CiPlatformContext.Users.ToList();

            return adminVM;
        }

        public void ApproveStory(long storyid)
        {
            var story = _CiPlatformContext.Stories.FirstOrDefault(u => u.StoryId == storyid);
            story.Status = "Approved";
            _CiPlatformContext.Stories.Update(story);
            _CiPlatformContext.SaveChanges();
        }

        public void RejectStory(long storyid)
        {
            var story = _CiPlatformContext.Stories.FirstOrDefault(u => u.StoryId == storyid);

            story.Status = "Rejected";
            _CiPlatformContext.Stories.Update(story);
            _CiPlatformContext.SaveChanges();
        }


        public void StoryDelete(long id)
        {
            var story = _CiPlatformContext.Stories.FirstOrDefault(u => u.StoryId == id);
            var media = _CiPlatformContext.StoryMedia.FirstOrDefault(u => u.StoryId == id);

            story.DeletedAt = DateTime.Now;
            media.DeletedAt = DateTime.Now;

            _CiPlatformContext.StoryMedia.Update(media);
            _CiPlatformContext.Stories.Update(story);
            _CiPlatformContext.SaveChanges();
        }


        //MissionApplication
        public AdminVM GetApplicationData()
        {
            AdminVM adminVM = new AdminVM();
            adminVM.missionApplications = _CiPlatformContext.MissionApplications.Where(u => u.ApprovalStatus == "Pending").ToList();
            adminVM.missions = _CiPlatformContext.Missions.ToList();
            adminVM.users = _CiPlatformContext.Users.ToList();

            return adminVM;
        }

        public void ApproveApplication(long Missionapplicationid)
        {
            var missapp = _CiPlatformContext.MissionApplications.FirstOrDefault(u => u.MissionApplicationId == Missionapplicationid);
            missapp.ApprovalStatus = "Approved";

            _CiPlatformContext.MissionApplications.Update(missapp);
            _CiPlatformContext.SaveChanges();
        }

        public void RejectApplication(long Missionapplicationid)
        {
            var missapp = _CiPlatformContext.MissionApplications.FirstOrDefault(u => u.MissionApplicationId == Missionapplicationid);
            missapp.ApprovalStatus = "Rejected";

            _CiPlatformContext.MissionApplications.Update(missapp);
            _CiPlatformContext.SaveChanges();
        }

        //Theme
        public AdminVM GetThemeData()
        {
            AdminVM adminVM = new AdminVM();
            adminVM.missionThemes = _CiPlatformContext.MissionThemes.ToList();

            return adminVM;
        }

        public MissionTheme ThemeAddEdit(long Themeid, string title, byte status)
        {
            if (Themeid == 0)
            {
                MissionTheme missionTheme = new MissionTheme()
                {
                    Title = title,
                    Status = status,
                };

                _CiPlatformContext.MissionThemes.Add(missionTheme);
                _CiPlatformContext.SaveChanges();

                return missionTheme;
            }

            else
            {
                MissionTheme missionTheme = _CiPlatformContext.MissionThemes.FirstOrDefault(mt => mt.MissionThemeId == Themeid);
                missionTheme.Title = title;
                missionTheme.Status = status;
                missionTheme.UpdatedAt = DateTime.Now;

                _CiPlatformContext.MissionThemes.Update(missionTheme);
                _CiPlatformContext.SaveChanges();

                return missionTheme;
            }
        }

        public MissionTheme GetThemeEditData(long Themeid)
        {
            return _CiPlatformContext.MissionThemes.FirstOrDefault(t => t.MissionThemeId == Themeid);
        }

        public void ThemeDelete(long id)
        {
            var theme = _CiPlatformContext.MissionThemes.FirstOrDefault(t => t.MissionThemeId == id);

            _CiPlatformContext.MissionThemes.Remove(theme);
            _CiPlatformContext.SaveChanges();
        }


        //Skill
        public AdminVM GetSkillData()
        {
            AdminVM adminVM = new AdminVM();
            adminVM.skills = _CiPlatformContext.Skills.Where(u => u.DeletedAt == null).ToList();

            return adminVM;
        }

        public Skill SkillAddEdit(string status, string title, long SkillId)
        {
            if (SkillId == 0)
            {
                Skill skills = new Skill()
                {
                    Status = status,
                    SkillName = title,
                };
                _CiPlatformContext.Skills.Add(skills);
                _CiPlatformContext.SaveChanges();

                return skills;
            }

            else
            {
                var skill = _CiPlatformContext.Skills.FirstOrDefault(u => u.SkillId == SkillId);

                skill.Status = status;
                skill.SkillName = title;
                skill.UpdatedAt = DateTime.Now;

                _CiPlatformContext.Skills.Update(skill);
                _CiPlatformContext.SaveChanges();

                return skill;
            }
        }

        public Skill GetSkillEditData(long SkillId)
        {
            return _CiPlatformContext.Skills.FirstOrDefault(u => u.SkillId == SkillId);
        }

        public void SkillDelete(long SkillId)
        {
            var skill = _CiPlatformContext.Skills.FirstOrDefault(u => u.SkillId == SkillId);
            skill.DeletedAt = DateTime.Now;

            _CiPlatformContext.Skills.Remove(skill);
            //_CiPlatformContext.Skills.Update(skill);
            _CiPlatformContext.SaveChanges();
        }

        public AdminVM GetAdminData()
        {
            AdminVM adminVM = new AdminVM();
            adminVM.banners = _CiPlatformContext.Banners.Where(u => u.DeletedAt == null).ToList();
            return adminVM;
        }

        public Banner AddEditAdmin(AdminVM adminVM)
        {
            if (adminVM.BannerId == 0)
            {
                Banner banner = new Banner();

                banner.Text = adminVM.Text;
                banner.SortOrder = adminVM.SortOrder;

                var FileName = "";
                using (var ms = new MemoryStream())
                {
                    adminVM.Image.CopyToAsync(ms);
                    var imageBytes = ms.ToArray();
                    var base64String = Convert.ToBase64String(imageBytes);
                    FileName = "data:image/png;base64," + base64String;
                }

                banner.Image = FileName;

                _CiPlatformContext.Banners.Add(banner);
                _CiPlatformContext.SaveChanges();

                return banner;
            }

            else
            {
                var banner = _CiPlatformContext.Banners.FirstOrDefault(u => u.BannerId == adminVM.BannerId);
                banner.Text = adminVM.Text;
                banner.SortOrder = adminVM.SortOrder;


                if (adminVM.Image != null)
                {
                    var FileName = "";
                    using (var ms = new MemoryStream())
                    {
                        adminVM.Image.CopyToAsync(ms);
                        var imageBytes = ms.ToArray();
                        var base64String = Convert.ToBase64String(imageBytes);
                        FileName = "data:image/png;base64," + base64String;
                    }

                    banner.Image = FileName;
                }
                _CiPlatformContext.Banners.Update(banner);
                _CiPlatformContext.SaveChanges();

                return banner;
            }
        }

        public AdminVM BannerEdit(long id)
        {
            AdminVM adminVM = new AdminVM();
            adminVM.banners = _CiPlatformContext.Banners.ToList();
            var banner = _CiPlatformContext.Banners.FirstOrDefault(u => u.BannerId == id);

            adminVM.Text = banner.Text;
            adminVM.SortOrder = banner.SortOrder;
            adminVM.getimage = banner.Image;
            adminVM.BannerId = id;

            return adminVM;
        }


        public void BannerDelete(long id)
        {
            var banner = _CiPlatformContext.Banners.FirstOrDefault(ban => ban.BannerId == id);
            banner.DeletedAt = DateTime.Now;

            _CiPlatformContext.Banners.Update(banner);
            _CiPlatformContext.SaveChanges();
        }

    }
}