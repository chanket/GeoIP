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
        protected bool LeaveOpen { get; }

        protected Stream BaseStream { get; }

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
        public Gl2GeoIP(Stream stream, bool leaveOpen = false)
        {
            Reader = new MaxMind.Db.Reader(stream);
            BaseStream = stream;
            LeaveOpen = leaveOpen;
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

        #region IDisposable Support
        private bool disposedValue = false; // 要检测冗余调用

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: 释放托管状态(托管对象)。
                    Reader.Dispose();
                    if (!LeaveOpen)
                    {
                        BaseStream.Dispose();
                    }
                }

                // TODO: 释放未托管的资源(未托管的对象)并在以下内容中替代终结器。
                // TODO: 将大型字段设置为 null。

                disposedValue = true;
            }
        }

        // TODO: 仅当以上 Dispose(bool disposing) 拥有用于释放未托管资源的代码时才替代终结器。
        // ~Gl2GeoIP() {
        //   // 请勿更改此代码。将清理代码放入以上 Dispose(bool disposing) 中。
        //   Dispose(false);
        // }

        // 添加此代码以正确实现可处置模式。
        public void Dispose()
        {
            // 请勿更改此代码。将清理代码放入以上 Dispose(bool disposing) 中。
            Dispose(true);
            // TODO: 如果在以上内容中替代了终结器，则取消注释以下行。
            // GC.SuppressFinalize(this);
        }
        #endregion
    }
}
