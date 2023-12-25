using Microsoft.AspNetCore.Mvc;
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

        [HttpPost]
        public IActionResult FirstTask()
        {
            var waterVolumes = from enterprise in db.Enterprises
                               join heatNetwork in db.HeatNetworks on enterprise.EnterpriseId equals heatNetwork.EnterpriseId
                               join heatConsumer in db.HeatConsumers on heatNetwork.NetworkId equals heatConsumer.NetworkId
                               group heatConsumer by new { enterprise.EnterpriseName, enterprise.ManagementOrganization } into g
                               select new
                               {
                                   g.Key.EnterpriseName,
                                   g.Key.ManagementOrganization,
                                   WaterVolume = g.Sum(hc => hc.CalculatedPower)
                               };
            return View();
        }


        [HttpPost]
        public IActionResult SecondTask()
        {
            var heatNetworkData = from heatNetwork in db.HeatNetworks
                                  join heatWell in db.HeatWells on heatNetwork.NetworkId equals heatWell.NetworkId into heatWells
                                  join heatConsumer in db.HeatConsumers on heatNetwork.NetworkId equals heatConsumer.NetworkId into heatConsumers
                                  join steelPipe in db.SteelPipes on heatNetwork.NetworkId equals steelPipe.PipeId into steelPipes
                                  select new
                                  {
                                      heatNetwork.NetworkName,
                                      MetalWeight = steelPipes.Sum(sp => sp.LinearWeight),
                                      TotalPower = heatConsumers.Sum(hc => hc.CalculatedPower)
                                  };
            return View();
        }

        [HttpPost]
        public IActionResult ThirdTask()
        {
            var steelPipeData = from enterprise in db.Enterprises
                                join heatNetwork in db.HeatNetworks on enterprise.EnterpriseId equals heatNetwork.EnterpriseId
                                join steelPipe in db.SteelPipes on heatNetwork.NetworkId equals steelPipe.PipeId
                                group steelPipe by new { enterprise.EnterpriseName, steelPipe.OuterDiameter } into g
                                select new
                                {
                                    g.Key.EnterpriseName,
                                    g.Key.OuterDiameter,
                                    TotalLength = g.Sum(sp => sp.LinearInternalVolume)
                                };
            return View();
        }

        [HttpPost]
        public IActionResult FoursTask()
        {
            var repairData = from pipelineSection in db.PipelineSections
                             join startHeatPoint in db.HeatPoints on pipelineSection.StartNodeNumber equals startHeatPoint.PointId
                             join endHeatPoint in db.HeatPoints on pipelineSection.EndNodeNumber equals endHeatPoint.PointId
                             join heatNetwork in db.HeatNetworks on startHeatPoint.NetworkId equals heatNetwork.NetworkId
                             join enterprise in db.Enterprises on heatNetwork.EnterpriseId equals enterprise.EnterpriseId
                             select new
                             {
                                 pipelineSection.PipelineLength,
                                 pipelineSection.Diameter,
                                 heatNetwork.NetworkName,
                                 enterprise.EnterpriseName
                             };
            return View();
        }
    }
}
