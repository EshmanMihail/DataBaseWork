using InfoStruct.Service;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using ModelsLibrary.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InfoStruct.MiddlewaresFolder
{
    public static partial class Middlewares
    {
        public static class Tables
        {
            public static void ShowEnterprice(IApplicationBuilder app)
            {
                app.Run(async context =>
                {
                    var enterpriceCache = context.RequestServices.GetService<EnterpriceCacheData>()?.Get("Enterprice20");

                    string answer = "" +
                    "<HTML>" +
                    "<HEAD> <Title>Компания</title> </head>" +
                    "<META http-equiv='Content-Type' content='text/html; charset=utf-8 />'" +
                    "<body> " +
                        "<h1>Список компаний</h1>" +
                        "<table border = 1>" +
                            "<tr> " +
                                "<th>Код</th>" +
                                "<th>Имя компании</th>" +
                                "<th>Организация</th>" +
                            "</tr>";

                    foreach (Enterprise enterprise in enterpriceCache)
                    {
                        answer += "<tr>";
                        answer += "<td>" + enterprise.ID + "</td>";
                        answer += "<td>" + enterprise.EnterpriseName + "</td>";
                        answer += "<td>" + enterprise.ManagementOrganization + "</td>";
                        answer += "</tr>";
                    }
                    answer += "</table> </body> </html>";

                    await context.Response.WriteAsync(answer);
                });
            }

            public static void ShowConsumer(IApplicationBuilder app)
            {
                app.Run(async context =>
                {
                    var consumerCache = context.RequestServices.GetService<HeatConsumerCacheData>()?.Get("Consumer20");

                    string answer = "" +
                    "<HTML>" +
                    "<HEAD> <Title>Клиенты</title> </head>" +
                    "<META http-equiv='Content-Type' content='text/html; charset=utf-8 />'" +
                    "<body> " +
                        "<h1>Список клиентов</h1>" +
                        "<table border = 1>" +
                            "<tr> " +
                                "<th>Код</th>" +
                                "<th>Имя</th>" +
                                "<th>Номер сети</th>" +
                                "<th>Потраченная мощность</th>" +
                            "</tr>";

                    foreach (HeatConsumer con in consumerCache)
                    {
                        answer += "<tr>";
                        answer += "<td>" + con.ID + "</td>";
                        answer += "<td>" + con.ConsumerName + "</td>";
                        answer += "<td>" + con.NetworkId + "</td>";
                        answer += "<td>" + con.CalculatedPower + "</td>";
                        answer += "</tr>";
                    }
                    answer += "</table> </body> </html>";

                    await context.Response.WriteAsync(answer);
                });
            }

            public static void ShowNetwork(IApplicationBuilder app)
            {
                app.Run(async context =>
                {
                    var cacheData = context.RequestServices.GetService<HeatNetworkCacheData>()?.Get("Consumer20");

                    string answer = "" +
                    "<HTML>" +
                    "<HEAD> <Title>Сеть</title> </head>" +
                    "<META http-equiv='Content-Type' content='text/html; charset=utf-8 />'" +
                    "<body> " +
                        "<h1>Список сетей</h1>" +
                        "<table border = 1>" +
                            "<tr> " +
                                "<th>Код</th>" +
                                "<th>Имя</th>" +
                                "<th>Номер компании</th>" +
                                "<th>Тип сети</th>" +
                            "</tr>";

                    foreach (HeatNetwork el in cacheData)
                    {
                        answer += "<tr>";
                        answer += "<td>" + el.ID + "</td>";
                        answer += "<td>" + el.NetworkName + "</td>";
                        answer += "<td>" + el.EnterpriseId + "</td>";
                        answer += "<td>" + el.NetworkType + "</td>";
                        answer += "</tr>";
                    }
                    answer += "</table> </body> </html>";

                    await context.Response.WriteAsync(answer);
                });
            }

            public static void ShowPoint(IApplicationBuilder app)
            {
                app.Run(async context =>
                {
                    var cacheData = context.RequestServices.GetService<HeatPointCacheData>()?.Get("Consumer20");

                    string answer = "" +
                    "<HTML>" +
                    "<HEAD> <Title>Тепловые точки</title> </head>" +
                    "<META http-equiv='Content-Type' content='text/html; charset=utf-8 />'" +
                    "<body> " +
                        "<h1>Список тепловых точек</h1>" +
                        "<table border = 1>" +
                            "<tr> " +
                                "<th>Код</th>" +
                                "<th>Имя</th>" +
                                "<th>Номер сети</th>" +
                            "</tr>";

                    foreach (HeatPoint el in cacheData)
                    {
                        answer += "<tr>";
                        answer += "<td>" + el.ID + "</td>";
                        answer += "<td>" + el.PointName + "</td>";
                        answer += "<td>" + el.NetworkId + "</td>";
                        answer += "</tr>";
                    }
                    answer += "</table> </body> </html>";

                    await context.Response.WriteAsync(answer);
                });
            }

            public static void ShowWell(IApplicationBuilder app)
            {
                app.Run(async context =>
                {
                    var cacheData = context.RequestServices.GetService<HeatWellCacheData>()?.Get("Consumer20");

                    string answer = "" +
                    "<HTML>" +
                    "<HEAD> <Title>Тепловые колодцы</title> </head>" +
                    "<META http-equiv='Content-Type' content='text/html; charset=utf-8 />'" +
                    "<body> " +
                        "<h1>Список тепловых колодцев</h1>" +
                        "<table border = 1>" +
                            "<tr> " +
                                "<th>Код</th>" +
                                "<th>Имя</th>" +
                                "<th>Номер сети</th>" +
                            "</tr>";

                    foreach (HeatWell el in cacheData)
                    {
                        answer += "<tr>";
                        answer += "<td>" + el.ID + "</td>";
                        answer += "<td>" + el.WellName + "</td>";
                        answer += "<td>" + el.NetworkId + "</td>";
                        answer += "</tr>";
                    }
                    answer += "</table> </body> </html>";

                    await context.Response.WriteAsync(answer);
                });
            }

            public static void ShowPipeline(IApplicationBuilder app)
            {
                app.Run(async context =>
                {
                    var cacheData = context.RequestServices.GetService<PipelineSectionCacheData>()?.Get("Consumer20");

                    string answer = "" +
                    "<HTML>" +
                    "<HEAD> <Title>Водопроводы</title> </head>" +
                    "<META http-equiv='Content-Type' content='text/html; charset=utf-8 />'" +
                    "<body> " +
                        "<h1>Список труп</h1>" +
                        "<table border = 1>" +
                            "<tr> " +
                                "<th>Код</th>" +
                                "<th>Имя</th>" +
                                "<th>Длина трубопровода</th>" +
                            "</tr>";

                    foreach (PipelineSection el in cacheData)
                    {
                        answer += "<tr>";
                        answer += "<td>" + el.ID + "</td>";
                        answer += "<td>" + el.SectionNumber + "</td>";
                        answer += "<td>" + el.PipelineLength + "</td>";
                        answer += "</tr>";
                    }
                    answer += "</table> </body> </html>";

                    await context.Response.WriteAsync(answer);
                });
            }
        }
    }
}
