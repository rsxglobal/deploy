using Sabio.Web.Models.ViewModels;
using System.Web.Mvc;

namespace Sabio.Web.Controllers
{
    
    [RoutePrefix("person")]
    public class PersonController : BaseController
    {
        //[Authorize(Roles = "Admin")]
        [Route]
        [Route("index")]
        public ActionResult Index()
        {
            return View();
        }
        [Route("{id:int}")]
        public ActionResult IndexRead(int id)
        {
            ItemViewModel<int> model = new ItemViewModel<int>();
            model.Item = id;
            return View(model);
        }
        [Authorize]
        [Route("{id:int}/edit")]
        public ActionResult Create(int id = 0)
        {
            ItemViewModel<int> model = new ItemViewModel<int>();
            DecorateViewModel(model);
            if(id == 0)
            {
                id = model.Id;
            }
            model.Item = id;
            return View(model);
        }


        [AllowAnonymous]
        [Route("indexbase")]
        public ActionResult IndexBase()
        {
            return View();
        }
    }
}
