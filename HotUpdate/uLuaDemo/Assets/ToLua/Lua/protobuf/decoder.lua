--
--------------------------------------------------------------------------------
--  FILE:  decoder.lua
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
--  CREATED:  2010年07月29日 19时30分51秒 CST
--------------------------------------------------------------------------------
--
local string = string
local table = table
local assert = assert
local ipairs = ipairs
local error = error
local print = print

local pb = require "pb"
local encoder = require "protobuf.encoder"
local wire_format = require "protobuf.wire_format"
module "protobuf.decoder"

local _DecodeVarint = pb.varint_decoder
local _DecodeSignedVarint = pb.signed_varint_decoder

local _DecodeVarint32 = pb.varint_decoder
local _DecodeSignedVarint32 = pb.signed_varint_decoder

local _DecodeVarint64 = pb.varint_decoder64
local _DecodeSignedVarint64 = pb.signed_varint_decoder64

ReadTag = pb.read_tag

local function _SimpleDecoder(wire_type, decode_value)
    return function(field_number, is_repeated, is_packed, key, new_default)
        if is_packed then
            local DecodeVarint = _DecodeVarint
            return function (buffer, pos, pend, message, field_dict)
                local value = field_dict[key]
                if value == nil then
                    value = new_default(message)
                    field_dict[key] = value
                end
                local endpoint
                endpoint, pos = DecodeVarint(buffer, pos)
                endpoint = endpoint + pos
                if endpoint > pend then
                    error('Truncated message.')
                end
                local element
                while pos < endpoint do
                    element, pos = decode_value(buffer, pos)
                    value[#value + 1] = element
                end
                if pos > endpoint then
                    value:remove(#value)
                    error('Packed element was truncated.')
                end
                return pos
            end
        elseif is_repeated then
            local tag_bytes = encoder.TagBytes(field_number, wire_type)
            local tag_len = #tag_bytes
            local sub = string.sub
            return function(buffer, pos, pend, message, field_dict)
                local value = field_dict[key]
                if value == nil then
                    value = new_default(message)
                    field_dict[key] = value
                end
                while 1 do
                    local element, new_pos = decode_value(buffer, pos)
                    value:append(element)
                    pos = new_pos + tag_len
                    if sub(buffer, new_pos+1, pos) ~= tag_bytes or new_pos >= pend then
                        if new_pos > pend then
                            error('Truncated message.')
                        end
                        return new_pos
                    end
                end
            end
        else
            return function (buffer, pos, pend, message, field_dict)
                field_dict[key], pos = decode_value(buffer, pos)
                if pos > pend then
                    field_dict[key] = nil
                    error('Truncated message.')
                end
                return pos
            end
        end
    end
end

local function _ModifiedDecoder(wire_type, decode_value, modify_value)
    local InnerDecode = function (buffer, pos)
        local result, new_pos = decode_value(buffer, pos)
        return modify_value(result), new_pos
    end
    return _SimpleDecoder(wire_type, InnerDecode)
end

local function _StructPackDecoder(wire_type, value_size, format)
    local struct_unpack = pb.struct_unpack

    function InnerDecode(buffer, pos)
        local new_pos = pos + value_size
        local result = struct_unpack(format, buffer, pos)
        return result, new_pos
    end
    return _SimpleDecoder(wire_type, InnerDecode)
end

local function _Boolean(value)
    return value ~= 0
end

Int32Decoder = _SimpleDecoder(wire_format.WIRETYPE_VARINT, _DecodeSignedVarint32)
EnumDecoder = Int32Decoder

Int64Decoder = _SimpleDecoder(wire_format.WIRETYPE_VARINT, _DecodeSignedVarint64)

UInt32Decoder = _SimpleDecoder(wire_format.WIRETYPE_VARINT, _DecodeVarint32)
UInt64Decoder = _SimpleDecoder(wire_format.WIRETYPE_VARINT, _DecodeVarint64)

SInt32Decoder = _ModifiedDecoder(wire_format.WIRETYPE_VARINT, _DecodeVarint32, wire_format.ZigZagDecode32)
SInt64Decoder = _ModifiedDecoder(wire_format.WIRETYPE_VARINT, _DecodeVarint64, wire_format.ZigZagDecode64)

Fixed32Decoder  = _StructPackDecoder(wire_format.WIRETYPE_FIXED32, 4, string.byte('I'))
Fixed64Decoder  = _StructPackDecoder(wire_format.WIRETYPE_FIXED64, 8, string.byte('Q'))
SFixed32Decoder = _StructPackDecoder(wire_format.WIRETYPE_FIXED32, 4, string.byte('i'))
SFixed64Decoder = _StructPackDecoder(wire_format.WIRETYPE_FIXED64, 8, string.byte('q'))
FloatDecoder    = _StructPackDecoder(wire_format.WIRETYPE_FIXED32, 4, string.byte('f'))
DoubleDecoder   = _StructPackDecoder(wire_format.WIRETYPE_FIXED64, 8, string.byte('d'))


BoolDecoder = _ModifiedDecoder(wire_format.WIRETYPE_VARINT, _DecodeVarint, _Boolean)


function StringDecoder(field_number, is_repeated, is_packed, key, new_default)
    local DecodeVarint = _DecodeVarint
    local sub = string.sub
    --    local unicode = unicode
    assert(not is_packed)
    if is_repeated then
        local tag_bytes = encoder.TagBytes(field_number, wire_format.WIRETYPE_LENGTH_DELIMITED)
        local tag_len = #tag_bytes
        return function (buffer, pos, pend, message, field_dict)
            local value = field_dict[key]
            if value == nil then
                value = new_default(message)
                field_dict[key] = value
            end
            while 1 do
                local size, new_pos
                size, pos = DecodeVarint(buffer, pos)
                new_pos = pos + size
                if new_pos > pend then
                    error('Truncated string.')
                end
                value:append(sub(buffer, pos+1, new_pos))
                pos = new_pos + tag_len
                if sub(buffer, new_pos + 1, pos) ~= tag_bytes or new_pos == pend then
                    return new_pos
                end
            end
        end
    else
        return function (buffer, pos, pend, message, field_dict)
            local size, new_pos
            size, pos = DecodeVarint(buffer, pos)
            new_pos = pos + size
            if new_pos > pend then
                error('Truncated string.')
            end
            field_dict[key] = sub(buffer, pos + 1, new_pos)
            return new_pos
        end
    end
end

function BytesDecoder(field_number, is_repeated, is_packed, key, new_default)
    local DecodeVarint = _DecodeVarint
    local sub = string.sub
    assert(not is_packed)
    if is_repeated then
        local tag_bytes = encoder.TagBytes(field_number, wire_format.WIRETYPE_LENGTH_DELIMITED)
        local tag_len = #tag_bytes
        return function (buffer, pos, pend, message, field_dict)
            local value = field_dict[key]
            if value == nil then
                value = new_default(message)
                field_dict[key] = value
            end
            while 1 do
                local size, new_pos
                size, pos = DecodeVarint(buffer, pos)
                new_pos = pos + size
                if new_pos > pend then
                    error('Truncated string.')
                end
                value:append(sub(buffer, pos + 1, new_pos))
                pos = new_pos + tag_len
                if sub(buffer, new_pos + 1, pos) ~= tag_bytes or new_pos == pend then
                    return new_pos
                end
            end
        end
    else
        return function(buffer, pos, pend, message, field_dict)
            local size, new_pos
            size, pos = DecodeVarint(buffer, pos)
            new_pos = pos + size
            if new_pos > pend then
                error('Truncated string.')
            end
            field_dict[key] = sub(buffer, pos + 1, new_pos)
            return new_pos
        end
    end
end

function MessageDecoder(field_number, is_repeated, is_packed, key, new_default)
    local DecodeVarint = _DecodeVarint
    local sub = string.sub

    assert(not is_packed)
    if is_repeated then
        local tag_bytes = encoder.TagBytes(field_number, wire_format.WIRETYPE_LENGTH_DELIMITED)
        local tag_len = #tag_bytes
        return function (buffer, pos, pend, message, field_dict)
            local value = field_dict[key]
            if value == nil then
                value = new_default(message)
                field_dict[key] = value
            end
            while 1 do
                local size, new_pos
                size, pos = DecodeVarint(buffer, pos)
                new_pos = pos + size
                if new_pos > pend then
                    error('Truncated message.')
                end
                if value:add():_InternalParse(buffer, pos, new_pos) ~= new_pos then
                    error('Unexpected end-group tag.')
                end
                pos = new_pos + tag_len
                if sub(buffer, new_pos + 1, pos) ~= tag_bytes or new_pos == pend then
                    return new_pos
                end
            end
        end
    else
        return function (buffer, pos, pend, message, field_dict)
            local value = field_dict[key]
            if value == nil then
                value = new_default(message)
                field_dict[key] = value
            end
            local size, new_pos
            size, pos = DecodeVarint(buffer, pos)
            new_pos = pos + size
            if new_pos > pend then
                error('Truncated message.')
            end
            if value:_InternalParse(buffer, pos, new_pos) ~= new_pos then
                error('Unexpected end-group tag.')
            end
            return new_pos
        end
    end
end

function _SkipVarint(buffer, pos, pend)
    local value
    value, pos = _DecodeVarint(buffer, pos)
    return pos
end

function _SkipFixed64(buffer, pos, pend)
    pos = pos + 8
    if pos > pend then 
        error('Truncated message.')
    end
    return pos
end

function _SkipLengthDelimited(buffer, pos, pend)
    local size
    size, pos = _DecodeVarint(buffer, pos)
    pos = pos + size
    if pos > pend then
        error('Truncated message.')
    end
    return pos
end

function _SkipFixed32(buffer, pos, pend)
    pos = pos + 4
    if pos > pend then
        error('Truncated message.')
    end
    return pos
end

function _RaiseInvalidWireType(buffer, pos, pend)
    error('Tag had invalid wire type.')
end

function _FieldSkipper()
    WIRETYPE_TO_SKIPPER = {
        _SkipVarint,
        _SkipFixed64,
        _SkipLengthDelimited,
        _SkipGroup,
        _EndGroup,
        _SkipFixed32,
        _RaiseInvalidWireType,
        _RaiseInvalidWireType,
    }

--    wiretype_mask = wire_format.TAG_TYPE_MASK
    local ord = string.byte
    local sub = string.sub

    return function (buffer, pos, pend, tag_bytes)
        local wire_type = ord(sub(tag_bytes, 1, 1)) % 8 + 1
        return WIRETYPE_TO_SKIPPER[wire_type](buffer, pos, pend)
    end
end

SkipField = _FieldSkipper()
