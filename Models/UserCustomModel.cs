using System.IO;
using WubiMaster.Common;

namespace WubiMaster.Models
{
    // 虽然本类后缀为 Custom 类型，但实际上不操作 custom 文件的
    // 本类功能是切换方案，因为切换方案需要操作 usr.yaml
    // 操作原理与custom文件类似，因此使用custombasemodel基类
    public partial class UserCustomModel : CustomBaseModel
    {
        private string schema_name;

        public UserCustomModel()
        {
            FilePath = "user.yaml";
        }

        // 扩展方法，用于切换方案
        public void SetSchema(string name)
        {
            if (string.IsNullOrEmpty(name))
                return;

            this.schema_name = name;
        }

        public new void Write()
        {
            if (string.IsNullOrEmpty(schema_name))
                return;

            string content_str = $"var:\n  previously_selected_schema: {schema_name}";
            string file_path = GlobalValues.UserPath + $"\\{FilePath}";
            if (File.Exists(file_path))
                File.Delete(file_path);
            File.WriteAllText(file_path, content_str);
        }
    }
}