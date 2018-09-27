using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Vivo.web.Areas.MP.Controllers
{
    public class Common
    {
        public static List<SelectListItem> GetListOrderID()
        {
            List<SelectListItem> list = new List<SelectListItem>();
            for (int i = 1; i < 10; i++)
            {
                list.Add(new SelectListItem() { Text = i.ToString(), Value = i.ToString() });
            }
            return list;

        }
        public static List<SelectListItem> EnumToSelectListItem(Type enumType)
        {
            List<SelectListItem> list = new List<SelectListItem>();
            foreach (var item in Enum.GetValues(enumType))
            {
                list.Add(new SelectListItem() { Text = Enum.GetName(enumType, (int)item), Value = ((int)item).ToString() });
            }
            return list;
        }


    }

}