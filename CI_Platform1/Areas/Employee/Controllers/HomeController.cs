using CI_Entities1.Data;
using CI_Entities1.Models;
using CI_Entities1.Models.ViewModel;
using CI_Platform1.Models;
using CI_Project.Repository.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.EntityFrameworkCore;
using System.Data;
using System.Diagnostics;
using System.Diagnostics.Metrics;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Web;

namespace CI_Platform1.Controllers
{
    [Area("Employee")]

    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly CiPlatformContext _CiPlatformContext;
        private readonly IUser _Interface;
        private readonly ILanding _Landing;
        private readonly IStory _Story;

        public dynamic SearchingMission { get; private set; }

        public HomeController(CiPlatformContext CiPlatformContext, IStory _Istories, IUser UserInterface, ILanding landing, ILogger<HomeController> logger)
        {
            _CiPlatformContext = CiPlatformContext;
            _logger = logger ;
            _Interface = UserInterface;
            _Landing = landing;
            _Story = _Istories;
        }

        //login method
        public IActionResult Login()
        {
            HttpContext.Session.Clear();
            LoginModel lm = new LoginModel();
            lm.banners = _CiPlatformContext.Banners.OrderBy(u => u.SortOrder).Where(ban => ban.DeletedAt == null).ToList();

            return View(lm);
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            TempData["logout"] = "Logged Out Successfully";
            return RedirectToAction("LandingPage", "Home", new { Area = "Employee" });
        }

        public IActionResult Carousel()
        {
            //var banner = _CiPlatformContext.Banners.Where(ban => ban.DeletedAt == null).ToList();
            return PartialView("Carousel");
        }

        public IActionResult contact(LandingPageVM lvm)
        {
            var userid = HttpContext.Session.GetString("userID");
            if (userid == null)
            {
                return RedirectToAction("Login", "Home", new { Area = "Employee" });
            }
            var use = Convert.ToInt32(userid);

            ContactU cont = new ContactU()
            {
                Subject = lvm.Subject,
                Message = lvm.Message,
                UserId= use,
            };

            _CiPlatformContext.ContactUs.Add(cont);
            _CiPlatformContext.SaveChanges();

            return RedirectToAction("LandingPage", "Home", new { Area = "Employee" });
        }

        [HttpPost]
        public IActionResult Login(LoginModel model)
        {
            LoginModel LM= new LoginModel();
            LM.banners= _CiPlatformContext.Banners.OrderBy(u=> u.SortOrder).Where(ban => ban.DeletedAt == null).ToList();

            if (ModelState.IsValid)
            {

                var Admin = _CiPlatformContext.Admins.FirstOrDefault(u => u.Email == model.Email);
                if (Admin != null)
                {
                    if (Admin.Password == model.Password)
                    {
                        HttpContext.Session.SetString("Firstname", Admin.FirstName);
                        return RedirectToAction("Index", "Admin", new { Area = "Admin" });

                    }
                }
                var user = _Interface.UserByEmailPassword(model.Email, model.Password);
                var username = model.Email.Split('@')[0];
                if (user == null)
                {
                    ViewBag.Error = "Email or Password has not Matched to the registered Credentials";
                }
                else
                {
                    HttpContext.Session.SetString("userID", user.UserId.ToString());
                    HttpContext.Session.SetString("Firstname", user.FirstName);

                    return RedirectToAction("LandingPage", "Home", new { @id = user.UserId });
                }
            }

            return View(LM);
        }

        //Registration 
        public IActionResult Registration()
        {
            RegistrationViewModel rvm = new RegistrationViewModel();
            rvm.banners = _CiPlatformContext.Banners.OrderBy(u => u.SortOrder).Where(ban => ban.DeletedAt == null).ToList();
            //var banner = _CiPlatformContext.Banners.Where(ban => ban.DeletedAt == null).ToList();
            return View(rvm);
        }

        [HttpPost]
        public IActionResult Registration(User user)
        {
            RegistrationViewModel rvm = new RegistrationViewModel();
            rvm.banners = _CiPlatformContext.Banners.Where(ban => ban.DeletedAt == null).ToList();
            var obj = _Interface.UserByEmail(user.Email);

            if (rvm.ConfirmPassword == rvm.Password)
            {
                if (obj == null)
                {
                    _Interface.addUser(user);
                    return RedirectToAction("Login", "Home");
                }
                else
                {
                    ViewBag.Email = "Email already exists";
                }
            }

            else
            {
                ViewBag.password = "Password & confirm password Does not Matches ";
            }


            return View(rvm);
        }


