using MassTransit;
using SagaStateMachine.Service.StateInstances;
using Shared.OrderEvents;
using Shared.PaymentEvents;
using Shared.StockEvents;

namespace SagaStateMachine.Service.StateMachines
{
    public class OrderStateMachine : MassTransitStateMachine<OrderStateInstance>
    {
        public Event<OrderStartedEvent> OrderStartedEvent { get; set; }
        public Event<StockReservedEvent> StockReservedEvent { get; set; }
        public Event<StockNotReservedEvent> StockNotReservedEvent { get; set; }
        public Event<PaymentCompletedEvent> PaymentCompletedEvent { get; set; }
        public Event<PaymentFailedEvent> PaymentFailedEvent { get; set; }

        public OrderStateMachine()
        {
            InstanceState(instance => instance.CurrentState);

            Event(() => OrderStartedEvent,
               orderStateInstance => orderStateInstance.CorrelateBy<int>(database => database.OrderId, @event => @event.Message.OrderId)
                                                       .SelectId(e => Guid.NewGuid()));
            Event(() => StockReservedEvent,
                orderStateInstance => orderStateInstance.CorrelateById(@event => @event.Message.CorrelationId));

            Event(() => StockNotReservedEvent,
                orderStateInstance => orderStateInstance.CorrelateById(@event => @event.Message.CorrelationId));

            Event(() => PaymentCompletedEvent,
                orderStateInstance => orderStateInstance.CorrelateById(@event => @event.Message.CorrelationId));

            Event(() => PaymentFailedEvent,
                orderStateInstance => orderStateInstance.CorrelateById(@event => @event.Message.CorrelationId));
        }
    }
}