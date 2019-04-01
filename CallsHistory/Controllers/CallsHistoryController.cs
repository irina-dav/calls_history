using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CallsHistory.Repositories;
using CallsHistory.Models;
using CallsHistory.Services;
using Microsoft.AspNetCore.Authorization;

namespace CallsHistory.Controllers
{
    public class CallsHistoryController : Controller
    {
        private readonly IRepository repo;
        private readonly IUsersRepository usersRepo;

        public CallsHistoryController(IRepository repository, IUsersRepository usersRepository)
        {
            repo = repository;
            usersRepo = usersRepository;
           
        }

        [Authorize]
        public ViewResult Index()
        {
            HistoryViewModel historyVM = new HistoryViewModel()
            {                
                DateFrom = DateTime.Now.AddDays(-1).Date,
                DateTo = DateTime.Now.AddDays(1).Date,
            };
            ViewBag.Title = "История телефонных звонков";
            return View(historyVM);
        }

        [Authorize]
        [HttpPost]
        public IActionResult SearchCalls(CallsFilter filter)
        {
            var searchResult = repo.SearchCalls(filter);
            List<Call> calls = searchResult.CallsPage.ToList();  
            var callsVm = calls.Select(c => new CallViewModel(c)
            {
                DstName = usersRepo.GetUser(c.Dst.ToString())?.Name,
                SrcName = repo.GetCallerName(c)
            });
            // return PartialView("_Calls", callsVm);

            var json = Json(new { total = searchResult.TotalCalls, rows = callsVm});
            return json;

        }

        [Authorize]
        public JsonResult LoadData(int? pageSize, int? pageNumber, string sortOrder)
        {
            var calls = repo.Calls.TakeLast(100);
            return Json(calls);
        }

        }
}
