-- Saga is Licensed under Creative Commons Attribution-NonCommerial-ShareAlike 3.0 License
-- http://creativecommons.org/licenses/by-nc-sa/3.0/
-- Generated By Quest Extractor on 2/8/2008 3:46:14 PM

local QuestID = 20;
local ReqClv = 5;
local ReqJlv = 0;
local NextQuest = 330;
local RewZeny = 40;
local RewCxp = 65;
local RewJxp = 0;
local RewWxp = 0; 
local RewItem1 = 1700113; 
local RewItem2 = 16099; 
local RewItemCount1 = 1; 
local RewItemCount2 = 1; 
local StepID = 0;

-- Modify steps below for gameplay

function QUEST_VERIFY(cid)
	Saga.GeneralDialog(cid, 3957);
	return 0;
end

function QUEST_START(cid)
	Saga.AddStep(cid, QuestID, 2001);
	Saga.AddStep(cid, QuestID, 2002);
	Saga.InsertQuest(cid, QuestID, 2);
	return 0;
end

function QUEST_FINISH(cid)
	-- Gives all rewards
	local freeslots = Saga.FreeInventoryCount(cid, 0);
	if freeslots > 1 then
		Saga.GiveZeny(cid, RewZeny);
		Saga.GiveExp(cid, RewCxp, RewJxp, RewWxp);
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
	--personal quest
	Saga.StepComplete(cid, QuestID, 2001);
	return 0;
end

function QUEST_STEP_2(cid)
--Talk with Klaret Natali
	Saga.AddWaypoint(cid, QuestID, 2002, 1, 1001);

--check completion
	local ret = Saga.GetNPCIndex(cid);
	if ret == 1001
then
	Saga.GeneralDialog(cid, 3936);
	Saga.SubstepComplete(cid,QuestID,StepID,1);
	end



--check if all substeps completed
	for i = 1, 1 do
	if Saga.IsSubStepCompleted(cid,QuestID,StepID,i) == false
then
	return -1;
	end

end

	Saga.ClearWaypoints(cid, QuestID);
	Saga.StepComplete(cid,QuestID,StepID);
	Saga.QuestComplete(cid, QuestID);
	return -1;
end


function QUEST_CHECK(cid)
	-- Check all steps for progress
	local CurStepID = Saga.GetStepIndex(cid, QuestID );
	local ret = -1;
	StepID = CurStepID;
	
	if CurStepID == 2001 then
		ret = QUEST_STEP_1(cid);
	elseif CurStepID == 2002 then
		ret = QUEST_STEP_2(cid);
	end
	
	if ret == 0 then
		QUEST_CHECK(cid)
	end
	
	return ret;
end

	