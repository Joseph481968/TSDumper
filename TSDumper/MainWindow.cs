﻿////////////////////////////////////////////////////////////////////////////////// 
//                                                                              //
//      Copyright (C) 2005-2012 nzsjb, Harun Esur                                           //
//                                                                              //
//  This Program is free software; you can redistribute it and/or modify        //
//  it under the terms of the GNU General Public License as published by        //
//  the Free Software Foundation; either version 2, or (at your option)         //
//  any later version.                                                          //
//                                                                              //
//  This Program is distributed in the hope that it will be useful,             //
//  but WITHOUT ANY WARRANTY; without even the implied warranty of              //
//  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the                //
//  GNU General Public License for more details.                                //
//                                                                              //
//  You should have received a copy of the GNU General Public License           //
//  along with GNU Make; see the file COPYING.  If not, write to                //
//  the Free Software Foundation, 675 Mass Ave, Cambridge, MA 02139, USA.       //
//  http://www.gnu.org/copyleft/gpl.html                                        //
//                                                                              //  
//////////////////////////////////////////////////////////////////////////////////

using System;
using System.Reflection;
using System.Windows;
using System.Windows.Forms;
using DVBServices;
using DirectShow;
using DomainObjects;
using Application = System.Windows.Forms.Application;
using MessageBox = System.Windows.Forms.MessageBox;

namespace TSDumper
{
    public partial class MainWindow : Form
    {
        private static MainWindow mainWindow;
        
        public MainWindow()
        {
            InitializeComponent();

            Logger.Instance.WriteSeparator("TSDumper (Version " + RunParameters.SystemVersion + ")");

            Logger.Instance.Write("Executable build: " + AssemblyVersion);
            Logger.Instance.Write("DirectShow build: " + DirectShowGraph.AssemblyVersion);
            Logger.Instance.Write("DomainObjects build: " + RunParameters.AssemblyVersion);
            Logger.Instance.Write("DVBServices build: " + Utils.AssemblyVersion);
            Logger.Instance.Write("");
            Logger.Instance.Write("Privilege level: " + RunParameters.Role);
            Logger.Instance.Write("");
            Logger.Instance.Write("Base directory: " + RunParameters.BaseDirectory);
            Logger.Instance.Write("Data directory: " + RunParameters.DataDirectory);
            Logger.Instance.Write("");

            mainWindow = this;

            BDAGraph.LoadTuners();
            if (Tuner.TunerCollection.Count == 0)
            {
                MessageBox.Show("There are no tuners installed on this machine.", "TSDumper", MessageBoxButtons.OK,
                                MessageBoxIcon.Error);
                Environment.Exit(10);
            }


            //mainTabControl.TabPages.Clear();

            for (int i = 0; i < Tuner.TunerCollection.Count;i++ )
            {
                Tuner t = Tuner.TunerCollection[i];
                t.Tag = i;
                TransportStreamDumpControl tsDumpControl = new TransportStreamDumpControl();
                TabPage tPage = new TabPage(String.Format("{0}-{1}...",i,t.Name.Substring(0,10)));
                tPage.Controls.Add(tsDumpControl);
                tsDumpControl.Dock = DockStyle.Fill;
                mainTabControl.TabPages.Add(tPage);
            
                 Cursor.Current = Cursors.WaitCursor;
                 tsDumpControl.Process(t);
                 Cursor.Current = Cursors.Arrow;

            }


                
            //this.Text = tsDumpControl.Heading;
            //tsDumpControl.Tag = new ControlStatus(this.Text);

           

            Logger.Instance.Write("TSDumper is loaded");
        }
        private void positionControl(Control control)
        {
            control.Parent = this;
            control.Location = new System.Drawing.Point(0,0);
            control.Height = this.ClientSize.Height;
            control.Width = this.ClientSize.Width;
        }
        /// <summary>
        /// Get the full assembly version number.
        /// </summary>
        public static string AssemblyVersion
        {
            get
            {
                Version version = Assembly.GetExecutingAssembly().GetName().Version;
                return (version.Major + "." + version.Minor + "." + version.Build + "." + version.Revision);
            }
        }

     
        
        

       
       

       
        private void MainWindow_FormClosed(object sender, FormClosedEventArgs e)
        {
            //System.Runtime.
            Application.Exit();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            //

            var ls = (string[])Logger.last_log.ToArray(typeof(string));

            if (ls.Length > 0)
            {
                last_log.Items.AddRange(ls);


                last_log.SelectedIndex = last_log.Items.Count - 1;

                Logger.last_log.Clear();
            }
        }
    }
}