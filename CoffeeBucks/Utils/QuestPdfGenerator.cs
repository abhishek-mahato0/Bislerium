using System;
using System.Collections.Generic;
using System.Linq;
using QuestPDF.Fluent;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using CoffeeBucks.models;
using CoffeeBucks.Controllers;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System.Data.Common;

namespace CoffeeBucks.Utils
{
	public class QuestPdfGenerator
	{
		public static void GenerateBillPdf(OrderModel order)
		{
			CustomerControllers customerController = new CustomerControllers();
			if (order != null) {

				Document.Create(container =>
				{
					container.Page(page =>
					{
						page.Margin(30);
						page.Header().Padding(4).Element(header =>
						{
							header.Row(row =>
							{
								row.RelativeItem().Column(column =>
								{
									column.Item().AlignCenter().Text("Customer Invoice").FontSize(16);
									column.Item().Text($"Bill #{order.Id}").FontSize(14);

									column.Item().Text(text =>
									{
										text.Span("Customer Name: ").SemiBold();
										text.Span($"{customerController.getCustomerById(order.customerId)}");
									});

									column.Item().Text(text =>
									{
										text.Span("Created Date: ").SemiBold();
										text.Span($"{order.DateCreated}");
									});
								});
							});
							page.Content().Element(content =>
							{
								content.Table(table =>
								{
									table.ColumnsDefinition(columns =>
									{
										columns.ConstantColumn(25);
										columns.RelativeColumn(3);
										columns.RelativeColumn();
										columns.RelativeColumn();
										columns.RelativeColumn();
									});
									table.Header(header =>
									{ 
										header.Cell().Background("#3f81bf").BorderBottom(1).Padding(3).Text("SN").FontColor("#FFFFFF");
										header.Cell().Background("#3f81bf").BorderBottom(1).Padding(3).Text("Product").FontColor("#FFFFFF");
										header.Cell().Background("#3f81bf").BorderBottom(1).Padding(3).Text("Addins").FontColor("#FFFFFF");
										header.Cell().Background("#3f81bf").BorderBottom(1).Padding(3).Text("Quantity").FontColor("#FFFFFF");
										header.Cell().Background("#3f81bf").BorderBottom(1).Padding(3).Text("Total").FontColor("#FFFFFF");
									});

									foreach (var item in order.product.prod)
									{
										var topings = item.topings.Any() ? string.Join(", ", item.topings.Select(p => p.Name)) : "No Topings";

										table.Cell().BorderBottom(1).Padding(2).Text(order.product.prod.IndexOf(item) + 1);
										table.Cell().BorderBottom(1).Padding(2).Text(item.Name);
										table.Cell().BorderBottom(1).Padding(2).Text($"{topings}");
										table.Cell().BorderBottom(1).Padding(2).Text(item.qty);
										table.Cell().BorderBottom(1).Padding(2).Text($"{(item.qty * item.price) + item.topings.Sum(item => item.price * item.qty)}");
									}
									table.Footer(foot =>
									{
										foot.Cell().Row(4).ColumnSpan(4).AlignRight().Text($"Order Type: {order.type}").FontSize(12);
										foot.Cell().Row(5).ColumnSpan(4).AlignRight().Text($"Discount: {order.discount}").FontSize(12);
										foot.Cell().Row(6).ColumnSpan(4).AlignRight().Text($"Grand Total: Rs. {order.net_total}").FontSize(14);

									});

								});
								
							});
							page.Footer().AlignCenter().Text(x =>
										{
											x.CurrentPageNumber();
											x.Span(" / ");
											x.TotalPages();
										});
						});
					});
					}).GeneratePdf(Path.Combine(FilePaths.GetAppDirectoryPath(), "Bill.pdf"));
				}
			}

