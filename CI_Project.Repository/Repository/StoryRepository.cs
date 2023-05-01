using CI_Entities1.Data;
using CI_Entities1.Models;
using CI_Entities1.Models.ViewModel;
using CI_Project.Repository.Interface;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CI_Project.Repository.Repository
{
    public class StoryRepository: IStory
    {
        private readonly CiPlatformContext _CiPlatformContext;

        public StoryRepository(CiPlatformContext db)
        {
            _CiPlatformContext = db;
        }

        //StoryListing
        public StoryShareVM Storylist()
        {
            StoryShareVM storylist = new StoryShareVM();
            storylist.Stories = _CiPlatformContext.Stories.ToList();
            storylist.users = _CiPlatformContext.Users.ToList();
            storylist.missions = _CiPlatformContext.Missions.ToList();
            storylist.storymedia = _CiPlatformContext.StoryMedia.ToList();

            return storylist;
        }

        public StoryShareVM PartialStories(int jpg)
        {
            StoryShareVM storylist = new StoryShareVM();

            storylist.Stories = _CiPlatformContext.Stories.Where(u => u.Status == "Approved").ToList();
            storylist.missionThemes = _CiPlatformContext.MissionThemes.ToList();
            storylist.storymedia = _CiPlatformContext.StoryMedia.ToList();
            storylist.users = _CiPlatformContext.Users.ToList();
            storylist.missions = _CiPlatformContext.Missions.ToList();

            return storylist;
        }

//End
        public void AddStoryMedia(string mediaType, string mediaPath, long missionId, long userId, long storyId, long sId)
        {

            StoryMedium sm = new StoryMedium();
            sm.StoryId = sId;
            sm.StoryType = mediaType;
            sm.StoryPath = mediaPath;
            _CiPlatformContext.Add(sm);
            _CiPlatformContext.SaveChanges();
        }


        public StoryShareVM getstorydata(long id)
        {
            StoryShareVM sl = new StoryShareVM();
            sl.missions = _CiPlatformContext.Missions.ToList();
            sl.missionApplications = _CiPlatformContext.MissionApplications.ToList();

            if (id != 0)
            {
                var story = _CiPlatformContext.Stories.FirstOrDefault(u => u.StoryId == id);
                sl.StoryId = id;
                sl.mission_id = Convert.ToInt32(story.MissionId);
                sl.Title = story.Title;
                sl.editor1 = story.Description;
                sl.PublishedAt = story.PublishedAt;

                sl.storymedia = _CiPlatformContext.StoryMedia.Where(t => t.StoryId == id && t.StoryType == "image").ToList();
            }
            return sl;
        }


    }
}
