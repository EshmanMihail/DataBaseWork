using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InfoStruct.Sessions
{
    public class SessionSearchStuff
    {
        public string columnName;
        public string textForSearch;
        public bool isSaved = false;

		public void Save(string column, string text, HttpContext context, string saveKey)
		{
			columnName = column;
			textForSearch = text;
			isSaved = true;

			context.Session.Set(saveKey, this);
		}
	}
}
