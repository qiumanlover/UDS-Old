using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;

namespace UDS.Models
{
    interface ISQLModel
    {
        int AddInfo();

        int UpdateInfo(int id);

        List<SelectListItem> GetTypeList();

        Object GetInfoById(int innerid);
    }
}
