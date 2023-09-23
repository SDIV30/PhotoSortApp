using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Diagnostics;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
//using System.Windows.Shell;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Forms;
//using static System.Windows.Forms.VisualStyles.VisualStyleElement.Window;
using Path = System.IO.Path;
using Ookii.Dialogs.Wpf;
using Microsoft.WindowsAPICodePack.Shell;
using Microsoft.WindowsAPICodePack.Dialogs;
using Microsoft.WindowsAPICodePack.Taskbar;
using System.Net;
using System.Configuration;
using System.Collections.Specialized;
//using Microsoft.Win32;

namespace WpfApp1
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        // string ImPath ="";
        readonly string[] extensions = new[] { ".jpg", ".png", ".bmp", ".jpeg" };//доступные расширения пикч мб .CR2
        List<FileInfo> picturesList = new List<FileInfo>();
        List<FileInfo> favoritePicturesList = new List<FileInfo>();//избранные файлы
        //JumpList resF = new JumpList();
        
        //resF.ShowRecentCategory = true; //джамплист ДОДЕЛАТЬ!!!
        int MaxFiles=0;
        int currentFiles = 0;
        string newFilePath = "";

        public StringCollection recentFilesCollection { get { return Properties.Settings.Default.RecentFiles; } }
        public List<string> www { get { return new List<string> { "111", "222" }; } }
        public StringCollection shortName { get { return Properties.Settings.Default.RecentFilesName; } }


        public MainWindow()
        {
            InitializeComponent();
            this.DataContext = this;
/*
            qqq.Insert(0,"123");
            while(qqq.Count > 10)
            {
                qqq.RemoveAt(10);
            }
            Properties.Settings.Default.Save();
*/

        }
        //private void Zoom(object sender, RoutedEventArgs e)//добавить возможность по клику на картинку открывать её фулскрин
        //{
        //    Fzoom();
        //}

        private void ImgSetSize(object sender, RoutedEventArgs e)
        {
            double titleBarHeight = SystemParameters.CaptionHeight + SystemParameters.ResizeFrameHorizontalBorderHeight + SystemParameters.BorderWidth;
            ImagePic.MaxWidth = this.ActualWidth;
            ImagePic.MaxHeight = this.ActualHeight - titleBarHeight-40.0;
            loadHeart();
        }

        private void GDirectory(object sender, RoutedEventArgs e)//xaml 18 57
        { 
            CommonOpenFileDialog defaultFilePath = new CommonOpenFileDialog();
            defaultFilePath.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures);
            defaultFilePath.IsFolderPicker = true;

            if(defaultFilePath.ShowDialog() == CommonFileDialogResult.Ok)
            {   
                if (picturesList.Count != 0)
                {
                    picturesList.Clear();
                    MaxFiles = 0;
                    currentFiles = 0;
                    favoritePicturesList.Clear();
                }
                //fromFolderLoad(defaultFilePath.FileName);
                newFilePath = defaultFilePath.FileName;
                DirectoryInfo picturesDirectory = new DirectoryInfo(newFilePath);
                //ImPath = Npath;
                picturesList = picturesDirectory.GetFiles().Where(f => extensions.Contains(f.Extension.ToLower())).ToList();//набор в массив по расширению

                if (picturesList.Count != 0)
                {//подгрузка менюшки
                    //qqq.Insert(0, picDir.FullName);// продумать коротокий путь + строка 274
                    //shrtName.Insert(0, picDir.Root + "...\\" + picDir.Parent + "\\" + picDir.Name);//короткий путь
                    //while (qqq.Count > 10)
                    //{
                    //    shrtName.RemoveAt(10);
                    //    qqq.RemoveAt(10);
                    //}
                    //Properties.Settings.Default.Save();
                    //работа с счетчиком и тп
                    PathFold.Content = newFilePath;
                    MaxFiles = picturesList.Count() - 1;
                    PathFold.Content = picturesList[0];
                    var Uri = new Uri($"{newFilePath}\\{Convert.ToString(picturesList[currentFiles])}");
                    var bitmap = new BitmapImage(Uri);
                    ImagePic.Source = bitmap;
                    counterPics.Content = Convert.ToString(currentFiles + 1) + "/" + Convert.ToString(MaxFiles + 1);
                    loadHeart();
                }
                else
                    System.Windows.MessageBox.Show("Directory has no files or none of them are: JPG,PNG,JPEG,BMP.");
            }
        }
        //private void fromFolderLoad(string path)//если что закоментить
        //{
        //    DirectoryInfo dirInfo = new DirectoryInfo(path);
        //    newFilePath = dirInfo.FullName + "\\";
        //    DirectoryInfo picturesDirectory = new DirectoryInfo(newFilePath);
        //    //ImPath = Npath;
        //    picturesList = picturesDirectory.GetFiles().Where(f => extensions.Contains(f.Extension.ToLower())).ToList();//набор в массив по расширению

        //    if (picturesList.Count != 0)
        //    {//подгрузка менюшки
        //        recentFilesCollection.Insert(0, picturesDirectory.FullName);// продумать коротокий путь + строка 274
        //        shortName.Insert(0, picturesDirectory.Root + "...\\" + picturesDirectory.Parent + "\\" + picturesDirectory.Name);//короткий путь
        //        while (recentFilesCollection.Count > 10)
        //        {
        //            shortName.RemoveAt(10);
        //            recentFilesCollection.RemoveAt(10);
        //        }
        //        Properties.Settings.Default.Save();
        //        //работа с счетчиком и тп
        //        PathFold.Content = newFilePath;
        //        MaxFiles = picturesList.Count() - 1;
        //        PathFold.Content = picturesList[0];
        //        var Uri = new Uri(newFilePath + '\\' + Convert.ToString(picturesList[currentFiles]));
        //        var bitmap = new BitmapImage(Uri);
        //        ImagePic.Source = bitmap;
        //        counterPics.Content = Convert.ToString(currentFiles + 1) + "/" + Convert.ToString(MaxFiles + 1);
        //    }
        //    else
        //        System.Windows.MessageBox.Show("Directory has no files or none of them are: JPG,PNG,JPEG,BMP.");
        //}

        private void loadHeart()//zoom on image with click
        {
            
            var uriFavorite = new Uri(@"\Imagery\heartF.png", UriKind.Relative);
            var uriNormal = new Uri(@"\Imagery\heartNF.png", UriKind.Relative);
            var bitmapFavorite = new BitmapImage(uriFavorite);
            var bitmapNormal = new BitmapImage(uriNormal);

            if ((favoritePicturesList.Count != 0) && (favoritePicturesList.Contains(picturesList[currentFiles])))
            { 
                HeartFav.Source = bitmapFavorite;
                FavImg.Content = "Unfavorite";
            }
            else
            { 
                HeartFav.Source = bitmapNormal;
                FavImg.Content = "Favorite";
            }
        }

        private void FavI() //Favourite image 
        {
            if (newFilePath.Length != 0)
            {
                DirectoryInfo picturesDirectory = new DirectoryInfo(newFilePath);//в список накидам файл, потом экспорт 
                if (picturesList.Count != 0 && !favoritePicturesList.Contains(picturesList[currentFiles]))
                {
                    favoritePicturesList.Add(picturesList[currentFiles]);//список собирает файлы из нынешней директории, при смене чистится
                    loadHeart();
                }
                   
                    
                else
                    { //паралельно проверяются файлы они в этом листе или нет, чтоб показать сердечко на фаворитов
                    //string message = "Image is already in favorites, would you like to remove it?";
                    //string title = "Duplicate";
                    //if(System.Windows.MessageBox.Show(message, title, MessageBoxButton.OKCancel) == MessageBoxResult.OK)
                    //{
                        favoritePicturesList.Remove(picturesList[currentFiles]);
                        loadHeart();
                    //}
                }
                }
            }
            
        private void SaveI()//ДОБАВИТЬ СОХРАНЕНИЕ ФАВОРИТОВ
        {
            if (favoritePicturesList.Count!=0)
            {
                CommonOpenFileDialog _fileDialog = new CommonOpenFileDialog();
                
                _fileDialog.InitialDirectory= Environment.GetFolderPath(Environment.SpecialFolder.MyPictures);
                _fileDialog.IsFolderPicker = true;
                //Nullable<bool> result = fvdlg.ShowDialog();
                MessageBoxResult Mres;
                //var msg = "File " + favoritePicturesList[currentFiles].Name +" already exists in directory, rewrite it?";
                var titlem = "File already in folder";
                if (_fileDialog.ShowDialog()==CommonFileDialogResult.Ok)
                {
                    string path = Path.GetDirectoryName(_fileDialog.FileName); //выбирает не ту папку                                                                   
                    for (int i = 0; i < favoritePicturesList.Count; i++)
                    {
                        var msg = "File " + favoritePicturesList[i].Name + " already exists in directory, rewrite it?";
                        string FromDir = newFilePath + "\\" + favoritePicturesList[i];
                        string NewPath = Path.Combine(path, Convert.ToString(favoritePicturesList[i]));//new dir + file to which we'll write
                        if (path != newFilePath)
                        {
                            if (File.Exists(NewPath))
                            {
                                Mres = System.Windows.MessageBox.Show(msg, titlem, MessageBoxButton.YesNo, MessageBoxImage.Question);
                                if (Mres == MessageBoxResult.Yes)//убедиться что пути разные у файлов(не перизаписываем тот же файл тем что у нас уже есть)
                                    File.Copy(FromDir, NewPath, overwrite: true);
                            }
                            else
                                File.Copy(FromDir, NewPath);
                        }
                        else
                            System.Windows.MessageBox.Show("Check if you're not saving into origin directory","Directory is currently in use",MessageBoxButton.OK,MessageBoxImage.Error);
                    }
                    System.Windows.MessageBox.Show("Files were successfully saved", "Save complete", MessageBoxButton.OK, MessageBoxImage.Information);
                }             
            }
            
        }
        private void NextI() //next image //добавить загрузку сердечка
        {   
            
            if (MaxFiles != 0)
            {
                if (currentFiles == MaxFiles)
                    currentFiles = 0;
                else
                    currentFiles++;

                PathFold.Content = picturesList[currentFiles];
                counterPics.Content = Convert.ToString(currentFiles + 1) + "/" + Convert.ToString(MaxFiles + 1);
                var Uri = new Uri(newFilePath + '\\' + Convert.ToString(picturesList[currentFiles]));
                var bitmap = new BitmapImage(Uri);
                ImagePic.Source = bitmap;
                loadHeart();
            }
        }
        private void PrevI() //previous image
        {
            if (MaxFiles != 0)
            {
                if (currentFiles == 0)
                {
                    currentFiles = MaxFiles;
                }
                else
                    currentFiles--;
                PathFold.Content = picturesList[currentFiles];

                counterPics.Content = Convert.ToString(currentFiles + 1) + "/" + Convert.ToString(MaxFiles + 1);
                var Uri = new Uri(newFilePath + '\\' + Convert.ToString(picturesList[currentFiles]));
                var bitmap = new BitmapImage(Uri);
                ImagePic.Source = bitmap;
                loadHeart();
            }
        }

        //методы на кнопки
        private void SImg(object sender, RoutedEventArgs e)
        { 
            SaveI();
        }
        private void FImg(object sender, RoutedEventArgs e)
        {
            FavI();
        }
        private void NImg(object sender, RoutedEventArgs e)
        {
            NextI();
        }
        private void PImg(object sender, RoutedEventArgs e)
        {
            PrevI();
        }
        //методы на клавиши
        private void Grid_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == Key.Right)
            { NextI(); }
            if (e.Key == Key.Left)
            { PrevI(); }
            //на пробел добавляем в избранное
            if (e.Key == Key.Space)
            { FavI(); }
        }

        private void MakeMenuItem(object sender, RoutedEventArgs e)//currently unused неиспользуется
        {
            //получили короткое имя, втянули название папки по нему ищим полный путь и загружаем
            //string realName= Environment.GetFolderPath(Environment.SpecialFolder.MyPictures);
            //var fff = ((MenuItem)(e.OriginalSource)).DataContext;//сделать наоборот
            //int itemIndex = shortName.IndexOf(fff as string);
            //DirectoryInfo bigPath = new DirectoryInfo(shortName[itemIndex]);
            //var smallPath= bigPath.Parent+"\\"+ bigPath.Name;
            //var diskName = bigPath.Root;
            //for(int i = 0; i < recentFilesCollection.Count; i++)
            //{
            //    if (recentFilesCollection[i].Contains(smallPath) && recentFilesCollection[i].Contains(Convert.ToString(diskName)) == true)
            //    {
            //        realName = recentFilesCollection[i];//расшифровка в полное имя
            //        break;
            //    }
            //}
            ///////////////////////////
            //shrtName.Contains(smalP)
            //fromFolderLoad(realName);
            
            //что то сделать с имадж инфо взять рут парент и тд тп по ним отыскать нашу вторую коллекцию только все наоборот
            //тк в меню будет короткий путь, а на деле ищем и загружаем из длинного

            //var rrr = ((MenuItem)(e.O))

        }
    }
}