        //Forget Password Model
        public IActionResult Forget()
        {
            ForgetModel FM = new ForgetModel();
            FM.banners = _CiPlatformContext.Banners.OrderBy(u => u.SortOrder).Where(ban => ban.DeletedAt == null).ToList();
            return View(FM);
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public IActionResult Forget(ForgetModel model)
        {
            ForgetModel FM = new ForgetModel();
            FM.banners = _CiPlatformContext.Banners.OrderBy(u => u.SortOrder).Where(ban => ban.DeletedAt == null).ToList();

            if (ModelState.IsValid)
            {
                var user = _Interface.UserByEmail(model.Email);
                ViewBag.user = user;
                if (user == null)
                {
                    return RedirectToAction("ForgetPass", "Home");
                }

                var token = Guid.NewGuid().ToString();
                _Interface.token(model.Email, token);

                var resetLink = Url.Action("Resetpass", "Home", new { email = model.Email, token }, Request.Scheme);


                var fromAddress = new MailAddress("ciplatformoffice@gmail.com", "Sender Name");
                var toAddress = new MailAddress(model.Email);
                var subject = "Password reset request";
                var body = $"Hi,<br /><br />Please click on the following link to reset your password:<br /><br /><a href='{resetLink}'>{resetLink}</a>";
                var message = new MailMessage(fromAddress, toAddress)
                {
                    Subject = subject,
                    Body = body,
                    IsBodyHtml = true
                };
                var smtpClient = new SmtpClient("smtp.gmail.com", 587)
                {
                    UseDefaultCredentials = false,
                    Credentials = new NetworkCredential("ciplatformoffice@gmail.com", "cydalqjrspgeumxm"),
                    EnableSsl = true
                };
                smtpClient.Send(message);

                return RedirectToAction("Login", "Home");
            }
            return View(FM);
        }


        //Reset Password
        [HttpGet]
        public IActionResult Resetpass(string email, string token)
        {
            var passwordReset = _Interface.Reset(email, token);
            if (passwordReset == null)
            {
                return RedirectToAction("Login", "Home");
            }
            // Pass the email and token to the view for resetting the password
            var model = new PasswordReset
            {
                Email = email,
                Token = token
            };
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public IActionResult Resetpass(Resetpassword model)
        {
            if (ModelState.IsValid)
            {
                // Find the user by email
                var user = _Interface.UserByEmail(model.Email);
                if (user == null)
                {
                    return RedirectToAction("Forget", "User");
                }

                // Find the password reset record by email and token
                var passwordReset = _Interface.Reset(model.Email, model.Token);
                if (passwordReset == null)
                {
                    return RedirectToAction("Login", "Home");
                }

                // Update the user's password
                user.Password = model.Password;
                _CiPlatformContext.SaveChanges();

            }

            return RedirectToAction("Login", "Home");
        }


        //Landing Page
        public IActionResult LandingPage()
        {
            //User Admin Name
            if (ViewBag.user != null)
            {
                var userid = HttpContext.Session.GetString("userID");
                ViewBag.UserId = int.Parse(userid);
                if (userid == null)
                {
                    return RedirectToAction("Login", "Home", new { Area = "Employee" });
                }
            }
            List<User> Alluser = _CiPlatformContext.Users.ToList();
            ViewBag.alluser = Alluser;

            List<VolunteeringVM> relatedlist = new List<VolunteeringVM>();
            VolunteeringVM volunteeringVM = new VolunteeringVM();
            ViewBag.Missiondetail = volunteeringVM;

            LandingPageVM lp = new LandingPageVM();

            lp.missions = _Landing.missions();
            lp.users = _Landing.users();
            lp.missionRatings = _Landing.missionRatings();
            lp.cities = _Landing.city();
            lp.countries = _Landing.country();
            lp.missionThemes = _Landing.missionThemes();
            lp.goalMissions = _Landing.goalMissions();
            lp.favoriteMissions = _Landing.favoriteMissions();
            lp.missionApplications = _Landing.missionApplications();
            lp.skills = _Landing.skills();
            lp.MissionSkills = _Landing.missionSkills();
            lp.missionMedia = _CiPlatformContext.MissionMedia.ToList();
            lp.timesheets = _CiPlatformContext.Timesheets.ToList();

            return View(lp);
        }
        [HttpGet]
        public IActionResult _Missions(int jpg, string SearchingMission, string sort, string skill, string country, string city, string theme, long missionId)
        {
            LandingPageVM lp = new LandingPageVM();

            if (ViewBag.user != null)
            {
                var userid = HttpContext.Session.GetString("userID");
                if(userid == null)
                {
                    return RedirectToAction("Login", "Home", new { Area = "Employee" });
                }

                ViewBag.UserId = int.Parse(userid);

                var fav = lp.favoriteMissions.FirstOrDefault(u => u.UserId == Convert.ToInt32(userid) && u.MissionId == missionId);
                ViewBag.fav = fav;
            }


            lp.missions = _Landing.missions();
            lp.users = _Landing.users();
            lp.missionRatings = _Landing.missionRatings();
            lp.cities = _Landing.city();
            lp.countries = _Landing.country();
            lp.missionThemes = _Landing.missionThemes();
            lp.goalMissions = _Landing.goalMissions();
            lp.favoriteMissions = _Landing.favoriteMissions();
            lp.missionApplications = _Landing.missionApplications();
            lp.skills = _Landing.skills();
            lp.MissionSkills = _Landing.missionSkills();
            lp.missionMedia = _CiPlatformContext.MissionMedia.ToList();
            lp.timesheets = _CiPlatformContext.Timesheets.ToList();

            //Search Mission
            if (SearchingMission != null)
            {
                //char.ToUpper(str[0]) + str.Substring(1)
                var search = char.ToUpper(SearchingMission[0]) + SearchingMission.Substring(1);
                lp.missions = lp.missions.Where(m => m.Title.Contains(search)).ToList();

                if (lp.missions.Count() == 0)
                {
                    return RedirectToAction("NoMissionFound", "Home");
                }
            }

            //filter
            if (country != null)
            {
                string[] countryText = country.Split(',');
                lp.missions = lp.missions.Where(m => countryText.Contains(m.Country.Name)).ToList();
            }

            if (city != null)
            {
                string[] cityText = city.Split(',');
                lp.missions = lp.missions.Where(m => cityText.Contains(m.City.Name)).ToList();
            }

            if (theme != null)
            {
                string[] themeText = theme.Split(',');
                lp.missions = lp.missions.Where(m => themeText.Contains(m.Theme.Title)).ToList();
            }


            if (skill != null)
            {
                string[] skillText = skill.Split(',');
                var tempFill = lp.MissionSkills.Where(x => skillText.Contains(x.Skill.SkillName)).Select(x => x.MissionId).ToList();
                lp.missions = lp.missions.Where(e => tempFill.Contains(e.MissionId)).ToList();
            }


            if (lp.missions.Count() == 0)
            {
                return RedirectToAction("nomissionfound", "Home", new { Areas = "Employee" });
            }

            //Add to favrouite

            if (ViewBag.user != null)
            {
                lp.favoriteMissions = _CiPlatformContext.FavoriteMissions.ToList();

            }
            //Order By
            switch (sort)
            {
                case "1":
                    lp.missions = lp.missions.OrderBy(e => e.Title).ToList();
                    break;
                case "2":
                    //lp.missions = lp.missions.OrderByDescending(e => e.StartDate).ToList();
                    lp.missions = lp.missions.OrderBy(e => e.StartDate).ToList();
                    break;
                case "3":
                    lp.missions = lp.missions.OrderByDescending(e => e.StartDate).ToList();
                    break;

                case "4":
                    lp.missions = lp.missions.OrderBy(e => e.Seatleft).ToList();
                    break;

                case "5":
                    lp.missions = lp.missions.OrderBy(e => e.Theme.Title).ToList();
                    break;

                default:
                    lp.missions = lp.missions.OrderBy(e => e.Title).ToList();
                    break;

            }



            //Pagination
            ViewBag.missionCount = lp.missions.Count();
            const int pageSize = 6;
            if (jpg < 1)
            {
                jpg = 1;
            }
            int recsCount = lp.missions.Count();
            var pager = new Pager(recsCount, jpg, pageSize);
            int recSkip = (jpg - 1) * pageSize;
            var data = lp.missions.Skip(recSkip).Take(pager.PageSize).ToList();
            this.ViewBag.pager = pager;
            ViewBag.missionTempDate = data;
            lp.missions = data.ToList();
            ViewBag.TotalMission = recsCount;

            return PartialView("_Missions", lp);
        }

        //Cascading for city and country
        public JsonResult filterCity(long missionCountry)
        {
            IList<City> cities = _CiPlatformContext.Cities.Where(m => m.CountryId == missionCountry).ToList();
            return Json(cities);
        }


        //Volunteering Missions Method
        public IActionResult volunteering(long missionid, long id, long missionId)
        {
            LandingPageVM lp = new LandingPageVM();
            lp.missions = _Landing.missions();
            //lp.users = _Landing.users();
            lp.users = _CiPlatformContext.Users.Where(u => u.DeletedAt == null).ToList();
            lp.cities = _Landing.city();
            lp.countries = _Landing.country();
            lp.missionThemes = _Landing.missionThemes();
            lp.missionRatings = _Landing.missionRatings();
            lp.goalMissions = _Landing.goalMissions();

            //ViewBag.listofmission = lp.missions;
            ViewBag.alluser = lp.users;

            var userid = HttpContext.Session.GetString("userID");

            ViewBag.user = lp.users.FirstOrDefault(e => e.UserId == id);
            List<VolunteeringVM> relatedlist = new List<VolunteeringVM>();

            //IEnumerable<Mission> objMis = _CiPlatformContext.Missions.ToList();
            //IEnumerable<Comment> objComm = _CiPlatformContext.Comments.ToList();
            //IEnumerable<Mission> selected = lp.missions.Where(m => m.MissionId == missionid).ToList();

            //var volmission = lp.missions.FirstOrDefault(m => m.MissionId == Convert.ToInt32(missionid));
            var miss= _CiPlatformContext.Missions.FirstOrDefault(m=> m.MissionId ==missionid);
            var theme = lp.missionThemes.FirstOrDefault(m => m.MissionThemeId == miss.ThemeId);
            var City = lp.cities.FirstOrDefault(m => m.CityId == miss.CityId);
            var prevRating = lp.missionRatings.FirstOrDefault(e => e.MissionId == missionid && e.UserId == id);
            var themeobjective = lp.goalMissions.FirstOrDefault(m => m.MissionId == missionid);

            string[] Startdate = miss.StartDate.ToString().Split(" ");
            string[] Enddate = miss.EndDate.ToString().Split(" ");

            VolunteeringVM volunteeringVM = new VolunteeringVM();
            if (userid == null)
            {
                volunteeringVM.missionApplications = _CiPlatformContext.MissionApplications.Where(u => u.MissionId == missionId).ToList();
            }
            else
            {
                volunteeringVM.missionApplications = _CiPlatformContext.MissionApplications.Where(u => u.MissionId == missionId && u.UserId != Convert.ToInt32(userid)).ToList();
            }
            volunteeringVM.goalMissions = _CiPlatformContext.GoalMissions.Where(u => u.MissionId == missionid).ToList();
            volunteeringVM.timesheets = _CiPlatformContext.Timesheets.Where(u => u.MissionId == missionid).ToList();

            volunteeringVM.missionMedia = _CiPlatformContext.MissionMedia.Where(u => u.MissionId == missionId).ToList();
            volunteeringVM.MissionId = missionid;
            volunteeringVM.users = _Landing.users();
            volunteeringVM.Title = miss.Title;
            volunteeringVM.ShortDescription = miss.ShortDescription;
            volunteeringVM.OrganizationName = miss.OrganizationName;
            volunteeringVM.Description = miss.Description;
            volunteeringVM.OrganizationDetail = miss.OrganizationDetail;
            volunteeringVM.Availability = miss.Availability;
            volunteeringVM.MissionType = miss.MissionType;
            volunteeringVM.Cityname = City.Name;
            volunteeringVM.Themename = theme.Title;
            volunteeringVM.EndDate = Enddate[0];
            volunteeringVM.StartDate = Startdate[0];
            volunteeringVM.favoriteMissions = _CiPlatformContext.FavoriteMissions.ToList();

            if (userid != null)
            {
                var ratings = _Interface.missionRatings().FirstOrDefault(MR => MR.MissionId == missionId && MR.UserId == int.Parse(userid));
                volunteeringVM.Rating = ratings != null ? Convert.ToInt64(ratings.Rating) : 0;

                //Approval status
                var app= _CiPlatformContext.MissionApplications.FirstOrDefault(u=> u.UserId == Convert.ToInt32(userid)  && u.MissionId == missionId);

                if(app != null)
                {
                    if(app.ApprovalStatus == "Approved")
                    {
                        volunteeringVM.isapplied = 1;
                    }

                    else if (app.ApprovalStatus == "Rejected")
                    {
                        volunteeringVM.isapplied = 2;
                    }

                    else
                    {
                        volunteeringVM.isapplied = 3;
                        //Pending
                    }
                }
                else
                {
                    volunteeringVM.isapplied = 0;
                }
                ViewBag.isapplied = volunteeringVM.isapplied;

                var fav = volunteeringVM.favoriteMissions.FirstOrDefault(u => u.UserId == Convert.ToInt32(userid) && u.MissionId == missionId);
                ViewBag.fav = fav;

            }
            if(themeobjective != null)
            {
                volunteeringVM.GoalObjectiveText = themeobjective.GoalObjectiveText;
            }
            volunteeringVM.comments = _CiPlatformContext.Comments.OrderByDescending(x=> x.CreatedAt).Where(m => m.MissionId == missionid && m.DeletedAt==null && m.UserId == Convert.ToInt32(userid)).ToList();

            //Average Rating
            int finalrating = 0;
            var ratinglist = _Interface.missionRatings().Where(m => m.MissionId == miss.MissionId).ToList();
            if (ratinglist.Count() > 0)
            {
                int rat = 0;
                foreach (var m in ratinglist)
                {
                    rat = rat + Convert.ToInt32(m.Rating);
                }
                finalrating = rat / ratinglist.Count();
            }
            ViewBag.finalrating = finalrating;
            ViewBag.totalvol = ratinglist.Count();
            ViewBag.Missiondetail = volunteeringVM;

            //Related Missions
            var relatedmission = lp.missions.Where(m => m.ThemeId == miss.ThemeId && m.MissionId != missionid).ToList();
            foreach (var item in relatedmission.Take(3))
            {

                var relcity = lp.cities.FirstOrDefault(m => m.CityId == item.CityId);
                var reltheme = lp.missionThemes.FirstOrDefault(m => m.MissionThemeId == item.ThemeId);
                var relgoalobj = lp.goalMissions.FirstOrDefault(m => m.MissionId == item.MissionId);
                string[] Startdate1 = item.StartDate.ToString().Split(" ");
                string[] Enddate2 = item.EndDate.ToString().Split(" ");

                relatedlist.Add(new VolunteeringVM
                {
                    MissionId = item.MissionId,
                    Cityname = relcity.Name,
                    Themename = reltheme.Title,
                    Title = item.Title,
                    ShortDescription = item.ShortDescription,
                    StartDate = Startdate1[0],
                    EndDate = Enddate2[0],
                    Availability = item.Availability,
                    OrganizationName = item.OrganizationName,
                    GoalObjectiveText = relgoalobj!=null? relgoalobj.GoalObjectiveText:null,
                    MissionType = item.MissionType,
                }
                );
                ;
            }
            ViewBag.relatedmission = relatedlist.Take(3);
            return View(volunteeringVM);
        }

        //Applied
        public IActionResult Applied(int missonid)
        {
            var userid = HttpContext.Session.GetString("userID");
            if (userid == null)
            {
                return RedirectToAction("Login", "Home", new { Area = "Employee" });
            }

            _Interface.ApplyMission(missonid, Convert.ToInt32(userid));
            return RedirectToAction("volunteering", new { id = Convert.ToInt64(HttpContext.Session.GetString("userID")), missionid = missonid });
        }

        public IActionResult nomissionfound()
        {
            return View();
        }

        //Story Listing
        public IActionResult Storylisting()
        {
            var userid = HttpContext.Session.GetString("userID");
            if (userid == null)
            {
                return RedirectToAction("Login", "Home", new { Area = "Employee" });

            }
            var data = _Story.Storylist();
            return View(data);
        }

        //StoryListing Partial View(Display of Stories)
        public IActionResult _StoryList(int jpg)
        {
            //var data= _Story.PartialStories(jpg);
            var userid = HttpContext.Session.GetString("userID");
            if (userid == null)
            {
                return RedirectToAction("Login", "Home", new { Area = "Employee" });

            }

            StoryShareVM storylist = new StoryShareVM();

            storylist.Stories = _CiPlatformContext.Stories.Where(u => u.Status == "Approved").ToList();
            storylist.missionThemes = _CiPlatformContext.MissionThemes.ToList();
            storylist.storymedia = _CiPlatformContext.StoryMedia.ToList();

            //Pagination
            ViewBag.missionCount = storylist.Stories.Count();
            const int pageSize = 6;
            if (jpg < 1)
            {
                jpg = 1;
            }
            int recsCount = storylist.Stories.Count();
            var pager = new Pager(recsCount, jpg, pageSize);
            int recSkip = (jpg - 1) * pageSize;
            var data = storylist.Stories.Skip(recSkip).Take(pager.PageSize).ToList();
            this.ViewBag.pager = pager;
            ViewBag.missionTempDate = data;
            storylist.Stories = data.ToList();
            ViewBag.TotalMission = recsCount;

            storylist.users = _CiPlatformContext.Users.ToList();
            storylist.missions = _CiPlatformContext.Missions.ToList();
            return PartialView("_StoryList", storylist);
        }

        //Storyshare
        public async Task<IActionResult> StoryshareAsync(long id)
        {
            var userid = HttpContext.Session.GetString("userID");
            if (userid == null)
            {
                return RedirectToAction("Login", "Home", new { Area = "Employee" });

            }
            var data = _Story.getstorydata(id);

            return View(data);
        }

        [HttpPost]
        public async Task<IActionResult> Storyshare(StoryShareVM ss, string action, long storyid)
        {
            //var data = _Story.PostStoryShareAsync(ss, action, storyid);

            //storyid comes here  is from storyshare page
            var userid = HttpContext.Session.GetString("userID");
            if (userid == null)
            {
                return RedirectToAction("Login", "Home", new { Area = "Employee" });

            }
            ss.missions = _CiPlatformContext.Missions.ToList();
            ss.missionApplications = _CiPlatformContext.MissionApplications.Where(u => u.UserId == Convert.ToInt32(userid)).ToList();

            if (storyid == 0)
            {
                Story stories = new Story();
                stories.UserId = Convert.ToInt64(HttpContext.Session.GetString("userID"));
                stories.MissionId = ss.MissionId;
                stories.Title = ss.Title;
                stories.Description = ss.editor1;
                stories.PublishedAt = ss.PublishedAt;
                stories.CreatedAt = DateTime.Now;

                if (action == "submit")
                {
                    stories.Status = "Pending";
                }

                //Action= save
                else
                {
                    stories.Status = "Draft";
                }
                _CiPlatformContext.Stories.Add(stories);
                _CiPlatformContext.SaveChanges();

                var sId = stories.StoryId;

                if (ss.attachment != null)
                {
                    foreach (var i in ss.attachment)
                    {
                        var FileName = "";
                        using (var ms = new MemoryStream())
                        {
                            await i.CopyToAsync(ms);
                            var imageBytes = ms.ToArray();
                            var base64String = Convert.ToBase64String(imageBytes);
                            FileName = "data:image/png;base64," + base64String;
                        }
                        _Story.AddStoryMedia(i.ContentType.Split("/")[0], FileName, ss.mission_id, Convert.ToInt32(userid), ss.StoryId, sId);
                    }
                }
                return View(ss);
            }

            else
            {
                var stories = _CiPlatformContext.Stories.FirstOrDefault(u => u.StoryId == storyid);
                stories.UserId = Convert.ToInt64(HttpContext.Session.GetString("userID"));
                stories.MissionId = ss.MissionId;
                stories.Title = ss.Title;
                stories.Description = ss.editor1;
                stories.PublishedAt = ss.PublishedAt;
                stories.CreatedAt = DateTime.Now;

                if (action == "submit")
                {
                    stories.Status = "Pending";
                }

                //Action ==Save
                else
                {
                    stories.Status = "Draft";
                }

                _CiPlatformContext.Stories.Update(stories);
                _CiPlatformContext.SaveChanges();

                var sId = stories.StoryId;
                if (ss.attachment != null)
                {
                    foreach (var i in ss.attachment)
                    {
                        var FileName = "";
                        using (var ms = new MemoryStream())
                        {
                            await i.CopyToAsync(ms);
                            var imageBytes = ms.ToArray();
                            var base64String = Convert.ToBase64String(imageBytes);
                            FileName = "data:image/png;base64," + base64String;
                        }
                        _Story.AddStoryMedia(i.ContentType.Split("/")[0], FileName, ss.mission_id, Convert.ToInt32(userid), ss.StoryId, sId);
                    }
                }
                return View(ss);
            }
        }

        public IActionResult Draft()
        {
            var userid = HttpContext.Session.GetString("userID");
            if (userid == null)
            {
                return RedirectToAction("Login", "Home", new { Area = "Employee" });
            }

            StoryShareVM sl = new StoryShareVM();
            sl.Stories = _CiPlatformContext.Stories.Where(u => (u.Status == "Draft") && (u.UserId == Convert.ToInt64(userid))).ToList();
            sl.users = _CiPlatformContext.Users.ToList();
            sl.missions = _CiPlatformContext.Missions.ToList();
            sl.missionThemes = _CiPlatformContext.MissionThemes.ToList();
            sl.storymedia = _CiPlatformContext.StoryMedia.ToList();

            return View(sl);
        }

        //Story Detail
        public IActionResult StoryDetail(int missionid)
        {
            //var userid = HttpContext.Session.GetString("userID");
            //if (userid == null)
            //{
            //    return RedirectToAction("Login", "Home", new { Area = "Employee" });
            //}
            StoryShareVM st = new StoryShareVM();

            var Story = _CiPlatformContext.Stories.Where(w => w.StoryId == missionid).FirstOrDefault();
            st.singleStory = Story;
            st.users = _CiPlatformContext.Users.ToList();
            st.missions = _CiPlatformContext.Missions.Where(u => u.DeletedAt == null).ToList();
            st.ShortDescription = HttpUtility.HtmlDecode(Story.Description);
            st.Title = Story.Title;
            st.storymedia = _CiPlatformContext.StoryMedia.Where(x => x.StoryId == missionid).ToList();

            var userId = HttpContext.Session.GetString("userID");
            if (userId != null)
            {
                //views
                if (Story.StoryViews == null)
                {
                    Story.StoryViews = 0;
                }

                Story.StoryViews = Story.StoryViews + 1;
                st.StoryViews = Story.StoryViews;
                _CiPlatformContext.Stories.Update(Story);
                _CiPlatformContext.SaveChanges();
            }
            return View(st);
        }


        private void ToList()
        {
            throw new NotImplementedException();
        }

        public IActionResult Privacy()
        {
            AdminVM adminVM = new AdminVM();
            adminVM.Pages = _CiPlatformContext.CmsPages.Where(u=> u.Status== "Active").ToList();
            return View(adminVM);
        }

        //Edit Profile
        public IActionResult EditProfile()
        {
            var userid = HttpContext.Session.GetString("userID");
            if (userid == null)
            {
                return RedirectToAction("Login", "Home", new { Area = "Employee" });
            }

            UserVM user = new UserVM();
            // user.Singleuser = _CiPlatformContext.Users.FirstOrDefault(u => u.UserId == Convert.ToInt32(userid));
            var u = _CiPlatformContext.Users.FirstOrDefault(u => u.UserId == Convert.ToInt32(userid));

            user.Countries = _CiPlatformContext.Countries.ToList();
            user.cities = _CiPlatformContext.Cities.ToList();
            user.userSkills = _CiPlatformContext.UserSkills.Where(u => u.UserId == Convert.ToInt32(userid)).ToList();
            user.skills = _CiPlatformContext.Skills.ToList();

            user.FirstName = u.FirstName;
            user.LastName = u.LastName;
            user.EmployeeId = u.EmployeeId;
            user.Title = u.Title;
            user.Department = u.Department;
            user.ProfileText = u.ProfileText;
            user.WhyIVolunteer = u.WhyIVolunteer;
            user.CountryId = u.CountryId;
            user.CityId = u.CityId;
            user.LinkedInUrl = u.LinkedInUrl;
            if (u.Avatar != null)
            {
                user.UserAvatar = u.Avatar;
            }

            var allskills = _CiPlatformContext.Skills.ToList();
            ViewBag.allskills = allskills;
            var skills = from US in _CiPlatformContext.UserSkills
                         join S in _CiPlatformContext.Skills on US.SkillId equals S.SkillId
                         select new { US.SkillId, S.SkillName, US.UserId };
            var uskills = skills.Where(e => e.UserId == Convert.ToInt32(userid)).ToList();
            ViewBag.userskills = uskills;
            foreach (var skill in uskills)
            {
                var rskill = allskills.FirstOrDefault(e => e.SkillId == skill.SkillId);
                allskills.Remove(rskill);
            }
            ViewBag.remainingSkills = allskills;


            return View(user);
        }

        [HttpPost]
        public async Task<IActionResult> EditProfileAsync(UserVM user)
        {
            var userid = HttpContext.Session.GetString("userID");
            if (userid == null)
            {
                return RedirectToAction("Login", "Home", new { Area = "Employee" });
            }

            user.Countries = _CiPlatformContext.Countries.ToList();
            user.cities = _CiPlatformContext.Cities.ToList();
            var obj = _CiPlatformContext.Users.Where(u => u.UserId == Convert.ToInt32(userid)).FirstOrDefault();

            obj.FirstName = user.FirstName;
            obj.LastName = user.LastName;
            obj.EmployeeId = user.EmployeeId;
            obj.Title = user.Title;
            obj.Department = user.Department;
            obj.ProfileText = user.ProfileText;
            obj.WhyIVolunteer = user.WhyIVolunteer;
            obj.LinkedInUrl = user.LinkedInUrl;
            obj.CityId = user.CityId;
            obj.CountryId = user.CountryId;

            var FileName = "";
            if (user.Avatar != null)
            {
                using (var ms = new MemoryStream())
                {
                    await user.Avatar.CopyToAsync(ms);
                    var imageBytes = ms.ToArray();
                    var base64String = Convert.ToBase64String(imageBytes);
                    FileName = "data:image/png;base64," + base64String;
                }
                obj.Avatar = FileName;
            }

            _CiPlatformContext.Users.Add(obj);
            _CiPlatformContext.Users.Update(obj);
            _CiPlatformContext.SaveChanges();

            UserVM user1 = new UserVM();
            user1.cities = _CiPlatformContext.Cities.ToList();
            user1.Countries = _CiPlatformContext.Countries.ToList();
            user1.FirstName = user.FirstName;
            user1.LastName = user.LastName;
            user1.EmployeeId = user.EmployeeId;
            user1.Title = user.Title;
            user1.Department = user.Department;
            user1.ProfileText = user.ProfileText;
            user1.WhyIVolunteer = user.WhyIVolunteer;
            user1.LinkedInUrl = user.LinkedInUrl;
            user1.CityId = user.CityId;
            user1.CountryId = user.CountryId;
            user1.UserAvatar = FileName;

            return View(user1);
        }

        //Editprofile ChangePassword
        [HttpPost]
        public bool ChangePassword(string old, string newp, string cnf)
        {

            var userid = HttpContext.Session.GetString("userID");
            var user = _CiPlatformContext.Users.Where(e => e.UserId == Convert.ToInt32(userid)).FirstOrDefault();

            if (old != user.Password)
            {
                return false;
            }
            else
            {
                var pass = _CiPlatformContext.Users.FirstOrDefault(u => u.Password == old);
                pass.Password = newp;

                _CiPlatformContext.Users.Update(pass);
                _CiPlatformContext.SaveChanges();

                return true;
            }

        }

        //Timesheet
        public IActionResult Timesheet()
        {
            var userid = HttpContext.Session.GetString("userID");
            if (userid == null)
            {
                return RedirectToAction("Login", "Home", new { Area = "Employee" });
            }
            StoryShareVM ss = new StoryShareVM();
            ss.missions = _CiPlatformContext.Missions.ToList();
            ss.missionApplications = _CiPlatformContext.MissionApplications.Where(e => e.UserId == Convert.ToInt64(userid)).ToList();
            ss.timesheets = _CiPlatformContext.Timesheets.ToList();

            return View(ss);
        }

        [HttpPost]
        public IActionResult Timesheet(StoryShareVM ss)
        {

            if (ss.TimesheetId != 0)
            {
                Timesheet sheet = _CiPlatformContext.Timesheets.FirstOrDefault(e => e.TimesheetId == ss.TimesheetId);
                if (ss.hour != 0 && ss.minute != 0)
                {
                    var userid = HttpContext.Session.GetString("userID");
                    var user = Convert.ToInt32(userid);
                    sheet.UserId = user;
                    sheet.MissionId = ss.MissionId;
                    sheet.TimesheetTime = ss.hour + ":" + ss.minute;
                    ss.DateVolunteered = ss.DateVolunteered;
                    sheet.DateVolunteered = ss.DateVolunteered;
                    sheet.Notes = ss.Notes;
                }

                else
                {
                    var userid = HttpContext.Session.GetString("userID");
                    var user = Convert.ToInt32(userid);
                    sheet.UserId = user;
                    sheet.MissionId = ss.MissionId;
                    ss.DateVolunteered = ss.DateVolunteered;
                    sheet.DateVolunteered = ss.DateVolunteered;
                    sheet.Notes = ss.Notes;
                    sheet.Action = ss.Action;
                }

                _CiPlatformContext.Timesheets.Update(sheet);
                _CiPlatformContext.SaveChanges();
            }

            else
            {
                Timesheet sheets = new Timesheet();
                if (ss.hour != 0 && ss.minute != 0)
                {
                    var userid = HttpContext.Session.GetString("userID");
                    var user = Convert.ToInt32(userid);
                    sheets.UserId = user;
                    sheets.MissionId = ss.MissionId;
                    sheets.TimesheetTime = ss.hour + ":" + ss.minute;
                    ss.DateVolunteered = ss.DateVolunteered;
                    sheets.DateVolunteered = ss.DateVolunteered;
                    sheets.Notes = ss.Notes;
                }

                else
                {
                    var userid = HttpContext.Session.GetString("userID");
                    var user = Convert.ToInt32(userid);
                    sheets.UserId = user;
                    sheets.MissionId = ss.MissionId;
                    ss.DateVolunteered = ss.DateVolunteered;
                    sheets.DateVolunteered = ss.DateVolunteered;
                    sheets.Notes = ss.Notes;
                    sheets.Action = ss.Action;
                }

                _CiPlatformContext.Timesheets.Add(sheets);
                _CiPlatformContext.SaveChanges();
            }

            return RedirectToAction("Timesheet", "Home");
        }

        public IActionResult EditTimesheet(int id)
        {
            StoryShareVM ss = new StoryShareVM();
            var userid = HttpContext.Session.GetString("userID");
            ss.missions = _CiPlatformContext.Missions.ToList();
            ss.missionApplications = _CiPlatformContext.MissionApplications.Where(e => e.UserId == Convert.ToInt64(userid)).ToList();
            ss.timesheets = _CiPlatformContext.Timesheets.ToList();

            ss.Singlesheet = _CiPlatformContext.Timesheets.FirstOrDefault(u => u.UserId == Convert.ToInt32(userid));
            var sheet = _CiPlatformContext.Timesheets.FirstOrDefault(u => u.TimesheetId == id);

            ss.DateVolunteered = sheet.DateVolunteered;
            ss.TimesheetId = sheet.TimesheetId;
            ss.Notes = sheet.Notes;

            //if (ss.hour != 0 && ss.minute != 0)
            if (sheet.TimesheetTime != null)
            {
                ss.hour = Convert.ToInt32(sheet.TimesheetTime.Split(":")[0]);
                ss.minute = Convert.ToInt32(sheet.TimesheetTime.Split(":")[1]);
            }

            else
            {
                //ss.hour = 0;
                //ss.minute = 0;
                ss.Action = sheet.Action;
            }


            return View("Timesheet", ss);
        }


        //Delete Timesheet
        [HttpPost]
        public IActionResult Delete(int id)
        {
            var time = _CiPlatformContext.Timesheets.FirstOrDefault(x => x.TimesheetId == id);
            _CiPlatformContext.Timesheets.Remove(time);
            _CiPlatformContext.SaveChanges();

            return View();
        }
        [HttpPost]
        public async Task<IActionResult> SaveUserSkills(long[] selectedSkills)
        {
            var userid = HttpContext.Session.GetString("userID");
            long id = Convert.ToInt64(userid);
            var abc = _CiPlatformContext.UserSkills.Where(e => e.UserId == id).ToList();
            _CiPlatformContext.RemoveRange(abc);
            _CiPlatformContext.SaveChanges();
            foreach (var skills in selectedSkills)
            {

                //_IUser.AddUserSkills(skills, Convert.ToInt32(userid));
                UserSkill u = new UserSkill();
                u.SkillId = skills;
                u.UserId = id;
                _CiPlatformContext.Add(u);
                _CiPlatformContext.SaveChanges();

            }

            return RedirectToAction("EditProfile", "Home");


        }


        //--------------------Error--------------------
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        //Add to favrouite
        public IActionResult addToFavourite(int missonid)
        {
            var userId = HttpContext.Session.GetString("userID");
            if (userId != null)
            {
                if (missonid != null)
                {
                    _Interface.FavMission(missonid, Convert.ToInt32(userId));
                }
                return RedirectToAction("Volunteering", new { id = int.Parse(userId), missionid = missonid });
            }
            return View();
        }

        //favroite landing page
        public IActionResult addToFavouriteLanding(int missonid)
        {
            var userId = HttpContext.Session.GetString("userID");
            if (userId != null)
            {
                if (missonid != null)
                {
                    //var tempFav = _IUser.favoriteMissions().Where(e => (e.MissionId == missonid) && (e.UserId == Convert.ToInt32(userId))).FirstOrDefault();
                    _Interface.FavMission(missonid, Convert.ToInt32(userId));

                }
                return RedirectToAction("_Missions", new { id = int.Parse(userId), missionid = missonid });
            }
            return RedirectToAction("_Missions");
            return PartialView("_Missions");

        }



        //---------------------Comments---------------------------
        public IActionResult PostComment(int missionId, string Content)
        {
            var userId = HttpContext.Session.GetString("userID");
            if (userId != null)
            {
                Comment objComment = new Comment();
                objComment.UserId = Convert.ToInt64(HttpContext.Session.GetString("userID"));
                objComment.MissionId = missionId;
                objComment.Comment1 = Content;
                objComment.CreatedAt = DateTime.Now;
                _CiPlatformContext.Comments.Add(objComment);
                _CiPlatformContext.SaveChanges();
                return RedirectToAction("volunteering", new { id = Convert.ToInt64(HttpContext.Session.GetString("userID")), missionid = missionId });
            }
            return View();
        }


        //-----------------Reccomend to coworker----------------------

        [HttpPost]
        public async Task<IActionResult> Sendmail(long[] userid, int id)
        {
            var userId = HttpContext.Session.GetString("userID");
            if (userId != null)
            {
                foreach (var item in userid)
                {
                    var user = _CiPlatformContext.Users.FirstOrDefault(u => u.UserId == item);
                    var resetLink = Url.Action("Volunteering", "Home", new { user = user.UserId, missionId = id }, Request.Scheme);

                    var fromAddress = new MailAddress("ciplatformoffice@gmail.com");
                    var toAddress = new MailAddress(user.Email);
                    var subject = "Password reset request";
                    var body = $"Hi,<br /><br />This is to <br /><br /><a href='{resetLink}'>{resetLink}</a>";
                    var message = new MailMessage(fromAddress, toAddress)
                    {
                        Subject = subject,
                        Body = body,
                        IsBodyHtml = true
                    };
                    var smtpClient = new SmtpClient("smtp.gmail.com", 587)
                    {
                        UseDefaultCredentials = false,
                        Credentials = new NetworkCredential("ciplatformoffice@gmail.com", "cydalqjrspgeumxm"),
                        EnableSsl = true
                    };
                    smtpClient.Send(message);

                }
                return Json(new { success = true });
            }
            return View();
        }

        //-----------------Rating-----------------
        public IActionResult Addrating(int rating, long missionId, long Id)
        {
            MissionRating ratingExists = _Interface.MissionratingByUserid_Missionid(Id, missionId);
            if (ratingExists != null)
            {
                _Interface.updaterating(ratingExists, rating);
            }
            else
            {
                _Interface.addratings(rating, Id, missionId);
            }
            return RedirectToAction("Volunteering", new { id = Id, missionId = missionId });
        }

    }
}