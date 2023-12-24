using InfoStruct.Sessions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
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

        [ResponseCache(Duration = CacheDuration)]
        public IActionResult ShowTable()
        {
            ViewBag.data = _steelpipe;
            return View();
        }

        [HttpPost]
        public IActionResult Update(int pipeId, decimal outerDiameter, decimal thickness, decimal linearInternalVolume, decimal linearWeight)
        {
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
            return View("ShowTable");
        }



        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
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
        public IActionResult Delete()
        {
            return View();
        }

        [HttpPost]
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
			List<string> result = new();
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
						.Where(e => e.OuterDiameter.HasValue && e.OuterDiameter.Value.ToString().Contains(searchText))
						.Select(e => e.OuterDiameter.Value.ToString())
						.ToList();
					break;
				case "Thickness":
					result = _steelpipe
						.Where(e => e.Thickness.HasValue && e.Thickness.Value.ToString().Contains(searchText))
						.Select(e => e.Thickness.Value.ToString())
						.ToList();
					break;
				case "LinearInternalVolume":
					result = _steelpipe
						.Where(e => e.LinearInternalVolume.HasValue && e.LinearInternalVolume.Value.ToString().Contains(searchText))
						.Select(e => e.LinearInternalVolume.Value.ToString())
						.ToList();
					break;
				case "LinearWeight":
					result = _steelpipe
						.Where(e => e.LinearWeight.HasValue && e.LinearWeight.Value.ToString().Contains(searchText))
						.Select(e => e.LinearWeight.Value.ToString())
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

			List<string> result = new();

			if (searchSession.isSaved || TryGetFromServer(searchSession, searchColumn, searchText))
			{
				switch (searchSession.columnName)
				{
					case "OuterDiameter":
						result = _steelpipe
							.Where(e => e.OuterDiameter.HasValue && e.OuterDiameter.Value.ToString().Contains(searchSession.textForSearch))
							.Select(e => e.OuterDiameter.Value.ToString())
							.ToList();
						break;
					case "Thickness":
						result = _steelpipe
							.Where(e => e.Thickness.HasValue && e.Thickness.Value.ToString().Contains(searchSession.textForSearch))
							.Select(e => e.Thickness.Value.ToString())
							.ToList();
						break;
					case "LinearInternalVolume":
						result = _steelpipe
							.Where(e => e.LinearInternalVolume.HasValue && e.LinearInternalVolume.Value.ToString().Contains(searchSession.textForSearch))
							.Select(e => e.LinearInternalVolume.Value.ToString())
							.ToList();
						break;
					case "LinearWeight":
						result = _steelpipe
							.Where(e => e.LinearWeight.HasValue && e.LinearWeight.Value.ToString().Contains(searchSession.textForSearch))
							.Select(e => e.LinearWeight.Value.ToString())
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
