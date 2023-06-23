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
                    "Authorization": `Bearer ${JWTToken}`,
                },
                body: JSON.stringify({
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
            console.error("There was a problem with the play card operation:", error);
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
            const response = await fetch(`/api/games/GermanWhist/${gameId}/player-view/`, {
                method: "GET",
                headers: {
                    "Content-Type": "application/json",
                    "Authorization": `Bearer ${JWTToken}`,
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
            console.log("Card List Received:", cardsList);
            return cardsList
        } catch (error) {
            console.error("There was a problem with the get cards list operation:", error);
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
}

function addFileNamesToCards(cardsList) {
    for (let i = 0; i < cardsList.length; i++) {
        let card = cardsList[i];
        card.fileName = "/cards/" + card.name + "_of_" + card.suitName.toLowerCase() + ".svg"
        cardsList[i] = card;
    }
}

async function startNewGameAgainstHumanPlayer() {
    var JsonBody = JSON.stringify({
        opponentPlayerId: null,
        againstBotOpponent: false,
        botDifficulty: 0
    });
    await startNewGame(JsonBody);
}

async function startNewGameAgainstBotPlayer() {
    var JsonBody = JSON.stringify({
        opponentPlayerId: null,
        againstBotOpponent: true,
        botDifficulty: 0
    });
    await startNewGame(JsonBody);
}

async function startNewGame(JsonBody) {
    try {
        const response = await fetch(`/api/games/GermanWhist`, {
            method: "POST",
            headers: {
                "Content-Type": "application/json",
                "Authorization": `Bearer ${JWTToken}`
            },
            body: JsonBody

        });

        if (!response.ok) {
            throw new Error("Network response was not ok");
        }
        const newGameResponse = await response.json();
        console.log("New Game Started:", newGameResponse);

        gameId = newGameResponse.id;
        playerId = newGameResponse.userPlayerId;
        drawGameView()
        return;

    } catch (error) {
        console.error("There was a problem with the start game operation:", error);
    }
}


async function loginPlayer(event) {
    event.preventDefault();
    let userName = document.getElementById("username").value;
    let password = document.getElementById("password").value;
    console.log("start login");
    try {
        const response = await fetch(`/api/Users/BearerToken`, {
            method: "POST",
            headers: {
                "Content-Type": "application/json",
            },
            body: JSON.stringify({
                userName: userName,
                password: password
            }),
        });
        console.log("Response:", response);

        if (!response.ok) {
            throw new Error("Network response was not ok", response);
        }
        const loginResponse = await response.json();
        console.log("Login Successful");
        JWTToken = loginResponse.token;
        drawMenuView()
        return;
    } catch (error) {
        console.error("There was a problem with the login operation:", error);
    }
}

async function JoinGame(event) {
    var sourceButton = event.srcElement;
    var id = sourceButton.getAttribute("gameid");
    try {
        const response = await fetch(`/api/games/GermanWhist/${id}`, {
            method: "POST",
            headers: {
                "Content-Type": "application/json",
                "Authorization": `Bearer ${JWTToken}`
            }
        });

        if (!response.ok) {
            throw new Error("Network response was not ok");
        }
        const gameInfo = await response.json();
        console.log("Join Game successful");

        gameId = gameInfo.id;
        playerId = gameInfo.userPlayerId;
        drawGameView();

        return;
    } catch (error) {
        console.error("There was a problem with the join game operation:", error);
    }
}

async function LoadOpenGames() {
    try {
        const response = await fetch(`/api/games/GermanWhist`, {
            method: "GET",
            headers: {
                "Content-Type": "application/json",
                "Authorization": `Bearer ${JWTToken}`
            }
        });

        if (!response.ok) {
            throw new Error("Network response was not ok");
        }
        const openGamesResponse = await response.json();
        console.log("Fetching open games successful");
        var openGamesList = [];
        openGamesResponse.forEach(openGame => {
            var li = document.createElement("li");

            var span = document.createElement("span");
            span.innerHTML = `Join Game of Player ${openGame.opponentPlayerId}`;
            li.appendChild(span);

            var button = document.createElement("button");
            button.setAttribute("gameId", openGame.id);
            button.setAttribute("class", "btn btn-primary float-end");
            button.addEventListener('click', JoinGame);
            button.innerHTML = "Join"
            li.appendChild(button);
            openGamesList.push(li);
        });
        document.getElementById("openGamesList").replaceChildren(...openGamesList);
        return;
    } catch (error) {
        console.error("There was a problem with the loading of open games:", error);
    }
}

async function LoadOngoingGamesOfUser() {
    try {
        const response = await fetch(`/api/Users/games/GermanWhist`, {
            method: "GET",
            headers: {
                "Content-Type": "application/json",
                "Authorization": `Bearer ${JWTToken}`,
            }
        });

        if (!response.ok) {
            throw new Error("Network response was not ok");
        }
        const ongoingGamesResponse = await response.json();
        console.log("Fetching open games successful");
        var ongoingGamesList = [];
        ongoingGamesResponse.forEach(ongoingGame => {
            var li = document.createElement("li");
            li.setAttribute("class", "m-1");

            var outerDiv = document.createElement("div");
            outerDiv.setAttribute("class", "d-flex justify-content-between");

            var span = document.createElement("span");
            span.innerHTML = `Resume Game ${ongoingGame.id} against Player ${ongoingGame.opponentPlayerId}`;

            var resumeButton = document.createElement("button");
            resumeButton.setAttribute("gameId", ongoingGame.id);
            resumeButton.setAttribute("class", "btn btn-primary");
            resumeButton.addEventListener('click', resumeGame);
            resumeButton.innerHTML = "Resume"

            var deleteButton = document.createElement("button");
            deleteButton.setAttribute("gameId", ongoingGame.id);
            deleteButton.setAttribute("class", "btn btn-danger");
            deleteButton.addEventListener('click', deleteGameEvent);
            deleteButton.innerHTML = "Delete"

            var buttonDiv = document.createElement("div");
            buttonDiv.className = "d-flex justify-content-end";
            buttonDiv.appendChild(resumeButton);
            buttonDiv.appendChild(deleteButton);

            li.appendChild(outerDiv);
            outerDiv.appendChild(span);
            outerDiv.appendChild(buttonDiv);

            ongoingGamesList.push(li);
        });
        document.getElementById("ongoingGamesList").replaceChildren(...ongoingGamesList);
        return;
    } catch (error) {
        console.error("There was a problem with the Loading of the ongoing games:", error);
    }
}

async function deleteGameEvent(event) {
    var sourceButton = event.srcElement;
    var id = sourceButton.getAttribute("gameid");
    var result = confirm(`Are you sure that you want to delete game ${id}?`);
    if (result) {
        await deleteGame(id);
    }
}
async function deleteGame(id) {

    try {
        const response = await fetch(`/api/games/GermanWhist/${id}`, {
            method: "DELETE",
            headers: {
                "Content-Type": "application/json",
                "Authorization": `Bearer ${JWTToken}`,
            },
        });

        if (!response.ok) {
            throw new Error("Network response was not ok");
        }
        drawMenuView();
    } catch (error) {
        console.error("There was a problem with the delete operation:", error);
    }
}

async function resumeGame(event) {
    var sourceButton = event.srcElement;
    var id = sourceButton.getAttribute("gameid");

    try {
        const response = await fetch(`/api/games/GermanWhist/${id}`, {
            method: "GET",
            headers: {
                "Content-Type": "application/json",
                "Authorization": `Bearer ${JWTToken}`,
            },
        });

        if (!response.ok) {
            throw new Error("Network response was not ok");
        }
        const gameResponse = await response.json();
        gameId = gameResponse.id;
        playerId = gameResponse.userPlayerId;
        drawGameView();
    } catch (error) {
        console.error("There was a problem with the resume game operation:", error);
    }
}

function drawLoginView() {
    document.getElementById("loginBox").style.display = "contents";
    document.getElementById("menuBox").style.display = "none";
    document.getElementById("gameBox").style.display = "none";
}

async function drawMenuView() {
    document.getElementById("loginBox").style.display = "none";
    document.getElementById("menuBox").style.display = "contents";
    document.getElementById("gameBox").style.display = "none";

    document.getElementById("startNewGameButton").addEventListener("click", startNewGameAgainstHumanPlayer);
    document.getElementById("startBotGameButton").addEventListener("click", startNewGameAgainstBotPlayer);


    await LoadOpenGames();
    await LoadOngoingGamesOfUser();
}

async function drawGameView() {
    document.getElementById("loginBox").style.display = "none";
    document.getElementById("menuBox").style.display = "none";
    document.getElementById("gameBox").style.display = "contents";

    document.getElementById("menuButton").addEventListener("click", drawMenuView);
    document.getElementById("playerId").innerHTML = playerId;
    document.getElementById("gameId").innerHTML = gameId;


    gameState = await gameController.getPlayerView();
    cardsList = await gameController.getCardsList();
    addFileNamesToCards(cardsList);
    if (!gameState.isPlayerCurrentPlayer) {
        gameController.startPollingGameState();
    }
    drawer.drawGameState()
}

var JWTToken = "";

var OpenGames;

var gameId;
var playerId;


var gameState;
var cardsList;

var gameController = new GameController();
var drawer = new Drawer();
gameController.setDrawer(drawer);
drawer.setGameController(gameController);

document.addEventListener("DOMContentLoaded", async (event) => {
    drawLoginView()
    document.getElementById("loginButton").addEventListener("click", loginPlayer)
})

