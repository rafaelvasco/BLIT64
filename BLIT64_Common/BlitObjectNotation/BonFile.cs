using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace BLIT64_Common
{
    public class SingleValueProp
    {
        private readonly string _value;

        public SingleValueProp(string value)
        {
            _value = value;
        }

        public string GetValue()
        {
            return _value;
        }

        public int GetIntValue()
        {
            if (int.TryParse(_value, out var result))
            {
                return result;
            }

            throw new InvalidOperationException("This property value can't be cast to Int");
        }

        public float GetFloatValue()
        {
            if (float.TryParse(_value, NumberStyles.Float, CultureInfo.InvariantCulture, out var result))
            {
                return result;
            }

            throw new InvalidOperationException("This property value can't be cast to Float");
        }

        public override string ToString()
        {
            return _value;
        }
    }

    public class ListValueProp
    {
        private readonly List<string> _list;

        public List<string> Items => _list;

        public ListValueProp()
        {
            _list = new List<string>();
        }
    }

    public class BonFileSection
    {
        public string SectionName;
        public Dictionary<string, SingleValueProp> ValueProps;
        public Dictionary<string, ListValueProp> ListProps;

        public BonFileSection(string name)
        {
            SectionName = name;
            ValueProps = new Dictionary<string, SingleValueProp>();
            ListProps = new Dictionary<string, ListValueProp>();
        }

        public override string ToString()
        {
            var string_builder = new StringBuilder();

            string_builder.AppendLine($"[{SectionName}]");

            foreach (var single_value_prop in ValueProps)
            {
                string_builder.AppendLine($"{single_value_prop.Key} = {single_value_prop.Value}");
            }

            foreach (var list_value_prop in ListProps)
            {
                string_builder.Append($"{list_value_prop.Key} = [");

                foreach (var prop_value in list_value_prop.Value.Items)
                {
                    string_builder.Append(prop_value);
                    string_builder.Append(",");
                }

                string_builder.Append("]");
            }


            return string_builder.ToString();
        }
    }

    public class BonFile
    {
        public Dictionary<string, BonFileSection> Sections;

        public BonFileSection Main
        {
            get
            {
                if (Sections.TryGetValue("Root", out var section))
                {
                    return section;
                }

                return null;
            }
        }

        public BonFile()
        {
            Sections = new Dictionary<string, BonFileSection>();
        }

        public override string ToString()
        {
            var string_builder = new StringBuilder();

            foreach (var bon_file_section in Sections)
            {
                string_builder.AppendLine(bon_file_section.ToString());
            }

            return string_builder.ToString();
        }
    }
}
