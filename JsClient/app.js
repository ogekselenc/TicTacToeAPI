// Global variables
let players = { 1: null, 2: null };
let currentGame = null;
const API_BASE_URL = 'http://localhost:5000/api';

// DOM Elements
const gameContainer = document.getElementById('game-container');
const statusElement = document.getElementById('status');
const boardElement = document.getElementById('board');

// Player Management
async function createPlayer(playerNumber) {
    const nameInput = document.getElementById(`player${playerNumber}-name`);
    const name = nameInput.value.trim();
    
    if (!name) {
        alert('Please enter a name');
        return;
    }

    try {
        const response = await fetch(`${API_BASE_URL}/players`, {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify({ name })
        });
        
        const player = await response.json();
        players[playerNumber] = player;
        document.getElementById(`player${playerNumber}-id`).textContent = `(ID: ${player.id})`;
        
        if (players[1] && players[2]) {
            document.getElementById('game-setup').style.display = 'block';
        }
    } catch (error) {
        console.error('Error creating player:', error);
    }
}

// Game Management
async function createGame() {
    const boardSize = parseInt(document.getElementById('board-size').value);
    const winningLine = parseInt(document.getElementById('winning-line').value);
    
    if (!players[1]?.id) {
        alert('Player X not registered');
        return;
    }

    try {
        const response = await fetch(`${API_BASE_URL}/games`, {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify({
                playerXId: players[1].id,
                boardSize,
                winningLine
            })
        });
        
        currentGame = await response.json();
        setupGameUI(currentGame);
        loadActiveGames();
    } catch (error) {
        console.error('Error creating game:', error);
    }
}

async function joinGame(gameId) {
    if (!players[2]?.id) {
        alert('Player O not registered');
        return;
    }

    try {
        const response = await fetch(`${API_BASE_URL}/games/${gameId}/join`, {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify({ playerOId: players[2].id })
        });
        
        currentGame = await response.json();
        setupGameUI(currentGame);
    } catch (error) {
        console.error('Error joining game:', error);
    }
}

async function resignGame() {
    if (!currentGame) return;
    
    const currentPlayerId = prompt("Enter your player ID to resign:");
    if (!currentPlayerId) return;

    try {
        const response = await fetch(`${API_BASE_URL}/games/${currentGame.id}/resign?playerId=${currentPlayerId}`, {
            method: 'POST'
        });
        
        currentGame = await response.json();
        updateGameStatus(currentGame);
    } catch (error) {
        console.error('Error resigning game:', error);
    }
}

// Game UI
function setupGameUI(game) {
    gameContainer.style.display = 'block';
    document.getElementById('game-id').textContent = game.id;
    
    // Create board
    boardElement.style.gridTemplateColumns = `repeat(${game.boardSize}, 1fr)`;
    boardElement.innerHTML = '';
    
    for (let y = 0; y < game.boardSize; y++) {
        for (let x = 0; x < game.boardSize; x++) {
            const cell = document.createElement('div');
            cell.className = 'cell';
            cell.dataset.x = x;
            cell.dataset.y = y;
            cell.addEventListener('click', () => makeMove(x, y));
            boardElement.appendChild(cell);
        }
    }
    
    updateGameStatus(game);
    renderMoveHistory(game.moves);
}

function updateGameStatus(game) {
    let statusText = '';
    
    if (game.status === 'InProgress') {
        const lastMove = game.moves[game.moves.length - 1];
        const nextPlayer = !lastMove || lastMove.playerId === game.playerO?.id 
            ? `X (${game.playerX.name})` 
            : `O (${game.playerO?.name || 'waiting'})`;
        statusText = `Next: Player ${nextPlayer}`;
    } else {
        statusText = `Game Over: ${game.outcomeReason}`;
    }
    
    statusElement.textContent = statusText;
}

function renderMoveHistory(moves) {
    const historyElement = document.getElementById('move-history');
    historyElement.innerHTML = '<h3>Move History</h3>';
    
    if (!moves || moves.length === 0) {
        historyElement.innerHTML += '<p>No moves yet</p>';
        return;
    }
    
    const list = document.createElement('ul');
    moves.forEach(move => {
        const item = document.createElement('li');
        item.textContent = `Player ${move.playerName}: (${move.positionX}, ${move.positionY})`;
        list.appendChild(item);
    });
    historyElement.appendChild(list);
}

// Game Logic
async function makeMove(x, y) {
    if (!currentGame || currentGame.status !== 'InProgress') return;
    
    const currentPlayerId = prompt("Enter your player ID to move:");
    if (!currentPlayerId) return;

    try {
        const response = await fetch(`${API_BASE_URL}/games/${currentGame.id}/move`, {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify({
                playerId: parseInt(currentPlayerId),
                positionX: x,
                positionY: y
            })
        });
        
        currentGame = await response.json();
        updateBoard(currentGame);
        updateGameStatus(currentGame);
        renderMoveHistory(currentGame.moves);
    } catch (error) {
        console.error('Error making move:', error);
        alert(error.message);
    }
}

function updateBoard(game) {
    // Clear board
    document.querySelectorAll('.cell').forEach(cell => {
        cell.textContent = '';
        cell.className = 'cell';
    });
    
    // Update with current moves
    game.moves.forEach(move => {
        const cell = document.querySelector(`.cell[data-x="${move.positionX}"][data-y="${move.positionY}"]`);
        if (cell) {
            cell.textContent = move.playerId === game.playerX.id ? 'X' : 'O';
            cell.classList.add(move.playerId === game.playerX.id ? 'x' : 'o');
        }
    });
}

// Active Games List
async function loadActiveGames() {
    try {
        const response = await fetch(`${API_BASE_URL}/games/active`);
        const games = await response.json();
        const container = document.getElementById('active-games');
        
        container.innerHTML = '<h3>Active Games</h3>';
        if (games.length === 0) {
            container.innerHTML += '<p>No active games</p>';
            return;
        }
        
        const list = document.createElement('ul');
        games.forEach(game => {
            const item = document.createElement('li');
            item.innerHTML = `Game ${game.id} (${game.boardSize}x${game.boardSize}) - 
                X: ${game.playerX.name} ${!game.playerO ? `<button onclick="joinGame(${game.id})">Join</button>` : ''}`;
            list.appendChild(item);
        });
        container.appendChild(list);
    } catch (error) {
        console.error('Error loading active games:', error);
    }
}

// Initialize
document.addEventListener('DOMContentLoaded', () => {
    loadActiveGames();
});
