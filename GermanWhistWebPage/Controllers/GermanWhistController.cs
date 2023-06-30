using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using GermanWhistWebPage.Models;
using GermanWhistWebPage.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace GermanWhistWebPage.Controllers
{
    [Route("api/games/[controller]")]
    [ApiController]
    public class GermanWhistController : ControllerBase
    {
        private readonly GameContext _context;
        private readonly BotService _botService;
        private readonly GameService _gameService;
        private readonly CardService _cardService;
        private readonly Microsoft.AspNetCore.Identity.UserManager<IdentityUser> _userManager;

        public GermanWhistController(GameContext context, BotService botService,
            GameService gameService, CardService cardService, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _botService = botService;
            _gameService = gameService;
            _cardService = cardService;
            _userManager = userManager;
        }

        [Authorize]
        // GET: api/games/GermanWhist
        [HttpGet]
        public async Task<ActionResult<IEnumerable<GameInfoDTO>>> GetGames()
        {
            if (_context.Games == null)
            {
                return NotFound();
            }
            Player player = GetCurrentPlayer();
            if (player == null)
                return Problem("User could not be mapped to player");

            var gameDTOs = await _context.Games
                .Include(g => (g.Player1 as HumanPlayer).IdentityUser)
                .Where(g => g.Player2Id == null && g.Player1Id != player.Id)
                .Select(game => new GameInfoDTO(game, player.Id))
                .ToListAsync();


            return gameDTOs;
        }

        [Authorize]
        // GET: api/games/GermanWhist/5
        [HttpGet("{id}")]
        public async Task<ActionResult<GameInfoDTO>> GetGame(int id)
        {
            if (_context.Games == null)
            {
                return NotFound();
            }
            var game = await _context.Games.FindAsync(id);

            if (game == null)
            {
                return NotFound();
            }
            Player player = GetCurrentPlayer();
            if (player == null)
                return Problem("User could not be mapped to player");

            return new GameInfoDTO(game,player.Id);
        }

        // POST: api/games/GermanWhist/5
        [Authorize]
        [HttpPost("{id}")]
        public async Task<ActionResult<GameInfoDTO>> JoinGame(int id)
        {
            if (_context.Games == null)
                return NotFound();


            var game = await _context.Games.FindAsync(id);

            if (game == null)
                return NotFound();

            await _context.Entry(game)
            .Reference(g => g.Player1)
            .LoadAsync();

            if (game.Player1 is HumanPlayer humanPlayer)
            {
                await _context.Entry(humanPlayer)
                    .Reference(p => p.IdentityUser)
                    .LoadAsync();
            }


            if (game.HasGameStarted)
                return BadRequest("Game has already started and cannot be joined");

            Player? player = await GetCurrentPlayerAsync();
            if (player == null)
                return Problem("User Could not be connected to a player");

            game.Player2 = player;

            await _context.SaveChangesAsync();

            return new GameInfoDTO(game, player.Id);
        }

        // GET: api/games/GermanWhist/5/player-view
        [Authorize]
        [HttpGet("{id}/player-view")]
        public async Task<ActionResult<PlayerViewOfGameStateDTO>> GetPlayerView(int id)
        {
            if (_context.Games == null)
            {
                return NotFound();
            }
            var game = await _context.Games.FindAsync(id);

            if (game == null)
            {
                return NotFound();
            }

            Player? player = await GetCurrentPlayerAsync();
            if (player == null)
                return Problem("User Could not be connected to a player");

            if (game.Player1 != player && game.Player2 != player)
                return BadRequest("You are not a player of this game");

            if (game.IsBotGame && game.CurrentPlayerId != player.Id)
            {
                _botService.MakeBotMove(game);
                await _context.SaveChangesAsync();
            }

            return new PlayerViewOfGameStateDTO(game, player.Id, _gameService.getValidMoves(game, player.Id));
        }

        // GET: api/games/GermanWhist/5/game-state
        // [Authorize]
        [HttpGet("{id}/game-state")]
        public async Task<ActionResult<Game>> GetGameState(int id)
        {
            if (_context.Games == null)
            {
                return NotFound();
            }
            var game = await _context.Games.FindAsync(id);

            if (game == null)
            {
                return NotFound();
            }
            return game;
        }


        // POST: api/games/GermanWhist/5/move
        [Authorize]
        [HttpPost("{id}/move")]
        public async Task<ActionResult<PlayerViewOfGameStateDTO>> MakeAMove(int id, MoveDTO move)
        {
            if (_context.Games == null || _context.HumanPlayers == null)
            {
                return NotFound();
            }
            var game = await _context.Games.FindAsync(id);

            if (game == null)
            {
                return NotFound();
            }

            if (!game.HasGameStarted)
            {
                return BadRequest("Game has not yet started");
            }

            if (game.HasGameEnded)
            {
                return BadRequest("Game has already ended");
            }

            Player? player = await GetCurrentPlayerAsync();
            if (player == null)
                return Problem("User Could not be connected to a player");

            try
            {
                _gameService.makeMove(game, player.Id, move.CardId);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }

            await _context.SaveChangesAsync();

            PlayerViewOfGameStateDTO playerView = new PlayerViewOfGameStateDTO(game, player.Id, _gameService.getValidMoves(game, player.Id));
            return playerView;
        }


        // POST: api/games/GermanWhist
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<GameInfoDTO>> StartNewGame([FromBody] NewGameInfoDTO newGameInfoDTO)
        {
            if (_context.Games == null)
            {
                return Problem("Entity set 'GameContext.Games'  is null.");
            }

            Player? player1 = await GetCurrentPlayerAsync();
            if (player1 == null)
                return Problem("User Could not be connected to a player");


            Player? player2 = null;
            if (newGameInfoDTO.AgainstBotOpponent)
            {
                int botDifficulty = newGameInfoDTO.BotDifficulty.GetValueOrDefault();

                player2 = _botService.getBotPlayer(botDifficulty);
                if (player2 == null)
                    return BadRequest("Bad Bot Settings in request");
            }
            else
            {
                if (newGameInfoDTO.OpponentPlayerId != null)
                {
                    player2 = await _context.HumanPlayers.FindAsync(newGameInfoDTO.OpponentPlayerId);
                    if (player2 == null)
                        return BadRequest("Opponent Player not found");
                }

            }

            int? player2Id = player2 == null ? null : player2.Id;

            Game game = _gameService.createGame(player1.Id, player2Id, newGameInfoDTO.AgainstBotOpponent);

            _context.Games.Add(game);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetGame", new { id = game.Id }, new GameInfoDTO(game, player1.Id));
        }

        // DELETE: api/games/GermanWhist/5
        [Authorize]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteGame(int id)
        {
            if (_context.Games == null)
            {
                return NotFound();
            }
            var game = await _context.Games.FindAsync(id);
            if (game == null)
            {
                return NotFound();
            }

            Player? player = await GetCurrentPlayerAsync();
            if (player == null)
                return Problem("User Could not be connected to a player");

            if (game.Player1 != player && game.Player2 != player)
            {
                return BadRequest("Player not part of this game");
            }
            _context.Games.Remove(game);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // GET: api/games/GermanWhist/Cards
        [HttpGet("Cards")]
        public async Task<ActionResult<IEnumerable<Card>>> GetCards()
        {

            return _cardService.Cards;
        }

        private bool GameExists(int id)
        {
            return (_context.Games?.Any(e => e.Id == id)).GetValueOrDefault();
        }

        private async Task<Player?> GetCurrentPlayerAsync()
        {
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            return await _context.HumanPlayers.
                Include(p => p.IdentityUser)
                .FirstOrDefaultAsync(p => p.IdentityUserId == userId);
        }
        private Player? GetCurrentPlayer()
        {
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            return _context.HumanPlayers
                .Include(p => p.IdentityUser)
                .FirstOrDefault(p => p.IdentityUserId == userId);
        }
    }
}
