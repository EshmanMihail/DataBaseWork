using Lab2BD5;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace MainProgram
{
    public class Program
    {
        static void Main(string[] args)
        {
            string connectionString = GetConnectionString();
            Console.WriteLine(connectionString);

            using (HeatSchemeStorageContext db = new HeatSchemeStorageContext())
            {
                Console.WriteLine("1.Выборку всех данных из таблицы, стоящей в схеме базы данных нас стороне отношения «один»");
                Console.ReadKey();
                //Task1(db);
                Console.WriteLine("2.Выборку данных из таблицы, стоящей в схеме базы данных нас стороне отношения «один», отфильтрованные по определенному условию, налагающему ограничения на одно или несколько полей");
                Console.ReadKey();
                //Task2(db);
                Console.WriteLine("3.Выборку данных, сгруппированных по любому из полей данных с выводом какого-либо итогового результата (min, max, avg, сount или др.) по выбранному полю из таблицы, стоящей в схеме базы данных нас стороне отношения «многие»");
                //Task3(db);
                Console.WriteLine("4.Выборку данных из двух полей двух таблиц, связанных между собой отношением «один-ко-многим» ");
                Console.ReadKey();
                //Task4(db);
                Console.WriteLine("5.Выборку данных из двух таблиц, связанных между собой отношением «один-ко-многим» и отфильтрованным по некоторому условию, налагающему ограничения на значения одного или нескольких полей ");
                //Task5(db);
                Console.WriteLine("6.Вставку данных в таблицы, стоящей на стороне отношения «Один» ");
                Console.ReadKey();
                //Task6(db);
                Console.WriteLine("7.Вставку данных в таблицы, стоящей на стороне отношения «Многие»");
                Console.ReadKey();
                //Task7(db);
                Console.WriteLine("8.Удаление данных из таблицы, стоящей на стороне отношения «Один» ");
                Console.ReadKey();
                //Task8(db);
                Console.WriteLine("9.Удаление данных из таблицы, стоящей на стороне отношения «Многие»");
                Console.ReadKey();
                //Task9(db);
                Console.WriteLine("10.Обновление удовлетворяющих определенному условию записей в любой из таблиц базы данных ");
                Console.ReadKey();
                Task10(db);
            }
        }
        private static string GetConnectionString()
        {
            string filePath = "C:\\torrentdaun\\Lab2BD5\\Lab2BD5\\Models\\appsettings.json";
            string connectionString = "";

            using (StreamReader reader = new StreamReader(filePath, Encoding.UTF8))
            {
                string jsonString = reader.ReadToEnd();
                JObject jsonObject = JObject.Parse(jsonString);
                connectionString = (string)jsonObject["ConnectionStrings"]["DefaultConnection"];
            }
            return connectionString;
        }

        private static void Task1(HeatSchemeStorageContext db)
        {
            db.Enterprise.Select(x => new {x.EnterpriseId, x.EnterpriseName, x.ManagementOrganization})
                /*.Take(10)*/
                .ToList()
                .ForEach(x=>Console.WriteLine($"{x.EnterpriseId}; {x.EnterpriseName}; {x.ManagementOrganization}"));
        }
        private static void Task2(HeatSchemeStorageContext db)
        {
            db.PipelineSection
                .Where(x => x.Diameter <= 15)
                .Select(x => new { x.SectionId, x.LastRepairDate, x.Diameter })
                .ToList()
                .ForEach(x => Console.WriteLine($"{x.SectionId}; {x.LastRepairDate}; {x.Diameter}"));
        }
        private static void Task3(HeatSchemeStorageContext db)
        {
            var res = db.HeatConsumer
                .OrderByDescending(x => x.CalculatedPower)
                .Select(x => new { x.Network.NetworkName, x.ConsumerId, x.ConsumerName, x.CalculatedPower })
                .Take(100)
                .ToList();
            res.ForEach(Console.WriteLine);
            var averange = db.HeatConsumer
                .OrderByDescending(x => x.CalculatedPower)
                .Select(x => new { x.Network.NetworkName, x.ConsumerId, x.ConsumerName, x.CalculatedPower })
                .ToList();
            Console.WriteLine($"\nСредняя потребляемая мощность в сети: {averange.Average(x => x.CalculatedPower):F2};");
        }
        private static void Task4(HeatSchemeStorageContext db)
        {
            db.HeatNetwork
                .Select(x => new { x.NetworkName, x.Enterprise.EnterpriseName })
                .Take(100)
                .ToList()
                .ForEach(Console.WriteLine);
        }
        private static void Task5(HeatSchemeStorageContext db)
        {
            db.HeatConsumer
                .Where(x => x.CalculatedPower % 2 != 0)
                .Select(x => new {x.ConsumerName, x.Network.NetworkNumber, x.CalculatedPower })
                .Take(100)
                .ToList()
                .ForEach(Console.WriteLine);
        }
        private static void Task6(HeatSchemeStorageContext db)
        {
            var lastEnterprice = db.Enterprise
                .OrderBy(x => x.EnterpriseId)
                .Select(x => new { x.EnterpriseId, x.EnterpriseName })
                .LastOrDefault();
            int lastId = lastEnterprice.EnterpriseId + 1;
            Console.WriteLine($"Последняя компания до добавления:\n{lastEnterprice}");

            Enterprise enterprise = new Enterprise 
            { 
                EnterpriseId = lastId,
                EnterpriseName = "Lab2Enterprice" + lastId.ToString(), 
                ManagementOrganization = "FromLab2Organization2" 
            };
            db.Enterprise.Add(enterprise);
            db.SaveChanges();

            lastEnterprice = db.Enterprise
                .OrderBy(x => x.EnterpriseId)
                .Select(x => new { x.EnterpriseId, x.EnterpriseName })
                .LastOrDefault();
            Console.WriteLine($"Последняя компания после добавления:\n{lastEnterprice}");
        }
        private static void Task7(HeatSchemeStorageContext db)
        {
            var lastHeatNerwork = db.HeatNetwork
                .OrderBy(x => x.NetworkId)
                .Select(x => new { x.NetworkId, x.NetworkName, x.EnterpriseId })
                .LastOrDefault();
            Console.WriteLine($"Последняя тепловая сеть до добавления:\n{lastHeatNerwork}");

            int lastId = lastHeatNerwork.NetworkId + 1;
            List<int> existingIds = db.Enterprise.Select(e => e.EnterpriseId).ToList();
            Random random = new Random();
            int randomId = existingIds[random.Next(existingIds.Count)];

            HeatNetwork heatNetwork = new HeatNetwork
            {
                NetworkId = lastId,
                NetworkName = "NetworkLab2_" + lastId.ToString(),
                NetworkNumber = lastId,
                EnterpriseId = randomId,
                NetworkType = "Type A"
            };
            db.HeatNetwork.Add(heatNetwork);
            db.SaveChanges();

            lastHeatNerwork = db.HeatNetwork
                .OrderBy(x => x.NetworkId)
                .Select(x => new { x.NetworkId, x.NetworkName, x.EnterpriseId })
                .LastOrDefault();
            Console.WriteLine($"Последняя тепловая сеть после добавления:\n{lastHeatNerwork}");
        }
        private static void Task8(HeatSchemeStorageContext db)
        {
            var lastSection = db.PipelineSection
                .OrderBy(x => x.SectionId)
                .LastOrDefault();
            Console.WriteLine($"Последный тепловой трубопровод до удаления:\n{lastSection}");

            db.PipelineSection.Remove(lastSection);
            db.SaveChanges();

            lastSection = db.PipelineSection
                .OrderBy(x => x.SectionId)
                .LastOrDefault();
            Console.WriteLine($"Последный тепловой трубопровод после удаления:\n{lastSection}");
        }
        private static void Task9(HeatSchemeStorageContext db)
        {
            var lastHeatPoint = db.HeatPoint
                .OrderBy(x => x.PointId)
                .LastOrDefault();
            Console.WriteLine($"Последняя тепловая точка до удаления:\n{lastHeatPoint.PointId}");

            //var allDependentPiplinesStartNodeNum = db.PipelineSection
            //    .Where(x => x.StartNodeNumber == lastHeatPoint.PointId)
            //    .ToList();
            //var allDependentPiplinesEndNodeNum = db.PipelineSection
            //   .Where(x => x.EndNodeNumber == lastHeatPoint.PointId)
            //   .ToList();

            var allDependentPipelines = db.PipelineSection
                .Where(x => x.StartNodeNumber == lastHeatPoint.PointId || x.EndNodeNumber == lastHeatPoint.PointId)
                .ToList();

            //db.PipelineSection.RemoveRange(allDependentPiplinesStartNodeNum);
            //db.PipelineSection.RemoveRange(allDependentPiplinesEndNodeNum);
            db.PipelineSection.RemoveRange(allDependentPipelines);
            db.HeatPoint.Remove(lastHeatPoint);
            db.SaveChanges();

            lastHeatPoint = db.HeatPoint
                .OrderBy(x => x.PointId)
                .LastOrDefault();
            Console.WriteLine($"Последняя тепловая точка после удаления:\n{lastHeatPoint.PointId}");
        }
        private static void Task10(HeatSchemeStorageContext db)
        {
            var customersToUpdate = db.HeatConsumer.Where(x => x.CalculatedPower <= 10);
            if (customersToUpdate != null)
            {
                foreach (var customer in customersToUpdate)
                {
                    customer.ConsumerName = customer.ConsumerName + "-Good";
                }
                db.SaveChanges();

                db.HeatConsumer
                .Where(x => x.CalculatedPower <= 10)
                .Select(x => new { x.ConsumerName, x.CalculatedPower })
                .ToList()
                .ForEach(Console.WriteLine);
            }
            else
            {
                Console.WriteLine("Данных с таким условием нет.");
            }
        }
    }
}
