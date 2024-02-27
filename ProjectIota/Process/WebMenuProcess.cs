using Microsoft.Extensions.Caching.Distributed;
using ProjectIota.Data;
using ProjectIota.Models;

namespace ProjectIota.Process
{
	public class WebMenuProcess
	{
		public List<object> GetWebMenuList(DataContext dataContext, HttpContext httpContext, IDistributedCache cache)
		{
			List<object> menuItems = new List<object>();

			if (httpContext.Session.GetString("UserId") != null)
			{
				var cachedData = Task.Run(async () =>
				{
					return await cache.GetAsync<List<object>>("WebMenu_" + httpContext.Session.GetString("UserId").ToString());  
				}).GetAwaiter().GetResult();

				if (cachedData is null)
				{
					var menuData = dataContext.WebMenu.ToList().OrderBy(x => x.Position);

					foreach (var item in menuData)
					{
						if (item.ParentId == null)
						{
							var subItems = menuData.Where(x => x.ParentId == item.WebMenuId).ToList();

							if (subItems.Count == 0)
							{
								menuItems.Add(new
								{
									text = item.Text,
									url = item.Path
								});
							}
							else
							{
								List<object> subItemList = new List<object>();
								foreach (var subItem in subItems)
								{
									subItemList.Add(new { text = subItem.Text, url = subItem.Path });
								}

								menuItems.Add(new
								{
									text = item.Text,
									items = subItemList
								});
							}
						}
					}

					cache.SetAsync("WebMenu_" + httpContext.Session.GetString("UserId").ToString(), menuItems, 1800, 1800).Wait();
				}
				else
				{
					menuItems = cachedData;
				}
			}
			else
			{
				menuItems.Add(new
				{
					text = "Giriş",
					url = "/UserLogin/Index"
				});
			}

			return menuItems;
		}
	}
}
