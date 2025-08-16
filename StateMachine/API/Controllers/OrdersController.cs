using MediatR;
using Microsoft.AspNetCore.Mvc;
using StateMachine.API.DTOs;
using StateMachine.Features.Orders.CreateOrder;
using StateMachine.Features.Orders.GetOrder;
using StateMachine.Features.Orders.GetOrderByNumber;
using StateMachine.Features.Orders.GetOrderHistory;
using StateMachine.Features.Orders.GetOrderStates;
using StateMachine.Features.Orders.ListOrders;
using StateMachine.Features.Orders.TransitionOrder;

namespace StateMachine.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class OrdersController(IMediator mediator) : ControllerBase
{

    /// <summary>Get all orders with pagination</summary>
    [HttpGet]
    public async Task<ActionResult<ListOrdersResult>> GetOrders([FromQuery] int page = 1, [FromQuery] int pageSize = 20)
    {
        return Ok( await mediator.Send(new ListOrdersQuery(page, pageSize)));
    }

    /// <summary>Get order by ID</summary>
    [HttpGet("{id:int}")]
    public async Task<ActionResult<GetOrderResult>> GetOrder(int id)
    {
        var result = await mediator.Send(new GetOrderQuery(id));
        return result != null ? Ok(result) : NotFound($"Order with ID {id} not found");
    }

    /// <summary>Get order by order number</summary>
    [HttpGet("by-number/{orderNumber}")]
    public async Task<ActionResult<GetOrderByNumberResult>> GetOrderByNumber(string orderNumber)
    {
        var result = await mediator.Send(new GetOrderByNumberQuery(orderNumber));
        return result != null ? Ok(result) : NotFound($"Order with number {orderNumber} not found");
    }

    /// <summary>Create a new order</summary>
    [HttpPost]
    public async Task<ActionResult<CreateOrderResult>> CreateOrder([FromBody] CreateOrderCommand command)
    {
        var result = await mediator.Send(command);
        return CreatedAtAction(nameof(GetOrder), new { id = result.Id }, result);
    }

    /// <summary>Transition order to a new state</summary>
    [HttpPost("{id:int}/transition")]
    public async Task<ActionResult<TransitionOrderResult>> TransitionOrder(int id, [FromBody] TransitionOrderRequest request)
    {
        var command = new TransitionOrderCommand(id, request.TargetState, request.Notes);
        var result = await mediator.Send(command);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    /// <summary>Get order state history</summary>
    [HttpGet("{id:int}/history")]
    public async Task<ActionResult<GetOrderHistoryResult>> GetOrderHistory(int id)
    {
        var result = await mediator.Send(new GetOrderHistoryQuery(id));
        return Ok(result);
    }

    /// <summary>Get available order states</summary>
    [HttpGet("states")]
    public async Task<ActionResult<GetOrderStatesResult>> GetOrderStates()
    {
        var result = await mediator.Send(new GetOrderStatesQuery());
        return Ok(result);
    }
}