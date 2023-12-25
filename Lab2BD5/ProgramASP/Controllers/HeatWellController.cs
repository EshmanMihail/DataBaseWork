using InfoStruct.Sessions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ModelsLibrary.Models;

namespace ProgramASP.Controllers
{
    public class HeatWellController : Controller
    {
        private HeatSchemeStorageContext db;
        private const int CacheDuration = 258;
        private List<HeatWell> _heatwells;

        public HeatWellController([FromServices] HeatSchemeStorageContext context)
        {
            db = context;
            _heatwells = db.HeatWells.Include(x => x.Network).OrderByDescending(x => x.WellId).Take(30).ToList();
        }

        [ResponseCache(Duration = CacheDuration)]
        public IActionResult ShowTable()
        {
            ViewBag.data = _heatwells;
            return View();
        }

        [HttpPost]
        [Authorize]
        public IActionResult Update(int wellId, string wellName, int networkId, int nodeNumber)
        {
            var network = db.HeatNetworks.Find(networkId);
            if (wellId != 0 && wellName != null && network != null && nodeNumber > 0)
            {
                var wellToUpdate = _heatwells.Find(x => x.ID == wellId);
                wellToUpdate.WellName = wellName;
                wellToUpdate.NetworkId = networkId;
                wellToUpdate.NodeNumber = nodeNumber;

                var wellToUpdateInBD = db.HeatWells.Find(wellId);
                wellToUpdateInBD = wellToUpdate;

                db.SaveChanges();
            }
            ViewBag.data = _heatwells;
            return View("ShowTable");
        }



        [HttpGet]
        [Authorize]
        public IActionResult Create() 
        { 
            return View(); 
        }

        [HttpPost]
        [Authorize]
        public IActionResult Create(string wellName, int networkId, int nodeNumber)
        {
            var network = db.HeatNetworks.Find(networkId);
            if (network != null && nodeNumber > 0)
            {
                var lastWell = db.HeatWells.OrderBy(e => e.WellId).LastOrDefault();
                int lastID = lastWell.WellId + 1;
                var newWell = new HeatWell
                {
                    WellId = lastID,
                    WellName = wellName,
                    NetworkId = networkId,
                    NodeNumber = nodeNumber
                };
                db.HeatWells.Add(newWell);
                db.SaveChanges();
            }
            return RedirectToAction("ShowTable", "HeatWell");
        }



        [HttpGet]
        [Authorize]
        public IActionResult Delete()
        {
            return View();
        }

        [HttpPost]
        [Authorize]
        public IActionResult Delete(int wellId)
        {
            var well = db.HeatWells.Find(wellId);
            if (well != null)
            {
                db.HeatWells.Remove(well);
                db.SaveChanges();
            }
            return RedirectToAction("ShowTable", "HeatWell");
        }



		public IActionResult Search(string searchColumn, string searchText)
		{
			List<string> result = new();
			if (searchColumn is null || searchText is null)
			{
				if (TryGetCookie("columnWell", ref searchColumn))
				{
					ViewBag.column = searchColumn;
				}
				if (TryGetCookie("textForSearchWell", ref searchText))
				{
					ViewBag.text = searchText;
				}
			}
			else
			{
				ViewBag.column = searchColumn;
				ViewBag.text = searchText;
				HttpContext.Response.Cookies.Append("columnWell", searchColumn);
				HttpContext.Response.Cookies.Append("textForSearchWell", searchText);
			}

			if (searchColumn is null || searchText is null) return View();

			HttpContext.Response.Cookies.Append("columnWell", searchColumn);
			HttpContext.Response.Cookies.Append("textForSearchWell", searchText);

			switch (searchColumn)
			{
				case "WellName":
					result = _heatwells.Where(e => e.WellName != null && e.WellName.Contains(searchText))
						.Select(e => e.WellName).ToList();
					break;
				case "NodeNumber":
					result = _heatwells
						.Where(e => e.NodeNumber.HasValue && e.NodeNumber.Value.ToString().Contains(searchText))
						.Select(e => e.NodeNumber.Value.ToString())
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
				searchSession = HttpContext.Session.Get<SessionSearchStuff>("searchSessionWell") ?? new SessionSearchStuff();
			}

			List<string> result = new();

			if (searchSession.isSaved || TryGetFromServer(searchSession, searchColumn, searchText))
			{
				switch (searchSession.columnName)
				{
					case "WellName":
						result = _heatwells.Where(e => e.WellName != null && e.WellName.Contains(searchSession.textForSearch))
							.Select(e => e.WellName).ToList();
						break;
					case "NodeNumber":
						result = _heatwells
							.Where(e => e.NodeNumber.HasValue && e.NodeNumber.Value.ToString().Contains(searchSession.textForSearch))
							.Select(e => e.NodeNumber.Value.ToString())
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
			session.Save(searchColumn, searchText, HttpContext, "searchSessionWell");
			return true;
		}
	}
}
