using System;
using NLog.Web;
using System.IO;
using System.Linq;
using NorthwindConsole.Model;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace NorthwindConsole
{
    class Program
    {
        // create static instance of Logger
        private static NLog.Logger logger = NLogBuilder.ConfigureNLog(Directory.GetCurrentDirectory() + "\\nlog.config").GetCurrentClassLogger();
        static void Main(string[] args)
        {
            logger.Info("Program started");

            try
            {
                string choice;
                do
                {
                    Console.WriteLine("1) Display All Categories (Name & Desc.)");
                    Console.WriteLine("2) Add Category");
                    Console.WriteLine("3) Display a Category and its active Product data");
                    Console.WriteLine("4) Display All Categories and their active Product Data");
                    Console.WriteLine("5) Add Product");
                    Console.WriteLine("6) Edit Product");
                    Console.WriteLine("7) Display All Products");
                    Console.WriteLine("8) Display a Product");
                    Console.WriteLine("9) Edit Category");
                    Console.WriteLine("10) Delete a Product");
                    Console.WriteLine("11) Delete a Category");
                    Console.WriteLine("\"q\" to quit");
                    choice = Console.ReadLine();
                    Console.Clear();
                    logger.Info($"Option {choice} selected");
                    if (choice == "1")// x display cats
                    {
                        var db = new NorthwindConsole_32_WHContext();
                        var query = db.Categories.OrderBy(p => p.CategoryName);

                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.WriteLine($"{query.Count()} records returned");
                        Console.ForegroundColor = ConsoleColor.Magenta;
                        foreach (var item in query)
                        {
                            Console.WriteLine($"{item.CategoryName} - {item.Description}");
                        }
                        Console.ForegroundColor = ConsoleColor.White;
                    }
                    else if (choice == "2")// x add cats
                    {
                        Category category = new Category();
                        Console.WriteLine("Enter Category Name:");
                        category.CategoryName = Console.ReadLine();
                        Console.WriteLine("Enter the Category Description:");
                        category.Description = Console.ReadLine();

                        ValidationContext context = new ValidationContext(category, null, null);
                        List<ValidationResult> results = new List<ValidationResult>();

                        var isValid = Validator.TryValidateObject(category, context, results, true);
                        if (isValid)
                        {
                            var db = new NorthwindConsole_32_WHContext();
                            // check for unique name
                            if (db.Categories.Any(c => c.CategoryName == category.CategoryName))
                            {
                                // generate validation error
                                isValid = false;
                                results.Add(new ValidationResult("Name exists", new string[] { "CategoryName" }));
                            }
                            else
                            {
                                logger.Info("Validation passed");
                                db.AddCategory(category);
                            }
                        }
                        if (!isValid)
                        {
                            foreach (var result in results)
                            {
                                logger.Error($"{result.MemberNames.First()} : {result.ErrorMessage}");
                            }
                        }
                    }
                    else if (choice == "3")// x display cat+prod
                    {
                        var db = new NorthwindConsole_32_WHContext();
                        var query = db.Categories.OrderBy(p => p.CategoryId);

                        Console.WriteLine("Select the category whose products you want to display:");
                        Console.ForegroundColor = ConsoleColor.DarkRed;
                        foreach (var item in query)
                        {
                            Console.WriteLine($"{item.CategoryId}) {item.CategoryName}");
                        }
                        Console.ForegroundColor = ConsoleColor.White;
                        int id = int.Parse(Console.ReadLine());
                        Console.Clear();
                        logger.Info($"CategoryId {id} selected");
                        //lazy loading, will not populate the category.Products list
                        //Category category = db.Categories.FirstOrDefault(c => c.CategoryId == id);

                        //eager loading, will load Products list
                        Category category = db.Categories.Include("Products").FirstOrDefault(c => c.CategoryId == id);

                        Console.WriteLine($"{category.CategoryName} - {category.Description}");
                        foreach (Product p in category.Products)
                        {
                            var d = p.Discontinued ? "DISCONTINUED" : "ACTIVE";
                            if (!p.Discontinued) {
                                Console.ForegroundColor =  p.Discontinued ?  ConsoleColor.Red : ConsoleColor.Green;
                                Console.WriteLine($"[{p.ProductId}] {p.ProductName} Status:[{d}]");
                                Console.WriteLine($"\tSupplier: {p.SupplierId}\tCategory: {p.CategoryId}");
                                Console.WriteLine($"\tQtyPerUnit: {p.QuantityPerUnit}\tUnitPrice: {p.UnitPrice}");
                                Console.WriteLine($"\tInStock: {p.UnitsInStock}\tOnOrder: {p.UnitsOnOrder}\tReorderLevel: {p.ReorderLevel}");
                                Console.WriteLine();
                            }
                        }
                        Console.ForegroundColor = ConsoleColor.White;
                    }
                    else if (choice == "4")// x display all cat+prod
                    {
                        var db = new NorthwindConsole_32_WHContext();
                        var query = db.Categories.Include("Products").OrderBy(p => p.CategoryId);
                        foreach (var cat in query)
                        {
                            Console.WriteLine($"{cat.CategoryId}) {cat.CategoryName}");
                            foreach (Product p in cat.Products)
                            {
                                var d = p.Discontinued ? "DISCONTINUED" : "ACTIVE";
                                if (!p.Discontinued) {
                                    Console.ForegroundColor =  p.Discontinued ?  ConsoleColor.Red : ConsoleColor.Green;
                                    Console.WriteLine($"[{p.ProductId}] {p.ProductName} Status:[{d}]");
                                    Console.WriteLine($"\tSupplier: {p.SupplierId}\tCategory: {p.CategoryId}");
                                    Console.WriteLine($"\tQtyPerUnit: {p.QuantityPerUnit}\tUnitPrice: {p.UnitPrice}");
                                    Console.WriteLine($"\tInStock: {p.UnitsInStock}\tOnOrder: {p.UnitsOnOrder}\tReorderLevel: {p.ReorderLevel}");
                                    Console.WriteLine();
                                }
                            }
                        }
                        Console.ForegroundColor = ConsoleColor.White;
                    }
                    else if (choice == "5")// x add a product
                    {
                        Product product = new Product();
                        try
                        { //might not need this, the db doesn't care about junk values

                            Console.WriteLine("Enter ProductName:");
                            product.ProductName = Console.ReadLine();
                            Console.WriteLine("Enter the SupplierId :");
                            product.SupplierId = int.Parse(Console.ReadLine());
                            Console.WriteLine("Enter the CategoryId :");
                            product.CategoryId = int.Parse(Console.ReadLine());
                            Console.WriteLine("Enter the QuantityPerUnit :");
                            product.QuantityPerUnit = Console.ReadLine();
                            Console.WriteLine("Enter the UnitPrice :");
                            product.UnitPrice = decimal.Parse(Console.ReadLine());
                            Console.WriteLine("Enter the UnitsInStock :");
                            product.UnitsInStock = short.Parse(Console.ReadLine());
                            Console.WriteLine("Enter the UnitsOnOrder :");
                            product.UnitsOnOrder = short.Parse(Console.ReadLine());
                            Console.WriteLine("Enter the ReorderLevel :");
                            product.ReorderLevel = short.Parse(Console.ReadLine());
                            Console.WriteLine("Enter the Discontinued Status (true / false) :");
                            product.Discontinued = bool.Parse(Console.ReadLine());

                            ValidationContext context = new ValidationContext(product, null, null);
                            List<ValidationResult> results = new List<ValidationResult>();

                            var isValid = Validator.TryValidateObject(product, context, results, true);
                            if (isValid)
                            {
                                var db = new NorthwindConsole_32_WHContext();
                                // db = new NorthwindConsole_32_WHContext();
                                // check for unique name
                                if (db.Products.Any(c => c.ProductName == product.ProductName))
                                {
                                    // generate validation error
                                    isValid = false;
                                    results.Add(new ValidationResult("Product Name exists", new string[] { "CategoryName" }));
                                }
                                else if (!db.Suppliers.Any(s => s.SupplierId == product.SupplierId))
                                {
                                    //check for valid supplier id
                                    logger.Error("Invalid SupplierID");
                                    isValid = false;
                                }
                                else if (!db.Categories.Any(c => c.CategoryId == product.CategoryId))
                                {
                                    logger.Error("Invalid CategoryID");
                                    isValid = false;
                                }
                                else
                                {
                                    logger.Info("Validation passed");
                                    db.AddProduct(product);
                                    logger.Info($"Product - {product.ProductName} added");
                                }
                            }
                            if (!isValid)
                            {
                                foreach (var result in results)
                                {
                                    logger.Error($"{result.MemberNames.First()} : {result.ErrorMessage}");
                                }
                            }
                        }
                        catch (System.FormatException)
                        {
                            Console.ForegroundColor = ConsoleColor.White;  
                            logger.Error($"Incorrect datatype entered:");
                        }
                        catch (Exception e) {
                            logger.Error(e.Message);
                        }
                    }
                    else if (choice == "6")// x edit a product
                    {
                        //Product product = new Product();
                        try
                        {
                            var db = new NorthwindConsole_32_WHContext();
                            Console.WriteLine("Enter ProductID of Product to Edit:");
                            int pid = int.Parse(Console.ReadLine());
                            //grab from db
                            Product product = db.Products.SingleOrDefault(p => p.ProductId == pid);
                            Boolean hasChanges = false;
                            if (product != null)
                            {
                                Console.WriteLine("Press Enter to skip field, or type in new value");
                                Console.WriteLine($"ProductName: {product.ProductName} ?");
                                string productName = Console.ReadLine();
                                if (productName != "")
                                {
                                    product.ProductName = productName;
                                    hasChanges = true;
                                }

                                Console.WriteLine($"SupplierId : {product.SupplierId} ?");
                                string supplierId = Console.ReadLine();
                                if (supplierId != "")
                                {
                                    product.SupplierId = int.Parse(supplierId);
                                    hasChanges = true;
                                }

                                Console.WriteLine($"CategoryId : {product.CategoryId} ?");
                                string categoryId = Console.ReadLine();
                                if (categoryId != "")
                                {
                                    product.CategoryId = int.Parse(categoryId);
                                    hasChanges = true;
                                }
                                Console.WriteLine($"QuantityPerUnit : {product.QuantityPerUnit} ?");
                                string quantityPerUnit = Console.ReadLine();
                                if (quantityPerUnit != "")
                                {
                                    product.QuantityPerUnit = quantityPerUnit;
                                    hasChanges = true;
                                }
                                Console.WriteLine($"UnitPrice : {product.UnitPrice} ?");
                                string unitPrice = Console.ReadLine();
                                if (unitPrice != "")
                                {
                                    product.UnitPrice = decimal.Parse(unitPrice);
                                    hasChanges = true;
                                }
                                Console.WriteLine($"UnitsInStock : {product.UnitsInStock} ?");
                                string unitsInStock = Console.ReadLine();
                                if (unitsInStock != "")
                                {
                                    product.UnitsInStock = short.Parse(unitsInStock);
                                    hasChanges = true;
                                }
                                Console.WriteLine($"UnitsOnOrder : {product.UnitsOnOrder} ?");
                                string unitsOnOrder = Console.ReadLine();
                                if (unitsOnOrder != "")
                                {
                                    product.UnitsOnOrder = short.Parse(unitsOnOrder);
                                    hasChanges = true;
                                }
                                Console.WriteLine($"ReorderLevel : {product.ReorderLevel} ?");
                                string reorderLevel = Console.ReadLine();
                                if (reorderLevel != "")
                                {
                                    product.ReorderLevel = short.Parse(reorderLevel);
                                    hasChanges = true;
                                }
                                Console.WriteLine($"Discontinued : {product.Discontinued} ?");
                                string discontinued = Console.ReadLine();
                                if (discontinued != "")
                                {
                                    product.Discontinued = bool.Parse(discontinued);
                                    hasChanges = true;
                                }

                                if (hasChanges)
                                {
                                    ValidationContext context = new ValidationContext(product, null, null);
                                    List<ValidationResult> results = new List<ValidationResult>();

                                    var isValid = Validator.TryValidateObject(product, context, results, true);
                                    if (isValid)
                                    {
                                        if (!db.Suppliers.Any(s => s.SupplierId == product.SupplierId))
                                        {
                                            //check for valid supplier id
                                            logger.Error("Invalid SupplierID");
                                            isValid = false;
                                        }
                                        else if (!db.Categories.Any(c => c.CategoryId == product.CategoryId))
                                        {
                                            logger.Error("Invalid CategoryID");
                                            isValid = false;
                                        }
                                        else
                                        {
                                            logger.Info("Validation passed");
                                            //var changeMe = db.Products.SingleOrDefault(p => p.ProductId == product.ProductId);
                                            //changeMe.
                                            db.SaveChanges();
                                            logger.Info($"Product - {product.ProductName} updated");
                                        }
                                    }
                                    if (!isValid)
                                    {
                                        foreach (var result in results)
                                        {
                                            logger.Error($"{result.MemberNames.First()} : {result.ErrorMessage}");
                                        }
                                    }
                                }
                                else
                                {
                                    Console.ForegroundColor = ConsoleColor.White;  
                                    logger.Info("No Changes Entered.");
                                }
                            }
                            else
                            {
                                Console.ForegroundColor = ConsoleColor.White;  
                                logger.Error($"ProductID: {pid} not found/valid");
                            }
                        }
                        catch (System.FormatException)
                        {
                            Console.ForegroundColor = ConsoleColor.White;  
                            logger.Error($"Incorrect datatype entered:");
                        }
                        catch (Exception e) {
                            logger.Error(e.Message);
                        }
                    }
                    else if (choice == "7")// x display all products
                    {
                        try {
                            var db = new NorthwindConsole_32_WHContext();
                            IEnumerable<Product> query2;
                            Console.WriteLine("What Products would you to like to display?");
                            Console.WriteLine("1) All Products");
                            Console.WriteLine("2) Active Products");
                            Console.WriteLine("3) Discontinued Products");
                            int whatProducts = int.Parse(Console.ReadLine());
                            //switch query based on input
                            switch (whatProducts) {
                                case 1 : 
                                    query2 = db.Products; 
                                    foreach(var p in query2) {
                                        var d = p.Discontinued ? "DISCONTINUED" : "ACTIVE";
                                        Console.ForegroundColor =  p.Discontinued ?  ConsoleColor.Red : ConsoleColor.Green;
                                        Console.WriteLine($"[{p.ProductId}] {p.ProductName} Status:[{d}]");
                                        // Console.WriteLine($"\tSupplier: {p.SupplierId}\tCategory: {p.CategoryId}");
                                        // Console.WriteLine($"\tQtyPerUnit: {p.QuantityPerUnit}\tUnitPrice: {p.UnitPrice}");
                                        // Console.WriteLine($"\tInStock: {p.UnitsInStock}\tOnOrder: {p.UnitsOnOrder}\tReorderLevel: {p.ReorderLevel}");
                                        Console.WriteLine();
                                    }
                                    break;
                                case 2 : 
                                    query2 = db.Products.Where(p => p.Discontinued == false); 
                                    foreach(var p in query2) {
                                        var d = p.Discontinued ? "DISCONTINUED" : "ACTIVE";
                                        Console.ForegroundColor =  p.Discontinued ?  ConsoleColor.Red : ConsoleColor.Green;
                                        Console.WriteLine($"[{p.ProductId}] {p.ProductName} Status:[{d}]");
                                        // Console.WriteLine($"\tSupplier: {p.SupplierId}\tCategory: {p.CategoryId}");
                                        // Console.WriteLine($"\tQtyPerUnit: {p.QuantityPerUnit}\tUnitPrice: {p.UnitPrice}");
                                        // Console.WriteLine($"\tInStock: {p.UnitsInStock}\tOnOrder: {p.UnitsOnOrder}\tReorderLevel: {p.ReorderLevel}");
                                        Console.WriteLine();
                                    }
                                    break;
                                case 3 : 
                                    query2 = db.Products.Where(p => p.Discontinued == true); 
                                    foreach(var p in query2) {
                                        var d = p.Discontinued ? "DISCONTINUED" : "ACTIVE";
                                        Console.ForegroundColor =  p.Discontinued ?  ConsoleColor.Red : ConsoleColor.Green;
                                        Console.WriteLine($"[{p.ProductId}] {p.ProductName} Status:[{d}]");
                                        // Console.WriteLine($"\tSupplier: {p.SupplierId}\tCategory: {p.CategoryId}");
                                        // Console.WriteLine($"\tQtyPerUnit: {p.QuantityPerUnit}\tUnitPrice: {p.UnitPrice}");
                                        // Console.WriteLine($"\tInStock: {p.UnitsInStock}\tOnOrder: {p.UnitsOnOrder}\tReorderLevel: {p.ReorderLevel}");
                                        Console.WriteLine();
                                    }                                    
                                    break;
                                default : break;
                            }
                            Console.ForegroundColor = ConsoleColor.White;
                        }catch (System.FormatException) {
                            Console.ForegroundColor = ConsoleColor.White;  
                            Console.WriteLine("ProductID must be an integer");
                        }
                        catch (Exception e) {
                            logger.Error(e.Message);
                        }
                    }
                    else if (choice == "8")// x display specific product
                    {
                        //ask for id
                        Console.WriteLine("Enter ProductID of the Product would you to like to display?");
                        try {
                            var db = new NorthwindConsole_32_WHContext();
                            IEnumerable<Product> query2;

                            int pid = int.Parse(Console.ReadLine());
                            //switch query based on input
                            query2 = db.Products.Where(p => p.ProductId == pid); 
                            foreach(var p in query2) {
                                var d = p.Discontinued ? "DISCONTINUED" : "ACTIVE";
                                Console.ForegroundColor =  p.Discontinued ?  ConsoleColor.Red : ConsoleColor.Green;
                                Console.WriteLine($"[{p.ProductId}] {p.ProductName} Status:[{d}]");
                                Console.WriteLine($"\tSupplier: {p.SupplierId}\tCategory: {p.CategoryId}");
                                Console.WriteLine($"\tQtyPerUnit: {p.QuantityPerUnit}\tUnitPrice: {p.UnitPrice}");
                                Console.WriteLine($"\tInStock: {p.UnitsInStock}\tOnOrder: {p.UnitsOnOrder}\tReorderLevel: {p.ReorderLevel}");
                                Console.WriteLine();
                            }
                            Console.ForegroundColor = ConsoleColor.White;
                        }catch (System.FormatException) {
                            Console.ForegroundColor = ConsoleColor.White;  
                            Console.WriteLine("ProductID must be an integer");
                        }
                        catch (Exception e) {
                            logger.Error(e.Message);
                        }
                    }
                    else if (choice == "9")// x edit a category
                    {
                        //Product product = new Product();
                        try
                        {
                            var db = new NorthwindConsole_32_WHContext();
                            Console.WriteLine("Enter CategoryID of Category to Edit:");
                            int cid = int.Parse(Console.ReadLine());
                            //grab from db
                            Category cat = db.Categories.SingleOrDefault(c => c.CategoryId == cid);
                            Boolean hasChanges = false;
                            if (cat != null)
                            {
                                Console.WriteLine("Press Enter to skip field, or type in new value");
                                Console.WriteLine($"CategoryName: {cat.CategoryName} ?");
                                string categoryName = Console.ReadLine();
                                if (categoryName != "")
                                {
                                    cat.CategoryName = categoryName;
                                    hasChanges = true;
                                }

                                Console.WriteLine($"Description : {cat.Description} ?");
                                string description = Console.ReadLine();
                                if (description != "")
                                {
                                    cat.Description = description;
                                    hasChanges = true;
                                }
                                if (hasChanges)
                                {
                                    ValidationContext context = new ValidationContext(cat, null, null);
                                    List<ValidationResult> results = new List<ValidationResult>();

                                    var isValid = Validator.TryValidateObject(cat, context, results, true);
                                    if (isValid)
                                    {
                                        logger.Info("Validation passed");
                                        db.SaveChanges();
                                        logger.Info($"Category - {cat.CategoryName} updated");

                                    }
                                    if (!isValid)
                                    {
                                        foreach (var result in results)
                                        {
                                            logger.Error($"{result.MemberNames.First()} : {result.ErrorMessage}");
                                        }
                                    }
                                }
                                else
                                {
                                    Console.ForegroundColor = ConsoleColor.White;  
                                    logger.Info("No Changes Entered.");
                                }
                            }
                            else
                            {
                                Console.ForegroundColor = ConsoleColor.White;  
                                logger.Error($"ProductID: {cid} not found/valid");
                            }
                        }
                        catch (System.FormatException)
                        {
                            Console.ForegroundColor = ConsoleColor.White;  
                            logger.Error($"Incorrect datatype entered:");
                        }
                        catch (Exception e) {
                            logger.Error(e.Message);
                        }
                    }
                    else if (choice == "10")// x delete products
                    {
                        //orphans are orderdetails rows
                        //just wipe em
                        try
                        {
                            Console.ForegroundColor = ConsoleColor.Red;
                            var db = new NorthwindConsole_32_WHContext();
                            Console.WriteLine("Enter ProductID of the Product would you to like to DELETE?");
                            int pid = int.Parse(Console.ReadLine());
                            //grab from db
                            Product product = db.Products.SingleOrDefault(p => p.ProductId == pid);
                            
                            if (product != null)
                            {
                                var orderDetails = db.OrderDetails.Where(o => o.ProductId == product.ProductId);
                                Console.WriteLine($"Found {orderDetails.Count()} OrderDetails orphans");
                                Console.WriteLine($"Deleting Product {product.ProductName} will delete these as well are you sure? Y/N");
                                string amSure = Console.ReadLine().ToUpper();

                                if (amSure=="Y")
                                {
                                    db.OrderDetails.RemoveRange(db.OrderDetails.Where(x=>x.ProductId==product.ProductId));
                                    logger.Info($"OrderDetails with ProductId{product.ProductId} deleted!");
                                    db.SaveChanges();
                                    db.DeleteProduct(product);
                                    logger.Info($"Product - {product.ProductName} deleted!");
                                }
                                else
                                {
                                    logger.Info("Deletion canceled.");
                                }
                            }
                            else
                            {
                                logger.Error($"ProductID: {pid} not found/valid");
                            }
                        }
                        catch (System.FormatException)
                        {
                            Console.ForegroundColor = ConsoleColor.White;  
                            logger.Error($"Incorrect datatype entered:");
                        }
                        catch (Exception e) {
                            logger.Error(e.Message);
                        }
                        Console.ForegroundColor = ConsoleColor.White;                    
                    }
                    else if (choice == "11")// x delete catagories
                    {
                        //orphans are orderdetails rows
                        //just wipe em
                        
                        try
                        {
                            Console.ForegroundColor = ConsoleColor.Red;
                            var db = new NorthwindConsole_32_WHContext();
                            Console.WriteLine("Enter CategoryId of the Category would you to like to DELETE?");
                            int cid = int.Parse(Console.ReadLine());
                            //grab from db
                            Category category = db.Categories.SingleOrDefault(c => c.CategoryId == cid);
                            
                            if (category != null)
                            {
                                var ods = db.OrderDetails.FromSqlInterpolated($"SELECT o.Discount, o.OrderId,o.ProductId,o.UnitPrice,o.Quantity,o.OrderDetailsId FROM OrderDetails o JOIN Products p ON o.ProductId = p.ProductId JOIN Categories c  on p.CategoryId = c.CategoryId WHERE c.CategoryId = {cid}").ToList();
                                Console.WriteLine($"Orphan OrderDetailsFound : {ods.Count}");
                                foreach (var od in ods) {
                                    Console.WriteLine($"{od.OrderDetailsId}::");
                                }
                                var products = db.Products.Where(p=>p.CategoryId == cid);

                                Console.WriteLine($"Found {ods.Count()} OrderDetails orphans");
                                Console.WriteLine($"Found {products.Count()} Product orphans");
                                Console.WriteLine($"Deleting Product {category.CategoryId} will delete these as well are you sure? Y/N");
                                string amSure = Console.ReadLine().ToUpper();

                                if (amSure=="Y")
                                {
                                    foreach(var od in ods) {
                                        db.OrderDetails.Remove(od);
                                    }

                                    logger.Info($"OrderDetails with CategoryId{category.CategoryId} deleted!");
                                    db.SaveChanges();

                                    db.Products.RemoveRange(db.Products.Where(p=>p.CategoryId==category.CategoryId));
                                    logger.Info($"Products with CategoryId{category.CategoryId} deleted!");
                                    db.SaveChanges();

                                    db.DeleteCategory(category);
                                    logger.Info($"category - {category.CategoryName} deleted!");
                                }
                                else
                                {
                                    logger.Info("Deletion canceled.");
                                }
                            }
                            else
                            {
                                logger.Error($"CategoryId: {cid} not found/valid");
                            }
                        }
                        catch (System.FormatException)
                        {
                            Console.ForegroundColor = ConsoleColor.White;  
                            logger.Error($"Incorrect datatype entered:");
                        }
                        catch (Exception e) {
                            logger.Error(e.Message);
                        }
                        Console.ForegroundColor = ConsoleColor.White;  
                    }
                    Console.WriteLine();
                } while (choice.ToLower() != "q");
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.White;  
                logger.Error(ex.Message);
            }
            logger.Info("Program ended");
        }
    }
}
