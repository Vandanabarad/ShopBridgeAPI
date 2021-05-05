using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using ShopBridgeAPI.Models;
using System.Threading.Tasks;
using System.Data.Entity;

namespace ShopBridgeAPI.Controllers
{
    public class ProductController : ApiController
    {
        private readonly ShopBridgeEntities shopBridgeEntities = new ShopBridgeEntities();

        [HttpGet]
        [ActionName("GetProductList")]
        public async Task<IHttpActionResult> GetProductList()
        {
            List<Product> products = null;
            try 
            {
                products = await shopBridgeEntities.InventoryProducts
                    .Select(s=> new Product() 
                    {
                       Id  = s.Id,
                       Description=s.Description,
                       Price= s.Price,
                       Quantity=s.Quantity

                    }).ToListAsync();
                
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
            if (products.Count == 0)
            {
                return NotFound();
            }
            return Ok(products);
        }

        [HttpPost]
        [ActionName("AddProduct")]

        public async Task<IHttpActionResult> AddProduct(Product product)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest("Invalid data.");
            }
            try
            {
                shopBridgeEntities.InventoryProducts.Add(new InventoryProduct()
                {
                    Description = product.Description,
                    Price = product.Price,
                    Quantity = product.Quantity
                });

                await shopBridgeEntities.SaveChangesAsync();
            }
            catch(Exception ex)
            {
                return BadRequest(ex.Message);
            }

            return Ok();
        }

        [HttpPut]
        [ActionName("UpdateProduct")]

        public async Task<IHttpActionResult> UpdateProduct(Product product)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest("Invalid data.");
            }
            try
            {
                var existingProduct = await shopBridgeEntities.InventoryProducts.Where(s => s.Id == product.Id).FirstOrDefaultAsync();

                if (existingProduct != null)
                {
                    existingProduct.Description = product.Description;
                    existingProduct.Price = product.Price;
                    existingProduct.Quantity = product.Quantity;

                    shopBridgeEntities.Entry(existingProduct).State = System.Data.Entity.EntityState.Modified;
                    await shopBridgeEntities.SaveChangesAsync();
                }
                else
                {
                    return NotFound();
                }

            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

            return Ok();

        }

        [HttpDelete]
        [ActionName("DeleteProduct")]
        public async Task<IHttpActionResult> DeleteProduct(int id)
        {
            if (id <= 0)
            {
                return BadRequest("Not a valid Product");

            }

            try
            {
                var product = await shopBridgeEntities.InventoryProducts.Where(s => s.Id == id).FirstOrDefaultAsync();

                if (product != null)
                {
                    shopBridgeEntities.Entry(product).State = System.Data.Entity.EntityState.Deleted;
                    await shopBridgeEntities.SaveChangesAsync();
                }
                else
                {
                    return NotFound();
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);

            }
            return Ok();

        }
    }
}
