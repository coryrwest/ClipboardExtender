using System.Diagnostics;
using MouseKeyboardActivityMonitor;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MouseKeyboardActivityMonitor.WinApi;
using System.Runtime.InteropServices;

namespace ClipboardExtender
{
    class TrayApplicationContext : ApplicationContext
    {
        private KeyboardHookListener _keyboardHookListener;

        NotifyIcon _notifyIcon;
        System.ComponentModel.IContainer components;
        const string DefaultTooltip = "ClipboardExtender";
        public ClipboardMonitor clipboardMonitor;

        public TrayApplicationContext()
        {
            InitializeContext();
        }

        public void ActivateHook()
        {
            _keyboardHookListener = new KeyboardHookListener(new GlobalHooker());
            _keyboardHookListener.Enabled = true;
            _keyboardHookListener.KeyDown += _keyboardHookListener_KeyUp;
        }

        void _keyboardHookListener_KeyUp(object sender, KeyEventArgs e)
        {
            Console.WriteLine(e.Modifiers);

            if ((e.Control && e.Shift) && e.KeyCode == Keys.X)
            {
                Console.Beep();
            }
        }

        private void InitializeContext()
        {
            components = new Container();
            _notifyIcon = new NotifyIcon(components)
            {
                ContextMenuStrip = new ContextMenuStrip(),
                Icon = TrayIcon.route,
                Text = DefaultTooltip,
                Visible = true
            };

            _notifyIcon.ContextMenuStrip.Items.Add(ToolStripMenuItemWithHandler("Settings", settings_click));
            _notifyIcon.ContextMenuStrip.Items.Add(new ToolStripSeparator());
            _notifyIcon.ContextMenuStrip.Items.Add(ToolStripMenuItemWithHandler("Quit", quit_Click));
            _notifyIcon.DoubleClick += notifyIcon_DoubleClick;
            _notifyIcon.MouseUp += notifyIcon_MouseUp;
        }

        private void ShowClipboardMonitor()
        {
            if (clipboardMonitor == null)
            {
                clipboardMonitor = new ClipboardMonitor();
                clipboardMonitor.Show();
            }
            else
            {
                clipboardMonitor.Show();
            }
        }

        #region Event Handlers

        private void settings_click(object sender, EventArgs e)
        {
            ShowClipboardMonitor();
        }

        private void notifyIcon_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                MethodInfo mi = typeof(NotifyIcon).GetMethod("ShowContextMenu", BindingFlags.Instance | BindingFlags.NonPublic);
                mi.Invoke(_notifyIcon, null);
            }
        }

        private void notifyIcon_DoubleClick(object sender, EventArgs e)
        {
            ShowClipboardMonitor();
        }

        private void quit_Click(object sender, EventArgs e)
        {
            ExitThread();
        }
        #endregion

        #region Helpers and Overrides

        protected override void Dispose(bool disposing)
        {
            if (disposing && components != null)
                components.Dispose();
        }

        protected override void ExitThreadCore()
        {
            if (clipboardMonitor != null)
                clipboardMonitor.Close();

            _notifyIcon.Visible = false;
            base.ExitThreadCore();
        }

        public ToolStripMenuItem ToolStripMenuItemWithHandler(string displayText, EventHandler eventHandler)
        {
            var item = new ToolStripMenuItem(displayText);
            if (eventHandler != null) { item.Click += eventHandler; }
            return item;
        }
        #endregion
    }
}
