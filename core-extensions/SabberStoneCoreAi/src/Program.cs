﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using SabberStoneCore.Config;
using SabberStoneCore.Enums;
using SabberStoneCore.Model;
using SabberStoneCore.Tasks;
using SabberStoneCore.Tasks.PlayerTasks;
using SabberStoneCoreAi.Meta;
using SabberStoneCoreAi.Nodes;
using SabberStoneCoreAi.Score;
using System.Threading;
using System.Threading.Tasks;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using System.IO;
//using System.Data.DataSet;

namespace SabberStoneCoreAi
{
	internal class Program
	{//this is the heavily modified code by me, Aditya. see comments in the code for clarity. If any more questions, then contact me on the email thread.
		private static readonly Random Rnd = new Random();
		static Dictionary<string, int> cardname = new Dictionary<string, int>();
		private static string locOfMaster = "D:\\GameInnovationLab\\SabberStone-master";//most important line, this indicates location of the project in your machine. edit here only, exactly as shown.
		private static void Main(string[] args)
		{

			//Console.WriteLine(Cards.FromName("This is an invalid name") == Cards.FromName("Default"));-ignore this, this was for testing

			Console.WriteLine("Starting test setup.");
			SabberStoneCoreAi.src.CreateAndMutate createMutateObj = new src.CreateAndMutate();//this is the class I added which contains all functions
			//this above object will help you mutate or create a deck, without worrying about underlying code.

			//OneTurn();-ignore this
			Dictionary<int, string> allCards = getAllCards();//important function, must be done in main before anything else, this will get all the valid hearthstone cards (1300+ cards in total) from the data file
			string[] results = new string[15];//max 15 tests, can be increased/changed without any problems, make sure max j * max i <= size of results array

			List<Card> victorious = new List<Card>(); 
			int parallelThreads = 1;// number of parallel running threads
			int testsInEachThread = 1;//number of games in each thread
			//you are advised not to set more than 3 parallel threads if you are doing this on your laptop, otherwise the laptop will not survive
			Parallel.For(0, parallelThreads, j =>
			  {
				  for (int i = testsInEachThread * j; i < (j + 1) * testsInEachThread; i++)
				  {
					  List<Card> playerDeck = createMutateObj.createRandomDeck(allCards, cardname);//this liine returns randomly created deck from all cards in hearthsone, which is passed as parameter 
					  results[i] = FullGame(playerDeck, i);
					  //if (results[i].Contains("Player1: WON"))-this is to get the deck only if it wins
					  {
						  victorious = playerDeck; //this gives the last deck that played the game
					  }
				  }
			  });
			for (int i = 0; i < 15; i++)//for 15 results here, if parallel threads * testInEachThread = 6, then 6 tests will show here
			{
				Console.WriteLine("Game " + i + " : " + results[i] + "\n");
			}
			Console.WriteLine("Before Mutation Deck:");
			createMutateObj.print(victorious);


			List<Card> myDeck = new List<Card>();
			myDeck = victorious;//myDeck can be anything, I have made it =victorious/last deck created in the loop.
			List<Card> mutated = createMutateObj.mutate(myDeck, allCards, cardname);//make your deck myDeck and pass it here to mutate it.
			
			//RandomGames(); - ignore this line
			Console.WriteLine("\n Mutated Deck: \n");
			createMutateObj.print(mutated);
			Console.WriteLine("Test end!");
			Console.ReadLine();
		}

