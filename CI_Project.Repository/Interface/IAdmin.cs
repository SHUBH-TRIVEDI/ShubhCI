using CI_Entities1.Models;
using CI_Entities1.Models.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CI_Project.Repository.Interface
{
    public interface IAdmin
    {
        public AdminVM GetUserData();

        public User PostUserData(string img, long USERID, string first, string last, string mail, string password, string employeeid, string department, long country, long city, string profile, string status);

        public User GetUserEditData(long USERID);

        public void UserDelete(long USERID);

        //CMS

        public AdminVM GetCMSData();

        public CmsPage PostCMSData(long CMSId, String Title, String Desc, String Slug, String Status);

        public CmsPage GetCMSEditData(long CMSId);

        public void CMSDelete(long CMSId);

        //Misssions

        public AdminVM GetMissionData();

        public Mission PostMissionData(string title, string shortdesc, string description, string orgname, int country, int city, string orgdetail, string misstype, int themeid, DateTime start, DateTime end, string avail, int MISSIONID, int skill);

        public void MissionDelete(long MISSIONID);

        public Mission GetMissionEditData(long MISSIONID);

        public MissionSkill PostSkillData(int skill);

        public MissionSkill PostSkillEditData(int skill);

        //Story

        public AdminVM GetStoryData();

        public void ApproveStory(long storyid);

        public void RejectStory(long storyid);
        public void StoryDelete(long storyid);

        //MissionApplication
        public AdminVM GetApplicationData();

        public void ApproveApplication(long Missionapplicationid);
        public void RejectApplication(long Missionapplicationid);

        //Theme
        public AdminVM GetThemeData();

        public MissionTheme ThemeAddEdit(long Themeid, string title, byte status);

        public MissionTheme GetThemeEditData(long Themeid);

        public void ThemeDelete(long id);

        //Skill

        public AdminVM GetSkillData();

        public Skill SkillAddEdit(string status, string title, long SkillId);

        public Skill GetSkillEditData(long SkillId);

        public void SkillDelete(long SkillId);

        //Admin

        public AdminVM GetAdminData();

        public Banner AddEditAdmin(AdminVM adminVM);

        public AdminVM BannerEdit(long id);

        public void BannerDelete(long id);
    }
}
