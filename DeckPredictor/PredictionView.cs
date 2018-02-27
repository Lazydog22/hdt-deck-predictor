﻿using Hearthstone_Deck_Tracker.Hearthstone;
using Hearthstone_Deck_Tracker.Utility.Extensions;
using Hearthstone_Deck_Tracker.Utility.Logging;
using Hearthstone_Deck_Tracker;
using System.Diagnostics;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System;

namespace DeckPredictor
{
	public class PredictionView
	{
		private IOpponent _opponent;

		public PredictionView(IOpponent opponent)
		{
			_opponent = opponent;
		}

		public void OnPredictionUpdate(IPredictor predictor)
		{
			_opponent.UpdatePredictedCards(new List<Card>());
		}
	}
}