		//create
		/*
		public static Dictionary<int, int> createDeck(Dictionary<int, string> allcards)
		{
			Dictionary<int, int> chosenDeck = new Dictionary<int, int>();
			Random rand = new Random();
			int Count = 0;
			while (Count != 30)
			{
				int oneRand = rand.Next(0, allcards.Count + 1);
				if (allcards.ContainsKey(oneRand)) //&& (Cards.FromName(allcards[oneRand])))
				{
					if (chosenDeck.ContainsKey(oneRand) && (chosenDeck[oneRand] == 1))
					{
						chosenDeck[oneRand] = 2;
						Count++;
					}
					else if (!chosenDeck.ContainsKey(oneRand))
					{
						chosenDeck.Add(oneRand, 1);
						Count++;
					}
				}
			}
			//List<Card> playerDeck = convertDictToList(chosenDeck, allcards);
			Console.WriteLine("Count:" + Count);
			return chosenDeck;
		}
		*/
		//build allcards in dictionary from txt file
		public static Dictionary<int, string> getAllCards()
		{
			Dictionary<int, string> allcards = new Dictionary<int, string>();
			
			string fileName = locOfMaster +"\\SabberStoneCore\\resources\\Data\\CardDefs.xml";
			if (System.IO.File.Exists(fileName))
			{
				int i = 1;
				XDocument doc = XDocument.Load(fileName);
				var authors = doc.Descendants("Entity").Descendants("Tag").Where(x => x.Attribute("name").Value == "CARDNAME").Elements("enUS");
				byte[] byteArray = Encoding.UTF8.GetBytes("D:\\GameInnovationLab\\xmldata test.txt");
				//byte[] byteArray = Encoding.ASCII.GetBytes(contents);
				MemoryStream stream = new MemoryStream(byteArray);
				System.IO.StreamWriter file = new System.IO.StreamWriter(stream);
				foreach (var author in authors)
				{
					
					if (!cardname.ContainsKey(author.Value) && (Cards.FromName(author.Value) != Cards.FromName("Default")))
					{
						
						cardname.Add(author.Value, i);
						allcards.Add(i, author.Value);
						i++;
						Console.WriteLine(author.Value);
					}
					
				}
				/*XmlReader xmlReader = XmlReader.Create(fileName);
			while (xmlReader.Read())
			{
				if ( (xmlReader.Name == "CARDNAME"))
				{
					if (xmlReader.HasAttributes)
					{

						allcards.Add(i, xmlReader.GetAttribute("enUS"));
						i++;
					}
				}
			}*/
				//D:\\GameInnovationLab\\xmldata test.txt


			}
			else
			{
				Console.WriteLine("File not found");
			}
			return allcards;
		}


		public static void RandomGames()
		{
			const int total = 100;
			var watch = Stopwatch.StartNew();

			int turns = 0;
			int[] wins = new[] { 0, 0 };
			for (int i = 0; i < total; i++)
			{
				var game = new Game(new GameConfig
				{
					StartPlayer = 1,
					Player1HeroClass = CardClass.MAGE,
					Player2HeroClass = CardClass.MAGE,
					FillDecks = true,
					Logging = false
				});
				game.StartGame();

				while (game.State != State.COMPLETE)
				{
					List<PlayerTask> options = game.CurrentPlayer.Options();
					PlayerTask option = options[Rnd.Next(options.Count)];
					//Console.WriteLine(option.FullPrint());
					game.Process(option);


				}
				turns += game.Turn;
				if (game.Player1.PlayState == PlayState.WON)
					wins[0]++;
				if (game.Player2.PlayState == PlayState.WON)
					wins[1]++;

			}
			watch.Stop();

			Console.WriteLine($"{total} games with {turns} turns took {watch.ElapsedMilliseconds} ms => " +
							  $"Avg. {watch.ElapsedMilliseconds / total} per game " +
							  $"and {watch.ElapsedMilliseconds / (total * turns)} per turn!");
			Console.WriteLine($"playerA {wins[0] * 100 / total}% vs. playerB {wins[1] * 100 / total}%!");
		}

