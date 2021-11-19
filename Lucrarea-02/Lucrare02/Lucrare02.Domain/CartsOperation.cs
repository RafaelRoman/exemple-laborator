using Lucrare02.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Lucrare02.Domain.Models.Cart;

namespace Lucrare02.Domain
{
    public static class CartsOperation
    {
        public static Carts ValidateCart(Func<ProductID, TryAsync<bool>> checkProductExists, UnvalidatedProduct productList) => 
            productList.ProductList
                .Select(ValidatedProduct(checkProductExists))
                .Aggregate(CreateEmptyValidatedProductList().ToAsync(), ReduceValidCart)
                .Match(
                    Right: ValidatedProduct => new ValidatedCart(validatedProduct),
                    LeftAsync: errorMessage => Task.FromResult((ICart)new InvalidatedCart(productList.ProductList, errorMessage))
                );
        private static Either<string, List<ValidatedProduct>> CreateEmptyValidatedProductList() =>
            Right(new List<ValidatedProduct>());
        
        private static EitherAsync<string, List<ValidatedProduct>> ReduceValidCart(EitherAsync<string, List<ValidatedProduct>> acc, EitherAsync<string, ValidatedProduct> next) =>
            from list in acc
            from nextProduct in next
            select list.AppendValidProduct(nextProduct);

        private static List<ValidatedProduct> AppendValidProduct(this List<ValidatedProduct> list, ValidatedProduct validProduct)
        {
            list.Add(validProduct);
            return list;
        }
        private static EitherAsync<string, ValidatedProduct> ValidatedProduct(Func<ProductID, TryAsync<bool>> checkProductExists, UnvalidatedProducts unvalidatedProduct) =>
            from productId in ProductID.TryParseProductCode(unvalidatedProduct.ProductID)
                                   .ToEitherAsync(() => $"Invalid product code ({unvalidatedProduct.ProductID})")
            from productQuantity in ProductQuantity.TryParseQuantity(unvalidatedProduct.ProductQuantity)
                                   .ToEitherAsync(() => $"Invalid product quantity ({unvalidatedProduct.ProductQuantity}, {unvalidatedProduct.ProductQuantity})")
            from address in Address.TryParseAdresa(unvalidatedProduct.Address)
                                   .ToEitherAsync(() => $"Invalid student registration number ({unvalidatedProduct.Address})")
            select new ValidatedProduct(productCode, productQuantity, Address);

        public static ICart CalculatePrice(ICart products) => products.Match(
            whenUnvalidatedCart: unvalidatedProduct => unvalidatedProduct,
            whenInvalidatedCart: invalidatedProduct => invalidatedProduct,
            whenCalculatedCart: calculatedCart => calculatedCart,
            whenPaidCart: paidCart => paidCart,
            whenValidatedCart: CalculateFinalPrices
        );

        private static ICart CalculateFinalPrices(ValidatedCarucior validProduct) =>
                  new CalculatePrice(validProduct.ProductList
                                                 .Select(CalculateProductPrices)
                                                 .ToList()
                                                 .AsReadOnly());

        private static CalculatedCart CalculateProductPrices(ValidatedProduct validProduct) =>
                                                new CalculatedCart(validProduct.ProductID,
                                                                    validProduct.ProductQuantity,
                                                                    validProduct.Address,
                                                                    Convert.ToInt32(validProduct.ProductQuantity) * 10);

        public static ICart PaidCart(ICart products) => products.Match(
            whenUnvalidatedCart: unvalidatedCart => unvalidatedCart,
            whenInvalidatedCart: invalidatedCart => invalidatedCart,
            whenValidatedCart: validatedCart => validatedCart,
            whenPaidCart: paidCart => paidCart,
            whenCalculatedCart: GenerateExport
        );

        private static ICart GenerateExport(CalculatedCart calculatedPrice) => 
            new PaidCart(calculatedPrice.productList, calculatedPrice.productList.Aggregate(new StringBuilder(), CreateCsvLine).ToString(), DateTime.Now)
    };
}
