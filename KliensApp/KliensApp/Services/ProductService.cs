using Hotcakes.CommerceDTO.v1.Catalog;
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

namespace KliensApp.Services
{
    public class ProductService
    {
        private readonly IApiClient _api;


        public ProductService(IApiClient api)
        {
            _api = api;
        }

        public async Task<List<ProductDTO>> GetAllProductsAsync()
        {
            var response = await Task.Run(() => _api.ProductsFindAll());
            if (response.Errors.Any())
            {
                throw new Exception(string.Join(", ", response.Errors.Select(e => e.Description)));
            }
            return response.Content;
        }

        public async Task<List<ProductTypeDTO>> GetAllProductTypesAsync()
        {
            var response = await Task.Run(() => _api.ProductTypesFindAll());
            if (response.Errors.Any())
            {
                throw new Exception(string.Join(", ", response.Errors.Select(e => e.Description)));
            }
            return response.Content;
        }

        public async Task<ProductDTO> GetProductByIdAsync(string bvin)
        {
            var response = await Task.Run(() => _api.ProductsFind(bvin));
            if (response.Errors.Any())
            {
                throw new Exception(string.Join(", ", response.Errors.Select(e => e.Description)));
            }
            return response.Content;
        }

        public async Task<ProductDTO> UpdateProductAsync(ProductDTO product)
        {
            var response = await Task.Run(() => _api.ProductsUpdate(product));
            if (response.Errors.Any())
            {
                throw new Exception(string.Join(", ", response.Errors.Select(e => e.Description)));
            }
            return response.Content;
        }
    }
}
