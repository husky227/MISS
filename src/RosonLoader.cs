using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace CityDriver
{
    class RosonLoader
    {
        public static void LoadRoson(string path)
        {
            var json = System.IO.File.ReadAllText(path);

            Dictionary<String, JArray> objects = JsonConvert.DeserializeObject<Dictionary<String, JArray>>(json);
            //Console.WriteLine(objects["walls"]);
            foreach (String jObject in objects.Keys)
            {
                JArray objectsArray = objects[jObject];
                foreach (JObject obj in objectsArray)
                {
                    /*
                    var type = (string)obj.GetValue("type");
                    if (type != null)
                    {
                        switch (type)
                        {
                            case "wall":
                                var id = (string)obj.GetValue("id");
                                var width = (string)obj.GetValue("width");
                                var height = (string)obj.GetValue("height");
                                var color = (string)obj.GetValue("color");
                                JObject from = (string)obj.GetValue("from");
                                JObject to = (string)obj.GetValue("to");



                                Wall wall = new Wall(id, width, height, color, x1, y1, x2, y2);
                                break;
                            default:
                                break;
                        }
                    }*/
                    foreach (KeyValuePair<String, JToken> app in obj)
                    {
                        var appName = obj;
                        var description = (String)app.Key;
                        var value = "";
                        if (description != "from" && description != "to" && description != "position" && description != "location")
                        {
                            value = (String)app.Value;
                        }
                        else if (description == "location")
                        {
                            value = "x: " + (String)app.Value["x"] + " y: " + (String)app.Value["y"] + " z: " + (String)app.Value["z"] + " alpha: " + (String)app.Value["alpha"];
                        }
                        else
                        {
                            value = "x: " + (String)app.Value["x"] + " y: " + (String)app.Value["y"];
                        }

                        Console.WriteLine(appName);
                        Console.WriteLine(description);
                        Console.WriteLine(value);
                        Console.WriteLine("\n");
                    }
                }
            }
        }
    }
}
