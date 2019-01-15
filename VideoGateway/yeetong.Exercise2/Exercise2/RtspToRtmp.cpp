

#include "ffmpegLib.h"

using namespace System;
using namespace System::Data;
using namespace ToolAPI;

public class CRtspToRtmp{

public:

	void  RunTransfer(String^ deviceid, bool isFirst, const char* rtsptemp, const char* rtmptemp)
	{
		String^ pushflowstatus = "-1";
		while (true)
		{
			Threading::Thread::Sleep(3000);//延时三秒
			try{
				if (isFirst) isFirst = false;
				else
				{
					/*String^ sql = "update vsg_FIB set pushflowstatus ='2'  ,isupdate='1' where deviceid='" + deviceid + "'";
					int result = AccessOperate::AccessHelper::InsertData(sql);
					ToolAPI::XMLOperation::WriteLogXmlNoTail("传输失败结果已经存入数据库", deviceid + ";" + result.ToString());*/
					if (pushflowstatus != "3")
					{
						pushflowstatus = "3";
						String^ sql = "update vsg_FIB set pushflowstatus ='3' ,isupdate='1' where deviceid='" + deviceid + "'";
						int result = AccessOperate::AccessHelper::InsertData(sql);
						ToolAPI::XMLOperation::WriteLogXmlNoTail("重新传输连接开始", deviceid + ";" + result.ToString());
					}
				}
				try{
					String^ sql = "";
					AVFormatContext *inputContext = nullptr;
					AVFormatContext * outputContext;

					//先初始化ffmpeg相关
					av_register_all();//只有调用了该函数，才能使用复用器，编码器等  一切注册的来源
					avfilter_register_all();
					avformat_network_init();//加载socket库以及网络加密协议相关的库，为后续使用网络相关提供支持
					av_log_set_level(AV_LOG_ERROR);
					ToolAPI::XMLOperation::WriteLogXmlNoTail("初始化完成", deviceid);
					//打开输入视频流
					inputContext = avformat_alloc_context();
					AVDictionary* options = nullptr;
					int result0 = av_dict_set(&options, "rtsp_transport", "udp", 0);
					int result1 = av_dict_set(&options, "stimeout", "5000000", 0);//超时一直设置不成功
					int ret = avformat_open_input(&inputContext, rtsptemp, nullptr, &options);
					if (ret < 0)
					{
						if (ret == -1482175992){
							
							avformat_free_context(inputContext);// 回收一下
							DBLog(deviceid, "5", "打开输入流失败跳出线程" + ret.ToString());
							ToolAPI::XMLOperation::WriteLogXmlNoTail("程序关闭", ret.ToString());
							System::Environment::Exit(0);//先这样改一下，不能让他们一直推不上来啊
							break;
						}
						DBLog(deviceid, "2", "打开输入流失败" + ret.ToString());
						avformat_free_context(inputContext);// 回收一下
						continue;
					}
					ret = avformat_find_stream_info(inputContext, nullptr);

					ToolAPI::XMLOperation::WriteLogXmlNoTail("打开输入流完成", deviceid);






					//Output  
					//AVOutputFormat *ofmt = NULL;
					//int i;
					//avformat_alloc_output_context2(&outputContext, NULL, "flv", rtmptemp); //RTMP  
					////avformat_alloc_output_context2(&ofmt_ctx, NULL,"mpegts", output_str);//UDP  
					//if (!outputContext)
					//{
					//	printf("Could not create outputcontext\n");
					//	ret = AVERROR_UNKNOWN;
					//	
					//}
					//ofmt = outputContext->oformat;
					//int videoindex = -1;
					//for (i = 0; i<inputContext->nb_streams; i++)
					//	if (inputContext->streams[i]->codec->codec_type == AVMEDIA_TYPE_VIDEO)
					//	{
					//	videoindex = i;
					//	break;
					//	}
					////Create output AVStream according to input AVStream  
					//AVStream *in_stream = inputContext->streams[videoindex];
					//AVStream *out_stream = avformat_new_stream(outputContext, in_stream->codec->codec);
					//if (!out_stream)
					//{
					//	printf("Error occurred when allocating output stream\n");
					//	ret = AVERROR_UNKNOWN;
					//}
					////Copythe settings of AVCodecContext  
					//ret = avcodec_copy_context(out_stream->codec, in_stream->codec);
					//if (ret< 0)
					//{
					//	printf("error to copy context from input to output stream codeccontext\n");
					//}
					//out_stream->codec->codec_tag = 0;
					//if (outputContext->oformat->flags & AVFMT_GLOBALHEADER)
					//	out_stream->codec->flags |= CODEC_FLAG_GLOBAL_HEADER;

					////Openoutput URL  
					//if (!(ofmt->flags & AVFMT_NOFILE))
					//{
					//	ret = avio_open(&outputContext->pb, rtmptemp, AVIO_FLAG_WRITE);
					//	if (ret < 0)
					//	{
					//		printf("Could not open output URL '%s'", rtmptemp);
					//	}
					//}
					////Writefile header  
					//ret = avformat_write_header(outputContext, NULL);
					//if (ret< 0)
					//{
					//	printf("Error occurred when opening output URL : %d\n", ret);
					//}















					////打开输出视频流
					ret = avformat_alloc_output_context2(&outputContext, nullptr, "flv", rtmptemp);
					if (ret < 0)
					{
						if (outputContext)
						{
							for (unsigned int i = 0; i < outputContext->nb_streams; i++)
							{
								avcodec_close(outputContext->streams[i]->codec);
							}
							avformat_close_input(&outputContext);
						}
						DBLog(deviceid, "2", "打开输出流失败avformat_alloc_output_context2" + ret.ToString());
						avformat_free_context(inputContext);// 回收一下
						avformat_free_context(outputContext);// 回收一下
						continue;
					}
					ret = avio_open2(&outputContext->pb, rtmptemp, AVIO_FLAG_READ_WRITE, nullptr, nullptr);
					if (ret < 0)
					{
						if (outputContext)
						{
							for (unsigned int i = 0; i < outputContext->nb_streams; i++)
							{
								avcodec_close(outputContext->streams[i]->codec);
							}
							avformat_close_input(&outputContext);
						}
						DBLog(deviceid, "2", "打开输出流失败avio_open2" + ret.ToString());
						avformat_free_context(inputContext);// 回收一下
						avformat_free_context(outputContext);// 回收一下
						continue;
					}
					for (unsigned int i = 0; i < inputContext->nb_streams; i++)
					{
						AVStream * stream = avformat_new_stream(outputContext, inputContext->streams[i]->codec->codec);
						ret = avcodec_copy_context(stream->codec, inputContext->streams[i]->codec);
						if (ret < 0)
						{
							if (outputContext)
							{
								for (unsigned int i = 0; i < outputContext->nb_streams; i++)
								{
									avcodec_close(outputContext->streams[i]->codec);
								}
								avformat_close_input(&outputContext);
							}
							DBLog(deviceid, "2", "打开输出流失败avcodec_copy_context" + ret.ToString());
							avformat_free_context(inputContext);// 回收一下
							avformat_free_context(outputContext);// 回收一下
							continue;
						}
					}
					ret = avformat_write_header(outputContext, nullptr);
					if (ret < 0)
					{
						if (outputContext)
						{
							for (unsigned int i = 0; i < outputContext->nb_streams; i++)
							{
								avcodec_close(outputContext->streams[i]->codec);
							}
							avformat_close_input(&outputContext);
						}
						DBLog(deviceid, "2", "打开输出流失败avformat_write_header" + ret.ToString());
						avformat_free_context(inputContext);// 回收一下
						avformat_free_context(outputContext);// 回收一下
						continue;
					}
					if (ret < 0){
						DBLog(deviceid, "2", "打开输出流失败" + ret.ToString());
						avformat_free_context(inputContext);// 回收一下
						avformat_free_context(outputContext);// 回收一下
						continue;
					}
					ToolAPI::XMLOperation::WriteLogXmlNoTail("打开输出流完成", deviceid);

					//开始传输
					DBLog(deviceid, "1", "正在推流");
					pushflowstatus = "1";
					int flag = 0;
					int flag22 = 0;
					int flagnull = 0;
					while (true)
					{
						flag++;
						if (flag == 2000){
							ToolAPI::XMLOperation::WriteLogXmlNoTail("正在进行传输", deviceid);
							flag = 0;
						}
						AVPacket *pkt1 = new AVPacket();
						av_init_packet(pkt1);
						(*pkt1).data = NULL;
						(*pkt1).size = 0;
						int ret = av_read_frame(inputContext, pkt1);
						if (ret < 0)
						{
							ToolAPI::XMLOperation::WriteLogXmlNoTail("传输过程错误", deviceid + "；av_read_frame" + ret.ToString());
							break;
						}
						else
						{
							if (pkt1)
							{
								auto inputStream = inputContext->streams[pkt1->stream_index];
								auto outputStream = outputContext->streams[pkt1->stream_index];
								av_packet_rescale_ts(pkt1, inputStream->time_base, outputStream->time_base);
								////写Packet到服务端
								/*ToolAPI::XMLOperation::WriteLogXmlNoTail("做记录",  pkt1->dts.ToString() + ";" + pkt1->pts.ToString() + ";" + pkt1->duration.ToString());*/
								ret = av_interleaved_write_frame(outputContext, pkt1);
								if (ret >= 0)
								{
									/*	ToolAPI::XMLOperation::WriteLogXmlNoTail("正在传输","" );*/
									av_free_packet(pkt1);
								}
								else if (ret == -22)
								{
									av_free_packet(pkt1);
									flag22++;
									if (flag22 == 500)
									{
										ToolAPI::XMLOperation::WriteLogXmlNoTail("传输偏差-22", deviceid);
										flag22 = 0;
									}
								}
								else
								{
									ToolAPI::XMLOperation::WriteLogXmlNoTail("传输失败", deviceid + ";" + ret.ToString() + ";" + pkt1->dts.ToString() + ";" + pkt1->pts.ToString() + ";" + pkt1->duration.ToString());
									av_free_packet(pkt1);
									break;
								}
							}
							else
							{
								flagnull++;
								if (flagnull == 500)
								{
									ToolAPI::XMLOperation::WriteLogXmlNoTail("传输偏差null", deviceid);
									flagnull = 0;
								}
							}

						}
					}
					DBLog(deviceid, "2", "推流意外结束");
					avformat_free_context(inputContext);// 回收一下
					avformat_free_context(outputContext);// 回收一下
					continue;
				}
				catch (int ex)
				{
					DBLog(deviceid, "2", "推流异常结束");
					continue;
				}
				DBLog(deviceid, "2", "推流异常结束");
				continue;
			}
			catch (int ee){
				DBLog(deviceid, "2", "推流异常结束");
				continue;
			}
		}
	}

