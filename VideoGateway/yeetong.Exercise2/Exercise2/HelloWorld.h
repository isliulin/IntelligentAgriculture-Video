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
	/// HelloWorld ժҪ
	/// </summary>
	public ref class HelloWorld : public System::Windows::Forms::Form
	{
	public:
		HelloWorld(void)
		{
			InitializeComponent();
			//
			//TODO:  �ڴ˴���ӹ��캯������
			//
		}
		void Start()
		{
			try{
				ToolAPI::XMLOperation::WriteLogXmlNoTail("��������", "");
				String^ sql = "update vsg_FIB set pushflowstatus ='-1'";
				int result = AccessOperate::AccessHelper::InsertData(sql);
				ToolAPI::XMLOperation::WriteLogXmlNoTail("������ʼ������״̬Ϊδ����", result.ToString());
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
				ToolAPI::XMLOperation::WriteLogXmlNoTail("���д��䱻�ر�", result.ToString());
				ToolAPI::XMLOperation::WriteLogXmlNoTail("����ر�", "");
			}
			catch (int ex){}
		}
		delegate void RunTransferDelegate(String^ rtsp, String^ rtmp);
	protected:
		/// <summary>
		/// ������������ʹ�õ���Դ��
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
				//cp->ExStyle &= (~WS_EX_APPWINDOW);    // ����ʾ��TaskBar
				//cp->ExStyle |= WS_EX_TOOLWINDOW;      // ����ʾ��Alt+Tab
				//return cp;

				System::Windows::Forms::CreateParams^ cp = __super::CreateParams;
				cp->ExStyle &= (~0x40000);    // ����ʾ��TaskBar
				cp->ExStyle |= 0x80;      // ����ʾ��Alt+Tab
				return cp;
			}
		}





	protected:

	private:
		/// <summary>
		/// ����������������
		/// </summary>
		System::ComponentModel::Container ^components;

