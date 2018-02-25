﻿using System;
using System.Collections.Generic;
using HearthDb.Enums;
using HearthMirror.Objects;
using Hearthstone_Deck_Tracker.Enums;
using Hearthstone_Deck_Tracker.Enums.Hearthstone;
using Hearthstone_Deck_Tracker.Hearthstone;
using Hearthstone_Deck_Tracker.Hearthstone.Entities;
using Hearthstone_Deck_Tracker.Hearthstone.Secrets;
using Hearthstone_Deck_Tracker.Stats;
using Card = Hearthstone_Deck_Tracker.Hearthstone.Card;
using Deck = HearthMirror.Objects.Deck;

namespace DeckPredictorTests.Mocks
{
	public class MockGame : IGame
	{
		private int _nextEntityId;

		public MockGame()
		{
			Player = new Player(this, true);
			Opponent = new Player(this, false);
			Entities = new Dictionary<int, Entity>();

			// Setup basic game entities.
			var gameEntity = CreateNewEntity(null);
			gameEntity.Name = "GameEntity";
			var heroPlayer = CreateNewEntity("HERO_01");
			heroPlayer.SetTag(GameTag.CARDTYPE, (int)CardType.HERO);
			heroPlayer.SetTag(GameTag.CONTROLLER, heroPlayer.Id);
			Player.Id = heroPlayer.Id;
			var heroOpponent = CreateNewEntity("HERO_02");
			heroOpponent.SetTag(GameTag.CARDTYPE, (int)CardType.HERO);
			heroOpponent.SetTag(GameTag.CONTROLLER, heroOpponent.Id);
			Opponent.Id = heroOpponent.Id;
			var opponentEntity = CreateNewEntity("");
			opponentEntity.SetTag(GameTag.PLAYER_ID, heroOpponent.Id);
		}

		public void AddOpponentCard(string cardId, CardType cardType)
		{
			var card = CreateNewEntity(cardId);
			card.SetTag(GameTag.CONTROLLER, Opponent.Id);
			card.SetTag(GameTag.CARDTYPE, (int)cardType);
		}

		private Entity CreateNewEntity(string cardId)
		{
			int id = _nextEntityId++;
			var entity = new Entity(id) { CardId = cardId };
			Entities.Add(id, entity);
			return entity;
		}

		public Player Player { get; set; }
		public Player Opponent { get; set; }
		public Entity GameEntity { get; set; }
		public Entity PlayerEntity { get; set; }
		public Entity OpponentEntity { get; set; }
		public bool IsMulliganDone { get; set; }
		public bool IsInMenu { get; set; }
		public bool IsUsingPremade { get; set; }
		public bool IsRunning { get; set; }
		public Region CurrentRegion { get; set; }
		public GameMode CurrentGameMode { get; set; }
		public GameStats CurrentGameStats { get; set; }
		public Deck CurrentSelectedDeck { get; set; }
		public List<Card> DrawnLastGame { get; set; }
		public Dictionary<int, Entity> Entities { get; set; }
		public bool SavedReplay { get; set; }
		public GameMetaData MetaData { get; }
		public MatchInfo MatchInfo { get; set; }
		public Mode CurrentMode { get; set; }
		public Mode PreviousMode { get; set; }
		public GameTime GameTime { get; set; }
		public void Reset(bool resetStats = true)
		{
			throw new NotImplementedException();
		}

		public void StoreGameState()
		{
			throw new NotImplementedException();
		}

		public string GetStoredPlayerName(int id)
		{
			throw new NotImplementedException();
		}

		public SecretsManager SecretsManager { get; set; }
		public int OpponentMinionCount { get; set; }
		public int OpponentHandCount { get; set; }
		public bool IsMinionInPlay { get; set; }
		public int PlayerMinionCount { get; set; }
		public GameType CurrentGameType { get; set; }
		public Format? CurrentFormat { get; set; }
		public int ProposedAttacker { get; set; }
		public int ProposedDefender { get; set; }
		public bool? IsDungeonMatch { get; set; }
		public bool PlayerChallengeable { get; }
	}
}