print("PLEASE EDIT THIS SCRIPT SO VALUES IN THE CONFIG SECTION MATCH YOUR DATABASE(S)")


-- You can change here the credentionals for your own
-- database settings.

local mysqlbinpath = "D:\\Program Files\\MySQL\\MySQL Server 4.1\\bin";
local username = promptstring("Please enter username");
local password = promptstring("Please enter password");
local database = promptstring("Please enter database");
local host = promptstring("Please enter host");
local mysqldumpPath = mysqlbinpath .. "\\mysqldump.exe";
local mysqlPath = mysqlbinpath .. "\\mysql.exe";

-- Don't change anything below here

function Install()
	print(" Deleting database tables for new content.");
	print(" creating accounts table");
	print(" creating channels tables");
	print(" creating characters table");
	print(" creating items table");
	print(" creating list_clans table");
	print(" creating list_friend table");
	print(" creating list_quest table");	
end

function Upgrade()
	print(" Installing new database content.");
	print(" updating list_npcs");
	-- %mysqlPath% -h %host% -u %user% --password=%pass% -D %db% < list_npcs.sql
	print(" updating list_respawnzones");
	-- %mysqlPath% -h %host% -u %user% --password=%pass% -D %db% < list_respawnzones.sql
	print(" updating spawnareas");
	-- %mysqlPath% -h %host% -u %user% --password=%pass% -D %db% < list_spawnareas.sql
	print(" updating telegates");
	-- %mysqlPath% -h %host% -u %user% --password=%pass% -D %db% < list_telegates.sql
	print(" updating npc_data");
	-- %mysqlPath% -h %host% -u %user% --password=%pass% -D %db% < npc_data.sql
	print(" updating quest_data");
	-- %mysqlPath% -h %host% -u %user% --password=%pass% -D %db% < quest_data.sql
	print(" updating skills_data");
	-- %mysqlPath% -h %host% -u %user% --password=%pass% -D %db% < skills_data.sql
	print(" updating storage");
	-- %mysqlPath% -h %host% -u %user% --password=%pass% -D %db% < storage.sql
end


function Ask()
	local c = prompt();
	if c == 102 then
		Install();
		Upgrade();
	elseif c == 117 then
		Upgrade();			
	elseif c == 113 then
	else	
		Ask();
	end
end

print("");
print("");
print("Making a backup of the original database.");
doproccess(mysqldumpPath, "--add-drop-table -h ".. host .." -u ".. username .." --password=".. password .." -B ".. database .."  > osaga_backup.sql");
print("");
print("WARNING: A full install (f) will destroy data in your");
print("accounts,characters,items,quest_data and storage tables.");
print("Choose upgrade (u) if you already have a running server");
print("");
print("DB install type: (f) full or (u) upgrade or (q) quit?");
print("");
Ask();
print("Script complete");


