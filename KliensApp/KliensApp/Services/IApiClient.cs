using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Hotcakes.CommerceDTO.v1;
using Hotcakes.CommerceDTO.v1.Catalog;

namespace KliensApp.Services
{
    public interface IApiClient
    {
        ApiResponse<List<ProductDTO>> ProductsFindAll();
        ApiResponse<List<ProductTypeDTO>> ProductTypesFindAll();
        ApiResponse<ProductDTO> ProductsFind(string bvin);
        ApiResponse<ProductDTO> ProductsUpdate(ProductDTO product);
    }
}
