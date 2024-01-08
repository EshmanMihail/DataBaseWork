using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ModelsLibrary.Models;

namespace ProgramWebApi.Controllers
{
    [ApiController]
    [Route("{controller}")]
    public class HeatNetworkController : ControllerBase
    {
        private HeatSchemeStorageContext _context;

        public HeatNetworkController(HeatSchemeStorageContext context) => _context = context;    

        [HttpGet]
        [Route("/Get")]
        public IEnumerable<HeatNetwork> ShowTable() => _context.HeatNetworks.ToList();

        [HttpPost]
        [Route("/Add")]
        public IActionResult Create(string networkName, int networkNumber, int enterpriseId, string networkType)
        {
            var enterprise = _context.Enterprises.Find(enterpriseId);
            if (enterprise != null && networkNumber > 0)
            {
                var lastNetwork = _context.HeatNetworks.OrderBy(e => e.NetworkId).LastOrDefault();
                int lastID = lastNetwork.NetworkId + 1;
                var newNetwork = new HeatNetwork
                {
                    NetworkId = lastID,
                    NetworkName = networkName,
                    NetworkNumber = networkNumber,
                    EnterpriseId = enterpriseId,
                    NetworkType = networkType
                };
                _context.HeatNetworks.Add(newNetwork);
                _context.SaveChanges();

            }
            return NoContent();
        }

        [HttpPut]
        [Route("/update")]
        public IActionResult Update(int id, string networkName, int networkNumber, int enterpriseId, string networkType)
        {
            var PageNumber = HttpContext.Request.Query["PageNumber"];
            var enterprise = _context.Enterprises.Find(enterpriseId);

            if (id != 0 && networkName != null && networkNumber > 0 && enterprise != null && networkType != null)
            {
                var networkToUpdate = _context.HeatNetworks.ToList().Find(x => x.ID == id);
                networkToUpdate.NetworkName = networkName;
                networkToUpdate.NetworkNumber = networkNumber;
                networkToUpdate.EnterpriseId = enterpriseId;
                networkToUpdate.NetworkType = networkType;

                var networkToUpdateInBD = _context.HeatNetworks.Find(id);
                networkToUpdateInBD = networkToUpdate;

                _context.SaveChanges();
            }

            return NoContent();
        }

        [HttpDelete]
        [Route("/Delete")]
        public IActionResult Delete(int networkId)
        {
            var network = _context.HeatNetworks.Find(networkId);
            if (network != null)
            {
                var consumers = _context.HeatConsumers.Where(x => x.NetworkId == networkId).ToList();

                var wells = _context.HeatWells.Where(x => x.NetworkId == networkId).ToList();

                var points = _context.HeatPoints.Where(x => x.NetworkId == networkId).ToList();

                var pointIds = points.Select(p => (int?)p.PointId).ToList();
                var sections = _context.PipelineSections
                    .Where(x => pointIds.Contains(x.StartNodeNumber) || pointIds.Contains(x.EndNodeNumber)).ToList();

                _context.PipelineSections.RemoveRange(sections);
                _context.HeatPoints.RemoveRange(points);
                _context.HeatWells.RemoveRange(wells);
                _context.HeatConsumers.RemoveRange(consumers);

                _context.HeatNetworks.Remove(network);
                _context.SaveChanges();
            }
            return NoContent();
        }
    }
}
