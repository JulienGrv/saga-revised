-- Saga is Licensed under Creative Commons Attribution-NonCommerial-ShareAlike 3.0 License
-- http://creativecommons.org/licenses/by-nc-sa/3.0/
-- Generated By Quest Extractor on 2/8/2008 3:46:14 PM
-- BattleSpunk 24/07/2008

local QuestID = 30;
local ReqClv = 4;
local ReqJlv = 0;
local NextQuest = 0;
local RewZeny = 60;
local RewCxp = 108;
local RewJxp = 42;
local RewWxp = 0; 
local RewItem1 = 1700113; 
local RewItem2 = 51500003; 
local RewItemCount1 = 3; 
local RewItemCount2 = 2; 

-- Modify steps below for gameplay

function QUEST_START(cid)
	-- Initialize all quest steps
	-- Initialize all starting navigation points
	Saga.AddStep(cid, QuestID, 3001);
	Saga.AddStep(cid, QuestID, 3002);
	Saga.InsertQuest(cid, QuestID, 1);	
	return 0;
end

function QUEST_FINISH(cid)
	-- Gives all rewards
	If freeslots > 1 then
		Saga.GiveZeny(RewZeny);
		Saga.GiveExp( RewCxp, RewJxp, RewWxp);
		Saga.GiveItem(cid, RewItem1, RewItemCount1 );
		Saga.GiveItem(cid, RewItem2, RewItemCount2 );
		return 0;
	else
		Saga.EmptyInventory(cid);
		return -1;
	end
end

function QUEST_CANCEL(cid)
	return 0;
end

function QUEST_STEP_1(cid)
	-- Get 4 Wicked Star Loots	
	Saga.FindQuestItem(cid,QuestID,StepID,10012,2604,10000,4,1);
	Saga.FindQuestItem(cid,QuestID,StepID,10013,2604,10000,4,1);
	Saga.FindQuestItem(cid,QuestID,StepID,10014,2604,10000,4,1);
	
    -- Check if all substeps are completed
     if Saga.IsSubStepCompleted(cid,QuestID,3001, 1) == false then
		return -1;
	 end
		
	Saga.StepComplete(cid,QuestID,StepID);
end

function QUEST_STEP_2(cid)
	
	-- Check for completion
	local ret = Saga.GetNPCIndex(cid);    
	if ret == 1123 then
		Saga.NpcTakeItem(cid, 2604, 4);
		Saga.SubstepComplete(cid,QuestID,StepID,1);
	end	
	
    -- Check if all substeps are completed
     if Saga.IsSubStepCompleted(cid,QuestID,3002, 1) == false then
		return -1;
	 end
    
    Saga.StepComplete(cid,QuestID,StepID);
    Saga.QuestComplete(cid, QuestID);	    
	return 0;
end

function QUEST_CHECK(cid)
	-- Check all steps for progress
	local CurStepID = Saga.GetStepIndex(cid, QuestID );
	local ret = -1;
	StepID = CurStepID;
	
	if CurStepID == 3001 then
		ret = QUEST_STEP_1(cid);
	elseif CurStepID == 3002 then
		ret = QUEST_STEP_2(cid);
	end
	
	if ret == 0 then
		QUEST_CHECK(cid)
	end
	
	return ret;
end