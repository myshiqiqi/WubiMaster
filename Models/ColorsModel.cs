using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YamlDotNet.Serialization;

namespace WubiMaster.Models
{
    public class ColorsModel
    {
        public Description description { get; set; }
        public ColorStyle style { get; set; }
        public Dictionary<string, ColorScheme> preset_color_schemes { get; set; }

        public ColorsModel()
        {
            description = new Description();
            style = new ColorStyle();
            preset_color_schemes = new Dictionary<string, ColorScheme>();
        }
    }

    public class Description
    {
        public string color_name { get; set; }
        public string display_name { get; set; }
        public string create_time { get; set; }
        public string update_time { get; set; }
        public string is_template { get; set; }
    }
}
