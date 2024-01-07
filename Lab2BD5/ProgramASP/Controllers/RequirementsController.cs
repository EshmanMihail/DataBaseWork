using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ModelsLibrary.Models;

namespace ProgramASP.Controllers
{
    public class RequirementsController : Controller
    {
        private HeatSchemeStorageContext db;

        public RequirementsController([FromServices] HeatSchemeStorageContext context)
        {
            db = context;
        }

        [HttpGet]
        public IActionResult RequirementsShow()
        {
            return View();
        }

        [HttpGet]
        public IActionResult FirstReq()
        {
            var networks = db.HeatNetworks.Take(50).ToList();

            var waterVolumes = new List<object>();

            foreach (var network in networks)
            {
                var points = db.HeatPoints.Where(x => x.NetworkId == network.ID).ToList();

                decimal waterVolume = 0;

                foreach (var point in points)
                {
                    var sections = db.PipelineSections.Where(x => x.StartNodeNumber == point.PointId || x.EndNodeNumber == point.PointId).ToList();

                    foreach (var section in sections)
                    {
                        waterVolume += (decimal)((decimal)3.14159 * (section.Diameter / 2) * (section.Diameter / 2) * section.PipelineLength);
                    }
                }
                var result = new
                {
                    NetworkName = network.NetworkName,
                    WaterVolume = waterVolume
                };

                waterVolumes.Add(result);
            }

            ViewBag.Result = waterVolumes;

            return View();
        }

        [HttpGet]
        public IActionResult SecondReq()
        {
            var networks = db.HeatNetworks.Take(30).ToList();

            var steelpipe = db.SteelPipes.Take(1).ToList();

            var resautl = new List<object>();

            foreach (var network in networks)
            {
                var points = db.HeatPoints.Where(x => x.NetworkId == network.ID).ToList();

                decimal weight = 0;

                foreach (var point in points)
                {
                    var sections = db.PipelineSections.Where(x => x.StartNodeNumber == point.PointId || x.EndNodeNumber == point.PointId).ToList();

                    foreach (var section in sections)
                    {
                        weight += (decimal)(section.PipelineLength * steelpipe[0].LinearWeight);
                    }
                }

                var consumers =  db.HeatConsumers.Where(x => x.NetworkId == network.ID).ToList();
                decimal totalCalculatedPower = 0;
                foreach (var consumer in consumers)
                {
                    totalCalculatedPower += (decimal)consumer.CalculatedPower; 
                }

                var result = new
                {
                    NetworkName = network.NetworkName,
                    Weight = weight,
                    TotalCalculatedPower = totalCalculatedPower
                };
                resautl.Add(result);
            }

            ViewBag.Result = resautl;

            return View();
        }


        [HttpGet]
        public IActionResult ThirdReq()
        {
            return View();
        }
        [HttpPost]
        public IActionResult ThirdReq(string enterpriseName, decimal diameter, decimal thickness)
        {
            var enterprise = db.Enterprises.Where(x => x.EnterpriseName == enterpriseName).FirstOrDefault();

            decimal totalLength = 0;
            List<PipelineSection> resault = new();

            if (enterprise != null)
            {
                var networks = db.HeatNetworks.Where(x => x.EnterpriseId == enterprise.EnterpriseId).ToList();
                var networksIds = networks.Select(x => (int?)x.NetworkId).ToList();

                var points = db.HeatPoints.Where(x => networksIds.Contains(x.NetworkId)).ToList();
                var pointIds = points.Select(x => (int?)x.PointId).ToList();

                resault = db.PipelineSections
                    .Where(x => pointIds.Contains(x.StartNodeNumber) || pointIds.Contains(x.EndNodeNumber)).ToList();

                if (diameter > 0 && thickness > 0)
                {
                    resault = resault.Where(x => x.Diameter == diameter && x.Thickness == thickness).ToList();
                    
                    foreach (var section in resault)
                    {
                        totalLength += (decimal)section.PipelineLength;
                    }
                }
                else if (thickness > 0 && diameter == 0)
                {
                    resault = resault.Where(x => x.Thickness == thickness).ToList();

                    foreach (var section in resault)
                    {
                        totalLength += (decimal)section.PipelineLength;
                    }
                }
                else if (diameter > 0 && thickness == 0)
                {
                    resault = resault.Where(x => x.Diameter == diameter).ToList();

                    foreach (var section in resault)
                    {
                        totalLength += (decimal)section.PipelineLength;
                    }
                }
                else
                {
                    foreach (var section in resault)
                    {
                        totalLength += (decimal)section.PipelineLength;
                    }
                }
            }
            ViewBag.data = resault;
            ViewBag.Length = totalLength;
            return View();
        }

        [HttpGet]
        public IActionResult ForthReq()
        {
            return View();
        }

        [HttpPost]
        public IActionResult ForthReq(DateTime startDate, DateTime endDate) 
        {
            var start = new DateOnly(startDate.Year, startDate.Month, startDate.Day);
            var end = new DateOnly(endDate.Year, endDate.Month, endDate.Day);

            var data = db.PipelineSections
                .Where(section => section.LastRepairDate >= start && section.LastRepairDate <= end)
                .Include(section => section.StartNodeNumberNavigation.Network)
                .ThenInclude(network => network.Enterprise)
                .Include(section => section.EndNodeNumberNavigation.Network)
                .ThenInclude(network => network.Enterprise)
                .ToList();

            ViewBag.data = data;

            return View();
        }
    }
}
