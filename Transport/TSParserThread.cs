﻿using System;
using System.Threading;

namespace opentuner
{
    public class TSParserThread
    {
        public const uint MAX_PID = 8192;

        public const byte TS_PACKET_SIZE = 188;
        public const byte TS_HEADER_SYNC = 0x47;

        public const ushort TS_PID_PAT = 0x0000;
        public const ushort TS_PID_SDT = 0x0011;
        public const ushort TS_PID_NULL = 0x1FFF;

        public const byte TS_TABLE_PAT = 0x00;
        public const byte TS_TABLE_PMT = 0x02;
        public const byte TS_TABLE_SDT = 0x42;

        TSDataCallback ts_data_callback = null;

        //ConcurrentQueue<byte> parser_ts_data_queue = null;
        //CircularBuffer parser_ts_data_queue = null;

        CircularBuffer parser_ts_data_queue = new CircularBuffer(GlobalDefines.CircularBufferStartingCapacity);


        //public TSParserThread(TSDataCallback _ts_data_callback, CircularBuffer _parser_ts_data_queue)
        public TSParserThread(TSDataCallback _ts_data_callback, TSThread TransportThread)
        {
            ts_data_callback = _ts_data_callback;
            TransportThread.RegisterTSConsumer(parser_ts_data_queue);
        }

        public void worker_thread()
        {
            uint ts_packet_total_count = 0;
            uint ts_packet_null_count = 0;
            uint ts_invalid_packet_count = 0;

            string prevServiceName = "";
            string prevServiceProvider = "";

            try
            {
                while (true)
                {
                    int ts_data_count = parser_ts_data_queue.Count;

                    if (ts_data_count > TS_PACKET_SIZE)
                    {
                        byte check = 0;

                        if (parser_ts_data_queue.Count > 0)
                        {
                            check = parser_ts_data_queue.Peek();

                            if (check == TS_HEADER_SYNC)
                            {
                                // get the complete packet
                                byte[] ts_packet = new byte[TS_PACKET_SIZE];

                                int byte_counter = 0;

                                while (byte_counter < TS_PACKET_SIZE)
                                {
                                    byte data;

                                    //if (parser_ts_data_queue.TryDequeue(out data))
                                    if (parser_ts_data_queue.Count > 0)
                                    {
                                        data = parser_ts_data_queue.Dequeue();
                                        ts_packet[byte_counter++] = data;
                                    }

                                }

                                //Console.WriteLine("TS Packet Received");

                                // parse ts packet
                                ts_packet_total_count += 1;

                                UInt32 ts_pid = (UInt32)((ts_packet[1] & 0x1F) << 8) | (UInt32)ts_packet[2];

                                //Console.WriteLine("TS Pid: " + ts_pid.ToString("X"));

                                UInt32 ts_adaption_field_flag = (UInt32)(ts_packet[3] & 0x20) >> 5;

                                byte ts_payload_content_offset = 4;
                                byte ts_adaption_field_length = 0;

                                if (ts_adaption_field_flag > 0)
                                {
                                    ts_adaption_field_length = ts_packet[4];

                                    if (ts_adaption_field_length == 0 || ts_adaption_field_length > 183)
                                    {
                                        //Console.WriteLine("Length Invalid: Packet likely Invalid");
                                        ts_invalid_packet_count += 1;
                                        continue;
                                    }

                                }

                                ts_payload_content_offset += ts_adaption_field_length;

                                if (ts_pid == TS_PID_NULL)
                                {
                                    //Console.WriteLine("Null Packet");
                                    ts_packet_null_count += 1;
                                    continue;
                                }

                                if (ts_pid == TS_PID_SDT)   // service description table
                                {
                                    //Console.WriteLine("Payload Data: " + ts_payload_content_offset.ToString());

                                    int ts_payload_offset = ts_payload_content_offset + 1 + ts_packet[ts_payload_content_offset];

                                    string hex_data = "";

                                    //  temp debug
                                    /*
                                    int hex_count = 0;

                                    for (int c = 0; c < ts_packet.Length;c++)
                                    {
                                        hex_data += " " + ts_packet[c].ToString("X");
                                        hex_count += 1;

                                        if (hex_count > 10)
                                        {
                                            hex_count = 0;
                                            hex_data += "\n";
                                        }
                                    }

                                    Console.WriteLine(hex_data);
                                    */

                                    if (ts_packet[ts_payload_offset] != TS_TABLE_SDT)
                                    {
                                        continue;
                                    }

                                    UInt32 ts_payload_section_length = ((UInt32)(ts_packet[ts_payload_offset + 1] & 0x0F) << 8) | (UInt32)ts_packet[ts_payload_offset + 2];

                                    if (ts_payload_section_length < 1)
                                    {
                                        continue;
                                    }

                                    //Console.WriteLine("SDT Section Len: " + ts_payload_section_length);

                                    Int32 ts_service_provider_name_length = ts_packet[ts_payload_offset + 19];

                                    string service_provider = "";
                                    try
                                    {
                                        service_provider = System.Text.Encoding.ASCII.GetString(ts_packet, ts_payload_offset + 19 + 1, ts_service_provider_name_length);
                                    }
                                    catch (Exception Ex)
                                    { }

                                    //Console.WriteLine(service_provider);

                                    Int32 ts_service_name_length = ts_packet[ts_payload_offset + 19 + ts_service_provider_name_length + 1];

                                    string service_provider_name = "";

                                    try
                                    {
                                        service_provider_name = System.Text.Encoding.ASCII.GetString(ts_packet, ts_payload_offset + 19 + ts_service_provider_name_length + 2, ts_service_name_length);
                                    }
                                    catch (Exception Ex)
                                    {

                                    }

                                    //Console.WriteLine(service_provider);
                                    //Console.WriteLine(service_provider_name);

                                    // temp hack to reset null counters
                                    if (prevServiceName != service_provider_name || prevServiceProvider != service_provider)
                                    {
                                        ts_packet_total_count = 0;
                                        ts_packet_null_count = 0;

                                        prevServiceName = service_provider_name;
                                        prevServiceProvider = service_provider;
                                    }


                                    if (ts_data_callback != null)
                                    {
                                        TSStatus new_status = new TSStatus();
                                        new_status.ServiceName = service_provider_name;
                                        new_status.ServiceProvider = service_provider;
                                        new_status.TotalTSPackets = ts_packet_total_count;

                                        if (ts_packet_total_count > 0)
                                        {
                                            new_status.NullPacketsPerc = Convert.ToUInt32((Convert.ToDouble(ts_packet_null_count) / Convert.ToDouble(ts_packet_total_count)) * 100);
                                        }

                                        ts_data_callback(new_status);
                                    }

                                }

                            }
                            else
                            {
                                // remove the byte and continue
                                
                                check = parser_ts_data_queue.Dequeue();
                                continue;
                            }
                        }
                    }
                    else
                    {
                        Thread.Sleep(100);
                    }


                }

            }
            catch (ThreadAbortException)
            {
                Console.WriteLine("TS Thread: Closing ");
            }
            finally
            {
                Console.WriteLine("Closing TS");
            }

        }
    }
}