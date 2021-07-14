using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;

namespace vrisian
{
    public class DirectoryItem
    {
        private DirectoryInfo item;
        private ObservableCollection<DirectoryItem> subItems;

        public string Name
        {
            get { return item.Name; }
        }

        public string FullPath
        {
            get
            {
                return item.FullName;
            }

            set
            {
                item = new DirectoryInfo(value);
                if (Root == null)
                {
                    Root = value;
                }
            }
        }

        public string SubPath
        {
            get
            {
                return FullPath.Substring(Root.Length);
            }
            
        }

        public string Root { get; set; }

        public ObservableCollection<DirectoryItem> SubItems
        {
            get
            {
                if (subItems is null)
                {
                    try
                    {
                        subItems = new ObservableCollection<DirectoryItem>();
                        if (Directory.Exists(item.FullName))
                        {
                            var dirs = item.GetDirectories();
                            var files = item.GetFiles();
                            foreach (DirectoryInfo dir in dirs)
                            {
                                subItems.Add(new DirectoryItem { FullPath = dir.FullName, Root = Root });
                            }

                            foreach (FileInfo file in files)
                            {
                                subItems.Add(new DirectoryItem { FullPath = file.FullName, Root = Root });
                            }

                            Console.WriteLine(item.Name + ": " + dirs.Length.ToString() + " dir(s) / " + files.Length.ToString() + " file(s)");
                        }
                        
                    }
                    catch (Exception ex)
                    {
                        Trace.WriteLine(ex.Message);
                    }
                }
                return subItems;
            }
        } 
    }
}