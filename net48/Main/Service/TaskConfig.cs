using Newtonsoft.Json.Linq;

using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Xml.Serialization;

namespace A.UI.Service
{
    [XmlRoot("root")]
    public class TaskConfig
    {
        /// <summary>
        /// 加载任务配置。
        /// </summary>
        /// <param name="reader"></param>
        /// <returns></returns>
        public static TaskConfig Load(TextReader reader)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(TaskConfig));
            TaskConfig config = (TaskConfig)serializer.Deserialize(reader);

            return config;
        }

        [XmlElement("shopRow")]
        public ShopRow ShopRow { get; set; }

        [XmlElement("schedulerProcessName")]
        public string SchedulerProcessName { get; set; }

        [XmlArray("tasks")]
        [XmlArrayItem("task")]
        public TaskItem[] Tasks { get; set; }

        internal Dictionary<string, string> GetShopArguments() => ShopRow.GetArguments();
    }

    public sealed class ShopRow
    {
        [XmlAttribute("memoryMappedFileName")]
        public string MemoryMappedFileName { get; set; }

        [XmlAttribute("serverUrl")]
        public string ServerUrl { get; set; }

        [XmlAttribute("installType")]
        public string InstallType { get; set; }

        [XmlAttribute("storeNo")]
        public string StoreNo { get; set; }

        [XmlAttribute("regNo")]
        public string RegNo { get; set; }

        private Dictionary<string, string> _shopArguments;

        internal Dictionary<string, string> GetArguments()
        {
            if (_shopArguments == null)
            {
                _shopArguments = new Dictionary<string, string>();

                foreach (PropertyInfo p in GetType().GetProperties())
                {
                    if (Attribute.IsDefined(p, typeof(XmlIgnoreAttribute)))
                    {
                        continue;
                    }

                    _shopArguments.Add(p.Name, (string)p.GetValue(this));
                }
            }

            return _shopArguments;
        }
    }

    public sealed class TaskItem
    {
        [XmlAttribute("name")]
        public string Name { get; set; }

        [XmlAttribute("localPath")]
        public string LocalPath { get; set; }

        [XmlAttribute("args")]
        public string Arguments { get; set; }

        [XmlAttribute("number")]
        public int Number { get; set; }

        [XmlAttribute("shouldWait")]
        public bool ShouldWait { get; set; }

        [XmlAttribute("runNextOnFailed")]
        public bool RunNextOnFailed { get; set; }

        [XmlAttribute("specifiedTime")]
        public string SpecifiedTime { get; set; }

        [XmlAttribute("intervalTime")]
        public string IntervalTime { get; set; }

        public string GetTaskArgument(TaskConfig taskConfig)
        {
            JObject o = new JObject();

            CopyProperties(taskConfig.GetShopArguments(), o);
            CopyProperties(GetArguments(), o);

            return o.ToString();
        }

        private static void CopyProperties(Dictionary<string, string> source, JObject destination)
        {
            foreach (string key in source.Keys)
            {
                destination.Add(key, source[key]);
            }
        }

        private Dictionary<string, string> GetArguments()
        {
            string[] argProps = new string[]
                    {
                        nameof(SpecifiedTime),
                        nameof(IntervalTime),
                    };

            Dictionary<string, string> args = new Dictionary<string, string>();

            foreach (PropertyInfo p in GetType().GetProperties())
            {
                if (Array.IndexOf(argProps, p.Name) == -1)
                {
                    continue;
                }

                args.Add(p.Name, (string)p.GetValue(this));
            }

            return args;
        }

        public override string ToString() => Name;
    }
}

