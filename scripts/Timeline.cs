using System;
using System.IO;
using System.Text;
using System.Collections.Generic;
using System.Reflection;
using Newtonsoft.Json;

namespace Honeycodes.Dialogue
{
    public class Timeline 
    {
        public static Dictionary<int, TimelineEvent> LoadFromJson(string Path) 
        {
            Dictionary<int, TimelineEvent> EventDict =  JsonConvert.DeserializeObject<Dictionary<int, TimelineEvent>>(File.ReadAllText(Path));
            return EventDict;
        }

        public class TimelineEvent
        {
            public int Next { get; set; } = 1;
            public string ParentId { get; set; } = null;
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
            public double Scale { get; set; } = 1;

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
                        if((double) value == 1) return false;
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