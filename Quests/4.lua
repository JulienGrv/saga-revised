-- Saga is Licensed under Creative Commons Attribution-NonCommerial-ShareAlike 3.0 License
-- http://creativecommons.org/licenses/by-nc-sa/3.0/
-- Generated By Quest Extractor on 2/8/2008 3:46:14 PM

local QuestID = 4;
local ReqClv = 14;
local ReqJlv = 0;
local NextQuest = 0;
local RewZeny = 441;
local RewCxp = 1458;
local RewJxp = 573;
local RewWxp = 0; 
local RewItem1 = 3439; 
local RewItem2 = 0; 
local RewItemCount1 = 1; 
local RewItemCount2 = 0; 

-- Modify steps below for gameplay

function QUEST_START(cid)
	Saga.AddStep(cid, QuestID, 401);
	Saga.AddStep(cid, QuestID, 402);	
	Saga.InsertQuest(cid, QuestID, 1);
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
	Saga.Eliminate(cid,QuestID,401,10063,1,1);	

    -- Check if all substeps are completed
    for i = 1, 1 do
         if Saga.IsSubStepCompleted(cid,QuestID,401,i) == false then
			return -1;
		 end
    end		
	
	Saga.StepComplete(cid,QuestID,401);
	return 0;
end

function QUEST_STEP_2(cid)
	-- Talk to mischa              
	Saga.AddWaypoint(cid, QuestID, 402, 1, 1000);      
	
	-- Check for completion
	local ret = Saga.GetNPCIndex(cid);    
	if ret == 1000 then
		Saga.GeneralDialog(cid, 35);			
		Saga.SubstepComplete(cid, QuestID, 402, 1);
	end	
	
    -- Check if all substeps are completed
    for i = 1, 1 do
         if Saga.IsSubStepCompleted(cid,QuestID,402,i) == false then
			return -1;
		 end
    end	
    
	Saga.ClearWaypoints(cid, QuestID);  
    Saga.StepComplete(cid,QuestID,402);  
    Saga.QuestComplete(cid, QuestID);	       
	return -1;	
end

function QUEST_CHECK(cid)
	-- Check all steps for progress
	local CurStepID = Saga.GetStepIndex(cid, QuestID );
	local ret = -1;
	StepID = CurStepID;
	
	if CurStepID == 401 then
		ret = QUEST_STEP_1(cid);
	elseif CurStepID == 402 then
		ret = QUEST_STEP_2(cid);
	end
	
	if ret == 0 then
		QUEST_CHECK(cid)
	end
	
	return ret;
end
