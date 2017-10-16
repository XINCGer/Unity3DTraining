--
--------------------------------------------------------------------------------
--  FILE:  protobuf.lua
--  DESCRIPTION:  protoc-gen-lua
--      Google's Protocol Buffers project, ported to lua.
--      https://code.google.com/p/protoc-gen-lua/
--
--      Copyright (c) 2010 , 林卓毅 (Zhuoyi Lin) netsnail@gmail.com
--      All rights reserved.
--
--      Use, modification and distribution are subject to the "New BSD License"
--      as listed at <url: http://www.opensource.org/licenses/bsd-license.php >.
--
--  COMPANY:  NetEase
--  CREATED:  2010年07月29日 14时30分02秒 CST
--------------------------------------------------------------------------------
--

local setmetatable = setmetatable
local rawset = rawset
local rawget = rawget
local error = error
local ipairs = ipairs
local pairs = pairs
local print = print
local table = table 
local string = string
local tostring = tostring
local type = type

local pb = require "pb"
local wire_format = require "protobuf.wire_format"
local type_checkers = require "protobuf.type_checkers"
local encoder = require "protobuf.encoder"
local decoder = require "protobuf.decoder"
local listener_mod = require "protobuf.listener"
local containers = require "protobuf.containers"
local descriptor = require "protobuf.descriptor"
local FieldDescriptor = descriptor.FieldDescriptor
local text_format = require "protobuf.text_format"

module("protobuf.protobuf")

local function make_descriptor(name, descriptor, usable_key)
    local meta = {
        __newindex = function(self, key, value)
            if usable_key[key] then
                rawset(self, key, value)
            else
                error("error key: "..key)
            end
        end
    };
    meta.__index = meta
    meta.__call = function()
        return setmetatable({}, meta)
    end

    _M[name] = setmetatable(descriptor, meta);
end


make_descriptor("Descriptor",  {}, {
    name = true,
    full_name = true,
    filename = true,
    containing_type = true,
    fields = true,
    nested_types = true,
    enum_types = true,
    extensions = true,
    options = true,
    is_extendable = true,
    extension_ranges = true,
})

make_descriptor("FieldDescriptor", FieldDescriptor, {
    name = true,
    full_name = true,
    index = true,
    number = true,
    type = true,
    cpp_type = true,
    label = true,
    has_default_value = true,
    default_value = true,
    containing_type = true,
    message_type = true,
    enum_type = true,
    is_extension = true,
    extension_scope = true,
})

make_descriptor("EnumDescriptor", {}, {
    name = true,
    full_name = true,
    values = true,
    containing_type  = true,
    options = true
})

make_descriptor("EnumValueDescriptor", {}, {
    name = true,
    index = true,
    number = true,
    type = true,
    options = true
})

-- Maps from field type to expected wiretype.
local FIELD_TYPE_TO_WIRE_TYPE = {
    [FieldDescriptor.TYPE_DOUBLE] = wire_format.WIRETYPE_FIXED64,
    [FieldDescriptor.TYPE_FLOAT] = wire_format.WIRETYPE_FIXED32,
    [FieldDescriptor.TYPE_INT64] = wire_format.WIRETYPE_VARINT,
    [FieldDescriptor.TYPE_UINT64] = wire_format.WIRETYPE_VARINT,
    [FieldDescriptor.TYPE_INT32] = wire_format.WIRETYPE_VARINT,
    [FieldDescriptor.TYPE_FIXED64] = wire_format.WIRETYPE_FIXED64,
    [FieldDescriptor.TYPE_FIXED32] = wire_format.WIRETYPE_FIXED32,
    [FieldDescriptor.TYPE_BOOL] = wire_format.WIRETYPE_VARINT,
    [FieldDescriptor.TYPE_STRING] = wire_format.WIRETYPE_LENGTH_DELIMITED,
    [FieldDescriptor.TYPE_GROUP] = wire_format.WIRETYPE_START_GROUP,
    [FieldDescriptor.TYPE_MESSAGE] = wire_format.WIRETYPE_LENGTH_DELIMITED,
    [FieldDescriptor.TYPE_BYTES] = wire_format.WIRETYPE_LENGTH_DELIMITED,
    [FieldDescriptor.TYPE_UINT32] = wire_format.WIRETYPE_VARINT,
    [FieldDescriptor.TYPE_ENUM] = wire_format.WIRETYPE_VARINT,
    [FieldDescriptor.TYPE_SFIXED32] = wire_format.WIRETYPE_FIXED32,
    [FieldDescriptor.TYPE_SFIXED64] = wire_format.WIRETYPE_FIXED64,
    [FieldDescriptor.TYPE_SINT32] = wire_format.WIRETYPE_VARINT,
    [FieldDescriptor.TYPE_SINT64] = wire_format.WIRETYPE_VARINT
}

