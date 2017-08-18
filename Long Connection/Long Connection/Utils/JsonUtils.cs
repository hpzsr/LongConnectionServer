using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

class JsonError:Exception
{
    public string m_errorMsg;

    public JsonError(string errorMsg)
    {
        m_errorMsg = errorMsg;
    }
}

class JsonUtils
{
    public static string getJsonValueOfStr(string jsonData, string key)
    {
        JObject jo = JObject.Parse(jsonData);
        if (jo.GetValue(key) != null)
        {
            return jo.GetValue(key).ToString();
        }
        else
        {
            throw new JsonError("no hava value:" + key);
        }
    }

    public static int getJsonValueOfInt(string jsonData, string key)
    {
        JObject jo = JObject.Parse(jsonData);
        if (jo.GetValue(key) != null)
        {
            return Convert.ToInt32(jo.GetValue(key));
        }
        else
        {
            throw new JsonError("no hava value:" + key);
        }
    }
}
