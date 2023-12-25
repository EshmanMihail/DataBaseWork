using InfoStruct.Sessions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ModelsLibrary.Models;
using System.Linq;

namespace ProgramASP.Controllers
{
    public class HeatNetworkController : Controller
    {
        private HeatSchemeStorageContext db;
        private const int CacheDuration = 258;
        private List<HeatNetwork> _networks;

        public HeatNetworkController([FromServices] HeatSchemeStorageContext context)
        {
            db = context;
            _networks = db.HeatNetworks.Include(x => x.Enterprise).OrderByDescending(x => x.NetworkId).Take(30).ToList();
        }

        [ResponseCache(Duration = CacheDuration)]
        public IActionResult ShowTable()
        {
            ViewBag.data = _networks;
            return View();
        }

        [HttpPost]
        [Authorize]
        public IActionResult Update(int networkId, string networkName, int networkNumber, int enterpriseId, string networkType) 
        {
            var enterprise = db.Enterprises.Find(enterpriseId);
            if (networkId != 0 && networkName != null && networkNumber > 0 && enterprise != null && networkType != null)
            {
                var networkToUpdate = _networks.Find(x => x.ID == networkId);
                networkToUpdate.NetworkName = networkName;
                networkToUpdate.NetworkNumber = networkNumber;
                networkToUpdate.EnterpriseId = enterpriseId;
                networkToUpdate.NetworkType = networkType;

                var networkToUpdateInBD = db.HeatNetworks.Find(networkId);
                networkToUpdateInBD = networkToUpdate;

                db.SaveChanges();
            }
            ViewBag.data = _networks; 
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
        public IActionResult Create(string networkName, int networkNumber, int enterpriseId, string networkType)
        {
            var enterprise = db.Enterprises.Find(enterpriseId);
            if (enterprise != null && networkNumber > 0)
            {
                var lastNetwork = db.HeatNetworks.OrderBy(e => e.NetworkId).LastOrDefault();
                int lastID = lastNetwork.NetworkId + 1;
                var newNetwork = new HeatNetwork
                {
                    NetworkId = lastID,
                    NetworkName = networkName,
                    NetworkNumber = networkNumber,
                    EnterpriseId = enterpriseId,
                    NetworkType = networkType
                };
                db.HeatNetworks.Add(newNetwork);
                db.SaveChanges();
                
            }
            return RedirectToAction("ShowTable", "HeatNetwork");
        }



        [HttpGet]
        [Authorize]
        public IActionResult Delete()
        {
            return View();
        }

        [HttpPost]
        [Authorize]
        public IActionResult Delete(int networkId)
        {
            var network = db.HeatNetworks.Find(networkId);
            if (network != null)
            {
                var consumers = db.HeatConsumers.Where(x => x.NetworkId == networkId).ToList();

                var wells = db.HeatWells.Where(x => x.NetworkId == networkId).ToList();

                var points = db.HeatPoints.Where(x => x.NetworkId == networkId).ToList();

                var pointIds = points.Select(p => (int?)p.PointId).ToList();
                var sections = db.PipelineSections
                    .Where(x => pointIds.Contains(x.StartNodeNumber) || pointIds.Contains(x.EndNodeNumber)).ToList();

                db.PipelineSections.RemoveRange(sections);
                db.HeatPoints.RemoveRange(points);
                db.HeatWells.RemoveRange(wells);
                db.HeatConsumers.RemoveRange(consumers);

                db.HeatNetworks.Remove(network);
                db.SaveChanges();
            }
            return RedirectToAction("ShowTable", "HeatNetwork");
        }



		public IActionResult Search(string searchColumn, string searchText)
		{
			List<string> result = new();

			if (searchColumn is null || searchText is null)
			{
				if (TryGetCookie("columnNetwork", ref searchColumn))
				{
					ViewBag.column = searchColumn;
				}
				if (TryGetCookie("textForSearchNetwork", ref searchText))
				{
					ViewBag.text = searchText;
				}
			}
			else
			{
				ViewBag.column = searchColumn;
				ViewBag.text = searchText;
				HttpContext.Response.Cookies.Append("columnNetwork", searchColumn);
				HttpContext.Response.Cookies.Append("textForSearchNetwork", searchText);
			}

			if (searchColumn is null || searchText is null) return View();

			HttpContext.Response.Cookies.Append("columnNetwork", searchColumn);
			HttpContext.Response.Cookies.Append("textForSearchNetwork", searchText);

			switch (searchColumn)
			{
				case "NetworkName":
					result = _networks.Select(e => e.NetworkName).Where(x => x.Contains(searchText)).ToList();
					break;
				case "NetworkNumber":
					result = _networks
						.Where(e => e.NetworkNumber.HasValue && e.NetworkNumber.Value.ToString().Contains(searchText))
						.Select(e => e.NetworkNumber.Value.ToString())
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
				searchSession = HttpContext.Session.Get<SessionSearchStuff>("searchSessionNetwork") ?? new SessionSearchStuff();
			}

			List<string> result = new();

			if (searchSession.isSaved || TryGetFromServer(searchSession, searchColumn, searchText))
			{
				switch (searchSession.columnName)
				{
					case "NetworkName":
						result = _networks.Select(e => e.NetworkName).Where(x => x.Contains(searchSession.textForSearch)).ToList();
						break;
					case "NetworkNumber":
						result = _networks
							.Where(e => e.NetworkNumber.HasValue && e.NetworkNumber.Value.ToString().Contains(searchSession.textForSearch))
							.Select(e => e.NetworkNumber.Value.ToString())
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
			session.Save(searchColumn, searchText, HttpContext, "searchSessionNetwork");
			return true;
		}
	}
}