local NON_PACKABLE_TYPES = {
    [FieldDescriptor.TYPE_STRING] = true,
    [FieldDescriptor.TYPE_GROUP] = true,
    [FieldDescriptor.TYPE_MESSAGE] = true,
    [FieldDescriptor.TYPE_BYTES] = true
}

local _VALUE_CHECKERS = {
    [FieldDescriptor.CPPTYPE_INT32] = type_checkers.Int32ValueChecker(),
    [FieldDescriptor.CPPTYPE_INT64] = type_checkers.TypeChecker({string = true, number = true}),
    [FieldDescriptor.CPPTYPE_UINT32] = type_checkers.Uint32ValueChecker(),
    [FieldDescriptor.CPPTYPE_UINT64] = type_checkers.TypeChecker({string = true, number = true}),
    [FieldDescriptor.CPPTYPE_DOUBLE] = type_checkers.TypeChecker({number = true}),
    [FieldDescriptor.CPPTYPE_FLOAT] = type_checkers.TypeChecker({number = true}),
    [FieldDescriptor.CPPTYPE_BOOL] = type_checkers.TypeChecker({boolean = true, bool = true, int=true}),
    [FieldDescriptor.CPPTYPE_ENUM] = type_checkers.Int32ValueChecker(),
    [FieldDescriptor.CPPTYPE_STRING] = type_checkers.TypeChecker({string = true})
}


local TYPE_TO_BYTE_SIZE_FN = {
    [FieldDescriptor.TYPE_DOUBLE] = wire_format.DoubleByteSize,
    [FieldDescriptor.TYPE_FLOAT] = wire_format.FloatByteSize,
    [FieldDescriptor.TYPE_INT64] = wire_format.Int64ByteSize,
    [FieldDescriptor.TYPE_UINT64] = wire_format.UInt64ByteSize,
    [FieldDescriptor.TYPE_INT32] = wire_format.Int32ByteSize,
    [FieldDescriptor.TYPE_FIXED64] = wire_format.Fixed64ByteSize,
    [FieldDescriptor.TYPE_FIXED32] = wire_format.Fixed32ByteSize,
    [FieldDescriptor.TYPE_BOOL] = wire_format.BoolByteSize,
    [FieldDescriptor.TYPE_STRING] = wire_format.StringByteSize,
    [FieldDescriptor.TYPE_GROUP] = wire_format.GroupByteSize,
    [FieldDescriptor.TYPE_MESSAGE] = wire_format.MessageByteSize,
    [FieldDescriptor.TYPE_BYTES] = wire_format.BytesByteSize,
    [FieldDescriptor.TYPE_UINT32] = wire_format.UInt32ByteSize,
    [FieldDescriptor.TYPE_ENUM] = wire_format.EnumByteSize,
    [FieldDescriptor.TYPE_SFIXED32] = wire_format.SFixed32ByteSize,
    [FieldDescriptor.TYPE_SFIXED64] = wire_format.SFixed64ByteSize,
    [FieldDescriptor.TYPE_SINT32] = wire_format.SInt32ByteSize,
    [FieldDescriptor.TYPE_SINT64] = wire_format.SInt64ByteSize
}

local TYPE_TO_ENCODER = {
    [FieldDescriptor.TYPE_DOUBLE] = encoder.DoubleEncoder,
    [FieldDescriptor.TYPE_FLOAT] = encoder.FloatEncoder,
    [FieldDescriptor.TYPE_INT64] = encoder.Int64Encoder,
    [FieldDescriptor.TYPE_UINT64] = encoder.UInt64Encoder,
    [FieldDescriptor.TYPE_INT32] = encoder.Int32Encoder,
    [FieldDescriptor.TYPE_FIXED64] = encoder.Fixed64Encoder,
    [FieldDescriptor.TYPE_FIXED32] = encoder.Fixed32Encoder,
    [FieldDescriptor.TYPE_BOOL] = encoder.BoolEncoder,
    [FieldDescriptor.TYPE_STRING] = encoder.StringEncoder,
    [FieldDescriptor.TYPE_GROUP] = encoder.GroupEncoder,
    [FieldDescriptor.TYPE_MESSAGE] = encoder.MessageEncoder,
    [FieldDescriptor.TYPE_BYTES] = encoder.BytesEncoder,
    [FieldDescriptor.TYPE_UINT32] = encoder.UInt32Encoder,
    [FieldDescriptor.TYPE_ENUM] = encoder.EnumEncoder,
    [FieldDescriptor.TYPE_SFIXED32] = encoder.SFixed32Encoder,
    [FieldDescriptor.TYPE_SFIXED64] = encoder.SFixed64Encoder,
    [FieldDescriptor.TYPE_SINT32] = encoder.SInt32Encoder,
    [FieldDescriptor.TYPE_SINT64] = encoder.SInt64Encoder
}

