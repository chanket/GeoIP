using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Licc.GeoIP
{
    public class Gl2GeoIP : IGeoIP
    {
        protected MaxMind.Db.Reader Reader { get; }

        protected string GetCurrentCultured(Dictionary<string, object> names)
        {
            CultureInfo culture = CultureInfo.InstalledUICulture;

            string addr = "";
            if (names.ContainsKey(culture.Name))
            {
                return names[culture.Name].ToString();
            }
            else if (names.ContainsKey(culture.TwoLetterISOLanguageName))
            {
                return names[culture.Name].ToString();
            }
            else if (names.ContainsKey("en"))
            {
                addr += names["en"].ToString();
            }

            return addr;
        }

        protected string GetString(Dictionary<string, object> data)
        {
            string addr = "";
            if (data != null)
            {
                if (data.ContainsKey("country"))
                {
                    var country = data["country"] as Dictionary<string, object>;
                    if (country != null)
                    {
                        if (country.ContainsKey("names"))
                        {
                            var names = country["names"] as Dictionary<string, object>;
                            if (names != null)
                            {
                                addr += GetCurrentCultured(names);
                            }
                        }
                    }
                }

                if (data.ContainsKey("subdivisions"))
                {
                    var subdivisions = data["subdivisions"] as List<object>;
                    for (int i = 0; i < subdivisions.Count(); i++)
                    {
                        var subdivision = subdivisions[i] as Dictionary<string, object>;
                        if (subdivision != null)
                        {
                            if (subdivision.ContainsKey("names"))
                            {
                                var names = subdivision["names"] as Dictionary<string, object>;
                                if (names != null)
                                {
                                    addr += GetCurrentCultured(names);
                                }
                            }
                        }
                    }
                }

                if (data.ContainsKey("city"))
                {
                    var city = data["city"] as Dictionary<string, object>;
                    if (city != null)
                    {
                        if (city.ContainsKey("names"))
                        {
                            var names = city["names"] as Dictionary<string, object>;
                            if (names != null)
                            {
                                addr += GetCurrentCultured(names);
                            }
                        }
                    }
                }
            }

            return addr;
        }

        /// <summary>
        /// The stream to GeoLite2-City.mmdb
        /// </summary>
        /// <param name="stream"></param>
        public Gl2GeoIP(Stream stream)
        {
            Reader = new MaxMind.Db.Reader(stream);
        }

        public string Search(IPAddress ip)
        {
            var data = Reader.Find<Dictionary<string, object>>(ip);
            string addr = GetString(data);

            if (string.IsNullOrEmpty(addr)) return "";
            else return addr;
        }

        public async Task<string> SearchAsync(IPAddress ip)
        {
            return Search(ip);
        }
    }
}
