using InfoStruct.Sessions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ModelsLibrary.Models;

namespace ProgramASP.Controllers
{
    public class HeatConsumerController : Controller
    {
        private HeatSchemeStorageContext db;
        private const int CacheDuration = 258;
        private List<HeatConsumer> _consumers;

        public HeatConsumerController([FromServices] HeatSchemeStorageContext context)
        {
            db = context;
            _consumers = db.HeatConsumers.Include(x => x.Network).OrderByDescending(x => x.ConsumerId).Take(30).ToList();
        }

        [ResponseCache(Duration = CacheDuration)]
        public IActionResult ShowTable()
        {
            ViewBag.data = _consumers;
            return View();
        }

        [HttpPost]
        public IActionResult Update(int consumerId, string consumerName, int networkId, int nodeNumber, decimal calculatedPower)
        {
            var network = db.HeatNetworks.Find(networkId);
            if (consumerId != 0 && consumerName != null && network != null && nodeNumber > 0 && calculatedPower >= 0)
            {
                var consumerToUpdate = _consumers.Find(x => x.ID == consumerId);
                consumerToUpdate.ConsumerName = consumerName;
                consumerToUpdate.NetworkId = networkId;
                consumerToUpdate.NodeNumber = nodeNumber;
                consumerToUpdate.CalculatedPower = calculatedPower;

                var consumerToUpdateInBD = db.HeatConsumers.Find(consumerId);
                consumerToUpdateInBD = consumerToUpdate;

                db.SaveChanges();
            }

            ViewBag.data = _consumers;
            return View("ShowTable");
        }



        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(string consumerName, int networkId, int nodeNumber, decimal calculatedPower)
        {
            var network = db.HeatNetworks.Find(networkId);
            if (network != null && nodeNumber > 0 && calculatedPower >= 0)
            {
                var lastConsumer = db.HeatConsumers.OrderBy(e => e.ConsumerId).LastOrDefault();
                int lastID = lastConsumer.ConsumerId + 1;
                var newConsumer = new HeatConsumer
                {
                    ConsumerId = lastID,
                    ConsumerName = consumerName,
                    NetworkId = networkId,
                    NodeNumber = nodeNumber,
                    CalculatedPower = calculatedPower
                };
                db.HeatConsumers.Add(newConsumer);
                db.SaveChanges();
            }
            return RedirectToAction("ShowTable", "HeatConsumer");
        }



        [HttpGet]
        public IActionResult Delete()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Delete(int consumerId)
        {
            var consumer = db.HeatConsumers.Find(consumerId);
            if (consumer != null)
            {
                db.HeatConsumers.Remove(consumer);
                db.SaveChanges();
            }
            return RedirectToAction("ShowTable", "HeatConsumer");
        }



		public IActionResult Search(string searchColumn, string searchText)
		{
			List<string> result = new();
			if (searchColumn is null || searchText is null)
            {
				if (TryGetCookie("columnConsumer", ref searchColumn))
				{
					ViewBag.column = searchColumn;
				}
				if (TryGetCookie("textForSearchConsumer", ref searchText))
				{
					ViewBag.text = searchText;
				}
			}
			else
			{
				ViewBag.column = searchColumn;
				ViewBag.text = searchText;
				HttpContext.Response.Cookies.Append("columnConsumer", searchColumn);
				HttpContext.Response.Cookies.Append("textForSearchConsumer", searchText);
			}

			if (searchColumn is null || searchText is null) return View();

			HttpContext.Response.Cookies.Append("columnConsumer", searchColumn);
			HttpContext.Response.Cookies.Append("textForSearchConsumer", searchText);

			switch (searchColumn)
			{
				case "ConsumerName":
					result = _consumers.Where(e => e.ConsumerName != null && e.ConsumerName.Contains(searchText))
						.Select(e => e.ConsumerName).ToList();
					break;
				case "NodeNumber":
					result = _consumers
                        .Where(e => e.NodeNumber.HasValue && e.NodeNumber.Value.ToString().Contains(searchText))
						.Select(e => e.NodeNumber.Value.ToString())
                        .ToList();
					break;
				case "CalculatedPower":
					result = _consumers
		                .Where(e => e.CalculatedPower.HasValue && e.CalculatedPower.Value.ToString().Contains(searchText))
		                .Select(e => e.CalculatedPower.Value.ToString())
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
				searchSession = HttpContext.Session.Get<SessionSearchStuff>("searchSessionConsumer") ?? new SessionSearchStuff();
			}

			List<string> result = new();

			if (searchSession.isSaved || TryGetFromServer(searchSession, searchColumn, searchText))
            {
                switch (searchSession.columnName)
                {
                    case "ConsumerName":
						result = _consumers.Where(e => e.ConsumerName != null && e.ConsumerName.Contains(searchSession.textForSearch))
						.Select(e => e.ConsumerName).ToList();
						break;
                    case "ManagementOrganization":
                        result = _consumers.Where(e => e.NodeNumber.HasValue && e.NodeNumber.Value.ToString().Contains(searchSession.textForSearch))
                            .Select(e => e.NodeNumber.Value.ToString()).ToList();
                        break;
                    case "CalculatedPower":
                        result = _consumers.Select(e => e.CalculatedPower).Where(x => x.HasValue && x.Value.ToString().Contains(searchSession.textForSearch))
                            .Select(x => x.Value.ToString()).ToList();
                        break;
                }
            }

            ViewBag.data = result;
			return View(searchSession);
		}

		private bool TryGetFromServer(SessionSearchStuff session, string searchColumn, string searchText)
		{
			if (searchColumn is null || searchText is null) return false;
			session.Save(searchColumn, searchText, HttpContext, "searchSessionConsumer");
			return true;
		}
	}
}