local TYPE_TO_SIZER = {
    [FieldDescriptor.TYPE_DOUBLE] = encoder.DoubleSizer,
    [FieldDescriptor.TYPE_FLOAT] = encoder.FloatSizer,
    [FieldDescriptor.TYPE_INT64] = encoder.Int64Sizer,
    [FieldDescriptor.TYPE_UINT64] = encoder.UInt64Sizer,
    [FieldDescriptor.TYPE_INT32] = encoder.Int32Sizer,
    [FieldDescriptor.TYPE_FIXED64] = encoder.Fixed64Sizer,
    [FieldDescriptor.TYPE_FIXED32] = encoder.Fixed32Sizer,
    [FieldDescriptor.TYPE_BOOL] = encoder.BoolSizer,
    [FieldDescriptor.TYPE_STRING] = encoder.StringSizer,
    [FieldDescriptor.TYPE_GROUP] = encoder.GroupSizer,
    [FieldDescriptor.TYPE_MESSAGE] = encoder.MessageSizer,
    [FieldDescriptor.TYPE_BYTES] = encoder.BytesSizer,
    [FieldDescriptor.TYPE_UINT32] = encoder.UInt32Sizer,
    [FieldDescriptor.TYPE_ENUM] = encoder.EnumSizer,
    [FieldDescriptor.TYPE_SFIXED32] = encoder.SFixed32Sizer,
    [FieldDescriptor.TYPE_SFIXED64] = encoder.SFixed64Sizer,
    [FieldDescriptor.TYPE_SINT32] = encoder.SInt32Sizer,
    [FieldDescriptor.TYPE_SINT64] = encoder.SInt64Sizer
}

local TYPE_TO_DECODER = {
    [FieldDescriptor.TYPE_DOUBLE] = decoder.DoubleDecoder,
    [FieldDescriptor.TYPE_FLOAT] = decoder.FloatDecoder,
    [FieldDescriptor.TYPE_INT64] = decoder.Int64Decoder,
    [FieldDescriptor.TYPE_UINT64] = decoder.UInt64Decoder,
    [FieldDescriptor.TYPE_INT32] = decoder.Int32Decoder,
    [FieldDescriptor.TYPE_FIXED64] = decoder.Fixed64Decoder,
    [FieldDescriptor.TYPE_FIXED32] = decoder.Fixed32Decoder,
    [FieldDescriptor.TYPE_BOOL] = decoder.BoolDecoder,
    [FieldDescriptor.TYPE_STRING] = decoder.StringDecoder,
    [FieldDescriptor.TYPE_GROUP] = decoder.GroupDecoder,
    [FieldDescriptor.TYPE_MESSAGE] = decoder.MessageDecoder,
    [FieldDescriptor.TYPE_BYTES] = decoder.BytesDecoder,
    [FieldDescriptor.TYPE_UINT32] = decoder.UInt32Decoder,
    [FieldDescriptor.TYPE_ENUM] = decoder.EnumDecoder,
    [FieldDescriptor.TYPE_SFIXED32] = decoder.SFixed32Decoder,
    [FieldDescriptor.TYPE_SFIXED64] = decoder.SFixed64Decoder,
    [FieldDescriptor.TYPE_SINT32] = decoder.SInt32Decoder,
    [FieldDescriptor.TYPE_SINT64] = decoder.SInt64Decoder
}

local FIELD_TYPE_TO_WIRE_TYPE = {
    [FieldDescriptor.TYPE_DOUBLE] = wire_format.WIRETYPE_FIXED64,
    [FieldDescriptor.TYPE_FLOAT] = wire_format.WIRETYPE_FIXED32,
    [FieldDescriptor.TYPE_INT64] = wire_format.WIRETYPE_VARINT,
    [FieldDescriptor.TYPE_UINT64] = wire_format.WIRETYPE_VARINT,
    [FieldDescriptor.TYPE_INT32] = wire_format.WIRETYPE_VARINT,
    [FieldDescriptor.TYPE_FIXED64] = wire_format.WIRETYPE_FIXED64,
    [FieldDescriptor.TYPE_FIXED32] = wire_format.WIRETYPE_FIXED32,
    [FieldDescriptor.TYPE_BOOL] = wire_format.WIRETYPE_VARINT,
    [FieldDescriptor.TYPE_STRING] = wire_format.WIRETYPE_LENGTH_DELIMITED,
    [FieldDescriptor.TYPE_GROUP] = wire_format.WIRETYPE_START_GROUP,
    [FieldDescriptor.TYPE_MESSAGE] = wire_format.WIRETYPE_LENGTH_DELIMITED,
    [FieldDescriptor.TYPE_BYTES] = wire_format.WIRETYPE_LENGTH_DELIMITED,
    [FieldDescriptor.TYPE_UINT32] = wire_format.WIRETYPE_VARINT,
    [FieldDescriptor.TYPE_ENUM] = wire_format.WIRETYPE_VARINT,
    [FieldDescriptor.TYPE_SFIXED32] = wire_format.WIRETYPE_FIXED32,
    [FieldDescriptor.TYPE_SFIXED64] = wire_format.WIRETYPE_FIXED64,
    [FieldDescriptor.TYPE_SINT32] = wire_format.WIRETYPE_VARINT,
    [FieldDescriptor.TYPE_SINT64] = wire_format.WIRETYPE_VARINT
}

