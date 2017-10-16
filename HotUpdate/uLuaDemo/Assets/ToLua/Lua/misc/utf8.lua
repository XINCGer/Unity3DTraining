local utf8 = {}

--byte index of the next char after the char at byte index i, followed by a valid flag for the char at byte index i.
--nil if not found. invalid characters are iterated as 1-byte chars.
function utf8.next_raw(s, i)
	if not i then
		if #s == 0 then return nil end
		return 1, true --fake flag (doesn't matter since this flag is not to be taken as full validation)
	end
	if i > #s then return end
	local c = s:byte(i)
	if c >= 0x00 and c <= 0x7F then
		i = i + 1
	elseif c >= 0xC2 and c <= 0xDF then
		i = i + 2
	elseif c >= 0xE0 and c <= 0xEF then
		i = i + 3
	elseif c >= 0xF0 and c <= 0xF4 then
		i = i + 4
	else --invalid
		return i + 1, false
	end
	if i > #s then return end
	return i, true
end

--next() is the generic iterator and can be replaced for different semantics. next_raw() must preserve its semantics.
utf8.next = utf8.next_raw

--iterate chars, returning the byte index where each char starts
function utf8.byte_indices(s, previ)
	return utf8.next, s, previ
end

--number of chars in string
function utf8.len(s)
	assert(s, "bad argument #1 to 'len' (string expected, got nil)")
	local len = 0
	for _ in utf8.byte_indices(s) do
		len = len + 1
	end
	return len
end

--byte index given char index. nil if the index is outside the string.
function utf8.byte_index(s, target_ci)
	if target_ci < 1 then return end
	local ci = 0
	for i in utf8.byte_indices(s) do
		ci = ci + 1
		if ci == target_ci then
			return i
		end
	end
	assert(target_ci > ci, "invalid index")
end

--char index given byte index. nil if the index is outside the string.
function utf8.char_index(s, target_i)
	if target_i < 1 or target_i > #s then return end
	local ci = 0
	for i in utf8.byte_indices(s) do
		ci = ci + 1
		if i == target_i then
			return ci
		end
	end
	error("invalid index")
end

--byte index of the prev. char before the char at byte index i, which defaults to #s + 1.
--nil if the index is outside the 2..#s+1 range.
--NOTE: unlike next(), this is a O(N) operation!
function utf8.prev(s, nexti)
	nexti = nexti or #s + 1
	if nexti <= 1 or nexti > #s + 1 then return end
	local lasti, lastvalid = utf8.next(s)
	for i, valid in utf8.byte_indices(s) do
		if i == nexti then
			return lasti, lastvalid
		end
		lasti, lastvalid = i, valid
	end
	if nexti == #s + 1 then
		return lasti, lastvalid
	end
	error("invalid index")
end

--iterate chars in reverse order, returning the byte index where each char starts.
function utf8.byte_indices_reverse(s, nexti)
	if #s < 200 then
		--using prev() is a O(N^2/2) operation, ok for small strings (200 chars need 40,000 iterations)
		return utf8.prev, s, nexti
	else
		--store byte indices in a table and iterate them in reverse.
		--this is 40x slower than byte_indices() but still fast at 2mil chars/second (but eats RAM and makes garbage).
		local t = {}
		for i in utf8.byte_indices(s) do
			if nexti and i >= nexti then break end
			table.insert(t, i)
		end
		local i = #t + 1
		return function()
			i = i - 1
			return t[i]
		end
	end
end

--sub based on char indices, which, unlike with standard string.sub(), can't be negative.
--start_ci can be 1..inf and end_ci can be 0..inf. end_ci can be nil meaning last char.
--if start_ci is out of range or end_ci < start_ci, the empty string is returned.
--if end_ci is out of range, it is considered to be the last position in the string.
function utf8.sub(s, start_ci, end_ci)
	--assert for positive indices because we might implement negative indices in the future.
	assert(start_ci >= 1)
	assert(not end_ci or end_ci >= 0)
	local ci = 0
	local start_i, end_i
	for i in utf8.byte_indices(s) do
		ci = ci + 1
		if ci == start_ci then
			start_i = i
		end
		if ci == end_ci then
			end_i = i
		end
	end
	if not start_i then
		assert(start_ci > ci, 'invalid index')
		return ''
	end
	if end_ci and not end_i then
		if end_ci < start_ci then
			return ''
		end
		assert(end_ci > ci, 'invalid index')
	end
	return s:sub(start_i, end_i and end_i - 1)
end

--check if a string contains a substring at byte index i without making garbage.
--nil if the index is out of range. true if searching for the empty string.
function utf8.contains(s, i, sub)
	if i < 1 or i > #s then return nil end
	for si = 1, #sub do
		if s:byte(i + si - 1) ~= sub:byte(si) then
			return false
		end
	end
	return true
end

--count the number of occurences of a substring in a string. the substring cannot be the empty string.
function utf8.count(s, sub)
	assert(#sub > 0)
	local count = 0
	local i = 1
	while i do
		if utf8.contains(s, i, sub) then
			count = count + 1
			i = i + #sub
			if i > #s then break end
		else
			i = utf8.next(s, i)
		end
	end
	return count
end

--utf8 validation and sanitization

--check if there's a valid utf8 codepoint at byte index i. valid ranges for each utf8 byte are:
-- byte  1          2           3          4
--------------------------------------------
-- 00 - 7F
-- C2 - DF    80 - BF
-- E0         A0 - BF     80 - BF
-- E1 - EC    80 - BF     80 - BF
-- ED         80 - 9F     80 - BF
-- EE - EF    80 - BF     80 - BF
-- F0         90 - BF     80 - BF    80 - BF
-- F1 - F3    80 - BF     80 - BF    80 - BF
-- F4         80 - 8F     80 - BF    80 - BF
function utf8.isvalid(s, i)
	local c = s:byte(i)
	if not c then
		return false
	elseif c >= 0x00 and c <= 0x7F then
		return true
	elseif c >= 0xC2 and c <= 0xDF then
		local c2 = s:byte(i + 1)
		return c2 and c2 >= 0x80 and c2 <= 0xBF
	elseif c >= 0xE0 and c <= 0xEF then
		local c2 = s:byte(i + 1)
		local c3 = s:byte(i + 2)
		if c == 0xE0 then
			return c2 and c3 and
				c2 >= 0xA0 and c2 <= 0xBF and
				c3 >= 0x80 and c3 <= 0xBF
		elseif c >= 0xE1 and c <= 0xEC then
			return c2 and c3 and
				c2 >= 0x80 and c2 <= 0xBF and
				c3 >= 0x80 and c3 <= 0xBF
		elseif c == 0xED then
			return c2 and c3 and
				c2 >= 0x80 and c2 <= 0x9F and
				c3 >= 0x80 and c3 <= 0xBF
		elseif c >= 0xEE and c <= 0xEF then
			if c == 0xEF and c2 == 0xBF and (c3 == 0xBE or c3 == 0xBF) then
				return false --uFFFE and uFFFF non-characters
			end
			return c2 and c3 and
				c2 >= 0x80 and c2 <= 0xBF and
				c3 >= 0x80 and c3 <= 0xBF
		end
	elseif c >= 0xF0 and c <= 0xF4 then
		local c2 = s:byte(i + 1)
		local c3 = s:byte(i + 2)
		local c4 = s:byte(i + 3)
		if c == 0xF0 then
			return c2 and c3 and c4 and
				c2 >= 0x90 and c2 <= 0xBF and
				c3 >= 0x80 and c3 <= 0xBF and
				c4 >= 0x80 and c4 <= 0xBF
		elseif c >= 0xF1 and c <= 0xF3 then
			return c2 and c3 and c4 and
				c2 >= 0x80 and c2 <= 0xBF and
				c3 >= 0x80 and c3 <= 0xBF and
				c4 >= 0x80 and c4 <= 0xBF
		elseif c == 0xF4 then
			return c2 and c3 and c4 and
				c2 >= 0x80 and c2 <= 0x8F and
				c3 >= 0x80 and c3 <= 0xBF and
				c4 >= 0x80 and c4 <= 0xBF
		end
	end
	return false
end

--byte index of the next valid utf8 char after the char at byte index i.
--nil if indices go out of range. invalid characters are skipped.
function utf8.next_valid(s, i)
	local valid
	i, valid = utf8.next_raw(s, i)
	while i and (not valid or not utf8.isvalid(s, i)) do
		i, valid = utf8.next(s, i)
	end
	return i
end

--iterate valid chars, returning the byte index where each char starts
function utf8.valid_byte_indices(s)
	return utf8.next_valid, s
end

--assert that a string only contains valid utf8 characters
function utf8.validate(s)
	for i, valid in utf8.byte_indices(s) do
		if not valid or not utf8.isvalid(s, i) then
			error(string.format('invalid utf8 char at #%d', i))
		end
	end
end

local function table_lookup(s, i, j, t)
	return t[s:sub(i, j)]
end

--replace characters in string based on a function f(s, i, j, ...) -> replacement_string | nil
function utf8.replace(s, f, ...)
	if type(f) == 'table' then
		return utf8.replace(s, table_lookup, f)
	end
	if s == '' then
		return s
	end
	local t = {}
	local lasti = 1
	for i in utf8.byte_indices(s) do
		local nexti = utf8.next(s, i) or #s + 1
		local repl = f(s, i, nexti - 1, ...)
		if repl then
			table.insert(t, s:sub(lasti, i - 1))
			table.insert(t, repl)
			lasti = nexti
		end
	end
	table.insert(t, s:sub(lasti))
	return table.concat(t)
end

local function replace_invalid(s, i, j, repl_char)
	if not utf8.isvalid(s, i) then
		return repl_char
	end
end

--replace invalid utf8 chars with a replacement char
function utf8.sanitize(s, repl_char)
	repl_char = repl_char or '�' --\uFFFD
	return utf8.replace(s, replace_invalid, repl_char)
end

return utf8
