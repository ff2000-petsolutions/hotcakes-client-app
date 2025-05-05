using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hotcakes.CommerceDTO.v1;
using Hotcakes.CommerceDTO.v1.Catalog;
using Hotcakes.CommerceDTO.v1.Client;
using KliensApp.Services;
using Moq;
using Xunit;

namespace KliensApp.Tests
{
    public class ProductServiceTests
    {
        private readonly Mock<IApiClient> mockApi;
        private readonly ProductService productService;

        public ProductServiceTests()
        {
            mockApi = new Mock<IApiClient>();
            productService = new ProductService(mockApi.Object);
        }

        [Fact]
        public async Task GetAllProductsAsync_ReturnsProducts_WhenNoErrors()
        {
            var expected = new List<ProductDTO> { new ProductDTO { ProductName = "Teszt 1" } };

            mockApi.Setup(a => a.ProductsFindAll())
                .Returns(new ApiResponse<List<ProductDTO>> { Content = expected, Errors = new List<ApiError>() });

            var result = await productService.GetAllProductsAsync();

            Assert.Single(result);
            Assert.Equal("Teszt 1", result[0].ProductName);
        }

        [Fact]
        public async Task GetAllProductsAsync_Throws_WhenErrorsExist()
        {
            mockApi.Setup(a => a.ProductsFindAll())
                .Returns(new ApiResponse<List<ProductDTO>>
                {
                    Content = new List<ProductDTO>(),
                    Errors = new List<ApiError> { new ApiError { Description = "hiba" } }
                });

            var ex = await Assert.ThrowsAsync<Exception>(() => productService.GetAllProductsAsync());
            Assert.Contains("hiba", ex.Message);
        }

        [Fact]
        public async Task GetAllProductTypesAsync_ReturnsTypes_WhenNoErrors()
        {
            var expected = new List<ProductTypeDTO> { new ProductTypeDTO { ProductTypeName = "Típus A" } };

            mockApi.Setup(a => a.ProductTypesFindAll())
                .Returns(new ApiResponse<List<ProductTypeDTO>> { Content = expected, Errors = new List<ApiError>() });

            var result = await productService.GetAllProductTypesAsync();

            Assert.Single(result);
            Assert.Equal("Típus A", result[0].ProductTypeName);
        }

        [Fact]
        public async Task GetAllProductTypesAsync_Throws_WhenErrorsExist()
        {
            mockApi.Setup(a => a.ProductTypesFindAll())
                .Returns(new ApiResponse<List<ProductTypeDTO>>
                {
                    Content = new List<ProductTypeDTO>(),
                    Errors = new List<ApiError> { new ApiError { Description = "hiba" } }
                });

            var ex = await Assert.ThrowsAsync<Exception>(() => productService.GetAllProductTypesAsync());
            Assert.Contains("hiba", ex.Message);
        }

        [Fact]
        public async Task GetProductByIdAsync_ReturnsProduct_WhenNoErrors()
        {
            var expected = new ProductDTO { ProductName = "ID szerint termék" };

            mockApi.Setup(a => a.ProductsFind("123"))
                .Returns(new ApiResponse<ProductDTO> { Content = expected, Errors = new List<ApiError>() });

            var result = await productService.GetProductByIdAsync("123");

            Assert.Equal("ID szerint termék", result.ProductName);
        }

        [Fact]
        public async Task GetProductByIdAsync_Throws_WhenErrorsExist()
        {
            mockApi.Setup(a => a.ProductsFind("123"))
                .Returns(new ApiResponse<ProductDTO>
                {
                    Content = null,
                    Errors = new List<ApiError> { new ApiError { Description = "hiba" } }
                });

            var ex = await Assert.ThrowsAsync<Exception>(() => productService.GetProductByIdAsync("123"));
            Assert.Contains("hiba", ex.Message);
        }

        [Fact]
        public async Task UpdateProductAsync_ReturnsProduct_WhenNoErrors()
        {
            var updated = new ProductDTO { ProductName = "Frissített" };

            mockApi.Setup(a => a.ProductsUpdate(It.IsAny<ProductDTO>()))
                .Returns(new ApiResponse<ProductDTO> { Content = updated, Errors = new List<ApiError>() });

            var result = await productService.UpdateProductAsync(new ProductDTO());

            Assert.Equal("Frissített", result.ProductName);
        }

        [Fact]
        public async Task UpdateProductAsync_Throws_WhenErrorsExist()
        {
            mockApi.Setup(a => a.ProductsUpdate(It.IsAny<ProductDTO>()))
                .Returns(new ApiResponse<ProductDTO>
                {
                    Content = null,
                    Errors = new List<ApiError> { new ApiError { Description = "hiba" } }
                });

            var ex = await Assert.ThrowsAsync<Exception>(() => productService.UpdateProductAsync(new ProductDTO()));
            Assert.Contains("hiba", ex.Message);
        }
    }
}
