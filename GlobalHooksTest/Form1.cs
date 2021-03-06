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
using System.Threading;
using Microsoft.Win32;

namespace GlobalHooksTest
{
	public class Form1 : System.Windows.Forms.Form
	{
		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.Button BtnInitCbt;
        private System.Windows.Forms.Button BtnUninitCbt;
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
        //private CompareForm compareForm;
		private  static GlobalHooks _GlobalHooks;
        //private static bool _GlobalFlag;
        //private delegate void DialogCloseListener();
        //private event DialogCloseListener DialogCloseEvent;
        private static string strPath;
        private Label label1;
        private Label label2;
        private TextBox textBox2;

        private bool mouseDownFlag = false;
        private bool drawFlag = false;
        private bool hookFlag = false;
        //private bool moveFristFlag = true;
        private Thread workerThread;
        private GroupBox groupBox2;
        private Label label3;
        private Button button2;

        //CompareForm dialogFrom;
        //private AutomationEventHandler UIAEventHandler;
        //private AutomationFocusChangedEventHandler focusHandler = null;
        //private bool comboBoxFlag = true;
        //private bool stopFlag = true;

		public Form1()
		{
			InitializeComponent();
            Control.CheckForIllegalCrossThreadCalls = false;
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
            _GlobalHooks.MouseLL.MouseDoubleClick += new MouseEventHandler(MouseLL_MouseDClick);
            _GlobalHooks.KeyboardLL.KeyDown += new KeyEventHandler(KeyboardLL_KeyDown);
            _GlobalHooks.KeyboardLL.KeyUp += new KeyEventHandler(KeyboardLL_KeyUp);
            _GlobalHooks.CallWndProc.CallWndProc += new GlobalHooksTest.GlobalHooks.WndProcEventHandler(_GlobalHooks_CallWndProc);
            _GlobalHooks.SysMsgFilter.Menu += new GlobalHooksTest.GlobalHooks.WindowEventHandler(_GlobalHooks_SysMsgFilterMenu);
            _GlobalHooks.SysMsgFilter.DialogBox += new GlobalHooksTest.GlobalHooks.WindowEventHandler(_GlobalHooks_SysMsgFilterDialogBox);
            _GlobalHooks.GetMsg.GetMsg += new GlobalHooksTest.GlobalHooks.WndProcEventHandler(_GlobalHooks_GetMsg);

            elemManage = new ElementManage();
            //compareForm = new CompareForm();
            elemManage.SendMessageBack += new ElementManage.CallBackMessage(_GetMessage);
            //compareForm.SendMessageBack += new CompareForm.CallBackMessage(_GetMessageFrom);
            //filePath = elemManage.GetUserPath();
            this.textBox2.Text = elemManage.GetUserPath();

            //
            //DialogCloseEvent += new DialogCloseListener(StartHook);
            //AutomationEventHandler eventHandler = new AutomationEventHandler(elemManage.OnWindowOpenOrClose);
            //_GlobalFlag = false;
            //Automation.AddAutomationEventHandler(WindowPattern.WindowOpenedEvent, AutomationElement.RootElement, TreeScope.Children, eventHandler);
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
                
                elemManage.WriteToFile(textBox2.Text);
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
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.button1 = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.textBox2 = new System.Windows.Forms.TextBox();
            this.button2 = new System.Windows.Forms.Button();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.label3 = new System.Windows.Forms.Label();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.textBoxMessages);
            this.groupBox1.Location = new System.Drawing.Point(8, 114);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(573, 266);
            this.groupBox1.TabIndex = 2;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Output";
            // 
            // textBoxMessages
            // 
            this.textBoxMessages.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.textBoxMessages.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.textBoxMessages.ForeColor = System.Drawing.SystemColors.WindowText;
            this.textBoxMessages.HideSelection = false;
            this.textBoxMessages.Location = new System.Drawing.Point(8, 19);
            this.textBoxMessages.Multiline = true;
            this.textBoxMessages.Name = "textBoxMessages";
            this.textBoxMessages.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.textBoxMessages.Size = new System.Drawing.Size(556, 238);
            this.textBoxMessages.TabIndex = 100;
            this.textBoxMessages.TabStop = false;
            // 
            // BtnUninitCbt
            // 
            this.BtnUninitCbt.Enabled = false;
            this.BtnUninitCbt.Location = new System.Drawing.Point(154, 76);
            this.BtnUninitCbt.Name = "BtnUninitCbt";
            this.BtnUninitCbt.Size = new System.Drawing.Size(113, 32);
            this.BtnUninitCbt.TabIndex = 2;
            this.BtnUninitCbt.Text = "Stop Record";
            this.BtnUninitCbt.Click += new System.EventHandler(this.BtnUninitCbt_Click);
            // 
            // BtnInitCbt
            // 
            this.BtnInitCbt.Location = new System.Drawing.Point(15, 76);
            this.BtnInitCbt.Name = "BtnInitCbt";
            this.BtnInitCbt.Size = new System.Drawing.Size(113, 32);
            this.BtnInitCbt.TabIndex = 1;
            this.BtnInitCbt.Text = "Start Record";
            this.BtnInitCbt.Click += new System.EventHandler(this.BtnInitCbt_Click);
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(119, 13);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(364, 20);
            this.textBox1.TabIndex = 9;
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
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(27, 49);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(82, 13);
            this.label1.TabIndex = 7;
            this.label1.Text = "输出脚本文件:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(27, 15);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(82, 13);
            this.label2.TabIndex = 8;
            this.label2.Text = "应用程序路径:";
            // 
            // textBox2
            // 
            this.textBox2.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.FileSystem;
            this.textBox2.Location = new System.Drawing.Point(119, 47);
            this.textBox2.Name = "textBox2";
            this.textBox2.Size = new System.Drawing.Size(364, 20);
            this.textBox2.TabIndex = 5;
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(497, 44);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(75, 23);
            this.button2.TabIndex = 10;
            this.button2.Text = "Open";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.label3);
            this.groupBox2.Location = new System.Drawing.Point(339, 76);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(233, 32);
            this.groupBox2.TabIndex = 11;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "注意";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.ForeColor = System.Drawing.Color.Red;
            this.label3.Location = new System.Drawing.Point(41, 13);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(181, 13);
            this.label3.TabIndex = 0;
            this.label3.Text = "F2,F3,F6键被屏蔽，用于测试软件";
            this.label3.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // Form1
            // 
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.ClientSize = new System.Drawing.Size(589, 390);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.textBox2);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.BtnUninitCbt);
            this.Controls.Add(this.textBox1);
            this.Controls.Add(this.BtnInitCbt);
            this.Controls.Add(this.groupBox1);
            this.Name = "Form1";
            this.Text = "FunAuto Record";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

		}
		#endregion

		[STAThread]
		static void Main() 
		{
			Application.Run(new Form1());
            
		}
        
        private void Start()
        {
            _GlobalHooks.Shell.Start();
            _GlobalHooks.CBT.Start();
            //AddText("CBT hook Adding");
            _GlobalHooks.MouseLL.Start();
            //AddText("MouseLL hook Adding");
            _GlobalHooks.KeyboardLL.Start();
            elemManage.SetHookFlag(true);
            hookFlag = true;
            BtnInitCbt.Enabled = false;
            BtnUninitCbt.Enabled = true;
        }

        private void StartHook()
        {
            elemManage.SetHookFlag(true);
            hookFlag = true;
        }

        private void StartHookEx()
        {
            _GlobalHooks.Shell.Start();
            _GlobalHooks.CBT.Start();
            //AddText("CBT hook Adding");
            _GlobalHooks.MouseLL.Start();
            //AddText("MouseLL hook Adding");
            //_GlobalHooks.KeyboardLL.Start();
            elemManage.SetHookFlag(true);
            hookFlag = true;
            BtnInitCbt.Enabled = false;
            BtnUninitCbt.Enabled = true;
        }

        private void Stop()
        {
            _GlobalHooks.Shell.Stop();

            _GlobalHooks.CBT.Stop();
            //AddText("CBT hook Remove");
            _GlobalHooks.MouseLL.Stop();
            //AddText("MouseLL hook Remove");
            _GlobalHooks.KeyboardLL.Stop();
            elemManage.SetHookFlag(false);
            hookFlag = false;
            BtnInitCbt.Enabled = true;
            BtnUninitCbt.Enabled = false;
        }

        private void StopHook()
        {
            elemManage.SetHookFlag(false);
            hookFlag = false;
        }

        private void StopHookEx()
        {
            _GlobalHooks.Shell.Stop();

            _GlobalHooks.CBT.Stop();
            //AddText("CBT hook Remove");
            _GlobalHooks.MouseLL.Stop();
            //AddText("MouseLL hook Remove");
            //_GlobalHooks.KeyboardLL.Stop();
            elemManage.SetHookFlag(false);
            hookFlag = false;
            BtnInitCbt.Enabled = true;
            BtnUninitCbt.Enabled = false;
        }

		private void BtnInitCbt_Click(object sender, System.EventArgs e)
		{
            
            try
            {
                elemManage.StartProcess(strPath);
                AddText("Start \"" + strPath + "\"");
            }
            catch (System.Exception ex)
            {
                return;
            }
            
            Start();
            
		}

		private void BtnUninitCbt_Click(object sender, System.EventArgs e)
		{
            AddText("Stop");
            Stop();
            //stopFlag = false;
            //StringBuilder builder = elemManage.AnalysisStr();
            
            
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
            //string time = elemManage.GetTime();
            //string name = elemManage.GetElementInfo(elemManage.GetElementFromHandle(Handle));
            if (elemManage.hasWindowsProcess(Handle)&&hookFlag)
            {
//                 string name = elemManage.AddMainElementFromHandle(Handle);
//                 if (name == null || name == "\"" || name == " " || name == "")
//                 {
//                     return;
//                 }
                elemManage.ActivateWnd(Handle);
                if (elemManage.addHandler(Handle))
                {
                    ThreadStart threadDelegate = new ThreadStart(StartUiaWorkerThread);
                    workerThread = new Thread(threadDelegate);
                    workerThread.Start();
                    //AddText("Thread Start");
                }
                //AddText("Activate|" + name);
            }
            
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
//             string time = elemManage.GetTime();
//             if (elemManage.hasWindowsProcess(Handle))
//             {
//                 string name = elemManage.GetNameFromHandle(Handle);
//                 if (name == null || name == "\"" || name == " " || name == "")
//                 {
//                     return;
//                 }
//                 AddText("Destroy|" + name);
//             }
		//	this.ListCbt.Items.Add("Destroy: " + GetWindowName(Handle));
		}

        private void _GlobalHooks_CbtMoveSize(IntPtr Handle)
        {
            
            //string name = elemManage.GetWindowName(Handle);
            //elemManage.UpdateCache();
            //if (name==null||name==""||name==" ")
            //{
            //    return;
            //}
            //AddText("MoveSize: \"" + name+"\"");
        }

        private void _GlobalHooks_CbtSetFocus(IntPtr Handle)
        {
            //string name = elemManage.GetNameFromHandle(Handle);

            //if (name == null || name == "" || name == " ")
            //{
            //    AddText("SetFocus: \"" + elemManage.GetWindowName(Handle) + "\"");
            //    return;
            //}
            //AddText("SetFocus|" + name);
        }

        private void _GlobalHooks_SysCommand(int SysCommand, int lParam)
        {
            //AddText("SysCommand: " + SysCommand + " " + lParam);
        }

		private void _GlobalHooks_ShellWindowActivated(IntPtr Handle)
		{
			//this.ListShell.Items.Add("Activated: " + elemManage.GetWindowName(Handle));
		}

		private void _GlobalHooks_ShellWindowCreated(IntPtr Handle)
		{
			//this.ListShell.Items.Add("Created: " + elemManage.GetWindowName(Handle));
            //string time = elemManage.GetTime();
            if (elemManage.hasWindowsProcess(Handle) && hookFlag)
            {
                string name = elemManage.AddMainElementFromHandle(Handle);
                if (name == null || name == "\"" || name == " " || name == "")
                {
                    return;
                }
                //elemManage.ActivateWnd(Handle);
                //if (elemManage.addHandler(Handle))
                //{
                //    ThreadStart threadDelegate = new ThreadStart(StartUiaWorkerThread);
                //    workerThread = new Thread(threadDelegate);
                //    workerThread.Start();
                //    //AddText("ThreadStart:");
                //}
                AddText("WindowCreate " + name);
            }
            //if (elemManage.hasWindowsProcess(Handle))
            //{
            //    AddText("WindowCreate|\"" + elemManage.GetWindowName(Handle)+"\"");
            //}
            
		}

		private void _GlobalHooks_ShellWindowDestroyed(IntPtr Handle)
		{
			//this.ListShell.Items.Add("Destroyed: " + elemManage.GetWindowName(Handle));
		}

		private void _GlobalHooks_ShellRedraw(IntPtr Handle)
		{
			//this.ListShell.Items.Add("Redraw: " + elemManage.GetWindowName(Handle)+"   "+Handle);
            //if (elemManage.hasWindowsProcess(Handle))
            
		}

		private void MouseLL_MouseMove(object sender, MouseEventArgs e)
		{
            if (mouseDownFlag && hookFlag)
            {
                mouseDownFlag = false;
                //string action = "MouseMove";
                //elemManage.GetMouseAction(action, e);
                elemManage.MouseMoveAction(e.X,e.Y);
            }

            if (drawFlag)
            {
                elemManage.DrawElement(e.X, e.Y);
            }
            #region MyRegion
            //if (elemManage.IsEnablePoint(e.X,e.Y))
            //{
            //    string message = elemManage.GetMenuElementByPoint(e.X, e.Y);
            //    if (message == "")
            //    {
            //        return;
            //    }
            //    string msg = string.Format("MouseMove|{0}", message);
            //    AddText(msg);
            //}

            //string message = elemManage.GetElementFromPoint(new Point(e.X, e.Y));
            //if (message == null)
            //{
            //    return;
            //}
            //string hwnd = elemManage.GetNameFromHandle((IntPtr)e.Clicks);
            //// string msg = string.Format("Mouse event: {0}-->{1}: ({2},{3}).,{4}", mEvent.ToString(), name, point.X, point.Y, hwnd)
            ////this.ListCbt.Items.Add("MouseDown:"+ message + e.X + "," + e.Y);
            //string msg = string.Format("{0}MouseDown|{1}|{2}", e.Button.ToString(), message, e.Delta);
            //AddText(msg);
            //this.LblMouse.Text = "Mouse at: " + e.X + ", " + e.Y; 
            #endregion
		}

        private void MouseLL_MouseDown(object sender, MouseEventArgs e)
        {
            if (hookFlag)
            {
                mouseDownFlag = true;
                //string action = e.Button.ToString() + "MouseDown";
                //elemManage.GetMouseAction(action, e);
                if ("Left" == e.Button.ToString())
                {
                    elemManage.LeftMouseDownAction(e.X, e.Y);
                }
                else if ("Right" == e.Button.ToString())
                {
                    elemManage.RightMouseDownAction(e.X, e.Y);
                }
            }
            
        }

        private void MouseLL_MouseUp(object sender, MouseEventArgs e)
        {
            if (hookFlag)
            {
                mouseDownFlag = false;
                //moveFristFlag = true;
                //string action = e.Button.ToString() + "MouseUp";
                //elemManage.GetMouseAction(action, e);
                if ("Left" == e.Button.ToString())
                {
                    elemManage.LeftMouseUpAction(e.X, e.Y);
                }
                else if ("Right" == e.Button.ToString())
                {
                    elemManage.RightMouseUpAction(e.X, e.Y);
                }
            }

        }

        private void MouseLL_MouseDClick(object sender, MouseEventArgs e)
        {
            AddText("Double Click");
        }

        private void KeyboardLL_KeyDown(object sender, KeyEventArgs e)
        {
            //string time = elemManage.GetTime();
            
            Keys key = e.KeyData;

            if (key == Keys.F3&&hookFlag)
            {
                drawFlag = true;
            }
            else if (elemManage.isTopWindow()&&key == Keys.F2)
            {
                if (hookFlag)
                {
                    StopHookEx();
                }
                else
                {
                    StartHookEx();
                }
            }
            else if (elemManage.isTopWindow()&&key == Keys.F6 && hookFlag)
            {
                    
                ThreadStart threadDelegate = new ThreadStart(OpenCompareDialog);
                workerThread = new Thread(threadDelegate);
                workerThread.Start();
                //workerThread.Join();
                    
            }
            else 
            {
                if (elemManage.isTopWindow()&&hookFlag)
                {
                    elemManage.KeyDownAction(e.KeyData);
                }
            }
        }

        private void KeyboardLL_KeyUp(object sender, KeyEventArgs e)
        {
            Keys key = e.KeyData;
            if (key == Keys.F3 && hookFlag)
            {
                drawFlag = false;
                elemManage.SetWaitListener();
            }
            else if (key == Keys.F2||key == Keys.F6)
            {
                //drawFlag = false;
                //elemManage.SetValueListener();
            }
            else
            {
                if (hookFlag&&elemManage.isTopWindow())
                {
                    elemManage.KeyUpAction(e.KeyData);
                }
                    
            }
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
                string name = elemManage.GetNameFromHandle(lParam);
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
                //AddText("show drop down");
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
                //    AddText("hidden windows: " + elemManage.GetNameFromHandle(Handle));
                //}
                //else
                //{
                //    AddText("show windows: " + elemManage.GetNameFromHandle(Handle));
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
            openFileDialog.Filter = "可执行文件|*.exe*|所有文件|*.*";
            openFileDialog.RestoreDirectory = true;
            openFileDialog.FilterIndex = 1;
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                this.textBox1.Text = openFileDialog.FileName;
                strPath = openFileDialog.FileName;
            }
        }

        //public void SubscribeToFocusChange()
        //{
        //    focusHandler = new AutomationFocusChangedEventHandler(OnFocusChange);
        //    Automation.AddAutomationFocusChangedEventHandler(focusHandler);
        //}

        //private void OnFocusChange(object src, AutomationFocusChangedEventArgs e)
        //{
        //    // TODO Add event handling code.
        //    // The arguments tell you which elements have lost and received focus.
        //    string message = "";
        //    Thread thread = new Thread(() =>
        //    {
        //        AutomationElement element = src as AutomationElement;
        //        message = elemManage.GetCurrentElementInfo(element);


        //    });
        //    thread.Start();
        //    thread.Join();
        //    AddText("SetFocus " + message);
        //}

        //public void UnsubscribeFocusChange()
        //{
        //    if (focusHandler != null)
        //    {
        //        Automation.RemoveAutomationFocusChangedEventHandler(focusHandler);
        //    }
        //}

        //private void SubscribeToMenu()
        //{
        //    UIAEventHandler = new AutomationEventHandler(OnUIAEvent);
        //    //Automation.AddAutomationEventHandler(InvokePattern.InvokedEvent,
        //    //                        AutomationElement.RootElement,
        //    //                        TreeScope.Descendants, UIAEventHandler);
        //    Automation.AddAutomationEventHandler(AutomationElement.MenuOpenedEvent,
        //                            AutomationElement.RootElement,
        //                            TreeScope.Descendants, UIAEventHandler);
        //    Automation.AddAutomationEventHandler(AutomationElement.MenuClosedEvent,
        //                            AutomationElement.RootElement,
        //                            TreeScope.Descendants,
        //                            UIAEventHandler);
        //}
        
        //public void OnUIAEvent(object src, AutomationEventArgs args)
        //{
        //    AutomationElement element;
        //    try
        //    {
        //        element = src as AutomationElement;
        //    }
        //    catch
        //    {
        //        return;
        //    }
        //    string name = "";
        //    if (element == null)
        //        name = "null";
        //    else
        //    {
        //        name = element.GetCurrentPropertyValue(
        //                AutomationElement.NameProperty) as string;
        //    }
        //    if (name.Length == 0) name = "<NoName>";
        //    string str = string.Format("Menu {0} : {1}", name,
        //                                args.EventId.ProgrammaticName);
        //    //Console.WriteLine(str);
        //    AddText(str);
        //}
        
        private void AddText(string m)
        {
            //this.ListCbt.Items.Add(message);
            string message = m;
            string str = message;
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
            if (!str.EndsWith("\n"))
            {
                str += "\n";
            }
            textBoxMessages.Text = message + textBoxMessages.Text;
            elemManage.AddText(str);
        }

        public void StartUiaWorkerThread()
        {
            elemManage.RegisterForEvents();
        }

        private void OpenCompareDialog()
        {
            StopHook();
            CompareForm form = new CompareForm();
            //form.ShowDialog();
            //dialogFrom = new CompareForm();
            //dialogFrom.Owner = this;
            if (form.ShowDialog() == DialogResult.OK)
            {
                string info = form.GetCompareInfo();
                AddText(info);
            }
            //form.Close();
            form.Dispose();
            form = null;
            StartHook();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Script file|*.aui";
            openFileDialog.RestoreDirectory = true;
            openFileDialog.FilterIndex = 1;
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                string path = openFileDialog.FileName;
                if (!path.EndsWith(".aui"))
                {
                    MessageBox.Show("Please select aui file");
                    return;
                }
                this.textBox2.Text = openFileDialog.FileName;
                
            }
        }

        private void _GetMessage(string message)
        {
            AddText(message);
        }

        public void _GetMessageFrom(string message)
        {
            AddText(message);
        }

        private void SetWaitListener()
        {
            elemManage.SetWaitListener();
        }

        private void SetValueListener()
        {
            elemManage.SetValueListener();
        }
    }
}
