using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Data;
using System.Text;
using System.Runtime.InteropServices;
using System.IO;
using System.Windows.Automation;

namespace GlobalHooksTest
{
	public class Form1 : System.Windows.Forms.Form
	{
		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.Button BtnInitCbt;
		private System.Windows.Forms.Button BtnUninitCbt;
		private System.Windows.Forms.GroupBox groupBox2;
		private System.Windows.Forms.Button BtnUninitShell;
        private System.Windows.Forms.Button BtnInitShell;
        private System.Windows.Forms.ListBox ListShell;
		private System.ComponentModel.Container components = null;
        private TextBox textBox1;
        private TextBox textBoxMessages;
        private Button button1;

        private const int WM_INITMENU =         0x0116;
        private const int WM_MENUSELECT =       0x011F;
        private const int WM_INITMENUPOPUP =    0x0117;
        private const int WM_COMMAND =          0x0111;
        private const int WM_SYSCOMMAND =       0x0112;
        private const int WM_PAINT =            0x000F;
        private const int WM_DRAWITEM =         0x002B;
        private const int WM_WINDOWPOSCHANGED = 0x0047;
        private const int WM_SHOWWINDOW =       0x0018;
        private const int CBN_DROPDOWN =        7;
        private const int CBN_CLOSEUP =         8;
        private const int CBN_EDITCHANGE =      5;
        private const int CB_SHOWDROPDOWN =     0x014F;
        //private const int WM_COMMAND =          0x0111;
		// API calls to give us a bit more information about the data we get from the hook
		

        private ElementManage elemManage;
		private GlobalHooks _GlobalHooks;
        private static string strPath;
        private static string filePath;
        private static StringBuilder log;
        //private bool comboBoxFlag = true;

		public Form1()
		{
			InitializeComponent();

			// Instantiate our GlobalHooks object
			_GlobalHooks = new GlobalHooks(this.Handle);

			// Set the hook events
			_GlobalHooks.CBT.Activate += new GlobalHooksTest.GlobalHooks.WindowEventHandler(_GlobalHooks_CbtActivate);
			_GlobalHooks.CBT.CreateWindow += new GlobalHooksTest.GlobalHooks.WindowEventHandler(_GlobalHooks_CbtCreateWindow);
			_GlobalHooks.CBT.DestroyWindow += new GlobalHooksTest.GlobalHooks.WindowEventHandler(_GlobalHooks_CbtDestroyWindow);
            _GlobalHooks.CBT.MoveSize += new GlobalHooksTest.GlobalHooks.WindowEventHandler(_GlobalHooks_CbtMoveSize);
            _GlobalHooks.CBT.SetFocus += new GlobalHooksTest.GlobalHooks.WindowEventHandler(_GlobalHooks_CbtSetFocus);
            _GlobalHooks.CBT.SysCommand += new GlobalHooksTest.GlobalHooks.SysCommandEventHandler(_GlobalHooks_SysCommand);
			//_GlobalHooks.Shell.WindowActivated += new GlobalHooksTest.GlobalHooks.WindowEventHandler(_GlobalHooks_ShellWindowActivated);
			_GlobalHooks.Shell.WindowCreated += new GlobalHooksTest.GlobalHooks.WindowEventHandler(_GlobalHooks_ShellWindowCreated);
			//_GlobalHooks.Shell.WindowDestroyed += new GlobalHooksTest.GlobalHooks.WindowEventHandler(_GlobalHooks_ShellWindowDestroyed);
			//_GlobalHooks.Shell.Redraw += new GlobalHooksTest.GlobalHooks.WindowEventHandler(_GlobalHooks_ShellRedraw);
			_GlobalHooks.MouseLL.MouseMove += new MouseEventHandler(MouseLL_MouseMove);
            _GlobalHooks.MouseLL.MouseDown += new MouseEventHandler(MouseLL_MouseDown);
            _GlobalHooks.MouseLL.MouseUp += new MouseEventHandler(MouseLL_MouseUp);
            _GlobalHooks.KeyboardLL.KeyDown += new KeyEventHandler(KeyboardLL_KeyDown);
            _GlobalHooks.KeyboardLL.KeyUp += new KeyEventHandler(KeyboardLL_KeyUp);
            _GlobalHooks.CallWndProc.CallWndProc += new GlobalHooksTest.GlobalHooks.WndProcEventHandler(_GlobalHooks_CallWndProc);
            _GlobalHooks.SysMsgFilter.Menu += new GlobalHooksTest.GlobalHooks.WindowEventHandler(_GlobalHooks_SysMsgFilterMenu);
            _GlobalHooks.SysMsgFilter.DialogBox += new GlobalHooksTest.GlobalHooks.WindowEventHandler(_GlobalHooks_SysMsgFilterDialogBox);
            _GlobalHooks.GetMsg.GetMsg += new GlobalHooksTest.GlobalHooks.WndProcEventHandler(_GlobalHooks_GetMsg);

            elemManage = new ElementManage();
            log = new StringBuilder();
            filePath = "E:\\GitHub\\AutoUITest\\log.txt";

            AutomationEventHandler eventHandler = new AutomationEventHandler(elemManage.OnWindowOpenOrClose);

            Automation.AddAutomationEventHandler(WindowPattern.WindowOpenedEvent, AutomationElement.RootElement, TreeScope.Children, eventHandler);
		}

		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				// Make sure we stop hooking before quitting! Otherwise, the hook procedures in GlobalCbtHook.dll
				// will keep being called, even after our application quits, which will needlessly degrade system
				// performance. And it's just plain sloppy.
				_GlobalHooks.CBT.Stop();
				_GlobalHooks.Shell.Stop();
				_GlobalHooks.MouseLL.Stop();
                _GlobalHooks.KeyboardLL.Stop();
                //_GlobalHooks.SysMsgFilter.Stop();
                //_GlobalHooks.CallWndProc.Stop();
                //_GlobalHooks.GetMsg.Stop();

