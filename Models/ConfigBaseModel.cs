using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using WubiMaster.Common;

namespace WubiMaster.Models
{
    public class ConfigBaseModel : ObservableRecipient
    {
        public ConfigBaseModel()
        {
            LoadConfig();
        }

        public void LoadConfig()
        {
            foreach (var p in this.GetType().GetProperties())
            {
                object value;
                if (p.Name == "IsActive") continue;

                var default_value = p.GetValue(this);
                var type = p.PropertyType;
                if (type == typeof(bool))
                    value = ConfigHelper.ReadConfigByBool(p.Name, bool.Parse(default_value?.ToString()));
                else if (type == typeof(int))
                    value = ConfigHelper.ReadConfigByInt(p.Name, int.Parse(default_value?.ToString()));
                else
                    value = ConfigHelper.ReadConfigByString(p.Name, default_value?.ToString());

                p.SetValue(this, Convert.ChangeType(value, p.PropertyType));
            }
        }

        public void SaveConfig()
        {
            Dictionary<string, ConfigModel> dcit = GetPropertys();

            foreach (var k in dcit.Keys)
            {
                var type = dcit[k].ValueType;
                if (type == typeof(bool))
                    ConfigHelper.WriteConfigByBool(k, bool.Parse(dcit[k].ConfigValue.ToString()));
                else if (type == typeof(int))
                    ConfigHelper.WriteConfigByInt(k, int.Parse(dcit[k].ConfigValue.ToString()));
                else
                    ConfigHelper.WriteConfigByString(k, dcit[k].ConfigValue.ToString());
            }
        }

        private Dictionary<string, ConfigModel> GetPropertys()
        {
            Dictionary<string, ConfigModel> tempDict = new Dictionary<string, ConfigModel>();
            foreach (var p in this.GetType().GetProperties())
            {
                if (p.Name == "IsActive") continue;
                var v = this.GetType().GetProperty(p.Name).GetValue(this, null);

                ConfigModel coModel = new ConfigModel();
                coModel.ConfigKey = p.Name;
                coModel.ConfigValue = v;
                coModel.ValueType = p.PropertyType;
                tempDict.Add(p.Name, coModel);
            }
            return tempDict;
        }
    }

    public class ConfigModel
    {
        private string configKey;

        private object valueType;

        private object configValue;

        public string ConfigKey
        {
            get { return configKey; }
            set { configKey = value; }
        }

        public object ValueType
        {
            get { return valueType; }
            set { valueType = value; }
        }

        public object ConfigValue
        {
            get { return configValue; }
            set { configValue = value; }
        }
    }
}