local function IsTypePackable(field_type)
    return NON_PACKABLE_TYPES[field_type] == nil
end

local function GetTypeChecker(cpp_type, field_type)
    if (cpp_type == FieldDescriptor.CPPTYPE_STRING and field_type == FieldDescriptor.TYPE_STRING) then
        return type_checkers.UnicodeValueChecker()
    end
    return _VALUE_CHECKERS[cpp_type]
end

local function _DefaultValueConstructorForField(field)
    if field.label == FieldDescriptor.LABEL_REPEATED then
        if type(field.default_value) ~= "table" or #(field.default_value) ~= 0  then
            error('Repeated field default value not empty list:' .. tostring(field.default_value))
        end
        if field.cpp_type == FieldDescriptor.CPPTYPE_MESSAGE then
            local message_type = field.message_type
            return function (message)
                return containers.RepeatedCompositeFieldContainer(message._listener_for_children, message_type)
            end
        else
            local type_checker = GetTypeChecker(field.cpp_type, field.type)
            return function (message)
                return containers.RepeatedScalarFieldContainer(message._listener_for_children, type_checker)
            end
        end
    end
    if field.cpp_type == FieldDescriptor.CPPTYPE_MESSAGE then
        local message_type = field.message_type
        return function (message)
            result = message_type._concrete_class()
            result._SetListener(message._listener_for_children)
            return result
        end
    end
    return function (message)
        return field.default_value
    end
end

local function _AttachFieldHelpers(message_meta, field_descriptor)
    local is_repeated = (field_descriptor.label == FieldDescriptor.LABEL_REPEATED)
    local is_packed = (field_descriptor.has_options and field_descriptor.GetOptions().packed)

    rawset(field_descriptor, "_encoder", TYPE_TO_ENCODER[field_descriptor.type](field_descriptor.number, is_repeated, is_packed))
    rawset(field_descriptor, "_sizer", TYPE_TO_SIZER[field_descriptor.type](field_descriptor.number, is_repeated, is_packed))
    rawset(field_descriptor, "_default_constructor", _DefaultValueConstructorForField(field_descriptor))

    local AddDecoder = function(wiretype, is_packed)
        local tag_bytes = encoder.TagBytes(field_descriptor.number, wiretype)
        message_meta._decoders_by_tag[tag_bytes] = TYPE_TO_DECODER[field_descriptor.type](field_descriptor.number, is_repeated, is_packed, field_descriptor, field_descriptor._default_constructor)
    end
  
    AddDecoder(FIELD_TYPE_TO_WIRE_TYPE[field_descriptor.type], False)
    if is_repeated and IsTypePackable(field_descriptor.type) then
        AddDecoder(wire_format.WIRETYPE_LENGTH_DELIMITED, True)
    end
end

local function _AddEnumValues(descriptor, message_meta)
    for _, enum_type in ipairs(descriptor.enum_types) do
        for _, enum_value in ipairs(enum_type.values) do
            message_meta._member[enum_value.name] = enum_value.number
        end
    end
end

local function _InitMethod(message_meta)
    return function()
        local self = {}
        self._cached_byte_size = 0
        self._cached_byte_size_dirty = false
        self._fields = {}
        self._is_present_in_parent = false
        self._listener = listener_mod.NullMessageListener()
        self._listener_for_children = listener_mod.Listener(self) 
        return setmetatable(self, message_meta)
    end
end

local function _AddPropertiesForRepeatedField(field, message_meta)
    local property_name = field.name

    message_meta._getter[property_name] = function(self)
        local field_value = self._fields[field]
        if field_value == nil then
            field_value = field._default_constructor(self)
            self._fields[field] = field_value

            if not self._cached_byte_size_dirty then
                message_meta._member._Modified(self)
            end
        end
        return field_value
    end

    message_meta._setter[property_name] = function(self)
        error('Assignment not allowed to repeated field "' .. property_name .. '" in protocol message object.')
    end
end