		public static void OneTurn()
		{
			var game = new Game(
				new GameConfig()
				{
					StartPlayer = 1,
					Player1Name = "FitzVonGerald",
					Player1HeroClass = CardClass.WARRIOR,
					Player1Deck = Decks.AggroPirateWarrior,
					Player2Name = "RehHausZuckFuchs",
					Player2HeroClass = CardClass.SHAMAN,
					Player2Deck = Decks.MidrangeJadeShaman,
					FillDecks = false,
					Shuffle = false,
					SkipMulligan = false
				});
			game.Player1.BaseMana = 10;
			game.StartGame();

			var aiPlayer1 = new AggroScore();
			var aiPlayer2 = new AggroScore();

			game.Process(ChooseTask.Mulligan(game.Player1, aiPlayer1.MulliganRule().Invoke(game.Player1.Choice.Choices.Select(p => game.IdEntityDic[p]).ToList())));
			game.Process(ChooseTask.Mulligan(game.Player2, aiPlayer2.MulliganRule().Invoke(game.Player2.Choice.Choices.Select(p => game.IdEntityDic[p]).ToList())));

			game.MainReady();

			while (game.CurrentPlayer == game.Player1)
			{
				Console.WriteLine($"* Calculating solutions *** Player 1 ***");

				List<OptionNode> solutions = OptionNode.GetSolutions(game, game.Player1.Id, aiPlayer1, 10, 500);

				var solution = new List<PlayerTask>();
				solutions.OrderByDescending(p => p.Score).First().PlayerTasks(ref solution);
				Console.WriteLine($"- Player 1 - <{game.CurrentPlayer.Name}> ---------------------------");

				foreach (PlayerTask task in solution)
				{
					Console.WriteLine(task.FullPrint());
					game.Process(task);
					if (game.CurrentPlayer.Choice != null)
						break;
				}
			}

			Console.WriteLine(game.Player1.HandZone.FullPrint());
			Console.WriteLine(game.Player1.BoardZone.FullPrint());
		}

		//the game we need
		public static string FullGame(List<Card> player1Deck, int where)
		{
			var game = new Game(
				new GameConfig()
				{
					StartPlayer = 1,
					Player1Name = "FitzVonGerald",
					Player1HeroClass = CardClass.WARRIOR,
					Player1Deck = player1Deck,//Decks.AggroPirateWarrior,
					Player2Name = "RehHausZuckFuchs",
					Player2HeroClass = CardClass.SHAMAN,
					Player2Deck = Decks.MidrangeJadeShaman,
					FillDecks = false,
					Shuffle = true,
					SkipMulligan = false
				});
			game.StartGame();

			var aiPlayer1 = new AggroScore();
			var aiPlayer2 = new MidRangeScore();

			List<int> mulligan1 = aiPlayer1.MulliganRule().Invoke(game.Player1.Choice.Choices.Select(p => game.IdEntityDic[p]).ToList());
			List<int> mulligan2 = aiPlayer2.MulliganRule().Invoke(game.Player2.Choice.Choices.Select(p => game.IdEntityDic[p]).ToList());

			Console.WriteLine($"Player1: Mulligan {string.Join(",", mulligan1)}");
			Console.WriteLine($"Player2: Mulligan {string.Join(",", mulligan2)}");

			game.Process(ChooseTask.Mulligan(game.Player1, mulligan1));
			game.Process(ChooseTask.Mulligan(game.Player2, mulligan2));

			game.MainReady();

			while (game.State != State.COMPLETE)
			{
				Console.WriteLine("here:" + where);
				Console.WriteLine($"Player1: {game.Player1.PlayState} / Player2: {game.Player2.PlayState} - " +
								  $"ROUND {(game.Turn + 1) / 2} - {game.CurrentPlayer.Name}");
				Console.WriteLine($"Hero[P1]: {game.Player1.Hero.Health} / Hero[P2]: {game.Player2.Hero.Health}");
				Console.WriteLine("");
				while (game.State == State.RUNNING && game.CurrentPlayer == game.Player1)
				{
					Console.WriteLine($"* Calculating solutions *** Player 1 ***");
					List<OptionNode> solutions = OptionNode.GetSolutions(game, game.Player1.Id, aiPlayer1, 10, 500);
					var solution = new List<PlayerTask>();
					solutions.OrderByDescending(p => p.Score).First().PlayerTasks(ref solution);
					Console.WriteLine($"- Player 1 - <{game.CurrentPlayer.Name}> ---------------------------");
					foreach (PlayerTask task in solution)
					{
						Console.WriteLine(task.FullPrint());
						game.Process(task);
						if (game.CurrentPlayer.Choice != null)
						{
							Console.WriteLine($"* Recaclulating due to a final solution ...");
							break;
						}
					}
				}

				// Random mode for Player 2
				Console.WriteLine($"- Player 2 - <{game.CurrentPlayer.Name}> ---------------------------");
				while (game.State == State.RUNNING && game.CurrentPlayer == game.Player2)
				{
					//var options = game.Options(game.CurrentPlayer);
					//var option = options[Rnd.Next(options.Count)];
					//Log.Info($"[{option.FullPrint()}]");
					//game.Process(option);
					Console.WriteLine($"* Calculating solutions *** Player 2 ***");
					List<OptionNode> solutions = OptionNode.GetSolutions(game, game.Player2.Id, aiPlayer2, 10, 500);
					var solution = new List<PlayerTask>();
					solutions.OrderByDescending(p => p.Score).First().PlayerTasks(ref solution);
					Console.WriteLine($"- Player 2 - <{game.CurrentPlayer.Name}> ---------------------------");
					foreach (PlayerTask task in solution)
					{
						Console.WriteLine(task.FullPrint());
						game.Process(task);
						if (game.CurrentPlayer.Choice != null)
						{
							Console.WriteLine($"* Recaclulating due to a final solution ...");
							break;
						}
					}
				}
			}
			Console.WriteLine($"Game: {game.State}, Player1: {game.Player1.PlayState} / Player2: {game.Player2.PlayState}");
			return "Game: {game.State}, Player1: " + game.Player1.PlayState + " / Player2:" + game.Player2.PlayState;
		}

