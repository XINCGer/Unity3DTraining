using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Google.Protobuf;

namespace Fitness.SocketClient
{
    public class ProtoDic
    {
        private static ProtoDic instance;
        public static ProtoDic Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new ProtoDic();
                }
                return instance;
            }
        }

        private Dictionary<int,Type> _protoType = new Dictionary<int,Type>();
        private Dictionary<RuntimeTypeHandle, MessageParser> Parsers = new Dictionary<RuntimeTypeHandle, MessageParser>();


        public bool AddProtoType(int id,Type type)
        {
            if (!_protoType.ContainsKey(id))
            {
                _protoType.Add(id, type);
            }
            return false;
        }
        public bool AddMessageParser(RuntimeTypeHandle typeHandle, MessageParser messageParser)
        {
            if (!Parsers.ContainsKey(typeHandle))
            {
                Parsers.Add(typeHandle, messageParser);
                return true;
            }
            return false;
        }
        public MessageParser GetMessageParser(RuntimeTypeHandle typeHandle)
        {
            MessageParser messageParser;
            Parsers.TryGetValue(typeHandle, out messageParser);
            return messageParser;
        }

        public Type GetProtoTypeByProtoId(int protoId)
        {
            Type type;
            _protoType.TryGetValue(protoId,out type);
            return type;
        }

        public bool ContainProtoId(int protoId)
        {
            if (_protoType.ContainsKey(protoId))
            {
                return true;
            }
            return false;
        }

        public bool ContainProtoType(Type type)
        {
            if (_protoType.ContainsValue(type))
            {
                return true;
            }
            return false;
        }
    }
}
