using MediatR;

namespace Cloud.$ext_safeprojectname$.Events.Integration
{
    public class QuantityChangedIntegrationEvent : IntegrationEvent, IRequest<bool>
    {
        public new int Id { get; set; }
        public int Quantity { get; set; }
        public override string GetKeyForEventBus()
        {
            return Id.ToString();
        }
    }
}
