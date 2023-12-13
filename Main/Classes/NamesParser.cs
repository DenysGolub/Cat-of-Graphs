using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Xceed.Wpf.Toolkit.PropertyGrid;

namespace Main.Classes
{
    static class NamesParser
    {
        static public int SingleNodeName(this string node)
        {
            Regex regex = new Regex(@"_(\d*)");
            Match m = regex.Match(node);

            return Convert.ToInt32(m.Groups[1].Value);
        }
        static public void DoubleNodeName(this string node, out int first_node, out int second_node)
        {
            Regex regex = new Regex(@"(\d*)_(\d*)");
            Match m = regex.Match(node);

            first_node = int.Parse(m.Groups[1].Value);
            second_node = int.Parse(m.Groups[2].Value);
        }
        static public void DoubleEdgeName(this string node, out string first_node, out string second_node)
        {
            Regex regex = new Regex(@"(.*)_dek_(.*)");
            Match m = regex.Match(node);

            first_node = m.Groups[1].Value;
            second_node = m.Groups[2].Value;
        }
        static public void EdgesNames(this string edge, out int first_node, out int second_node)
        {
            Regex regex = new Regex(@"_(\d*)_(\d*)");

            Match m = regex.Match(edge);

            first_node = Convert.ToInt32(m.Groups[1].Value);
            second_node = Convert.ToInt32(m.Groups[2].Value);
        }
    }
}
