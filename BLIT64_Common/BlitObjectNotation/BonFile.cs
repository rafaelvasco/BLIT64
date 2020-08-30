using System;
using System.Collections.Generic;
using System.Globalization;

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
    }

    public class ListValueProp
    {
        private readonly List<SingleValueProp> _list;

        public List<SingleValueProp> Items => _list;

        public ListValueProp()
        {
            _list = new List<SingleValueProp>();
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
    }

    public class BonFile
    {
        public Dictionary<string, BonFileSection> Sections;

        public BonFileSection Main { get; internal set; }

        public BonFile()
        {
            Sections = new Dictionary<string, BonFileSection>();
        }
    }
}
