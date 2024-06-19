﻿using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WubiMaster.Common;

namespace WubiMaster.Models
{
    public partial class AttributeModel : ObservableRecipient
    {
        [ObservableProperty]
        private bool showEs;

        [ObservableProperty]
        private bool zhTrad;

        [ObservableProperty]
        private bool showSpelling = true;

        [ObservableProperty]
        private bool isGb2312;

        [ObservableProperty]
        private bool isSingleChar;

        [ObservableProperty]
        private bool isFullShape;

        public AttributeModel()
        {
            LoadConfig();
        }

        public void SaveConfig()
        {
            Dictionary<string, string> dcit = GetPropertys();

            foreach (var k in dcit.Keys)
            {
                ConfigHelper.WriteConfigByString(k, dcit[k]);
            }

        }

        public void LoadConfig()
        {
            foreach (var p in this.GetType().GetProperties())
            {
                if (p.Name == "IsActive") continue;
                string value = ConfigHelper.ReadConfigByString(p.Name, "false");
                p.SetValue(this, Convert.ChangeType(value, p.PropertyType));
            }
        }

        private Dictionary<string, string> GetPropertys()
        {
            Dictionary<string, string> tempDict = new Dictionary<string, string>();
            foreach (var p in this.GetType().GetProperties())
            {
                if (p.Name == "IsActive") continue;
                var v = this.GetType().GetProperty(p.Name).GetValue(this, null);
                tempDict.Add(p.Name, v.ToString());
            }
            return tempDict;
        }
    }
}
