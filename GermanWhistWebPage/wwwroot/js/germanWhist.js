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
            img.src = staticURL + "back.svg";
            img.classList.add("opponent-hand-card");
            cardBackImage.push(img)
        }

        document.getElementById("opponent-card-row").replaceChildren(...cardBackImage)
    }

    drawUserCards() {
        var playerHand = gameState.playerHandCardIdList;

        var cardImages = [];
        for (let i = playerHand.length - 1; i >= 0; i--) {
            let card = cardsList[playerHand[i]];
            var img = document.createElement("img");
            img.src = staticURL + card.fileName;
            img.setAttribute("card-id", card.id);
            img.classList.add("player-hand-card");
            if (card.id == gameState.playersNewCardId) {
                img.classList.add("new-hand-card");
            }
            if (gameState.currentPlayer === userPlayerId) {
                if (gameState.validPlayerMoveIds.includes(card.id)) {
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
        var topCard = gameState.stackTopCardId;
        if (gameState.stackTopCardId === null) {
            document.getElementById("top-card-container").innerHTML = "";
            var textContainer = document.getElementById("top-card-and-round-score-text");

            var roundScore = gameState.roundScores;
            textContainer.innerHTML = "Round Score: " + roundScore[0] + ":" + roundScore[1]
        }
        else {
            topCard = cardsList[gameState.stackTopCardId]
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
        var playedCards = gameState.playedCardIds;

        let trickWinner = gameState.trickWinner;

        this.fillContainerWithPlayedCard("player-played-card-container",
            playedCards[userPlayerId],
            trickWinner === userPlayerId);
        this.fillContainerWithPlayedCard("opponent-played-card-container",
            playedCards[opponentPlayerId],
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
        }
    }

    drawTrumpColor() {
        var trumpColor = gameState.trumpColor;
        var img = document.createElement("img");
        img.src = staticURL + trumpColor + ".PNG";
        document.getElementById("trump-indicator-container").replaceChildren(img);
    }

    drawTotalScore() {
        var totalScore = gameState.totalScores;
        document.getElementById("player-score").innerHTML = totalScore[0];
        document.getElementById("opponent-score").innerHTML = totalScore[1];
        document.getElementById("target-score").innerHTML = gameState.targetScore;
    }

}


class GameController {
    constructor() { }

    setDrawer(drawer) {
        this.drawer = drawer;
    }

    async playCard(cardId) {
        try {
            const response = await fetch(`/games/german-whist/game-states/${gameStateId}/played-cards`, {
                method: "PUT",
                headers: {
                    "Content-Type": "application/json",
                },
                body: JSON.stringify({ "cardId": cardId },)
            });

            if (!response.ok) {
                throw new Error("Network response was not ok");
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
        gameState = await this.playCard(clickedCardId);
        this.drawer.drawGameState();
    }

    async getNewGameState() {
        try {
            const response = await fetch(`/games/german-whist/game-states/${gameStateId}`, {
                method: "GET",
                headers: {
                    "Content-Type": "application/json",
                },
            });

            if (!response.ok) {
                throw new Error("Network response was not ok");
            }
            const gameState = await response.json();
            console.log("Card played:", gameState);
            return gameState
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
        gameState = await this.getNewGameState();
        this.drawer.drawGameState();
    }

}


async function goToMenu() {
    window.location.href = "/";
}

const userPlayerId = 0;
const opponentPlayerId = 1;
var gameController = new GameController();
var drawer = new Drawer();
gameController.setDrawer(drawer);
drawer.setGameController(gameController);

document.addEventListener("DOMContentLoaded", (event) => {
    drawer.drawGameState()
    document.getElementById("menu-button").addEventListener("click", goToMenu)
})

