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

        [HttpGet("{id}/game-state")]
        public async Task<ActionResult<PlayerViewOfGameStateDTO>> GetGameState(int id)
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

            int playerId = _playerService.getUserPlayerId();

            return new PlayerViewOfGameStateDTO(game, playerId);
        }

        [HttpPatch("{id}/game-state")]
        public async Task<ActionResult<PlayerViewOfGameStateDTO>> MakeAMove(int id, int cardId)
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

            int playerId = _playerService.getUserPlayerId();

            // Card card = _cardService.getCardFromId(cardId);

            if (! _gameService.isValidMove(game, playerId, cardId))
            {
                return Forbid();
            }
            _gameService.makeMove(game, playerId, cardId);    
            await _context.SaveChangesAsync();
            return new PlayerViewOfGameStateDTO(game, playerId);
        }



        //// PUT: api/Games/5
        //// To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        //[HttpPut("{id}")]
        //public async Task<IActionResult> PutGame(int id, Game game)
        //{
        //    if (id != game.Id)
        //    {
        //        return BadRequest();
        //    }

        //    _context.Entry(game).State = EntityState.Modified;

        //    try
        //    {
        //        await _context.SaveChangesAsync();
        //    }
        //    catch (DbUpdateConcurrencyException)
        //    {
        //        if (!GameExists(id))
        //        {
        //            return NotFound();
        //        }
        //        else
        //        {
        //            throw;
        //        }
        //    }

        //    return NoContent();
        //}

        // POST: api/Games
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Game>> PostGame( Player opponent)
        {
            if (_context.Games == null)
            {
                return Problem("Entity set 'GameContext.Games'  is null.");
            }
            int userPlayerId = _playerService.getUserPlayerId();
            Game game = _gameService.createGame(userPlayerId, opponent.Id);

            //_context.Entry(userPlayer).State = EntityState.Unchanged;
            //_context.Entry(opponent).State = EntityState.Unchanged;

            _context.Games.Add(game);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetGame", new { id = game.Id }, game);
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

        private bool GameExists(int id)
        {
            return (_context.Games?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
