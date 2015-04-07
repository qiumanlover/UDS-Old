using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace UDS.Models
{
    public static class GlobeLoad
    {
        public static Dictionary<string, string> loadInfoDic = new Dictionary<string, string>();
        public static void Load(string uid, string guid)
        {
            if (loadInfoDic.ContainsKey(uid))
            {
                loadInfoDic[uid] = guid;
            }
            else
            {
                loadInfoDic.Add(uid, guid);
            }
        }

        public static bool CheckLoad(string uid, string guid)
        {
            if (loadInfoDic[uid].Equals(guid))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

    }
}