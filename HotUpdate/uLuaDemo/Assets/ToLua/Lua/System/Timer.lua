--------------------------------------------------------------------------------
--      Copyright (c) 2015 , 蒙占志(topameng) topameng@gmail.com
--      All rights reserved.
--      Use, modification and distribution are subject to the "MIT License"
--------------------------------------------------------------------------------
local setmetatable = setmetatable
local UpdateBeat = UpdateBeat
local CoUpdateBeat = CoUpdateBeat
local Time = Time

Timer = {}

local Timer = Timer
local mt = {__index = Timer}

--scale false 采用deltaTime计时，true 采用 unscaledDeltaTime计时
function Timer.New(func, duration, loop, scale)
	scale = scale or false and true	
	loop = loop or 1
	return setmetatable({func = func, duration = duration, time = duration, loop = loop, scale = scale, running = false}, mt)	
end

function Timer:Start()
	self.running = true

	if not self.handle then
		self.handle = UpdateBeat:CreateListener(self.Update, self)
	end

	UpdateBeat:AddListener(self.handle)	
end

function Timer:Reset(func, duration, loop, scale)
	self.duration 	= duration
	self.loop		= loop or 1
	self.scale		= scale
	self.func		= func
	self.time		= duration		
end

function Timer:Stop()
	self.running = false

	if self.handle then
		UpdateBeat:RemoveListener(self.handle)	
	end
end

function Timer:Update()
	local delta = self.scale and Time.deltaTime or Time.unscaledDeltaTime	
	self.time = self.time - delta
	
	if self.time <= 0 then
		self.func()
		
		if self.loop > 0 then
			self.loop = self.loop - 1
			self.time = self.time + self.duration
		end
		
		if self.loop == 0 then
			self:Stop()
		elseif self.loop < 0 then
			self.time = self.time + self.duration
		end
	end
end

--给协同使用的帧计数timer
FrameTimer = {}

local FrameTimer = FrameTimer
local mt2 = {__index = FrameTimer}

function FrameTimer.New(func, count, loop)	
	local c = Time.frameCount + count
	loop = loop or 1
	return setmetatable({func = func, loop = loop, duration = count, count = c, running = false}, mt2)		
end

function FrameTimer:Reset(func, count, loop)
	self.func = func
	self.duration = count
	self.loop = loop
	self.count = Time.frameCount + count	
end

function FrameTimer:Start()		
	if not self.handle then
		self.handle = CoUpdateBeat:CreateListener(self.Update, self)
	end
	
	CoUpdateBeat:AddListener(self.handle)	
	self.running = true
end

function FrameTimer:Stop()	
	self.running = false

	if self.handle then
		CoUpdateBeat:RemoveListener(self.handle)	
	end
end

function FrameTimer:Update()	
	if Time.frameCount >= self.count then
		self.func()	
		
		if self.loop > 0 then
			self.loop = self.loop - 1
		end
		
		if self.loop == 0 then
			self:Stop()
		else
			self.count = Time.frameCount + self.duration
		end
	end
end

CoTimer = {}

local CoTimer = CoTimer
local mt3 = {__index = CoTimer}

function CoTimer.New(func, duration, loop)	
	loop = loop or 1
	return setmetatable({duration = duration, loop = loop, func = func, time = duration, running = false}, mt3)			
end

function CoTimer:Start()		
	if not self.handle then	
		self.handle = CoUpdateBeat:CreateListener(self.Update, self)
	end
	
	self.running = true
	CoUpdateBeat:AddListener(self.handle)	
end

function CoTimer:Reset(func, duration, loop)
	self.duration 	= duration
	self.loop		= loop or 1	
	self.func		= func
	self.time		= duration		
end

function CoTimer:Stop()
	self.running = false

	if self.handle then
		CoUpdateBeat:RemoveListener(self.handle)	
	end
end

function CoTimer:Update()	
	if self.time <= 0 then
		self.func()		
		
		if self.loop > 0 then
			self.loop = self.loop - 1
			self.time = self.time + self.duration
		end
		
		if self.loop == 0 then
			self:Stop()
		elseif self.loop < 0 then
			self.time = self.time + self.duration
		end
	end
	
	self.time = self.time - Time.deltaTime
end