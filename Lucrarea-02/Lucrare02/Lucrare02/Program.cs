﻿using Lucrare02.Domain.Models;
using System;
using System.Collections.Generic;
using static Lucrare02.Domain.Models.Cart;
using static Lucrare02.Domain.CartsOperation;

namespace Lucrarea02
{
    class Program
    {

        static void Main(string[] args)
        {

            string answer = ReadValue("Start shopping?[Y/N]");
            if (answer.Contains("Y"))
            {
                var ProductsList = ReadProducts().ToArray();
                var CartDetails = ReadDetails();
                PublishGradesCommand command = new(ProductsList);
                PublishGradeWorkflow workflow = new();
                var result = workflow.Execute(command, CartDetails);

                result.Match{
                    whenCartPublishFaildEvent: @event =>
                    {
                        Console.WriteLine($"Publish failed: {@event.Reason}");
                        return @event;
                    },
                    whenCartPublishSuccededEvent: @event =>
                    {
                        Console.WriteLine($"Publish succeded.");
                        Console.WriteLine(@event.Csv);
                        return @event
                    }
                    
                }

                ICart result = CheckCart(unvalidatedCart);
                result.Match(
                    whenUnvalidatedCart: unvalidatedCart => unvalidatedCart,
                    whenEmptyCart: invalidResult => invalidResult,
                    whenInvalidatedCart: invalidResult => invalidResult,
                    whenValidatedCart: validatedCart => PaidCart(validatedCart, CartDetails, DateTime.Now),
                    whenPaidCart: paidCart => paidCart
                );



                Console.WriteLine(result);

            }
            else Console.WriteLine("BYE!");

        }




        private static ICart CheckCart(UnvalidatedCart unvalidatedCart) =>
           ( (unvalidatedCart.ProductsList.Count == 0) ? new EmptyCart()
                : ((string.IsNullOrEmpty(unvalidatedCart.CartDetails.PaymentAddress.Value))? new InvalidatedCart(new List<UnvalidatedProducts>(), "Invalid Cart")
                      :( (unvalidatedCart.CartDetails.PaymentState.Value == 0) ? new ValidatedCart(new List<ValidatedProducts>(), unvalidatedCart.CartDetails)
                             :new PaidCart(new List<ValidatedProducts>(), unvalidatedCart.CartDetails, DateTime.Now))));

        private static ICart PaidCart(ValidatedCart validatedResult, CartDetails CartDetails, DateTime PublishedDate) =>
                new PaidCart(new List<ValidatedProducts>(), CartDetails, DateTime.Now);




        private static List<UnvalidatedProducts> ReadProducts()
        {
            List<UnvalidatedProducts> listOfProducts = new();
            object answer = null;
            do
            {
                answer = ReadValue("add product?[Y/N]: ");

                if (answer.Equals("Y"))
                {
                    var ProductID = ReadValue("ProductID: ");
                    if (string.IsNullOrEmpty(ProductID))
                    {
                        break;
                    }

                    var QuantityProduct = ReadValue("QuantityProduct: ");
                    if (string.IsNullOrEmpty(QuantityProduct))
                    {
                        break;
                    }
                    UnvalidatedProducts toAdd = new(ProductID, QuantityProduct);
                    listOfProducts.Add(toAdd);
                }

            } while (!answer.Equals("N"));
            
            return listOfProducts;
        }

        public static CartDetails ReadDetails()
        {
            PaymentState paymentState;
            PaymentAddress paymentAddress;
            CartDetails cartDetails;

            string answer = ReadValue("Finish the command?[Y/N]");

            if (answer.Contains("Y"))
            {

                var Address = ReadValue("Adresa: ");
                if (string.IsNullOrEmpty(Address))
                {
                    paymentAddress = new PaymentAddress("NONE");
                }
                else
                {
                    paymentAddress = new PaymentAddress(Address);
                }
                var payment = ReadValue("Pay now?[Y/N] ");
                if (payment.Contains("Y"))
                {
                    paymentState = new PaymentState(1);
                }
                else
                {
                    paymentState = new PaymentState(0);
                }
            }
            else
            {
                paymentAddress = new PaymentAddress("NONE");
                paymentState = new PaymentState(0);
            }
            cartDetails = new CartDetails(paymentAddress, paymentState);
            return cartDetails;
        }

        private static string? ReadValue(string prompt)
        {
            Console.Write(prompt);
            return Console.ReadLine();
        }

    }
}
