using ODataClient.MSProducts.ODataService.Models;
using System;
using System.Data.Services.Client;
using System.Linq;

namespace ODataClient
{
    /// <summary>
    /// This sample client uses WCF DS to communicate with ODataServiceSample
    /// </summary>
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Welcome to the OData Web Api command line client sample.");
            Console.WriteLine("\tType '?' for options.");

            while (true)
            {
                Console.Write("> ");
                string command = Console.ReadLine().ToLower();

                switch (command)
                {
                    case "get products":
                        Get_Products();
                        break;

                    case "get productfamilies":
                        Get_ProductFamilies();
                        break;

                    case "get productfamily.products":
                        Get_ProductFamily_Products();
                        break;

                    case "get productfamily.supplier":
                        Get_ProductFamily_Supplier();
                        break;

                    case "post productfamily":
                        Post_ProductFamily();
                        break;

                    case "delete productfamily":
                        Delete_ProductFamily();
                        break;

                    case "patch productfamily":
                        Patch_ProductFamily();
                        break;

                    case "put productfamily":
                        Put_ProductFamily();
                        break;

                    case "put product..family":
                        Put_Product_link_Family();
                        break;

                    case "delete product..family":
                        Delete_Product_link_Family();
                        break;

                    case "post productfamily..products":
                        Post_ProductFamily_link_Products();
                        break;

                    case "put productfamily..supplier":
                        Put_ProductFamily_link_Supplier();
                        break;

                    case "test":
                        Test();
                        break;

                    case "?":
                    case "h":
                    case "help":
                        PrintOptions();
                        break;

                    case "q":
                    case "quit":
                        return;

                    default:
                        HandleUnknownCommand();
                        break;
                }
                Console.WriteLine("");
            }
        }

        private static void Test()
        {
            Get_Products();
            Get_ProductFamily_Products();
            Get_ProductFamily_Supplier();

            Get_ProductFamilies();
            Delete_ProductFamily();
            Get_ProductFamilies();
            Post_ProductFamily();
            Get_ProductFamilies();
            Patch_ProductFamily();
            Get_ProductFamilies();
            Put_ProductFamily();
            Get_ProductFamilies();

            Put_Product_link_Family();
            Delete_Product_link_Family();

            Post_ProductFamily_link_Products();
            Put_ProductFamily_link_Supplier();
        }

        #region Product
        private static void Get_Products()
        {
            Container ctx = new Container();
            Console.WriteLine("\t<< get products >>");
            foreach (var product in ctx.Products)
                Console.WriteLine("\t{0}-{1}", product.ID, product.Name);
        }

        private static void Put_Product_link_Family()
        {
            Container ctx = new Container();
            Console.WriteLine("\t<< put product..family >>");
            var product = ctx.Products.AsEnumerable().First();
            var family = ctx.ProductFamilies.AsEnumerable().Skip(1).First();
            ctx.SetLink(product, "Family", family);
            ctx.SaveChanges();
        }

        private static void Delete_Product_link_Family()
        {
            Container ctx = new Container();
            Console.WriteLine("\t<< delete product..family >>");
            var product = ctx.Products.AsEnumerable().First();
            ctx.LoadProperty(product, "Family");
            ctx.SetLink(product, "Family", null);
            ctx.SaveChanges();
        }
        #endregion

        #region ProductFamily
        private static void Get_ProductFamilies()
        {
            Container ctx = new Container();
            Console.WriteLine("\t<< get productfamilies >>");
            foreach (var productFamily in ctx.ProductFamilies)
                Console.WriteLine("\t{0}-{1}: {2}", productFamily.ID, productFamily.Name, productFamily.Description);
        }

        private static void Post_ProductFamily()
        {
            Container ctx = new Container();
            Console.WriteLine("\t<< post productfamily >>");
            ProductFamily sql = new ProductFamily
            {
                ID = 4,
                Name = "SQL SERVER",
                Description = "A relational database engine."
            };
            ctx.AddObject("ProductFamilies", sql);
            ctx.SaveChanges();
        }

        private static void Patch_ProductFamily()
        {
            Container ctx = new Container();
            Console.WriteLine("\t<< patch productfamily >>");
            ProductFamily family = ctx.ProductFamilies.Where(pf => pf.ID == 4).AsEnumerable().SingleOrDefault();
            if (family != null)
            {
                family.Description = "Patched Description";
                ctx.UpdateObject(family);

                ctx.SaveChanges(SaveChangesOptions.PatchOnUpdate);
            }
        }

        private static void Put_ProductFamily()
        {
            Container ctx = new Container();
            Console.WriteLine("\t<< put productfamily >>");
            ProductFamily family = ctx.ProductFamilies.Where(pf => pf.ID == 4).FirstOrDefault();
            if (family != null)
            {
                family.Description = "Updated Description";
                ctx.UpdateObject(family);

                ctx.SaveChanges(SaveChangesOptions.ReplaceOnUpdate);
            }
        }

        private static void Delete_ProductFamily()
        {
            Container ctx = new Container();
            Console.WriteLine("\t<< delete productfamily >>");
            ProductFamily family = ctx.ProductFamilies.Where(pf => pf.ID == 4).FirstOrDefault();

            if (family != null)
            {
                ctx.DeleteObject(family);
                ctx.SaveChanges();
            }
        }

        private static void Get_ProductFamily_Supplier()
        {
            Container ctx = new Container();
            Console.WriteLine("\t<< get productfamily.supplier >>");
            var query = ctx.ProductFamilies.Where(p => p.ID == 1).Select(p => p.Supplier);
            foreach (var supplier in query)
                Console.WriteLine("\t{0}-{1}", supplier.ID, supplier.Name);
        }

        private static void Get_ProductFamily_Products()
        {
            Container ctx = new Container();
            Console.WriteLine("\t<< get productfamily.products >>");
            var query = ctx.ProductFamilies.Where(p => p.ID == 3).SelectMany(p => p.Products);
            foreach (var product in query)
                Console.WriteLine("\t{0}-{1}", product.ID, product.Name);
        }

        private static void Post_ProductFamily_link_Products()
        {
            Container ctx = new Container();
            Console.WriteLine("\t<< post productfamily..products >>");
            var product = ctx.Products.OrderBy(p => p.ID).First(); // OrderBy need to avoid Take throw.
            var family = ctx.ProductFamilies.OrderBy(pf => pf.ID).First();
            ctx.AddLink(family, "Products", product);
            ctx.SaveChanges();
        }

        private static void Put_ProductFamily_link_Supplier()
        {
            Container ctx = new Container(new Uri("http://localhost:8085/"));
            Console.WriteLine("\t<< patch productfamily..supplier >>");
            var family = ctx.ProductFamilies.OrderBy(pf => pf.ID).First();
            var supplier = ctx.Suppliers.Where(s => s.ID == 1).First();
            ctx.SetLink(family, "Supplier", supplier);
            ctx.SaveChanges();
        }
        #endregion

        #region Misc
        private static void PrintOptions()
        {
            Console.WriteLine("Available commands:");
            Console.WriteLine("\tget products                   -> Print all the Products.");
            Console.WriteLine("\tget productfamilies            -> Print all the ProductFamilies.");
            Console.WriteLine("\tget productfamily.products     -> Print all the Products in the Office family.");
            Console.WriteLine("\tget productfamily.supplier     -> Print the supplier of the MS-DOS family.");
            Console.WriteLine("\tpost productfamily             -> Create productfamily 4 (SQL SERVER).");
            Console.WriteLine("\tdelete productfamily           -> Delete productfamily 4 (SQL SERVER).");
            Console.WriteLine("\tpatch productfamily            -> Patch ProductFamily 4 (SQL SERVER).");
            Console.WriteLine("\tput productfamily              -> Replace ProductFamily 4 (SQL SERVER).");
            Console.WriteLine("\tput product..family            -> Set Product.Family to a ProductFamily.");
            Console.WriteLine("\tdelete product..family         -> Set Product.Family to NULL.");
            Console.WriteLine("\tpost productfamily..products   -> ProductFamily.Products.Add(product).");
            Console.WriteLine("\tput productfamily..supplier    -> ProductFamily.Supplier = supplier.");
            Console.WriteLine("\ttest                           -> Run all the above commands");
            Console.WriteLine("\t?                              -> Print Available Commands.");
            Console.WriteLine("\tquit                           -> Quit.");
        }

        private static void HandleUnknownCommand()
        {
            Console.WriteLine("command not recognized, enter '?' for options");
        }
        #endregion
    }
}
