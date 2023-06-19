"use strict"

class Drawer {
    constructor() { }

    setGameController(gameController) {
        this.gameController = gameController;
    }

    drawGameState() {
        if (gameState.hasGameEnded) {
            var scores = gameState.totalScores;
            if (gameState.totalScorePlayer > gameState.totalScoreOpponent) {
                alert("Congratulations, you won " + gameState.totalScorePlayer + " to " + gameState.totalScoreOpponent);
            }
            else {
                alert("You lost " + gameState.totalScorePlayer + " to " + gameState.totalScoreOpponent);
            }
            // gameController.deleteGameState()
        }
        document.getElementById("changePlayerButton").innerHTML = `Change to view of Player ${opponentPlayerId}`
        document.getElementById("newGameButton").innerHTML = "Start new Game (Current Game id = " + gameState.id + ")";
        this.drawOpponentCards();
        this.drawUserCards();
        this.drawStackTopCardAndRoundScore();
        this.drawPlayedCards();
        this.drawTrumpColor();
        this.drawTotalScore();

    }

    drawOpponentCards() {
        var cardBackImage = []
        for (let i = 0; i < gameState.numberOfHandCardsOpponent; i++) {
            var img = document.createElement("img");
            img.src = "/back.svg";
            img.classList.add("opponent-hand-card");
            cardBackImage.push(img)
        }

        document.getElementById("opponent-card-row").replaceChildren(...cardBackImage)
    }

    drawUserCards() {
        var playerHand = gameState.hand;

        var cardImages = [];
        for (let i = playerHand.length - 1; i >= 0; i--) {
            let card = cardsList[playerHand[i]];
            var img = document.createElement("img");
            img.src = staticURL + card.fileName;
            img.setAttribute("card-id", card.id);
            img.classList.add("player-hand-card");
            if (card.id == gameState.newHandCardId) {
                img.classList.add("new-hand-card");
            }
            if (gameState.isPlayerCurrentPlayer) {
                if (gameState.validMoves.includes(card.id)) {
                    img.classList.add("valid-move")
                    img.addEventListener("click", (event) => this.gameController.userClicksCard(event))
                }
                else {
                    img.classList.add("not-valid-move")
                }
            }
            cardImages.push(img)
        }

        document.getElementById("user-card-row").replaceChildren(...cardImages)
    }

    drawStackTopCardAndRoundScore() {
        var topCard = gameState.topCardId;
        if (gameState.topCardId === null) {
            document.getElementById("top-card-container").innerHTML = "";
            var textContainer = document.getElementById("top-card-and-round-score-text");

            var roundScore = gameState.roundScores;
            textContainer.innerHTML = "Round Score: " + gameState.totalScorePlayer + ":" + gameState.totalScoreOpponent;
        }
        else {
            let topCard = cardsList[gameState.topCardId];
            var img = document.createElement("img");
            img.src = staticURL + topCard.fileName;
            img.classList.add("top-card")
            document.getElementById("top-card-container").replaceChildren(img);
            document.getElementById("top-card-and-round-score-text").innerHTML = "Top Card of Stack:";
        }
    }

    fillContainerWithPlayedCard(containerName, cardId, isTrickWinner) {
        if (cardId === null) {
            document.getElementById(containerName).replaceChildren([]);
            return;
        }

        let card = cardsList[cardId];
        var img = document.createElement("img");
        img.src = staticURL + card.fileName;
        if (isTrickWinner) {
            img.classList.add("winning-played-card");
        }
        document.getElementById(containerName).replaceChildren(img);
    }

    drawPlayedCards() {
        let removeButtonContainerId = "remove-cards-button-container"

        if (gameController.canCardsBeRemoved) {
            let button = document.createElement("button");
            button.id = "remove-cards-button"
            button.classList.add("btn");
            button.classList.add("btn-primary");
            button.innerHTML = "Remove Cards"
            button.addEventListener("click", (event) => this.gameController.removeCards())
            document.getElementById(removeButtonContainerId).replaceChildren(button);

            let trickWinner = gameState.trickWinner;

            this.fillContainerWithPlayedCard("player-played-card-container",
                gameState.previousPlayedCardIdPlayer,
                gameState.isPlayerPreviousTrickWinner);
            this.fillContainerWithPlayedCard("opponent-played-card-container",
                gameState.previousPlayedCardIdOpponent,
                !gameState.isPlayerPreviousTrickWinner);
        }
        else {
            this.fillContainerWithPlayedCard("player-played-card-container",
                gameState.playedCardIdPlayer,
                false);
            this.fillContainerWithPlayedCard("opponent-played-card-container",
                gameState.playedCardIdOpponent,
                false);

            if (gameController.waitingForOtherPlayer) {
                document.getElementById(removeButtonContainerId).innerHTML = "Waiting for other Player";
            }
            else {
                document.getElementById(removeButtonContainerId).innerHTML = "";
            }
        }
    }

    drawTrumpColor() {
        var img = document.createElement("img");
        img.src = staticURL + gameState.trumpSuitName.toLowerCase() + ".PNG";
        document.getElementById("trump-indicator-container").replaceChildren(img);
    }

    drawTotalScore() {
        var totalScore = gameState.totalScores;
        document.getElementById("player-score").innerHTML = gameState.totalScorePlayer;
        document.getElementById("opponent-score").innerHTML = gameState.totalScoreOpponent;
        document.getElementById("target-score").innerHTML = gameState.targetScore;
    }
}


class GameController {
    waitingForOtherPlayer;
    canCardsBeRemoved;
    constructor() { }

    setDrawer(drawer) {
        this.drawer = drawer;
        this.canCardsBeRemoved = false;
    }

