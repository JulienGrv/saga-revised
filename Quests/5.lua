-- Saga is Licensed under Creative Commons Attribution-NonCommerial-ShareAlike 3.0 License
-- http://creativecommons.org/licenses/by-nc-sa/3.0/
-- Generated By Quest Extractor on 2/8/2008 3:46:14 PM

local QuestID = 5;
local ReqClv = 2;
local ReqJlv = 0;
local NextQuest = 0;
local RewZeny = 11;
local RewCxp = 30;
local RewJxp = 0;
local RewWxp = 0; 
local RewItem1 = 51500002; 
local RewItem2 = 0; 
local RewItemCount1 = 20; 
local RewItemCount2 = 0; 
local StepID = 0;

-- Modify steps below for gameplay

function QUEST_VERIFY(cid)
	Saga.GeneralDialog(cid, 3957);
	return 0;
end

function QUEST_START(cid)
	Saga.AddStep(cid, QuestID, 501);
	Saga.AddStep(cid, QuestID, 502);	
	Saga.AddStep(cid, QuestID, 503);	
	Saga.InsertQuest(cid, QuestID, 2);
	return 0;
end

function QUEST_FINISH(cid)
	-- Gives all rewards
	local freeslots = Saga.FreeInventoryCount(cid, 0);
	if freeslots > 0 then
		Saga.GiveZeny(cid, RewZeny);
		Saga.GiveExp(cid, RewCxp, RewJxp, RewWxp);
		Saga.GiveItem(cid, RewItem1, RewItemCount1 );
		return 0;
	else
		Saga.EmptyInventory(cid);
		return -1;
	end
end

function QUEST_CANCEL(cid)
	-- Do nothing
	return 0;
end

function QUEST_STEP_1(cid)

--Scacciano Dialog: 36
	Saga.StepComplete(cid,QuestID,501);
	return 0;
end

function QUEST_STEP_2(cid)
	-- Talk with Zarko Ruzzoli           
	Saga.AddWaypoint(cid, QuestID, 502, 1, 1005);      
	
	-- Check for completion
	local ret = Saga.GetNPCIndex(cid);    
	if ret == 1005 then
		Saga.NpcGiveItem(cid, 2618, 1);
		Saga.GeneralDialog(cid, 41);			
		Saga.SubstepComplete(cid,QuestID,502,1);
	end	
	
    -- Check if all substeps are completed
    for i = 1, 1 do
         if Saga.IsSubStepCompleted(cid,QuestID,502,i) == false then
			return -1;
		 end
    end	
    
	Saga.ClearWaypoints(cid, QuestID);
    Saga.StepComplete(cid,QuestID,502);
	return 0;	
end

function QUEST_STEP_3(cid)
	-- Deliver knife to Scacciano Morrigan       
	Saga.AddWaypoint(cid, QuestID, 503, 1, 1003);      
	
	-- Check for completion
	local ret = Saga.GetNPCIndex(cid);    
	if ret == 1003 then
		Saga.NpcTakeItem(cid, 2618, 1);
		Saga.GeneralDialog(cid, 43);			
		Saga.SubstepComplete(cid,QuestID,503,1);
	end	
	
    -- Check if all substeps are completed
    for i = 1, 1 do
         if Saga.IsSubStepCompleted(cid,QuestID,503,i) == false then
			return -1;
		 end
    end	
    
    Saga.ClearWaypoints(cid, QuestID); 
    Saga.StepComplete(cid,QuestID,503);
    Saga.QuestComplete(cid, QuestID);	       
	return -1;	
end

function QUEST_CHECK(cid)
	-- Check all steps for progress
	local CurStepID = Saga.GetStepIndex(cid, QuestID );
	local ret = -1;
	StepID = CurStepID;
	
	if CurStepID == 501 then
		ret = QUEST_STEP_1(cid);
	elseif CurStepID == 502 then
		ret = QUEST_STEP_2(cid);
	elseif CurStepID == 503 then
		ret = QUEST_STEP_3(cid);
	end
	
	if ret == 0 then
		QUEST_CHECK(cid)
	end
	
	return ret;
end
