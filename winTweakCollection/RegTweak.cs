using System;
using Microsoft.Win32;

namespace winTweakCollection
{
    internal class RegTweak
    {
        private string rootKey;
        private string subKey;
        public string keyName;
        private string valueName;
        private readonly RegistryView rv = RegistryView.Default;
        private readonly RegistryHive rh;
        private bool valueExist;
        private object keyValue;
        private RegistryValueKind keyType;
        private object newValue = null;
        private string cmd = "";
        private bool cmdExist = false;

        public RegTweak(string subKey, string valueName): this ("HKLM", subKey, valueName)  {  }

        public RegTweak(string rootKey, string subKey, string valueName)
        {
            Name = "key";
            this.rootKey = rootKey;
            this.subKey = subKey;
            this.valueName = valueName;
            keyName = $"{rootKey}\\{subKey}";
            rh = RegistryHive.LocalMachine;
            if ((rootKey == "HKCU") || (rootKey == "HKEY_CURRENT_USER"))
                rh = RegistryHive.CurrentUser;
            if ((rootKey == "HKCU") || (rootKey == "HKEY_CLASSES_ROOT"))
                rh = RegistryHive.ClassesRoot;

            RegistryKey key = RegistryKey.OpenBaseKey(rh, rv).OpenSubKey(subKey);
            if (key != null)
            {
                keyValue = key.GetValue(valueName);
                if (keyValue is null)
                {
                    valueExist = false;
                    keyType = RegistryValueKind.DWord;
                    keyValue = 0;
                }
                else
                {
                    valueExist = true;
                    keyType = key.GetValueKind(valueName);
                }
            }
            else
            {
                valueExist = false;
                keyType = RegistryValueKind.DWord;
                keyValue = 0;
            }
        }

        public string Name { get; set; }
        public string RootKey { get => rootKey; set => rootKey = value; }
        public string SubKey { get => subKey; set => subKey = value; }
        public string ValueName { get => valueName; set => valueName = value; }
        public bool ValueExist { get => valueExist; set => valueExist = value; }
        public RegistryValueKind KeyType { get => keyType; set => keyType = value; }
        public object KeyValue { get => keyValue; }
        public object NewValue { get => newValue; set => newValue = value; }
        public bool CmdExist { get => cmdExist; set => cmdExist = value; }
        public string Cmd { get => cmd; set => cmd = value; }

        public string SaveNewValue()
        {
            try
            {
                RegistryKey key = RegistryKey.OpenBaseKey(rh, rv).CreateSubKey(subKey, true);
                key.SetValue(valueName, newValue, keyType);
                return valueName + " записан\n";
            }
            catch (ArgumentException e)
            {
                //_ = MessageBox.Show(e.Message); //Значение параметра value является неподдерживаемым типом данных.
                return e.Message;
            }
            catch (ObjectDisposedException e)
            {
                //_ = MessageBox.Show(e.Message); //Объект RegistryKey, содержащий заданное значение, закрыт(доступ к закрытым разделам отсутствует).
                return e.Message;
            }
            catch (Exception e)
            {
                return e.Message;
            }
        }
    }
}
