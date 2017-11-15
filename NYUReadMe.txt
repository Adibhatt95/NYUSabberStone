NYU Research - the readMe file you need for our project
Make sure you run SabberStoneCoreAi

What this is:
Aditya has created a framework over sabberstone that will enable us to perform functions necessary for our research project

this includes all funtions in file "Create and Mutate" which need not be accessed at all

an object of the above class is present in the program.cs code which is used to call it

program.cs is the code which needs to be used for our project

all comments in program.cs describe how to use it

if any questions, then contact me (Aditya) 

if any questions about sabberstone, can ask me but best people to ask are sabberstone devs themselves at: https://discordapp.com/channels/195326447118712832/303272962444754956


a quick summary of what you must do:
-if you directly run, it will not work.
-you MUST edit string locOfMaster to give file path on your machine
-then run it, it will begin one game between a randomly created deck and a pre-determined deck
-one game because initially 
int parallelThreads = 1;// number of parallel running threads
int testsInEachThread = 1;//number of games in each thread

to create a deck: 
-List<Card> playerDeck = createMutateObj.createRandomDeck(allCards, cardname);//this liine returns randomly created deck from all cards in hearthsone, which is passed as parameter 
-results[i] = FullGame(playerDeck, i);

please note: allcards is the dictionary containing all possible cards in hearthstone, which it gets from calling 
Dictionary<int, string> allCards = getAllCards();//important function, must be done in main before anything else, this will get all the valid hearthstone cards (1300+ cards in total) from the data file
cardname is another dictionary which is similar to allCards and required by createMutateObj.createRandomDeck();

to mutate a deck:
take your deck and put it in this function:
List<Card> mutated = createMutateObj.mutate(myDeck, allCards, cardname);//make your deck myDeck and pass it here to mutate it.

your deck will be mutated and given back to you.

note: the deck gets mutated by choosing random card from myDeck and swaps it with random card IN allCards, which is the reason it is passed to the mutate function

