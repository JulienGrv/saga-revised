using Saga.Map;
using Saga.Map.Utils.Definitions.Misc;
using Saga.Packets;
using Saga.PrimaryTypes;
using System;
using System.Collections.Generic;

public static class CommonDialogs
{
    public static void OpenSpecialSkillsDialog(Character target, IEnumerable<uint> skilllist)
    {
        Saga.Factory.Spells.Info info;
        SMSG_SENDSPECIALSKILLS spkt = new SMSG_SENDSPECIALSKILLS();

        foreach (uint i in skilllist)
        {
            if (Singleton.SpellManager.TryGetSpell(i, out info))
            {
                //Helper predicate
                Predicate<Skill> FindSkill = delegate(Skill skill)
                {
                    return info.skillid == skill.info.skillid;
                };

                //Check if we already learned the spells
                if (info.special > 0 &&
                    target.learnedskills.FindIndex(FindSkill) == -1 &&
                    Common.Skills.HasSpecialSkillPresent(target, i) == false) spkt.Add(i);
            }
        }
        spkt.SessionId = target.id;
        target.client.Send((byte[])spkt);
    }
}