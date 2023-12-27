﻿using InfoStruct.Sessions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ModelsLibrary;
using ModelsLibrary.Models;
using System.IO.Pipelines;

namespace ProgramASP.Controllers
{
    public class SteelPipeController : Controller
    {
        private HeatSchemeStorageContext db;
        private const int CacheDuration = 258;
        private List<SteelPipe> _steelpipe;

        public SteelPipeController([FromServices] HeatSchemeStorageContext context)
        {
            db = context;
            _steelpipe = db.SteelPipes.ToList();
        }

        [ResponseCache(Duration = CacheDuration, VaryByQueryKeys = new[] { "pageNumber" })]
        public async Task<IActionResult> ShowTable(int pageNumber = 1)
        {
            var query = db.SteelPipes.AsQueryable();

            int total = await query.CountAsync();
            int totalPages = (int)Math.Ceiling((double)total / 20);

            var data = await query
                .Skip((pageNumber - 1) * 20)
                .Take(20)
                .ToListAsync();

            ViewBag.data = data;
            ViewBag.pageNumber = pageNumber;
            ViewBag.totalPages = totalPages;
            ViewBag.DbContext = db;

            return View();
        }

        [HttpPost]
        [Authorize]
        public IActionResult Update(int pipeId, decimal outerDiameter, decimal thickness, decimal linearInternalVolume, decimal linearWeight)
        {
            var PageNumber = HttpContext.Request.Query["PageNumber"];
            if (pipeId != 0 && outerDiameter > 0 && thickness > 0 && linearInternalVolume > 0 && linearWeight > 0)
            {
                var steelpipeToUpdate = _steelpipe.Find(x => x.ID == pipeId);
                steelpipeToUpdate.OuterDiameter = outerDiameter;
                steelpipeToUpdate.Thickness = thickness;
                steelpipeToUpdate.LinearInternalVolume = linearInternalVolume;
                steelpipeToUpdate.LinearWeight = linearWeight;

                var steelpipeToUpdateInBD = db.SteelPipes.Find(pipeId);
                steelpipeToUpdateInBD = steelpipeToUpdate;

                db.SaveChanges();
            }

            ViewBag.data = _steelpipe;
            return RedirectToAction("ShowTable", "SteelPipe", new { pageNumber = PageNumber });
        }



        [HttpGet]
        [Authorize]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [Authorize]
        public IActionResult Create(decimal outerDiameter, decimal thickness, decimal linearInternalVolume, decimal linearWeight)
        {
            if (outerDiameter > 0 && thickness > 0 && linearInternalVolume > 0 && linearWeight > 0)
            {
                var lastSteelpipe = db.SteelPipes.OrderBy(e => e.PipeId).LastOrDefault();
                int lastID = lastSteelpipe.PipeId + 1;
                var newSteelpipe = new SteelPipe
                {
                    PipeId = lastID,
                    OuterDiameter = outerDiameter,
                    Thickness = thickness,
                    LinearInternalVolume = linearInternalVolume,
                    LinearWeight = linearWeight
                };
                db.SteelPipes.Add(newSteelpipe);
                db.SaveChanges();
            }
            
            return RedirectToAction("ShowTable", "SteelPipe");
        }



        [HttpGet]
        [Authorize]
        public IActionResult Delete()
        {
            return View();
        }

        [HttpPost]
        [Authorize]
        public IActionResult Delete(int pipeId)
        {
            var steelPipe = db.SteelPipes.Find(pipeId);
            if (steelPipe != null)
            {
                db.SteelPipes.Remove(steelPipe);
                db.SaveChanges();
            }
            return RedirectToAction("ShowTable", "SteelPipe");
        }



		public IActionResult Search(string searchColumn, string searchText)
		{
			List<SteelPipe> result = new();
			if (searchColumn is null || searchText is null)
			{
				if (TryGetCookie("columnSteelPipe", ref searchColumn))
				{
					ViewBag.column = searchColumn;
				}
				if (TryGetCookie("textForSearchSteelPipe", ref searchText))
				{
					ViewBag.text = searchText;
				}
			}
			else
			{
				ViewBag.column = searchColumn;
				ViewBag.text = searchText;
				HttpContext.Response.Cookies.Append("columnSteelPipe", searchColumn);
				HttpContext.Response.Cookies.Append("textForSearchSteelPipe", searchText);
			}

			if (searchColumn is null || searchText is null) return View();

			HttpContext.Response.Cookies.Append("columnSteelPipe", searchColumn);
			HttpContext.Response.Cookies.Append("textForSearchSteelPipe", searchText);

			switch (searchColumn)
			{
				case "OuterDiameter":
					result = _steelpipe
						.Where(e => e.OuterDiameter.ToString() == searchText)
						.ToList();
					break;
				case "Thickness":
					result = _steelpipe
						.Where(e => e.Thickness.ToString() == searchText)
						.ToList();
					break;
				case "LinearInternalVolume":
					result = _steelpipe
						.Where(e => e.LinearInternalVolume.ToString() == searchText)
						.ToList();
					break;
				case "LinearWeight":
					result = _steelpipe
						.Where(e => e.LinearWeight.Value.ToString() == searchText)
						.ToList();
					break;
			}

			ViewBag.data = result;

			return View();
		}

		private bool TryGetCookie(string key, ref string? res)
		{
			if (HttpContext.Request.Cookies.ContainsKey(key))
			{

				res = HttpContext.Request.Cookies[key];
				return true;
			}

			return false;
		}



		public IActionResult Search2(string searchColumn, string searchText)
		{
			SessionSearchStuff searchSession = new();

			if (searchColumn is not null && searchText is not null)
			{
				TryGetFromServer(searchSession, searchColumn, searchText);
			}
			else
			{
				searchSession = HttpContext.Session.Get<SessionSearchStuff>("searchSessionSteelPipe") ?? new SessionSearchStuff();
			}

			List<SteelPipe> result = new();

			if (searchSession.isSaved || TryGetFromServer(searchSession, searchColumn, searchText))
			{
				switch (searchSession.columnName)
				{
                    case "OuterDiameter":
                        result = _steelpipe
                            .Where(e => e.OuterDiameter.ToString() == searchSession.textForSearch)
                            .ToList();
                        break;
                    case "Thickness":
                        result = _steelpipe
                            .Where(e => e.Thickness.ToString() == searchSession.textForSearch)
                            .ToList();
                        break;
                    case "LinearInternalVolume":
                        result = _steelpipe
                            .Where(e => e.LinearInternalVolume.ToString() == searchSession.textForSearch)
                            .ToList();
                        break;
                    case "LinearWeight":
                        result = _steelpipe
                            .Where(e => e.LinearWeight.ToString() == searchSession.textForSearch)
                            .ToList();
                        break;
                }
			}

			ViewBag.data = result;
			return View(searchSession);
		}

		private bool TryGetFromServer(SessionSearchStuff session, string searchColumn, string searchText)
		{
			if (searchColumn is null || searchText is null) return false;
			session.Save(searchColumn, searchText, HttpContext, "searchSessionSteelPipe");
			return true;
		}
	}
}
