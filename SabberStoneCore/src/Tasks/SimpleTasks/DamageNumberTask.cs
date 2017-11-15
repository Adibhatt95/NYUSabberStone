﻿using SabberStoneCore.Actions;
using SabberStoneCore.Model.Entities;

namespace SabberStoneCore.Tasks.SimpleTasks
{
	public class DamageNumberTask : SimpleTask
	{
		public DamageNumberTask(EntityType type, bool spellDmg = false)
		{
			Type = type;
			SpellDmg = spellDmg;
		}

		public EntityType Type { get; set; }

		public bool SpellDmg { get; set; }

		public override TaskState Process()
		{
			if (Number < 1)
			{
				return TaskState.STOP;
			}

			int spellDmgValue = SpellDmg ? (Source is Spell && ((Spell)Source).ReceveivesDoubleSpellDamage ? Controller.Hero.SpellPowerDamage * 2 : Controller.Hero.SpellPowerDamage) : 0;
			IncludeTask.GetEntites(Type, Controller, Source, Target, Playables)
				.ForEach(p => Generic.DamageCharFunc.Invoke(Source as IPlayable, p as ICharacter, Number,
					spellDmgValue));

			return TaskState.COMPLETE;
		}

		public override ISimpleTask Clone()
		{
			var clone = new DamageNumberTask(Type, SpellDmg);
			clone.Copy(this);
			return clone;
		}
	}
}