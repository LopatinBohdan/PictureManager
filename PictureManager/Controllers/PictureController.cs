using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PictureManager.Entities;
using System.Net;
using System.Text.Json;
using System.Web.Helpers;

namespace PictureManager.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class PictureController : ControllerBase
	{
		private readonly ImageDbContext db;
		object locker = new();
		public PictureController(ImageDbContext db)
		{
			this.db = db;
		}

		[HttpPost("upload-by-url")]
		public string upload(Picture pic)
		{
			WebImage image = null;
			lock (locker)
			{
				image = WebImage.GetImageFromRequest();
				if (image == null)
				{
					return JsonSerializer.Serialize(new { code = 204, status = "no content" });
				}
				if (image.GetBytes().Length > 5 * 1024 * 1024)
				{
					return JsonSerializer.Serialize(new { code = 413, status = "request entity too large" });
				}

				pic.Path = @"images\" + image.FileName.ToString();
				pic.Data = image.GetBytes();

				var path100 = @"images\" + image.FileName.ToString()+"_100";
				image.Resize(width: 60, height: 60, preserveAspectRatio: true,
								preventEnlarge: true);
				image.Save(@"~\" + path100);

				var path300 = @"images\" + image.FileName.ToString()+"_300";
				image.Resize(width: 60, height: 60, preserveAspectRatio: true,
								preventEnlarge: true);
				image.Save(@"~\" + path300);

				db.Pictures.Add(pic);
				try
				{
					db.SaveChanges();
				}
				catch (Exception)
				{
					return JsonSerializer.Serialize(new { code = 504, status = "db is not available" });
				}
				return JsonSerializer.Serialize(new { code = 200, status = "ok", url = pic.Path });
			}
		}

		[HttpGet("get-url/{id}")]
		public string GetPicture(int id)
		{
			lock (locker)
			{
				Picture? temp = db.Pictures.FirstOrDefault(i => i.Id == id);

				if (temp == null)
				{
					return JsonSerializer.Serialize(new { code = 204, status = "no content" });
				}
				return JsonSerializer.Serialize(new { code = 200, status = "ok" });
			}
		}
		[HttpGet("get-url/{id}/size/{size}")]
		public string GetPicture(int id, int size)
		{
			lock (locker)
			{
				Picture? temp=db.Pictures.FirstOrDefault(p => p.Id == id);
				if (temp == null)
				{
					return JsonSerializer.Serialize(new { code = 204, status = "no content" });
				}
				switch (size)
				{
					case 100:
						{
							return JsonSerializer.Serialize(new { code = 200, status = "ok", url=$"{temp.Path}_100" });
						}
						break;

					case 300:
						{
							return JsonSerializer.Serialize(new { code = 200, status = "ok", url = $"{temp.Path}_300" });
						}
						break;
					default:
						{
							return JsonSerializer.Serialize(new { code = 404, status = "not found" });////
						}
						break;
				}
			}

		}


		[HttpDelete("remove/{id}")]
		public string DeletePicture(int id)
		{
			lock (locker)
			{
				Picture? temp = db.Pictures.FirstOrDefault(x => x.Id == id);
				if (temp != null)
				{
					db.Pictures.Remove(temp);
					db.SaveChanges();
					return JsonSerializer.Serialize(new { code = 200, status = "ok" });
				}
				return JsonSerializer.Serialize(new { code = 204, status = "no content" });
			}
		}

	}
}
