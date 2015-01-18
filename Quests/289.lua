-- Saga is Licensed under Creative Commons Attribution-NonCommerial-ShareAlike 3.0 License
-- http://creativecommons.org/licenses/by-nc-sa/3.0/
-- Generated By Quest Extractor on 2/8/2008 3:46:17 PM

local QuestID = 289;
local ReqClv = 21;
local ReqJlv = 0;
local NextQuest = 290;
local RewZeny = 927;
local RewCxp = 3045;
local RewJxp = 1218;
local RewWxp = 0; 
local RewItem1 = 0; 
local RewItem2 = 0; 
local RewItemCount1 = 0; 
local RewItemCount2 = 0; 

-- Modify steps below for gameplay

function QUEST_START()
	-- Initialize all quest steps
	-- Initialize all starting navigation points
	return 0;
end

function QUEST_FINISH()
	Saga.GiveZeny(RewZeny);
	Saga.GiveExp( RewCxp, RewJxp, RewWxp);
	return 0;
end

function QUEST_CANCEL()
	return 0;
end

function QUEST_CHECK()
	-- Check all steps for progress
	local CurStepID = Saga.GetStepIndex();
	-- TODO: Add code to check all progress
	return -1;
end
