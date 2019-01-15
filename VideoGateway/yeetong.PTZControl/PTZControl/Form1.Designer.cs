namespace PTZControl
{
    partial class Form1
    {
        /// <summary>
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows 窗体设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.RealPlayWnd = new System.Windows.Forms.Panel();
            this.button1 = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.textBoxIP = new System.Windows.Forms.TextBox();
            this.textBoxPort = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.textBoxPassword = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.textBoxUserName = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.button2 = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.button4 = new System.Windows.Forms.Button();
            this.button3 = new System.Windows.Forms.Button();
            this.label9 = new System.Windows.Forms.Label();
            this.IRIS_CLOSE = new System.Windows.Forms.Button();
            this.IRIS_OPEN = new System.Windows.Forms.Button();
            this.label8 = new System.Windows.Forms.Label();
            this.FOCUS_FAR = new System.Windows.Forms.Button();
            this.FOCUS_NEAR = new System.Windows.Forms.Button();
            this.label7 = new System.Windows.Forms.Label();
            this.ZOOM_OUT = new System.Windows.Forms.Button();
            this.ZOOM_IN = new System.Windows.Forms.Button();
            this.comboBoxSpeed = new System.Windows.Forms.ComboBox();
            this.label6 = new System.Windows.Forms.Label();
            this.btnAuto = new System.Windows.Forms.Button();
            this.btnDown = new System.Windows.Forms.Button();
            this.btnUp = new System.Windows.Forms.Button();
            this.btnRight = new System.Windows.Forms.Button();
            this.btnLeft = new System.Windows.Forms.Button();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // RealPlayWnd
            // 
            this.RealPlayWnd.BackColor = System.Drawing.Color.Black;
            this.RealPlayWnd.Location = new System.Drawing.Point(23, 80);
            this.RealPlayWnd.Name = "RealPlayWnd";
            this.RealPlayWnd.Size = new System.Drawing.Size(517, 351);
            this.RealPlayWnd.TabIndex = 0;
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(348, 15);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 55);
            this.button1.TabIndex = 1;
            this.button1.Text = "登录";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(23, 15);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(17, 12);
            this.label1.TabIndex = 2;
            this.label1.Text = "ip";
            // 
            // textBoxIP
            // 
            this.textBoxIP.Location = new System.Drawing.Point(70, 10);
            this.textBoxIP.Name = "textBoxIP";
            this.textBoxIP.Size = new System.Drawing.Size(100, 21);
            this.textBoxIP.TabIndex = 3;
            this.textBoxIP.Text = "10.10.10.100";
            // 
            // textBoxPort
            // 
            this.textBoxPort.Location = new System.Drawing.Point(223, 10);
            this.textBoxPort.Name = "textBoxPort";
            this.textBoxPort.Size = new System.Drawing.Size(100, 21);
            this.textBoxPort.TabIndex = 5;
            this.textBoxPort.Text = "8000";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(176, 15);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(29, 12);
            this.label2.TabIndex = 4;
            this.label2.Text = "端口";
            // 
            // textBoxPassword
            // 
            this.textBoxPassword.Location = new System.Drawing.Point(223, 53);
            this.textBoxPassword.Name = "textBoxPassword";
            this.textBoxPassword.Size = new System.Drawing.Size(100, 21);
            this.textBoxPassword.TabIndex = 9;
            this.textBoxPassword.Text = "admin";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(176, 58);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(29, 12);
            this.label3.TabIndex = 8;
            this.label3.Text = "密码";
            // 
            // textBoxUserName
            // 
            this.textBoxUserName.Location = new System.Drawing.Point(70, 53);
            this.textBoxUserName.Name = "textBoxUserName";
            this.textBoxUserName.Size = new System.Drawing.Size(100, 21);
            this.textBoxUserName.TabIndex = 7;
            this.textBoxUserName.Text = "admin";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(23, 58);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(41, 12);
            this.label4.TabIndex = 6;
            this.label4.Text = "账户名";
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(455, 15);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(75, 55);
            this.button2.TabIndex = 10;
            this.button2.Text = "预览";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.button4);
            this.groupBox1.Controls.Add(this.button3);
            this.groupBox1.Controls.Add(this.label9);
            this.groupBox1.Controls.Add(this.IRIS_CLOSE);
            this.groupBox1.Controls.Add(this.IRIS_OPEN);
            this.groupBox1.Controls.Add(this.label8);
            this.groupBox1.Controls.Add(this.FOCUS_FAR);
            this.groupBox1.Controls.Add(this.FOCUS_NEAR);
            this.groupBox1.Controls.Add(this.label7);
            this.groupBox1.Controls.Add(this.ZOOM_OUT);
            this.groupBox1.Controls.Add(this.ZOOM_IN);
            this.groupBox1.Controls.Add(this.comboBoxSpeed);
            this.groupBox1.Controls.Add(this.label6);
            this.groupBox1.Controls.Add(this.btnAuto);
            this.groupBox1.Controls.Add(this.btnDown);
            this.groupBox1.Controls.Add(this.btnUp);
            this.groupBox1.Controls.Add(this.btnRight);
            this.groupBox1.Controls.Add(this.btnLeft);
            this.groupBox1.Location = new System.Drawing.Point(560, 15);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(268, 416);
            this.groupBox1.TabIndex = 11;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "云台控制";
            // 
            // button4
            // 
            this.button4.Location = new System.Drawing.Point(156, 387);
            this.button4.Name = "button4";
            this.button4.Size = new System.Drawing.Size(75, 23);
            this.button4.TabIndex = 28;
            this.button4.Text = "左下";
            this.button4.UseVisualStyleBackColor = true;
            this.button4.MouseDown += new System.Windows.Forms.MouseEventHandler(this.button4_MouseDown);
            this.button4.MouseUp += new System.Windows.Forms.MouseEventHandler(this.button4_MouseUp);
            // 
            // button3
            // 
            this.button3.Location = new System.Drawing.Point(37, 387);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(75, 23);
            this.button3.TabIndex = 27;
            this.button3.Text = "button3";
            this.button3.UseVisualStyleBackColor = true;
            this.button3.Click += new System.EventHandler(this.button3_Click);
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(118, 342);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(29, 12);
            this.label9.TabIndex = 26;
            this.label9.Text = "光圈";
            // 
            // IRIS_CLOSE
            // 
            this.IRIS_CLOSE.Location = new System.Drawing.Point(157, 337);
            this.IRIS_CLOSE.Name = "IRIS_CLOSE";
            this.IRIS_CLOSE.Size = new System.Drawing.Size(75, 23);
            this.IRIS_CLOSE.TabIndex = 25;
            this.IRIS_CLOSE.Text = "缩小";
            this.IRIS_CLOSE.UseVisualStyleBackColor = true;
            this.IRIS_CLOSE.MouseDown += new System.Windows.Forms.MouseEventHandler(this.IRIS_CLOSE_MouseDown);
            this.IRIS_CLOSE.MouseUp += new System.Windows.Forms.MouseEventHandler(this.IRIS_CLOSE_MouseUp);
            // 
            // IRIS_OPEN
            // 
            this.IRIS_OPEN.Location = new System.Drawing.Point(37, 337);
            this.IRIS_OPEN.Name = "IRIS_OPEN";
            this.IRIS_OPEN.Size = new System.Drawing.Size(75, 23);
            this.IRIS_OPEN.TabIndex = 24;
            this.IRIS_OPEN.Text = "扩大";
            this.IRIS_OPEN.UseVisualStyleBackColor = true;
            this.IRIS_OPEN.MouseDown += new System.Windows.Forms.MouseEventHandler(this.IRIS_OPEN_MouseDown);
            this.IRIS_OPEN.MouseUp += new System.Windows.Forms.MouseEventHandler(this.IRIS_OPEN_MouseUp);
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(118, 294);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(29, 12);
            this.label8.TabIndex = 23;
            this.label8.Text = "焦点";
            // 
            // FOCUS_FAR
            // 
            this.FOCUS_FAR.Location = new System.Drawing.Point(157, 289);
            this.FOCUS_FAR.Name = "FOCUS_FAR";
            this.FOCUS_FAR.Size = new System.Drawing.Size(75, 23);
            this.FOCUS_FAR.TabIndex = 22;
            this.FOCUS_FAR.Text = "后调";
            this.FOCUS_FAR.UseVisualStyleBackColor = true;
            this.FOCUS_FAR.MouseDown += new System.Windows.Forms.MouseEventHandler(this.FOCUS_FAR_MouseDown);
            this.FOCUS_FAR.MouseUp += new System.Windows.Forms.MouseEventHandler(this.FOCUS_FAR_MouseUp);
            // 
            // FOCUS_NEAR
            // 
            this.FOCUS_NEAR.Location = new System.Drawing.Point(37, 289);
            this.FOCUS_NEAR.Name = "FOCUS_NEAR";
            this.FOCUS_NEAR.Size = new System.Drawing.Size(75, 23);
            this.FOCUS_NEAR.TabIndex = 21;
            this.FOCUS_NEAR.Text = "前调";
            this.FOCUS_NEAR.UseVisualStyleBackColor = true;
            this.FOCUS_NEAR.MouseDown += new System.Windows.Forms.MouseEventHandler(this.FOCUS_NEAR_MouseDown);
            this.FOCUS_NEAR.MouseUp += new System.Windows.Forms.MouseEventHandler(this.FOCUS_NEAR_MouseUp);
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(118, 248);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(29, 12);
            this.label7.TabIndex = 20;
            this.label7.Text = "焦距";
            // 
            // ZOOM_OUT
            // 
            this.ZOOM_OUT.Location = new System.Drawing.Point(157, 243);
            this.ZOOM_OUT.Name = "ZOOM_OUT";
            this.ZOOM_OUT.Size = new System.Drawing.Size(75, 23);
            this.ZOOM_OUT.TabIndex = 19;
            this.ZOOM_OUT.Text = "变小";
            this.ZOOM_OUT.UseVisualStyleBackColor = true;
            this.ZOOM_OUT.MouseDown += new System.Windows.Forms.MouseEventHandler(this.ZOOM_OUT_MouseDown);
            this.ZOOM_OUT.MouseUp += new System.Windows.Forms.MouseEventHandler(this.ZOOM_OUT_MouseUp);
            // 
            // ZOOM_IN
            // 
            this.ZOOM_IN.Location = new System.Drawing.Point(37, 243);
            this.ZOOM_IN.Name = "ZOOM_IN";
            this.ZOOM_IN.Size = new System.Drawing.Size(75, 23);
            this.ZOOM_IN.TabIndex = 18;
            this.ZOOM_IN.Text = "变大";
            this.ZOOM_IN.UseVisualStyleBackColor = true;
            this.ZOOM_IN.MouseDown += new System.Windows.Forms.MouseEventHandler(this.ZOOM_IN_MouseDown);
            this.ZOOM_IN.MouseUp += new System.Windows.Forms.MouseEventHandler(this.ZOOM_IN_MouseUp);
            // 
            // comboBoxSpeed
            // 
            this.comboBoxSpeed.FormattingEnabled = true;
            this.comboBoxSpeed.Items.AddRange(new object[] {
            "1",
            "2",
            "3",
            "4",
            "5",
            "6",
            "7"});
            this.comboBoxSpeed.Location = new System.Drawing.Point(95, 30);
            this.comboBoxSpeed.Name = "comboBoxSpeed";
            this.comboBoxSpeed.Size = new System.Drawing.Size(137, 20);
            this.comboBoxSpeed.TabIndex = 16;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(35, 38);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(53, 12);
            this.label6.TabIndex = 14;
            this.label6.Text = "云台速度";
            // 
            // btnAuto
            // 
            this.btnAuto.Location = new System.Drawing.Point(109, 131);
            this.btnAuto.Name = "btnAuto";
            this.btnAuto.Size = new System.Drawing.Size(52, 33);
            this.btnAuto.TabIndex = 13;
            this.btnAuto.Text = "自动";
            this.btnAuto.UseVisualStyleBackColor = true;
            this.btnAuto.Click += new System.EventHandler(this.btnAuto_Click);
            // 
            // btnDown
            // 
            this.btnDown.Location = new System.Drawing.Point(109, 185);
            this.btnDown.Name = "btnDown";
            this.btnDown.Size = new System.Drawing.Size(52, 33);
            this.btnDown.TabIndex = 12;
            this.btnDown.Text = "下";
            this.btnDown.UseVisualStyleBackColor = true;
            this.btnDown.MouseDown += new System.Windows.Forms.MouseEventHandler(this.btnDown_MouseDown);
            this.btnDown.MouseUp += new System.Windows.Forms.MouseEventHandler(this.btnDown_MouseUp);
            // 
            // btnUp
            // 
            this.btnUp.Location = new System.Drawing.Point(109, 79);
            this.btnUp.Name = "btnUp";
            this.btnUp.Size = new System.Drawing.Size(52, 33);
            this.btnUp.TabIndex = 11;
            this.btnUp.Text = "上";
            this.btnUp.UseVisualStyleBackColor = true;
            this.btnUp.MouseDown += new System.Windows.Forms.MouseEventHandler(this.btnUp_MouseDown);
            this.btnUp.MouseUp += new System.Windows.Forms.MouseEventHandler(this.btnUp_MouseUp);
            // 
            // btnRight
            // 
            this.btnRight.Location = new System.Drawing.Point(180, 131);
            this.btnRight.Name = "btnRight";
            this.btnRight.Size = new System.Drawing.Size(52, 33);
            this.btnRight.TabIndex = 10;
            this.btnRight.Text = "右";
            this.btnRight.UseVisualStyleBackColor = true;
            this.btnRight.MouseDown += new System.Windows.Forms.MouseEventHandler(this.btnRight_MouseDown);
            this.btnRight.MouseUp += new System.Windows.Forms.MouseEventHandler(this.btnRight_MouseUp);
            // 
            // btnLeft
            // 
            this.btnLeft.Location = new System.Drawing.Point(37, 131);
            this.btnLeft.Name = "btnLeft";
            this.btnLeft.Size = new System.Drawing.Size(52, 33);
            this.btnLeft.TabIndex = 9;
            this.btnLeft.Text = "左";
            this.btnLeft.UseVisualStyleBackColor = true;
            this.btnLeft.MouseDown += new System.Windows.Forms.MouseEventHandler(this.btnLeft_MouseDown);
            this.btnLeft.MouseUp += new System.Windows.Forms.MouseEventHandler(this.btnLeft_MouseUp);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(854, 443);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.textBoxPassword);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.textBoxUserName);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.textBoxPort);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.textBoxIP);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.RealPlayWnd);
            this.Name = "Form1";
            this.Text = "云台控制";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Panel RealPlayWnd;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox textBoxIP;
        private System.Windows.Forms.TextBox textBoxPort;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox textBoxPassword;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox textBoxUserName;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Button IRIS_CLOSE;
        private System.Windows.Forms.Button IRIS_OPEN;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Button FOCUS_FAR;
        private System.Windows.Forms.Button FOCUS_NEAR;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Button ZOOM_OUT;
        private System.Windows.Forms.Button ZOOM_IN;
        private System.Windows.Forms.ComboBox comboBoxSpeed;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Button btnAuto;
        private System.Windows.Forms.Button btnDown;
        private System.Windows.Forms.Button btnUp;
        private System.Windows.Forms.Button btnRight;
        private System.Windows.Forms.Button btnLeft;
        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.Button button4;
    }
}

