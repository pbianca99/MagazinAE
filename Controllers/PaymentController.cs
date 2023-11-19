using MagazinAE.Models.VMs;
using Microsoft.AspNetCore.Mvc;
using PayPal.Api;

namespace MagazinAE.Controllers
{
    public class PaymentController : Controller
    {

        private readonly IHttpContextAccessor httpContextAccessor;
        private IConfiguration configuration;
        private Payment payment;
        private readonly MagazinAEContext context;


        public PaymentController(IHttpContextAccessor httpContextAccessor, IConfiguration configuration,MagazinAEContext context)
        {
            this.httpContextAccessor = httpContextAccessor;
            this.configuration = configuration;
            this.context = context;
        }

        public IActionResult Index()
        {
            return View();
        }

        public ActionResult PaymentWithPayPal(string cancel = null, string blogId = "", string payerId = "", string guid = "")
        {
            var clientId = configuration.GetValue<string>("PayPal:Key");
            var clientSecret = configuration.GetValue<string>("PayPal:Secret");
            var mode = configuration.GetValue<string>("PayPal:mode");
            var apiContext = PayPalConfiguration.GetAPIContext(clientId, clientSecret, mode);

            try
            {
                if (string.IsNullOrWhiteSpace(payerId))
                {
                    var baseURI = this.Request.Scheme + "://" + this.Request.Host + "/Payment/PaymentWithPayPal?";
                    var guidd = Convert.ToString((new Random()).Next(100000));
                    guid = guidd;

                    var shoppingList = HttpContext.Session.Get<List<int>>(SessionHelper.ShoppingCart);
                    var productsInCart = context.Products
                        .Where(p => shoppingList.Contains(p.Id))
                        .Select(p => new ProductVM().ProdToProdVM(p))
                        .ToList();
                    var totalAmount = productsInCart.Sum(p => p.Price);

                    var createdPayment = this.CreatePayment(apiContext, baseURI + $"guid={guid}&totalAmount={totalAmount}", blogId, totalAmount);

                    var links = createdPayment.links.GetEnumerator();
                    string? paypalRedirectUrl = null;
                    while (links.MoveNext())
                    {
                        var lnk = links.Current;
                        if (lnk.rel.ToLower().Trim().Equals("approval_url"))
                        {
                            paypalRedirectUrl = lnk.href;
                        }
                    }

                    httpContextAccessor.HttpContext.Session.SetString("payment", createdPayment.id);
                    return Redirect(paypalRedirectUrl);
                }
                else
                {
                    var paymentId = httpContextAccessor.HttpContext.Session.GetString("payment");
                    var executedPayment = ExecutePayment(apiContext, payerId, paymentId);
                    if (executedPayment.state.ToLower() != "approved")
                    {
                        return View("PaymentFailed");
                    }

                    var blogs = executedPayment.transactions[0].item_list.items[0].sku;

                    return View("PaymentSuccess");
                }
            }
            catch (Exception e)
            {
                return View("PaymentFailed");
            }
        }

        private Payment ExecutePayment(APIContext apiContext, string payerId, string paymentId)
        {
            var paymentExecution = new PaymentExecution()
            {
                payer_id = payerId,
            };

            this.payment = new Payment()
            {
                id = paymentId,
            };

            return this.payment.Execute(apiContext, paymentExecution);
        }

        private Payment CreatePayment(APIContext apiContext, string redirectUrl, string blogId, decimal totalAmount)
        {
            var itemList = new ItemList()
            {
                items = new List<Item>()
            };

            itemList.items.Add(new Item()
            {
                name = "Item Detail",
                currency = "USD",
                price = totalAmount.ToString("0.00"),
                quantity = "1",
                sku = "asd"
            });

            var payer = new Payer()
            {
                payment_method = "paypal"
            };

            var redirectUrls = new RedirectUrls()
            {
                cancel_url = redirectUrl + "&Cancel=true",
                return_url = redirectUrl
            };

            var amount = new Amount()
            {
                currency = "USD",
                total = totalAmount.ToString("0.00")  // Use the dynamic totalAmount variable here
            };

            var tramsactionList = new List<Transaction>();

            tramsactionList.Add(new Transaction()
            {
                description = "Transaction description",
                invoice_number = Guid.NewGuid().ToString(),
                amount = amount,
                item_list = itemList
            });

            this.payment = new Payment()
            {
                intent = "sale",
                payer = payer,
                transactions = tramsactionList,
                redirect_urls = redirectUrls
            };

            return this.payment.Create(apiContext);
        }

    }
}
