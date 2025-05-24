using Grpc.Net.ClientFactory;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Ora.GameManaging.Mafia.Infrastructure.Services;
using Ora.GameManaging.Mafia.Model;
using Ora.GameManaging.Mafia.Model.Mapping;
using Ora.GameManaging.Mafia.Protos;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Ora.GameManaging.Mafia.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class GameRoomController(GrpcClientFactory clientFactory, IGeneralAttributeService generalAttributeService) : ControllerBase
    {
        private readonly GameRoomGrpc.GameRoomGrpcClient _gameRoomClient = clientFactory.CreateClient<GameRoomGrpc.GameRoomGrpcClient>("GameManaging");

        [HttpGet]
        public async Task<ActionResult<List<GameRoomModel>>> GetAll(CancellationToken cancellationToken)
        {
            var response = await _gameRoomClient.GetAllRoomsAsync(new GetAllRoomsRequest(), cancellationToken: cancellationToken);
            var models = response.Rooms.Select(r => r.ToModel()).ToList();
            return Ok(models);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<GameRoomModel>> GetById(string id, CancellationToken cancellationToken)
        {
            var response = await _gameRoomClient.GetRoomByIdAsync(new GetRoomByIdRequest { RoomId = id }, cancellationToken: cancellationToken);
            if (response.Room == null)
                return NotFound();
            return Ok(response.Room.ToModel());
        }

        [HttpPost("by-app")]
        public async Task<ActionResult<List<GameRoomModel>>> GetAllByAppId(RoomRequestModel requestModel, CancellationToken cancellationToken)
        {
            var response = await _gameRoomClient.GetRoomsByAppIdAsync(new GetRoomByAppIdRequest { AppId = requestModel.AppId, Pagination = new Pagination { Count = requestModel.Count, Size = requestModel.Size, Skip = requestModel.Skip } }, cancellationToken: cancellationToken);
            var rooms = response.Rooms.Select(r => r.ToModel()).ToList();

            var attributes = await generalAttributeService.GetAllByApplicationIdAsync(requestModel.AppId, EntityKeys.GameRoom);
            var attributesByRoom = attributes
                .GroupBy(a => a.EntityId)
                .ToDictionary(g => g.Key, g => g.ToList());

            rooms.ForEach(room =>
            {
                if (attributesByRoom.TryGetValue(room.RoomId, out var roomAttributes))
                {
                    AttributeReflectionHelper.ApplyAttributesToModel(room, roomAttributes);
                }
            });

            return Ok(new RoomResultModel { Data = rooms, Count = 1, Successful = true, Error = null }); ;
        }

        [HttpPost]
        public async Task<ActionResult> Create([FromBody] GameRoomModel model, CancellationToken cancellationToken)
        {
            var protoRoom = model.ToProto();
            var response = await _gameRoomClient.CreateRoomAsync(new CreateRoomRequest { Room = protoRoom }, cancellationToken: cancellationToken);
            if (!response.Result)
                return BadRequest();
            return CreatedAtAction(nameof(GetById), new { id = model.RoomId }, model);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> Update(string id, [FromBody] GameRoomModel model, CancellationToken cancellationToken)
        {
            if (id != model.RoomId)
                return BadRequest();

            var protoRoom = model.ToProto();
            var response = await _gameRoomClient.UpdateRoomAsync(new UpdateRoomRequest { Room = protoRoom }, cancellationToken: cancellationToken);
            if (!response.Result)
                return NotFound();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(string id, CancellationToken cancellationToken)
        {
            var response = await _gameRoomClient.DeleteRoomAsync(new DeleteRoomRequest { RoomId = id }, cancellationToken: cancellationToken);
            if (!response.Result)
                return NotFound();
            return NoContent();
        }
    }
}