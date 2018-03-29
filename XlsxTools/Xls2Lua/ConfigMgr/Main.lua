require "Class"
require "ConfigMgr"

function Main()
	local configMgr = ConfigMgr:Instance()
	local lang = configMgr:GetConfig("Language")
	print(lang.items[1].id .. " " .. lang.items[1].text)
	local myText = configMgr:GetItem("Language",10000).text
	print(myText)
end

Main()