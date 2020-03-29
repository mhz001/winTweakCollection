using System;
using System.Windows;
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
        private RegistryHive rh = RegistryHive.LocalMachine;
        private bool valueExist;
        private object keyValue;
        private RegistryValueKind keyType;
        private object newValue = null;

        public RegTweak(string subKey, string valueName): this ("HKLM", subKey, valueName)  {  }

        public RegTweak(string rootKey, string subKey, string valueName)
        {
            Name = "key";
            this.rootKey = rootKey;
            this.subKey = subKey;
            this.valueName = valueName;
            keyName = $"{rootKey}\\{subKey}";
            //if ((rootKey == "HKLM") || (rootKey == "HKEY_LOCAL_MACHINE"))
            //    rh = RegistryHive.LocalMachine;
            if ((rootKey == "HKCU") || (rootKey == "HKEY_CURRENT_USER"))
                rh = RegistryHive.CurrentUser;
            if ((rootKey == "HKCU") || (rootKey == "HKEY_CLASSES_ROOT"))
                rh = RegistryHive.ClassesRoot;
            RegistryKey key = RegistryKey.OpenBaseKey(rh, rv).OpenSubKey(subKey);
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

        public string Name { get; set; }
        public string RootKey { get => rootKey; set => rootKey = value; }
        public string SubKey { get => subKey; set => subKey = value; }
        public string ValueName { get => valueName; set => valueName = value; }
        public bool ValueExist { get => valueExist; set => valueExist = value; }
        public RegistryValueKind KeyType { get => keyType; set => keyType = value; }
        public object KeyValue { get => keyValue; }
        public object NewValue { get => newValue; set => newValue = value; }

        public void SaveNewValue()
        {
            try
            {
                RegistryKey key = RegistryKey.OpenBaseKey(rh, rv).OpenSubKey(subKey, true);
                key.SetValue(valueName, newValue, keyType);
            }
            catch (ArgumentNullException e)
            {
                _ = MessageBox.Show(e.Message); //Свойство value имеет значение null.
            }
            catch (ArgumentException e)
            {
                _ = MessageBox.Show(e.Message); //Значение параметра value является неподдерживаемым типом данных.
            }
            catch (ObjectDisposedException e)
            {
                _ = MessageBox.Show(e.Message); //Объект RegistryKey, содержащий заданное значение, закрыт(доступ к закрытым разделам отсутствует).
            }
            catch (UnauthorizedAccessException e)
            {
                _ = MessageBox.Show(e.Message); //Раздел RegistryKey является разделом только для чтения, и запись в него невозможна. Например, этот раздел не был открыт с доступом для записи.
            }
        }
    }
}
