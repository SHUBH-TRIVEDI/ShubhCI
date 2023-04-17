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
            return View();
        }

        public IActionResult _User()
        {
            UserVM uv = new UserVM();
            uv.users = _CiPlatformContext.Users.ToList();
            return PartialView("_User", uv);
        }

        public IActionResult User()
        {
            UserVM uv=new UserVM();
            uv.users = _CiPlatformContext.Users.ToList();
            
            //var user = _CiPlatformContext.Users.FirstOrDefault();
            //uv.FirstName= user.FirstName;
            //uv.LastName = user.LastName;
            //uv.Email= user.Email;
            ////uv.Department=user.Department;
            ////uv.EmployeeId=user.EmployeeId;
            //uv.Status=user.Status;
            return View(uv);
        }
    }
}
