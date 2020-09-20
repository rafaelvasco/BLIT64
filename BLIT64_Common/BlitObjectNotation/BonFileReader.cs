using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;

namespace BLIT64_Common
{
    public static class BonFileReader
    {
        private const char SectionStart = '[';
        private const char SectionEnd = ']';
        private const char EqualsSign = '=';
        private const char ArrayStart = '[';
        private const char ArrayEnd = ']';

        public static T Parse<T>(string bon_file_path) where T : new()
        {
            static void ProcessObjectProperties<T1>(object obj, BonFile bon_file)
            {
                var obj_type = obj.GetType();

                var section_name = obj.GetType() == typeof(T1) ? "Root" : obj.GetType().Name;

                foreach (var property_info in obj_type.GetProperties())
                {
                    if ((property_info.PropertyType == typeof(string)) ||
                        (property_info.PropertyType == typeof(int)) || (
                            property_info.PropertyType == typeof(float)))
                    {
                        var matching_property = GetMatchingSingleValueProperty(bon_file, section_name, property_info.Name);

                        if (matching_property != null)
                        {
                            SetSingleValueProperty(obj, property_info, matching_property);
                        }
                    }
                    else if (property_info.PropertyType.IsGenericType && property_info.PropertyType.GetGenericTypeDefinition() == typeof(List<>))
                    {
                        var matching_property = GetMatchingListValueProperty(bon_file, section_name, property_info.Name);

                        if (matching_property != null)
                        {
                            SetListValueProperty(obj, property_info, matching_property);
                        }
                    }
                    else if (property_info.PropertyType.IsClass && section_name == "root")
                    {
                        property_info.SetValue(obj, Activator.CreateInstance(property_info.PropertyType));
                        ProcessObjectProperties<T1>(property_info.GetValue(obj), bon_file);
                    }
                }
            }

            static string NormalizePropertyName(string value)
            {
                return value.Replace("_", "").ToLower();
            }

            static SingleValueProp GetMatchingSingleValueProperty(BonFile file, string section_name, string property_name)
            {
                foreach (var value_props in file.Sections[section_name].ValueProps)
                {
                    var normalized_file_property_name = NormalizePropertyName(value_props.Key);
                    var normalized_param_property_name = NormalizePropertyName(property_name);

                    if (normalized_file_property_name == normalized_param_property_name)
                    {
                        return value_props.Value;
                    }
                }

                return null;
            }

            static ListValueProp GetMatchingListValueProperty(BonFile file, string section_name, string property_name)
            {
                foreach (var value_props in file.Sections[section_name].ListProps)
                {
                    if (NormalizePropertyName(value_props.Key) == NormalizePropertyName(property_name))
                    {
                        return value_props.Value;
                    }
                }

                return null;
            }

            static void SetSingleValueProperty(
                object target_object, 
                PropertyInfo target_object_prop_info,
                SingleValueProp bon_file_prop_value)
            {
                if (target_object_prop_info.PropertyType == typeof(string))
                {
                    target_object_prop_info.SetValue(target_object, bon_file_prop_value.GetValue());    
                }
                else if (target_object_prop_info.PropertyType == typeof(int))
                {
                    target_object_prop_info.SetValue(target_object, bon_file_prop_value.GetIntValue());
                }
                else if (target_object_prop_info.PropertyType == typeof(float))
                {
                    target_object_prop_info.SetValue(target_object, bon_file_prop_value.GetFloatValue());
                }
            }

            static void SetListValueProperty(
                object target_object, 
                PropertyInfo target_object_prop_info,
                ListValueProp bon_file_prop_value
            )
            {
                var target_list_generic_type = target_object_prop_info.PropertyType.GetGenericArguments()[0];

                var constructed_result_list_type = typeof(List<>).MakeGenericType(target_list_generic_type);
                var result_values = (IList) Activator.CreateInstance(constructed_result_list_type);

                foreach (var value in bon_file_prop_value.Items)
                {
                    try
                    {
                        var converted_value = Convert.ChangeType(value, target_list_generic_type);
                        result_values.Add(converted_value);
                    }
                    catch (Exception)
                    {
                        Console.WriteLine($"Could not convert from source string type in bon list property to target list of target type: {target_list_generic_type}");
                        throw;
                    }
                            
                }

                target_object_prop_info.SetValue(target_object, result_values);
            }

            var bon_file = Parse(bon_file_path);

            var result = new T();

            ProcessObjectProperties<T>(result, bon_file);

            return result;
        }

        public static BonFile Parse(string bon_file_path)
        {
            var lines = File.ReadAllLines(bon_file_path);
            var length = lines.Length;
            var props_file = new BonFile();
            var line_index = 0;

            var last_section_key = string.Empty;

            while (line_index < length)
            {
                var line = lines[line_index].Trim();

                if (line.Length == 0)
                {
                    ++line_index;
                    continue;
                }

                if (line[0] == SectionStart)
                {
                    var section_name = GetTextBetweenCharacters(line, SectionStart, SectionEnd);

                    if (section_name.Length > 0)
                    {
                        last_section_key = section_name;
                        var section = new BonFileSection(last_section_key);
                        props_file.Sections.Add(last_section_key, section);
                    }
                }
                else
                {
                    if (!string.IsNullOrEmpty(last_section_key))
                    {
                        ParseKeyValueLine(props_file, last_section_key, line);
                    }
                    else
                    {
                        props_file.Sections.Add("Root", new BonFileSection("Root"));
                        last_section_key = "Root";
                        ParseKeyValueLine(props_file, last_section_key, line);
                    }
                    
                }

                ++line_index;
            }

            return props_file;
        }

        private static void ParseKeyValueLine(BonFile bon_file, string last_section_key, string value)
        {
            var line_split = value.Split( EqualsSign);

            if (line_split.Length == 2)
            {
                var prop_key = line_split[0].Trim();
                var prop_value = NormalizeBonFilePropValueString(line_split[1]);

                switch (prop_value[0])
                {
                    case ArrayStart:
                    {
                        ListValueProp prop = ParsePropValueList(prop_value);
                        bon_file.Sections[last_section_key].ListProps[prop_key] = prop;
                        break;
                    }
                    default:
                    {
                        SingleValueProp prop = new SingleValueProp(prop_value);
                        bon_file.Sections[last_section_key].ValueProps[prop_key] = prop;
                        break;
                    }
                }
            }
        }

        private static string NormalizeBonFilePropValueString(string value)
        {
            return value.Trim().Replace("'", "").Replace("\"", "");
        }

        private static ListValueProp ParsePropValueList(string value)
        {
            var list_prop = new ListValueProp();

            var str_list = GetTextBetweenCharacters(value, ArrayStart, ArrayEnd);

            var str_plit = str_list.Split(',');

            foreach (var s in str_plit)
            {
                var processed_value = NormalizeBonFilePropValueString(s);

                if (processed_value.Length > 0)
                {
                    list_prop.Items.Add(processed_value);
                }
            }

            return list_prop;
        }

        

        private static string GetTextBetweenCharacters(string value, char start_ch, char end_ch)
        {
            var regex = new StringBuilder();

            regex.Append("\\");
            regex.Append(start_ch);
            regex.Append(".*?\\");
            regex.Append(end_ch);

            return Regex.Match(value,  regex.ToString()).Value.Trim(start_ch, end_ch);
        }

    }
}