#pragma region Windows Form Designer generated code
		/// <summary>
		/// �����֧������ķ��� - ��Ҫ
		/// ʹ�ô���༭���޸Ĵ˷��������ݡ�
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
			this->Text = L"��������";
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
			this->label1->Text = "��������";
		}*/
		static void RunTransferFun()
		{
			while (true)
			{
				try{
					//�������ݿ�
					String^ sql = "select * from vsg_FIB";
					DataTable^ dt = AccessOperate::AccessHelper::GetTable(sql);
					String^ deviceidList = "&";
					if (dt != nullptr&&dt->Rows->Count > 0){
						for (int i = 0; i < dt->Rows->Count; i++){
							DataRow^ dr = dt->Rows[i];
							String^ pushflowmode = dr["pushflowmode"]->ToString();
							String^ iscall = dr["iscall"]->ToString();//�Ƿ񱻺���
							String^ deviceid = dr["deviceid"]->ToString();//�豸���
							String^ pushflowstatus = dr["pushflowstatus"]->ToString();//����״̬
							String^ rtsp = dr["rtspurl"]->ToString();
							String^ rtmp = dr["rtmpurl"]->ToString();
							deviceidList += deviceid + "&";
							if (VSGTool::PingToServer::IsNet){
								//����Ƿ����˸ı䣬������ֹͣ��Ӧ���̡߳������ֵ����һ�¡�
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
											ToolAPI::XMLOperation::WriteLogXmlNoTail("�����߳���rtsp��rtmp�ı䱻�ر�", deviceid + ";" + result.ToString());
										}
									}
								}
								DictionaryDataRowTool::Add(deviceid, dr);

								if ((pushflowmode == "0"&&iscall == "1") || pushflowmode == "1")//���б��������߳���
								{
									if ((rtsp == "" || rtmp == "") && pushflowstatus != "4")
									{
										String^ sql = "update vsg_FIB set pushflowstatus ='4' ,isupdate='1' where deviceid='" + deviceid + "'";
										int result = AccessOperate::AccessHelper::InsertData(sql);
										ToolAPI::XMLOperation::WriteLogXmlNoTail("������������ַΪ��", deviceid + ";" + result.ToString());
									}
									else if (pushflowstatus == "5")//��ʾҪ���ǂ��̸߳��滻������һ����������ĸ����������-1482175992���쳣����
									{
										System::Environment::Exit(0);
										DictionaryTool::Remove(deviceid);
										String^ sql = ""; int result = -1;
										Thread^ StartRunTransferT = gcnew Thread(gcnew ParameterizedThreadStart(&HelloWorld::StartRunTransfer));
										sql = "update vsg_FIB set pushflowstatus ='0' ,isupdate='1' where deviceid='" + deviceid + "'";
										result = AccessOperate::AccessHelper::InsertData(sql);
										ToolAPI::XMLOperation::WriteLogXmlNoTail("����-1482175992�������߳�", deviceid + ";" + result.ToString());
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


										//ֱ�Ӿ��Ǳ�����û���Ǹ��߳�Ů��������оͲ������ˣ����û�оʹ������߳�
										if (!DictionaryTool::IsExist(deviceid))
										{
											String^ sql = ""; int result = -1;
											Thread^ StartRunTransferT = gcnew Thread(gcnew ParameterizedThreadStart(&HelloWorld::StartRunTransfer));
											sql = "update vsg_FIB set pushflowstatus ='0' ,isupdate='1' where deviceid='" + deviceid + "'";
											result = AccessOperate::AccessHelper::InsertData(sql);
											ToolAPI::XMLOperation::WriteLogXmlNoTail("�״δ������ӿ�ʼ", deviceid + ";" + result.ToString());
											StartRunTransferT->IsBackground = true;
											StartRunTransferT->Start(dr);
											DictionaryTool::Add(deviceid, StartRunTransferT);
										}
										else
										{
											if (pushflowstatus == "-1"){//��Ϊ-1ʱ������������û��ԭ�������������ˣ������߳�ȴ����ִ�У�ֻ��ɱ���߳����¿�ʼ��
												DictionaryTool::Remove(deviceid);//��ɱ���߳�
												//��ʼ�����¿�ʼ
												String^ sql = ""; int result = -1;
												Thread^ StartRunTransferT = gcnew Thread(gcnew ParameterizedThreadStart(&HelloWorld::StartRunTransfer));
												sql = "update vsg_FIB set pushflowstatus ='0' ,isupdate='1' where deviceid='" + deviceid + "'";
												result = AccessOperate::AccessHelper::InsertData(sql);
												ToolAPI::XMLOperation::WriteLogXmlNoTail("�״δ������ӿ�ʼ", deviceid + ";" + result.ToString());
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
										ToolAPI::XMLOperation::WriteLogXmlNoTail("�����̱߳��ر�", deviceid + ";" + result.ToString());
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
									ToolAPI::XMLOperation::WriteLogXmlNoTail("�����߳���Ϊ��ʧ���类�ر�", deviceid + ";" + result.ToString());
								}
							}
						}
					}
					bool result2 = DictionaryTool::SelfChecking(deviceidList);//�Լ�ɾ�������ڵ��豸 ɾ���߳�
					bool result3 = DictionaryDataRowTool::SelfChecking(deviceidList);//�Լ�ɾ�������ڵ��豸 ɾ��DataRow
					if (isFirst) isFirst = false;//��һ���Ǳ���Ҫ���������ġ�
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
			//�豸���
			String^ deviceidtemp = dr["deviceid"]->ToString();//�豸���
			//����״̬
			String^ pushflowstatus = dr["pushflowstatus"]->ToString();//����״̬
			bool isFirst = true;
		/*	if (pushflowstatus == "-1")
				isFirst = true;
			else if (pushflowstatus == "2")
				isFirst = false;*/
	/*		����ַ*/
			String^ rtsp =dr["rtspurl"]->ToString();
			/*String^ rtsp = "ffmpeg -stimeout 5000000  -i " + dr["rtspurl"]->ToString();*/
			String^ rtmp = dr["rtmpurl"]->ToString();
			const char* rtsp_char = (char*)(void*)Marshal::StringToHGlobalAnsi(rtsp);
			const char* rtmp_char = (char*)(void*)Marshal::StringToHGlobalAnsi(rtmp);
			//��ʼ��������
			CRtspToRtmp*  rtspToRtmp = new CRtspToRtmp();
			rtspToRtmp->RunTransfer(deviceidtemp, isFirst, rtsp_char, rtmp_char);
			try{
				delete rtspToRtmp;//ɾ�����������Ķ���
			}
			catch (int ex){
				ToolAPI::XMLOperation::WriteLogXmlNoTail("ɾ��rtspToRtmp�����쳣", deviceidtemp);
			}
		/*	String^ sql = "update vsg_FIB set pushflowstatus ='2'  ,isupdate='1' where deviceid='" + deviceidtemp + "'";
			int result = AccessOperate::AccessHelper::InsertData(sql);
			ToolAPI::XMLOperation::WriteLogXmlNoTail("����ʧ�ܽ���Ѿ��������ݿ�", deviceidtemp + ";" + result.ToString());*/
		}
	private: System::Void button2_Click(System::Object^  sender, System::EventArgs^  e) {
		Stop();
		/*this->label1->Text = "��������";*/
	}
			 //���ط���
	private: System::Void HelloWorld_Load(System::Object^  sender, System::EventArgs^  e) {
		this->Visible = false;
		Start();
	}
			 //�رշ���
	private: System::Void HelloWorld_FormClosing(System::Object^  sender, System::Windows::Forms::FormClosingEventArgs^  e) {
		Stop();
	}
	};
}