		/*
		//checks if a deck is valid when inserting a particular card (specified by int index)
		public static int checkValid(Dictionary<int, int> listToCheck, int index, Dictionary<int, string> allCards)
		{
			if (listToCheck.ContainsKey(index) && (listToCheck[index] == 1))
			{
				return 2;
			}
			else if (!listToCheck.ContainsKey(index))
			{
				return 1;
			}
			return -1;// if not valid insert
		}

		//self explanatory
		public static List<Card> convertDictToList(Dictionary<int, int> chosenDeck, Dictionary<int, string> allCards)
		{
			foreach (int key in chosenDeck.Keys)
			{
				Console.WriteLine("Key: {0} Value{1} \n", key, chosenDeck[key].ToString());
			}
			//Console.WriteLine("Count:" + Count);
			List<Card> playerDeck = new List<Card>();
			foreach (int key in chosenDeck.Keys)
			{
				for (int i = 1; i <= chosenDeck[key]; i++)
				{
					playerDeck.Add(Cards.FromName(allCards[key]));
					//Console.WriteLine(Cards.FromName(allCards[key]) == null);
				}
			}

			return playerDeck;
		}



		public static Dictionary<int, Dictionary<int, int>> mutation(Dictionary<int, Dictionary<int, int>> dictToMutate, Dictionary<int, string> allCards)
		{
			foreach (int key in dictToMutate.Keys)
			{

				Random rand = new Random();
				bool swapSuccess = false;
				while (!swapSuccess)
				{

					int oldCard = rand.Next(0, allCards.Count + 1);
					int newCard = rand.Next(0, allCards.Count + 1);
					if (dictToMutate[key].ContainsKey(oldCard) && allCards.ContainsKey(newCard) && (oldCard != newCard))
					{
						if (dictToMutate[key].ContainsKey(newCard) && dictToMutate[key][newCard] == 2)
						{
							swapSuccess = false;
						}
						else if (dictToMutate[key].ContainsKey(newCard) && dictToMutate[key][newCard] == 1)
						{
							swapSuccess = true;
							dictToMutate[key][newCard] = 2;
						}
						else if (!dictToMutate[key].ContainsKey(newCard))
						{
							swapSuccess = true;
							dictToMutate[key].Add(newCard, 1);
						}
						else
						{
							swapSuccess = false;
						}
						if (swapSuccess)
						{
							if (dictToMutate[key][oldCard] == 2)
							{
								dictToMutate[key][oldCard] = 1;
							}
							else if (dictToMutate[key][oldCard] == 1)
							{
								dictToMutate[key].Remove(oldCard);
							}
						}
					}
					else
					{
						swapSuccess = false;
					}
				}
			}
			return dictToMutate;

		}


		public static void print(Dictionary<int, Dictionary<int, int>> printDict)
		{
			foreach (int key in printDict.Keys)
			{
				Console.WriteLine("number " + key);
				foreach (int inKey in printDict[key].Keys)
				{
					Console.WriteLine(inKey + " : " + printDict[key][inKey]);
				}
			}
		}*/
	}
}
