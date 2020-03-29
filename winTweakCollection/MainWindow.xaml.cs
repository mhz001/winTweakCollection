using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Diagnostics;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.IO;

namespace winTweakCollection
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        public MainWindow()
        {
            InitializeComponent();
            Loaded += MainWindow_Loaded;

            string ininame = "LongPathsEnabled" + ".ini";

            if (!Directory.Exists(".\\Tweaks"))
            {
                Directory.CreateDirectory(".\\Tweaks");

                INIManager manager = new INIManager(".\\Tweaks\\" + ininame);
                manager.WritePrivateString("REG", "ROOT", @"HKEY_LOCAL_MACHINE");
                manager.WritePrivateString("REG", "PATH", @"SYSTEM\CurrentControlSet\Control\FileSystem");
                manager.WritePrivateString("REG", "NAME", @"HKEY_LOCAL_MACHINE");
                manager.WritePrivateString("REG", "VALUE", @"0");
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            _ = MessageBox.Show("nothing");
        }
        private void MainWindow_Loaded(object s, RoutedEventArgs e)
        {
            string[] inifiles = Directory.GetFiles(".\\Tweaks", "*.ini", SearchOption.TopDirectoryOnly);
            int count = inifiles.Length;
            TextBlock[] tbarray = new TextBlock[count];
            RegTweak[] regTweaks = new RegTweak[count];
            CheckBox[] checks = new CheckBox[count];
            
            for (int i = 0; i < count; i++)
            {
                TBox.AppendText(inifiles[i] + "\n");
                regTweaks[i] = ReadiniFile(inifiles[i]);
                tbarray[i] = new TextBlock { Text = regTweaks[i].Name + "\t" };
                checks[i] = new CheckBox { Margin = new Thickness(5 ,5 ,5 ,5) };
                if (regTweaks[i].ValueExist) 
                    if (regTweaks[i].NewValue.ToString() != regTweaks[i].KeyValue.ToString())
                    {
                        checks[i].IsChecked = true;
                        tbarray[i].Text += regTweaks[i].keyName;
                    }
                    else
                    {
                        checks[i].IsEnabled = false;
                        tbarray[i].Text += "Alredy Done.";
                    }
                else
                {
                    checks[i].IsChecked = true;
                    tbarray[i].Text += "not foundet";
                }
                stackpan1.Children.Add(tbarray[i]); stackpan1.Children.Add(checks[i]);
            }

            // LUA ConsentPromptBehaviorAdmin PromptOnSecureDesktop 0 0 0 off  1 5 1 on
            //[HKEY_CURRENT_USER\Software\Microsoft\Windows\CurrentVersion\Explorer\HideDesktopIcons\NewStartPanel]
            //"{ 20D04FE0 - 3AEA - 1069 - A2D8 - 08002B30309D}"= dword:00000000
            //System.IO.Path.GetFileNameWithoutExtension(Process.GetCurrentProcess().MainModule.FileName); //имя без расширения
            //System.Environment.GetCommandLineArgs()[0];// полный путь


        }

        private RegTweak ReadiniFile(string ini)
        {
            string ROOT, PATH, NAME, VALUE;

            INIManager manager = new INIManager(ini);
            ROOT = manager.GetPrivateString("REG", "ROOT");
            PATH = manager.GetPrivateString("REG", "PATH");
            NAME = manager.GetPrivateString("REG", "NAME");
            VALUE = manager.GetPrivateString("REG", "VALUE");

            RegTweak tw = new RegTweak(ROOT, PATH, NAME)
            {
                NewValue = VALUE,
                Name = System.IO.Path.GetFileNameWithoutExtension(ini)
            };

            //string[] lines = File.ReadAllLines(ini);
            //foreach (string line in lines)
            //{
            //    TBox.AppendText(line + "\n");
            //}

            return tw;
        }
    }
}
