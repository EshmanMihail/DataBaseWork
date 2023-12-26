using InfoStruct.Sessions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ModelsLibrary.Models;

namespace ProgramASP.Controllers
{
    public class HeatPointController : Controller
    {
        private HeatSchemeStorageContext db;
        private const int CacheDuration = 258;
        private List<HeatPoint> _heatpoints;

        public HeatPointController([FromServices] HeatSchemeStorageContext context)
        {
            db = context;
            _heatpoints = db.HeatPoints.Include(x => x.Network).ToList();

        }

        [ResponseCache(Duration = CacheDuration, VaryByQueryKeys = new[] { "pageNumber" })]
        public async Task<IActionResult> ShowTable(int pageNumber = 1)
        {
            var query = db.HeatPoints.AsQueryable();

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
        public IActionResult Update(int pointId, string pointName, int networkId, int nodeNumber)
        {
            var PageNumber = HttpContext.Request.Query["PageNumber"];
            var network = db.HeatNetworks.Find(networkId);
            if (network != null && pointId > 0 && pointName != null && nodeNumber > 0)
            {
                var pointToUpdate = _heatpoints.Find(x => x.ID == pointId);
                pointToUpdate.PointName = pointName;
                pointToUpdate.NetworkId = networkId;
                pointToUpdate.NodeNumber = nodeNumber;

                var pointToUpdateInBD = db.HeatPoints.Find(pointId);
                pointToUpdateInBD = pointToUpdate;

                db.SaveChanges();
            }

            ViewBag.data = _heatpoints;
            return RedirectToAction("ShowTable", "HeatPoint", new { pageNumber = PageNumber });
        }



        [HttpGet]
        [Authorize]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [Authorize]
        public IActionResult Create(string pointName, int networkId, int nodeNumber)
        {
            var network = db.HeatNetworks.Find(networkId);
            if (network != null && nodeNumber > 0)
            {
                var lastPoint = db.HeatPoints.OrderBy(e => e.PointId).LastOrDefault();
                int lastID = lastPoint.PointId + 1;
                var newPoint = new HeatPoint
                {
                    PointId = lastID,
                    PointName = pointName,
                    NetworkId = networkId,
                    NodeNumber = nodeNumber
                };
                db.HeatPoints.Add(newPoint);
                db.SaveChanges();
            }
            return RedirectToAction("ShowTable", "HeatPoint");
        }



        [HttpGet]
        [Authorize]
        public IActionResult Delete()
        {
            return View();
        }

        [HttpPost]
        [Authorize]
        public IActionResult Delete(int pointId) 
        {
            var point = db.HeatPoints.Find(pointId);
            if (point != null)
            {
                var pipelineSections = db.PipelineSections
                    .Where(x => x.StartNodeNumberNavigation.PointId == pointId || x.EndNodeNumberNavigation.PointId == pointId)
                    .ToList();
                db.PipelineSections.RemoveRange(pipelineSections);
                db.HeatPoints.Remove(point);
                db.SaveChanges();
            }
            return RedirectToAction("ShowTable", "HeatPoint");
        }



		public IActionResult Search(string searchColumn, string searchText)
		{
			List<string> result = new();
			if (searchColumn is null || searchText is null)
			{
				if (TryGetCookie("columnPoint", ref searchColumn))
				{
					ViewBag.column = searchColumn;
				}
				if (TryGetCookie("textForSearchPoint", ref searchText))
				{
					ViewBag.text = searchText;
				}
			}
			else
			{
				ViewBag.column = searchColumn;
				ViewBag.text = searchText;
				HttpContext.Response.Cookies.Append("columnPoint", searchColumn);
				HttpContext.Response.Cookies.Append("textForSearchPoint", searchText);
			}

			if (searchColumn is null || searchText is null) return View();

			HttpContext.Response.Cookies.Append("columnPoint", searchColumn);
			HttpContext.Response.Cookies.Append("textForSearchPoint", searchText);

			switch (searchColumn)
			{
				case "PointName":
					result = _heatpoints.Where(e => e.PointName != null && e.PointName.Contains(searchText))
						.Select(e => e.PointName).ToList();
					break;
				case "NodeNumber":
					result = _heatpoints
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
				searchSession = HttpContext.Session.Get<SessionSearchStuff>("searchSessionPoint") ?? new SessionSearchStuff();
			}

			List<string> result = new();

			if (searchSession.isSaved || TryGetFromServer(searchSession, searchColumn, searchText))
			{
				switch (searchSession.columnName)
				{
					case "PointName":
						result = _heatpoints.Where(e => e.PointName != null && e.PointName.Contains(searchSession.textForSearch))
							.Select(e => e.PointName).ToList();
						break;
					case "NodeNumber":
						result = _heatpoints
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
			session.Save(searchColumn, searchText, HttpContext, "searchSessionPoint");
			return true;
		}
	}
}
