using InfoStruct.Services;
using Lab2BD5;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InfoStruct.Midlewares
{
    public static partial class Middlewares
    {
        public class Tables
        {
            public static void ShowEnterprices(IApplicationBuilder app)
            {
                app.Run(async context =>
                {
                    var enterpriceCache = context.RequestServices.GetService<EnterpriceCacheData>().Get("Enterprice20");

                    string answer = "" +
                    "<HTML>" +
                    "<HEAD> <Title>Компании</title> </head>" +
                    "<META http-equiv='Content-Type' content='text/html; charset=utf-8 />'" +
                    "<body> " +
                        "<h1>Список компаний</h1>" +
                        "<table border = 1>" +
                            "<tr> " +
                                "<th>Код</th>" +
                                "<th>Имя компании</th>" +
                                "<th>Организация</th>" +
                            "</tr>";

                    foreach (Enterprise ent in enterpriceCache)
                    {
                        answer += "<tr>";
                        answer += "<td>" + ent.ID + "</td>";
                        answer += "<td>" + ent.EnterpriseName + "</td>";
                        answer += "<td>" + ent.ManagementOrganization + "</td>";
                        answer += "</tr>";
                    }

                    answer += "</table> </body> </html>";
                    await context.Response.WriteAsync(answer);
                });
            }
        }
    }
}
