using Lucrare02.Domain.Models;
using static Lucrare02.Domain.Models.CartPublishedEvent;
using static Lucrare02.Domain.CartsOperation;
using System;
using static Lucrare02.Domain.Models.Carts;

namespace Lucrare02.Domain
{
    public class PublishCartWorkflow
    {
        public async Task<ICartPublishedEvent> ExecuteAsync(PublishCartsCommand command, Func<ProductID, TryAsync<bool>> checkProductExist)
        {
            UnvalidatedCart unvalidatedCart = new UnvalidatedCart(command.InputUnvalidatedCart);
            ICart cart = ValidateCart(checkProductExists, unvalidatedGrades);
            cart = CalculatePrice(cart);
            cart = PaidCart(cart);

            return grades.Match(
                    whenUnvalidatedCart: unvalidatedCart => new CartPublishFaildEvent("Unexpected unvalidated state") as ICartPublishedEvent,
                    whenInvalidatedCart: invalidCart => new CartPublishFaildEvent(invalidCart.Reason),
                    whenValidatedCart: validatedCart => new CartPublishFaildEvent("Unexpected validated state"),
                    whenCalculatedCart: calculatedCart => new CartPublishFaildEvent("Unexpected calculated state"),
                    whenPublishedCart: publishedCart => new CartPublishSucceededEvent(publishedCart.Csv, publishedCart.PublishedDate)
                );
        }
    }
}