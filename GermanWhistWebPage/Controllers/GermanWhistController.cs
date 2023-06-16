using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using GermanWhistWebPage.Models;
using GermanWhistWebPage.Services;

namespace GermanWhistWebPage.Controllers
{
    [Route("api/games/[controller]")]
    [ApiController]
    public class GermanWhistController : ControllerBase
    {
        private readonly GameContext _context;
        private readonly PlayerService  _playerService;
        private readonly GameService _gameService;
        private readonly CardService _cardService;

        public GermanWhistController(GameContext context, PlayerService playerService, 
            GameService gameService, CardService cardService)
        {
            _context = context;
            _playerService = playerService;
            _gameService = gameService;
            _cardService = cardService;
        }

        // GET: api/Games
        [HttpGet]
        public async Task<ActionResult<IEnumerable<GameInfoDTO>>> GetGames()
        {
          if (_context.Games == null)
          {
              return NotFound();
          }
        var games = await _context.Games.ToListAsync();
        if (games == null)
        {
            return NotFound();
        }
            return games.Select(game => new GameInfoDTO(game)).ToList();
        }

        // GET: api/Games/5
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

            return new GameInfoDTO(game);
        }

        [HttpGet("{id}/player-view")]
        public async Task<ActionResult<PlayerViewOfGameStateDTO>> GetPlayerView(int id, int PlayerId)
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

            Player player = _context.Players.Find(PlayerId);
            if (player == null)
            {
                return NotFound();
            }

            return new PlayerViewOfGameStateDTO(game, player.Id, _gameService.getValidMoves(game, PlayerId));
        }

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

        [HttpPost("{id}/move")]
        public async Task<ActionResult<PlayerViewOfGameStateDTO>> MakeAMove(int id, MoveDTO move)
        {
            if (_context.Games == null || _context.Players == null)
            {
                return NotFound();
            }
            var game = await _context.Games.FindAsync(id);

            if (game == null)
            {
                return NotFound();
            }

            Player player = await _context.Players.FindAsync(move.PlayerId);
            if (player == null)
            {
                return NotFound();
            }

            try
            {
                _gameService.makeMove(game, player.Id, move.CardId);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            
            await _context.SaveChangesAsync();
            return new PlayerViewOfGameStateDTO(game, player.Id, _gameService.getValidMoves(game, player.Id));
        }

        // POST: api/Games
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<GameInfoDTO>> PostGame([FromBody] PlayerInfoDTO playerInfo)
        {
            if (_context.Games == null)
            {
                return Problem("Entity set 'GameContext.Games'  is null.");
            }
            Player player1 = await _context.Players.FindAsync(playerInfo.Player1Id);
            Player player2 = await _context.Players.FindAsync(playerInfo.Player2Id);
            if (player1 == null || player2 == null)
            {
                return NotFound();
            }

            Game game = _gameService.createGame(player1.Id, player2.Id);

            _context.Games.Add(game);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetGame", new { id = game.Id }, new GameInfoDTO(game));
        }

        // DELETE: api/Games/5
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

            _context.Games.Remove(game);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // GET: api/GermanWhist/Cards
        [HttpGet("Cards")]
        public async Task<ActionResult<IEnumerable<Card>>> GetCards()
        {
            
            return _cardService.Cards;
        }

        private bool GameExists(int id)
        {
            return (_context.Games?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
