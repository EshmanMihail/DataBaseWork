using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.DependencyInjection;
using ModelsLibrary.Models;
using System;
using System.Collections.Generic;
using System.Linq.Dynamic.Core;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Linq;
using InfoStruct.Sessions;

namespace InfoStruct.MiddlewaresFolder
{
    public static partial class Middlewares
    {
        public static class Search
        {
            public static void ShowForm1(IApplicationBuilder app)
            {
                app.Run(async context => {
                    bool isChosenTableNotNull = GetOrAddCookie("choosingList", context, out string? chosenTable);
                    bool isChosenColumnNotNull = GetOrAddCookie("column", context, out string? chosenColumn);
                    bool isTextForSearchNotNull = GetOrAddCookie("textForSearch", context, out string? textForSearch);

                    string answer = GetDefaultForm(chosenTable, chosenColumn, textForSearch, 1);

                    if (isChosenTableNotNull && isChosenColumnNotNull && isTextForSearchNotNull)
                    {

                        answer += $"Результат поиска в таблице {chosenTable} по столбцу {chosenColumn}:";
                        answer += Find(chosenTable, chosenColumn, textForSearch, context);

                        // Без этого поиск больше нормально работать не будет.
                        context.Response.Cookies.Delete("choosingList");
                        context.Response.Cookies.Delete("column");
                        context.Response.Cookies.Delete("textForSearch");
                    }

                    await context.Response.WriteAsync(answer);
                });
            }

            public static void ShowForm2(IApplicationBuilder app)
            {
                app.Run(async context =>
                {
                    // из-за сессии теперь эта форма не позволяет делать больше одного поиска.
                    // Оставил потому что иначе пропадёт фишка использования сессий.

                    var searchSession = context.Session.Get<SessionSearchStuff>("searchSession") ?? new SessionSearchStuff();

                    string chosenTable;
                    string chosenColumn;
                    string textForSearch;
                    if (searchSession.isSaved)
                    {

                        chosenTable = searchSession.tableName;
                        chosenColumn = searchSession.columnName;
                        textForSearch = searchSession.textForSearch;
                    }
                    else
                    {
                        chosenTable = context.Request.Query["choosingList"];
                        chosenColumn = context.Request.Query["column"];
                        textForSearch = context.Request.Query["textForSearch"];
                    }

                    string answer = GetDefaultForm(chosenTable, chosenColumn, textForSearch, 2);

                    if (chosenTable is not null)
                    {
                        searchSession.tableName = chosenTable;
                        searchSession.columnName = chosenColumn;
                        searchSession.textForSearch = textForSearch;
                        searchSession.isSaved = true;

                        context.Session.Set("searchSession", searchSession);

                        answer += $"Результат поиска в таблице {chosenTable} по столбцу {chosenColumn}:";
                        answer += Find(chosenTable, chosenColumn, textForSearch, context);
                    }

                    await context.Response.WriteAsync(answer);
                });
            }

            /// <summary>
            /// Получает куки по ключу. Если этой куки ещё не существует, но в запросе фигурирует, то будет добавлена кука.
            /// </summary>
            /// <param name="key"></param>
            /// <param name="context"></param>
            /// <param name="res"></param>
            /// <returns>true, если получилось получить элемент из кук или из запроса.</returns>
            private static bool GetOrAddCookie(string key, HttpContext context, out string? res)
            {
                if (context.Request.Cookies.ContainsKey(key))
                {

                    res = context.Request.Cookies[key];
                    return true;
                }

                if (context.Request.Query.ContainsKey(key))
                {

                    res = context.Request.Query[key];
                    context.Response.Cookies.Append(key, res);
                    return true;
                }

                res = null;
                return false;
            }

            private static string GetDefaultForm(string? table, string? column, string? searchText, int formNumber)
            {
                string _defaultSearchForm =
                    "<html> <head>Поиск по таблицам</head>" +
                    "<META http-equiv='Content-Type' content='text/html; charset=utf-8' /><body> " +
                    $"<form action = /searchform{formNumber}> " +

                    "<label for='choosingList'>Выберите таблицу для поиска:<br></label>" +
                    "<select name='choosingList' id='choosingList' required >" +
                    $"<option value='Enterprise'>Компания</option>" +
                    $"<option value='HeatConsumer'>Пользователи</option>" +
                    $"<option value='HeatNetwork'>Тепловая сеть</option>" +
                    $"<option value='HeatPoint'>Тепловая точка</option>" +
                    $"<option value='HeatWell'>Тепловой котёл</option>" +
                    $"<option value='PipelineSection'>Друбопровод</option>" +
                    "</select><br> " +

                    "<label for='column'>Введите название столбца для поиска по таблице:<br></label>" +
                    $"<input type='text' name='column' id='column' value='{column}' required /> <br>" +

                    "<label for='textForSearch'>Введите фрагмент для поиска: <br></label>" +
                    $"<input type='text' name='textForSearch' id='textForSearch' value='{searchText}' required /> <br>" +

                    "<input type='submit' value='найти' >" +
                    "</form> </body> </html>" +
                    "<script>" +
                    "window.onload = function() {" +
                    $"document.getElementById(\"choosingList\").value = \"{table}\"" +
                    "}" +
                    "</script>";

                return _defaultSearchForm;
            }

            private static string Find(string chosenTable, string chosenColumn, string textForSearch, HttpContext context)
            {
                string answer = "";
                List<string> columnNames = GetColumnNames(chosenTable, context);

                if (columnNames.Contains(chosenColumn))
                {

                    var table = GetTable(chosenTable, context);

                    foreach (var item in table.Select(chosenColumn))
                    {

                        if (item.ToString().Contains(textForSearch))
                        {

                            answer += $"<li>{item}</li>";
                        }
                    }

                    return answer;
                }

                return ShowColumnNotExist(columnNames);
            }

            private static List<string> GetColumnNames(string tableName, HttpContext context)
            {
                var fullTableName = $"ModelsLibrary.Models.{tableName}, ModelsLibrary";
                var dbContext = context.RequestServices.GetService<HeatSchemeStorageContext>();

                var modelType = Type.GetType(fullTableName);
                var entityType = dbContext.Model.FindEntityType(modelType);

                return entityType.GetProperties().Select(x => x.Name).ToList();
            }

            private static IQueryable GetTable(string tableName, HttpContext context)
            {
                var fullTableName = $"ModelsLibrary.Models.{tableName}, ModelsLibrary";
                var dbContext = context.RequestServices.GetService<HeatSchemeStorageContext>();

                var modelType = Type.GetType(fullTableName);
                return (IQueryable)dbContext.GetType().GetMethod("Set", Type.EmptyTypes).MakeGenericMethod(modelType).Invoke(dbContext, null);
            }

            private static string ShowColumnNotExist(List<string> columnNames)
            {
                string answer = "<p style='color:red;'>Такого столбца нет в выбранной таблице.</p>";
                answer += "Вы можете выбрать из:";

                foreach (var a in columnNames)
                {

                    answer += $"<li>{a}</li>";
                }

                return answer;
            }
        }
    }
}