local function _AddPropertiesForNonRepeatedCompositeField(field, message_meta)
    local property_name = field.name
    local message_type = field.message_type

    message_meta._getter[property_name] = function(self)
        local field_value = self._fields[field]
        if field_value == nil then
            field_value = message_type._concrete_class()
            field_value:_SetListener(self._listener_for_children)            
            self._fields[field] = field_value

            if not self._cached_byte_size_dirty then
                message_meta._member._Modified(self)
            end
        end
        return field_value
    end
    message_meta._setter[property_name] = function(self, new_value)
        error('Assignment not allowed to composite field' .. property_name .. 'in protocol message object.' )
    end
end

local function _AddPropertiesForNonRepeatedScalarField(field, message)
    local property_name = field.name
    local type_checker = GetTypeChecker(field.cpp_type, field.type)
    local default_value = field.default_value

    message._getter[property_name] = function(self)
        local value =  self._fields[field] 
        if value ~= nil then
            return self._fields[field]
        else
            return default_value
        end
    end

    message._setter[property_name] = function(self, new_value)
        type_checker(new_value)
        self._fields[field] = new_value
        if not self._cached_byte_size_dirty then
            message._member._Modified(self)
        end
    end
end

local function _AddPropertiesForField(field, message_meta)
    constant_name = field.name:upper() .. "_FIELD_NUMBER"
    message_meta._member[constant_name] = field.number

    if field.label == FieldDescriptor.LABEL_REPEATED then
        _AddPropertiesForRepeatedField(field, message_meta)
    elseif field.cpp_type == FieldDescriptor.CPPTYPE_MESSAGE then
        _AddPropertiesForNonRepeatedCompositeField(field, message_meta)
    else
        _AddPropertiesForNonRepeatedScalarField(field, message_meta)
    end
end

local _ED_meta = {
    __index = function(self, extension_handle)
        local _extended_message = rawget(self, "_extended_message")
        local value = _extended_message._fields[extension_handle]
        if value ~= nil then
            return value
        end
        if extension_handle.label == FieldDescriptor.LABEL_REPEATED then
            value = extension_handle._default_constructor(self._extended_message)
        elseif extension_handle.cpp_type == FieldDescriptor.CPPTYPE_MESSAGE then
            value = extension_handle.message_type._concrete_class()
            value:_SetListener(_extended_message._listener_for_children)
        else
            return extension_handle.default_value
        end
        _extended_message._fields[extension_handle] = value
        return value
    end,
    __newindex = function(self, extension_handle, value)
        local _extended_message = rawget(self, "_extended_message")
        if (extension_handle.label == FieldDescriptor.LABEL_REPEATED or
            extension_handle.cpp_type == FieldDescriptor.CPPTYPE_MESSAGE) then
            error('Cannot assign to extension "'.. extension_handle.full_name .. '" because it is a repeated or composite type.')
        end

        local type_checker = GetTypeChecker(extension_handle.cpp_type, extension_handle.type)
        type_checker.CheckValue(value)
        _extended_message._fields[extension_handle] = value
        _extended_message._Modified()
    end
}

local function _ExtensionDict(message)
    local o = {}
    o._extended_message = message
    return setmetatable(o, _ED_meta)
end

local function _AddPropertiesForFields(descriptor, message_meta)
    for _, field in ipairs(descriptor.fields) do
        _AddPropertiesForField(field, message_meta)
    end
    if descriptor.is_extendable then
        message_meta._getter.Extensions = function(self) return _ExtensionDict(self) end
    end
end

local function _AddPropertiesForExtensions(descriptor, message_meta)
  local extension_dict = descriptor._extensions_by_name
  for extension_name, extension_field in pairs(extension_dict) do
      local constant_name = string.upper(extension_name) .. "_FIELD_NUMBER"
      message_meta._member[constant_name] = extension_field.number
  end
end

local function _AddStaticMethods(message_meta)
    message_meta._member.RegisterExtension = function(extension_handle)
        extension_handle.containing_type = message_meta._descriptor
        _AttachFieldHelpers(message_meta, extension_handle)

        if message_meta._extensions_by_number[extension_handle.number] == nil then
            message_meta._extensions_by_number[extension_handle.number] = extension_handle
        else
            error(
                string.format('Extensions "%s" and "%s" both try to extend message type "%s" with field number %d.',
                    extension_handle.full_name, actual_handle.full_name,
                    message_meta._descriptor.full_name, extension_handle.number))
        end
        message_meta._extensions_by_name[extension_handle.full_name] = extension_handle
    end

    message_meta._member.FromString = function(s)
        local message = message_meta._member.__call()
        message.MergeFromString(s)
        return message
    end
end

local function _IsPresent(descriptor, value)
    if descriptor.label == FieldDescriptor.LABEL_REPEATED then
        return value
    elseif descriptor.cpp_type == FieldDescriptor.CPPTYPE_MESSAGE then
        return value._is_present_in_parent
    else
        return true
    end