		public static void GenerateSalesPdf(OrderRevenueModel order)
		{
			CustomerControllers cusController = new CustomerControllers();
			UserController userController = new UserController();
			//var filePath = FilePaths.GetJSONFilePath("");
			if (order != null)
			{

				Document.Create(container =>
				{
					container.Page(page =>
					{
						page.Margin(30);
						page.Header().Padding(4).Element(header =>
						{
							header.Row(row =>
							{
								row.RelativeItem().Column(column =>
								{
									column.Item().AlignCenter().Text("Sales Report").FontSize(16);
									column.Item().Text($"Date: #{order.selectedDate}").FontSize(14);

									column.Item().Text(text =>
									{
										text.Span("Total Orders: ").SemiBold();
										text.Span($"{order.totalorders}");
									});

									column.Item().Text(text =>
									{
										text.Span("Revenue Generated: ").SemiBold();
										text.Span($"{order.revenue}");
									});
								});
							});
							page.Content().Column(column =>
							{
								column.Item().Text("Top 5 most sold Coffee").FontSize(14).SemiBold();
								column.Item().Table(table =>
								{
								
									table.ColumnsDefinition(columns =>
									{
										columns.ConstantColumn(25);
										columns.RelativeColumn(3);
										columns.RelativeColumn();
										columns.RelativeColumn();
								
									});
									table.Header(header =>
									{
										header.Cell().Background("#3f81bf").BorderBottom(1).Padding(3).Text("SN").FontColor("#FFFFFF");
										header.Cell().Background("#3f81bf").BorderBottom(1).Padding(3).Text("Coffee").FontColor("#FFFFFF");
										header.Cell().Background("#3f81bf").BorderBottom(1).Padding(3).Text("Qty").FontColor("#FFFFFF");
										header.Cell().Background("#3f81bf").BorderBottom(1).Padding(3).Text("Revenue").FontColor("#FFFFFF");
									});

									foreach (var item in order.mostOrderedCoffee)
									{

										table.Cell().BorderBottom(1).Padding(2).Text(order.mostOrderedCoffee.IndexOf(item) + 1);
										table.Cell().BorderBottom(1).Padding(2).Text(item.Name);
										table.Cell().BorderBottom(1).Padding(2).Text(item.Quantity);
										table.Cell().BorderBottom(1).Padding(2).Text((item.TotalRevenue));
									}
									

								});
								column.Item().Height(10);
								column.Item().Text("Top 5 most sold Addins").FontSize(14).SemiBold();
								column.Item().Table(table =>
								{
									table.ColumnsDefinition(columns =>
									{
										columns.ConstantColumn(25);
										columns.RelativeColumn(3);
										columns.RelativeColumn();
										columns.RelativeColumn();
									});
									table.Header(header =>
									{
										header.Cell().Background("#3f81bf").BorderBottom(1).Padding(3).Text("SN").FontColor("#FFFFFF");
										header.Cell().Background("#3f81bf").BorderBottom(1).Padding(3).Text("Coffee").FontColor("#FFFFFF");
										header.Cell().Background("#3f81bf").BorderBottom(1).Padding(3).Text("Qty").FontColor("#FFFFFF");
										header.Cell().Background("#3f81bf").BorderBottom(1).Padding(3).Text("Revenue").FontColor("#FFFFFF");
									});

									foreach (var item in order.mostOrderdAddins)
									{

										table.Cell().BorderBottom(1).Padding(2).Text(order.mostOrderdAddins.IndexOf(item) + 1);
										table.Cell().BorderBottom(1).Padding(2).Text(item.Name);
										table.Cell().BorderBottom(1).Padding(2).Text(item.Quantity);
										table.Cell().BorderBottom(1).Padding(2).Text((item.TotalRevenue));
									}


								});
								column.Item().Height(20);
								column.Item().Text("Orders List").FontSize(14).SemiBold();
								column.Item().Table(table =>
								{
									table.ColumnsDefinition(columns =>
									{
										columns.ConstantColumn(25);
										columns.RelativeColumn(2);
										columns.RelativeColumn(3);
										columns.RelativeColumn();
										columns.RelativeColumn();
										columns.RelativeColumn();
										columns.RelativeColumn();

									});
									table.Header(header =>
									{
										header.Cell().Background("#3f81bf").BorderBottom(1).Padding(3).Text("SN").FontColor("#FFFFFF");
										header.Cell().Background("#3f81bf").BorderBottom(1).Padding(3).Text("Customer").FontColor("#FFFFFF");
										header.Cell().Background("#3f81bf").BorderBottom(1).Padding(3).Text("Product and Addins").FontColor("#FFFFFF");
										header.Cell().Background("#3f81bf").BorderBottom(1).Padding(3).Text("Type").FontColor("#FFFFFF");
										header.Cell().Background("#3f81bf").BorderBottom(1).Padding(3).Text("Disc").FontColor("#FFFFFF");
										header.Cell().Background("#3f81bf").BorderBottom(1).Padding(3).Text("N_total").FontColor("#FFFFFF");
										header.Cell().Background("#3f81bf").BorderBottom(1).Padding(3).Text("Cr.By").FontColor("#FFFFFF");
									});

									foreach (var item in order.filtorders)
									{

										var customer = cusController.getCustomerById(item.customerId);
										var user = userController.getUserById(item.orderBy);
										table.Cell().BorderBottom(1).Padding(2).Text(order.filtorders.IndexOf(item) + 1);
										table.Cell().BorderBottom(1).Padding(2).Text(customer);
										if (item.product.prod != null)
										{
											table.Cell().BorderBottom(1).Column(col =>
											{
											foreach (var prod in item.product.prod)
											{
												decimal total = (prod.qty * prod.price) + prod.topings.Sum(item => item.price * item.qty);
												var topings = prod.topings.Any() ? string.Join(", ", prod.topings.Select(p => p.Name)) : "No Topings";
												
													col.Item().Text($"Product: {prod.Name}");
													col.Item().Text($"Addins: {topings}");
													col.Item().Text($"Total: {total}");
													col.Item().Height(10);
												
											}
											});
										}
										table.Cell().BorderBottom(1).Padding(2).Text(item.type);
										table.Cell().BorderBottom(1).Padding(2).Text(item.discount);
										table.Cell().BorderBottom(1).Padding(2).Text(item.net_total);
										table.Cell().BorderBottom(1).Padding(2).Text(user);
									}
								});

							});
							page.Footer().AlignCenter().Text(x =>
							{
								x.CurrentPageNumber();
								x.Span(" / ");
								x.TotalPages();
							});
						});
					});
				}).GeneratePdf(Path.Combine(FilePaths.GetAppDirectoryPath(), "Sales.pdf"));
			}
		}

	}
}
