﻿using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CallsHistory.Repositories;
using CallsHistory.Models;
using CallsHistory.Services;

namespace CallsHistory.Controllers
{
    public class CallsHistory : Controller
    {
        private readonly IRepository repo;
        private readonly IUsersRepository usersRepo;

        public CallsHistory(IRepository repository, IUsersRepository usersRepository)
        {
            repo = repository;
            usersRepo = usersRepository;
           
        }

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

        [HttpPost]
        public IActionResult SearchCalls(CallsFilter filter)
        {
            List<Call> calls = repo.SearchCalls(filter).ToList();  
            var callsVm = calls.Select(c => new CallViewModel(c)
            {
                DstName = usersRepo.GetUser(c.Dst.ToString())?.Name,
                SrcName = repo.GetCallerName(c)
            });
            return PartialView("_Calls", callsVm);
        }

    }
}
