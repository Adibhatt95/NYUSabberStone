﻿using SabberStoneCore.Enums;
using SabberStoneCore.Model.Entities;

namespace SabberStoneCore.Tasks.SimpleTasks
{
	public class ActivateEnchantment : SimpleTask
	{
		public ActivateEnchantment(EntityType entityType, EnchantmentActivation activation)
		{
			Type = entityType;
			Activation = activation;
		}

		public EntityType Type { get; set; }

		public EnchantmentActivation Activation { get; set; }

		public override TaskState Process()
		{

			System.Collections.Generic.List<IPlayable> entities = IncludeTask.GetEntites(Type, Controller, Source, Target, Playables);

			entities.ForEach(p =>
			{
				p.Enchantments.ForEach(t =>
				{
					if (t.Activation == Activation)
					{
						t.Activate(Controller, p, Target as IPlayable);
					}
				});

			});

			return TaskState.COMPLETE;
		}

		public override ISimpleTask Clone()
		{
			var clone = new ActivateEnchantment(Type, Activation);
			clone.Copy(this);
			return clone;
		}
	}
}