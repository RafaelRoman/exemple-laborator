using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Lucrare02.Domain.Models.Cart;

namespace Lucrare02.Domain.Models
{
    public record PublishCartsCommand
    {
        public PublishCartsCommand(IReadOnlyCollection<UnvalidatedCart> inputUnvalidatedCart)
        {
            InputUnvalidatedCart = inputUnvalidatedCart;
        }

        public IReadOnlyCollection<UnvalidatedCart> InputUnvalidatedCart { get; }
    }
}