end

function sortFunc(a, b)
    return a.index < b.index
end
function pairsByKeys (t, f)
    local a = {}
    for n in pairs(t) do table.insert(a, n) end
    table.sort(a, f)
    local i = 0                 -- iterator variable
    local iter = function ()    -- iterator function
       i = i + 1
       if a[i] == nil then return nil
       else return a[i], t[a[i]]
       end
    end
    return iter
end

local function _AddListFieldsMethod(message_descriptor, message_meta)
    message_meta._member.ListFields = function (self)
        local list_field = function(fields)
            --local f, s, v = pairs(self._fields)
            local f,s,v = pairsByKeys(self._fields, sortFunc)
            local iter = function(a, i)
                while true do
                    local descriptor, value = f(a, i)
                    if descriptor == nil then
                        return                     
                    elseif _IsPresent(descriptor, value) then
                        return descriptor, value
                    end
                end
            end
            return iter, s, v
        end
        return list_field(self._fields)
    end
end

local function _AddHasFieldMethod(message_descriptor, message_meta)
    local singular_fields = {}
    for _, field in ipairs(message_descriptor.fields) do
        if field.label ~= FieldDescriptor.LABEL_REPEATED then
            singular_fields[field.name] = field
        end
    end
    message_meta._member.HasField = function (self, field_name)
        field = singular_fields[field_name]
        if field == nil then
            error('Protocol message has no singular "'.. field_name.. '" field.')
        end
        if field.cpp_type == FieldDescriptor.CPPTYPE_MESSAGE then
            value = self._fields[field]
            return value ~= nil  and value._is_present_in_parent
        else
            local valueTmp =  self._fields[field]
            return valueTmp ~= nil
        end
    end
end

local function _AddClearFieldMethod(message_descriptor, message_meta)
	local singular_fields = {}
    for _, field in ipairs(message_descriptor.fields) do
        if field.label ~= FieldDescriptor.LABEL_REPEATED then
            singular_fields[field.name] = field
        end
    end

    message_meta._member.ClearField = function(self, field_name)
		field = singular_fields[field_name]
		if field == nil then
				error('Protocol message has no singular "'.. field_name.. '" field.')
		end

		if self._fields[field] then
				self._fields[field] = nil
		end
		message_meta._member._Modified(self)
	end
end

local function _AddClearExtensionMethod(message_meta)
    message_meta._member.ClearExtension = function(self, extension_handle)
        if self._fields[extension_handle] == nil then
            self._fields[extension_handle] = nil
        end
        message_meta._member._Modified(self)
    end
end

local function _AddClearMethod(message_descriptor, message_meta)
    message_meta._member.Clear = function(self)
        self._fields = {}
        message_meta._member._Modified(self)
    end
end

local function _AddStrMethod(message_meta)
    local format = text_format.msg_format
    message_meta.__tostring = function(self)
        return format(self)    
    end
end

local function _AddHasExtensionMethod(message_meta)
    message_meta._member.HasExtension = function(self, extension_handle)
        if extension_handle.label == FieldDescriptor.LABEL_REPEATED then
            error(extension_handle.full_name .. ' is repeated.')
        end
        if extension_handle.cpp_type == FieldDescriptor.CPPTYPE_MESSAGE then
            value = self._fields[extension_handle]
            return value ~= nil and value._is_present_in_parent
        else
            return self._fields[extension_handle]
        end
    end
end

local function _AddSetListenerMethod(message_meta)
    message_meta._member._SetListener = function(self, listener)
        if listener ~= nil then
            self._listener = listener_mod.NullMessageListener()
        else
            self._listener = listener
        end
    end
end

local function _AddByteSizeMethod(message_descriptor, message_meta)
    message_meta._member.ByteSize = function(self)
        --kaiser
        --bug:这里在Repeat字段的结构体如果第一个字段不是int变量会产生_cached_byte_size_dirty为false而导致byte size为0
        --如果bytesize为0让它强制计算byte size
        if not self._cached_byte_size_dirty and self._cached_byte_size > 0 then
            return self._cached_byte_size
        end
        local size = 0
        for field_descriptor, field_value in message_meta._member.ListFields(self) do
            size = field_descriptor._sizer(field_value) + size
        end
        self._cached_byte_size = size
        self._cached_byte_size_dirty = false
        self._listener_for_children.dirty = false
        return size
    end
end

local function _AddSerializeToStringMethod(message_descriptor, message_meta)
    message_meta._member.SerializeToString = function(self)
        if not message_meta._member.IsInitialized(self) then
            error('Message is missing required fields: ' .. 
                table.concat(message_meta._member.FindInitializationErrors(self), ','))
        end
        return message_meta._member.SerializePartialToString(self)
    end
    message_meta._member.SerializeToIOString = function(self, iostring)
        if not message_meta._member.IsInitialized(self) then
            error('Message is missing required fields: ' .. 
                table.concat(message_meta._member.FindInitializationErrors(self), ','))
        end
        return message_meta._member.SerializePartialToIOString(self, iostring)
    end