	int push(char* input_str, char* output_str)
	{
		AVOutputFormat *ofmt = NULL;
		AVFormatContext *ifmt_ctx = NULL, *ofmt_ctx = NULL;
		AVPacket pkt;

		int ret, i;

		//FFmpeg av_log() callback  
		/*av_log_set_callback(custom_log);*/

		av_register_all();
		//Network  
		avformat_network_init();
		//Input  
		if ((ret = avformat_open_input(&ifmt_ctx, input_str, 0, 0)) < 0)
		{
			printf("Could not open input file.");
			goto end;
		}
		/*LOGI("avformat_find_stream_info");*/
		if ((ret = avformat_find_stream_info(ifmt_ctx, 0)) < 0)
		{
			printf("error to retrieve input stream information");
			goto end;
		}

		//Output  
		avformat_alloc_output_context2(&ofmt_ctx, NULL, "flv", output_str); //RTMP  
		//avformat_alloc_output_context2(&ofmt_ctx, NULL,"mpegts", output_str);//UDP  
		if (!ofmt_ctx)
		{
			printf("Could not create outputcontext\n");
			ret = AVERROR_UNKNOWN;
			goto end;
		}
		ofmt = ofmt_ctx->oformat;
		int videoindex = -1;
		for (i = 0; i<ifmt_ctx->nb_streams; i++)
			if (ifmt_ctx->streams[i]->codec->codec_type == AVMEDIA_TYPE_VIDEO)
			{
			videoindex = i;
			break;
			}
		//Create output AVStream according to input AVStream  
		AVStream *in_stream = ifmt_ctx->streams[videoindex];
		AVStream *out_stream = avformat_new_stream(ofmt_ctx, in_stream->codec->codec);
		if (!out_stream)
		{
			printf("Error occurred when allocating output stream\n");
			ret = AVERROR_UNKNOWN;
			goto end;
		}
		//Copythe settings of AVCodecContext  
		ret = avcodec_copy_context(out_stream->codec, in_stream->codec);
		if (ret< 0)
		{
			printf("error to copy context from input to output stream codeccontext\n");
			goto end;
		}
		out_stream->codec->codec_tag = 0;
		if (ofmt_ctx->oformat->flags & AVFMT_GLOBALHEADER)
			out_stream->codec->flags |= CODEC_FLAG_GLOBAL_HEADER;

		//Openoutput URL  
		if (!(ofmt->flags & AVFMT_NOFILE))
		{
			ret = avio_open(&ofmt_ctx->pb, output_str, AVIO_FLAG_WRITE);
			if (ret < 0)
			{
				printf("Could not open output URL '%s'", output_str);
				goto end;
			}
		}
		//Writefile header  
		ret = avformat_write_header(ofmt_ctx, NULL);
		if (ret< 0)
		{
			printf("Error occurred when opening output URL : %d\n", ret);
			goto end;
		}

		int frame_index = 0;

		int64_t start_time = av_gettime();
		while (1)
		{
			AVStream *in_stream, *out_stream;
			//Get an AVPacket  
			ret = av_read_frame(ifmt_ctx, &pkt);
			if (ret < 0)
				break;
			if (pkt.stream_index != videoindex)
			{
				av_free_packet(&pkt);
				continue;
			}
			//FIX：NoPTS (Example: Raw H.264)  
			//Simple Write PTS  
			if (pkt.pts == AV_NOPTS_VALUE)
			{
				//Write PTS  
				AVRational time_base1 = ifmt_ctx->streams[videoindex]->time_base;
				//Duration between 2 frames (us)  
				int64_t calc_duration = (double)AV_TIME_BASE / av_q2d(ifmt_ctx->streams[videoindex]->r_frame_rate);
				//Parameters  
				pkt.pts = (double)(frame_index*calc_duration) / (double)(av_q2d(time_base1)*AV_TIME_BASE);
				pkt.dts = pkt.pts;
				pkt.duration = (double)calc_duration / (double)(av_q2d(time_base1)*AV_TIME_BASE);
			}
			//Important:Delay  
			if (pkt.stream_index == videoindex)
			{
				AVRational time_base = ifmt_ctx->streams[videoindex]->time_base;
				AVRational time_base_q = { 1, AV_TIME_BASE };
				int64_t pts_time = av_rescale_q(pkt.dts, time_base, time_base_q);
				int64_t now_time = av_gettime() - start_time;
				if (pts_time > now_time)
					av_usleep(pts_time - now_time);
			}

			in_stream = ifmt_ctx->streams[pkt.stream_index];
			out_stream = ofmt_ctx->streams[pkt.stream_index];
			/*copy packet */
			//Convert PTS/DTS  
			//pkt.pts = av_rescale_q_rnd(pkt.pts, in_stream->time_base, out_stream->time_base, AV_ROUND_NEAR_INF | AV_ROUND_PASS_MINMAX);
			
			if (pkt.pts != AV_NOPTS_VALUE)
				pkt.pts = av_rescale_q(pkt.pts, in_stream->time_base, out_stream->time_base);

			pkt.dts = pkt.pts;//av_rescale_q_rnd(pkt.dts, in_stream->time_base,out_stream->time_base, AV_ROUND_NEAR_INF|AV_ROUND_PASS_MINMAX);  
			pkt.duration = av_rescale_q(pkt.duration, in_stream->time_base, out_stream->time_base);
			pkt.pos = -1;
			//Print to Screen  
			if (pkt.stream_index == videoindex)
			{
				printf("Send %8d video frames to output URL\n", frame_index);
				frame_index++;
			}
			ret = av_interleaved_write_frame(ofmt_ctx, &pkt);

			if (ret < 0)
			{
				printf("Error muxing packet\n");
				break;
			}
			av_free_packet(&pkt);

		}
		//Writefile trailer  
		av_write_trailer(ofmt_ctx);
	end:
		avformat_close_input(&ifmt_ctx);
		/*close output */
		if (ofmt_ctx && !(ofmt->flags & AVFMT_NOFILE))
			avio_close(ofmt_ctx->pb);
		avformat_free_context(ofmt_ctx);
		if (ret< 0 && ret != AVERROR_EOF)
		{
			printf("Error occurred.\n");
			return -1;
		}
		return 0;
	}

protected:
	void av_packet_rescale_ts(AVPacket *pkt, AVRational src_tb, AVRational dst_tb)
	{
		if (pkt->pts != AV_NOPTS_VALUE)
			pkt->pts = av_rescale_q(pkt->pts, src_tb, dst_tb);
		if (pkt->dts != AV_NOPTS_VALUE)
			pkt->dts = av_rescale_q(pkt->dts, src_tb, dst_tb);
		if (pkt->duration > 0)
			pkt->duration = av_rescale_q(pkt->duration, src_tb, dst_tb);
	}
	void DBLog(String^ deviceid, String^ flag, String^ flagTitle)
	{
		if (flag == "1" || flag == "5")
		{
			String^ sql = "update vsg_FIB set pushflowstatus ='" + flag + "'  ,isupdate='1' where deviceid='" + deviceid + "'";
			int result = AccessOperate::AccessHelper::InsertData(sql);
		}
		ToolAPI::XMLOperation::WriteLogXmlNoTail(flagTitle, deviceid);
	}
};