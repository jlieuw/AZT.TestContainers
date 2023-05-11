namespace TestContainers.SpecflowDemo.StepDefinitions;

using System;
using System.Collections.Generic;

using TestContainers.Shared.Models;
using TestContainers.Shared.Services;

[Binding]
internal class OrdersStepDefinition
{
    private readonly OrderService _orderService;
    private IEnumerable<Order>? _orders;

    public OrdersStepDefinition(OrderService orderService) => _orderService = orderService ?? throw new ArgumentNullException(nameof(orderService));

    [Given(@"all available orders in the store")]
    public void GivenAllAvailableOrdersInTheStore() => _orders = _orderService.GetAllOrders();

    [Then(@"there should be no orders placed for a stale account")]
    public void ThenThereShouldBeNoOrdersPlacedForAStaleAccount() => _ = (_orders?.Should().OnlyContain(order => order.User!.LastLogin != null));
}
