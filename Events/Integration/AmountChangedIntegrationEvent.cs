using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Cloud.$ext_safeprojectname$.Events.Integration
{
    public class AmountChangedIntegrationEvent : IntegrationEvent, IRequest<bool>
    {
        public new int Id { get; set; }
        public double Amount { get; set; }

        public override string GetKeyForEventBus()
        {
            return Id.ToString();
        }
    }
}