                if (File.Exists(filePath))
                {
                    using (StreamWriter sw = File.CreateText(filePath))
                    {
                        sw.Write(log.ToString());
                    }
                }

				if (components != null) 
				{
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}

		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.textBoxMessages = new System.Windows.Forms.TextBox();
            this.BtnUninitCbt = new System.Windows.Forms.Button();
            this.BtnInitCbt = new System.Windows.Forms.Button();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.ListShell = new System.Windows.Forms.ListBox();
            this.BtnUninitShell = new System.Windows.Forms.Button();
            this.BtnInitShell = new System.Windows.Forms.Button();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.button1 = new System.Windows.Forms.Button();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.textBoxMessages);
            this.groupBox1.Controls.Add(this.BtnUninitCbt);
            this.groupBox1.Controls.Add(this.BtnInitCbt);
            this.groupBox1.Location = new System.Drawing.Point(4, 60);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(424, 311);
            this.groupBox1.TabIndex = 2;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "CBT Hooks";
            // 
            // textBoxMessages
            // 
            this.textBoxMessages.Location = new System.Drawing.Point(11, 56);
            this.textBoxMessages.Multiline = true;
            this.textBoxMessages.Name = "textBoxMessages";
            this.textBoxMessages.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.textBoxMessages.Size = new System.Drawing.Size(407, 238);
            this.textBoxMessages.TabIndex = 3;
            // 
            // BtnUninitCbt
            // 
            this.BtnUninitCbt.Location = new System.Drawing.Point(144, 16);
            this.BtnUninitCbt.Name = "BtnUninitCbt";
            this.BtnUninitCbt.Size = new System.Drawing.Size(128, 32);
            this.BtnUninitCbt.TabIndex = 2;
            this.BtnUninitCbt.Text = "Uninitialize CBT Hooks";
            this.BtnUninitCbt.Click += new System.EventHandler(this.BtnUninitCbt_Click);
            // 
            // BtnInitCbt
            // 
            this.BtnInitCbt.Location = new System.Drawing.Point(8, 16);
            this.BtnInitCbt.Name = "BtnInitCbt";
            this.BtnInitCbt.Size = new System.Drawing.Size(128, 32);
            this.BtnInitCbt.TabIndex = 1;
            this.BtnInitCbt.Text = "Initialize CBT Hooks";
            this.BtnInitCbt.Click += new System.EventHandler(this.BtnInitCbt_Click);
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.ListShell);
            this.groupBox2.Controls.Add(this.BtnUninitShell);
            this.groupBox2.Controls.Add(this.BtnInitShell);
            this.groupBox2.Location = new System.Drawing.Point(434, 60);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(235, 311);
            this.groupBox2.TabIndex = 3;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Shell Hooks";
            // 
            // ListShell
            // 
            this.ListShell.Location = new System.Drawing.Point(12, 56);
            this.ListShell.Name = "ListShell";
            this.ListShell.Size = new System.Drawing.Size(216, 238);
            this.ListShell.TabIndex = 3;
            // 
            // BtnUninitShell
            // 
            this.BtnUninitShell.Location = new System.Drawing.Point(12, 19);
            this.BtnUninitShell.Name = "BtnUninitShell";
            this.BtnUninitShell.Size = new System.Drawing.Size(97, 32);
            this.BtnUninitShell.TabIndex = 2;
            this.BtnUninitShell.Text = "Uninitialize Shell Hooks";
            this.BtnUninitShell.Click += new System.EventHandler(this.BtnUninitShell_Click);
            // 
            // BtnInitShell
            // 
            this.BtnInitShell.Location = new System.Drawing.Point(129, 19);
            this.BtnInitShell.Name = "BtnInitShell";
            this.BtnInitShell.Size = new System.Drawing.Size(99, 32);
            this.BtnInitShell.TabIndex = 1;
            this.BtnInitShell.Text = "Initialize Shell Hooks";
            this.BtnInitShell.Click += new System.EventHandler(this.BtnInitShell_Click);
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(30, 13);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(453, 20);
            this.textBox1.TabIndex = 5;
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(497, 10);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 6;
            this.button1.Text = "Open";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // Form1
            // 
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.ClientSize = new System.Drawing.Size(674, 390);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.textBox1);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Name = "Form1";
            this.Text = "Global Hooks Test";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

		}
		#endregion

		[STAThread]
		static void Main() 
		{
			Application.Run(new Form1());
		}

		private void BtnInitCbt_Click(object sender, System.EventArgs e)
		{
            _GlobalHooks.Shell.Start();
            AddText("Start: \"" + strPath + "\"");
            elemManage.StartProcess(strPath);
            
           
			_GlobalHooks.CBT.Start();
            //AddText("CBT hook Adding");
            _GlobalHooks.MouseLL.Start();
            //AddText("MouseLL hook Adding");
            _GlobalHooks.KeyboardLL.Start();

            //_GlobalHooks.SysMsgFilter.Start();

            //_GlobalHooks.CallWndProc.Start();

           // _GlobalHooks.GetMsg.Start();

		}

		private void BtnUninitCbt_Click(object sender, System.EventArgs e)
		{
			_GlobalHooks.CBT.Stop();
            //AddText("CBT hook Remove");
            _GlobalHooks.MouseLL.Stop();
            //AddText("MouseLL hook Remove");
            _GlobalHooks.KeyboardLL.Stop();

            //_GlobalHooks.SysMsgFilter.Stop();

            //_GlobalHooks.CallWndProc.Stop();

            _GlobalHooks.Shell.Stop();

            //_GlobalHooks.GetMsg.Stop();

            AddText("Stop:");

            if (File.Exists(filePath))
            {
                using (StreamWriter sw = File.CreateText(filePath))
                {
                    sw.Write(log.ToString());
                }
            }
		}

		private void BtnInitShell_Click(object sender, System.EventArgs e)
		{
			_GlobalHooks.Shell.Start();
		}

		private void BtnUninitShell_Click(object sender, System.EventArgs e)
		{
			_GlobalHooks.Shell.Stop();
		}

		protected override void WndProc(ref Message m)
		{
			// Check to see if we've received any Windows messages telling us about our hooks
			if (_GlobalHooks != null)
				_GlobalHooks.ProcessWindowMessage(ref m);

			base.WndProc (ref m);
		}

		private void _GlobalHooks_CbtActivate(IntPtr Handle)
		{
			//this.ListCbt.Items.Add("Activate: " + GetWindowName(Handle));
           
            //string name = elemManage.GetElementInfo(elemManage.GetElementFromHandle(Handle));
            string name = elemManage.GetNameFromHandle(Handle);
            if (name == null || name == "" || name == " ")
            {
                //string tmp = elemManage.GetWindowName(Handle);
                //if (tmp == "" || tmp == null)
                //{
                //    return;
                //}
                //AddText("Activate: \"" + elemManage.GetWindowName(Handle) + "\"");
                return;
            }
            AddText("Activate: " + name);
            //AddText("Activate: " + GetWindowName(Handle));
		}

		private void _GlobalHooks_CbtCreateWindow(IntPtr Handle)
		{
            //string name = elemManage.GetNameFromHandle(Handle);
            //if (name == null || name == "" || name == " ")
            //{
            //    string tmp = elemManage.GetWindowName(Handle);
            //    if (tmp==""||tmp==null)
            //    {
            //        return;
            //    }
            //    //AddText("Create: " + elemManage.GetWindowName(Handle));
            //    return;
            //}
           // AddText("Create: \"" + name+"\"");
            
			//this.ListCbt.Items.Add("Create: " + GetWindowClass(Handle));
		}

		private void _GlobalHooks_CbtDestroyWindow(IntPtr Handle)
		{
		//	this.ListCbt.Items.Add("Destroy: " + GetWindowName(Handle));
		}

        private void _GlobalHooks_CbtMoveSize(IntPtr Handle)
        {
            string name = elemManage.GetWindowName(Handle);
            elemManage.UpdateCache();
            if (name==null||name==""||name==" ")
            {
                return;
            }
            AddText("MoveSize: \"" + name+"\"");
        }

        private void _GlobalHooks_CbtSetFocus(IntPtr Handle)
        {
            //string name = elemManage.GetNameFromHandle(Handle);

            //if (name == null || name == "" || name == " ")
            //{
            //    AddText("SetFocus: \"" + elemManage.GetWindowName(Handle)+"\"");
            //    return;
            //}
            //AddText("SetFocus: \"" + name+"\"");
        }

        private void _GlobalHooks_SysCommand(int SysCommand, int lParam)
        {
            //AddText("SysCommand: " + SysCommand + " " + lParam);
        }

		private void _GlobalHooks_ShellWindowActivated(IntPtr Handle)
		{
			this.ListShell.Items.Add("Activated: " + elemManage.GetWindowName(Handle));
		}

		private void _GlobalHooks_ShellWindowCreated(IntPtr Handle)
		{
			//this.ListShell.Items.Add("Created: " + elemManage.GetWindowName(Handle));
            AddText("Shell Create: " + elemManage.GetWindowName(Handle));
            if (elemManage.hasWindowsProcess(Handle))
            {

            }
		}

		private void _GlobalHooks_ShellWindowDestroyed(IntPtr Handle)
		{
			this.ListShell.Items.Add("Destroyed: " + elemManage.GetWindowName(Handle));
		}

		private void _GlobalHooks_ShellRedraw(IntPtr Handle)
		{
			this.ListShell.Items.Add("Redraw: " + elemManage.GetWindowName(Handle)+"   "+Handle);
            //if (elemManage.hasWindowsProcess(Handle))
            {

            }
		}

		private void MouseLL_MouseMove(object sender, MouseEventArgs e)
		{
			//this.LblMouse.Text = "Mouse at: " + e.X + ", " + e.Y;
		}

        private void MouseLL_MouseDown(object sender, MouseEventArgs e)
        {
            string message = elemManage.GetElementFromPoint(new Point(e.X, e.Y));
            if (message == null)
            {
                return;
            }
            // string msg = string.Format("Mouse event: {0}-->{1}: ({2},{3}).,{4}", mEvent.ToString(), name, point.X, point.Y, hwnd)
            //this.ListCbt.Items.Add("MouseDown:"+ message + e.X + "," + e.Y);
            string msg = string.Format("{0}MouseDown: {1} {2}", e.Button.ToString(), message, e.Delta);
            AddText(msg);
            
        }

        private void MouseLL_MouseUp(object sender, MouseEventArgs e)
        {
    
            string message = elemManage.GetElementFromPoint(new Point(e.X, e.Y));
            if (message == null)
            {
                return;
            }
            // string msg = string.Format("Mouse event: {0}-->{1}: ({2},{3}).,{4}", mEvent.ToString(), name, point.X, point.Y, hwnd)
            //this.ListCbt.Items.Add("MouseDown:"+ message + e.X + "," + e.Y);
            string msg = string.Format("{0}MouseUp: {1} {2}", e.Button.ToString(), message,e.Delta);
            AddText(msg);
         
        }

        private void KeyboardLL_KeyDown(object sender, KeyEventArgs e)
        {
            string msg = string.Format("KeyDown: \"{0}\"",e.KeyData);
            AddText(msg);
        }

        private void KeyboardLL_KeyUp(object sender, KeyEventArgs e)
        {
            string msg = string.Format("KeyUp: \"{0}\"", e.KeyData);
            AddText(msg);
        }

        private void _GlobalHooks_CallWndProc(IntPtr Handle, IntPtr Message, IntPtr wParam, IntPtr lParam)
        {
            if ((int)Message == WM_MENUSELECT)
            {
                //string name = elemManage.GetNameFromHandle(lParam);
                //uint param = (uint)wParam.ToInt32();
                //uint loword = param & 0x0000ffff;
                //uint hiword = (param & 0xffff0000)>>16;
                //name = elemManage.GetNameFromHandle((IntPtr)hiword);
                //if (name == null || name == "" || name == " ")
                //{

                //    string tmp = elemManage.GetWindowName((IntPtr)hiword);
                //    if (tmp == "" || tmp == null)
                //    {
                //        //return;
                //    }
                //    AddText("SelectMenu: " + tmp + "---" + loword + "---" + hiword);
                //    // AddText("Menu: \"" + GetWindowName(Handle) + "\"");
                //    return;
                //}
                ////AddText("Menu: \"" + name + "\"");
                //AddText("SelectMenu: "+name+" "+wParam+" "+lParam);
            }
            else if ((int)Message == WM_INITMENUPOPUP)
            {
                //string name = elemManage.GetNameFromHandle(lParam);
                //uint param = (uint)wParam.ToInt32();
                //uint loword = param & 0x0000ffff;
                //uint hiword = (param & 0xffff0000) >> 16;
                //name = elemManage.GetNameFromHandle((IntPtr)hiword);
                //if (name == null || name == "" || name == " ")
                //{

                //    string tmp = elemManage.GetWindowName((IntPtr)hiword);
                //    if (tmp == "" || tmp == null)
                //    {
                //        //return;
                //    }
                //    AddText("MenuPopup: " + tmp + "---" + loword + "---" + hiword);
                //    // AddText("Menu: \"" + GetWindowName(Handle) + "\"");
                //    return;
                //}
                ////AddText("Menu: \"" + name + "\"");
                //AddText("MenuPopup: " + name + " " + wParam + " " + lParam);
            }
            else if (Message.ToInt32() == CB_SHOWDROPDOWN)
            {
                AddText("show drop down");
            }
            else if (Message.ToInt32() == WM_COMMAND)
            {
                //uint param = (uint)wParam.ToInt32();
                //uint loword = param & 0x0000ffff;
                //uint hiword = (param & 0xffff0000) >> 16;
                //if (hiword == CBN_DROPDOWN)
                //{
                //    //comboBoxFlag = false;
                //    //AddText("WndCommand:"+elemManage.GetWindowName(Handle));
                //}
                //else if (hiword ==CBN_CLOSEUP)
                //{
                //    string value = elemManage.GetSelectValue(lParam);
                //    //comboBoxFlag = true;
                //    AddText(value);
                    
                //}
                //else if (hiword == CBN_EDITCHANGE)
                //{
                //    AddText("Change Text:" + loword + "  " + lParam);
                //}
            }
            else if (Message.ToInt32() == WM_SHOWWINDOW)
            {
                //if (wParam == IntPtr.Zero)
                //{
                //    //AddText("hidden windows: " + elemManage.GetWindowName(Handle));
                //}
                //else
                //{
                //    //AddText("show windows: " + elemManage.GetWindowName(Handle));
                //}
                
            }
        }

        private void _GlobalHooks_SysMsgFilterMenu(IntPtr Handle)
        {
            //string name = elemManage.GetNameFromHandle(Handle);
            //if (name == null || name == "" || name == " ")
            //{
            //    string tmp = elemManage.GetWindowName(Handle);
            //    if (tmp == "" || tmp == null)
            //    {
            //        return;
            //    }
            //   // AddText("Menu: \"" + GetWindowName(Handle) + "\"");
            //    return;
            //}
            //AddText("Menu: \"" + name + "\"");
        }

        private void _GlobalHooks_SysMsgFilterDialogBox(IntPtr Handle)
        {
            //string name = elemManage.GetNameFromHandle(Handle);
            //if (name == null || name == "" || name == " ")
            //{
            //    string tmp = elemManage.GetWindowName(Handle);
            //    if (tmp == "" || tmp == null)
            //    {
            //        return;
            //    }
            //     AddText("DialogBox: \"" + elemManage.GetWindowName(Handle) + "\"");
            //    return;
            //}
            //AddText("DialogBox: \"" + elemManage.GetWindowName(Handle) + "\"");
        }

        private void _GlobalHooks_GetMsg(IntPtr Handle, IntPtr Message, IntPtr wParam, IntPtr lParam)
        {
            if (Message.ToInt32() == WM_PAINT)
            {
                //if (elemManage.isEnable(Handle))
                //{
                //    //string name = elemManage.GetNameFromHandle(Handle);

                //    //AddText("Paint: " + name);
                //}
                //string name = elemManage.GetWindowName(Handle);

                //AddText("Paint: " + name);
            }
            else if (Message.ToInt32() == WM_DRAWITEM)
            {
                string name = elemManage.GetWindowName(Handle);
                AddText("Paint: " + name);
            }
            else if (Message.ToInt32() == WM_WINDOWPOSCHANGED)
            {
                string name = elemManage.GetWindowName(Handle);
                AddText("Paint: " + name);
            }
            else if (Message.ToInt32() == WM_SHOWWINDOW)
            {
                string name = elemManage.GetWindowName(Handle);
                AddText("Show Windows: " + name);
            }
            else if (Message.ToInt32() == CBN_DROPDOWN)
            {
                AddText("Dropdown");
            }
            else if (Message.ToInt32() == CB_SHOWDROPDOWN)
            {
                AddText("show drop down");
            }
            else if (Message.ToInt32() == WM_COMMAND)
            {
                AddText("Command:");
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "��ִ���ļ�|*.exe*|�����ļ�|*.*";
            openFileDialog.RestoreDirectory = true;
            openFileDialog.FilterIndex = 1;
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                this.textBox1.Text = openFileDialog.FileName;
                strPath = openFileDialog.FileName;
            }
        }

        private void AddText(string message)
        {
            //this.ListCbt.Items.Add(message);
            if (message == null)
            {
                return;
            }
            int length = textBoxMessages.Text.Length + message.Length;
            if (length >= textBoxMessages.MaxLength)
            {
                textBoxMessages.Text = "";
            }

            if (!message.EndsWith("\r\n"))
            {
                message += "\r\n";
            }
            textBoxMessages.Text = message + textBoxMessages.Text;
            log.Append(message);
        }

        private StringBuilder AnalysisStr()
        {
            string buf = log.ToString();
            bool first = true;
            string cmd;
            string elem;
            string time;

            StringBuilder builder = new StringBuilder();

            string[] strLines = buf.Split(new char[] { '\n' });
            for (int i = 0; i < strLines.Length;i++ )
            {
                string[] args = strLines[i].Split(new char[] { ' ' });
                if (first)
                {
                    first = false;


                }

            }

            return builder;
        }
	}
}