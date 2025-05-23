﻿using MongoDB.Bson;
using MongoDB.Driver;
using StoreFrontDb;
using StoreFrontModel;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StoreFrontRepository
{
    public class StoreFrontRepository : IStoreFront
    {
        MongoContext context = null;
        IMongoDatabase mongoDatabase = null;

        public StoreFrontRepository()
        {
            try
            {
                context = new MongoContext();
                mongoDatabase = context.mongoDatabase;

            }
            catch (StoreFrontException ex)
            {
               throw new StoreFrontException($"Error Connecting to Database {ex.Message}");

            }
        }


        public bool CreateUser(User user)
        {
            try
            {

                
                var hashpassword = BCrypt.Net.BCrypt.HashPassword(user.Password);
                user.Password = hashpassword;

                mongoDatabase.GetCollection<User>("users").InsertOne(user);
                return true;
            }catch(Exception ex)
            {
                throw new StoreFrontException($"Error Creating New User { ex.Message }");
            }
        }
        public async Task<List<Product>> GetAllMenProduct()
        {
            try
            {

                var menCats = await mongoDatabase.GetCollection<Category>("category")
                    .FindAsync(x => x.Name == "Men");

                var menCat = menCats.FirstOrDefault();


                if (menCat == null)
                {
                   
                    return new List<Product>();
                }

                
                var filter = Builders<Product>.Filter.AnyEq(p => p.CategoryIds, menCat.Id);

                var menProducts = await mongoDatabase.GetCollection<Product>("products")
                    .FindAsync(filter);



                var menProductsList = await menProducts.ToListAsync();
                   

                if (menProductsList == null || menProductsList.Count == 0)
                {
                    return new List<Product>();
                }
                else
                {
                    foreach (var product in menProductsList)
                    {
                        var categoriesCollection = mongoDatabase.GetCollection<Category>("category");
                        product.Categories = new List<Category>();
                        foreach (var categoryId in product.CategoryIds)
                        {
                            var categoryCursor = await categoriesCollection.FindAsync(c => c.Id == categoryId);
                            var category = await categoryCursor.FirstOrDefaultAsync();
                            if (category != null)
                            {
                                product.Categories.Add(category);
                            }
                        }
                        if (product.IvaTypeId != ObjectId.Empty)
                        {
                            var vatCursor = await mongoDatabase.GetCollection<Vat>("vat").FindAsync(v => v.Id == product.IvaTypeId);
                            product.IvaType = await vatCursor.FirstOrDefaultAsync();
                        }
                        if (product.TagId != ObjectId.Empty)
                        {
                            var tagCursor = await mongoDatabase.GetCollection<ProductTag>("tag").FindAsync(t => t.Id == product.TagId);
                            product.Tag = await tagCursor.FirstOrDefaultAsync();
                        }
                    }
                }

                    return menProductsList;
            }
            catch (Exception ex)
            {
         
                throw new StoreFrontException($"Error Getting All Men Shoes: {ex.Message}");
            }
        }

        public async Task<List<Product>> GetAllWomenProduct()
        {
            try
            {

                var womenCats= await mongoDatabase.GetCollection<Category>("category")
                .FindAsync(x => x.Name == "Women");

                var women = womenCats.FirstOrDefault();

                if(women == null)
                {
                    return new List<Product>();
                }

                var filter = Builders<Product>.Filter.AnyEq(p => p.CategoryIds, women.Id);


                var womenProducts = await mongoDatabase.GetCollection<Product>("products")
                    .FindAsync(filter);

              var womenProductList =  await womenProducts.ToListAsync();
                if(womenProductList == null)
                {
                    return new List<Product>();
                }
                else
                {
                    foreach (var product in womenProductList)
                    {
                        var categoriesCollection = mongoDatabase.GetCollection<Category>("category");
                        product.Categories = new List<Category>();
                        foreach (var categoryId in product.CategoryIds)
                        {
                            var categoryCursor = await categoriesCollection.FindAsync(c => c.Id == categoryId);
                            var category = await categoryCursor.FirstOrDefaultAsync();
                            if (category != null)
                            {
                                product.Categories.Add(category);
                            }
                        }
                        if (product.IvaTypeId != ObjectId.Empty)
                        {
                            var vatCursor = await mongoDatabase.GetCollection<Vat>("vat").FindAsync(v => v.Id == product.IvaTypeId);
                            product.IvaType = await vatCursor.FirstOrDefaultAsync();
                        }
                        if (product.TagId != ObjectId.Empty)
                        {
                            var tagCursor = await mongoDatabase.GetCollection<ProductTag>("tag").FindAsync(t => t.Id == product.TagId);
                            product.Tag = await tagCursor.FirstOrDefaultAsync();
                        }
                    }
                }

                return womenProductList;
            }

            catch (Exception ex)
            {
                throw new StoreFrontException($"Error Getting All Women Shoes: {ex.Message}");
            }
        }


        public async Task<string> GenerateInvoiceNumberAsync()
        {
            try
            {
                int currentYear = DateTime.Now.Year;
                string counterName = $"invoice-{currentYear}";

                var counters = mongoDatabase.GetCollection<Counter>("invoiceCounter");

                var filter = Builders<Counter>.Filter.Eq(c => c.Name, counterName);
                var update = Builders<Counter>.Update.Inc(c => c.Value, 1);

                var options = new FindOneAndUpdateOptions<Counter>
                {
                    IsUpsert = true,
                    ReturnDocument = ReturnDocument.After
                };

                var counter = await counters.FindOneAndUpdateAsync(filter, update, options);

            
                return $"{currentYear}-{counter.Value:D6}";
            }
            catch (Exception ex)
            {
                throw new StoreFrontException($"Error generating invoice number: {ex.Message}");
            }
        }



        public async Task<List<Product>> GetAllSportsProduct()
        {
            try
            {

                var sportsCats = await mongoDatabase.GetCollection<Category>("category")
                .FindAsync(x => x.Name == "Sports");

                var sports = sportsCats.FirstOrDefault();

                if (sports == null)
                {
                    return new List<Product>();
                }

                var filter = Builders<Product>.Filter.AnyEq(p => p.CategoryIds, sports.Id);


                var sportsProducts = await mongoDatabase.GetCollection<Product>("products")
                    .FindAsync(filter);

                var sportsproductList = await sportsProducts.ToListAsync();
                if (sportsproductList == null)
                {
                    return new List<Product>();
                }
                else
                {
                    foreach (var product in sportsproductList)
                    {
                        var categoriesCollection = mongoDatabase.GetCollection<Category>("category");
                        product.Categories = new List<Category>();
                        foreach (var categoryId in product.CategoryIds)
                        {
                            var categoryCursor = await categoriesCollection.FindAsync(c => c.Id == categoryId);
                            var category = await categoryCursor.FirstOrDefaultAsync();
                            if (category != null)
                            {
                                product.Categories.Add(category);
                            }
                        }
                        if (product.IvaTypeId != ObjectId.Empty)
                        {
                            var vatCursor = await mongoDatabase.GetCollection<Vat>("vat").FindAsync(v => v.Id == product.IvaTypeId);
                            product.IvaType = await vatCursor.FirstOrDefaultAsync();
                        }
                        if (product.TagId != ObjectId.Empty)
                        {
                            var tagCursor = await mongoDatabase.GetCollection<ProductTag>("tag").FindAsync(t => t.Id == product.TagId);
                            product.Tag = await tagCursor.FirstOrDefaultAsync();
                        }
                    }
                }

                return sportsproductList;
            }

            catch (Exception ex)
            {
                throw new StoreFrontException($"Error Getting All Sports Shoes: {ex.Message}");
            }
        }

      

        public async Task<Product> GetProductByName(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new StoreFrontException("Product Name cannot be null or empty");
            }

            try
            {
                var productCursor = await mongoDatabase.GetCollection<Product>("products")
                    .FindAsync(x => x.Name == name);

                var product = await productCursor.FirstOrDefaultAsync(); 

                if (product == null)
                {
                    throw new StoreFrontException("Product not found");
                }

                var categoriesCollection = mongoDatabase.GetCollection<Category>("category");
                product.Categories = new List<Category>();

                foreach (var categoryId in product.CategoryIds)
                {
                    var categoryCursor = await categoriesCollection.FindAsync(c => c.Id == categoryId);
                    var category = await categoryCursor.FirstOrDefaultAsync();
                    if (category != null)
                    {
                        product.Categories.Add(category);
                    }
                }

                if (product.IvaTypeId != ObjectId.Empty)
                {
                    var vatCursor = await mongoDatabase.GetCollection<Vat>("vat").FindAsync(v => v.Id == product.IvaTypeId);
                    product.IvaType = await vatCursor.FirstOrDefaultAsync();
                }

                if (product.TagId != ObjectId.Empty)
                {
                    var tagCursor = await mongoDatabase.GetCollection<ProductTag>("tag").FindAsync(t => t.Id == product.TagId);
                    product.Tag = await tagCursor.FirstOrDefaultAsync();
                }

                return product;
            }
            catch (Exception ex)
            {
                throw new StoreFrontException($"Error Getting Product by Name: {ex.Message}");
            }
        }

        public User LoginUser(string username, string password)
        {
            if (username == null || password == null)
            {
                throw new StoreFrontException("Username or Password cannot be null");
            }

         
            var user = mongoDatabase.GetCollection<User>("users")
                .Find(x => x.Username == username)
                .FirstOrDefault();

        
            if (user == null || !VerifyPassword(user.Password, password))
            {
                return null;
            }

            return user;
        }

        

        private bool VerifyPassword(string storedPasswordHash, string enteredPassword)
        {
      
            return BCrypt.Net.BCrypt.Verify(enteredPassword, storedPasswordHash);
        }

        public async Task<List<Product>> GetAllChildrenProduct()
        {
            try
            {

                var childrenCats = await mongoDatabase.GetCollection<Category>("category")
                .FindAsync(x => x.Name == "Children");
                var children = childrenCats.FirstOrDefault();
                if (children == null)
                {
                    return new List<Product>();
                }
                var filter = Builders<Product>.Filter.AnyEq(p => p.CategoryIds, children.Id);

                var childrenProducts = await mongoDatabase.GetCollection<Product>("products")
                    .FindAsync(filter);
                var childrenProductList = await childrenProducts.ToListAsync();

                if (childrenProductList == null)
                {
                    return new List<Product>();
                }
                else
                {
                    foreach (var product in childrenProductList)
                    {
                        var categoriesCollection = mongoDatabase.GetCollection<Category>("category");
                        product.Categories = new List<Category>();
                        foreach (var categoryId in product.CategoryIds)
                        {           
                            var categoryCursor = categoriesCollection.Find(c => c.Id == categoryId);
                            var category = categoryCursor.FirstOrDefault();
                            if (category != null)
                            {
                                product.Categories.Add(category);
                            }
                        }
                        if (product.IvaTypeId != ObjectId.Empty)
                        {
                            var vatCursor = mongoDatabase.GetCollection<Vat>("vat").Find(v => v.Id == product.IvaTypeId);
                            product.IvaType = vatCursor.FirstOrDefault();
                        }
                        if (product.TagId != ObjectId.Empty)
                        {
                            var tagCursor = mongoDatabase.GetCollection<ProductTag>("tag").Find(t => t.Id == product.TagId);
                            product.Tag = tagCursor.FirstOrDefault();
                        }
                    }
                }

                return childrenProductList;
            }
            catch (Exception ex)
            {
                throw new StoreFrontException($"Error Getting All Children Shoes: {ex.Message}");
            }
        }

        public async Task<List<Product>> GetNewProducts()
        {
            try
            {
             var tag =   await mongoDatabase.GetCollection<ProductTag>("tag")
                    .FindAsync(x => x.Name == "NEW");

              var tagRes = await tag.FirstOrDefaultAsync();
                if (tagRes == null)
                {
                    return new List<Product>();
                }

                var filter = Builders<Product>.Filter.Eq(p => p.TagId, tagRes.Id);

                var products = await mongoDatabase.GetCollection<Product>("products")
                    .FindAsync(filter);
                var productList = await products.ToListAsync();

                if(productList == null)
                {
                    return new List<Product>();
                }
                else
                {
                    foreach (var product in productList)
                    {
                        var categoriesCollection = mongoDatabase.GetCollection<Category>("category");
                        product.Categories = new List<Category>();
                        foreach (var categoryId in product.CategoryIds)
                        {
                            var categoryCursor = await categoriesCollection.FindAsync(c => c.Id == categoryId);
                            var category = await categoryCursor.FirstOrDefaultAsync();
                            if (category != null)
                            {
                                product.Categories.Add(category);
                            }
                        }
                        if (product.IvaTypeId != ObjectId.Empty)
                        {
                            var vatCursor = await mongoDatabase.GetCollection<Vat>("vat").FindAsync(v => v.Id == product.IvaTypeId);
                            product.IvaType = await vatCursor.FirstOrDefaultAsync();
                        }
                        if (product.TagId != ObjectId.Empty)
                        {
                            var tagCursor = await mongoDatabase.GetCollection<ProductTag>("tag").FindAsync(t => t.Id == product.TagId);
                            product.Tag = await tagCursor.FirstOrDefaultAsync();
                        }
                    }
                }
                return productList;
            }
            catch (Exception ex)
            {
                throw new StoreFrontException($"Error Getting New Products: {ex.Message}");
            }
        }

        public async Task<List<Product>> SearchProductsByNameAsync(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                return new List<Product>();

            try
            {
                var filter = Builders<Product>.Filter.Regex("Name", new MongoDB.Bson.BsonRegularExpression(name, "i"));
                var products = await mongoDatabase.GetCollection<Product>("products")
                                                  .Find(filter)
                                                  .Limit(3)
                                                  .ToListAsync();
                if(products == null || products.Count == 0)
                {
                    return new List<Product>();
                }
                else
                {
                    foreach (var product in products)
                    {
                        var categoriesCollection = mongoDatabase.GetCollection<Category>("category");
                        product.Categories = new List<Category>();
                        foreach (var categoryId in product.CategoryIds)
                        {
                            var categoryCursor = await categoriesCollection.FindAsync(c => c.Id == categoryId);
                            var category = await categoryCursor.FirstOrDefaultAsync();
                            if (category != null)
                            {
                                product.Categories.Add(category);
                            }
                        }
                        if (product.IvaTypeId != ObjectId.Empty)
                        {
                            var vatCursor = await mongoDatabase.GetCollection<Vat>("vat").FindAsync(v => v.Id == product.IvaTypeId);
                            product.IvaType = await vatCursor.FirstOrDefaultAsync();
                        }
                        if (product.TagId != ObjectId.Empty)
                        {
                            var tagCursor = await mongoDatabase.GetCollection<ProductTag>("tag").FindAsync(t => t.Id == product.TagId);
                            product.Tag = await tagCursor.FirstOrDefaultAsync();
                        }
                    }
                }
                return products;
            }
            catch (Exception ex)
            {
                throw new StoreFrontException($"Error searching products: {ex.Message}");
            }
        }

        public async Task<List<Product>> GetAllProducts()
        {
            try
            {
                var products = await mongoDatabase.GetCollection<Product>("products")
                    .FindAsync(_ => true);
                var productList = await products.ToListAsync();

                if(productList == null || productList.Count<0) 
                {
                    return new List<Product>();
                }
                else
                {
                    foreach (var product in productList)
                    {
                        var categoriesCollection = mongoDatabase.GetCollection<Category>("category");
                        product.Categories = new List<Category>();
                        foreach (var categoryId in product.CategoryIds)
                        {
                            var categoryCursor = await categoriesCollection.FindAsync(c => c.Id == categoryId);
                            var category = await categoryCursor.FirstOrDefaultAsync();
                            if (category != null)
                            {
                                product.Categories.Add(category);
                            }
                        }
                        if (product.IvaTypeId != ObjectId.Empty)
                        {
                            var vatCursor = await mongoDatabase.GetCollection<Vat>("vat").FindAsync(v => v.Id == product.IvaTypeId);
                            product.IvaType = await vatCursor.FirstOrDefaultAsync();
                        }
                        if (product.TagId != ObjectId.Empty)
                        {
                            var tagCursor = await mongoDatabase.GetCollection<ProductTag>("tag").FindAsync(t => t.Id == product.TagId);
                            product.Tag = await tagCursor.FirstOrDefaultAsync();
                        }
                    }
                }
                return productList;

            }
            catch (Exception ex)
            {
                throw new StoreFrontException($"Error Getting All Products: {ex.Message}");
            }
        }

        public async Task<List<Product>> GetReleasedProducts()
        {
            try
            {
                var tag = await mongoDatabase.GetCollection<ProductTag>("tag")
                       .FindAsync(x => x.Name == "RELEASED");

                var tagRes = await tag.FirstOrDefaultAsync();
                if (tagRes == null)
                {
                    return new List<Product>();
                }

                var filter = Builders<Product>.Filter.Eq(p => p.TagId, tagRes.Id);

                var products = await mongoDatabase.GetCollection<Product>("products")
                    .FindAsync(filter);
                var productList = await products.ToListAsync();

                if (productList == null)
                {
                    return new List<Product>();
                }
                else
                {
                    foreach (var product in productList)
                    {
                        var categoriesCollection = mongoDatabase.GetCollection<Category>("category");
                        product.Categories = new List<Category>();
                        foreach (var categoryId in product.CategoryIds)
                        {
                            var categoryCursor = await categoriesCollection.FindAsync(c => c.Id == categoryId);
                            var category = await categoryCursor.FirstOrDefaultAsync();
                            if (category != null)
                            {
                                product.Categories.Add(category);
                            }
                        }
                        if (product.IvaTypeId != ObjectId.Empty)
                        {
                            var vatCursor = await mongoDatabase.GetCollection<Vat>("vat").FindAsync(v => v.Id == product.IvaTypeId);
                            product.IvaType = await vatCursor.FirstOrDefaultAsync();
                        }
                        if (product.TagId != ObjectId.Empty)
                        {
                            var tagCursor = await mongoDatabase.GetCollection<ProductTag>("tag").FindAsync(t => t.Id == product.TagId);
                            product.Tag = await tagCursor.FirstOrDefaultAsync();
                        }
                    }
                }
                return productList;
            }
            catch (Exception ex)
            {
                throw new StoreFrontException($"Error Getting Released Products: {ex.Message}");
            }
        }

        public async Task<Cart> GetCartByUserId(ObjectId userId)
        {
            try
            {

                var cat = await mongoDatabase.GetCollection<Cart>("cart")
                    .FindAsync(x => x.UserId == userId && x.Purchased== false);

                var userCat = await cat.FirstOrDefaultAsync();
                if(userCat == null)
                {
                    return null;
                }
                else
                {
                    return userCat;
                }

               
            }
            catch(Exception ex)
            {
                throw new StoreFrontException($"Error Getting Cart by UserId: {ex.Message}");
            }
        }

        public async Task AddOrUpdateCart(Cart cart, ObjectId userId)
        {
            try
            {
                if(cart == null)
                {
                    throw new StoreFrontException("Cart cannot be null");
                }

                if(userId == null)
                {
                    throw new StoreFrontException("UserId cannot be null");
                }

                var filter = Builders<Cart>.Filter.Eq(c => c.UserId, cart.UserId);
                var update = Builders<Cart>.Update
                    .SetOnInsert(c => c.CreatedAt, cart.CreatedAt)
                    .Set(c => c.Items, cart.Items)
                    .Set(c => c.SubTotal, cart.SubTotal)
                    .Set(c => c.ShippingCost, cart.ShippingCost)
                    .Set(c => c.TotalVat, cart.TotalVat)
                    .Set(c => c.Total, cart.Total)
                    .Set(c => c.Purchased, cart.Purchased)
                    .Set(c => c.UpdatedAt, DateTime.Now);


                var options = new UpdateOptions { IsUpsert = true }; 

                await mongoDatabase.GetCollection<Cart>("cart").UpdateOneAsync(filter, update, options);

            }
            catch (Exception ex)
            {
                throw new StoreFrontException($"Error Adding/Updating Cart: {ex.Message}");
            }
        }

        public async Task<List<ShippingMethod>> GetShippingMethods()
        {
            try
            {

                var shippingMethods = await mongoDatabase.GetCollection<ShippingMethod>("shippingMethods")
                    .FindAsync(_ => true);
                var shippingMethodList = await shippingMethods.ToListAsync();

                if (shippingMethodList == null || shippingMethodList.Count < 0)
                {
                    return new List<ShippingMethod>();
                }
               
                return shippingMethodList;

            }
            catch (Exception ex)
            {
                throw new StoreFrontException($"Error Getting Shipping Methods: {ex.Message}");
            }
        }

        public async Task<Company> GetCompanyInfo()
        {
            try
            {


                var company = await mongoDatabase.GetCollection<Company>("store").FindAsync(x=>x.Name == "Xapatos Wearables");
                var companyInfo = await company.FirstOrDefaultAsync();

                if(companyInfo == null)
                {
                    return null;
                }
                else
                {
                    return companyInfo;
                }

            }
            catch (Exception ex)
            {
                throw new StoreFrontException($"Error Getting Company Info: {ex.Message}");
            }
        }

        public async Task<bool> DeleteCart(ObjectId userId, ObjectId cartid)
        {
            bool ans = false;

            try
            {
                var cartsCollection = mongoDatabase.GetCollection<Cart>("cart");
                var deleteResult = await cartsCollection.DeleteOneAsync(c => c.UserId == userId && c.CartId == cartid);
                ans = true;
                
            }
            catch (Exception ex)
            {
                throw new StoreFrontException($"Error deleting cart: {ex.Message}");
               
            }

            return ans;
        }

   public async Task<bool> DeleteStock(ObjectId productId, ObjectId variantId, string size, int quantity)
{
    try
    {
        var products = mongoDatabase.GetCollection<Product>("products");

        var filter = Builders<Product>.Filter.And(
            Builders<Product>.Filter.Eq(p => p.Id, productId),
            Builders<Product>.Filter.ElemMatch(p => p.Variants, v => v.Id == variantId)
        );

        var update = Builders<Product>.Update.Inc("variants.$[variant].sizes.$[size].stock", -quantity);

        var arrayFilters = new List<ArrayFilterDefinition>
        {
            new JsonArrayFilterDefinition<BsonDocument>("{ 'variant._id': ObjectId('" + variantId + "') }"),
            new JsonArrayFilterDefinition<BsonDocument>("{ 'size.size': '" + size + "' }")
        };

        var updateOptions = new UpdateOptions { ArrayFilters = arrayFilters };

        var result = await products.UpdateOneAsync(filter, update, updateOptions);

        return result.ModifiedCount > 0;
    }
    catch (Exception ex)
    {
        throw new StoreFrontException($"Error deleting stock: {ex.Message}");
    }
}

        public async Task<bool> SaveInvoice(Invoice invoice)
        {
            bool ans = false;

            try
            {

                var invoicesCollection =  mongoDatabase.GetCollection<Invoice>("invoice");
                await invoicesCollection.InsertOneAsync(invoice);
                  ans = false;

            }
            catch (Exception ex)
            {
                throw new StoreFrontException($"Error Saving Invoice: {ex.Message}");

            }

            return ans;
        }

        public async Task<bool> ResetPassword(string email, string username, string newPassword)
        {
            bool ans = false;

            try
            {

                var usercon = await mongoDatabase.GetCollection<User>("users")
                    .FindAsync(x => x.Username == username && x.Email == email);

                var user = await usercon.FirstOrDefaultAsync();

                if (user == null)
                {

                    return false;
                }
                else
                {
                    var hashpassword = BCrypt.Net.BCrypt.HashPassword(newPassword);
                    user.Password = hashpassword;
                    var filter = Builders<User>.Filter.Eq(u => u.Id, user.Id);
                    var update = Builders<User>.Update.Set(u => u.Password, user.Password);
                    await mongoDatabase.GetCollection<User>("users").UpdateOneAsync(filter, update);
                    ans = true;
                }


                }
            catch(Exception ex)
            {
                throw new StoreFrontException($"Error Resetting Password: {ex.Message}");
            }

            return ans;
        }

        public async Task<List<Invoice>> GetUserInvoice(ObjectId userId)
        {
            try
            {
                var userCursor = await mongoDatabase
                    .GetCollection<User>("users")
                    .FindAsync(x => x.Id == userId);

                var user = await userCursor.FirstOrDefaultAsync();

                if (user?.Invoices == null || user.Invoices.Count == 0)
                {
                    return new List<Invoice>();
                }

                var filter = Builders<Invoice>.Filter.In(i => i.InvoiceNumber, user.Invoices);
                var invoiceCursor = await mongoDatabase
                    .GetCollection<Invoice>("invoice")
                    .FindAsync(filter);

                return await invoiceCursor.ToListAsync();
            }
            catch (Exception ex)
            {
                throw new StoreFrontException($"Error Getting User Invoices: {ex.Message}");
            }
        }

        public async Task<bool> UpdateUser(User user)
        {
            try
            {
                var collection = mongoDatabase.GetCollection<User>("users");
                var result = await collection.ReplaceOneAsync(
                    Builders<User>.Filter.Eq(u => u.Id, user.Id),
                    user
                );

                return result.ModifiedCount > 0;
            }
            catch (Exception ex)
            {
                throw new StoreFrontException($"Error Updating User: {ex.Message}");
            }
        }

        public async Task<bool> ChangeStockAsync(ObjectId productId, ObjectId variantId, string size, int quantity)
        {
            try
            {
                var products = mongoDatabase.GetCollection<Product>("products");

                var filter = Builders<Product>.Filter.And(
                    Builders<Product>.Filter.Eq(p => p.Id, productId),
                    Builders<Product>.Filter.ElemMatch(p => p.Variants, v => v.Id == variantId)
                );

                var update = Builders<Product>.Update.Inc("variants.$[variant].sizes.$[size].stock", quantity);

                var arrayFilters = new List<ArrayFilterDefinition>
        {
            new JsonArrayFilterDefinition<BsonDocument>("{ 'variant._id': ObjectId('" + variantId + "') }"),
            new JsonArrayFilterDefinition<BsonDocument>("{ 'size.size': '" + size + "' }")
        };

                var updateOptions = new UpdateOptions { ArrayFilters = arrayFilters };

                var result = await products.UpdateOneAsync(filter, update, updateOptions);

                return result.ModifiedCount > 0;
            }
            catch (Exception ex)
            {
                throw new StoreFrontException($"Error Changing Stock: {ex.Message}");
            }
        }


    }
}
