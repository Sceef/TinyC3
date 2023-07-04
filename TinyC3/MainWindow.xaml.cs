using Ionic.Zip;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using TinifyAPI;

namespace TinyC3
{

    public partial class MainWindow : Window
    {
        string path = System.IO.Path.Combine(Environment.CurrentDirectory, @"key.txt");
        public int curFile = 0;
        public string[] files;
        public bool Start = false;
        public MainWindow()
        {
            InitializeComponent();
            ProgressBar.Visibility = Visibility.Visible;

            this.Loaded += delegate
            {
                Grid.AllowDrop = true;
                Grid.PreviewDragEnter += FolderInPreviewDragEnter;
                Grid.PreviewDragOver += FolderInPreviewDragOver;
                Grid.Drop += FolderInDragDrop;
                if (File.Exists(path)) { TextAPI.Text = File.ReadAllText(path); }
            };
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            Tinify.Key = TextAPI.Text;
        }

        public async void CompressOne(string[] files)
        {

            foreach (string filename in files)
            {
                if (Regex.IsMatch(filename, @"\.jpg$|\.png$|\.jpeg$"))
                {
                    var source = Tinify.FromFile(filename.ToString());

                    var converted = source.Convert(new { type = new[] { "image/webp" } });
                    Result response = await converted.GetResult();
                    await converted.ToFile(filename.ToString().Split('.')[0] + "." + response.Extension);
                    System.IO.File.Delete(filename.ToString());
                    ProgressBar.Value++;
                    curFile++;
                    TextFile.Text = curFile + "/" + files.Count();
                }
            }
            Complite();


        }
        public async void Compress(List<string> filez)
        {

            foreach (string filename in filez)
            {
                if (Regex.IsMatch(filename, @"\.jpg$|\.png$|\.jpeg$"))
                {
                    var source = Tinify.FromFile(filename.ToString());

                    var converted = source.Convert(new { type = new[] { "image/webp" } });
                    Result response = await converted.GetResult();
                    await converted.ToFile(filename.ToString().Split('.')[0] + "." + response.Extension);
                    System.IO.File.Delete(filename.ToString());
                    ProgressBar.Value++;
                    curFile++;
                    TextFile.Text = curFile + "/" + files.Count();
                }
            }
            if (Start) { if (curFile == files.Count()) { Complite(); Start = false; } }

        }

        public void Complite()
        {
            using (var loanZip = new Ionic.Zip.ZipFile())
            {
                loanZip.AddDirectory(System.IO.Path.Combine(Environment.CurrentDirectory, "TinyTemp"));
                loanZip.Save(Environment.CurrentDirectory + @"\Compress.zip");
            }


            System.IO.Directory.Delete(Environment.CurrentDirectory + @"\TinyTemp", true);
            TextFile.Text = "COMPRESSION COMPLITE";
            System.Media.SystemSounds.Beep.Play();
        }
        void FolderInPreviewDragOver(object sender, DragEventArgs e)
        {
            e.Handled = true;
        }

        private void FolderInPreviewDragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                e.Effects = DragDropEffects.Copy;
            }
            else
            {
                e.Effects = DragDropEffects.None;
            }
        }

        private void FolderInDragDrop(object sender, DragEventArgs e)
        {
            // Get data object
            var dataObject = e.Data as DataObject;

            // Check for file list
            if (dataObject.ContainsFileDropList())
            {

                // Process file names
                StringCollection fileNames = dataObject.GetFileDropList();
                StringBuilder bd = new StringBuilder();
                foreach (var fileName in fileNames)
                {
                    bd.Append(fileName + "\n");
                    if ((File.GetAttributes(fileName) & System.IO.FileAttributes.Directory) != System.IO.FileAttributes.Directory)
                    {

                        if (fileName.EndsWith(".zip"))
                        {
                            string zipPath = fileName;
                            string path = Environment.CurrentDirectory + @"\TinyTemp";
                            using (var zip = Ionic.Zip.ZipFile.Read(zipPath))
                            {
                                zip.ExtractAll(path, ExtractExistingFileAction.OverwriteSilently);
                            }
                            //замена формата картинки
                            var json = Environment.CurrentDirectory + @"\TinyTemp\data.json";
                            string text = File.ReadAllText(json);
                            text = text.Replace(".png", ".webp");
                            text = text.Replace(".jpg", ".webp");
                            text = text.Replace(".jpeg", ".webp");
                            File.WriteAllText(json, text);
                            files = Directory.GetFiles(Environment.CurrentDirectory + @"\TinyTemp\images", "*.*", SearchOption.TopDirectoryOnly);
                            //Работа с картинками
                            TextFile.Text = 0 + "/" + files.Count();
                            curFile = 0;
                            Start = true;
                            ProgressBar.Maximum = files.Count();
                            ProgressBar.Value = 0;
                            if (files.Count() < 4) { new Thread(() => { Dispatcher.Invoke((Action)(() => { CompressOne(files); })); }).Start(); }
                            else if (files.Count() >= 4)
                            {
                                var filesList = files.Select((x, i) => new { Index = i, Value = x })
                                .GroupBy(x => x.Index / 2)
                                .Select(x => x.Select(v => v.Value).ToList())
                                .ToList();

                                foreach (var file0 in filesList)
                                {
                                    new Thread(() =>
                                    {
                                        Dispatcher.Invoke((Action)(() =>
                                        {
                                            Compress(file0);
                                        }));
                                    }).Start();
                                }

                            }


                        }

                    }

                }

            }
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            Process.Start("www.donationalerts.com/r/alexandrpeshkov");
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            Process.Start("www.yandex.ru/games/developer?name=Tufty%20Fluff");
        }

        private void Button_Click_3(object sender, RoutedEventArgs e)
        {
            Process.Start("www.t.me/sceef");
        }
    }

}
