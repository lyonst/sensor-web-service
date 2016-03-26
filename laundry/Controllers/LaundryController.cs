using System.Collections.Generic;
using System.Web.Http;
using System;
using System.IO;
using System.Xml;

namespace laundry.Controllers
{
    public class LaundryController : ApiController
    {
        // GET: api/Laundry
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET: api/Laundry/5
        public Status Get(int id)
        {
            if (id == 0)
            {
                return ReadStatus("washer.xml");
            }
            else
            {
                return ReadStatus("dryer.xml");
            }
        }

        // POST: api/Laundry
        public void Post(int id, [FromBody]string value)
        {
            if (id == 0)
            {
                WriteStatus("washer.xml", value);
            }
            else
            {
                WriteStatus("dryer.xml", value);
            }
        }

        private void WriteStatus(string file, string value)
        {
            var doc = new XmlDocument();

            var root = doc.CreateNode(XmlNodeType.Element, "root", string.Empty);
            var status = doc.CreateNode(XmlNodeType.Element, "status", string.Empty);
            var timeStamp = doc.CreateNode(XmlNodeType.Element, "timestamp", string.Empty);
            doc.AppendChild(root);
            root.AppendChild(status);
            root.AppendChild(timeStamp);

            status.InnerText = value.ToString();
            timeStamp.InnerText = DateTime.Now.ToString("s");

            string fileName = GetPath() + file;
            doc.Save(fileName);
        }

        private Status ReadStatus(string file)
        {
            var doc = new XmlDocument();

            var status = new Status();

            if (!File.Exists(GetPath() + file))
            {
                status.Running = false;
                status.TimeStamp = DateTime.Now;
                return status;
            }

            doc.Load(GetPath() + file);

            var root = doc.ChildNodes[0];
            var element = root.FirstChild;
            status.Running = Convert.ToBoolean(element.InnerText);

            element = root.LastChild;
            status.TimeStamp = Convert.ToDateTime(element.InnerText);

            return status;
        }

        private string GetPath()
        {
            string path = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData) + "\\sensor\\";

            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            return path;
        }

        public class Status
        {
            public bool Running { get; set; }

            public DateTime TimeStamp { get; set; }
        }
    }
}
