using InfoStruct.Sessions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ModelsLibrary.Models;

namespace ProgramASP.Controllers
{
    public class PipelineSectionController : Controller
    {
        private HeatSchemeStorageContext db;
        private const int CacheDuration = 258;
        private List<PipelineSection> _pipelines;

        public PipelineSectionController([FromServices] HeatSchemeStorageContext context)
        {
            db = context;
            _pipelines = db.PipelineSections.Include(ps => ps.StartNodeNumberNavigation)
                                            .Include(ps => ps.EndNodeNumberNavigation)
                                            .OrderByDescending(x => x.SectionId).Take(30).ToList();
        }

        [ResponseCache(Duration = CacheDuration)]
        public IActionResult ShowTable()
        {
            ViewBag.data = _pipelines;
            return View();
        }

        [HttpPost]
        public IActionResult Update(int sectionId, int sectionNumber, int startNodeNumber, int endNodeNumber,
            decimal pipelineLength, decimal diameter, decimal thickness, DateOnly lastRepairDate)
        {
            var heatPointFirst = db.HeatPoints.Find(startNodeNumber);
            var heatPointSecond = db.HeatPoints.Find(endNodeNumber);
            if (sectionId > 0 && sectionNumber > 0 && heatPointFirst != null && heatPointSecond != null
                && pipelineLength > 0 && diameter > 0 && thickness > 0)
            {
                var pipelineToUpdate = _pipelines.Find(x => x.ID == sectionId);
                pipelineToUpdate.SectionNumber = sectionNumber;
                pipelineToUpdate.StartNodeNumber = startNodeNumber;
                pipelineToUpdate.EndNodeNumber= endNodeNumber;
                pipelineToUpdate.PipelineLength = pipelineLength;
                pipelineToUpdate.Diameter = diameter;
                pipelineToUpdate.Thickness = thickness;
                pipelineToUpdate.LastRepairDate = lastRepairDate;

                var pipelineToUpdateInBD = db.PipelineSections.Find(sectionId);
                pipelineToUpdateInBD = pipelineToUpdate;

                db.SaveChanges();
            }

            ViewBag.data = _pipelines;
            return View("ShowTable");
        }



        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(int sectionNumber, int startNodeNumber, int endNodeNumber,
            decimal pipelineLength, decimal diameter, decimal thickness, DateOnly lastRepairDate)
        {
            var heatPointFirst = db.HeatPoints.Find(startNodeNumber);
            var heatPointSecond = db.HeatPoints.Find(endNodeNumber);
            if (sectionNumber > 0 && heatPointFirst != null && heatPointSecond != null
                && pipelineLength > 0 && diameter > 0 && thickness > 0)
            {
                var lastPipeline = db.PipelineSections.OrderBy(e => e.SectionId).LastOrDefault();
                int lastID = lastPipeline.SectionId + 1;
                var newPipeline = new PipelineSection
                {
                    SectionId = lastID,
                    SectionNumber = sectionNumber,
                    StartNodeNumber = startNodeNumber,
                    EndNodeNumber = endNodeNumber,
                    PipelineLength = pipelineLength,
                    Diameter = diameter,
                    Thickness = thickness,
                    LastRepairDate = lastRepairDate

                };
                db.PipelineSections.Add(newPipeline);
                db.SaveChanges();
            }
            return RedirectToAction("ShowTable", "PipelineSection");
        }



        [HttpGet]
        public IActionResult Delete()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Delete(int pipelineId)
        {
            var pipiline = db.PipelineSections.Find(pipelineId);
            if (pipiline != null)
            {
                db.PipelineSections.Remove(pipiline);
                db.SaveChanges();
            }
            return RedirectToAction("ShowTable", "PipelineSection");
        }



		public IActionResult Search(string searchColumn, string searchText)
		{
			List<string> result = new();
			if (searchColumn is null || searchText is null)
			{
				if (TryGetCookie("columnPipe", ref searchColumn))
				{
					ViewBag.column = searchColumn;
				}
				if (TryGetCookie("textForSearchPipe", ref searchText))
				{
					ViewBag.text = searchText;
				}
			}
			else
			{
				ViewBag.column = searchColumn;
				ViewBag.text = searchText;
				HttpContext.Response.Cookies.Append("columnPipe", searchColumn);
				HttpContext.Response.Cookies.Append("textForSearchPipe", searchText);
			}

			if (searchColumn is null || searchText is null) return View();

			HttpContext.Response.Cookies.Append("columnPipe", searchColumn);
			HttpContext.Response.Cookies.Append("textForSearchPipe", searchText);

			switch (searchColumn)
			{
				case "PipelineLength":
					result = _pipelines
						.Where(e => e.PipelineLength.HasValue && e.PipelineLength.Value.ToString().Contains(searchText))
						.Select(e => e.PipelineLength.Value.ToString())
						.ToList();
					break;
				case "Diameter":
					result = _pipelines
						.Where(e => e.Diameter.HasValue && e.Diameter.Value.ToString().Contains(searchText))
						.Select(e => e.Diameter.Value.ToString())
						.ToList();
					break;
				case "Thickness":
					result = _pipelines
						.Where(e => e.Thickness.HasValue && e.Thickness.Value.ToString().Contains(searchText))
						.Select(e => e.Thickness.Value.ToString())
						.ToList();
					break;
				case "LastRepairDate":
					result = _pipelines
						.Where(e => e.LastRepairDate.HasValue && e.LastRepairDate.Value.ToString().Contains(searchText))
						.Select(e => e.LastRepairDate.Value.ToString())
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
				searchSession = HttpContext.Session.Get<SessionSearchStuff>("searchSessionPipe") ?? new SessionSearchStuff();
			}

			List<string> result = new();

			if (searchSession.isSaved || TryGetFromServer(searchSession, searchColumn, searchText))
			{
				switch (searchSession.columnName)
				{
					case "PipelineLength":
						result = _pipelines
							.Where(e => e.PipelineLength.HasValue && e.PipelineLength.Value.ToString().Contains(searchSession.textForSearch))
							.Select(e => e.PipelineLength.Value.ToString())
							.ToList();
						break;
					case "Diameter":
						result = _pipelines
							.Where(e => e.Diameter.HasValue && e.Diameter.Value.ToString().Contains(searchSession.textForSearch))
							.Select(e => e.Diameter.Value.ToString())
							.ToList();
						break;
					case "Thickness":
						result = _pipelines
							.Where(e => e.Thickness.HasValue && e.Thickness.Value.ToString().Contains(searchSession.textForSearch))
							.Select(e => e.Thickness.Value.ToString())
							.ToList();
						break;
					case "LastRepairDate":
						result = _pipelines
							.Where(e => e.LastRepairDate.HasValue && e.LastRepairDate.Value.ToString().Contains(searchSession.textForSearch))
							.Select(e => e.LastRepairDate.Value.ToString())
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
			session.Save(searchColumn, searchText, HttpContext, "searchSessionPipe");
			return true;
		}
	}
}
