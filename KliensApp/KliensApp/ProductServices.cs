using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Hotcakes.CommerceDTO.v1;
using Hotcakes.CommerceDTO.v1.Catalog;
using Hotcakes.CommerceDTO.v1.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
namespace KliensApp
{
    public class ProductServices
    {
        public class ProductService
        {
            private readonly Api _api;

            public ProductService(Api api)
            {
                _api = api ?? throw new ArgumentNullException(nameof(api));
            }

            public async Task<List<ProductDTO>> GetAllProductsAsync()
            {
                var result = await Task.Run(() => _api.ProductsFindAll());
                ThrowIfErrors(result.Errors, "termékek lekérése");
                return result.Content;
            }

            public async Task<List<ProductTypeDTO>> GetAllProductTypesAsync()
            {
                var result = await Task.Run(() => _api.ProductTypesFindAll());
                ThrowIfErrors(result.Errors, "terméktípusok lekérése");
                return result.Content;
            }

            public async Task<ProductDTO> GetProductByBvinAsync(string bvin)
            {
                var result = await Task.Run(() => _api.ProductsFind(bvin));
                ThrowIfErrors(result.Errors, $"termék lekérése (ID: {bvin})");
                return result.Content;
            }

            public async Task UpdateProductAsync(ProductDTO product)
            {
                var result = await Task.Run(() => _api.ProductsUpdate(product));
                ThrowIfErrors(result.Errors, $"termék frissítése (ID: {product?.Bvin})");
            }

            private void ThrowIfErrors(List<ApiError> errors, string context)
            {
                if (errors != null && errors.Any())
                {
                    throw new Exception($"Hiba a(z) {context} során: {string.Join(", ", errors.Select(e => e.Description))}");
                }
            }
        }
    }
}