    async playCard(cardId) {
        try {
            const response = await fetch(`/api/games/GermanWhist/${gameId}/move`, {
                method: "POST",
                headers: {
                    "Content-Type": "application/json",
                },
                body: JSON.stringify({
                    playerId: currentViewPlayerId,
                    cardId: cardId
                })
            });

            if (!response.ok) {
                throw new Error("Network response was not ok");
                console.log("Network error response:", await response.json());
            }

            const gameState = await response.json();
            console.log("Card played:", gameState);
            gameController.canCardsBeRemoved = false;
            if (!gameState.isTrickOngoing) {
                gameController.canCardsBeRemoved = true;
            }
            else {
                if (!gameState.isPlayerCurrentPlayer) {
                    gameController.startPollingGameState();
                }
            }
            return gameState
        } catch (error) {
            console.error("There was a problem with the fetch operation:", error);
        }
    }

    async userClicksCard(event) {
        console.log("Card clicked");
        var clickedCardId = event.target.getAttribute("card-id");
        gameState = await this.playCard(parseInt(clickedCardId));
        this.drawer.drawGameState();
    }

    async getPlayerView() {
        try {
            const response = await fetch(`/api/games/GermanWhist/${gameId}/player-view/?PlayerId=${currentViewPlayerId}`, {
                method: "GET",
                headers: {
                    "Content-Type": "application/json",
                },
            });

            if (!response.ok) {
                throw new Error("Network response was not ok");
            }
            const gameState = await response.json();
            return gameState
        } catch (error) {
            console.error("There was a problem with the fetch operation:", error);
        }
    }

    async getCardsList() {
        try {
            const response = await fetch(`/api/games/GermanWhist/Cards`, {
                method: "GET",
                headers: {
                    "Content-Type": "application/json",
                },
            });

            if (!response.ok) {
                throw new Error("Network response was not ok");
            }
            const cardsList = await response.json();
            console.log("Card played:", cardsList);
            return cardsList
        } catch (error) {
            console.error("There was a problem with the fetch operation:", error);
        }
    }

    async deleteGameState() {
        try {
            const response = await fetch(`/api/games/GermanWhist/${gameStateId}`, {
                method: "DELETE",
                headers: {
                    "Content-Type": "application/json",
                },
            });

            if (!response.ok) {
                throw new Error("Network response was not ok");
            }
        } catch (error) {
            console.error("There was a problem with the delete operation:", error);
        }
    }

    removeCards() {
        this.canCardsBeRemoved = false;
        if (!gameState.isPlayerCurrentPlayer) {
            this.startPollingGameState();
        }
        this.drawer.drawGameState();
    }

    hasGameStateChanged(newGameState) {
        return (
            newGameState.previousPlayedCardIdOpponent != gameState.previousPlayedCardIdOpponent ||
            newGameState.playedCardIdOpponent != gameState.playedCardIdOpponent ||
            newGameState.previousPlayedCardIdPlayer != gameState.previousPlayedCardIdPlayer ||
            newGameState.playedCardIdPlayer != gameState.playedCardIdPlayer
        )
    }

    async startPollingGameState() {
        this.waitingForOtherPlayer = true;
        while (this.waitingForOtherPlayer) {
            await new Promise(resolve => setTimeout(resolve, 500));
            let newGameState = await this.getPlayerView();
            if (this.hasGameStateChanged(newGameState)) {
                gameState = newGameState;
                this.waitingForOtherPlayer = false;
                if (!gameState.isTrickOngoing) {
                    this.canCardsBeRemoved = true;
                }
                drawer.drawGameState();
            }
        }
    }

    async startNewGame() {
        try {
            const response = await fetch(`/api/games/GermanWhist`, {
                method: "POST",
                headers: {
                    "Content-Type": "application/json",
                },
                body: JSON.stringify({
                    player1Id: player1Id,
                    player2Id: player2Id
                })

            });

            if (!response.ok) {
                throw new Error("Network response was not ok");
            }
            const newGame = await response.json();
            console.log("New Game Started:", newGame);
            return newGame
        } catch (error) {
            console.error("There was a problem with the delete operation:", error);
        }
    }

}

function addFileNamesToCards(cardsList) {
    for (let i = 0; i < cardsList.length; i++) {
        let card = cardsList[i];
        card.fileName = "/cards/" + card.name + "_of_" + card.suitName.toLowerCase() + ".svg"
        cardsList[i] = card;
    }
}

async function changePlayer() {
    if (currentViewPlayerId == player1Id) {
        currentViewPlayerId = player2Id;
        opponentPlayerId = player1Id;
    }
    else {
        currentViewPlayerId = player1Id;
        opponentPlayerId = player2Id;
    }

    gameState = await gameController.getPlayerView();
    if (!gameState.isPlayerCurrentPlayer) {
        gameController.startPollingGameState();
    }
    else {
        gameController.waitingForOtherPlayer = false;
    }
    drawer.drawGameState()
}


var gameId = 2;
const player1Id = 1;
const player2Id = 2;

var currentViewPlayerId = player1Id;
var opponentPlayerId = player2Id;

var gameState;
var cardsList;

var gameController = new GameController();
var drawer = new Drawer();
gameController.setDrawer(drawer);
drawer.setGameController(gameController);

document.addEventListener("DOMContentLoaded", async (event) => {
    gameState = await gameController.getPlayerView();
    cardsList = await gameController.getCardsList();
    addFileNamesToCards(cardsList);
    document.getElementById("changePlayerButton").addEventListener("click", changePlayer)
    if (!gameState.isPlayerCurrentPlayer) {
        gameController.startPollingGameState();
    }
    drawer.drawGameState()
})

