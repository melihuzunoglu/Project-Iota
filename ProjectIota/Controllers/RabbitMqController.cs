using Microsoft.AspNetCore.Mvc;
using ProjectIota.Data;
using System.Text;
using ProjectIota.Models;
using RabbitMQ.Client;
using Microsoft.EntityFrameworkCore;
using ProjectIota.Process;
using Syncfusion.EJ2.Notifications;
using Microsoft.Extensions.Caching.Distributed;

namespace ProjectIota.Controllers
{
	[Route("[controller]")]
	public class RabbitMqController : Controller
	{
		private DataContext dataContext;
		private readonly IDistributedCache cache;

		public RabbitMqController(DataContext _dataContext, IDistributedCache _cache)
		{
			dataContext = _dataContext;
			cache = _cache;
		}

		[Route("Index")]
		public IActionResult Index()
		{
			if (HttpContext.Session.GetString("UserId") == null)
			{
				return RedirectToAction("Index", "UserLogin");
			}

			WebMenuProcess webMenuProcess = new WebMenuProcess();
			ViewBag.menuItems = webMenuProcess.GetWebMenuList(dataContext, HttpContext, cache);

			return View();
		}

		[Route("SendMessage")]
		public IActionResult SendMessage()
		{
			var value = dataContext.RabbitMq.FromSqlRaw("Select * From \"RabbitMq\"").AsEnumerable().ToList();

			List<RabbitMq> listTest = value;

			try
			{
				foreach (var item in listTest)
				{
					ConnectionFactory connectionFactory = new();
					connectionFactory.Uri = new Uri(uriString: "amqp://guest:guest@151.145.36.179:5672");
					connectionFactory.ClientProvidedName = "ProjectIota";

					IConnection connection = connectionFactory.CreateConnection();
					IModel channel = connection.CreateModel();

					string exchangeName = "iota-exchange";
					string routingKey = "iota-routing-key";
					string queueName = "iota-queue";

					channel.ExchangeDeclare(exchangeName, ExchangeType.Direct);
					channel.QueueDeclare(queueName, false, false, false, null);
					channel.QueueBind(queueName, exchangeName, routingKey, null);

					byte[] bytes = Encoding.UTF8.GetBytes(item.Id.ToString() + " " + item.Data);
					channel.BasicPublish(exchangeName, routingKey, null, bytes);
					channel.Close();
					connection.Close();

					System.Threading.Thread.Sleep(5000); //This line added for observe the que
				}
				ViewBag.MessageSendStatus = 1;//Message send successful
			}
			catch (Exception)
			{
				ViewBag.MessageSendStatus = 2;//Message send failed
			}

			return View("Index");
		}
	}
}
