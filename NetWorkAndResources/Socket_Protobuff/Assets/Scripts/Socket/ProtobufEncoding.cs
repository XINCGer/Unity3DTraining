using System;
using System.IO;
using UnityEngine;
using Com.Shapejoy.Remotecontrol.Proto;
using Fitness.SocketClient;

namespace Google.Protobuf
{
    /// <summary>
    /// 编码和解码
    /// </summary>
    public class ProtobufEncoding
    {

        private static ProtobufEncoding instance;
        public static ProtobufEncoding Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new ProtobufEncoding();
                }
                return instance;
            }
        }
        /// <summary>
        /// 将数据编码 长度+内容
        /// </summary>
        /// <param name="data">内容</param>
        //public static byte[] Encode(byte[] data)
        public byte[] Encode(IMessage msg)
        {
            using (MemoryStream rawOutput = new MemoryStream())
            {
                CodedOutputStream output = new CodedOutputStream(rawOutput);
                //output.WriteRawVarint32((uint)len);
                output.WriteMessage(msg);
                output.Flush();
                byte[] result = rawOutput.ToArray();

                return result;
            }
        }

        public IMessage Decode(byte[] msg)
        {
            IMessage message = null;

            CodedInputStream stream = new CodedInputStream(msg);

            //data length,Protobuf 变长头, 也就是消息长度
            int varint32 = (int)stream.ReadInt32();
            if (varint32 <= (msg.Length - (int)stream.Position))
            {
                try
                {
                    byte[] body = stream.ReadRawBytes(varint32);
                    message = Message.Parser.ParseFrom(body);
                }
                catch (Exception exception)
                {
                    //"[ProtobufEncoding] Decode exception :{0}", exception.Message);
                }
            }
            else
            {
               //"网络数据读取不完整，丢包了？");
            }

            return message;
        }

        #region 异步解码
        public class PacketBuffer
        {
            public byte[] Data { get; set; }
            public int Length { get; set; }
            public int Offset { get; set; }
        }
        public PacketBuffer buffer = new PacketBuffer();
        
        public void ResetBuffer()
        {
            buffer = new PacketBuffer();
        }
        /// <summary>
        /// 将数据解码
        /// </summary>
        public void DecodeAsync(byte[] msg)
        {
            if (msg.Length <= 0)
            {
                return;
            }
            if(buffer.Data == null)
            {
                buffer.Data = new byte[8192];
            }
            //把收取上来的自己全部缓存到本地 buffer 中
            Array.Copy(msg, 0, buffer.Data, buffer.Length, msg.Length);
            buffer.Length += msg.Length;

            //string.Format("cache buff length: {0}, offset: {1}", buffer.Length, buffer.Offset));


            CodedInputStream stream = new CodedInputStream(buffer.Data);
            while (!stream.IsAtEnd)
            {
                //标记读取的Position, 在长度不够时进行数组拷贝，到下一次在进行解析
                int markReadIndex = (int)stream.Position;

                //data length,Protobuf 变长头, 也就是消息长度
                int varint32 = (int)stream.ReadInt32();
                if (varint32 <= (buffer.Length - (int)stream.Position))
                {
                    try
                    {
                        byte[] body = stream.ReadRawBytes(varint32);

                        Message message = Message.Parser.ParseFrom(body);
                        
                        SocketManager.Instance.DispatchProto(message);
                        //TODO: dispatcher message, 这里就可以用多线程进行协议分发
                    }
                    catch (Exception exception)
                    {
                        //exception.Message);
                    }
                }
                else
                {
                    //本次数据不够长度,缓存进行下一次解析
                    byte[] dest = new byte[8192];
                    int remainSize = buffer.Length - markReadIndex;
                    Array.Copy(buffer.Data, markReadIndex, dest, 0, remainSize);

                    /**
                     * 缓存未处理完的字节 
                     */
                    buffer.Data = dest;
                    buffer.Offset = 0;
                    buffer.Length = remainSize;
                    break;
                }
            }
        }
        #endregion
    }
}
