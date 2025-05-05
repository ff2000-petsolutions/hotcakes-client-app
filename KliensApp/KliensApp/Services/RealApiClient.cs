using Hotcakes.CommerceDTO.v1.Catalog;
using Hotcakes.CommerceDTO.v1;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Hotcakes.CommerceDTO.v1;
using Hotcakes.CommerceDTO.v1.Client;
using Hotcakes.CommerceDTO.v1.Catalog;
using System.Collections.Generic;
namespace KliensApp.Services
{
    public class RealApiClient : IApiClient
    {
        private readonly Api _api;

        public RealApiClient(string url, string key)
        {
            _api = new Api(url, key);
        }

        public ApiResponse<List<ProductDTO>> ProductsFindAll() => _api.ProductsFindAll();
        public ApiResponse<List<ProductTypeDTO>> ProductTypesFindAll() => _api.ProductTypesFindAll();
        public ApiResponse<ProductDTO> ProductsFind(string bvin) => _api.ProductsFind(bvin);
        public ApiResponse<ProductDTO> ProductsUpdate(ProductDTO product) => _api.ProductsUpdate(product);
    }
}
