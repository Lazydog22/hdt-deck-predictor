﻿using Hearthstone_Deck_Tracker.API;
using Hearthstone_Deck_Tracker.Enums;
using Hearthstone_Deck_Tracker.Hearthstone;
using Hearthstone_Deck_Tracker.Plugins;
using Hearthstone_Deck_Tracker;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Controls;
using System;

namespace DeckPredictor
{
	public class DeckPredictorPlugin : IPlugin
	{
		public static readonly string DataDirectory = Path.Combine(Config.AppDataPath, "DeckPredictor");
		private static readonly string LogDirectory = Path.Combine(DataDirectory, "Logs");
		private static readonly string ReleaseUrl =
			"https://github.com/fatheroctopus/hdt-deck-predictor/releases";

		private PluginConfig _config;
		private ReadOnlyCollection<Deck> _metaDecks;
		private PredictionController _controller;
		private PredictionView _view;

		public string Author
		{
			get { return "fatheroctopus.bandcamp.com"; }
		}

		public string Description
		{
			get { return "Predicts the contents of the opponent's deck."; }
		}

		public MenuItem MenuItem
		{
			get { return null; }
		}

		public string Name
		{
			get { return "Deck Predictor"; }
		}

		public string ButtonText
		{
			get { return "Latest Release"; }
		}

		public void OnButtonPress()
		{
			System.Diagnostics.Process.Start(ReleaseUrl);
		}

		public void OnLoad()
		{
			Log.Initialize();
			Log.Debug("Starting");
			if (!Directory.Exists(DataDirectory))
			{
				Directory.CreateDirectory(DataDirectory);
			}
			CustomLog.Initialize(LogDirectory);

			_config = PluginConfig.Load();

			// Synchronously retrieve our meta decks and keep them in memory.
			var metaRetriever = new MetaRetriever();
			var retrieveTask =
				Task.Run<List<Deck>>(async () => await metaRetriever.RetrieveMetaDecks(_config));
			_metaDecks = new ReadOnlyCollection<Deck>(retrieveTask.Result);
			_view = new PredictionView();

			GameEvents.OnGameStart.Add(() =>
				{
					var format = Hearthstone_Deck_Tracker.Core.Game.CurrentFormat;
					var mode = Hearthstone_Deck_Tracker.Core.Game.CurrentGameMode;
					if (format == Format.Standard &&
						(mode == GameMode.Ranked || mode == GameMode.Casual || mode == GameMode.Friendly))
					{
						Log.Info("Enabling DeckPredictor for " + format + " " + mode + " game");
						var opponent = new Opponent(Hearthstone_Deck_Tracker.Core.Game);
						_controller = new PredictionController(opponent, _metaDecks);
						_view.SetEnabled(true);
						_controller.OnPredictionUpdate.Add(_view.OnPredictionUpdate);
					}
					else
					{
						Log.Info("No deck predictions for " + format + " " + mode + " game");
					}
				});
			GameEvents.OnInMenu.Add(() =>
				{
					if (_controller != null)
					{
						_view.SetEnabled(false);
						Log.Debug("Disabling DeckPredictor for end of game");
					}
					_controller = null;
				});
			GameEvents.OnOpponentDraw.Add(() => _controller?.OnOpponentDraw());
			GameEvents.OnTurnStart.Add(activePlayer => _controller?.OnTurnStart(activePlayer));

			// Events that reveal cards need a 100ms delay. This is because HDT takes some extra
			// time to process all the tags we need, but it doesn't wait to send these callbacks.
			int delayMs = 100;
			GameEvents.OnOpponentPlay.Add(async card =>
				{
					await Task.Delay(delayMs);
					_controller?.OnOpponentPlay(card);
				});
			GameEvents.OnOpponentHandDiscard.Add(async card =>
				{
					await Task.Delay(delayMs);
					_controller?.OnOpponentHandDiscard(card);
				});
			GameEvents.OnOpponentDeckDiscard.Add(async card =>
				{
					await Task.Delay(delayMs);
					_controller?.OnOpponentDeckDiscard(card);
				});
			GameEvents.OnOpponentSecretTriggered.Add(async card =>
				{
					await Task.Delay(delayMs);
					_controller?.OnOpponentSecretTriggered(card);
				});
			GameEvents.OnOpponentJoustReveal.Add(async card =>
				{
					await Task.Delay(delayMs);
					_controller?.OnOpponentJoustReveal(card);
				});
			GameEvents.OnOpponentDeckToPlay.Add(async card =>
				{
					await Task.Delay(delayMs);
					_controller?.OnOpponentDeckToPlay(card);
				});
		}

		public void OnUnload()
		{
			_config.Save();
			_view.OnUnload();
		}

		public void OnUpdate()
		{
		}

		public Version Version
		{
			get { return new Version(0, 3, 0); }
		}
	}
}
