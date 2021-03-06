﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using SabberStoneCore.Enums;
using SabberStoneCore.Exceptions;
using SabberStoneCore.Model.Entities;

namespace SabberStoneCore.Model.Zones
{
	/// <summary>
	/// Container object holding all zones owned by one <see cref="Entities.Controller"/>
	/// </summary>
	/// <seealso cref="IEnumerable{IZone}" />
	/// <autogeneratedoc />
	public class ControlledZones : IEnumerable<IZone>
	{
		/// <summary>Gets the game which contains the zones.</summary>
		/// <value><see cref="Model.Game"/></value>
		public Game Game { get; }

		/// <summary>Gets the owner of the contained zones.</summary>
		/// <value><see cref="Entities.Controller"/></value>
		public Controller Controller { get; }

		private readonly IZone[] _zones = new IZone[Enum.GetNames(typeof(Zone)).Length];

		/// <summary>Initializes a new instance of the <see cref="ControlledZones"/> class.</summary>
		/// <param name="game">The game.</param>
		/// <param name="controller">The controller.</param>
		/// <autogeneratedoc />
		public ControlledZones(Game game, Controller controller)
		{
			Game = game;
			Controller = controller;
		}

		/// <summary>Gets the <see cref="IZone"/> matching the zone identifier.</summary>
		/// <value>The <see cref="IZone"/>.</value>
		/// <param name="zone">The zone identifier.</param>
		/// <returns></returns>
		/// <exception cref="ZoneException">There is no zone implemented for the provided identifier</exception>
		public IZone this[Zone zone]
		{
			get
			{
				IZone result = _zones[(int)zone];

				if (result != null)
				{
					return result;
				}

				switch (zone)
				{
					case Zone.INVALID:
						result = null;
						break;
					case Zone.GRAVEYARD:
						result = new GraveyardZone(Game, Controller, zone);
						break;
					case Zone.PLAY:
						result = new BoardZone(Game, Controller, zone);
						break;
					case Zone.DECK:
						result = new DeckZone(Game, Controller);
						break;
					case Zone.HAND:
						result = new HandZone(Game, Controller);
						break;
					case Zone.SETASIDE:
						result = new SetasideZone(Game, Controller, zone);
						break;
					case Zone.SECRET:
						result = new SecretZone(Game, Controller, zone);
						break;
					case Zone.REMOVEDFROMGAME:
						break;
					default:
						throw new ZoneException("No such zone type when creating zone: " + zone);
				}

				_zones[(int)zone] = result;

				return result;
			}
		}

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

		public void Stamp(ControlledZones zones)
		{
			// setaside need to be cloned first for references like choose one cards ...
			var zoneEnums = new List<Zone> {
				Zone.SETASIDE,
				Zone.PLAY,
				Zone.DECK,
				Zone.HAND,
				Zone.GRAVEYARD,
				Zone.SECRET,
				Zone.REMOVEDFROMGAME
			};
			zoneEnums.ForEach(p =>
			{
				IZone zone = zones[p];
				if (zone != null)
					this[p].Stamp(zone);
			});

			//foreach (Zone value in Enum.GetValues(typeof(Zone)))
			//{
			//    if (value == Zone.INVALID)
			//        continue;

			//    var zone = zones[value];
			//    if (zone != null)
			//        this[value].Stamp(zone);
			//}
		}

		public string Hash(params GameTag[] ignore)
		{
			var str = new StringBuilder();
			foreach (IZone zone in _zones)
			{
				if (zone != null)
					str.Append(zone.Hash(ignore));
			}
			return str.ToString();
		}

		public IEnumerator<IZone> GetEnumerator()
		{
			return ((IEnumerable<IZone>)_zones).GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return _zones.GetEnumerator();
		}

		#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
	}
}
