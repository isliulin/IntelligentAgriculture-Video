#pragma once

namespace Exercise2 {

	using namespace System;
	using namespace System::ComponentModel;
	using namespace System::Collections;
	using namespace System::Collections::Generic;
	using namespace System::Windows::Forms;
	using namespace System::Data;
	using namespace System::Drawing;
	using namespace System::Runtime::InteropServices;
	using namespace System::Threading;
	using namespace AccessOperate;
	using namespace VSGTool;


#include "ffmpegLib.h";

#include "RtspToRtmp.cpp";
	/// <summary>
	/// HelloWorld 摘要
	/// </summary>
	public ref class HelloWorld : public System::Windows::Forms::Form
	{
	public:
		HelloWorld(void)
		{
			InitializeComponent();
			//
			//TODO:  在此处添加构造函数代码
			//
		}
		void Start()
		{
			try{
				ToolAPI::XMLOperation::WriteLogXmlNoTail("程序启动", "");
				String^ sql = "update vsg_FIB set pushflowstatus ='-1'";
				int result = AccessOperate::AccessHelper::InsertData(sql);
				ToolAPI::XMLOperation::WriteLogXmlNoTail("启动初始化推流状态为未启动", result.ToString());
				UpdateDbNetAndSnT = gcnew Thread(gcnew ThreadStart(&HelloWorld::RunTransferFun));
				UpdateDbNetAndSnT->Start();
			}
			catch (int ex){}
		}
		void Stop()
		{
			try{
				if (UpdateDbNetAndSnT != nullptr && UpdateDbNetAndSnT->IsAlive)
				{
					UpdateDbNetAndSnT->Abort();
					UpdateDbNetAndSnT = nullptr;
				}
				DictionaryTool::Clear();
				String^ sql = "update vsg_FIB set pushflowstatus ='-1'";
				int result = AccessOperate::AccessHelper::InsertData(sql);
				ToolAPI::XMLOperation::WriteLogXmlNoTail("所有传输被关闭", result.ToString());
				ToolAPI::XMLOperation::WriteLogXmlNoTail("程序关闭", "");
			}
			catch (int ex){}
		}
		delegate void RunTransferDelegate(String^ rtsp, String^ rtmp);
	protected:
		/// <summary>
		/// 清理所有正在使用的资源。
		/// </summary>
		~HelloWorld()
		{
			if (components)
			{
				delete components;
			}
		}
		property System::Windows::Forms::CreateParams^ CreateParams
		{

			virtual System::Windows::Forms::CreateParams^ get() override
			{

				// Extend the CreateParams property of the Button class.
				//System::Windows::Forms::CreateParams^ cp = __super::CreateParams;

				//// Update the button Style.
				//cp->Style |= 0x00000040; // BS_ICON value
				//return cp;

				//const int WS_EX_APPWINDOW = 0x40000;
				//const int WS_EX_TOOLWINDOW = 0x80;
				//System::Windows::Forms::CreateParams^ cp = __super::CreateParams;
				//cp->ExStyle &= (~WS_EX_APPWINDOW);    // 不显示在TaskBar
				//cp->ExStyle |= WS_EX_TOOLWINDOW;      // 不显示在Alt+Tab
				//return cp;

				System::Windows::Forms::CreateParams^ cp = __super::CreateParams;
				cp->ExStyle &= (~0x40000);    // 不显示在TaskBar
				cp->ExStyle |= 0x80;      // 不显示在Alt+Tab
				return cp;
			}
		}





	protected:

	private:
		/// <summary>
		/// 必需的设计器变量。
		/// </summary>
		System::ComponentModel::Container ^components;

#pragma region Windows Form Designer generated code
		/// <summary>
		/// 设计器支持所需的方法 - 不要
		/// 使用代码编辑器修改此方法的内容。
		/// </summary>
		void InitializeComponent(void)
		{
			this->SuspendLayout();
			// 
			// HelloWorld
			// 
			this->AutoScaleDimensions = System::Drawing::SizeF(6, 12);
			this->AutoScaleMode = System::Windows::Forms::AutoScaleMode::Font;
			this->ClientSize = System::Drawing::Size(0, 0);
			this->FormBorderStyle = System::Windows::Forms::FormBorderStyle::None;
			this->Name = L"HelloWorld";
			this->ShowIcon = false;
			this->ShowInTaskbar = false;
			this->SizeGripStyle = System::Windows::Forms::SizeGripStyle::Hide;
			this->Text = L"流控制器";
			this->WindowState = System::Windows::Forms::FormWindowState::Minimized;
			this->FormClosing += gcnew System::Windows::Forms::FormClosingEventHandler(this, &HelloWorld::HelloWorld_FormClosing);
			this->Load += gcnew System::EventHandler(this, &HelloWorld::HelloWorld_Load);
			this->ResumeLayout(false);

		}
#pragma endregion
	private:
		Thread^ UpdateDbNetAndSnT;
		static bool isFirst = true;
		/*System::Void button1_Click(System::Object^  sender, System::EventArgs^  e) {
			Start();
			this->label1->Text = "正在运行";
		}*/
		static void RunTransferFun()
		{
			while (true)
			{
				try{
					//查找数据库
					String^ sql = "select * from vsg_FIB";
					DataTable^ dt = AccessOperate::AccessHelper::GetTable(sql);
					String^ deviceidList = "&";
					if (dt != nullptr&&dt->Rows->Count > 0){
						for (int i = 0; i < dt->Rows->Count; i++){
							DataRow^ dr = dt->Rows[i];
							String^ pushflowmode = dr["pushflowmode"]->ToString();
							String^ iscall = dr["iscall"]->ToString();//是否被呼叫
							String^ deviceid = dr["deviceid"]->ToString();//设备编号
							String^ pushflowstatus = dr["pushflowstatus"]->ToString();//推流状态
							String^ rtsp = dr["rtspurl"]->ToString();
							String^ rtmp = dr["rtmpurl"]->ToString();
							deviceidList += deviceid + "&";
							if (VSGTool::PingToServer::IsNet){
								//如果是发生了改变，则立即停止对应的线程。最后把字典更新一下。
								DataRow^ drold = DictionaryDataRowTool::GetValueByKey(deviceid);
								if (drold != nullptr){
									String^ rtspOld = drold["rtspurl"]->ToString();
									String^ rtmpOld = drold["rtmpurl"]->ToString();
									if (rtspOld != rtsp || rtmpOld != rtmp){
										DictionaryTool::Remove(deviceid);
										if (pushflowstatus != "-1")
										{
											sql = "update vsg_FIB set pushflowstatus ='-1' ,isupdate='1' where deviceid='" + deviceid + "'";
											int result = AccessOperate::AccessHelper::InsertData(sql);
											ToolAPI::XMLOperation::WriteLogXmlNoTail("传输线程因rtsp和rtmp改变被关闭", deviceid + ";" + result.ToString());
										}
									}
								}
								DictionaryDataRowTool::Add(deviceid, dr);

								if ((pushflowmode == "0"&&iscall == "1") || pushflowmode == "1")//呼叫被触发或者长期
								{
									if ((rtsp == "" || rtmp == "") && pushflowstatus != "4")
									{
										String^ sql = "update vsg_FIB set pushflowstatus ='4' ,isupdate='1' where deviceid='" + deviceid + "'";
										int result = AccessOperate::AccessHelper::InsertData(sql);
										ToolAPI::XMLOperation::WriteLogXmlNoTail("拉流或推流地址为空", deviceid + ";" + result.ToString());
									}
									else if (pushflowstatus == "5")//表示要把那個线程给替换掉，看一看还会出现哪个情况（拉流-1482175992的异常）吗
									{
										System::Environment::Exit(0);
										DictionaryTool::Remove(deviceid);
										String^ sql = ""; int result = -1;
										Thread^ StartRunTransferT = gcnew Thread(gcnew ParameterizedThreadStart(&HelloWorld::StartRunTransfer));
										sql = "update vsg_FIB set pushflowstatus ='0' ,isupdate='1' where deviceid='" + deviceid + "'";
										result = AccessOperate::AccessHelper::InsertData(sql);
										ToolAPI::XMLOperation::WriteLogXmlNoTail("拉流-1482175992后重启线程", deviceid + ";" + result.ToString());
										StartRunTransferT->IsBackground = true;
										StartRunTransferT->Start(dr);
										DictionaryTool::Add(deviceid, StartRunTransferT);
									}
									else
									{
										/*CRtspToRtmp*  rtspToRtmp = new CRtspToRtmp();
										char* rtsp_char = (char*)(void*)Marshal::StringToHGlobalAnsi(rtsp);
										char* rtmp_char = (char*)(void*)Marshal::StringToHGlobalAnsi(rtmp);
										rtspToRtmp->push(rtsp_char, rtmp_char);*/


										//直接就是遍历有没有那个线程女儿，如果有就不管它了，如果没有就创建个线程
										if (!DictionaryTool::IsExist(deviceid))
										{
											String^ sql = ""; int result = -1;
											Thread^ StartRunTransferT = gcnew Thread(gcnew ParameterizedThreadStart(&HelloWorld::StartRunTransfer));
											sql = "update vsg_FIB set pushflowstatus ='0' ,isupdate='1' where deviceid='" + deviceid + "'";
											result = AccessOperate::AccessHelper::InsertData(sql);
											ToolAPI::XMLOperation::WriteLogXmlNoTail("首次传输连接开始", deviceid + ";" + result.ToString());
											StartRunTransferT->IsBackground = true;
											StartRunTransferT->Start(dr);
											DictionaryTool::Add(deviceid, StartRunTransferT);
										}
										else
										{
											if (pushflowstatus == "-1"){//当为-1时，可能是由于没个原因主程序被重启了，但是线程却还在执行，只能杀死线程重新开始了
												DictionaryTool::Remove(deviceid);//先杀死线程
												//初始化重新开始
												String^ sql = ""; int result = -1;
												Thread^ StartRunTransferT = gcnew Thread(gcnew ParameterizedThreadStart(&HelloWorld::StartRunTransfer));
												sql = "update vsg_FIB set pushflowstatus ='0' ,isupdate='1' where deviceid='" + deviceid + "'";
												result = AccessOperate::AccessHelper::InsertData(sql);
												ToolAPI::XMLOperation::WriteLogXmlNoTail("首次传输连接开始", deviceid + ";" + result.ToString());
												StartRunTransferT->IsBackground = true;
												StartRunTransferT->Start(dr);
												DictionaryTool::Add(deviceid, StartRunTransferT);
											}
										}
									}

								}
								else
								{
									DictionaryTool::Remove(deviceid);
									if (pushflowstatus != "-1")
									{
										sql = "update vsg_FIB set pushflowstatus ='-1' ,isupdate='1',callClientCount='0' where deviceid='" + deviceid + "'";
										int result = AccessOperate::AccessHelper::InsertData(sql);
										ToolAPI::XMLOperation::WriteLogXmlNoTail("传输线程被关闭", deviceid + ";" + result.ToString());
									}
									continue;
								}
							}
							else
							{
								DictionaryTool::Remove(deviceid);
								if (pushflowstatus != "-1")
								{
									sql = "update vsg_FIB set pushflowstatus ='-1' ,isupdate='1' ,callClientCount='0' where deviceid='" + deviceid + "'";
									int result = AccessOperate::AccessHelper::InsertData(sql);
									ToolAPI::XMLOperation::WriteLogXmlNoTail("传输线程因为丢失网络被关闭", deviceid + ";" + result.ToString());
								}
							}
						}
					}
					bool result2 = DictionaryTool::SelfChecking(deviceidList);//自检删除不存在的设备 删除线程
					bool result3 = DictionaryDataRowTool::SelfChecking(deviceidList);//自检删除不存在的设备 删除DataRow
					if (isFirst) isFirst = false;//第一次是必须要进行重启的。
				}
				catch (int ex)
				{
					
				}
				Thread::Sleep(500);
			}
		}
		static void StartRunTransfer(Object^ rtspAndrtmp)
		{
			DataRow^ dr = (DataRow^)rtspAndrtmp;
			//设备编号
			String^ deviceidtemp = dr["deviceid"]->ToString();//设备编号
			//推流状态
			String^ pushflowstatus = dr["pushflowstatus"]->ToString();//推流状态
			bool isFirst = true;
		/*	if (pushflowstatus == "-1")
				isFirst = true;
			else if (pushflowstatus == "2")
				isFirst = false;*/
	/*		流地址*/
			String^ rtsp =dr["rtspurl"]->ToString();
			/*String^ rtsp = "ffmpeg -stimeout 5000000  -i " + dr["rtspurl"]->ToString();*/
			String^ rtmp = dr["rtmpurl"]->ToString();
			const char* rtsp_char = (char*)(void*)Marshal::StringToHGlobalAnsi(rtsp);
			const char* rtmp_char = (char*)(void*)Marshal::StringToHGlobalAnsi(rtmp);
			//开始建立传输
			CRtspToRtmp*  rtspToRtmp = new CRtspToRtmp();
			rtspToRtmp->RunTransfer(deviceidtemp, isFirst, rtsp_char, rtmp_char);
			try{
				delete rtspToRtmp;//删除创建出来的对象
			}
			catch (int ex){
				ToolAPI::XMLOperation::WriteLogXmlNoTail("删除rtspToRtmp出现异常", deviceidtemp);
			}
		/*	String^ sql = "update vsg_FIB set pushflowstatus ='2'  ,isupdate='1' where deviceid='" + deviceidtemp + "'";
			int result = AccessOperate::AccessHelper::InsertData(sql);
			ToolAPI::XMLOperation::WriteLogXmlNoTail("传输失败结果已经存入数据库", deviceidtemp + ";" + result.ToString());*/
		}
	private: System::Void button2_Click(System::Object^  sender, System::EventArgs^  e) {
		Stop();
		/*this->label1->Text = "结束运行";*/
	}
			 //加载方法
	private: System::Void HelloWorld_Load(System::Object^  sender, System::EventArgs^  e) {
		this->Visible = false;
		Start();
	}
			 //关闭方法
	private: System::Void HelloWorld_FormClosing(System::Object^  sender, System::Windows::Forms::FormClosingEventArgs^  e) {
		Stop();
	}
	};
}
