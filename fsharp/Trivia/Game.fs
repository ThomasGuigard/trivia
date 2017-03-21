﻿
namespace UglyTrivia

open System;
open System.Collections.Generic;
open System.Linq;
open System.Text;

type Game() as this =

    let players = List<string>()

    let places = Array.create 6 0
    let purses = Array.create 6 0

    let inPenaltyBox = Array.create 6 false

    let popQuestions = LinkedList<string>()
    let scienceQuestions = LinkedList<string>()
    let sportsQuestions = LinkedList<string>()
    let rockQuestions = LinkedList<string>()

    let mutable currentPlayer = 0;

    let nextPlayer () =
            currentPlayer <- currentPlayer + 1;
                if currentPlayer = players.Count then currentPlayer <- 0;

    let winAPurse () =
        purses.[currentPlayer] <- purses.[currentPlayer] + 1;
        Console.WriteLine(players.[currentPlayer]
                            + " now has "
                            + purses.[currentPlayer].ToString()
                            + " Gold Coins.");
    
    let movePlayer roll =
        places.[currentPlayer] <- places.[currentPlayer] + roll;
        if places.[currentPlayer] > 11 then places.[currentPlayer] <- places.[currentPlayer] - 12;

        Console.WriteLine(players.[currentPlayer]
                            + "'s new location is "
                            + places.[currentPlayer].ToString());
        Console.WriteLine("The category is " + this.currentCategory());
    let mutable isGettingOutOfPenaltyBox = false;

    do
        for i = 1 to 50 do
            popQuestions.AddLast("Pop Question " + i.ToString()) |> ignore
            scienceQuestions.AddLast("Science Question " + i.ToString()) |> ignore
            sportsQuestions.AddLast("Sports Question " + i.ToString()) |> ignore
            rockQuestions.AddLast(this.createRockQuestion(i)) |> ignore
    
    member this.createRockQuestion(index: int): string =
        "Rock Question " + index.ToString()

    member this.isPlayable(): bool =
        this.howManyPlayers() >= 2

    member this.add(playerName: String): bool =
        players.Add(playerName);
        places.[this.howManyPlayers()] <- 0;
        purses.[this.howManyPlayers()] <- 0;
        inPenaltyBox.[this.howManyPlayers()] <- false;
        true

    member this.howManyPlayers(): int =
        players.Count;

    member this.roll (roll: int) currentPlayer =
        Console.WriteLine(players.[currentPlayer] + " is the current player");
        Console.WriteLine("They have rolled a " + roll.ToString());

        if inPenaltyBox.[currentPlayer] then
            if roll % 2 <> 0 then
                isGettingOutOfPenaltyBox <- true;

                Console.WriteLine(players.[currentPlayer].ToString() + " is getting out of the penalty box");
                movePlayer roll
                this.askQuestion();
               
            else
                Console.WriteLine(players.[currentPlayer].ToString() + " is not getting out of the penalty box");
                isGettingOutOfPenaltyBox <- false;

        else
            movePlayer roll
            this.askQuestion();

    member private this.askQuestion() =
        if this.currentCategory() = "Pop" then
            Console.WriteLine(popQuestions.First.Value);
            popQuestions.RemoveFirst();
            
        if this.currentCategory() = "Science" then
            Console.WriteLine(scienceQuestions.First.Value);
            scienceQuestions.RemoveFirst();
        
        if this.currentCategory() = "Sports" then
            Console.WriteLine(sportsQuestions.First.Value);
            sportsQuestions.RemoveFirst();

        if this.currentCategory() = "Rock" then
            Console.WriteLine(rockQuestions.First.Value);
            rockQuestions.RemoveFirst();


    member private this.currentCategory(): String =
        match places.[currentPlayer] % 4 with
        | 0 -> "Pop"
        | 1 -> "Science"
        | 2 -> "Sports"
        | _ -> "Rock"        
    member this.wasCorrectlyAnswered(): bool =
               
        if inPenaltyBox.[currentPlayer] then
            if isGettingOutOfPenaltyBox then
                Console.WriteLine("Answer was correct!!!!");
                winAPurse ()

                let winner = this.didPlayerWin();
                nextPlayer()
                winner;
            else
                nextPlayer()
                true;
        else

            Console.WriteLine("Answer was corrent!!!!");
            winAPurse()

            let winner = this.didPlayerWin();
            nextPlayer()
            winner;

    member this.wrongAnswer(): bool=
        Console.WriteLine("Question was incorrectly answered");
        Console.WriteLine(players.[currentPlayer] + " was sent to the penalty box");
        inPenaltyBox.[currentPlayer] <- true;

        nextPlayer()
        true;


    member private this.didPlayerWin(): bool =
        not (purses.[currentPlayer] = 6);

module NewGame =
    type Player = {
        Name: string
    }
    type RunningGame = {
        Players: Player list
        CurrentPlayer: Player
    }
    type GameState = NewGame | RunningGame of RunningGame | PlayerWon of Player

    let addPlayer name (oldGame: Game) game =
        let newPlayer = { Name = name }
        let runningGame = 
            match game with
            | NewGame -> { Players = [ newPlayer ]; CurrentPlayer = newPlayer }
            | RunningGame old -> { old with Players = List.append old.Players [{ Name = name }] }
            | PlayerWon _ -> failwith "Impossible to play a won game!!"
        oldGame.add(name) |> ignore
        printfn "%s was added" name
        printfn "They are player number %i" (runningGame.Players.Count())
        RunningGame runningGame

    let private nextPlayer players currentIndex =
        let nextIndex = 
            if currentIndex + 1 < List.length players 
            then currentIndex + 1 
            else 0
        List.nth players nextIndex

    let roll diceValue (oldGame: Game) game =
        let currentPlayerIndex = game.Players |> List.findIndex (fun x -> x = game.CurrentPlayer)
        oldGame.roll diceValue currentPlayerIndex
        { game with CurrentPlayer = (nextPlayer game.Players currentPlayerIndex) }

    let answerIncorrectly (oldGame: Game) game =
        oldGame.wrongAnswer() |> ignore
        RunningGame game

    let answerCorrectly (oldGame: Game) game =
        if oldGame.wasCorrectlyAnswered() then
            RunningGame game
        else
            PlayerWon game.CurrentPlayer

module GameRunner = 
    open NewGame

    [<EntryPoint>]
    let main (argv : string array) = 
        let mutable isFirstRound = true;
        let mutable notAWinner = false;
        let aGame = Game();
        let game = 
            NewGame
            |> addPlayer "Chet" aGame
            |> addPlayer "Pat" aGame
            |> addPlayer "Sue" aGame

        let rand = new Random(argv.[0] |> int);

        let rec play (runningGame : RunningGame) =
            let pendingAnswer = runningGame |> roll (rand.Next(5) + 1) aGame
            let newState = 
                if (rand.Next(9) = 7) then
                    pendingAnswer |> answerIncorrectly aGame
                else
                    pendingAnswer |> answerCorrectly aGame
            match newState with
            | NewGame -> failwith "Impossible to play a game not started..."
            | RunningGame runningGame -> play runningGame
            | PlayerWon _ -> ()

        match game with
        | NewGame -> failwith "Impossible to play a game not started..."
        | RunningGame runningGame -> play runningGame
        | PlayerWon _ -> failwith "Impossible to play a won game!"
        0
