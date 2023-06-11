using Godot;
using System;
using System.IO;
using System.Text;
using System.Collections.Generic;
using System.Reflection;
using Newtonsoft.Json;
using System.Text.RegularExpressions;
using System.Globalization;

namespace Honeycodes.Dialogue
{
    public class Timeline 
    {
        private const string TIMELINE_DIRECTORY = "timelines/";
        public static Dictionary<int, TimelineEvent> LoadTimeline(string path) 
        {
            if (path.EndsWith(".json"))
            {
                return LoadFromJson(path);
            }
            else if (path.EndsWith(".tsv"))
            {
                return LoadFromTSV(path);
            }
            else
            {
                throw new ArgumentException("Unsupported file format.");
            }
        }
        
        public static Dictionary<int, TimelineEvent> LoadFromJson(string path) 
        {
            Dictionary<int, TimelineEvent> eventDict =  JsonConvert.DeserializeObject<Dictionary<int, TimelineEvent>>(File.ReadAllText(TIMELINE_DIRECTORY + path));
            return eventDict;
        }

        public static Dictionary<int, TimelineEvent> LoadFromTSV(string path) 
        {
            Dictionary<int, TimelineEvent> eventDict =  new Dictionary<int, TimelineEvent>();
            string tsv = string.Empty;

            using (StreamReader reader = new StreamReader(TIMELINE_DIRECTORY + path))
            {
                tsv = reader.ReadToEnd();
            }

            string[] lines = tsv.Split(new string[] { "\n" }, System.StringSplitOptions.None);

            if (lines.Length > 1)
            {
                string[] headers = lines[0].Split(new string[] { "\t" }, System.StringSplitOptions.TrimEntries);

                for (int i = 1; i < lines.Length; i++)
                {
                    if (string.IsNullOrWhiteSpace(lines[i])) continue;
                    if (string.IsNullOrEmpty(lines[i])) continue;
                    string[] columns = lines[i].Split(new string[] { "\t" }, StringSplitOptions.None);
                    int n;
                    bool success = int.TryParse(columns[0], out n);
                    eventDict.Add(n, new TimelineEvent());

                    for (int j = 1; j < columns.Length; j++)
                    {
                        var header = headers[j];

                        switch (header)
                        {
                            case "Next":
                            case "ParentId":
                            case "ZIndex":
                            int k;
                            success = int.TryParse(columns[j], out k);
                            eventDict[n].Set<int>(header,k);
                            break;

                            case "Choices":
                            if (columns[j] == "") break;
                            List<int> choiceList = ExtractListFromString(columns[j]);
                            eventDict[n].Set<List<int>>(header, choiceList);
                            break;

                            case "SpriteMirrored":
                            bool b;
                            success = bool.TryParse(columns[j], out b);
                            eventDict[n].Set<bool>(header, b);
                            break;

                            case "Scale":
                            case "WaitTime":
                            float f;
                            success = float.TryParse(columns[j], NumberStyles.Float, CultureInfo.CreateSpecificCulture("en-US"), out f);
                
                            eventDict[n].Set<float>(header, f);
                            break;

                            default:
                            if (columns[j] == "") break;
                            eventDict[n].Set<string>(header,columns[j].Trim());
                            break;
                        }
                    }
                }
            }
            return eventDict;
        }

    static List<int> ExtractListFromString(string input)
    {
        List<int> numbers = new List<int>();
        Regex regex = new Regex(@"\d+");
        MatchCollection matches = regex.Matches(input);

        foreach (Match match in matches)
        {
            int number;
            if (int.TryParse(match.Value, out number))
            {
                numbers.Add(number);
            }
        }
        return numbers;
    }
        

        public class TimelineEvent
        {
            public int Next { get; set; } = 0;
            public int ParentId { get; set; } = 0;
            public string Line { get; set; } = null;
            public List<int> Choices { get; set; } = null;
            public string ChoiceLabel { get; set; } = null;

            public string Character { get; set; } = null;
            public string Condition { get; set; } = null;
            public string Animation { get; set; } = null;
            public string Expression { get; set; } = null;
            public string Position { get; set; } = null;
            public string AnimationLength { get; set; } = null;
            public bool SpriteMirrored { get; set; } = false;
            public int ZIndex { get; set; } = 0;
            public float Scale { get; set; } = 1f;

            public string Action { get; set; } = null;
            public string ChangeBackground { get; set; } = null;
            public string ChangeMusic { get; set; } = null;
            public string AudioEvent { get; set; } = null;
            public string ThemeChange { get; set; } = null;
            public float WaitTime { get; set; }  = 0.0f;
            public string Transition { get; set; } = null;
            public string ChangeTimeline { get; set; } = null;


            public bool Has(string propertyName)
            {
                var property = GetType().GetProperty(propertyName);
                if (property != null)
                {
                    var value = property.GetValue(this);
                    if (property.Name == "Scale") {
                        if((float) value == 1) return false;
                    }
                    return value != null;
                }
                return false;
            }

            public T Get<T>(string propertyName)
            {
                var property = GetType().GetProperty(propertyName);
                if (property != null)
                {
                    var value = property.GetValue(this);
                    if (value is T typedValue)
                    {
                        return typedValue;
                    }
                }
                return default(T);
            }


            public void Set<T>(string propertyName, T value)
            {
                var property = GetType().GetProperty(propertyName);
                property.SetValue(this, value);
            }
            

            public override string ToString() 
            {
                var props = this.GetType().GetProperties();
                var sb = new StringBuilder();
                sb.AppendLine("{");
                foreach (var p in props)
                {
                    if (p.GetValue(this) != null) 
                    {
                        if (p.Name == "ZIndex" && (int) p.GetValue(this) == 0)
                        {
                            continue;
                        }
                        if (p.Name == "WaitTime" && (float) p.GetValue(this) == 0.0f)
                        {
                            continue;
                        }
                        if (p.Name == "SpriteMirrored" && (bool) p.GetValue(this) == false)
                        {
                            continue;
                        }
                        sb.AppendLine(p.Name + ": " + p.GetValue(this));
                    }
                }
                sb.AppendLine("}");
                return sb.ToString();
            }
        }
    }
}