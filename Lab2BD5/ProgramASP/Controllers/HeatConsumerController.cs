using InfoStruct.Sessions;
using Microsoft.AspNetCore.Authorization;
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
            _consumers = db.HeatConsumers.Include(x => x.Network).ToList();
        }

        //[ResponseCache(Duration = CacheDuration)]
        //public IActionResult ShowTable()
        //{
        //    ViewBag.data = _consumers;
        //    return View();
        //}
        [ResponseCache(Duration = CacheDuration, VaryByQueryKeys = new[] { "pageNumber" })]
        public async Task<IActionResult> ShowTable(int pageNumber = 1)
        {
            var query = db.HeatConsumers.AsQueryable();

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
        public IActionResult Update(int consumerId, string consumerName, int networkId, int nodeNumber, decimal calculatedPower)
        {
            var PageNumber = HttpContext.Request.Query["PageNumber"];

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
            return RedirectToAction("ShowTable", "HeatConsumer", new { pageNumber = PageNumber });
        }



        [HttpGet]
        [Authorize]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [Authorize]
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
        [Authorize]
        public IActionResult Delete()
        {
            return View();
        }

        [HttpPost]
        [Authorize]
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
			List<HeatConsumer> result = new();
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
					result = _consumers.Where(e => e.ConsumerName != null && e.ConsumerName.Contains(searchText)).ToList();
                    break;
                case "NodeNumber":
                    result = _consumers.Where(e => e.NodeNumber.ToString() == searchText).ToList();
                    break;
                case "CalculatedPower":
                    result = _consumers
                   .Where(e => e.CalculatedPower.ToString() == searchText)
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

			List<HeatConsumer> result = new();

			if (searchSession.isSaved || TryGetFromServer(searchSession, searchColumn, searchText))
            {
                switch (searchSession.columnName)
                {
                    case "ConsumerName":
						result = _consumers.Where(e => e.ConsumerName != null && e.ConsumerName.Contains(searchSession.textForSearch)).ToList();
						break;
                    case "NodeNumber":
                        result = _consumers.Where(e => e.NodeNumber.ToString() == searchSession.textForSearch).ToList();
                        break;
                    case "CalculatedPower":
                        result = _consumers
                       .Where(e => e.CalculatedPower.ToString() == searchSession.textForSearch)
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
			session.Save(searchColumn, searchText, HttpContext, "searchSessionConsumer");
			return true;
		}
	}
}
