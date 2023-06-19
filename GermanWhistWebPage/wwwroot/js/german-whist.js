"use strict"

class Drawer {
    constructor() { }

    setGameController(gameController) {
        this.gameController = gameController;
    }

    drawGameState() {
        if (gameState.hasGameEnded) {
            var scores = gameState.totalScores;
            if (scores[0] > scores[1]) {
                alert("Congratulations, you won " + scores[0] + " to " + scores[1]);
            }
            if (scores[0] < scores[1]) {
                alert("You lost " + scores[0] + " to " + scores[1]);
            }
            if (scores[0] == scores[1]) {
                alert("You played to a draw: " + scores[0] + " to " + scores[1]);
            }
            gameController.deleteGameState()
            window.location.href = "/";
        }
        document.getElementById("changePlayerButton").innerHTML = `Change to view of Player ${opponentPlayerId}`
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
            if (gameState.currentPlayerId === currentViewPlayerId) {
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
            textContainer.innerHTML = "Round Score: " + roundScore[0] + ":" + roundScore[1]
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
        let trickWinner = gameState.trickWinner;

        this.fillContainerWithPlayedCard("player-played-card-container",
            gameState.playedCardIdPlayer,
            trickWinner === currentViewPlayerId);
        this.fillContainerWithPlayedCard("opponent-played-card-container",
            gameState.playedCardIdOpponent,
            trickWinner === opponentPlayerId);

        let removeButtonContainerId = "remove-cards-button-container"

        

        if (gameState.cardsCanBeRemoved) {
            let button = document.createElement("button");
            button.id = "remove-cards-button"
            button.classList.add("btn");
            button.classList.add("btn-primary");
            button.innerHTML = "Remove Cards"
            button.addEventListener("click", (event) => this.gameController.removeCards())
            document.getElementById(removeButtonContainerId).replaceChildren(button);
        }
        else {

            document.getElementById(removeButtonContainerId).replaceChildren([]);

            if (gameState.currentPlayerId != currentViewPlayerId) {
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
    isPollingGameState;
    constructor() { }

    setDrawer(drawer) {
        this.drawer = drawer;
        this.isPollingGameState = true;
    }

    async playCard(cardId) {
        try {
            let bdy = JSON.stringify({
                playerId: currentViewPlayerId,
                cardId: cardId
            })                        
            const response = await fetch(`/api/games/GermanWhist/${gameId}/move`, {
                method: "POST",
                headers: {
                    "Content-Type": "application/json",
                },
                body: bdy
            });

            if (!response.ok) {
                throw new Error("Network response was not ok");
                console.log("Network error response:", await response.json(););
            }

            const gameState = await response.json();
            console.log("Card played:", gameState);
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
            const response = await fetch(`/games/german-whist/game-states/${gameStateId}`, {
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

    async removeCards() {
        gameState = await this.getPlayerView();
        this.drawer.drawGameState();
    }

    async  pollGameState() {
        while (this.isPollingGameState) {
            await new Promise(resolve => setTimeout(resolve, 500));
            gameState = await this.getPlayerView();
            drawer.drawGameState();
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
    drawer.drawGameState()
}



const gameId = 1;
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
    drawer.drawGameState()
    document.getElementById("changePlayerButton").addEventListener("click", changePlayer)
    gameController.pollGameState()
})

