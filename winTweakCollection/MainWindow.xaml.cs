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
        //объявляем переменные
        private static string[] inifiles;// ини файлы с польховательскими твиками
        private static int count;// кол-во файлов
        private static TextBlock[] tbarray;// Текст блоки для твиков
        private static CheckBox[] checks;// Чекбоксы для твиков
        private static RegTweak[] regTweaks;// сами твики

        public MainWindow()
        {
            InitializeComponent();
            Loaded += MainWindow_Loaded;

            string ininame = "LongPathsEnabled" + ".ini"; // имя файла с твиком по умолчанию(если нет папки с твиками создается папка и этот файл) 

            if (!Directory.Exists(".\\Tweaks"))
            {
                Directory.CreateDirectory(".\\Tweaks");

                INIManager manager = new INIManager(".\\Tweaks\\" + ininame);
                manager.WritePrivateString("REG", "ROOT", @"HKEY_LOCAL_MACHINE");
                manager.WritePrivateString("REG", "PATH", @"SYSTEM\CurrentControlSet\Control\FileSystem");
                manager.WritePrivateString("REG", "NAME", @"HKEY_LOCAL_MACHINE");
                manager.WritePrivateString("REG", "VALUE", @"0");
            }

            // значения для переменных
            inifiles = Directory.GetFiles(".\\Tweaks", "*.ini", SearchOption.TopDirectoryOnly);
            count = inifiles.Length;
            tbarray = new TextBlock[count];
            regTweaks = new RegTweak[count];
            checks = new CheckBox[count];
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            //применяем твики для которых посталены чекбоксы
            for (int i = 0; i < count; i++)
            {
                bool isChecked = (bool)checks[i].IsChecked;
                if (isChecked)
                {
                    TBox.AppendText(regTweaks[i].SaveNewValue());
                    if (regTweaks[i].CmdExist)
                    {
                        //запуск процесса с смд
                        Process cmd = new Process();
                        cmd.StartInfo.FileName = "cmd.exe";
                        //cmd.StartInfo.Arguments = "/C " + regTweaks[i].Cmd;
                        cmd.StartInfo.RedirectStandardOutput = true;
                        cmd.StartInfo.CreateNoWindow = true;
                        cmd.StartInfo.UseShellExecute = false;
                        cmd.Start();

                        cmd.StandardInput.WriteLine("/C " + regTweaks[i].Cmd);
                        cmd.StandardInput.Flush();
                        cmd.StandardInput.Close();
                        cmd.WaitForExit();
                        TBox.AppendText(cmd.StandardOutput.ReadToEnd());
                    }
                }
            }

        }
        private void MainWindow_Loaded(object s, RoutedEventArgs e)
        {
            
            for (int i = 0; i < count; i++) // цикл для создания визуальнх компоненнтов
            {
                TBox.AppendText(inifiles[i] + "\n");
                regTweaks[i] = ReadiniFile(inifiles[i]); //заполнение массива с твиками
                tbarray[i] = new TextBlock { Text = regTweaks[i].Name + "\t" }; //заполнение массива с текстблоками
                checks[i] = new CheckBox { Margin = new Thickness(5 ,5 ,5 ,5) }; //заполнение массива с чекбоксами
                if (regTweaks[i].ValueExist) 
                    if (regTweaks[i].NewValue.ToString() != regTweaks[i].KeyValue.ToString())
                    {
                        checks[i].IsChecked = true;
                        tbarray[i].Text += regTweaks[i].ValueName;
                    }
                    else
                    {
                        checks[i].IsEnabled = false;
                        tbarray[i].Text += "уже выполнено";
                    }
                else
                {
                    checks[i].IsChecked = true;
                    tbarray[i].Text += "не найдено";
                }
                stackpan1.Children.Add(tbarray[i]); stackpan1.Children.Add(checks[i]);
            }

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

            string[] lines = File.ReadAllLines(ini);
            foreach (string line in lines)
            {
                //TBox.AppendText(line + "\n");
                if (line.StartsWith("[CMD]"))
                {
                    //TBox.AppendText(ini + " CMD is true\n");
                    tw.CmdExist = true;
                }
            }
            if (tw.CmdExist)
                tw.Cmd = manager.GetPrivateString("CMD", "cmd");

            return tw;
        }
    }
}
