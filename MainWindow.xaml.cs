using System;

using System.Windows;
using System.Windows.Input;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System.Security.Permissions;
using System.Security.Cryptography.X509Certificates;
using Microsoft.Win32;
using System.Drawing;
using System.Reflection;
using System.Security;
using System.IO;
using System.Diagnostics;
using TaskScheduler;


namespace Pro
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>


    public class GetScheduler
    {
        public List <string> sch_nm=new List<string>(); //string names in scheduler
        public List <string> sch_pth=new List<string>(); //string pathes in scheduler
        public List<string> sch_ow = new List<string>(); //owners of files
        public List<BitmapSource> sch_ic = new List<BitmapSource>();

        public byte AUTOSTART_TYPE = 3; //type Scheduler

        private string pt = Environment.GetFolderPath(Environment.SpecialFolder.Windows); //C:/Winsows/System32
        

        

        public GetScheduler()
        {
           
    }

        public void get_names_scheduler() //get names of scheduler file programms
        {
            string nm = pt + "\\"+"System32"+"\\"+"Tasks";//C:/Windows/System32/Tasks

            DirectoryInfo d = new DirectoryInfo(nm);
            if (!d.Exists) //if file doesn't exist
                return; //no file

            FileInfo[] f=d.GetFiles(); //get files list

            for(int i=0;i< f.Length;i++)
            {
                string tmp = f[i].Name;
                sch_nm.Add(tmp);

            }

            

        }


        public void get_path_scheduler() //get path of sceduler files
        {
            if (sch_nm.Count == 0)
                get_names_scheduler();

            string nm = pt + "\\" + "System32" + "\\" + "Tasks";  //C:/Windows/System32/Tasks
            DirectoryInfo d = new DirectoryInfo(nm);

            FileInfo[] f = d.GetFiles();
            for(int i=0;i< f.Length;i++)
            {
                string tmp = f[i].FullName;
                sch_pth.Add(tmp);
            }

        }


        public void get_owner_scheduler() //get owner
        {
            if (sch_pth.Count == 0)
                get_path_scheduler();
            for (int i = 0; i < sch_pth.Count; i++)
            {
                string ow = sch_pth[i]; //get parth

                FileInfo f = new FileInfo(ow);

                if (!f.Exists) //if file path does not exist
                {
                    sch_ow.Add(""); //no onwer
                    continue;
                }

                string tmp = f.GetAccessControl().GetOwner(typeof(System.Security.Principal.NTAccount)).ToString(); //get file owner
                sch_ow.Add(tmp);
            }



        }

        public void get_icon_scheduler()
        {
            if (sch_pth.Count == 0)
                get_path_scheduler(); //get files pathes of scheduler

            for (int i = 0; i < sch_pth.Count; i++)
            {
                string ow = sch_pth[i];
                FileInfo f = new FileInfo(ow);
                if(f.Exists==false) //if file does not exist
                {
                   BitmapSource nl = null; //does not exist
                   sch_ic.Add(nl);
                    continue;
                }

                Icon ic = System.Drawing.Icon.ExtractAssociatedIcon(ow); //extract icon from file path
                BitmapSource tmp = System.Windows.Interop.Imaging.CreateBitmapSourceFromHIcon(ic.Handle, new Int32Rect(0, 0, ic.Width, ic.Height), BitmapSizeOptions.FromEmptyOptions());
                sch_ic.Add(tmp);
            }

        }

    }

    public class GetDir //get  autostart programms from autostart path
    {

        public byte AUTOSTART_TYPE = 2; //autostart type is folder

        public List<string> nms_up = new List<string>(); //names of programms from startup directory
        public List<string> pth_up = new List<string>(); //names of pathes of programms
        public List<string> ow_up = new List<string>(); //owners of files
        public List<BitmapSource> ic_up = new List<BitmapSource>();

        public GetDir()
        {

        }

       private string pt = Environment.GetFolderPath(Environment.SpecialFolder.Startup); //get start menu path


        public void get_names_from_fil() //get files names from autostart folder
        {
            
            DirectoryInfo d = new DirectoryInfo(pt);
            FileInfo[] f = d.GetFiles(); //get files from startup directory

            if (f.Length <= 1) //if no files
                return ; //no files

            for(int i=0;i< f.Length;i++)
            {
               
                string tmp = f[i].Name;
                if (tmp == "desktop.ini") //desktop ini verpassen
                    continue;

                nms_up.Add(tmp); //add to the returned files names list

            }

           
        }


        public void get_pathes_from_fil() //get pathes from startup directory
        {
            if(nms_up.Count==0)
                get_names_from_fil(); //get files names

            for(int i=0;i<nms_up.Count;i++)
            {
                string tm = pt +"\\"+ nms_up[i]; //concatre file name to start menu path
                pth_up.Add(tm);
            }
            
        }

        public void get_owner() //get owners of programms from startup directory
        {
            if (pth_up.Count <= 1)
                get_pathes_from_fil();


            for(int i=0;i<pth_up.Count;i++)
            {
                string ow = pth_up[i];

                FileInfo f = new FileInfo(ow);
                if(!f.Exists)
                {
                    ow_up.Add("");
                    continue;
                }

                string tmp = f.GetAccessControl().GetOwner(typeof(System.Security.Principal.NTAccount)).ToString(); //get file owner
                ow_up.Add(tmp);
            }


        }


        public void get_icon()
        {
            if (pth_up.Count <= 1)
                get_pathes_from_fil();

            for (int i = 0; i < pth_up.Count; i++)
            {
                string ow = pth_up[i];
                FileInfo f = new FileInfo(ow);
                if(f.Exists==false) //if file does not exist
                {
                    BitmapSource nl = null; //does not exist
                    ic_up.Add(nl); //icon does not exist
                    continue;
                }

                Icon ic = System.Drawing.Icon.ExtractAssociatedIcon(ow); //extract icon from file path
                BitmapSource tmp = System.Windows.Interop.Imaging.CreateBitmapSourceFromHIcon(ic.Handle, new Int32Rect(0, 0, ic.Width, ic.Height), BitmapSizeOptions.FromEmptyOptions());
                ic_up.Add(tmp);
            }


        }

    }





    public class GetPro //get programm pathes and names  from 1)Registry
    {                   //                         2)AutoStart folder 
                        //                          3)Scheduler



        public  byte AUTOSTART_TYPE=1; //Aautostart type is registry

        public List<string> nms_reg = new List<string>(); //programms names from registry
        public List<string> pth_reg = new List<string>(); //programms pathes from registry
        public List<string> own_reg = new List<string>(); //owner
        public List<BitmapSource> ic_reg = new List<BitmapSource>();
       
       
        public GetPro()
        {

        }


        private RegistryKey get_reg_key() //get registry key
        {
            RegistryKey rg = Registry.LocalMachine; //LOCAL_MACHINE
            rg = rg.OpenSubKey("SOFTWARE");//SOFTWARE
            rg = rg.OpenSubKey("Microsoft"); //Microsoft
            rg = rg.OpenSubKey("Windows"); //Windows
            rg = rg.OpenSubKey("CurrentVersion"); //Current Version
            rg = rg.OpenSubKey("Run"); //Run

            return rg;
        }

         
        private string Trm(string s)
        {
            if(s.Contains("-") || s.Contains("/"))
            {
                string rt;
                int ind=s.Contains("-")?s.IndexOf("-"):s.IndexOf("/"); //get position of the charakter
                rt=s.Remove(ind ,(s.Length-ind)); //remove it

                if (rt.Substring(rt.Length - 3) != "exe")
                {   
                    while(rt.Substring(rt.Length-3)!="exe")
                        rt=rt.Remove(rt.Length - 1, 1); //delete last charakter
                }

                s = rt;
            }

            

            return s;
        }

       
        public void get_names_from_reg()             //1. get programms names from registry
        {

            RegistryKey reg_path = get_reg_key(); //get registry key
            string[] tmp = new string[reg_path.ValueCount];
            
            
            tmp=reg_path.GetValueNames();

            for(int i=0;i<tmp.Length;i++) //add values string names to the string list
                nms_reg.Add(tmp[i]);
            

            
        } 

        
        public void get_path_from_reg() //get programms pathes from registry
        {
         
            RegistryKey reg_path = get_reg_key(); //

            if(nms_reg.Count==0) //if no programms names got from registry
                get_names_from_reg(); //get programm names from registry


            for (int i = 0; i <nms_reg.Count; i++) //for each program from registry get its value (path)
            {
                string tmp = reg_path.GetValue(nms_reg[i]).ToString();
                tmp = Trm(tmp);
                pth_reg.Add(tmp); //add pathes of programms in registry to the List
            }
           

           

        }

        
        
        

        
        public void get_owner() //get owner company of programms
        {
            
            if(pth_reg.Count==0)
                get_path_from_reg(); //get programms pathes
           

            for (int i = 0; i < nms_reg.Count; i++)
            {
                string ow = Trm(pth_reg[i]); //get each file
                

                FileInfo inf = new FileInfo(ow); //get owner of each file path
                if (inf.Exists==false) //if doesnt exist
                {
                    own_reg.Add(""); //no owner
                    continue;
                }
                string tmp = inf.GetAccessControl().GetOwner(typeof(System.Security.Principal.NTAccount)).ToString(); //get file owner
                   
                own_reg.Add(tmp); //add to the result
            }
          
        }


        public void get_icon()
        {
            if (pth_reg.Count == 0)
                get_path_from_reg(); //get programms pathes

            for (int i = 0; i < pth_reg.Count; i++)
            {
                string ow = Trm(pth_reg[i]);
                FileInfo inf = new FileInfo(ow);

                if(inf.Exists==false)
                {
                    BitmapSource nl=null; //does not exist
                    ic_reg.Add(nl);
                    continue;
                }

                Icon ic = System.Drawing.Icon.ExtractAssociatedIcon(ow); //extract icon from file path
                BitmapSource tmp = System.Windows.Interop.Imaging.CreateBitmapSourceFromHIcon(ic.Handle, new Int32Rect(0, 0, ic.Width, ic.Height), BitmapSizeOptions.FromEmptyOptions());
                ic_reg.Add(tmp);
            }
        }
        
        

    }

   



    public partial class MainWindow : Window
    {

        private  static string pth;
      

        public static void op_folder(object sender, MouseButtonEventArgs e) //open folder in path
        {


            if (e.ChangedButton == MouseButton.Left) //if left button is clicked
            {
                
                FileInfo f = new FileInfo(pth);
                if (f.Exists == false) //if file path does not exists
                    System.Diagnostics.Process.Start("explorer.exe", Environment.GetFolderPath(Environment.SpecialFolder.Windows)); //open default folder

                System.Diagnostics.Process.Start("explorer.exe", Path.GetDirectoryName(pth)); //open file folder
            }
        }
        

       

        private void add_autostart_type(byte aut,int nm)  //PARAMS** 1) autostart type 2)how much time to add **
         {

            for (int i = 0; i < nm; i++)
            {
                TextBlock tp = new TextBlock();
                if (aut == 1) //reg
                    tp.Text = "Registry";    //add registry autostart
                else if (aut == 2)
                    tp.Text = "StartupFolder";
                else if (aut == 3)
                    tp.Text = "Scheduler";

                AutoType.Children.Add(tp);
            }
        }

       
        private void add_name(List <string>s) //add programm names to the text block 
        {

            for(int i=0;i<s.Count;i++)
            {
                TextBlock lb = new TextBlock();
                lb.Text = s[i];
                lb.HorizontalAlignment=HorizontalAlignment.Left;
                nam.Children.Add(lb);
            }
        }
       


       private void add_com(List <string> s) //add programms companies to the boxlist
       {
            for(int i=0;i<s.Count;i++)
            {
                TextBlock lb = new TextBlock();
                lb.Text = s[i];
                lb.HorizontalAlignment = HorizontalAlignment.Left;
                com.Children.Add(lb);
            }


       }

        private void add_path(List<string> s) //add paths to the boxlist
        {
            
           
            for(int i=0;i<s.Count;i++)
            {
                TextBlock lb = new TextBlock();
                lb.Text = s[i];
                pth = s[i];
                lb.HorizontalAlignment = HorizontalAlignment.Left;
                
                lb.MouseLeftButtonDown += new MouseButtonEventHandler(op_folder);

                path.Children.Add(lb);
            }
        }



        private void add_icon(List<BitmapSource> s)
        {
            
       
            for (int i = 0; i < s.Count; i++)
            {
                /*
                if(s[i]==null) //if does not exist
                {
                    
                }
                */

                System.Windows.Controls.Image im = new System.Windows.Controls.Image();
                im.Source = s[i];
                im.Width = 15;
                im.Height = 15;
                ico.Children.Add(im);

                
            }
        }


        public MainWindow()
        {
             InitializeComponent();


           
            
            
            
            GetPro one = new GetPro(); //get from registry
            GetDir two = new GetDir(); //programms from startup directoru
            GetScheduler three=new GetScheduler();


           

                 
                  one.get_names_from_reg(); //get progmrams names from registry
                  one.get_path_from_reg(); //get programmss pathes from registry
                  one.get_owner();
                  one.get_icon();
            
            
                  two.get_names_from_fil(); //get programms names list from Startup directory
                  two.get_pathes_from_fil(); //get pathes of programms from startup directory
                  two.get_owner();
                  two.get_icon();
            
                three.get_names_scheduler();
                three.get_path_scheduler();
                three.get_owner_scheduler();
                three.get_icon_scheduler();
            

            
            add_name(one.nms_reg);
            add_path(one.pth_reg);
            add_com(one.own_reg);
            add_icon(one.ic_reg);
            add_autostart_type(one.AUTOSTART_TYPE, one.nms_reg.Count);
            

            add_name(two.nms_up); //enlist programs names from registry in the textblock
            add_path(two.pth_up); //enlist pathes of  programss  in registry in textblock
            add_com(two.ow_up);
            add_autostart_type(two.AUTOSTART_TYPE,two.nms_up.Count);
            add_icon(two.ic_up);
            


           add_name(three.sch_nm);
            add_path(three.sch_pth);
            add_autostart_type(three.AUTOSTART_TYPE, three.sch_nm.Count);
            add_com(three.sch_ow);
            add_icon(three.sch_ic);
          
        }


       
    }
    
}
