using Registration;
using System;
using System.Windows.Forms;

namespace Company.VSAnything
{
	internal class RegistrationForm
	{
		public static bool ShowForm(Settings settings, bool show_in_taskbar)
		{
            //Registration.RegistrationForm form = new Registration.RegistrationForm(settings.EMail, settings.RegKey, show_in_taskbar, Resource1.FastFindIcon);
            //form.ShowDialog();
            //if (Registration.LooksLikeValidRegKey(form.EMail, form.RegKey))
            //{
            //    settings.EMail = form.EMail;
            //    settings.RegKey = form.RegKey;
            //    settings.Write();
            //}
            //string error = null;
            //if (Demo.Expired && !Registration.Registered)
            //{
            //    if (error != null)
            //    {
            //        MessageBox.Show("FastFind registration check error:\n" + error, "FastFind Error", MessageBoxButtons.OK, MessageBoxIcon.Hand);
            //    }
            //    return false;
            //}
			return true;
		}
	}
}
