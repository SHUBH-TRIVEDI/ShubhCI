using CI_Entities1.Models;
using CI_Entities1.Models.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CI_Project.Repository.Interface
{
    public interface IStory
    {
        public void AddStoryMedia(string mediaType, string mediaPath, long missionId, long userId, long storyId, long sId);

        //Story Listing
        public StoryShareVM Storylist();
        public StoryShareVM PartialStories(int jpg);

        public StoryShareVM getstorydata(long id);
    }
}
