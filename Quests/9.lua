-- Saga is Licensed under Creative Commons Attribution-NonCommerial-ShareAlike 3.0 License
-- http://creativecommons.org/licenses/by-nc-sa/3.0/
-- Generated By Quest Extractor on 2/8/2008 3:46:14 PM

local QuestID = 9;
local ReqClv = 6;
local ReqJlv = 0;
local NextQuest = 327;
local RewZeny = 83;
local RewCxp = 168;
local RewJxp = 0;
local RewWxp = 0; 
local RewItem1 = 1700113; 
local RewItem2 = 0; 
local RewItemCount1 = 2; 
local RewItemCount2 = 0; 
local StepID = 0;

-- Modify steps below for gameplay

function QUEST_START(cid)
	Saga.AddStep(cid, QuestID, 901);
	Saga.AddStep(cid, QuestID, 902);	
	Saga.AddStep(cid, QuestID, 903);	
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
	-- Get document from Shelphy Adriana       
	Saga.AddWaypoint(cid, QuestID, 901, 1, 1005);      
	
	-- Check for completion
	local ret = Saga.GetNPCIndex(cid);    
	if ret == 1005 then
		Saga.GeneralDialog(cid, 3936);			
		Saga.SubstepComplete(cid,QuestID,StepID,1);
	end	
	
    -- Check if all substeps are completed
    for i = 1, 1 do
         if Saga.IsSubStepCompleted(cid,QuestID,StepID,i) == false then
			return -1;
		 end
    end	
    
    Saga.StepComplete(cid,QuestID,StepID);
	Saga.ClearWaypoints(cid, QuestID); 
	return 0;
end

function QUEST_STEP_2(cid)       
	Saga.FindQuestItem(cid,QuestID,StepID,9,2621,10000,1,1);
	Saga.FindQuestItem(cid,QuestID,StepID,10,2622,10000,1,2);
	Saga.FindQuestItem(cid,QuestID,StepID,11,2623,10000,1,3);

	-- (De-)Activates the Action Objectd on request
	if Saga.IsSubStepCompleted(cid,QuestID,902, 1) == false then
		Saga.UserUpdateActionObjectType(cid,QuestID,StepID,9,0);		
	else
		Saga.UserUpdateActionObjectType(cid,QuestID,StepID,9,1);		
	end
	
	if Saga.IsSubStepCompleted(cid,QuestID,902, 2) == false then
		Saga.UserUpdateActionObjectType(cid,QuestID,StepID,10,0);		
	else
		Saga.UserUpdateActionObjectType(cid,QuestID,StepID,10,1);		
	end
	
	if Saga.IsSubStepCompleted(cid,QuestID,902, 3) == false then		
		Saga.UserUpdateActionObjectType(cid,QuestID,StepID,11,0);		
	else
		Saga.UserUpdateActionObjectType(cid,QuestID,StepID,11,1);				
	end

    -- Check if all substeps are completed
    for i = 1, 3 do
         if Saga.IsSubStepCompleted(cid,QuestID,StepID,i) == false then
			return -1;
		 end
    end	
        
    Saga.StepComplete(cid,QuestID,StepID);		
    return 0;

end

function QUEST_STEP_3(cid)       
	-- Get document from Shelphy Adriana       
	Saga.AddWaypoint(cid, QuestID, 903, 1, 1005);      
	
	-- Check for completion
	local ret = Saga.GetNPCIndex(cid);    
	if ret == 1005 then
		Saga.GeneralDialog(cid, 3936);		
		Saga.NpcTakeItem(cid, 2621, 1); 	
		Saga.NpcTakeItem(cid, 2622, 1);
		Saga.NpcTakeItem(cid, 2623, 1);
		Saga.SubstepComplete(cid,QuestID,StepID,1);
	end	
	
    -- Check if all substeps are completed
    for i = 1, 1 do
         if Saga.IsSubStepCompleted(cid,QuestID,StepID,i) == false then
			return -1;
		 end
    end	
    
    Saga.StepComplete(cid,QuestID,StepID);
	Saga.ClearWaypoints(cid, QuestID); 
    Saga.QuestComplete(cid, QuestID);	        
	return -1;	
end

function QUEST_CHECK(cid)
	-- Check all steps for progress
	local CurStepID = Saga.GetStepIndex(cid, QuestID );
	local ret = -1;
	StepID = CurStepID;
	
	if CurStepID == 901 then
		ret = QUEST_STEP_1(cid);
	elseif CurStepID == 902 then
		ret = QUEST_STEP_2(cid);
	elseif CurStepID == 903 then
		ret = QUEST_STEP_3(cid);
	end
	
	if ret == 0 then
		QUEST_CHECK(cid);
	end
	
	return ret;
end