﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using fireBwall.Logging;
using fireBwall.UI;
using fireBwall.Configuration;
using fireBwall.Filters.NDIS;

namespace fireBwall.UI.Tabs
{
    public partial class AdapterControl : DynamicUserControl
    {
        Thread t;

        bool timing = true;

        void Timing()
        {
            try
            {
                while (timing)
                {
                    Thread.Sleep(1000);
                    UpdateAdapterList();
                }
            }
            catch (ArgumentOutOfRangeException e)
            {
                LogCenter.Instance.LogException(e);
            }
        }

        public void Kill()
        {
            timing = false;
            t.Abort();
        }

        public AdapterControl() : base()
        {
            InitializeComponent();
            UpdateAdapterList();
            t = new Thread(new ThreadStart(Timing));
            t.Name = "AdapterControl Adapter Update Thread";
            t.Start();
            flowLayoutPanel1.SizeChanged += new EventHandler(flowLayoutPanel1_SizeChanged);
            Program.OnShutdown += Kill;
        }

        void flowLayoutPanel1_SizeChanged(object sender, EventArgs e)
        {
            foreach (Control c in flowLayoutPanel1.Controls)
                c.Width = flowLayoutPanel1.Width - 5;
        }

        public void UpdateAdapterList()
        {
            if (this.Parent == null)
            {
                return;
            }
            if (flowLayoutPanel1.InvokeRequired)
            {
                ThreadStart ts = new ThreadStart(UpdateAdapterList);
                flowLayoutPanel1.Invoke(ts);
            }
            else
            {
                if (flowLayoutPanel1.Controls.Count == 0)
                {
                    foreach (INDISFilter na in ProcessingConfiguration.Instance.NDISFilterList.GetAllAdapters())
                    {
                        AdapterDisplay ad = new AdapterDisplay(na.GetAdapterInformation());
                        ad.Width = flowLayoutPanel1.Width - 5;
                        flowLayoutPanel1.Controls.Add(ad);
                    }
                }
                else
                {
                    foreach (AdapterDisplay ad in flowLayoutPanel1.Controls)
                    {
                        ad.UpdateText();
                    }
                    foreach (INDISFilter na in ProcessingConfiguration.Instance.NDISFilterList.GetNewAdapters())
                    {
                        AdapterDisplay ad = new AdapterDisplay(na.GetAdapterInformation());
                        ThemeConfiguration.Instance.SetColorScheme(ad);
                        ad.Width = flowLayoutPanel1.Width - 5;
                        flowLayoutPanel1.Controls.Add(ad);
                    }
                }
            }
        }
    }
}
