const connection = new signalR.HubConnectionBuilder()
    .withUrl("/gameHub")
    .configureLogging(signalR.LogLevel.Information)
    .build();

connection.start().then(() => {
    console.log("Connected to SignalR hub");
}).catch(err => console.error(err.toString()));

function joinGame(gameId) {
    connection.invoke("JoinGame", gameId).catch(err => console.error(err.toString()));
}

function sendMove(gameId, move) {
    connection.invoke("SendMove", gameId, move).catch(err => console.error(err.toString()));
}

connection.on("ReceiveMove", (move) => {
    console.log("New move received:", move);
});

connection.on("GameOver", (outcome) => {
    console.log("Game Over:", outcome);
});
