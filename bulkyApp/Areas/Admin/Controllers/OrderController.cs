using Bulky.DataAccess.Repository;
using Bulky.DataAccess.Repository.IRepository;
using Bulky.Model.Models;
using Bulky.Model.Models.view_models;
using Bulky.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Stripe;
using Stripe.Checkout;
using Stripe.Climate;
using System.Security.Claims;

namespace BulkyApp.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize]
    public class OrderController : Controller
    {
        private readonly IuintOfWork _unitOfWork;

        // CRITICAL FIX: [BindProperty] makes this entire OrderVM available to all POST actions.
        // It must be public to work correctly.
        [BindProperty]
        public OrderVM OrderVM { get; set; }

        public OrderController(IuintOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Details(int orderId)
        {
            OrderVM = new()
            {
                // CORRECTED: Use PascalCase for all property names
                orderHeader = _unitOfWork.orderHeader.Get(u => u.Id == orderId, includeProperties: "ApplicationUser"),
                orderDetails = _unitOfWork.orderDetail.GetAll(u => u.OrderHeaderID == orderId, includeProperties: "Product")
            };
            return View(OrderVM);
        }

        [HttpPost]
        [Authorize(Roles = SD.Role_Admin + "," + SD.Role_Employee)]
        public IActionResult UpdateOrderDetail()
        {
            var orderHeaderFromDb = _unitOfWork.orderHeader.Get(u => u.Id == OrderVM.orderHeader.Id);

            // Update properties from the bound OrderVM
            orderHeaderFromDb.Name = OrderVM.orderHeader.Name;
            orderHeaderFromDb.PhoneNumber = OrderVM.orderHeader.PhoneNumber;
            orderHeaderFromDb.StreetAddress = OrderVM.orderHeader.StreetAddress;
            orderHeaderFromDb.City = OrderVM.orderHeader.City;
            orderHeaderFromDb.State = OrderVM.orderHeader.State;
            orderHeaderFromDb.PostalCode = OrderVM.orderHeader.PostalCode;


            // CORRECTED: Use two separate 'if' statements to check and update independently.
            if (!string.IsNullOrEmpty(OrderVM.orderHeader.Carrier))
            {
                orderHeaderFromDb.Carrier = OrderVM.orderHeader.Carrier;
            }
            if (!string.IsNullOrEmpty(OrderVM.orderHeader.TrackingNumber))
            {
                orderHeaderFromDb.TrackingNumber = OrderVM.orderHeader.TrackingNumber;
            }

            _unitOfWork.orderHeader.Update(orderHeaderFromDb);
            _unitOfWork.Save();

            TempData["success"] = "Order Details Updated successfully!";
            return RedirectToAction(nameof(Details), new { orderId = orderHeaderFromDb.Id });
        }

        [HttpPost]
        [Authorize(Roles = SD.Role_Admin + "," + SD.Role_Employee)]
        public IActionResult StartProcessing()
        {
            _unitOfWork.orderHeader.UpdateStatus(OrderVM.orderHeader.Id, SD.StatusInProcess);
            _unitOfWork.Save();
            TempData["success"] = "Order Status Updated To In Process!";
            return RedirectToAction(nameof(Details), new { orderId = OrderVM.orderHeader.Id });
        }

        [HttpPost]
        [Authorize(Roles = SD.Role_Admin + "," + SD.Role_Employee)]
        public IActionResult ShipOrder()
        {
            var orderHeader = _unitOfWork.orderHeader.Get(u => u.Id == OrderVM.orderHeader.Id);
            orderHeader.TrackingNumber = OrderVM.orderHeader.TrackingNumber;
            orderHeader.Carrier = OrderVM.orderHeader.Carrier;
            orderHeader.OrderStatus = SD.StatusShipped;
            orderHeader.ShippingDate = DateTime.Now;

            if (orderHeader.PaymentStatus == SD.PaymentStatusDelayedPayment)
            {
                orderHeader.PaymentDueDate = DateOnly.FromDateTime(DateTime.Now.AddDays(30));
            }

            _unitOfWork.orderHeader.Update(orderHeader);
            _unitOfWork.Save();
            TempData["success"] = "Order Shipped Successfully!";
            return RedirectToAction(nameof(Details), new { orderId = OrderVM.orderHeader.Id });
        }

        [HttpPost]
        [Authorize(Roles = SD.Role_Admin + "," + SD.Role_Employee)]
        public IActionResult CancelOrder()
        {
            var orderHeader = _unitOfWork.orderHeader.Get(u => u.Id == OrderVM.orderHeader.Id);

            if (!string.IsNullOrEmpty(orderHeader.PaymentIntentId))
            {
                var refundService = new RefundService();
                var options = new RefundCreateOptions
                {
                    Reason = RefundReasons.RequestedByCustomer,
                    PaymentIntent = orderHeader.PaymentIntentId
                };
                refundService.Create(options);
                _unitOfWork.orderHeader.UpdateStatus(orderHeader.Id, SD.StatusCancelled, SD.StatusRefunded);
            }
            else
            {
                _unitOfWork.orderHeader.UpdateStatus(orderHeader.Id, SD.StatusCancelled, SD.StatusCancelled);
            }

            _unitOfWork.Save();
            TempData["success"] = "Order Canceled successfully!";
            return RedirectToAction(nameof(Details), new { orderId = orderHeader.Id });
        }
        [HttpPost]
        [ActionName("Details")]
        public IActionResult Details_Pay_Now()
        {
            OrderVM.orderHeader = _unitOfWork.orderHeader.Get(u => u.Id == OrderVM.orderHeader.Id, includeProperties: "ApplicationUser");
            OrderVM.orderDetails = _unitOfWork.orderDetail.GetAll(u => u.OrderHeaderID == OrderVM.orderHeader.Id, includeProperties: "Product");
            var domain = Request.Scheme + "://" + Request.Host.Value + "/";
            var options = new SessionCreateOptions
            {
                SuccessUrl = domain + $"admin/order/PaymentConfirmation?id={OrderVM.orderHeader.Id}",
                CancelUrl = domain + $"admin/order/details?orderId={OrderVM.orderHeader.Id}",
                LineItems = new List<SessionLineItemOptions>(),
                Mode = "payment",
            };
            foreach (var item in OrderVM.orderDetails)
            {
                var sessionLineItem = new SessionLineItemOptions
                {
                    PriceData = new SessionLineItemPriceDataOptions
                    {
                        // The price must be provided in the smallest currency unit (e.g., cents for USD).
                        UnitAmount = (long)(item.price * 100),
                        Currency = "usd",
                        ProductData = new SessionLineItemPriceDataProductDataOptions
                        {
                            Name = item.Product.Title
                        }
                    },
                    Quantity = item.count
                };
                options.LineItems.Add(sessionLineItem);
            }

            // Create the Stripe session service and use it to create the session with the prepared options.
            var service = new SessionService();
            Session session = service.Create(options);

            _unitOfWork.orderHeader.UpdateStripePayment(OrderVM.orderHeader.Id, session.Id, session.PaymentIntentId);
            _unitOfWork.Save();
            Response.Headers.Add("Location", session.Url);

            return new StatusCodeResult(303);
        }
        public IActionResult PaymentConfirmation(int id)
        {
            OrderHeader orderHeader = _unitOfWork.orderHeader.Get(u => u.Id == id,includeProperties: "ApplicationUser");

            if (orderHeader.PaymentStatus != SD.PaymentStatusDelayedPayment)
            {
                try
                {
                    var service = new SessionService();
                    Session session = service.Get(orderHeader.SessionId);

                    // --- TEMPORARY DEBUGGING CODE ---
                    // This will display the values from Stripe on your confirmation page.
                    TempData["StripeStatus"] = $"Stripe Payment Status: {session.PaymentStatus.ToLower()}";
                    TempData["PaymentIntentId"] = $"Stripe Payment Intent ID: {session.PaymentIntentId ?? "NULL"}";
                    // --- END OF DEBUGGING CODE ---

                    if (session.PaymentStatus.ToLower() == "paid")
                    {
                        // This is the CRITICAL point. We save the PaymentIntentId here.
                        _unitOfWork.orderHeader.UpdateStripePayment(id, session.Id, session.PaymentIntentId);
                        _unitOfWork.orderHeader.UpdateStatus(id, SD.StatusApproved, SD.PaymentStatusApproved);
                        _unitOfWork.Save();
                    }
                }
                catch (StripeException ex)
                {
                    TempData["error"] = "There was an error confirming your payment with Stripe. Error: " + ex.Message;
                }
            }

            List<ShoppingCart> shoppingCarts = _unitOfWork.ShoppingCart
                .GetAll(u => u.ApplicationUserID == orderHeader.ApplicationUserID).ToList();

            if (shoppingCarts.Any())
            {
                _unitOfWork.ShoppingCart.RemoveRange(shoppingCarts);
                _unitOfWork.Save();
            }

            return View(id);
        }

        #region API CALLS
        [HttpGet]
        public IActionResult GetAll(string status)
        {
            IEnumerable<OrderHeader> orderHeaders;

            if (User.IsInRole(SD.Role_Admin) || User.IsInRole(SD.Role_Employee))
            {
                orderHeaders = _unitOfWork.orderHeader.GetAll(includeProperties: "ApplicationUser").ToList();
            }
            else
            {
                var claimsIdentity = (ClaimsIdentity)User.Identity;
                var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;
                orderHeaders = _unitOfWork.orderHeader.GetAll(u => u.ApplicationUserID == userId, includeProperties: "ApplicationUser");
            }

            switch (status)
            {
                case "pending":
                    orderHeaders = orderHeaders.Where(u => u.PaymentStatus == SD.PaymentStatusDelayedPayment);
                    break;
                case "inprocess":
                    orderHeaders = orderHeaders.Where(u => u.OrderStatus == SD.StatusInProcess);
                    break;
                case "completed":
                    // CORRECTED: Filter by OrderStatus, not PaymentStatus
                    orderHeaders = orderHeaders.Where(u => u.OrderStatus == SD.StatusShipped);
                    break;
                case "approved":
                    orderHeaders = orderHeaders.Where(u => u.OrderStatus == SD.StatusApproved);
                    break;
                default:
                    break;
            }

            return Json(new { data = orderHeaders });
        }
        #endregion
    }
}
