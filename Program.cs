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
                    Console.WriteLine("1) Display Categories");
                    Console.WriteLine("2) Add Category");
                    Console.WriteLine("3) Display Category and related products");
                    Console.WriteLine("4) Display all Categories and their related products");
                    //newstuff C threshold
                    Console.WriteLine("5) Add Product");
                    Console.WriteLine("6) Edit Product");
                    Console.WriteLine("7) Display All Products");
                    Console.WriteLine("8) Display a Product");
                    //newstuff B threshold
                    //add Category... done?
                    Console.WriteLine("9) Edit Category");
                    Console.WriteLine("10) Display All Categories (Name & Desc.)");
                    Console.WriteLine("11) Display All Categories and their active Product Data");
                    Console.WriteLine("12) Display a Category and its active Product data");
                    //A threshold
                    Console.WriteLine("13) Delete a Product");
                    Console.WriteLine("14) Delete a Category");
                    //use data annotations and handle ALL errors
                    Console.WriteLine("\"q\" to quit");
                    choice = Console.ReadLine();
                    Console.Clear();
                    logger.Info($"Option {choice} selected");
                    if (choice == "1")//display cats
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
                    else if (choice == "2")//add cats
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
                    else if (choice == "3")//display cat+prod
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
                            Console.WriteLine(p.ProductName);
                        }                                                
                    }                                        
                    else if (choice == "4")//display all cat+prod
                    {
                        var db = new NorthwindConsole_32_WHContext();
                        var query = db.Categories.Include("Products").OrderBy(p => p.CategoryId);
                        foreach (var item in query)
                        {
                            Console.WriteLine($"{item.CategoryName}");
                            foreach (Product p in item.Products)
                            {
                                Console.WriteLine($"\t{p.ProductName}");
                            }
                        }
                    }
                    else if (choice == "5"){//add a product
        // public int ProductId { get; set; }
        // public string ProductName { get; set; }
        // public int? SupplierId { get; set; }
        // public int? CategoryId { get; set; }
        // public string QuantityPerUnit { get; set; }
        // public decimal? UnitPrice { get; set; }
        // public short? UnitsInStock { get; set; }
        // public short? UnitsOnOrder { get; set; }
        // public short? ReorderLevel { get; set; }
        // public bool Discontinued { get; set; }
                        //need to find valid Foreign key ids otherise adding will die
                        var db = new NorthwindConsole_32_WHContext();
                        var suppIdsQuery = db.Suppliers;
                        
                        foreach(var s in suppIdsQuery) {
                            Console.WriteLine($"{s.SupplierId}");
                        }
                        Product product = new Product();
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
                        Console.WriteLine("Enter the Discontinued Boolean :");
                        product.Discontinued = bool.Parse(Console.ReadLine());

                        ValidationContext context = new ValidationContext(product, null, null);
                        List<ValidationResult> results = new List<ValidationResult>();

                        var isValid = Validator.TryValidateObject(product, context, results, true);
                        if (isValid)
                        {
                            //var db = new NorthwindConsole_32_WHContext();
                            db = new NorthwindConsole_32_WHContext();
                            // check for unique name
                            if (db.Products.Any(c => c.ProductName == product.ProductName))
                            {
                                // generate validation error
                                isValid = false;
                                results.Add(new ValidationResult("Product Name exists", new string[] { "CategoryName" }));
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
                    else if (choice == "6"){}
                    else if (choice == "7"){}
                    else if (choice == "8"){}
                    else if (choice == "9"){}
                    else if (choice == "10"){}
                    else if (choice == "11"){}
                    else if (choice == "12"){}
                    else if (choice == "13"){}
                    else if (choice == "14"){}
                    Console.WriteLine();
                } while (choice.ToLower() != "q");
            }
            catch (Exception ex)
            {
                logger.Error(ex.Message);
            }
            logger.Info("Program ended");
        }
    }                         
}
