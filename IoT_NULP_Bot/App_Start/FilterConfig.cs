﻿using System.Web;
using System.Web.Mvc;

namespace IoT_NULP_Bot
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }
    }
}
