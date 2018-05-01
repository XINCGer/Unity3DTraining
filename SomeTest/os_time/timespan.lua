Split = function(szFullString,szSeprator)
	local nFindStartIndex = 1
	local nSplitIndex = 1
	local nSplitArray = {}
	while true do
		local nFindLastIndex = string.find(szFullString,szSeprator,nFindStartIndex)
		if not nFindLastIndex then
			nSplitArray[nSplitIndex] = string.sub(szFullString,nFindStartIndex,string.len(szFullString))
			break
		end
		nSplitArray[nSplitIndex] = string.sub(szFullString,nFindStartIndex,nFindLastIndex - 1)
		nFindStartIndex = nFindLastIndex + string.len(szSeprator)
		nSplitIndex = nSplitIndex + 1
	end
	return nSplitArray
end

formatTime = function( timeStr )
	local splited = Split(timeStr, ":")
	local h = splited[1]
	local m = splited[2]
	return tonumber(h), tonumber(m)
end

getRemainTime = function(timeStr)
	local h1, m1 = formatTime(timeStr)
	local curTime = os.date("*t", os.time())
	local curH = curTime.hour
	local curM = curTime.min

	local remainH = h1 - curH
	local remainM = m1 - curM
	local totalRemainSeCond = remainH * 3600 + remainM * 60
	return totalRemainSeCond
end