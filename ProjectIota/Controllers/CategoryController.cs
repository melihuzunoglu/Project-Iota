using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using ProjectIota.Data;
using ProjectIota.Models;
using ProjectIota.PrivateModels;
using ProjectIota.Process;
using System.Drawing;
using System.Windows.Forms;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;

namespace ProjectIota.Controllers
{
	[Route("[controller]")]
	public class CategoryController : Controller
	{
		private DataContext dataContext;
		private readonly IDistributedCache cache;

		public CategoryController(DataContext _dataContext, IDistributedCache _cache)
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

			List<CategoryList> categoryList = dataContext.CategoryList.FromSqlRaw("select * from \"ViewCategoryList\" vcl ").ToList();
			ViewBag.gridData = categoryList;

			if (TempData["DeleteStatus"] != null) { ViewBag.DeleteStatus = TempData["DeleteStatus"].ToString(); }

			return View();
		}

		[Route("Add")]
		public IActionResult Add()
		{
			if (HttpContext.Session.GetString("UserId") == null)
			{
				return RedirectToAction("Index", "UserLogin");
			}

			WebMenuProcess webMenuProcess = new WebMenuProcess();
			ViewBag.menuItems = webMenuProcess.GetWebMenuList(dataContext, HttpContext, cache);

			BindCategoryDropdown();

			if (TempData["SaveStatus"] != null)
			{
				ViewBag.SaveStatus = TempData["SaveStatus"].ToString();
				TempData.Remove("SaveStatus");
			}

			Category category = new Category();
			if (TempData["category"] != null)
			{
				category = JsonConvert.DeserializeObject<Category>(TempData["category"].ToString());
				TempData.Remove("category");
			}

			return View(category);
		}

		[Route("AddCategory")]
		[HttpPost]
		public IActionResult AddCategory(Category category)
		{
			if (HttpContext.Session.GetString("UserId") == null)
			{
				return RedirectToAction("Index", "UserLogin");
			}

			if (category == null)
			{
				TempData["SaveStatus"] = 0;//Payload is null
			}
			else
			{
				if (category.Name == null)
				{
					TempData["SaveStatus"] = 1;//All fields must be filled
				}
				else
				{
					string error = "";
					int writtenEntries = 0;
					try
					{
						dataContext.Add(category);
						writtenEntries = dataContext.SaveChanges();
					}
					catch (Exception e)
					{
						error = e.GetType().ToString();
					}

					if (writtenEntries == 0)
					{
						TempData["SaveStatus"] = 2;//Add operation failed
					}
					else
					{
						TempData["SaveStatus"] = 3;//Add operation successful
					}
				}
			}

			TempData["category"] = JsonConvert.SerializeObject(category);

			return RedirectToAction("Add", "Category");
		}

		[Route("Edit/{value}")]
		public IActionResult Edit(int value)
		{
			if (HttpContext.Session.GetString("UserId") == null)
			{
				return RedirectToAction("Index", "UserLogin");
			}

			WebMenuProcess webMenuProcess = new WebMenuProcess();
			ViewBag.menuItems = webMenuProcess.GetWebMenuList(dataContext, HttpContext, cache);

			BindCategoryDropdown();

			Category category = new Category();
			if (TempData["category"] != null)
			{
				category = JsonConvert.DeserializeObject<Category>(TempData["category"].ToString());
				TempData.Remove("category");
			}
			else
			{
				category = dataContext.Category.Where(x => x.CategoryId == value).First();
			}

			if (TempData["SaveStatus"] != null)
			{
				ViewBag.SaveStatus = TempData["SaveStatus"].ToString();
				TempData.Remove("SaveStatus");
			}

			return View(category);
		}