end

local function _AddSerializePartialToStringMethod(message_descriptor, message_meta)
    local concat = table.concat
    local _internal_serialize = function(self, write_bytes)
        for field_descriptor, field_value in message_meta._member.ListFields(self) do
            field_descriptor._encoder(write_bytes, field_value)
        end
    end

    local _serialize_partial_to_iostring = function(self, iostring)
        local w = iostring.write
        local write = function(value)
            w(iostring, value) 
        end
        _internal_serialize(self, write)
        return 
    end

    local _serialize_partial_to_string = function(self)
        local out = {}
        local write = function(value)
            out[#out + 1] = value
        end
        _internal_serialize(self, write)
        return concat(out)
    end

    message_meta._member._InternalSerialize = _internal_serialize
    message_meta._member.SerializePartialToIOString = _serialize_partial_to_iostring
    message_meta._member.SerializePartialToString = _serialize_partial_to_string
end



local function _AddMergeFromStringMethod(message_descriptor, message_meta)
    local ReadTag = decoder.ReadTag
    local SkipField = decoder.SkipField
    local decoders_by_tag = message_meta._decoders_by_tag

    local _internal_parse = function(self, buffer, pos, pend)
        message_meta._member._Modified(self)
        local field_dict = self._fields
        local tag_bytes, new_pos 
        local field_decoder
        while pos ~= pend do
            tag_bytes, new_pos = ReadTag(buffer, pos)
            field_decoder = decoders_by_tag[tag_bytes]
            if field_decoder == nil then
                new_pos = SkipField(buffer, new_pos, pend, tag_bytes)
                if new_pos == -1 then
                    return pos
                end
                pos = new_pos
            else
                pos = field_decoder(buffer, new_pos, pend, self, field_dict)
            end
        end
        return pos
    end
    message_meta._member._InternalParse = _internal_parse 

    local merge_from_string = function(self, serialized)
        local length = #serialized
        if _internal_parse(self, serialized, 0, length) ~= length then
            error('Unexpected end-group tag.')
        end
        return length 
    end
    message_meta._member.MergeFromString = merge_from_string

    message_meta._member.ParseFromString = function(self, serialized)
        message_meta._member.Clear(self)
        merge_from_string(self, serialized)
    end
end

local function _AddIsInitializedMethod(message_descriptor, message_meta)
    local required_fields = {}
    for _, field in ipairs(message_descriptor.fields) do
        if field.label == FieldDescriptor.LABEL_REQUIRED then
            required_fields[#required_fields + 1] = field
        end
    end

    message_meta._member.IsInitialized = function(self, errors)
        for _, field in ipairs(required_fields) do
            if self._fields[field] == nil or 
                (field.cpp_type == FieldDescriptor.CPPTYPE_MESSAGE and not self._fields[field]._is_present_in_parent) then
                if errors ~= nil then
                    errors[#errors + 1] = message_meta._member.FindInitializationErrors(self)
                end
                return false
            end
        end

        for field, value in pairs(self._fields) do
            if field.cpp_type == FieldDescriptor.CPPTYPE_MESSAGE then
                if field.label == FieldDescriptor.LABEL_REPEATED then
                    for _, element in ipairs(value) do
                        if not element:IsInitialized() then
                            if errors ~= nil then
                                errors[#errors + 1] = message_meta._member.FindInitializationErrors(self)
                            end
                            return false
                        end
                    end
                elseif value._is_present_in_parent and not value:IsInitialized() then
                    if errors ~= nil then
                        errors[#errors + 1] = message_meta._member.FindInitializationErrors(self)
                    end
                    return false
                end
            end
        end
        return true
    end

    message_meta._member.FindInitializationErrors = function(self)
        local errors = {}

        for _,field in ipairs(required_fields) do
            if not message_meta._member.HasField(self, field.name) then
                errors[#errors + 1] = field.name
            end
        end

        for field, value in message_meta._member.ListFields(self) do
            if field.cpp_type == FieldDescriptor.CPPTYPE_MESSAGE then
                if field.is_extension then
                    name = string.format("(%s)", field.full_name)
                else
                    name = field.name
                end
                if field.label == FieldDescriptor.LABEL_REPEATED then
                    for i, element in ipairs(value) do
                        prefix = string.format("%s[%d].", name, i)
                        sub_errors = element:FindInitializationErrors()
                        for _, e in ipairs(sub_errors) do
                            errors[#errors + 1] = prefix .. e
                        end
                    end
                else
                    prefix = name .. "."
                    sub_errors = value:FindInitializationErrors()
                    for _, e in ipairs(sub_errors) do
                        errors[#errors + 1] = prefix .. e
                    end
                end
            end
        end
        return errors
    end
end

local function _AddMergeFromMethod(message_meta)
    local LABEL_REPEATED = FieldDescriptor.LABEL_REPEATED
    local CPPTYPE_MESSAGE = FieldDescriptor.CPPTYPE_MESSAGE

    message_meta._member.MergeFrom = function (self, msg)
        assert(msg ~= self)
        message_meta._member._Modified(self)

        local fields = self._fields

        for field, value in pairs(msg._fields) do
            if field.label == LABEL_REPEATED or field.cpp_type == CPPTYPE_MESSAGE then
                field_value = fields[field]
                if field_value == nil then
                    field_value = field._default_constructor(self)
                    fields[field] = field_value
                end
                field_value:MergeFrom(value)
            else
                self._fields[field] = value
            end
        end
    end
end

local function _AddMessageMethods(message_descriptor, message_meta)
    _AddListFieldsMethod(message_descriptor, message_meta)
    _AddHasFieldMethod(message_descriptor, message_meta)
    _AddClearFieldMethod(message_descriptor, message_meta)
    if message_descriptor.is_extendable then
        _AddClearExtensionMethod(message_meta)
        _AddHasExtensionMethod(message_meta)
    end
    _AddClearMethod(message_descriptor, message_meta)
--    _AddEqualsMethod(message_descriptor, message_meta)
    _AddStrMethod(message_meta)
    _AddSetListenerMethod(message_meta)
    _AddByteSizeMethod(message_descriptor, message_meta)
    _AddSerializeToStringMethod(message_descriptor, message_meta)
    _AddSerializePartialToStringMethod(message_descriptor, message_meta)
    _AddMergeFromStringMethod(message_descriptor, message_meta)
    _AddIsInitializedMethod(message_descriptor, message_meta)
    _AddMergeFromMethod(message_meta)
end

local function _AddPrivateHelperMethods(message_meta)
    local Modified = function (self)
        if not self._cached_byte_size_dirty then
            self._cached_byte_size_dirty = true
            self._listener_for_children.dirty = true
            self._is_present_in_parent = true
            self._listener:Modified()
        end
    end
    message_meta._member._Modified = Modified
    message_meta._member.SetInParent = Modified
end

local function property_getter(message_meta)
    local getter = message_meta._getter
    local member = message_meta._member
	
    return function (self, property)
		local g = getter[property]
		if g then
		    return g(self)
		else
		    return member[property]
		end
	end
end

local function property_setter(message_meta)
	local setter = message_meta._setter

	return function (self, property, value)
		local s = setter[property]
		if s then
			s(self, value)
		else
			error(property .. " not found")
		end
	end
end

function _AddClassAttributesForNestedExtensions(descriptor, message_meta)
    local extension_dict = descriptor._extensions_by_name
    for extension_name, extension_field in pairs(extension_dict) do
        message_meta._member[extension_name] = extension_field
    end
end

local function Message(descriptor)
    local message_meta = {}
    message_meta._decoders_by_tag = {}
    rawset(descriptor, "_extensions_by_name", {})
    for _, k in ipairs(descriptor.extensions) do
        descriptor._extensions_by_name[k.name] = k
    end
    rawset(descriptor, "_extensions_by_number", {})
    for _, k in ipairs(descriptor.extensions) do
        descriptor._extensions_by_number[k.number] = k
    end
    message_meta._descriptor = descriptor
    message_meta._extensions_by_name = {}
    message_meta._extensions_by_number = {}

    message_meta._getter = {}
    message_meta._setter = {}
    message_meta._member = {}
--    message_meta._name = descriptor.full_name

    local ns = setmetatable({}, message_meta._member)
    message_meta._member.__call = _InitMethod(message_meta)
    message_meta._member.__index = message_meta._member 
    message_meta._member.type = ns

    if rawget(descriptor, "_concrete_class") == nil then
        rawset(descriptor, "_concrete_class", ns)
        for k, field in ipairs(descriptor.fields) do  
            _AttachFieldHelpers(message_meta, field)
        end
    end
    _AddEnumValues(descriptor, message_meta)
    _AddClassAttributesForNestedExtensions(descriptor, message_meta)
    _AddPropertiesForFields(descriptor, message_meta)
    _AddPropertiesForExtensions(descriptor, message_meta)
    _AddStaticMethods(message_meta)
    _AddMessageMethods(descriptor, message_meta)
    _AddPrivateHelperMethods(message_meta)

    message_meta.__index = property_getter(message_meta)
    message_meta.__newindex = property_setter(message_meta) 

    return ns 
end

_M.Message = Message

