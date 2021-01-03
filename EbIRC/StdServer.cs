using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using EbiSoft.EbIRC.Settings;
using EbiSoft.EbIRC.Properties;
using System.Runtime.InteropServices;

namespace EbiSoft.EbIRC
{
    public partial class StdServer : Form
    {
        public StdServer()
        {
            InitializeComponent();
        }

        private void menuItem1_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void StdServer_Load(object sender, EventArgs e)
        {
            tServer.Text = SettingManager.Data.Profiles.ActiveProfile.Server;
            tPort.Text = SettingManager.Data.Profiles.ActiveProfile.Port.ToString();
            tServerPassword.Text = SettingManager.Data.Profiles.ActiveProfile.Password;
            cSSL.Checked = SettingManager.Data.Profiles.ActiveProfile.UseSsl;
            cValidate.Checked = SettingManager.Data.Profiles.ActiveProfile.NoValidation;
            tNick.Text = SettingManager.Data.Profiles.ActiveProfile.Nickname;
            tLoginName.Text = SettingManager.Data.Profiles.ActiveProfile.LoginName;
            tNickPass.Text = SettingManager.Data.Profiles.ActiveProfile.NickServPassword;
        }

        private void menuItem2_Click(object sender, EventArgs e)
        {
            SettingManager.Data.Profiles.ActiveProfile.Server = tServer.Text;
            SettingManager.Data.Profiles.ActiveProfile.Port = int.Parse(tPort.Text);
            SettingManager.Data.Profiles.ActiveProfile.Password = tServerPassword.Text;
            SettingManager.Data.Profiles.ActiveProfile.UseSsl = cSSL.Checked;
            SettingManager.Data.Profiles.ActiveProfile.NoValidation = cValidate.Checked;
            SettingManager.Data.Profiles.ActiveProfile.Nickname = tNick.Text;
            SettingManager.Data.Profiles.ActiveProfile.LoginName = tLoginName.Text;
            SettingManager.Data.Profiles.ActiveProfile.NickServPassword = tNickPass.Text;

            SettingManager.WriteSetting();

            Close();
        }
    }
}