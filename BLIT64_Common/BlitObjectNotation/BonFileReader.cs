using System.IO;
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

        public static BonFile Parse(string bon_file_path)
        {
            var lines = File.ReadAllLines(bon_file_path);
            var length = lines.Length;
            var props_file = new BonFile();
            string last_section_key = null;
            var line_index = 0;

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
                        if (props_file.Main == null)
                        {
                            props_file.Main = section;
                        }
                    }
                }
                else
                {
                    if (last_section_key == null)
                    {
                        ++line_index;
                        continue;
                    }

                    ParseKeyValueLine(props_file, last_section_key, line);
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
                var prop_value = line_split[1].Trim();

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

        private static ListValueProp ParsePropValueList(string value)
        {
            var list_prop = new ListValueProp();

            var str_list = GetTextBetweenCharacters(value, ArrayStart, ArrayEnd);

            var str_plit = str_list.Split(',');

            foreach (var s in str_plit)
            {
                var trimmed = s.Trim();

                if (trimmed.Length > 0)
                {
                    var prop_value = new SingleValueProp(s.Trim());

                    list_prop.Items.Add(prop_value);
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
