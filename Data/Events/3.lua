local QuestID = 1;
local ReqClv = 0;
local ReqJlv = 0;
local NextQuest = 0;
local RewItem1 = 1700113; 
local RewItemCount1 = 1; 
local RewItemCount2 = 1; 
local StepID = 0;

function EVENT_PARTICIPATE(cid)
	Saga.Rewards(cid);
	return -1;
end

function QUEST_START(cid)
	Saga.AddStep(cid, QuestID, 101);
	Saga.InsertQuest(cid, QuestID, 1);
	return 0;
end

function QUEST_FINISH(cid)
	-- Check for free inventory slot
	local freeslots = Saga.FreeInventoryCount(cid, 0);
	if freeslots > 1 then
		Saga.GiveItem(cid, RewItem1, RewItemCount1);
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
	-- Get 6 Chonchon Fry Wings	
	Saga.FindQuestItem(cid, QuestID, StepID, 10485, 2630, 10000, 1, 1);
	
    -- Check if all substeps are completed
    for i = 1, 1 do
         if Saga.IsSubStepCompleted(cid, QuestID, StepID, i) == false then
			return -1;
		 end
    end
	
	Saga.StepComplete(cid, QuestID, StepID);	
    Saga.QuestComplete(cid, QuestID);	    
    return -1;
end

function QUEST_CHECK(cid)
	-- Check all steps for progress
	local CurStepID = Saga.GetStepIndex(cid, QuestID);
	local ret = -1;
	StepID = CurStepID;
	
	if CurStepID == 101 then
		ret = QUEST_STEP_1(cid);
	end
	
	if ret == 0 then
		QUEST_CHECK(cid);
	end
	
	return ret;
end
