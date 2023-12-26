using InfoStruct.Sessions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using ModelsLibrary.Models;
using System.Threading.RateLimiting;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;

namespace ProgramASP.Controllers
{
    public class EnterpriseController : Controller
    {
        private HeatSchemeStorageContext db;
        private const int CacheDuration = 258;
        private List<Enterprise> _enterprises;

		public EnterpriseController([FromServices] HeatSchemeStorageContext context)
        {
            db = context;
            _enterprises = db.Enterprises.ToList();
        }

        //[ResponseCache(Duration = CacheDuration)]
        //public IActionResult ShowTable()
        //{
        //    ViewBag.data = _enterprises;
        //    return View();
        //}
        [ResponseCache(Duration = CacheDuration, VaryByQueryKeys = new[] { "pageNumber" })]
        public async Task<IActionResult> ShowTable(int pageNumber = 1)
        {
            var query = db.Enterprises.AsQueryable();

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
        public IActionResult Update(int enterpriceId, string name, string organization)
        {
            var PageNumber = HttpContext.Request.Query["PageNumber"];
            if (enterpriceId != 0 && name != null && organization != null)
            {
                var enterpriseToUpdate = _enterprises.Find(x => x.ID == enterpriceId);
                enterpriseToUpdate.EnterpriseName = name;
                enterpriseToUpdate.ManagementOrganization = organization;

                var enterepriseToUpdateInBD = db.Enterprises.Find(enterpriceId);
                enterepriseToUpdateInBD = enterpriseToUpdate;

                db.SaveChanges();
            }

            ViewBag.data = _enterprises;
            return RedirectToAction("ShowTable", "Enterprise", new { pageNumber = PageNumber });
        }



        [HttpGet]
        [Authorize]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [Authorize]
        public IActionResult Create(string enterpriseName, string organization)
        {
            var lastEnterprise = db.Enterprises.OrderBy(e => e.EnterpriseId).LastOrDefault();
            int lastID = lastEnterprise.EnterpriseId + 1;
            var newEnterprise = new Enterprise
            {
                EnterpriseId = lastID,
                EnterpriseName = enterpriseName,
                ManagementOrganization = organization
            };
            db.Enterprises.Add(newEnterprise);
            db.SaveChanges();

            return RedirectToAction("ShowTable", "Enterprise");
        }



        [HttpGet]
        [Authorize]
        public IActionResult Delete() 
        {
            return View();
        }

        [HttpPost]
        [Authorize]
        public IActionResult Delete(int enterpriceId)
        {
            var enterprise = db.Enterprises.Find(enterpriceId);
            if (enterprise != null)
            {
                var networks = db.HeatNetworks.Where(x => x.EnterpriseId == enterpriceId).ToList();
                var networksIds = networks.Select(x => (int?)x.NetworkId).ToList();

                var consumers = db.HeatConsumers.Where(x => networksIds.Contains(x.NetworkId)).ToList();

                var wells = db.HeatWells.Where(x => networksIds.Contains(x.NetworkId)).ToList();

                var points = db.HeatPoints.Where(x => networksIds.Contains(x.NetworkId)).ToList();

                var pointIds = points.Select(x => (int?)x.PointId).ToList();
                var sections = db.PipelineSections
                    .Where(x => pointIds.Contains(x.StartNodeNumber) || pointIds.Contains(x.EndNodeNumber)).ToList();

                
                db.PipelineSections.RemoveRange(sections);
                db.HeatPoints.RemoveRange(points);
                db.HeatWells.RemoveRange(wells);
                db.HeatConsumers.RemoveRange(consumers);
                db.HeatNetworks.RemoveRange(networks);

                db.Enterprises.Remove(enterprise);
                db.SaveChanges();
            }
            return RedirectToAction("ShowTable", "Enterprise");
        }



		public IActionResult Search(string searchColumn, string searchText)
		{
			List<string> result = new();

			if (searchColumn is null || searchText is null)
			{
				if (TryGetCookie("columnEnterprise", ref searchColumn))
				{
					ViewBag.column = searchColumn;
				}
				if (TryGetCookie("textForSearchEnterprise", ref searchText))
				{
					ViewBag.text = searchText;
				}
			}
			else
			{
				ViewBag.column = searchColumn;
				ViewBag.text = searchText;
				HttpContext.Response.Cookies.Append("columnEnterprise", searchColumn);
				HttpContext.Response.Cookies.Append("textForSearchEnterprise", searchText);
			}

			if (searchColumn is null || searchText is null) return View();

			HttpContext.Response.Cookies.Append("columnEnterprise", searchColumn);
			HttpContext.Response.Cookies.Append("textForSearchEnterprise", searchText);

			switch (searchColumn)
			{
				case "EnterpriseName":
					result = _enterprises.Select(e => e.EnterpriseName).Where(x => x.Contains(searchText)).ToList();
					break;
				case "ManagementOrganization":
					result = _enterprises.Select(e => e.ManagementOrganization).Where(x => x.Contains(searchText)).ToList();
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
                searchSession = HttpContext.Session.Get<SessionSearchStuff>("searchSessionEnterprise") ?? new SessionSearchStuff();
			}
			 
			List<string> result = new();

			if (searchSession.isSaved || TryGetFromServer(searchSession, searchColumn, searchText))
			{
				switch (searchSession.columnName)
				{
					case "EnterpriseName":
						result = _enterprises.Select(e => e.EnterpriseName).Where(x => x.Contains(searchSession.textForSearch)).ToList();
						break;
					case "ManagementOrganization":
						result = _enterprises.Select(e => e.ManagementOrganization).Where(x => x.Contains(searchSession.textForSearch)).ToList();
						break;
				}
			}

			ViewBag.data = result;
			return View(searchSession);
		}

		private bool TryGetFromServer(SessionSearchStuff session, string searchColumn, string searchText)
		{
			if (searchColumn is null || searchText is null) return false;
			session.Save(searchColumn, searchText, HttpContext, "searchSessionEnterprise");
			return true;
		}
	}
}
