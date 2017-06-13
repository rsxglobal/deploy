using Sabio.Web.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Sabio.Web.Controllers
{
    [RoutePrefix("users")]
    public class UsersController :BaseController
    {
      
        [Route("register")]
        public ActionResult Register()
        {
            return View();
        }

        [Route("confirm/{securityToken:guid}")]
        public ActionResult Confirm(Guid securityToken)
        {

            ItemViewModel<Guid> model = new ItemViewModel<Guid>();

            model.Item = securityToken;
            return View(model);
        }

        [Route("login")]
        public ActionResult Login()
        {
            return View();
        }

        [Route("resetpassword/{securityToken:guid}")]
        public ActionResult ResetPassword(Guid securityToken)
        {
            ItemViewModel<Guid> model = new ItemViewModel<Guid>();
            model.Item = securityToken;
            return View(model);
        }
        [Route("home")]
        public ActionResult Home()
        {
            return View();
        }
        [Route("cookies")]
        public ActionResult CookiePolicy()
        {
            return View();
        }
    }
}