		[Route("EditCategory")]
		[HttpPost]
		public IActionResult EditCategory(Category category)
		{
			if (HttpContext.Session.GetString("UserId") == null)
			{
				return RedirectToAction("Index", "UserLogin");
			}

			if (category == null)
			{
				TempData["SaveStatus"] = 0;//Payload is null
			}
			else
			{
				if (category.Name == null)
				{
					TempData["SaveStatus"] = 1;//All fields must be filled
				}
				else
				{
					string error = "";
					int writtenEntries = 0;
					try
					{
						dataContext.Update(category);
						writtenEntries = dataContext.SaveChanges();
					}
					catch (Exception e)
					{
						error = e.GetType().ToString();
					}

					if (writtenEntries == 0)
					{
						TempData["SaveStatus"] = 2;//Edit operation failed
					}
					else
					{
						TempData["SaveStatus"] = 3;//Edit operation successful
					}
				}
			}

			TempData["category"] = JsonConvert.SerializeObject(category);

			return RedirectToAction("Edit", "Category", new { value = category.CategoryId.ToString() });
		}

		[Route("DeleteCategory/{value}")]
		[HttpPost]
		public IActionResult DeleteCategory(int value)
		{
			if (HttpContext.Session.GetString("UserId") == null)
			{
				return RedirectToAction("Index", "UserLogin");
			}

			Category category = dataContext.Category.Where(x => x.CategoryId == value).First();
			if (category != null)
			{
				int countOfChildCategories = dataContext.Category.Where(x => x.ParentId == value.ToString()).Count();
				if (countOfChildCategories > 0)
				{
					TempData["DeleteStatus"] = 2;
				}
				else
				{
					dataContext.Remove(category);
					int writtenEntries = dataContext.SaveChanges();
					if (writtenEntries > 0)
					{
						TempData["DeleteStatus"] = 3;
					}
					else
					{
						TempData["DeleteStatus"] = 1;
					}
				}
			}
			else
			{
				TempData["DeleteStatus"] = 1;
			}


			return RedirectToAction("Index", "Category");
		}

		private void BindCategoryDropdown()
		{
			WebMenuProcess webMenuProcess = new WebMenuProcess();
			ViewBag.menuItems = webMenuProcess.GetWebMenuList(dataContext, HttpContext, cache);

			List<Parent> parentList = new List<Parent>();
			List<Category> allCategoryList = dataContext.Category.OrderBy(x => x.CategoryId).ToList();
			foreach (Category parentCategory in allCategoryList.Where(x => x.ParentId is null))
			{
				Parent parent = new Parent();
				parent.nodeId = parentCategory.CategoryId.ToString();
				parent.nodeText = parentCategory.Name;
				parent.expanded = false;
				parent.selected = false;

				List<Child> childList = new List<Child>();
				foreach (Category childCategory in allCategoryList.Where(x => x.ParentId == parentCategory.CategoryId.ToString()))
				{
					Child child = new Child();
					child.nodeId = childCategory.CategoryId.ToString();
					child.nodeText = childCategory.Name;
					child.expanded = false;
					child.selected = false;
					childList.Add(child);

					List<SubChildren> subChildrenList = new List<SubChildren>();
					foreach (var subChildrenCategory in allCategoryList.Where(x => x.ParentId == childCategory.CategoryId.ToString()))
					{
						SubChildren subChildren = new SubChildren();
						subChildren.nodeId = subChildrenCategory.CategoryId.ToString();
						subChildren.nodeText = subChildrenCategory.Name;
						subChildren.expanded = false;
						subChildren.selected = false;
						subChildrenList.Add(subChildren);
					}

					child.child = subChildrenList;
				}

				parent.child = childList;
				parentList.Add(parent);
			}
			ViewBag.CategoryDropdownData = parentList;
		}
	}
}

public class Parent
{
	public string nodeId;
	public string nodeText;
	public string icon;
	public bool expanded;
	public bool selected;
	public List<Child> child;

}
public class Child
{
	public string nodeId;
	public string nodeText;
	public string icon;
	public bool expanded;
	public bool selected;
	public List<SubChildren> child;

}
public class SubChildren
{
	public string nodeId;
	public string nodeText;
	public string icon;
	public bool expanded;
	public bool selected;

}
